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
        /// Return a list ParcelAtCustomer (BO object) of a specific customer occurding to the parameters.
        /// </summary>
        /// <param name="parcelsOfSpecificCustomer">Parcels that are sent / recieved by the specific customer</param>       
        /// <param name="cId">Customers' id</param>
        /// <param name="cName">Customers' name</param>
        /// <returns></returns>
        private List<ParcelAtCustomer> returnParcelsAtCustomer(IEnumerable<DO.Parcel> parcelsOfSpecificCustomer, int cId, string cName)
        {
            #region Objects declaration
            List<ParcelAtCustomer> customerParcels = new List<ParcelAtCustomer>();
            CustomerInParcel customerInParcel = new CustomerInParcel();
            DO.ParcelStatuses parcelStatus;
            #endregion

            foreach (DO.Parcel parcel in parcelsOfSpecificCustomer)
            {
                #region to erase
                //if (isSenderOrTarget) //sender
                //    customerInParcel.Id = parcel.SenderId;  //bLCustomerInParcel.Name = cName; // dal.getCustomerWithSpecificCondition(c => c.Id == parcel.SenderId).First().Name;
                //else //target
                //    customerInParcel.Id = parcel.TargetId; //bLCustomerInParcel.Name = cName; // dal.getCustomerWithSpecificCondition(c => c.Id == parcel.TargetId).First().Name;
                #endregion

                customerInParcel.Id = cId;
                customerInParcel.Name = cName;
                parcelStatus = findParcelStatus(parcel);
                customerParcels.Add(new ParcelAtCustomer()
                {
                    Id = parcel.Id,
                    Priority = parcel.Priority,
                    Weight = (BO.WeightCategories)parcel.Weight,
                    SenderOrTargetCustomer = customerInParcel,
                    ParcelStatus = parcelStatus
                });
            }
            return customerParcels;
        }

        /// <summary>
        /// Convert DO.Parcel to ParcelToList.
        /// </summary>
        /// <param name="parcel">Parcel To convert</param>
        /// <returns></returns>
        private ParcelToList convertParcelToParcelToList(DO.Parcel parcel)
        {
            String senderName = dal.getCustomerWithSpecificCondition(c => c.Id == parcel.SenderId).First().Name;
            String targetName = dal.getCustomerWithSpecificCondition(c => c.Id == parcel.TargetId).First().Name;
            return new ParcelToList()
            {
                Id = parcel.Id,
                SenderName = senderName,
                TargetName = targetName,
                Weight = (WeightCategories)parcel.Weight,
                Priority = (Priorities)parcel.Priority,
                ParcelStatus =(ParcelStatuses)findParcelStatus(parcel)
            };
        }

        /// <summary>
        /// Convert Parcel to ParcelToList.
        /// </summary>
        /// <param name="parcel">Parcel To convert</param>
        /// <returns></returns>
        private ParcelToList convertParcelToParcelToList(Parcel parcel)
        {
            return new ParcelToList()
            {
                Id = parcel.Id,
                SenderName = parcel.Sender.Name,
                TargetName = parcel.Target.Name,
                Weight = parcel.Weight,
                Priority = (Priorities)parcel.Priority,
                ParcelStatus = parcel.Delivered != null ? ParcelStatuses.Delivered :
                    parcel.PickUp != null ? ParcelStatuses.PickedUp :
                    parcel.Scheduled != null ? ParcelStatuses.Scheduled :
                    ParcelStatuses.Requeasted
            };
        }

        /// <summary>
        /// Praivet Convert Dal To BL Parcel needed to be used by the simulation.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Parcel convertDalToBLParcelSimulation(DO.Parcel p)
        {
            return convertDalToBLParcel(p);
        }

        /// <summary>
        /// Receive a BO parcel and return a converted DO parcel - copy information.
        /// </summary>
        /// <param name="parcel">Parcel to convert</param>
        /// <returns></returns>
        private DO.Parcel convertBLToDalParcel(Parcel parcel)
        {
            DO.Parcel convertedParcel = new DO.Parcel()
            {
                Id = parcel.Id,
                SenderId = parcel.Sender.Id,
                TargetId = parcel.Target.Id,
                Weight = (DO.WeightCategories)parcel.Weight,
                Priority = (DO.Priorities)parcel.Priority,
                Requeasted = parcel.Requeasted,
                Scheduled = parcel.Scheduled,
                PickUp = parcel.PickUp,
                Delivered = parcel.Delivered
            };
          
            if (parcel.Drone != null) convertedParcel.DroneId = parcel.Drone.Id;
            return convertedParcel;
        }

        /// <summary>
        /// Convert DalToBL Parcel
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Parcel convertDalToBLParcel(DO.Parcel p) 
        {
            DroneInParcel drone = null;
            Parcel parcel = new Parcel();
            if (!p.Scheduled.Equals(default(DO.Parcel).Scheduled))
            {
                drone = createDroneInParcel(p, getDroneWithSpecificConditionFromDronesList(d => d.Id == (int)p.DroneId).First().Id);
            }
            return new Parcel()
            {
                Id = p.Id,
                Sender = new CustomerInParcel() { Id = p.SenderId, Name = dal.getCustomerWithSpecificCondition(c => c.Id == p.SenderId).First().Name },
                Target = new CustomerInParcel() { Id = p.TargetId, Name = dal.getCustomerWithSpecificCondition(c => c.Id == p.TargetId).First().Name },
                Weight = (BO.WeightCategories)p.Weight,
                Drone = drone,
                Requeasted = p.Requeasted,
                Scheduled = p.Scheduled,
                PickUp = p.PickUp,
                Delivered = p.Delivered,
                Priority = (Priorities)p.Priority
            };
        }
    }
}