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
        /// Recieve a DO station and returns a converted BO station - add more infomation
        /// </summary>
        /// <param name="station">Station to convert</param>
        /// <returns></returns>
        public Station convertDalToBLStation(DO.Station station)
        {
            List<ChargingDrone> blDroneChargingByStation = new List<ChargingDrone>();
            IEnumerable<DO.DroneCharge> droneChargesByStation = dal.getDroneChargeWithSpecificCondition(d => d.StationId == station.Id);
            foreach (DO.DroneCharge droneCharge in droneChargesByStation)
            {
                blDroneChargingByStation.Add(new ChargingDrone() { Id = droneCharge.DroneId, Battery = GetDroneById(droneCharge.DroneId).Battery });
            };
            int availableChargingSlots = station.ChargeSlots - blDroneChargingByStation.Count();
            return new Station() { Id = station.Id, Name = station.Name, StationPosition = new BO.Position() { Longitude = station.Longitude, Latitude = station.Latitude }, DroneChargeAvailble = availableChargingSlots, DronesCharging = blDroneChargingByStation };
        }

        /// <summary>
        /// Receive a BO station and return a converted DO station - copy information.
        /// </summary>
        /// <param name="s">Station to convert</param>
        /// <returns></returns>
        private DO.Station convertBLToDalStation(Station s)
        {
            DO.Station station = new DO.Station();
            station.Id = s.Id;
            station.Name = s.Name;
            station.Longitude = s.StationPosition.Longitude;
            station.Latitude = s.StationPosition.Latitude;
            int DronesCharging;
            try 
            {
                DronesCharging = s.DronesCharging.Count();
            }
            catch (Exception)
            {
                DronesCharging = 0;
            }
            station.ChargeSlots = s.DroneChargeAvailble + DronesCharging;
            station.IsActive = true;
            return station;
        }
    }
}
