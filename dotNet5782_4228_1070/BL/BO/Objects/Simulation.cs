using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using DO;
using System.Threading;
using BO;
using static BL.BL;
using static BO.Exceptions;
using BlApi.Ibl;
using DalApi;


namespace BO
{
    class Simulation
    {
        /// <summary>
        /// Instance of Ibl interface.
        /// </summary>
        IBl Ibl;

        /// <summary>
        /// Instance of Idal interface.
        /// </summary>
        IDal Idal;

        /// <summary>
        /// Instance of Isimulation interface.
        /// </summary>
        ISimulation Isimulation;

        /// <summary>
        /// Drones' parcel when drone.Status = DroneStatus.Delivery.
        /// </summary>
        Parcel parcel;

        /// <summary>
        /// Drone parcels' sender customer when drone.Status = DroneStatus.Delivery.
        /// </summary>
        Customer sender;

        /// <summary>
        /// Drone parcels' target customer when drone.Status = DroneStatus.Delivery.
        /// </summary>
        Customer target;

        /// <summary>
        /// <param name="drone">The Drone</param>
        /// </summary>
        Drone drone;

        /// <summary>
        /// Time to stop
        /// </summary>
        const int DELAY = 1000;

        /// <summary>
        /// Ctor Simulation
        /// </summary>
        /// <param name="BL">Interface Ibl</param>
        public Simulation(IBl BL, IDal idal , Drone drone, Action< DroneStatusInSim, double> updateDrone, Func<bool> needToStop)
        {
            this.Ibl = BL;
            this.Idal = idal;
            this.Isimulation = BlApi.Isimulation.ISimFactory.GetSimulation();
            distace = 0;
            StartSim(drone, updateDrone, needToStop);
        }

        private double distace { get; set; }

        /// <summary>
        /// Start simulation run occurding to drone status.
        /// DroneStatusAvailable() => PairParcelWithDrone.
        /// DroneStatusMaintenance() => Charging drone && freeing it.
        /// DroneStatusDelivery() =>
        ///     case 1: Parcel wasn't pick up yet. 
        ///     case d: Parcel wasn't deliverd yet.
        ///     case 1: Parcel is delivered and drone destination is a station.
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        /// <param name="needToStop">Func to use to stop simulation</param>
        public void StartSim(Drone drone, Action< DroneStatusInSim, double> updateDrone, Func<bool> needToStop)
        {
            this.drone = drone;
            while (!needToStop())
            {
                switch (drone.Status)
                {
                    case BO.DroneStatus.Available:
                        DroneStatusAvailable(updateDrone, drone);
                        break;

                    case BO.DroneStatus.Maintenance:
                        DroneStatusMaintenance(updateDrone, drone);
                        freeDroneFromCharge(updateDrone, drone);
                        break;

                    case BO.DroneStatus.Delivery:
                        DroneStatusDelivery(updateDrone, drone);
                        break;
                    default:
                        break;
                }
                Thread.Sleep(DELAY);
            }
        }

        /// <summary>
        /// Simulation when drone.Status == DroneStatus.Available
        /// Try to PairParcelWithDrone;
        /// If not try to send to charge
        /// If Not enough battery to fly to station: drone will be a parcel and another drone will pick him up.  
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        private void DroneStatusAvailable(Action< DroneStatusInSim, double> updateDrone, Drone drone)
        {
            try
            {
                BO.Drone droneWithParcel = Ibl.PairParcelWithDrone(drone.Id);
                drone.Status = BO.DroneStatus.Delivery;
                drone.ParcelInTransfer = droneWithParcel.ParcelInTransfer;
                updateDrone(DroneStatusInSim.HideTextBlock, distace); 
                Ibl.changeDroneInfoInDroneList(drone);
            }
            #region Exceptions
            catch (ObjNotAvailableException)
            {
                try //not enough battery for delivery
                {
                    if (drone.Battery != 100)
                    {
                        sendDroneToCharge(updateDrone, drone);
                        DroneStatusMaintenance(updateDrone, drone);
                    }
                    else
                        Thread.Sleep(DELAY*5);
                }
                catch (ObjNotExistException) //No charging slots available
                {
                    Thread.Sleep(DELAY*5);
                }
                catch (ObjNotAvailableException)
                {
                    #region Exceptions
                    //Not supposed to happen - the program finds a close station before delivery. :
                    //Not enough battery to fly to a far station - place is full by closest station.
                    //could wait for the charge slot in the close station to be available again.
                    //The program will releas a charging slot.
                    #endregion
                }

            }
            catch (Exception)
            {
                Thread.Sleep(DELAY*2);
            }
            #endregion
        }


        /// <summary>
        /// Simulation when drone.Status == DroneStatus.Maintenance
        /// Charging drone and than drone.Status == DroneStatus.Available
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        private void DroneStatusMaintenance(Action<DroneStatusInSim, double> updateDrone, Drone drone)
        {
            TimeSpan second;
            double baterryToAdd;
            double batteryPerTime = Ibl.requestElectricity(0);
           
            while (drone.Battery < 100)
            {
                second = (TimeSpan)(DateTime.Now - drone.SartToCharge) * 1000;
                drone.SartToCharge = DateTime.Now;
                baterryToAdd = second.TotalMinutes * batteryPerTime;
                drone.Battery += baterryToAdd;
                drone.Battery = Math.Min(100, drone.Battery);
                drone.Battery = Math.Max(0, drone.Battery);
                drone.Battery = Math.Round(drone.Battery, 1);
                updateDrone( 0, 0);
                Ibl.changeDroneInfoInDroneList(drone);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// freeing Drone From Charge.
        /// Changing drone status from DroneStatus.Maintenance to drone.Status. = DroneStatus.Available;
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        private void freeDroneFromCharge(Action<DroneStatusInSim, double> updateDrone, Drone drone)
        {
            bool succeedFreeDroneFromCharge = false;
            do
            {
                try
                {
                    Isimulation.removeDroneChargeByDroneId(drone.Id);
                    drone.Status = BO.DroneStatus.Available;
                    Ibl.changeDroneInfoInDroneList(drone);
                    updateDrone( 0, distace);
                    succeedFreeDroneFromCharge = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(DELAY);
                }
            }
            while (!succeedFreeDroneFromCharge);
        }

        /// <summary>
        /// Simulation when drone.Status == DroneStatus.Delivery
        /// case 1: Parcel wasn't pick up yet.
        /// case d: Parcel wasn't deliverd yet .
        /// case 1: Parcel is delivered and drone destination is a station with an available charging slot.
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        private void DroneStatusDelivery(Action<DroneStatusInSim, double> updateDrone, Drone drone)
        {
            DeliveryStatusAction droneStatus = Ibl.returnDroneStatusInDelivery(drone);
            switch (droneStatus)
            {
                #region parcel was asigned
                case DeliveryStatusAction.AsignedParcel:
                    {
                        initializeObjectsWhenDroneInDelivery(drone);
                        updateDrone(DroneStatusInSim.ToPickUp, distace);
                        Thread.Sleep(DELAY);
                        drone = calcDisAndSimulateDlivery(updateDrone, drone, sender.CustomerPosition, Ibl.requestElectricity(0));
                        updateDrone(DroneStatusInSim.PickUp, distace);
                        parcel.PickUp = DateTime.Now;
                        Isimulation.changeParcelInfo(parcel);
                        break;
                    }
                #endregion
                #region Parcel was pick up but not delivered.
                case DeliveryStatusAction.PickedParcel: 
                    {
                        initializeObjectsWhenDroneInDelivery(drone);
                        updateDrone( DroneStatusInSim.ToDelivery, distace);
                        Thread.Sleep(DELAY);
                        drone = calcDisAndSimulateDlivery(updateDrone, drone, target.CustomerPosition, Ibl.requestElectricity((int)parcel.Weight));
                        updateDrone( DroneStatusInSim.Delivery, distace);
                        Thread.Sleep(DELAY);
                        drone.ParcelInTransfer = null;
                        parcel.Delivered = DateTime.Now;
                        updateDrone( DroneStatusInSim.HideTextBlock, distace);
                        drone.Status = BO.DroneStatus.Available;
                        Ibl.changeDroneInfoInDroneList(drone);
                        Isimulation.changeParcelInfo(parcel);
                        Thread.Sleep(DELAY);
                        parcel = null;
                        sender = null;
                        target = null;
                        break;
                    }
                #endregion
            }
            Thread.Sleep(DELAY);

        }

        /// <summary>
        /// case 1: Parcel is delivered and drone destination is a station with an available charging slot.
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        private void sendDroneToCharge(Action< DroneStatusInSim, double> updateDrone, Drone drone)
        {
            DO.Station s = Ibl.findAvailbleAndClosestStationForDrone(drone.DronePosition, drone.Battery);
            Idal.AddDroneToCharge(new DO.DroneCharge() { DroneId = drone.Id, StationId = s.Id });
            drone.Status = BO.DroneStatus.Maintenance;
            Ibl.changeDroneInfoInDroneList(drone);
            updateDrone( DroneStatusInSim.ToCharge, distace);
            Position stationPos = new Position() { Latitude = s.Latitude, Longitude = s.Longitude };
            drone = calcDisAndSimulateDlivery(updateDrone, drone, stationPos, Ibl.requestElectricity(0));
            drone.SartToCharge = DateTime.Now;
            Ibl.changeDroneInfoInDroneList(drone);
            updateDrone( DroneStatusInSim.HideTextBlock, distace);
            DroneStatusMaintenance(updateDrone, drone);
            freeDroneFromCharge(updateDrone, drone);
        }

        /// <summary>
        /// initialize parcel , parcels' sender and parcels' target when drone in delivery;
        /// </summary>
        private void initializeObjectsWhenDroneInDelivery(Drone drone)
        {
            parcel = Isimulation.convertDalToBLParcelSimulation(Idal.getParcelWithSpecificCondition(p => p.Id == drone.ParcelInTransfer.Id).First());
            sender = Isimulation.convertDalToBLCustomer(Idal.getCustomerWithSpecificCondition(c => parcel.Sender.Id == c.Id).First());
            target = Isimulation.convertDalToBLCustomer(Idal.getCustomerWithSpecificCondition(c => parcel.Target.Id == c.Id).First());
        }

        /// <summary>
        /// Calc Distance for each movment. Send to dispaly.
        /// </summary>
        /// <param name="updateDrone"></param>
        /// <param name="drone"></param>
        /// <param name="destination"></param>
        /// <param name="batteryUsage"></param>
        /// <returns></returns>
        private Drone calcDisAndSimulateDlivery(Action< DroneStatusInSim, double> updateDrone, Drone drone, Position destination, double batteryUsage)
        {
            #region declare and implement variables
            double distanceDroneToSender = distance(drone.DronePosition, destination);
            double batteryUsageByWeight = batteryUsage;
            double changeXInDis; //drone x > costomer x // go back by x
            double changeYInDis; //drone y > costomer y // go back by x
            double x = drone.DronePosition.Latitude;
            double y = drone.DronePosition.Longitude;
            double dis;
            double batteryUsageByWeightForM;
            double fullDis = distance(drone.DronePosition, destination);
            double sumBattery = fullDis * batteryUsage;
            #endregion

            #region Latitude is equal between drone and currentTarget
            if (drone.DronePosition.Latitude == destination.Latitude)
            {
                changeXInDis = 0;
                changeYInDis = drone.DronePosition.Longitude > destination.Longitude ? -1 : 1; 
                dis = changeYInDis; 
            }
            #endregion

            #region Longitude is equal between drone and currentTarget
            else if (drone.DronePosition.Longitude == destination.Longitude)
            {
                changeXInDis = drone.DronePosition.Latitude > destination.Latitude ? -1 : 1; 
                changeYInDis = 0;
                dis = changeXInDis; 
            }
            #endregion

            #region Position is dirrerent between drone and currentTarget
            else  //x = Latitude, y = Longitude.
            {
                double disInYPerUnit = Math.Abs(destination.Longitude - drone.DronePosition.Longitude);
                double disInXPerUnit = Math.Abs(destination.Latitude - drone.DronePosition.Latitude);
                changeXInDis = drone.DronePosition.Latitude > destination.Latitude ? -1 : 1; 
                changeYInDis = disInYPerUnit / disInXPerUnit;
                changeYInDis = drone.DronePosition.Longitude > destination.Longitude ? (-1 * changeYInDis) : changeYInDis;
                dis = Math.Pow(Math.Pow(disInXPerUnit, 2) + Math.Pow(disInYPerUnit, 2), 0.5);
                dis = distance(drone.DronePosition, new Position() { Latitude = x + changeXInDis, Longitude = y + changeYInDis }); 
            }
            #endregion

            Position currentPos;
            batteryUsageByWeightForM = (batteryUsageByWeight * dis);

            #region calc dis and display simulation
            while (sumBattery > 0 && fullDis > 0)
            {
                x += changeXInDis;
                y += changeYInDis;

                currentPos = new Position() { Latitude = Math.Round(x, 2), Longitude = Math.Round(y, 2) };
                #region changes of drone to update by display
                drone.DronePosition.Latitude = Math.Round(x, 2);
                drone.DronePosition.Longitude = Math.Round(y, 2);
                drone.DronePosition = currentPos;
                drone.Battery -= batteryUsageByWeightForM;
                drone.Battery = Math.Round(drone.Battery, 1);
                Ibl.changeDroneInfoInDroneList(drone);
                sumBattery -= batteryUsageByWeightForM;
                fullDis -= dis;
                updateDrone( DroneStatusInSim.DisFromDestination, fullDis);
                #endregion
                Thread.Sleep((int)distanceDroneToSender*2);
            }
            #endregion

            drone.DronePosition = destination;
            updateDrone(DroneStatusInSim.HideTextBlock, distace);
            return drone;
        }
    }
}
