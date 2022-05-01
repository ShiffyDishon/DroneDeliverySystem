using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public struct Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public override string ToString()
        {
            return ($"customer id: {Id}, customer name: {Name}, customer phone: {Phone}, customer latitude: {Latitude}, customer Longitude: {Longitude}\n");
        }
    }

}



