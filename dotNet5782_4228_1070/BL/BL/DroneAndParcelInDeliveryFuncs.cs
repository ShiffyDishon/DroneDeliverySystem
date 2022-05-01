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
        /// Find a parcel to delivery for drone.
        /// </summary>
        /// <param name="droneId">The drones' id</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone PairParcelWithDrone(int droneId) //ParcelStatuses.Scheduled
        {
            try
            {
                lock (dronesList)
                {
                    lock (dal)
                    {
                        Drone droneToParcel = getDroneWithSpecificConditionFromDronesList(d => d.Id == droneId).First();
                        DO.Parcel maxParcel = pairParcel(droneToParcel);

                        if (maxParcel.Equals(default(DO.Parcel)))
                            throw new Exceptions.ObjNotAvailableException("No Parcel matching drones' conditions");

                        droneToParcel.Status = DroneStatus.Delivery;
                        droneToParcel.ParcelInTransfer = returnAParcelInTransfer(
                            maxParcel, convertBLToDalCustomer(GetCustomerById(maxParcel.SenderId)),
                            convertBLToDalCustomer(GetCustomerById(maxParcel.TargetId)));

                        updateBLDrone(droneToParcel);
                        maxParcel.DroneId = droneToParcel.Id;
                        maxParcel.Scheduled = DateTime.Now;
                        dal.changeParcelInfo(maxParcel);
                        DroneChangeAction?.Invoke(droneToParcel);
                        ParcelChangeAction?.Invoke(convertDalToBLParcel(maxParcel));
                        return droneToParcel;
                    }
                }
            }
            #region Exceptions
            catch (Exception e)
            {
                throw new ObjNotAvailableException(e.Message);
            }
            #endregion
        }

        /// <summary>
        /// Pair a parcel to drone occurrding to conditions
        /// </summary>
        /// <param name="droneToParcel"></param>
        /// <returns></returns>
        private DO.Parcel pairParcel(Drone droneToParcel)
        {
            DO.Customer senderParcel;
            DO.Customer targetParcel;
            DO.Customer senderMaxParcel;
            double disMaxPToSender = -1;
            IEnumerable<DO.Parcel> parcels = dal.GetParcels();
            DO.Parcel maxParcel = new DO.Parcel();
            foreach (DO.Parcel p in parcels)
            {
                if (p.Scheduled == null) //&& requested!=null .Equals(default(DO.Parcel).Scheduled)
                {
                    #region declare and implement variables
                    senderParcel = dal.getCustomerWithSpecificCondition(c => c.Id == p.SenderId).First();
                    targetParcel = dal.getCustomerWithSpecificCondition(c => c.Id == p.TargetId).First();
                    Position senderPosition = new Position() { Longitude = senderParcel.Longitude, Latitude = senderParcel.Latitude };
                    Position targetPosition = new Position() { Longitude = targetParcel.Longitude, Latitude = targetParcel.Latitude };
                    double disDroneToSenderP = distance(droneToParcel.DronePosition, senderPosition);
                    double disSenderToTarget = distance(senderPosition, targetPosition);
                    double batteryAfterDeliveringByTarget = Math.Round(disDroneToSenderP * electricityUsageWhenDroneIsEmpty + disSenderToTarget * requestElectricity((int)p.Weight), 1);
                    DO.Station stationWithMinDisFromTarget = findAvailbleAndClosestStationForDrone(targetPosition, batteryAfterDeliveringByTarget);
                    double disTargetToStation = distance(targetPosition, new Position() { Longitude = stationWithMinDisFromTarget.Longitude, Latitude = stationWithMinDisFromTarget.Latitude });
                    double electricityUsageWithParcel = requestElectricity((int)p.Weight);
                    double totalBatteryForDeliveryUsage = (double)(disDroneToSenderP * electricityUsageWhenDroneIsEmpty + disSenderToTarget * electricityUsageWithParcel + disTargetToStation * electricityUsageWhenDroneIsEmpty);
                    totalBatteryForDeliveryUsage = Math.Round(totalBatteryForDeliveryUsage, 2);
                    #endregion

                    #region find the most matching parcel
                    if (droneToParcel.Battery - totalBatteryForDeliveryUsage > 0) //[4]
                    {
                        if ((BO.WeightCategories)p.Weight <= droneToParcel.MaxWeight) //BLParcel bLParcel = convertDalToBLParcel(p);
                        {
                            if (maxParcel.Equals(default(DO.Parcel)))
                            {
                                maxParcel = p;
                                senderMaxParcel = senderParcel;
                                disMaxPToSender = disDroneToSenderP;
                            }
                            else
                            {
                                if (maxParcel.Priority < p.Priority)
                                {
                                    maxParcel = p;
                                    senderMaxParcel = senderParcel;
                                    disMaxPToSender = disDroneToSenderP;
                                }
                                else if (maxParcel.Priority == p.Priority /*&& p.Weight <= droneToParcel.MaxWeight && maxParcel.Weight <= droneToParcel.MaxWeight*/)
                                {
                                    if (maxParcel.Weight < p.Weight)
                                        maxParcel = p;
                                    else if (maxParcel.Weight == p.Weight)
                                    {
                                        if (disDroneToSenderP < disMaxPToSender || disMaxPToSender == -1)
                                        {
                                            maxParcel = p;
                                            senderMaxParcel = senderParcel;
                                            disMaxPToSender = disDroneToSenderP;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            return maxParcel;
        }

        /// <summary>
        /// get sender and target of a parcel and create ParcelInTransfer
        /// </summary>
        /// <param name="p">DO.parcel</param>
        /// <param name="sender">sender of parcel</param>
        /// <param name="target">target of parcel</param>
        /// <returns></returns>
        private ParcelInTransfer returnAParcelInTransfer(DO.Parcel p, DO.Customer sender, DO.Customer target)
        {
            Position senderP = new Position() { Latitude = sender.Latitude, Longitude = sender.Longitude };
            Position targetP = new Position() { Latitude = target.Latitude, Longitude = target.Longitude };
            return new ParcelInTransfer()
            {
                TargetPosition = targetP,
                SenderPosition = senderP,
                Id = p.Id,
                SenderCustomer = convertDalToBLCustomerInParcel(sender),
                TargetCustomer = convertDalToBLCustomerInParcel(target),
                isWaiting = p.PickUp == null ? true : false,
                Priority = (Priorities)p.Priority,
                distance = distance(senderP, targetP),
                Weight = (WeightCategories)p.Weight
            };
        }

        /// <summary>
        /// return a DroneInParcel occurding to a parcel and drone id.
        /// </summary>
        /// <param name="p">a parcel in drone</param>
        /// <param name="droneId">drone that has a parcel</param>
        /// <returns></returns>
        private DroneInParcel createDroneInParcel(DO.Parcel p, int droneId)
        {
            Drone d = GetDroneById(droneId);
            return new DroneInParcel() { Id = d.Id, Battery = d.Battery, droneWithParcel = d.DronePosition };
        }
    }
}
