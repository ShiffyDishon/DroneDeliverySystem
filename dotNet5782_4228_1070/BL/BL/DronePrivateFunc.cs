using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using static BO.Exceptions;

namespace BL
{
    sealed partial class BL
    {
        

        /// <summary>
        /// Return a drone from droneList from BL
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private IEnumerable<Drone> getDroneWithSpecificConditionFromDronesList(Predicate<Drone> predicate)
        {
            IEnumerable<Drone> drones = (from drone in dronesList
                                         where predicate(drone)
                                         select drone);
            return from d in drones
                   select d.Clone<Drone>();
        }

        /// <summary>
        /// Update drones detailes.
        /// </summary>
        /// <param name="droneWithUpdateInfo"></param>
        private void updateBLDrone(Drone droneWithUpdateInfo)
        {
            try
            {
                lock (dronesList)
                {
                    int index = dronesList.FindIndex(d => d.Id == droneWithUpdateInfo.Id);
                    dronesList[index] = droneWithUpdateInfo;
                }
            }
            #region Exceptions
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
            #endregion
        }


        /// <summary>
        /// Find closest station (will be called when the charging slots are full)
        /// </summary>
        /// <param name="dronePostion"></param>
        /// <returns></returns>
        private DO.Station findClosestStation(Position dronePostion)
        {
            double minDis = 0;
            DO.Station station = new DO.Station();
            IEnumerable<DO.Station> stations = dal.GetStations();
            foreach (var s in stations)
            {
                if (minDis == 0) {
                    station = s;
                    double dis = distance(dronePostion, new Position() { Latitude = s.Latitude, Longitude = s.Longitude });
                }
                else
                {
                    double dis = distance(dronePostion, new Position() { Latitude = s.Latitude, Longitude = s.Longitude });
                    station = minDis > dis ? s : station;
                    minDis = minDis > dis ? dis : minDis;
                }
            }
            return station;
        }

        /// <summary>
        /// Release a drone from charging slots.
        /// </summary>
        /// <param name="s"></param>
        private void releaseADroneFromCharging(DO.Station s)
        {
            Station station = convertDalToBLStation(s);
            double maxBattery = station.DronesCharging.Max(s => s.Battery);
            int droneId = station.DronesCharging.Find(d => d.Battery == maxBattery).Id;
            FreeDroneFromCharging(droneId);
        }

        /// <summary>
        /// When charging slots are full and drone needs to charge
        /// and could fly only to a close station(was check before delivery)
        /// Find closest station and send to releas.
        /// </summary>
        /// <param name="d"></param>
        private DO.Station arrangeAnEmptyChargingSlot(Drone d)
        {
            DO.Station stationWithFullChargingSlots = findClosestStation(d.DronePosition);
            releaseADroneFromCharging(stationWithFullChargingSlots);
            return stationWithFullChargingSlots;
        }
    }
}