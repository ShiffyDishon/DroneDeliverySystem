using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;


namespace Dal
{
    internal static class DataSource
    {
        internal static List<Drone> Drones;
        internal static List<Station> Stations;
        internal static List<Customer> Customers;
        internal static List<Parcel> Parcels;
        internal static List<DroneCharge> DroneCharges;
        internal static List<Worker> Workers;

        /// <summary>
        /// Initialize info of the program.
        /// </summary>
        public static void Initialize()
        {
            Drones = new List<Drone>();
            Stations = new List<Station>();
            Customers = new List<Customer>();
            Parcels = new List<Parcel>();
            DroneCharges = new List<DroneCharge>();
            Workers = new List<Worker>();

            Random r = new Random();

            int amountStations = r.Next(2, 5);
            for (int i = 1; i <= amountStations; i++)
            {
                Station s = new Station()
                {
                    Id = i,
                    Name = $"station{i}",
                    ChargeSlots = r.Next(5, 10),
                    Latitude = r.Next(1, 90),
                    Longitude = r.Next(1, 90),
                    IsActive = true
                };
                Stations.Add(s);
            }

            int amountDrones = r.Next(5, 10);
            for (int i = 1; i <= amountDrones; i++)
            {
                Drone d = new Drone()
                {
                    Id = i,
                    Model = $"Drone{i}",
                    MaxWeight = (WeightCategories)r.Next(1, 4),
                    IsActive = true
                };
                Drones.Add(d);
            }

            int amountCustomer = r.Next(10, 100);
            for (int i = 1; i <= amountStations; i++)
            {
                Customer c = new Customer()
                {
                    //Id = r.Next(100000000, 1000000000),
                    Id = i,
                    Name = $"Customer{i}",
                    Phone = $"{r.Next(100000000, 1000000000)}",
                    Latitude = r.Next(1, 90),
                    Longitude = r.Next(1, 90),
                    IsActive = true
                };

                Customers.Add(c);
            }

            int amountParcels = r.Next(6, 8);
            for (int i = 1; i <= amountParcels; i++)
            {
                Parcel p = new Parcel()
                {
                    Id = i,
                    Weight = (WeightCategories)r.Next(1, 3),
                    Priority = (Priorities)r.Next(1, 4),
                    Requeasted = DateTime.Now,
                    IsActive = true
                };
                p.SenderId = Customers[r.Next(0, Customers.Count)].Id;
                p.TargetId = Customers[r.Next(0, Customers.Count)].Id;
                do
                {
                    p.TargetId = Customers[r.Next(0, Customers.Count)].Id;
                } while (p.SenderId == p.TargetId);
                int matchToDrone = r.Next(0, 2);
                if (matchToDrone == 0)  
                {
                    p.DroneId = Drones.FirstOrDefault(d =>
                    (d.MaxWeight >= p.Weight
                        &&
                   !Parcels.Any(p => p.DroneId == d.Id))).Id;
                }
                if (p.DroneId > 0)
                {
                    int amountTimesToStart = r.Next(0, 3);
                    switch (amountTimesToStart)
                    {
                        case 0:
                            p.Scheduled = DateTime.Now;
                            break;
                        case 1:
                            p.Scheduled = DateTime.Now;
                            p.PickUp = DateTime.Now;
                            break;
                        case 2:
                            p.Scheduled = DateTime.Now;
                            p.PickUp = DateTime.Now;
                            p.Delivered = DateTime.Now;
                            break;
                        default:
                            break;
                    }
                }
                Parcels.Add(p);
            }

            int amountWorkers = r.Next(2, 10);
            for (int i = 1; i <= amountStations; i++)
            {
                Workers.Add(new Worker()
                {
                    Id = i,
                    Password = $"Worker{i}",
                });
            }
           
            Config.empty = .1;
            Config.lightWeight = .2;
            Config.mediumWeight = .4;
            Config.heavyWeight = .6;
            Config.chargingRate = .7;
        }

        internal static class Config
        {
            internal static double empty = .1;
            internal static double lightWeight = .2;
            internal static double mediumWeight = .4;
            internal static double heavyWeight = .6;
            internal static double chargingRate = .7;
            internal static string DalObjectOrDalXml = "DalXml";
        }
    }
}
