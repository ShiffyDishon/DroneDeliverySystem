using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;


namespace DO
{
    public struct Station
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public int ChargeSlots { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsActive { get; set; }
        public override string ToString()
        {
            return ($"station name: {Name}, station Id: {Id}, station latitude: {Latitude}, station longitude: {Longitude}\n");
        }
    }
}
