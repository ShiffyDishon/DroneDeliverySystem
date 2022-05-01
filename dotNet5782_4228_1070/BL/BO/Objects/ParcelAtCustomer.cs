using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class ParcelAtCustomer
    {
        public int Id { get; set; }
        public WeightCategories Weight { get; set; }
        public DO.Priorities Priority { get; set; }
        public DO.ParcelStatuses ParcelStatus { get; set; }
        public CustomerInParcel SenderOrTargetCustomer { get; set; }
        public override string ToString()
        {
            return ($"Id: {Id} , Weight: {Weight} , Priority: {Priority}, ParcelStatus: {ParcelStatus},{SenderOrTargetCustomer}\n");
        }
    }

}

