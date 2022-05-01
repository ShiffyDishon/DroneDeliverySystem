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
        public Action<Parcel> ParcelChangeAction { get; set; }

        /// <summary>
        /// Add a new parcel. checks if this parcel exist already.
        /// If exist throw an error
        /// If doesn't exist send if it to a func to add
        /// Didn't sent an object because most of the props are initialize in the function and not by the workers/customer(client).
        /// </summary>
        /// <param name="senderId">Parcels' sender(customer) id</param>
        /// <param name="targetId">Parcels' target(customer) id</param>
        /// <param name="weight">Parcels' weight</param>
        /// <param name="priority">Parcels' priority</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(Parcel parcelToAdd)
        {
            if (parcelToAdd.Sender.Id != parcelToAdd.Target.Id)
            {
                lock (dal)
                {
                    DO.Parcel p = new DO.Parcel()
                    {
                        Id = dal.amountParcels() + 1,
                        SenderId = parcelToAdd.Sender.Id,
                        TargetId = parcelToAdd.Target.Id,
                        Weight = (DO.WeightCategories)parcelToAdd.Weight,
                        Priority = (DO.Priorities)parcelToAdd.Priority,
                        Requeasted = DateTime.Now
                    };

                    dal.AddParcel(p);
                    ParcelChangeAction?.Invoke(convertDalToBLParcel(p));
                }
            }
            else
            {
                throw new Exceptions.ObjNotAvailableException("Sender and targe customer are equal.\nplease change target or sender customer.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelToList()
        {
            lock (dal)
            {
                IEnumerable<DO.Parcel> parcelsList = dal.GetParcels();
                return (from parcel in parcelsList
                        select convertParcelToParcelToList(parcel));
            }
        }

        /// <summary>
        /// Receive weight, status and priority and returns List<ParcelToList> accurding to the conditions 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelToList> GetParcelsByConditions(int weight, int status, int priority)
        {
            #region Objects and variables declaration & implementation
            DO.ParcelStatuses parcelStatuses = (DO.ParcelStatuses)status;
            DO.WeightCategories weightCategories = (DO.WeightCategories)weight;
            DO.Priorities parcelPriority = (DO.Priorities)priority;
            #endregion

            lock (dal)
            {
                IEnumerable<DO.Parcel> parcelsList = new List<DO.Parcel>();

                if (weight >= 0 && status >= 0 && priority >= 0)
                    parcelsList = dal.getParcelWithSpecificCondition(p => p.Weight == weightCategories && findParcelStatus(p) == parcelStatuses && p.Priority == parcelPriority);
                else if (weight >= 0 && status >= 0 && priority == -1)
                    parcelsList = dal.getParcelWithSpecificCondition(p => p.Weight == weightCategories && findParcelStatus(p) == parcelStatuses);
                else if (weight >= 0 && status == -1 && priority >= 0)
                    parcelsList = dal.getParcelWithSpecificCondition(p => p.Weight == weightCategories && p.Priority == parcelPriority);
                else if (weight >= 0 && status == -1 && priority == -1)
                    parcelsList = dal.getParcelWithSpecificCondition(p => p.Weight == weightCategories);
                else if (weight == -1 && status >= 0 && priority >= 0)
                    parcelsList = dal.getParcelWithSpecificCondition(p => findParcelStatus(p) == parcelStatuses && p.Priority == parcelPriority);
                else if (weight == -1 && status >= 0 && priority == -1)
                    parcelsList = dal.getParcelWithSpecificCondition(p => findParcelStatus(p) == parcelStatuses);
                else if (weight == -1 && status == -1 && priority >= 0)
                    parcelsList = dal.getParcelWithSpecificCondition(p => p.Priority == parcelPriority);
                else parcelsList = dal.GetParcels();

                return from parcel in parcelsList
                       select convertParcelToParcelToList(convertDalToBLParcel(parcel));
            }
        }


        /// <summary>
        /// Return parcel by Id
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcelByDrone(int droneId)
        {
            try
            {
                lock (dal)
                {
                    DO.Parcel parcel = dal.getParcelWithSpecificCondition(p => p.DroneId == droneId).First();

                    if (parcel.Equals(null))
                        throw new Exceptions.ObjNotExistException(typeof(ParcelInTransfer), -1);//

                    return convertDalToBLParcel(parcel);
                }
            }
            catch (InvalidOperationException e)
            {
                throw new ObjNotExistException(typeof(ParcelInTransfer), -1, e);
            }
        }

        /// <summary>
        /// Return a BO.Parcel(converted) that the func receives it by an id from dal.getParcelWithSpecificCondition
        /// </summary>
        /// <param name="parcelRequestedId">The id of the parcel that's requested</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcelById(int parcelRequestedId)
        {
            lock (dal)
            {
                DO.Parcel p = dal.getParcelWithSpecificCondition(p => p.Id == parcelRequestedId).First();
                Parcel BLparcel = convertDalToBLParcel(p);
                return BLparcel;
            }
        }

        /// <summary>
        /// Update Parcel
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="targetCustomer"></param>
        /// <param name="priority"></param>
        /// <param name="weight"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void updateParcel(int Id, CustomerInParcel targetCustomer, Priorities priority, WeightCategories weight)
        {
            DO.Parcel parcel = dal.getParcelWithSpecificCondition(p=> p.Id == Id).First();
            parcel.TargetId = targetCustomer != default ? targetCustomer.Id : parcel.TargetId;
            parcel.Priority = priority != default ? (DO.Priorities)priority : parcel.Priority;
            parcel.Weight = weight != default ?  (DO.WeightCategories)weight : parcel.Weight;
            dal.changeParcelInfo(parcel);
        }


        /// <summary>
        /// Checks if the parcel to remove exist.
        /// If exist send to remove
        /// else throw an error
        /// </summary>
        /// <param name="parcel">The parcel </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveParcel(int parcelId)
        {
            lock (dal)
            {
                Parcel parcel = GetParcelById(parcelId);
                if (parcel.Drone == null)
                {
                    try
                    {
                        dal.removeParcel(convertBLToDalParcel(parcel));
                        //ParcelChangeAction?.Invoke(parcel);
                    }
                    catch (Exceptions.ObjNotExistException e) { throw new Exceptions.ObjNotExistException(typeof(Parcel), parcel.Id, e); }
                    catch (Exception) { throw new Exceptions.ObjNotExistException(typeof(Parcel), parcel.Id); }
                }
                else throw new Exceptions.ObjNotAvailableException("Can't remove parcel. Parcel asign to drone.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone DronePicksUpParcel(int droneId)// ParcelStatuses.PickedUp          
        {
            try
            {
                lock (dronesList)
                {
                    lock (dal)
                    {
                        Drone drone = getDroneWithSpecificConditionFromDronesList(d => d.Id == droneId && d.Status == DroneStatus.Delivery).First();
                        DO.Parcel parcel = dal.getParcelWithSpecificCondition(p => p.DroneId == droneId).First();
                        if (parcel.PickUp != null) 
                            throw new Exception("The parcel is collected already");
                        
                        if (parcel.Scheduled == null) 
                            throw new Exception("The parcel is not schedueld.");
                        
                        DO.Customer senderP;
                        try
                        {
                            senderP = dal.getCustomerWithSpecificCondition(customer => customer.Id == parcel.SenderId).First();
                        }
                        catch (ObjNotExistException)
                        {
                            throw new Exception("Drone wasn't abale to pick up the parcel");
                        }
                        Position senderPosition = new Position() { Longitude = senderP.Longitude, Latitude = senderP.Latitude };
                        double disDroneToSenderP = distance(drone.DronePosition, senderPosition);
                        if (disDroneToSenderP > drone.Battery)///not sure if we need it here. it was supposed to be checked in the pair a parcel with drone.
                            throw new ObjNotAvailableException("The battery usage will be bigger than the drones' battery ");
                        drone.Battery -= Math.Round(disDroneToSenderP * requestElectricity((int)parcel.Weight), 1);
                        drone.DronePosition = senderPosition;

                        updateBLDrone(drone);
                        parcel.PickUp = DateTime.Now;
                        dal.changeParcelInfo(parcel);
                        drone.ParcelInTransfer.isWaiting = false;
                        ParcelChangeAction?.Invoke(convertDalToBLParcel(parcel));
                        DroneChangeAction?.Invoke(drone);
                        return drone;
                    }
                }
            }

            catch (Exception e)
            {
                throw new ObjNotExistException(e.Message);
            }
        }

        /// <summary>
        /// Change parcel info.
        /// </summary>
        /// <param name="parcel"></param>
        public void changeParcelInfo(Parcel parcel)
        {
            dal.changeParcelInfo(convertBLToDalParcel(parcel));
            ParcelChangeAction?.Invoke(parcel);
        }


        /// <summary>
        /// Deliver parcel
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone DeliveryParcelByDrone(int droneId) 
        {
            try
            {
                lock (dronesList)
                {
                    lock (dal)
                    {
                        Drone bLDroneToSuplly = getDroneWithSpecificConditionFromDronesList(d => d.Id == droneId).First();
                        DO.Parcel parcelToDelivery = dal.getParcelWithSpecificCondition(p => p.DroneId == droneId).First();
                        if (parcelToDelivery.PickUp.Equals(default(DO.Parcel).PickUp) && !parcelToDelivery.Delivered.Equals(default(DO.Parcel).Delivered))
                        {
                            throw new Exception("Drone cann't deliver this parcel.");
                        }
                        DO.Customer senderP;
                        DO.Customer targetP;
                        senderP = dal.getCustomerWithSpecificCondition(c => c.Id == parcelToDelivery.SenderId).First();
                        targetP = dal.getCustomerWithSpecificCondition(c => c.Id == parcelToDelivery.TargetId).First();
                        Position senderPosition = new Position() { Longitude = senderP.Longitude, Latitude = senderP.Latitude };
                        Position targetPosition = new Position() { Longitude = senderP.Longitude, Latitude = senderP.Latitude };
                        double disSenderToTarget = distance(senderPosition, targetPosition);
                        double electricity = requestElectricity((int)parcelToDelivery.Weight);
                        bLDroneToSuplly.Battery -= Math.Round(electricity * disSenderToTarget, 1);
                        bLDroneToSuplly.DronePosition = targetPosition;
                        bLDroneToSuplly.Status = DroneStatus.Available;
                        updateBLDrone(bLDroneToSuplly);
                        parcelToDelivery.Delivered = DateTime.Now;
                        dal.changeParcelInfo(parcelToDelivery);
                        ParcelChangeAction?.Invoke(convertDalToBLParcel(parcelToDelivery));
                        DroneChangeAction?.Invoke(bLDroneToSuplly);
                        return bLDroneToSuplly;
                    }
                }
            }
            catch (ObjNotExistException)
            {
                throw new Exception("Can't deliver parcelby drone.");
            }
            catch (Exception)
            {
                throw new Exception("Can't deliver parcelby drone.");
            }
        }

        /// <summary>
        /// Find parcels' status: { Available, AsignedParcel, PickedParcel , DeliveredParcel}
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public DO.ParcelStatuses findParcelStatus(DO.Parcel p)
        {
            return (DO.ParcelStatuses)(p.Delivered != null ? ParcelStatuses.Delivered :
               p.PickUp != null ? ParcelStatuses.PickedUp :
               p.Scheduled != null ? ParcelStatuses.Scheduled :
               ParcelStatuses.Requeasted);
        }
    }
}
