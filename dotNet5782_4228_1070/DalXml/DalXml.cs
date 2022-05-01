using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using DO;
using System.Runtime.CompilerServices;


namespace Dal
{
    public sealed partial class DalXml : DalApi.IDal
    {
        //dir need to be up from bin
        static string dir = @"DalXml\DataSource\";

        /// <summary>
        /// instance of DalXml and will be equal to DalApi
        /// </summary>
        private static DalXml Instance;


        /// <summary>
        /// return one and only one instance of DalXml 
        /// </summary>
        // [MethodImpl(MethodImplOptions.Synchronized)]
        public static DalXml GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new DalXml();
                return Instance;
            }
        }

        string stationFilePath = @"StationsList.xml";
        string droneFilePath = @"DroneList.xml";
        string droneChargeFilePath = @"DroneChargesList.xml";
        string customerFilePath = @"CustomersList.xml";
        string parcelFilePath = @"ParcelsList.xml";
        string workerFilePath = @"WorkersList.xml";
        string configFilePath = @"Config.xml";

        private DalXml()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            DataSource.Initialize(); try
            {

                if (!File.Exists(dir + stationFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.Station>(DataSource.Stations, dir + stationFilePath);

                if (!File.Exists(dir + droneFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.Drone>(DataSource.Drones, dir + droneFilePath);

                if (!File.Exists(dir + droneChargeFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(DataSource.DroneCharges, dir + droneChargeFilePath);
                else
                    XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(new List<DroneCharge>(), dir + droneChargeFilePath);

                if (!File.Exists(dir + customerFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.Customer>(DataSource.Customers, dir + customerFilePath);

                if (!File.Exists(dir + parcelFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.Parcel>(DataSource.Parcels, dir + parcelFilePath);

                if (!File.Exists(dir + workerFilePath))
                    XMLTools.SaveListToXMLSerializer<DO.Worker>(DataSource.Workers, dir + workerFilePath);

                if (!File.Exists(dir + configFilePath))
                {
                    XElement configRoot = XMLTools.LoadData(dir + configFilePath);
                    double[] DroneElectricityUsage = { 0.1, 0.2, 0.4, 0.6, 0.7 };
                    XMLTools.SaveListToXMLSerializer<double>(DroneElectricityUsage, dir + configFilePath);
                }
            }
            catch (Exceptions.FileLoadCreateException e)
            {
                throw new Exceptions.FileLoadCreateException(null, e.Message, e);
            }
        }
    }
}
