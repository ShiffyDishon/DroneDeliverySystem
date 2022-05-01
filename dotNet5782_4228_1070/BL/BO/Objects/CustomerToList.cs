using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class CustomerToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int AmountSendingDeliveredParcels { get; set; }
        public int AmountSendingUnDeliveredParcels { get; set; }
        public int AmountTargetDeliveredParcels { get; set; }
        public int AmountTargetUnDeliveredParcels { get; set; }
    }
 
}

