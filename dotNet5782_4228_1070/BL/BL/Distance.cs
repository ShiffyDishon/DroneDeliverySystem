using System;
using System.Collections.Generic;
using System.Text;
using BO;
using System.Linq;
using static BO.Exceptions;

namespace BL
{
    sealed partial class BL 
    {
        /// <summary>
        /// Return Distance between two postions
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double distance(Position p1, Position p2) //not private becuase of simulation
        {
            return Math.Abs(Math.Pow((Math.Pow(p1.Longitude - p2.Longitude, 2) + Math.Pow(p1.Latitude - p2.Latitude, 2)), 0.5));
        }
    }
}
