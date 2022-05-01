using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DO
{
    public struct Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; } //not here
        public bool IsActive { get; set; }
        public override string ToString()
        {
            return ($"drone id: {Id}, drone model{Model}\n");
        }
    }
}

