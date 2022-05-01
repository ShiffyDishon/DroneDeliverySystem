using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Windows;

namespace PO
{
    public class Parcel : DependencyObject
    {
        public Parcel(BlApi.IBl blObject)
        {
            blObject.ParcelChangeAction += Update;
        }

        public Parcel(BlApi.IBl blObject, BO.Parcel p)
        {
            Id = p.Id;
            Sender = p.Sender;
            Target = p.Target;
            Weight = (WeightCategories)p.Weight;
            Priority = (Priorities)p.Priority;
            Drone = p.Drone;
            Requeasted = p.Requeasted;
            Scheduled = p.Scheduled;
            PickUp = p.PickUp;
            Delivered = p.Delivered;
            blObject.ParcelChangeAction += Update;
        }

        public void Update(BO.Parcel p)
        {
            Id = p.Id;
            Sender = p.Sender;
            Target = p.Target;
            Weight = (WeightCategories)p.Weight;
            Priority = (Priorities)p.Priority;
            Drone = p.Drone;
            Requeasted = p.Requeasted;
            Scheduled = p.Scheduled;
            PickUp = p.PickUp;
            Delivered = p.Delivered;
        }

        public BO.Parcel BO()
        {
            return new BO.Parcel()
            {
                Id = this.Id,
                Sender = this.Sender,
                Target = this.Target,
                Weight = (BO.WeightCategories)this.Weight,
                Priority = (BO.Priorities)this.Priority,
                Drone = this.Drone,
                Requeasted = this.Requeasted,
                Scheduled = this.Scheduled,
                PickUp = this.PickUp,
                Delivered = this.Delivered
            };
        }

        public int Id
        {
            get { return (int)GetValue(IdParcelProperty); }
            set { SetValue(IdParcelProperty, value); }
        }

        public BO.CustomerInParcel Sender
        {
            get { return (BO.CustomerInParcel)GetValue(SenderProperty); }
            set { SetValue(SenderProperty, value); }
        }

        public BO.CustomerInParcel Target
        {
            get { return (BO.CustomerInParcel)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public WeightCategories Weight
        {
            get { return (WeightCategories)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        public Priorities Priority
        {
            get { return (Priorities)GetValue(PriorityProperty); }
            set { SetValue(PriorityProperty, value); }
        }

        public BO.DroneInParcel Drone
        {
            get { return (BO.DroneInParcel)GetValue(DroneProperty); }
            set { SetValue(DroneProperty, value); }
        }

        public DateTime? Requeasted
        {
            get { return (DateTime?)GetValue(RequeastedProperty); }
            set { SetValue(RequeastedProperty, value); }
        } //prepare a parcel to delivery

        public DateTime? Scheduled
        {
            get { return (DateTime?)GetValue(ScheduledProperty); }
            set { SetValue(ScheduledProperty, value); }
        } //pair a parcel to drone

        public DateTime? PickUp
        {
            get { return (DateTime?)GetValue(PickUpProperty); }
            set { SetValue(PickUpProperty, value); }
        }

        public DateTime? Delivered
        {
            get { return (DateTime?)GetValue(DeliveredProperty); }
            set { SetValue(DeliveredProperty, value); }
        }

        public static readonly DependencyProperty IdParcelProperty = DependencyProperty.Register("Id", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty SenderProperty = DependencyProperty.Register("Sender", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty DroneProperty = DependencyProperty.Register("Drone", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty RequeastedProperty = DependencyProperty.Register("Requeasted", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ScheduledProperty = DependencyProperty.Register("Scheduled", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty PickUpProperty = DependencyProperty.Register("PickUp", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty DeliveredProperty = DependencyProperty.Register("Delivered", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(object), typeof(Parcel), new UIPropertyMetadata(0));
    }
}

