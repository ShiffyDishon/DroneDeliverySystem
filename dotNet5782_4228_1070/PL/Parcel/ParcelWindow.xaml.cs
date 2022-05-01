using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        private BlApi.IBl blObject;
        private PO.Parcel currentParcel;
        private BO.Customer clientCustomer;
        private bool isClientAndNotAdmin = false;
        private bool clientIsSender = false;
        private bool returnToParcelListWindow = false;
        private Window returnBackToUnupdateWindow;
        private bool customerUpdateHisParcel = false;
        private bool updateVisible;

        BO.ParcelStatuses parcelStatus;

        #region the closing button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        void ToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion

        /// <summary>
        /// As admin add a parcel
        /// </summary>
        /// <param name="blObject"></param>
        public ParcelWindow(BlApi.IBl blObject)
        {
            InitializeComponent();
            this.blObject = blObject;
            Loaded += ToolWindowLoaded;
            initializeDetailsAddForm();
            currentParcel = new PO.Parcel(blObject);
            visibleAddForm.Visibility = Visibility.Visible;
            visibleUpdateForm.Visibility = Visibility.Hidden;
            visibleUpdatedetails.Visibility = Visibility.Hidden;
            returnToParcelListWindow = true;
        }

        /// <summary>
        /// customer want's to add a parcel.
        /// </summary>
        /// <param name="blObject"></param>
        /// <param name="senderCustomerId"></param>
        public ParcelWindow(BlApi.IBl blObject, int senderCustomerId)
        {
            InitializeComponent();
            this.blObject = blObject;
            Loaded += ToolWindowLoaded;//The x button
            initializeDetailsAddForm();
            currentParcel = new PO.Parcel(blObject);
            isClientAndNotAdmin = true;
            visibleAddForm.Visibility = Visibility.Visible;
            visibleUpdateForm.Visibility = Visibility.Hidden;
            clientCustomer = blObject.GetCustomerById(senderCustomerId);
            ParcelSenderSelector.Visibility = Visibility.Hidden;
            UpdateButton.Visibility = Visibility.Hidden;
            senderName.Visibility = Visibility.Visible;
            senderName.Text = $" {clientCustomer.Name}";
            ParcelTargetSelector.ItemsSource = blObject.GetCustomersExeptOne(senderCustomerId);
            SenderText.Visibility = Visibility.Visible;
            SenderText.Content = clientCustomer.Name;
            clientIsSender = true;
            returnToParcelListWindow = false;
            visibleUpdatedetails.Visibility = currentParcel.Drone.Equals(null) ? Visibility.Visible : Visibility.Hidden;
            if (currentParcel.Drone.Equals(null))
            {
                ParcelWeightSelector1.ItemsSource = Enum.GetValues(typeof(WeightCategories));
                ParcelPrioritySelector1.ItemsSource = Enum.GetValues(typeof(Priorities));
                ParcelTargetSelector1.ItemsSource = blObject.GetCustomersExeptOne();

            }
        }

        /// <summary>
        /// Ctor display the update / see info a specific parcel Form.
        /// from customer window (as admin.)
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        /// <param name="parcel">The parcel to update / see info</param>
        /// <param name="cameFromPageParcelList">To know were to return back. if ture = parcelList, if false = return to a specific customer </param>
        public ParcelWindow(BlApi.IBl blObject, Parcel parcel, bool cameFromPageParcelList, bool isSender)
        {
            #region = initializeUpdate
            //InitializeComponent();
            //Loaded += ToolWindowLoaded;//The x button
            //this.blObject = blObject;
            //currentParcel = new PO.Parcel(blObject, parcel);
            //AddParcelDisplay.DataContext = currentParcel;
            //isClientAndNotAdmin = false;
            //visibleAddForm.Visibility = Visibility.Hidden;
            //visibleUpdateForm.Visibility = Visibility.Visible;
            //clientIsSender = isSender;
            //initializeCustomers(isSender);
            //setBtns();
            #endregion

            initializeUpdate(blObject, parcel, isSender);
            returnToParcelListWindow = cameFromPageParcelList;
            ConfirmButton.Visibility = Visibility.Hidden;
            visibleUpdatedetails.Visibility = currentParcel.Drone== null ? Visibility.Visible : Visibility.Hidden;

            if (currentParcel.Drone == null)
            {
                ParcelWeightSelector1.ItemsSource = Enum.GetValues(typeof(WeightCategories));
                ParcelPrioritySelector1.ItemsSource = Enum.GetValues(typeof(Priorities));
                ParcelTargetSelector1.ItemsSource = blObject.GetCustomersExeptOne();
            }
        }

        /// <summary>
        /// Ctor display the update / see info a specific parcel Form.
        /// Client!!
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        /// <param name="parcel">The parcel to update / see info </param>
        /// <param name="isSender">to know to witch specific customer to return back. sender / target</param>
        /// <param name="window"></param>
        public ParcelWindow(BlApi.IBl blObject, Parcel parcel, bool isSender, Window window)
        {
            initializeUpdate(blObject, parcel, isSender);
            visibleUpdatedetails.Visibility = currentParcel.Drone == null ? Visibility.Visible : Visibility.Hidden;
            if (currentParcel.Drone == null)
            {
                ParcelWeightSelector1.ItemsSource = Enum.GetValues(typeof(WeightCategories));
                ParcelPrioritySelector1.ItemsSource = Enum.GetValues(typeof(Priorities));
                ParcelTargetSelector1.ItemsSource = blObject.GetCustomersExeptOne();
            }
            isClientAndNotAdmin = true;
            returnBackToUnupdateWindow = window;
        }

        /// <summary>
        /// Initialize update details
        /// </summary>
        /// <param name="blObject"></param>
        /// <param name="parcel"></param>
        /// <param name="isSender"></param>
        private void initializeUpdate(BlApi.IBl blObject, Parcel parcel, bool isSender)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded; //The x button
            this.blObject = blObject;
            currentParcel = new PO.Parcel(blObject, parcel);
            AddParcelDisplay.DataContext = currentParcel;
            returnToParcelListWindow = false;
            visibleAddForm.Visibility = Visibility.Hidden;
            visibleUpdateForm.Visibility = Visibility.Visible;
            clientIsSender = isSender;
            initializeCustomer(clientIsSender);
            setBtns();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSender"></param>
        private void initializeCustomer(bool isSender)
        {
            if (isSender)
                this.clientCustomer = blObject.GetCustomerById(currentParcel.Sender.Id);
            else
                this.clientCustomer = blObject.GetCustomerById(currentParcel.Target.Id);
        }

        /// <summary>
        /// Set buttons
        /// </summary>
        private void setBtns()
        {
            if (currentParcel.Scheduled != null)
            {
                setConfirmBtn();
                initializeDrone();
            }
            setRemoveBtn();
        }

        /// <summary>
        /// Initialize drone
        /// </summary>
        private void initializeDrone()
        {
            BO.Drone parcelDrone = blObject.GetDroneById(currentParcel.Drone.Id);
            if (currentParcel.Drone != null && parcelDrone != null)
            {
                if ((currentParcel.PickUp != null && currentParcel.Delivered == null) && isClientAndNotAdmin
                    || !isClientAndNotAdmin)
                {
                    List<BO.Drone> droneOfParcel = new List<BO.Drone>();//for display
                    droneOfParcel.Add(parcelDrone);
                    DroneListView.ItemsSource = droneOfParcel;
                    DroneText.Visibility = Visibility.Hidden;
                    return;
                }
            }

            ExpenderDroneObj.Visibility = Visibility.Hidden;
            DroneText.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Set remove Btn
        /// </summary>
        private void setRemoveBtn()
        {
            if (!clientIsSender && isClientAndNotAdmin) //Admin could remove a parcel? isClientAndNotAdmin
                return;
            Visibility visibility = Visibility.Visible;
            if (currentParcel.Scheduled == null)
            {
                RemoveBtn.Visibility = Visibility.Visible;
                visibility = Visibility.Hidden;
            }
            else
            {
                RemoveBtn.Visibility = Visibility.Hidden;
            }
            setDroneVisibility(visibility);
        }

        private void setDroneVisibility(Visibility visibility)
        {
            DroneText.Visibility = visibility;
            ExpenderDroneObj.Visibility = visibility;
            DroneLabel.Visibility = visibility;

        }

        private void setConfirmBtn()
        {
            BO.Customer parcelSender = blObject.GetCustomerById(currentParcel.Sender.Id);
            BO.Customer parcelTarget = blObject.GetCustomerById(currentParcel.Target.Id);
            BO.Drone parcelDrone = blObject.GetDroneById(currentParcel.Drone.Id);

            if (currentParcel.Requeasted != null)
            {
                ConfirmButton.Visibility = Visibility.Hidden;
                if (currentParcel.PickUp == null && currentParcel.Scheduled != null)
                {
                    if ((parcelDrone.DronePosition.Latitude == parcelSender.CustomerPosition.Latitude //parcel was pick up now.
                        && parcelDrone.DronePosition.Longitude == parcelSender.CustomerPosition.Longitude)
                        && ((!isClientAndNotAdmin) //Admin
                        || (isClientAndNotAdmin && clientIsSender))) //customer is sender 
                    {
                        ConfirmButton.Visibility = Visibility.Visible;
                        ConfirmButton.Content = "Confirm pickUp";
                        parcelStatus = BO.ParcelStatuses.PickedUp;
                    }
                }
                if (currentParcel.PickUp != null && currentParcel.Delivered == null)
                {
                    if ((parcelDrone.DronePosition.Latitude == parcelTarget.CustomerPosition.Latitude //parcel was delivered now.
                        && parcelDrone.DronePosition.Longitude == parcelTarget.CustomerPosition.Longitude)
                        && ((!isClientAndNotAdmin) //Admin
                        || (isClientAndNotAdmin && !clientIsSender))) //customer is target 
                    {
                        ConfirmButton.Visibility = Visibility.Visible;
                        ConfirmButton.Content = "Confirm delivery";
                        parcelStatus = BO.ParcelStatuses.Delivered;
                    }
                }
            }
        }



        /// <summary>
        /// initialize add form details of parcels' textBoxes.
        /// </summary>
        private void initializeDetailsAddForm()
        {
            ParcelTitle.Content = "Add a Parcel";
            ParcelWeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            ParcelPrioritySelector.ItemsSource = Enum.GetValues(typeof(Priorities));
            ParcelTargetSelector.ItemsSource = blObject.GetCustomersExeptOne();
            ParcelSenderSelector.ItemsSource = blObject.GetCustomersExeptOne();
        }

        /// <summary>
        /// Try sending the parcel to remove.
        /// if isClientAndNotAdmin = true go to CustomerWindow
        /// If Admin go back to parcelListWIndow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeParcelBtnClick(object sender, RoutedEventArgs e)
        {
            if (!clientIsSender && isClientAndNotAdmin)
                return;

            try
            {
                blObject.RemoveParcel(currentParcel.Id);
                customerUpdateHisParcel = true;

                if (returnToParcelListWindow)
                    new ParcelListWindow_(blObject).Show();

                else if (isClientAndNotAdmin && !customerUpdateHisParcel)
                    returnBackToUnupdateWindow.Show();

                else //if (isClientAndNotAdmin)
                    new CustomerWindow(blObject, clientCustomer, true).Show();

                this.Close();
                PLFunctions.messageBoxResponseFromServer("Parcel Remove", "Parcel was removed succesfully");
            }
            catch (Exceptions.ObjNotExistException) { PLFunctions.messageBoxResponseFromServer("Remove Parcel", $"The requested parcel with id {currentParcel.Id} wasn't found"); }
            catch (Exceptions.ObjNotAvailableException e1) { PLFunctions.messageBoxResponseFromServer("Remove Parcel", $"{e1.Message}"); }
            catch (Exception) { PLFunctions.messageBoxResponseFromServer("Remove Parcel", $"Error\nPlease try again"); }
        }

        /// <summary>
        /// Add a parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// addParcelBtnClick
        private void addParcelBtnClick(object sender, RoutedEventArgs e)
        {

            if ((isComboBoxesFieldsFull(ParcelWeightSelector, ParcelPrioritySelector, ParcelSenderSelector, ParcelTargetSelector) && !isClientAndNotAdmin)
                || (isComboBoxesFieldsFull(ParcelWeightSelector, ParcelPrioritySelector, ParcelTargetSelector) && isClientAndNotAdmin))
            {
                CustomerInParcel senderCustomer;
                if (isClientAndNotAdmin)
                    senderCustomer = new CustomerInParcel() { Id = clientCustomer.Id, Name = clientCustomer.Name };
                else
                    senderCustomer = ((CustomerInParcel)ParcelSenderSelector.SelectedItem);
                CustomerInParcel targetCustomer = ((CustomerInParcel)ParcelTargetSelector.SelectedItem);
                WeightCategories weight = (WeightCategories)ParcelWeightSelector.SelectedItem;
                Priorities priority = (Priorities)ParcelPrioritySelector.SelectedItem;
                try
                {
                    blObject.AddParcel(new Parcel() { Sender = senderCustomer, Target = targetCustomer, Weight = weight, Priority = priority });
                    if (isClientAndNotAdmin)
                        new CustomerWindow(blObject, clientCustomer, true).Show();
                    else
                    {
                        new ParcelListWindow_(blObject).Show();
                        this.Close();
                    }
                }

                #region Exceptions
                catch (Exception)
                {
                    PLFunctions.messageBoxResponseFromServer("Add Parcel", "== ERROR receiving data ==\nPlease try again");
                }
                #endregion
            }
            else PLFunctions.messageBoxResponseFromServer("Add a parcel", "Missinig Details");
        }

        /// <summary>
        /// Return true if the comboBoxes' fields are full
        /// </summary>
        /// <param name="comboBoxes">The comboBoxes to check their fields.</param>
        /// <returns></returns>
        private bool isComboBoxesFieldsFull(params ComboBox[] comboBoxes)
        {
            foreach (ComboBox item in comboBoxes)
            {
                if (item.SelectedItem == null) return false;
            }
            return true;
        }

        /// <summary>
        /// return back:
        /// if as a client = CustomerWindow with client privilages
        /// else: parcelListWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (isClientAndNotAdmin)
            {
                new CustomerWindow(blObject, blObject.GetCustomerById(clientCustomer.Id), true).Show();
                this.Close();
            }
            else
            {
                if (returnToParcelListWindow)
                    new ParcelListWindow_(blObject).Show();
                else
                {
                    new CustomerWindow(blObject, blObject.GetCustomerById(clientCustomer.Id), false).Show();
                }
                this.Close();
            }
        }

        /// <summary>
        /// If Admin/worker go to the droneWindow and dispaly the parcels' drone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneClick(object sender, RoutedEventArgs e)
        {
            if (isClientAndNotAdmin)
                return;
            Drone d = blObject.GetDroneById(currentParcel.Drone.Id);
            new DroneWindow(blObject, d).Show();
        }

        /// <summary>
        /// If Admin/worker go to the CustomerWindow and dispaly the parcels' customer (sender / target).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerButton(object sender, RoutedEventArgs e)
        {
            if (isClientAndNotAdmin)
                return;
            CustomerInParcel customerClicked = ((sender as Button).Name == "TargetText") ? currentParcel.Target : currentParcel.Sender;
            Customer customer = blObject.GetCustomerById(customerClicked.Id);
            new CustomerWindow(blObject, customer, false, currentParcel.BO()).Show();
            this.Close();
        }

        private void ParcelCustomerSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void updateParcelInfoBtnClick(object sender, RoutedEventArgs e)
        {
            WeightCategories weight = default;
            if (isComboBoxesFieldsFull(ParcelWeightSelector1))
            {
                weight = (WeightCategories)ParcelWeightSelector1.SelectedItem;
            }
            Priorities priority = default;
            if (isComboBoxesFieldsFull(ParcelPrioritySelector1))
            {
                priority = (Priorities)ParcelPrioritySelector1.SelectedItem;
            }
            CustomerInParcel targetCustomer = default;
            if (isComboBoxesFieldsFull(ParcelTargetSelector1))
            {
                targetCustomer = ((CustomerInParcel)ParcelTargetSelector1.SelectedItem);
                if (targetCustomer.Id == currentParcel.Sender.Id)
                {
                    PLFunctions.messageBoxResponseFromServer("Update Parcel", "Sender and Target are equal.");
                    return;
                }
            }
            try
            {
                blObject.updateParcel(currentParcel.Id, targetCustomer, priority, weight);
                currentParcel.Target = targetCustomer != default ? targetCustomer : currentParcel.Target;
                currentParcel.Priority = targetCustomer != default ? (PO.Priorities)priority : currentParcel.Priority;
                currentParcel.Weight = weight != default ? (PO.WeightCategories)weight : currentParcel.Weight;
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Text of confirm parcel btn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmParcelBtn(object sender, RoutedEventArgs e)
        {
            if (ConfirmButton.Content == "Confirm pickUp")
            {
                blObject.DronePicksUpParcel(currentParcel.Drone.Id);
            }
            else if (parcelStatus == BO.ParcelStatuses.Delivered)
            {
                blObject.DeliveryParcelByDrone(currentParcel.Drone.Id);
            }
            setConfirmBtn();
        }


        /// <summary>
        /// Change background of drone info when there is a click on colla[s to close the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderCollapsedDroneExpender(object sender, RoutedEventArgs e)
        {
            DroneListView.Background = null;
        }

        /// <summary>
        /// Change background of drone info when there is a click on expender to close the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBackGroundExpenderExpandedDroneExpender(object sender, RoutedEventArgs e)
        {
            DroneListView.Background = Brushes.White;
        }

    }
}