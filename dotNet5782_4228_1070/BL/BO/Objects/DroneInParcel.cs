using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using DO;


namespace BO
{
    public class DroneInParcel //drone in pacel
    {
        public int Id { get; set; }
        public double Battery { get; set; }
        public Position droneWithParcel { get; set; }
        public override string ToString()
        {
            return ($"id: {Id} , battery: {Battery},\n\tdrone position: {droneWithParcel} \n");
        }
    }
}

