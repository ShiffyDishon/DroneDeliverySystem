using System;
using System.Collections.Generic;
using System.Text;
using BO;

namespace BlApi
{
    public interface IBl
    {
        public Action<Drone> DroneChangeAction { get; set; }
        Action<Drone, bool, bool> DroneListChangeAction { get; set; }
        Action<Customer> CustomerChangeAction { get; set; }
        Action<Parcel> ParcelChangeAction { get; set; }

        //=================
        //Station functions
        //=================
        IEnumerable<StationToList> GetStationsToList();
        IEnumerable<StationToList> GetStationsWithFreeSlots(int amountAvilableSlots = 0);
        Station GetStationById(int id);
        void AddStation(Station stationToAdd);
        void RemoveStation(Station station);
        void changeStationInfo(int id, string name = null, int ChargeSlots = -1);


        //===============
        //Drone functions
        //===============
        IEnumerable<DroneToList> GetDronesToList();
        IEnumerable<DroneToList> GetDronesByConditions(int weight, int status);
        Drone GetDroneById(int id);
        void AddDrone(Drone droneToAdd, int stationId);
        void RemoveDrone(Drone drone);
        void ChangeDroneModel(int droneId, string newModel);


        DO.Station findAvailbleAndClosestStationForDrone(Position dronePosition, double droneBattery);
        public double requestElectricity(int choice);
        Drone SendDroneToCharge(int droneId);
        Drone FreeDroneFromCharging(int droneId);
        Drone PairParcelWithDrone(int droneId);
        Drone DronePicksUpParcel(int droneId);
        void changeDroneInfoInDroneList(Drone droneWithUpdateInfo);
        Drone DeliveryParcelByDrone(int idDrone);
        int GetDroneStatusInDelivery(Drone droneId);
        DeliveryStatusAction returnDroneStatusInDelivery(Drone droneId);


        //==================
        //Customer functions
        //==================
        IEnumerable<CustomerToList> GetCustomersToList();
        IEnumerable<CustomerInParcel> GetCustomersExeptOne(int customerId = 0);
        Customer GetCustomerById(int id, string name = null);
        void AddCustomer(BO.Customer customer);
        void RemoveCustomer(int customerId);
        BO.Customer changeCustomerInfo(int id, string name = null, string phone = null);


        //================
        //Parcel functions
        //================
        IEnumerable<ParcelToList> GetParcelToList();
        IEnumerable<ParcelToList> GetParcelsByConditions(int weight, int status, int priority);
        Parcel GetParcelById(int id);
        Parcel GetParcelByDrone(int droneId);
        void AddParcel(Parcel parcelToAdd);
        void RemoveParcel(int parcelId);
        void updateParcel(int Id, CustomerInParcel targetCustomer, Priorities priority, WeightCategories weight);



        //===================
        //  StartSimulation
        //===================
        void StartSimulation(Drone std, Action< DroneStatusInSim, double> updateStudent, Func<bool> needToStop);

        bool CheckWorkerExist(Worker worker);
        Station convertDalToBLStation(DO.Station station);

    }
}


//בונוסים:

//ממשק לקוח

//אפשרויות כניסה לפי סיסמת לקוח \ כניסה לממשק ניהול עם סיסמת מנהל

//שני מימושים בפרויקטים נפרדים ל IDal

//מחיקת אובייקט – הופכת אובייקט ללא פעיל לצורך אפשרויות שחזור

//Singleton, DalFactory – ללא פרמטרים

//בסימולטור – תצוגה מתעדכנת כל הזמן בהתאם לנתונים – מרחק ובטרייה.

//אפשרות הפעלת סימולטור על כמה רחפנים במקביל.

//אין שיתוף מידע בין חלונות

//בתצוגה – סינון רשימות לפי כמה פרמטרים

//ארכיטקטורה משופרת ב PL:  Binding, PO objects, etc.

 

//מתלבטות אם לכלול כבונוס:

//·        (Clone  בהעתקת אובייקט)

//·        (פיצול לממשקים שונים – Ibl, ISimulation(

 

 

//מה כדאי להוריד:

//התמקדות ב exceptions מתאימים ותפיסתם.

//לא תמיד השתמשנו ב IEnumerable – אם אפשר להוריד פוקוס

//שימוש בטכנולוגיות של WPF(הדרישה הופיעה בפרויקט)




//במה להתמקד:

//Binding

//סימולטור(חשוב כי הוא ממש יפה)