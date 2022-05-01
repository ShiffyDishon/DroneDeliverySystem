using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using static BO.Exceptions;
using System.Runtime.CompilerServices;

namespace BL
{
    sealed partial class BL
    {
        /// <summary>
        /// Add a new station. checks if this station exist already.
        /// If exist throw an error
        /// If doesn't exist send if it to a func to add
        /// </summary>
        /// <param name="stationToAdd">The new station to add.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station stationToAdd)
        {
            lock (dal)
            {

                DO.Station sToChange = new DO.Station();
                try
                {
                    #region If drone.IsActive == false. What was changed when drone was added-> drone.IsActive == true;
                    try
                    {
                        sToChange = dal.getStationWithSpecificCondition(c => c.Id == stationToAdd.Id).First();
                    }
                    catch (Exception) { }
                    #endregion

                    dal.AddStation(convertBLToDalStation(stationToAdd));
                }
                catch (DO.Exceptions.DataChanged)
                {
                    string message = messageDataChanged(sToChange, stationToAdd);
                    if (message != "")
                        throw new Exceptions.DataChanged(typeof(Station), sToChange.Id, $"Data changed: {message} was changed");
                }
                catch (DO.Exceptions.ObjExistException)
                {
                    throw new ObjExistException(typeof(Customer), stationToAdd.Id);
                }
            }
        }

        /// <summary>
        /// Return dif between the changed and unchanges customer
        /// </summary>
        /// <param name="cToChange"></param>
        /// <param name="cWithChange"></param>
        /// <returns></returns>
        private string messageDataChanged(DO.Station sToChange, Station stationToAdd)
        {
            string message = "";
            if (stationToAdd.StationPosition.Latitude != sToChange.Latitude || stationToAdd.StationPosition.Longitude != sToChange.Longitude)
                message += "Position";
            if (stationToAdd.DroneChargeAvailble != sToChange.ChargeSlots)
                message += ", Amount charge slots";

            return message;
        }

        /// <summary>
        /// Returns a IEnumerable<StationToList> by recieving staions and converting them to BLStationToList
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> GetStationsToList()
        {
            lock (dal)
            {
                IEnumerable<DO.Station> stations = dal.GetStations();
                return (from s in stations
                        select convertStationToStationToList(s));
            }
        }

        /// <summary>
        /// Convert station to stationToList
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        private StationToList convertStationToStationToList(DO.Station station)
        {
            int amountDroneChargeOccupied = dal.getDroneChargeWithSpecificCondition(d => d.StationId == station.Id).Count();
            int amountDroneChargeAvailble = station.ChargeSlots - amountDroneChargeOccupied;
            return new StationToList()
            {
                Id = station.Id,
                Name = station.Name,
                DroneChargeAvailble = amountDroneChargeAvailble,
                DroneChargeOccupied = amountDroneChargeOccupied
            };
        }

        /// <summary>
        /// Return IEnumerable<StationToList> by receiving a converted list of station (one of BO.Station is availableChargeSlots).
        /// </summary>
        /// <param name="amountAvilableSlots"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> GetStationsWithFreeSlots(int amountAvilableSlots = 0)
        {
            lock (dal)
            {
                IEnumerable<StationToList> stationToList = GetStationsToList();
                return (from station in stationToList
                        where station.DroneChargeAvailble >= amountAvilableSlots
                        select station);
            }
        }

        /// <summary>
        /// Return a BO.Station(converted) by an id from dal.getStationWithSpecificCondition
        /// </summary>
        /// <param name="stationrequestedId">The id of the drone that's requested</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStationById(int stationrequestedId)
        {
            lock (dal)
            {
                DO.Station s = dal.getStationWithSpecificCondition(s => s.Id == stationrequestedId).First();
                Station BLstation = convertDalToBLStation(s);
                return BLstation;
            }
        }

        /// <summary>
        /// Change i=info of station
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="ChargeSlots"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void changeStationInfo(int id, string name = null, int ChargeSlots = -1)//-1 is defualt value
        {
            lock (dal)
            {
                DO.Station s = dal.getStationWithSpecificCondition(s => s.Id == id).First();
                if (name != null)
                    s.Name = name;

                if (s.ChargeSlots <= ChargeSlots)
                    s.ChargeSlots = ChargeSlots;

                if (s.ChargeSlots > ChargeSlots)
                {
                    int amountDroneChargesFull = dal.getDroneChargeWithSpecificCondition(station => station.StationId == s.Id).Count();
                    if (amountDroneChargesFull < ChargeSlots)
                        s.ChargeSlots = ChargeSlots;
                    else
                        throw new Exceptions.ObjNotAvailableException($"The amount Charging slots you want to change is smaller than the amount of drones that are charging now in station number {id}");
                }
                try
                {
                    dal.changeStationInfo(s);
                }
                catch (Exception) { throw new Exceptions.ObjNotExistException(typeof(Station), s.Id); }
            }
        }

        /// <summary>
        /// Remove specific station
        /// </summary>
        /// <param name="stationId">remove current station with stationId</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveStation(Station station)
        { 
            if (dal.getDroneChargeWithSpecificCondition(d => d.StationId == station.Id).Count() > 0)
                throw new Exceptions.ObjNotAvailableException(typeof(Station), station.Id, "Could not be remove\nHas drones charging.");

            try
            {
                lock (dal)
                    dal.removeStation(convertBLToDalStation(station));
            }
            catch (DO.Exceptions.ObjNotExistException e1) { throw new Exceptions.ObjNotExistException(typeof(Station), station.Id, e1); }
            catch (Exception) { throw new Exceptions.ObjNotExistException(typeof(Station), station.Id); }

        }
    }
}
