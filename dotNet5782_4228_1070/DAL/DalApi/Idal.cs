using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DalApi
{
    public interface IDal
    {
        //================
        //Drone functions
        //================
        IEnumerable<Drone> GetDrones();
        IEnumerable<Drone> getDroneWithSpecificCondition(Predicate<Drone> predicate);
        void AddDrone(Drone d);
        void removeDrone(Drone droneToRemove);
        void changeDroneInfo(Drone d);
        double[] electricityUseByDrone();


        //=================
        //Station functions
        //=================
        IEnumerable<Station> GetStations();
        IEnumerable<Station> getStationWithSpecificCondition(Predicate<Station> predicate);
        void AddStation(Station s);
        void removeStation(Station stationToRemove);
        void changeStationInfo(Station s);

        
        //==================
        //Customer functions
        //==================
        IEnumerable<Customer> GetCustomers();
        IEnumerable<Customer> getCustomerWithSpecificCondition(Predicate<Customer> predicate);
        void AddCustomer(Customer c);
        void removeCustomer(Customer customerToRemove);
        void changeCustomerInfo(Customer c);


        //================
        //Parcel functions
        //================
        IEnumerable<Parcel> GetParcels();
        public IEnumerable<Parcel> getParcelWithSpecificCondition(Predicate<Parcel> predicate);
        void AddParcel(Parcel parcel);
        void removeParcel(Parcel parcel);
        void changeParcelInfo(Parcel p);
        int amountParcels();

        //======================
        //Drone Charge functions
        //======================
        public IEnumerable<DroneCharge> getDroneChargeWithSpecificCondition(Predicate<DroneCharge> predicate);
        void AddDroneToCharge(DroneCharge droneCharge);
        void removeDroneChargeByDroneId(int droneId);


        //================
        //Worker functions
        //================
        IEnumerable<Worker> getWorkerWithSpecificCondition(Predicate<Worker> predicate);
    }
}