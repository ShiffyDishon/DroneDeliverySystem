/*using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using BL;
using BlApi;
using BO;
namespace BL_ConsoleUI
{
    class mainBL_ConsoleUI
    {
        Random r = new Random();
        public static BlApi.Ibl bl;
        /// <summary>
        /// The user chooses what to do with the objects:
        /// 1.Add 2.update 3.display obj by id 4.display arr of obj 5.exit.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bl = BlApi.IBL.BLFactory.Factory("BL");
            Choices choice; //defaualt 0
            do
            {
                Console.WriteLine("Choose action:\n 1.Add\n 2.Update\n 3.Display an object by Id\n 4.display a list of objects\n 5.Exit");
                try
                {
                    choice = (Choices)(Convert.ToInt32(Console.ReadLine()));
                }
                catch
                {
                    choice = (Choices)(-1);
                }

                switch (choice)
                {
                    case Choices.Add:
                        chooseObjToAdd();
                        break;
                    case Choices.Update:
                        UpdateFunc();
                        break;
                    case Choices.ShowWithId:
                        receivesAndDisplaysObjById();
                        break;
                    case Choices.ShowList:
                        receivesArrObjs();
                        break;
                    case Choices.exit:
                        return;
                    default:
                        Console.WriteLine("== ERROR ==");
                        break;
                }

            } while ((int)choice != 5);
        }
        
        /// <summary>
        /// Add an object by request
        /// </summary>
        public static void chooseObjToAdd()
        {
            Console.WriteLine("Enter your choice to add:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel ");
            objects obj;
            try
            {
                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Catch ArgumentNullException");
                obj = (objects)(-1);
            }
            catch
            {
                obj = (objects)(-1);
            }
            switch (obj)
            {
                case objects.Station:
                    addStation();
                    break;
                case objects.Drone:
                    addDrone();
                    break;
                case objects.Customer:
                    addCustomer();
                    break;
                case objects.Parcel:
                    addParcel();//ParcelStatuses.Requeasted;
                    break;
                default:
                    Console.WriteLine("== ERROR ==");
                    break;
            }
        }
        
        /// <summary>
        /// update objects details
        /// </summary>
        public static void UpdateFunc()
        {
            Console.WriteLine("Enter your choice to update:\n 1.DronesInfo \n 2.StationInfo\n 3.CustomerInfo\n 4.sendDroneToCharge\n 5.freeDroneFromCharging\n6.DroneScheduledWithAParcel.\n7.DronePicksUpParcel.\n8.DeliveryParcelByDrone. ");
            UpdateBL choice;
            try
            {
                choice = (UpdateBL)(Convert.ToInt32(Console.ReadLine()));
            }
            catch
            {
                choice = (UpdateBL)(-1);
            }
            switch (choice)
            {
                case UpdateBL.DronesInfo:
                    try
                    {
                        Console.WriteLine("Enter drones' id to change its' models' name: ");
                        int droneId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter a new Model");
                        string model = Console.ReadLine();

                        //bl.DroneChangeModel(droneId, model);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.StationInfo:
                    try
                    {
                        Console.WriteLine("Enter station id: ");
                        int stationId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter a station Name:(optional) ");
                        string sName = Console.ReadLine();
                        Console.WriteLine("Enter amount of Availble charging slots:(optional) ");
                        int chargeSlot = Convert.ToInt32(Console.ReadLine());
                        bl.StationChangeDetails(stationId, sName, chargeSlot);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.CustomerInfo:
                    try
                    {
                        Console.WriteLine("Enter costumer's id: ");
                        int customerId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter costumer's Name: ");
                        string customerName = Console.ReadLine();
                        Console.WriteLine("Enter costumer's Phone: ");
                        string phone = Console.ReadLine();
                        bl.UpdateCustomerDetails(customerId, customerName, phone);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.sendDroneToCharge:
                    try
                    {
                        Console.WriteLine("Enter drones' id that's sent to charging: ");
                        int droneIdSentToCharge = Convert.ToInt32(Console.ReadLine());
                        bl.SendDroneToCharge(droneIdSentToCharge);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.freeDroneFromCharging:
                    try
                    {
                        Console.WriteLine("Enter drones' id that's freed from charging: ");
                        int droneIdFreeFromCharging = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter time period's on charge: ");
                        int timePeriodOnCharge = Convert.ToInt32(Console.ReadLine());
                        bl.FreeDroneFromCharging(droneIdFreeFromCharging, timePeriodOnCharge);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.DroneScheduledWithAParcel://ParcelStatuses.Scheduled
                    try
                    {
                        Console.WriteLine("Enter drones' id that collects a parcel: ");
                        int droneIdToPairAParcelWith = Convert.ToInt32(Console.ReadLine());
                        bl.PairParcelWithDrone(droneIdToPairAParcelWith);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.DronePicksUpParcel://ParcelStatuses.pickUp
                    Console.WriteLine("Enter drones' id that pickes up a parcel: ");
                    int droneIdPickesUpAParcel = Convert.ToInt32(Console.ReadLine());
                    try
                    {
                        bl.DronePicksUpParcel(droneIdPickesUpAParcel);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case UpdateBL.DeliveryParcelByDrone://ParcelStatuses.delivery
                    Console.WriteLine("Enter drones' id that deliverd a parcel: ");
                    int droneIdDeliveredAParcel = Convert.ToInt32(Console.ReadLine());
                    try
                    {
                        bl.DeliveryParcelByDrone(droneIdDeliveredAParcel);
                    }
                    catch (FormatException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (OverflowException )
                    {
                        Console.WriteLine("== ERROR receiving data ==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// display an object by id
        /// </summary>
        public static void receivesAndDisplaysObjById()
        {
            Console.WriteLine("Enter your choice to display:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel ");
            objects obj;
            try
            {
                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
            }
            catch
            {
                obj = (objects)(-1);
            }
            int id = new int();
            if ((int)obj > 0 && (int)obj < 5)
            {
                Console.WriteLine("Enter the Id of the object");
                id = Convert.ToInt32(Console.ReadLine());
            }
            switch (obj)
            {
                case objects.Station:
                    try
                    {
                        BLStation s = bl.GetStationById(id);
                        Console.WriteLine(s.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case objects.Drone:
                    try
                    {
                        Drone d = bl.GetDroneById(id);
                        Console.WriteLine(d.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case objects.Customer:
                    try
                    {
                        Customer c = bl.GetCustomerById(id);
                        Console.WriteLine(c.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case objects.Parcel:
                    try
                    {
                        Parcel p = bl.GetParcelById(id);
                        Console.WriteLine(p.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                default:
                    Console.WriteLine("== ERROR displaying data ==");
                    break;
            }
        }
        
        /// <summary>
        /// Receives an array the objects. (according to a switch)
        /// and send the array to displayArrObjs.
        /// </summary>
        public static void receivesArrObjs()
        {
            Console.WriteLine("Enter your choice to display:\n 1.Station \n 2.Drone\n 3.CLient\n 4.Parcel\n 5.Parcels Without drone\n 6.Station with empty Charging positions ");
            objects obj;
            try
            {
                obj = (objects)(Convert.ToInt32(Console.ReadLine()));
            }
            catch
            {
                obj = (objects)(-1);
            }
            try
            {
                switch (obj)
                {
                    case objects.Station:
                        List<BlApi.BO.BLStation> stations = bl.DisplayStations();
                        stations.ForEach(s => Console.WriteLine(s.ToString() + '\n'));
                        break;
                    case objects.Drone:
                        List<BlApi.BO.Drone> drones = bl.DisplayDrones();
                        drones.ForEach(s => Console.WriteLine(s.ToString()));
                        break;
                    case objects.Customer:
                        List<BlApi.BO.Customer> customers = bl.DisplayCustomers();
                        customers.ForEach(s => Console.WriteLine(s.ToString()));
                        break;
                    case objects.Parcel:
                        List<BlApi.BO.Parcel> parcels = bl.DisplayParcel();
                        parcels.ForEach(s => Console.WriteLine(s.ToString()));
                        break;
                    case objects.FreeParcel:
                        List<BlApi.BO.Parcel> freeParcels = bl.DisplayFreeParcel();
                        freeParcels.ForEach(s => Console.WriteLine(s.ToString()));
                        break;
                    case objects.EmptyCharges:
                        List<BlApi.BO.BLStation> ChargeStand = bl.DisplayEmptyDroneCharge();
                        ChargeStand.ForEach(c => Console.WriteLine(c.ToString()));
                        break;
                    default:
                        Console.WriteLine("== ERROR ==");
                        break;
                }
            }
            catch (ArgumentNullException )
            {
                Console.WriteLine("== ERROR displaying data ==");
            }
            catch (InvalidOperationException )
            {
                Console.WriteLine("== ERROR displaying data ==");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public static void addStation()
        {
            int flag = 0;
            do
            {
                try
                {
                    Console.WriteLine("Enter station id: ");
                    int id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter a station Name: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter a Latitude");
                    int latitude = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter a Longitude");
                    int longitude = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter amount of Availble charging slots: ");
                    int chargeSlot = Convert.ToInt32(Console.ReadLine());
                    bl.AddStation(id, name, latitude, longitude, chargeSlot);
                    break;
                }
                catch (FormatException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (OverflowException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (Exception e)
                {
                    flag++;
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Plese try again:\nYou have {3 - flag} more times to try.");
                    if (flag == 2) Console.WriteLine("== Cann't add stations. ==");
                }
            } while (flag < 2);
        }

        public static void addDrone()
        {
            int flag = 0;
            do
            {
                try
                {
                    Console.WriteLine("Enter the manufacturer serial number: ");
                    int id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter a Model");
                    string Model = Console.ReadLine();
                    Console.WriteLine("Enter max weight of drone (category) (Light =1, Medium=2, Heavy=3): ");
                    int MaxWeight = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter station id for charging drone for the initial charging:");
                    int stationId = Convert.ToInt32(Console.ReadLine());

                    bl.AddDrone(id, Model, MaxWeight, stationId);
                    break;
                }
                catch (FormatException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (OverflowException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (Exception e)
                {
                    flag++;
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Plese try again:\n You have {3 - flag} more times to try.");
                    if (flag == 2) Console.WriteLine("== Cann't add drone. ==");
                }
            } while (flag < 2);
        }

        public static void addParcel()
        {
            int flag = 0;
            do
            {
                try
                {
                    Console.WriteLine("Enter sender's id: ");
                    int senderID = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter target's id: ");
                    int targetId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter weight of parcel (Light =1, Medium=2, Heavy=3): ");
                    int weight = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter priority of parcel (Regular=1, Fast=2, Emergency=3 ): ");
                    int priority = Convert.ToInt32(Console.ReadLine());
                    bl.AddParcel(senderID, targetId, weight, priority);
                    break;
                }
                catch (FormatException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (OverflowException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (Exception e)
                {
                    flag++;
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Plese try again:\n You have {3 - flag} more times to try.");
                    if (flag == 2) Console.WriteLine("== Cann't add parcel. ==");
                }
            } while (flag < 2);
        }

        public static void addCustomer()
        {
            Random r = new Random();
            int flag = 0;
            do
            {
                try
                {
                    Console.WriteLine("Enter costumer's id: ");
                    int id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter costumer's Name: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter costumer's Phone: ");
                    string phone = Console.ReadLine();
                    Console.WriteLine("Enter a Latitude: ");
                    int latitude = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter a Longitude: ");
                    int longitude = Convert.ToInt32(Console.ReadLine());
                    int ChargeSlots = r.Next(0, 200);
                    bl.AddCustomer(id, name, phone, new BlApi.BO.Position() { Longitude = longitude, Latitude = latitude });
                    break;
                }
                catch (FormatException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (OverflowException )
                {
                    Console.WriteLine("== ERROR receiving data ==");
                }
                catch (Exception e)
                {
                    flag++;
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Plese try again:\n You have {3 - flag} more times to try.");
                    if (flag == 2) Console.WriteLine("== Cann't add customer. ==");
                }
            } while (flag < 2);
        }
    }
}*/