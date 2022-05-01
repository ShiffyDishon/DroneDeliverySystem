using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;




namespace BO
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Position StationPosition { get; set; }
        public int DroneChargeAvailble { get; set; }
        public List<ChargingDrone> DronesCharging { get; set; }
        //public object ChargingDrone { get; private set; }//??????????????????????????
        public override string ToString()
        {
            return $"station name: {Name}, station Id: {Id} , DroneChargeAvailble: {DroneChargeAvailble},\n\t{StationPosition.ToString()}, \tChargingDrone: { string.Join(", ", DronesCharging)}";
        }

    }
}

