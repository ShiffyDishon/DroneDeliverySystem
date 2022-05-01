using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        /// <summary>
        /// Instance of IBl interface.
        /// </summary>
        private BlApi.IBl blObjectD;

        /// <summary>
        /// Current customer
        /// </summary>
        PO.Customer currentCustomer;

        /// <summary>
        /// Show update or add form
        /// </summary>
        private bool updateOrAddWindow { get; set; }

        /// <summary>
        /// Usage by client or admin
        /// </summary>
        bool isClient = false;
        
        /// <summary>
        /// If entered from parcels save the parcel of customer
        /// </summary>
        BO.Parcel parcelOfCustomer = null;

        /// <summary>
        /// If came from a spacific parcel window
        /// </summary>
        bool cameFromParcelWindow = false;

        #region the closing button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        /// <summary>
        /// Ctor display the add a customer Form
        /// </summary>
        /// <param name="blObject">Instance of the interface Ibl</param>
        public CustomerWindow(BlApi.IBl blObject)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded;
            blObjectD = blObject;
            currentCustomer = new PO.Customer(blObject);
            updateOrAddWindow = true;
            PLFunctions.ClearFormTextBox(IdTextBox, NameTextBox, PhoneTextBox, LatitudeTextBox, LongitudeTextBox);
            visibleAddForm.Visibility = Visibility.Visible;
            visibleUpdateForm.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Ctor display the update/see info a specific customer Form.
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        /// <param name="customerInCtor">The customer to update/see info</param>
        public CustomerWindow(BlApi.IBl blObject, BO.Customer customerInCtor)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded; 
            updateOrAddWindow = false;
            blObjectD = blObject;
            currentCustomer = new PO.Customer(blObject, customerInCtor);
            AddOrUpdateCustomer.DataContext = currentCustomer;
            visibleAddForm.Visibility = Visibility.Hidden;
            visibleUpdateForm.Visibility = Visibility.Visible;
            parcelsListViewContantAndDispaly();
            AddOrUpdateCustomer.Height = 400;
        }

        /// <summary>
        /// Ctor display the update/see info a specific clients Form.
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        /// <param name="client">The customer to update/see info</param>
        /// <param name="isClient">if the user is a client or a worker</param>
        public CustomerWindow(BlApi.IBl blObject, BO.Customer client, bool isClient , BO.Parcel parcelOfCustomer = null)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded; //The x button
            updateOrAddWindow = false;
            this.isClient = isClient;
            blObjectD = blObject;
           
            visibleAddForm.Visibility = Visibility.Hidden;
            visibleUpdateForm.Visibility = Visibility.Visible;
            currentCustomer = new PO.Customer(blObject, client);
            AddOrUpdateCustomer.DataContext = currentCustomer;

            parcelsListViewContantAndDispaly();
            if (isClient)
            {
                AddlButton.Visibility = Visibility.Visible;
                ReturnToPageDroneListWindow.Content = "Log Out";
                RemoveBtn.Visibility = Visibility.Hidden;
                AddParcelButton.Visibility = Visibility.Visible;
            }
            this.parcelOfCustomer = parcelOfCustomer;
            cameFromParcelWindow = parcelOfCustomer == null ? false : true;
        }

        /// <summary>
        /// If there is no list of parcels of the client/customer hide the list space
        /// </summary>
        private void parcelsListViewContantAndDispaly()
        {
            if (currentCustomer.CustomerAsSender.Count == 0)
                ExpenderSender.Visibility = Visibility.Hidden;
            if (currentCustomer.CustomerAsTarget.Count == 0)
                ExpenderTarget.Visibility = Visibility.Hidden;
        }

        /// <summary>
        // Code to remove close box from window
        /// false == show update window
        /// true == show add window
        /// </summary>
        void ToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        /// <summary>
        /// Add a customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCustomerBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.Customer newCustomer = new BO.Customer()
                {
                    Id = int.Parse(IdTextBox.Text),
                    Name = NameTextBox.Text,
                    Phone = PhoneTextBox.Text,
                    CustomerPosition = new BO.Position()
                    {
                        Latitude = int.Parse(LatitudeTextBox.Text),
                        Longitude = int.Parse(LongitudeTextBox.Text)
                    }
                };

                try
                {
                    blObjectD.AddCustomer(newCustomer);
                }
                catch (Exceptions.DataChanged e1) { PLFunctions.messageBoxResponseFromServer("Add a Customer", $"Customer was added successfully\n{e1.Message}"); }
                new CustomerListWindow(blObjectD).Show();
                this.Close();
            }

            #region catch exeptions
            catch (BO.Exceptions.ObjExistException e1)
            {
                PLFunctions.messageBoxResponseFromServer("Add Customer", e1.Message);
            }
            catch (Exception)
            {
                PLFunctions.messageBoxResponseFromServer("Add Customer", "== ERROR receiving data ==\nPlease try again");
            }
            #endregion
        }

        /// <summary>
        /// Return to parcel window of customer.
        /// </summary>
        private void returnToParcelWindowOfCustomer()
        {
            bool isSender = currentCustomer.Id == parcelOfCustomer.Sender.Id ? true : false;
            new ParcelWindow(blObjectD, parcelOfCustomer, true, isSender).Show();
            this.Close();
        }

        /// <summary>
        /// Reset all text boxes in the Add form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickRestart(object sender, RoutedEventArgs e)
        {
            PLFunctions.ClearFormTextBox(IdTextBox, NameTextBox, PhoneTextBox, LatitudeTextBox, LongitudeTextBox);
        }

        /// <summary>
        /// Return back to CustomerList
        /// if isClient = true return back to logIn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickReturnToPageCustomerListWindow(object sender, RoutedEventArgs e)
        {
            if (isClient)
            {
                MessageBoxResult messageBoxClosing = MessageBox.Show("Are you Sure you wan't to exit", "GoodBy", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (messageBoxClosing == MessageBoxResult.OK)
                {
                    new SignInOrUpWindow(blObjectD).Show();
                    this.Close();
                }
            }
            else
            {
                if (cameFromParcelWindow)
                    returnToParcelWindowOfCustomer();
                else
                {
                    new CustomerListWindow(blObjectD).Show();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Update the customers info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            blObjectD.changeCustomerInfo(currentCustomer.Id, NameTextBox.Text, PhoneTextBox.Text);
            //currentCustomer.Update(blObjectD.UpdateCustomerDetails(currentCustomer.Id, NameTextBox.Text, PhoneTextBox.Text));
            if (!isClient)
            {
                if (cameFromParcelWindow)
                    returnToParcelWindowOfCustomer();
                else
                {
                    new CustomerListWindow(blObjectD).Show();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Select a spaecific parcel of customer as sender and open specific parcelWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectParcelOfSender(object sender, MouseButtonEventArgs e)
        {
            if (cameFromParcelWindow)
                return;

            BO.ParcelAtCustomer parcelAtCustomer = (BO.ParcelAtCustomer)CustomerAsSenderParcelsListView.SelectedItem;
            BO.Parcel parcel = blObjectD.GetParcelById(parcelAtCustomer.Id);
            if (isClient)
                new ParcelWindow(blObjectD, parcel, true, this).Show();
            else
                new ParcelWindow(blObjectD, parcel, false , true).Show();
            this.Close();
        }

        /// <summary>
        /// Select a spaecific parcel of customer as target and open specific parcelWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectParcelOfTarget(object sender, MouseButtonEventArgs e)
        {
            if (cameFromParcelWindow)
                return;

            BO.ParcelAtCustomer parcelAtCustomer = (BO.ParcelAtCustomer)CustomerAsTargetParcelsListView.SelectedItem;
            BO.Parcel parcel = blObjectD.GetParcelById(parcelAtCustomer.Id);
            if (isClient)
                new ParcelWindow(blObjectD, parcel, false, this).Show();
            else
                new ParcelWindow(blObjectD, parcel, false , false).Show();
            this.Close();
        }


        /// <summary>
        /// Avoid writing chars in a specific textbox like id.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_OnlyNumbers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PLFunctions.TextBox_OnlyNumbers_PreviewKeyDown(sender, e);
        }

        /// <summary>
        /// Change background of parcels list when the there is a click on expender to close the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderCollapsed(object sender, RoutedEventArgs e)
        {
            CustomerAsSenderParcelsListView.Background = null;
            ExpenderTarget.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Change background of parcels list where customer is a sender when the there is a click on expender to open the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderExpanded(object sender, RoutedEventArgs e)
        {
            CustomerAsSenderParcelsListView.Background = Brushes.White;
            if (currentCustomer.CustomerAsTarget.Count > 0)
                ExpenderTarget.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Change background of parcels list where customer is the target when the there is a click on expender to close the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderExpandedTarget(object sender, RoutedEventArgs e)
        {
            CustomerAsTargetParcelsListView.Background = Brushes.White;
        }

        /// <summary>
        /// Change background of parcels where customer is the target list when the there is a click on expender to open the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderCollapsedTarget(object sender, RoutedEventArgs e)
        {
            CustomerAsTargetParcelsListView.Background = null;
        }

        /// <summary>
        /// Try to send the customer to be removed = not active.
        /// Occurding to instuctions the customer will be removed and no sending and recieving new parcels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                blObjectD.RemoveCustomer(currentCustomer.Id);
                new CustomerListWindow(blObjectD).Show();
                this.Close();
            }
            catch (BO.Exceptions.ObjNotExistException e1)
            {
                PLFunctions.messageBoxResponseFromServer("Remove Customer", e1.Message);
            }
        }

        /// <summary>
        /// Add a parcel if client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addParcelBtnClick(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(blObjectD, currentCustomer.Id).Show();
            this.Close();
        }
    }
}
