using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using DO;


namespace BO
{
    public class DroneToList
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatus droneStatus { get; set; }
        public Position DronePosition { get; set; }
        public int IdParcel { get; set; } //if there is
        public override string ToString()
        {
            if (DronePosition == null)
                return ($"drone id: {Id}, drone model: {Model}, drone MaxWeight: {MaxWeight}, drone battery: {Battery} , drone status: {droneStatus}");
            if (IdParcel == 0)
                return ($"drone id: {Id}, drone model: {Model}, drone MaxWeight: {MaxWeight},drone battery: {Battery} , drone status: {droneStatus}\n\tDronePosition : {DronePosition} , Parcel Id: -- no parcel -- ");
            return ($"drone id: {Id}, drone model: {Model}, drone MaxWeight: {MaxWeight},drone battery: {Battery} , drone status: {droneStatus}\n\tDronePosition : {DronePosition}");
        }
    }
}

