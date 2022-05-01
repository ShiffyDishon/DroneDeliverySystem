using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using static BO.Exceptions;
using System.Runtime.CompilerServices;

namespace BL
{
    sealed partial class BL 
    {
        /// <summary>
        /// Statrt the sumulation.
        /// </summary>
        /// <param name="drone">The Drone</param>
        /// <param name="updateDrone">Func to update info in PL</param>
        /// <param name="needToStop">Func to use to stop simulation</param>
        public void StartSimulation(Drone drone, Action< DroneStatusInSim, double> updateDrone, Func<bool> needToStop)
        {
            new Simulation(this , dal , drone, updateDrone,needToStop);
        }
    }
}
