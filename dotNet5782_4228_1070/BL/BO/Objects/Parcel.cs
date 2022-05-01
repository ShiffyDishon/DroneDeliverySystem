using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;



namespace BO
{
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneInParcel Drone { get; set; }
        public DateTime? Requeasted { get; set; } //prepare a parcel to delivery
        public DateTime? Scheduled { get; set; } //pair a parcel to drone
        public DateTime? PickUp { get; set; }
        public DateTime? Delivered { get; set; }
        public override string ToString()
        {
            //string notFilled = "not filled";
            return ($"parcel ID: {Id}, \n\tSender: {Sender.ToString()}, \tTarget: {Target.ToString()}, \tparcel Priority: {Priority}, parcel weight: {Weight},\n" +
                $"\n\tDrone: {(!Drone.Equals(default(DroneInParcel)) ? Drone.ToString() : (char)'-') } " +
                $"\n\tparcel Requeasted: {(Requeasted != null ? Requeasted : (char)'-')}, parcel scheduled: {(Scheduled != null ? Scheduled : (char)'-')}, parcel pickUp: {(PickUp != null ? PickUp : (char)'-')}, parcel delivered: {(Delivered != null ? Delivered : (char)'-')}\n");
        }
    }
}

