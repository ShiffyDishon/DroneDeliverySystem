using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using DO;


namespace BO
{
    public class Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatus Status { get; set; } //DroneStatus ?? the same name
        public ParcelInTransfer ParcelInTransfer { get; set; }
        public Position DronePosition { get; set; }
        public DateTime? SartToCharge { set; get; }
        public override string ToString()
        {
            if (DronePosition == null)
                return ($"drone id: {Id}, drone model: {Model}, drone MaxWeight: {MaxWeight}, drone battery: {Battery} , drone status: {Status}");
            return ($"drone id: {Id}, drone model: {Model}, drone MaxWeight: {MaxWeight} ,drone battery: {Battery} , drone status: {Status} \n\tDronePosition : {DronePosition}");
        }
    }
}

