//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DO;



//namespace DAL
//{
//    enum Choices { Add = 1, Update, ShowWithId, ShowList, exit }
//    enum objects { Station = 1, Drone, Customer, Parcel, FreeParcel, EmptyCharges }
//    enum UpdateObj { DroneReceivesParcel = 1, DroneCollectsAParcel, CostumerGetsParcel, sendDroneToCharge, freeDroneFromCharge }

//    class main
//    {
//        Random r = new Random();

//        /// <summary>
//        /// Instance of Idal interface.
//        /// </summary>
//        static DalApi.Idal dal ;

//        //public static void initializ() => dal = DalApi.DalFactory.factory();

//        /// <summary>
//        /// The user chooses what to do with the objects:
//        /// 1.Add 2.update 3.display obj by id 4.display arr of obj 5.exit.
//        /// </summary>
//        /// <param name="args"></param>
//        static void Main(string[] args)
//        {
//            dal = DalApi.DalFactory.factory();
//            Choices choice; //defaualt 0
//            do
//            {
//                Console.WriteLine("Choose action:\n 1.Add\n 2.Update\n 3.Display an object by Id\n 4.display a list of objects\n 5.Exit");
//                try
//                {
//                    choice = (Choices)(Convert.ToInt32(Console.ReadLine()));
//                }
//                catch
//                {
//                    choice = (Choices)(-1);
//                }

//                switch (choice)
//                {
//                    case Choices.Add:
//                        chooseObjToAdd();
//                        break;
//                    case Choices.Update:
//                        UpdateFunc();
//                        break;
//                    case Choices.ShowWithId:
//                        receivesAndDisplaysObjById();
//                        break;
//                    case Choices.ShowList:
//                        receivesArrObjs();
//                        break;
//                    case Choices.exit:
//                        return;
//                    default:
//                        Console.WriteLine("== ERROR ==");
//                        break;
//                }
//            } while ((int)choice != 5);
//        }


//        public static void chooseObjToAdd()
//        {
//            Console.WriteLine("Enter your choice to add:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel ");
//            objects obj;
//            try
//            {
//                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
//            }
//            catch
//            {
//                obj = (objects)(-1);
//            }
//            switch (obj)
//            {
//                case objects.Station:
//                    addStation();
//                    break;
//                case objects.Drone:
//                    addDrone();
//                    break;
//                case objects.Customer:
//                    addCustomer();
//                    break;
//                case objects.Parcel:
//                    addParcel();
//                    break;
//                default:
//                    Console.WriteLine("== ERROR ==");
//                    break;
//            }
//        }
//        public static void UpdateFunc()
//        {
//            Console.WriteLine("Enter your choice to update:\n 1.Drone receives parcel \n 2.Drone collects a parcel\n 3.Costumer gets parcel\n 4.send drone to charge\n 5.free drone from harge ");
//            UpdateObj choice;
//            try
//            {
//                choice = (UpdateObj)(Convert.ToInt32(Console.ReadLine()));
//            }
//            catch
//            {
//                choice = (UpdateObj)(-1);
//            }
//            switch (choice)
//            {
//                case UpdateObj.DroneReceivesParcel:
//                    Console.WriteLine("Enter ID of parcel");
//                    int parcelId = Convert.ToInt32(Console.ReadLine());
//                    Parcel parcel = dal.getParcelWithSpecificCondition(p=> p.Id == parcelId).First();
//                    if (parcel.DroneId == -1)
//                        Console.WriteLine(dal.PairAParcelWithADrone(parcel));
//                    else Console.WriteLine("Error: Parcel have a drone.");
//                    break;
//                case UpdateObj.DroneCollectsAParcel:
//                    Console.WriteLine("Enter ID of parcel");
//                    int parcelIdToCollect = Convert.ToInt32(Console.ReadLine());
//                    Parcel parcelCollect = dal.getParcelWithSpecificCondition(p => p.Id == parcelIdToCollect).First(); 
//                    dalObject.DroneCollectsAParcel(parcelCollect);
//                    break;
//                case UpdateObj.CostumerGetsParcel:
//                    Console.WriteLine("Enter ID of parcel");
//                    int parcelIdGet = Convert.ToInt32(Console.ReadLine());
//                    Parcel parcelGet = dal.getParcelWithSpecificCondition(p => p.Id == parcelIdGet).First();
//                    Drone drone = dal.getDroneWithSpecificCondition(d=> d.Id == parcelGet.DroneId).First();
//                    dalObject.CostumerGetsParcel(drone, parcelGet);
//                    break;
//                case UpdateObj.sendDroneToCharge:
//                    Console.WriteLine("Enter ID of drone");
//                    int droneId = Convert.ToInt32(Console.ReadLine());
//                    Drone droneToCharge = dal.getDroneWithSpecificCondition(d => d.Id == droneId).First();
//                    break;
//                case UpdateObj.freeDroneFromCharge:
//                    Console.WriteLine("Enter Id of drone to charge");
//                    int droneIdCharged = Convert.ToInt32(Console.ReadLine());
//                    Drone droneToFree = dal.getDroneWithSpecificCondition(d => d.Id == droneIdCharged).First();
//                    break;
//                default:
//                    Console.WriteLine("== ERROR ==");
//                    break;
//            }
//        }
//        public static void receivesAndDisplaysObjById()
//        {
//            Console.WriteLine("Enter your choice to display:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel ");
//            objects obj;
//            try
//            {
//                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
//            }
//            catch
//            {
//                obj = (objects)(-1);
//            }
//            int id = new int();
//            if ((int)obj > 0 && (int)obj < 5)
//            {
//                Console.WriteLine("Enter the Id of the object");
//                id = Convert.ToInt32(Console.ReadLine());
//            }
//            switch (obj)
//            {
//                case objects.Station:
//                    Station s = dal.getStationWithSpecificCondition(e=> e.Id == id).First();
//                    Console.WriteLine(s.ToString());
//                    break;
//                case objects.Drone:
//                    Drone d = dal.getDroneWithSpecificCondition(e => e.Id == id).First();
//                    Console.WriteLine(d.ToString());
//                    break;
//                case objects.Customer:
//                    Customer c = dal.getCustomerWithSpecificCondition(e => e.Id == id).First();
//                    Console.WriteLine(c.ToString());
//                    break;
//                case objects.Parcel:
//                    Parcel p = dal.getParcelWithSpecificCondition(e => e.Id == id).First();
//                    Console.WriteLine(p.ToString());
//                    break;
//                default:
//                    Console.WriteLine("== ERROR ==");
//                    break;
//            }
//        }

//        public static void displayArrObjs<T>(T[] arrObjects)
//        {
//            foreach (T obj in arrObjects)
//            {
//                Console.WriteLine(obj.ToString());
//            }
//        }

//        public static void dis<T>(IEnumerable<T> arrObjects)
//        {
//            foreach (T obj in arrObjects)
//            {
//                Console.WriteLine(obj.ToString());
//            }
//        }


//        /// <summary>
//        /// Receives an array the objects. (according to a switch)
//        /// and send the array to displayArrObjs.
//        /// </summary>
//        public static void receivesArrObjs()
//        {
//            Console.WriteLine("Enter your choice to display:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel\n 5.Parcels Without drone\n 6.Station with empty Charging positions ");
//            objects obj;
//            try
//            {
//                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
//            }
//            catch
//            {
//                obj = (objects)(-1);
//            }
//            switch (obj)
//            {
//                case objects.Station:
//                    IEnumerable<Station> stations = dal.GetStations();
//                    Station[] stationsArr = stations.Cast<Station>().ToArray();
//                    displayArrObjs(stationsArr);
//                    break;
//                case objects.Drone:
//                    IEnumerable<Drone> drones = dal.GetDrones();
//                    Drone[] dronesArr = drones.Cast<Drone>().ToArray();
//                    displayArrObjs(dronesArr);
//                    break;
//                case objects.Customer:
//                    IEnumerable<Customer> customers = dal.GetCustomers();
//                    Customer[] customersArr = customers.Cast<Customer>().ToArray();
//                    displayArrObjs(customersArr);
//                    break;
//                case objects.Parcel:
//                    IEnumerable<Parcel> parcels = dal.GetParcels();
//                    Parcel[] parcelsArr = parcels.Cast<Parcel>().ToArray();
//                    displayArrObjs(parcelsArr);
//                    break;
//                case objects.FreeParcel:
//                    IEnumerable<Parcel> freeParcels = dal.getParcelWithSpecificCondition(p=> p.Requeasted == null);
//                    Parcel[] freeParcelsArr = freeParcels.Cast<Parcel>().ToArray();
//                    displayArrObjs(freeParcelsArr);
//                    break;
//                case objects.EmptyCharges:
//                    IEnumerable<Station> s = dal.GetStations();
//                    int amountFullChargingSlots = 0;
//                    try
//                    {
//                        amountFullChargingSlots = dal.getDroneChargeWithSpecificCondition(d => d.StationId == s.Id).Count();
//                    }
//                    catch (Exception) { }
//                    s = s.Where(s => s.ChargeSlots - amountFullChargingSlots > 0);
//                    dis(s);
//                    break;
//                default:
//                    Console.WriteLine("== ERROR ==");
//                    break;
//            }
//        }
//        public static void addStation()
//        {
//            Random r = new Random();
//            int amountS = dal.amountStations();
//            if (amountS >= 5)
//            {
//                Console.WriteLine("== Cann't add stations ==");
//                return;
//            }
//            Console.WriteLine("Enter a station Name: ");
//            string Name = Console.ReadLine();
//            int ChargeSlots = r.Next(0, 5);
//            Console.WriteLine("Enter a Latitude");
//            int Latitude = Convert.ToInt32(Console.ReadLine());
//            Console.WriteLine("Enter a Longitude");
//            int Longitude = Convert.ToInt32(Console.ReadLine());
//            //dalObject.AddStation(amountS, Name, ChargeSlots, Longitude, Latitude);
//        }
//        public static void addDrone()
//        {
//            Random r = new Random();
//            int amountD = dal.GetDrones().Count();
//            if (amountD >= 10)
//            {
//                Console.WriteLine("== Cann't add Drones ==");
//                return;
//            }
//            Console.WriteLine("Enter a Model");
//            string Model = Console.ReadLine();
//            Console.WriteLine("Enter WeightCategory (1, 2 or 3):");
//            WeightCategories MaxWeight = (WeightCategories)Convert.ToInt32(Console.ReadLine());
//            //DroneStatus Status = DroneStatus.Available;
//            //double Battery = 100;
//            dal.AddDrone(amountD, Model, MaxWeight);
//        }
//        //need sometimes to use customer's detailes - return customer.
//        public static Customer addCustomer()
//        {
//            Random r = new Random();
//            Customer c = new Customer();
//            int amountC = DalObject.DalObject.amountCustomers();
//            if (amountC >= 100)
//            {
//                Console.WriteLine("== Cann't add costumers ==");
//                return c;
//            }
//            int id = 0;
//            do
//            {
//                id = r.Next(100, 1000);
//                c = DalObject.DalObject.getCustomerById(id);
//            } while (!c.Equals(null)); //check if the id exist
//            Console.WriteLine("Enter costumer's Name: ");
//            string Name = Console.ReadLine();
//            Console.WriteLine("Enter costumer's Phone: ");
//            string Phone = Console.ReadLine();
//            Console.WriteLine("Enter a Latitude: ");
//            int Latitude = Convert.ToInt32(Console.ReadLine());
//            Console.WriteLine("Enter a Longitude: ");
//            int Longitude = Convert.ToInt32(Console.ReadLine());
//            int ChargeSlots = r.Next(0, 200);
//            dalObject.AddCustomer(id, Name, Phone, Longitude, Latitude);
//            return c;
//        }
//        //need sometimes to use parcel's detailes - return parcel.
//        public static Parcel addParcel()
//        {
//            Random r = new Random();
//            int id = 0;
//            Parcel p = new Parcel();
//            int amountP = DalObject.DalObject.amountParcels();
//            if (amountP >= 1000)
//            {
//                Console.WriteLine("== Cann't add costumers ==");
//                return p;
//            }
//            do
//            {
//                id = r.Next(1000, 10000);
//                p = DalObject.DalObject.getParcelById(id);
//            } while (!p.Equals(null)); //check if the id exist
//            int Serderid;
//            int TargetId;
//            do
//            {
//                Console.WriteLine("Enter the sending costumer's id: ");
//                Serderid = Convert.ToInt32(Console.ReadLine());///////
//                if (DalObject.DalObject.getCustomerById(Serderid).Equals(null))
//                {
//                    Console.WriteLine("Error: no found customer");
//                }
//            } while (DalObject.DalObject.getCustomerById(Serderid).Equals(null));
//            do
//            {
//                Console.WriteLine("Enter the receiving costumer's id: ");
//                TargetId = Convert.ToInt32(Console.ReadLine());
//                if (DalObject.DalObject.getCustomerById(Serderid).Equals(null))
//                {
//                    Console.WriteLine("Error: no found customer");
//                }
//            } while (DalObject.DalObject.getCustomerById(TargetId).Equals(null));
//            Console.WriteLine("Enter WeightCategory (1, 2 or 3):");
//            WeightCategories Weight = (WeightCategories)Convert.ToInt32(Console.ReadLine());
//            Console.WriteLine("Enter priority (1, 2 or 3):");
//            Priorities Priority = (Priorities)Convert.ToInt32(Console.ReadLine());
//            DateTime requestedTime = DateTime.Now;
//            dalObject.AddParcelToDelivery(id, Serderid, TargetId, Weight, Priority, requestedTime);
//            return p;
//        }
//    }
//}



