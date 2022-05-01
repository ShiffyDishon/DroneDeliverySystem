using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;
using DalApi;
using System.Runtime.CompilerServices;


namespace Dal
{
    public partial class DalObject : DalApi.IDal
    {

        /// <summary>
        /// instance of DalObject and will be equal to DalApi
        /// </summary>
        private static DalObject Instance;

        /// <summary>
        /// Ctor - calls Initialize  = Initialize info of the program
        /// </summary>
        private DalObject()
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// Return Singelton DalObject 
        /// </summary>
        public static DalObject GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new DalObject();
                return Instance;
            }
        }
    }
}
