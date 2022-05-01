using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;
using DO;


namespace BO
{
    public class ChargingDrone
    {
        public int Id { get; set; }
        public double Battery { get; set; }
        public override string ToString()
        {
            return ($"ChargingDrone Id: {Id}, ChargingDrone Battery: {Battery}\n");
        }
    }
}

