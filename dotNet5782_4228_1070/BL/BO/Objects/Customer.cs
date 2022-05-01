using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class Customer
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Position CustomerPosition { get; set; }
        public List<ParcelAtCustomer> CustomerAsSender { get; set; }
        public List<ParcelAtCustomer> CustomerAsTarget { get; set; }
        public override string ToString()
        {
            return ($"customer id: {Id}, customer name: {Name}, customer phone: {Phone}, \n\tCustomerPosition: {CustomerPosition.ToString()}" +
              $"\tCustomerAsSenderAmount:  { CustomerAsSender.Count()}\n\tCustomerAsTargetAmount: {CustomerAsTarget.Count()}\n");
        }
    }
}

