using System.Collections.Generic;
using System.Linq;
using BO;
using static BO.Exceptions;

namespace BL
{
    sealed partial class BL
    {
        /// <summary>
        /// converte CustomerToList from DO.Customer
        /// </summary>
        /// <param name="customer">customer to convert</param>
        /// <returns></returns>
        private CustomerToList converteCustomerToList(DO.Customer customer)
        {
            int AmountSendingDeliveredParcels = dal.getParcelWithSpecificCondition(p => p.SenderId == customer.Id && p.Delivered != null).Count();
            int AmountSendingUnDeliveredParcels = dal.getParcelWithSpecificCondition(p => p.SenderId == customer.Id && p.Delivered == null).Count();
            int AmountTargetDeliveredParcels = dal.getParcelWithSpecificCondition(p => p.TargetId == customer.Id && p.Delivered != null).Count();
            int AmountTargetUnDeliveredParcels = dal.getParcelWithSpecificCondition(p => p.TargetId == customer.Id && p.Delivered == null).Count();
            return new CustomerToList()
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                AmountSendingDeliveredParcels = AmountSendingDeliveredParcels,
                AmountSendingUnDeliveredParcels = AmountSendingUnDeliveredParcels,
                AmountTargetDeliveredParcels = AmountTargetDeliveredParcels,
                AmountTargetUnDeliveredParcels = AmountTargetUnDeliveredParcels
            };
        }

        /// <summary>
        /// Recieve a DO customer and returns a converted BO customer - add more infomation
        /// </summary>
        /// <param name="customer">Customer to convert</param>
        /// <returns></returns>
        public Customer convertDalToBLCustomer(DO.Customer customer)
        {
            IEnumerable<DO.Parcel> sendingParcels = dal.getParcelWithSpecificCondition(p => p.SenderId == customer.Id);
            IEnumerable<DO.Parcel> targetParcels = dal.getParcelWithSpecificCondition(p => p.TargetId == customer.Id);
            List<ParcelAtCustomer> customerAsSender = returnParcelsAtCustomer(sendingParcels, customer.Id, customer.Name);
            List<ParcelAtCustomer> customerAsTarget = returnParcelsAtCustomer(targetParcels, customer.Id, customer.Name);
            return new Customer() { Id = customer.Id, Name = customer.Name, Phone = customer.Phone, CustomerPosition = new BO.Position() { Longitude = customer.Longitude, Latitude = customer.Latitude }, CustomerAsSender = customerAsSender, CustomerAsTarget = customerAsTarget };
        }

        /// <summary>
        /// Receive a DO customer and return a converted BO customer - copy information.      
        /// </summary>
        /// <param name="customer">Customer to convert</param>
        /// <returns></returns>
        private DO.Customer convertBLToDalCustomer(Customer customer)
        {
            return new DO.Customer()
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Longitude = customer.CustomerPosition.Longitude,
                Latitude = customer.CustomerPosition.Latitude,
                IsActive = true
            };
        }

        /// <summary>
        /// Convert DO.Customer CustomerInParcel
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private CustomerInParcel convertDalToBLCustomerInParcel(DO.Customer customer)
        {
            return new CustomerInParcel()
            {
                Id = customer.Id,
                Name = customer.Name
            };
        }
    }
}
