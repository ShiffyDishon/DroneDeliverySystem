using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class ParcelInTransfer
    {
        public int Id { get; set; }
        /// <summary>
        /// Awaiting collection =false\ On the way to the destination=true
        /// </summary>
        public bool parcelStatus { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public bool isWaiting { get; set; }     
        public CustomerInParcel SenderCustomer { get; set; }
        public CustomerInParcel TargetCustomer { get; set; }
        public Position SenderPosition { get; set; }
        public Position TargetPosition { get; set; }
        public double distance { get; set; }
        public override string ToString()
        {
            return ($"id: {Id} , parcelStatus: {parcelStatus},Weight: {Weight}, parcelPriority: {Priority},\n\tSenderCustomer:{SenderCustomer}, \tTargetCustomer :{TargetCustomer}, \tSenderPosition: {SenderPosition}, \tTargetPosition: {TargetPosition}, distance: {distance} ");
        }
    }
}

