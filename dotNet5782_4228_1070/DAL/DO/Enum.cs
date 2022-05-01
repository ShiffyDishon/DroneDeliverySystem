using System;



namespace DO
{
    /// <summary>
    /// WeightCategories of a parcel and drones' max weight.
    /// </summary>
    public enum WeightCategories { Light = 1, Medium, Heavy }

    /// <summary>
    /// Priorities of a parcel.
    /// </summary>
    public enum Priorities { Regular = 1, Fast, Emergency }

    /// <summary>
    /// Parcel status from Requeasted to Delivered
    /// </summary>
    public enum ParcelStatuses { Requeasted, Scheduled, PickedUp, Delivered };

}



