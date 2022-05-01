using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlApi;

namespace BlApi.Ibl
{
    public static class IBLFactory
    {
        public static IBl Factory()
        {
            return BL.BL.GetInstance;
        }
    }
}


namespace BlApi.Isimulation
{
    public static class ISimFactory
    {
        public static ISimulation GetSimulation()
        {
            return BL.BL.GetInstance;
        }
    }
}
