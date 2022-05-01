using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PO
{
    public class Customer : DependencyObject
    {
        public Customer(BlApi.IBl blObject)
        {
            blObject.CustomerChangeAction += Update;
        }

        public Customer(BlApi.IBl blObject, BO.Customer c)
        {
            this.Update(c);
            blObject.CustomerChangeAction += Update;
        }

        public void Update(BO.Customer c)
        {
            Id = c.Id;
            Name = c.Name;
            Phone = c.Phone;
            CustomerPosition = c.CustomerPosition;
            if (c.CustomerAsSender?.Count > 0)
                CustomerAsSender = c.CustomerAsSender;
            if (c.CustomerAsTarget?.Count > 0)
                CustomerAsTarget = c.CustomerAsTarget;
        }

        public BO.Customer BO()
        {
            return new BO.Customer()
            {
                Id = this.Id,
                Name = this.Name,
                Phone = this.Phone,
                CustomerPosition = new BO.Position()
                {
                    Longitude = this.CustomerPosition.Longitude,
                    Latitude = this.CustomerPosition.Latitude
                },
                CustomerAsSender = this.CustomerAsSender,
                CustomerAsTarget = this.CustomerAsTarget
            };
        }

        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public string Phone
        {
            get { return (string)GetValue(PhoneProperty); }
            set { SetValue(PhoneProperty, value); }
        }
        public BO.Position CustomerPosition
        {
            get { return (BO.Position)GetValue(CustomerPositionProperty); }
            set { SetValue(CustomerPositionProperty, value); }
        }
        public List<BO.ParcelAtCustomer> CustomerAsSender
        {
            get
            {
                if (GetValue(CustomerAsSenderProperty) != null)
                    return (List<BO.ParcelAtCustomer>)GetValue(CustomerAsSenderProperty);
                else return null;
            }
            set { SetValue(CustomerAsSenderProperty, value); }
        }

        public List<BO.ParcelAtCustomer> CustomerAsTarget
        {
            get
            {
                if (!(GetValue(CustomerAsTargetProperty)).Equals(null))
                    return (List<BO.ParcelAtCustomer>)GetValue(CustomerAsTargetProperty);
                else return null;
            }
            set { SetValue(CustomerAsTargetProperty, value); }
        }

        public override string ToString()
        {
            return ($"customer id: {Id}, customer name: {Name}, customer phone: {Phone}, \n\tCustomerPosition: {CustomerPosition.ToString()}" +
              $"\tCustomerAsSenderAmount:  { CustomerAsSender.Count()}\n\tCustomerAsTargetAmount: {CustomerAsTarget.Count()}\n");
        }

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
        public static readonly DependencyProperty PhoneProperty = DependencyProperty.Register("Phone", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
        public static readonly DependencyProperty CustomerPositionProperty = DependencyProperty.Register("CustomerPosition", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
        public static readonly DependencyProperty CustomerAsSenderProperty = DependencyProperty.Register("CustomerAsSender", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
        public static readonly DependencyProperty CustomerAsTargetProperty = DependencyProperty.Register("CustomerAsTarget", typeof(object), typeof(Customer), new UIPropertyMetadata(0));
    }

    public class CustomerInParcel : DependencyObject //targetId in parcel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public override string ToString()
        {
            return ($"Id: {Id} , Name: {name} \n");
        }
    }
}

