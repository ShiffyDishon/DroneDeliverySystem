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
        /// Receive a BO drone and return a converted DO drone - copy information.
        /// </summary>
        /// <param name="drone">Drone to convert</param>
        /// <returns></returns>
        private DO.Drone convertBLToDalDrone(Drone drone)
        {
            return new DO.Drone()
            {
                Id = drone.Id,
                Model = drone.Model,
                MaxWeight = (DO.WeightCategories)drone.MaxWeight
            };
        }

        /// <summary>
        /// Return a IEnumerable of DroneToList (BO object) from IEnumerable of BO.Drone.
        /// </summary>
        /// <param name="drones">The drones to convert</param>
        /// <returns></returns>
        private IEnumerable<DroneToList> convertDronesToDronesToList(IEnumerable<Drone> drones)
        {
            List<DroneToList> dronesToList = new List<DroneToList>();
            DroneToList toAdd = new DroneToList();
            foreach (Drone drone in drones)
            {
                if (drone.ParcelInTransfer == (null))
                    toAdd = new DroneToList() { Id = drone.Id, Model = drone.Model, MaxWeight = drone.MaxWeight, droneStatus = drone.Status, Battery = drone.Battery, DronePosition = drone.DronePosition };
                else
                    toAdd = new DroneToList() { Id = drone.Id, Model = drone.Model, MaxWeight = drone.MaxWeight, droneStatus = drone.Status, Battery = drone.Battery, DronePosition = drone.DronePosition, IdParcel = drone.ParcelInTransfer.Id };
                dronesToList.Add(toAdd);
            }
            return dronesToList;
        }
    }
}
