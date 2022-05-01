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
        public Action<Customer> CustomerChangeAction { get; set; }

        /// <summary>
        /// Add a new customer. checks if this customer exist already.
        /// If exist throw an error
        /// If doesn't exist send if it to a func to add
        /// </summary>
        /// <param name="customerToAdd">The new customer to add.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(BO.Customer customerToAdd)
        {
            lock (dal)
            {
                DO.Customer cToChange = new DO.Customer();
                try
                {
                    #region If Customer.IsActive == false. What was changed when drone was added-> Customer.IsActive == true;
                    try
                    {
                        cToChange = dal.getCustomerWithSpecificCondition(c => c.Id == customerToAdd.Id).First();
                    }
                    catch (Exception) { }
                    #endregion

                    dal.AddCustomer(convertBLToDalCustomer(customerToAdd));
                }
                #region Exceptions
                catch (DO.Exceptions.DataChanged)
                {
                    string message = messageDataChanged(cToChange, customerToAdd);
                    if (message != "")
                        throw new Exceptions.DataChanged(typeof(Customer), cToChange.Id, $"Data changed: {message} was changed");
                }
                catch (DO.Exceptions.ObjExistException)
                {
                    throw new ObjExistException(typeof(Customer), customerToAdd.Id);
                }
                #endregion
            }
        }

        /// <summary>
        /// Returns a IEnumerable<CustomerToList> by recieving customers and converting them to CustomerToList
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerToList> GetCustomersToList()
        {
            lock (dal)
            {
                IEnumerable<DO.Customer> customers = dal.GetCustomers();

                return from customer in customers
                       select converteCustomerToList(customer);
            }
        }

        /// <summary>
        /// Return a List<CustomerInParcel>
        /// </summary>
        /// <param name="customerInParcel"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerInParcel> GetCustomersExeptOne(int customerId = 0)
        {
            lock (dal)
            {
                IEnumerable<DO.Customer> customers = dal.GetCustomers();

                return (from c in customers
                        where customerId != c.Id
                        select convertDalToBLCustomerInParcel(c));
            }
        }

        /// <summary>
        /// Return a BO.Customer(converted) by id and name from dal.getCustomerWithSpecificCondition
        /// </summary>
        /// <param name="customerRequestedId">The id of the customer that requested<</param>
        /// <param name="customerRequestedName">The name of the customer that requested<</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomerById(int customerRequestedId, string customerRequestedName = null)
        {
            lock (dal)
            {
                DO.Customer c;
                try
                {
                    if (customerRequestedName != null)
                        c = dal.getCustomerWithSpecificCondition(c => c.Id == customerRequestedId && c.Name == customerRequestedName).First();
                    else
                        c = dal.getCustomerWithSpecificCondition(c => c.Id == customerRequestedId).First();
                }
                catch (Exception)
                {
                    throw new Exceptions.ObjNotExistException(typeof(Customer), customerRequestedId);
                }
                return convertDalToBLCustomer(c);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BO.Customer changeCustomerInfo(int id, string name = null, string phone = null)
        {
            lock (dal)
            {
                try
                {
                    DO.Customer cToUpdate = dal.getCustomerWithSpecificCondition(c => c.Id == id).First();

                    if (name != null)
                        cToUpdate.Name = name;
                    if (phone != null && phone.Length >= 9 && phone.Length <= 10)
                        cToUpdate.Phone = phone;

                    dal.changeCustomerInfo(cToUpdate);
                    Customer customer = convertDalToBLCustomer(cToUpdate);
                    CustomerChangeAction?.Invoke(customer);
                    return (customer);
                }
                catch (Exception e)
                {
                    throw new ObjNotExistException(typeof(DO.Customer), id, e);
                }
            }
        }

        /// <summary>
        /// Remove specific customer
        /// </summary>
        /// <param name="parcelToRemove">remove current customer</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveCustomer(int customerId)
        {
            try
            {
                lock (dal)
                {
                    dal.removeCustomer(dal.getCustomerWithSpecificCondition(c => c.Id == customerId).First());
                }
            }
            #region Exceptions
            catch (DO.Exceptions.ObjNotExistException e1)
            {
                throw new Exceptions.ObjNotExistException(typeof(Customer), customerId, e1);
            }
            catch (Exception e2)
            {
                throw new Exceptions.ObjNotExistException(typeof(Customer), customerId, e2);
            }
            #endregion
        }

        /// <summary>
        /// Return dif between the changed and unchanges customer
        /// </summary>
        /// <param name="cToChange"></param>
        /// <param name="cWithChange"></param>
        /// <returns></returns>
        private string messageDataChanged(DO.Customer cToChange, Customer cWithChange)
        {
            string message = "";
            if (cWithChange.CustomerPosition.Latitude != cToChange.Latitude || cWithChange.CustomerPosition.Longitude != cToChange.Longitude)
                message = "Position";
            if (cWithChange.Name != cToChange.Name)
                message += ", Name";
            if (cWithChange.Phone != cToChange.Phone)
                message += "and Phone";
            return message;
        }
    }
}