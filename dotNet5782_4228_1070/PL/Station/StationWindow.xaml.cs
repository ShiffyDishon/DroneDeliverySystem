using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class StationWindow : Window
    {
        /// <summary>
        /// Instance of IBl interface.
        /// </summary>
        private BlApi.IBl blObject;

        /// <summary>
        /// Current Statopn
        /// </summary>
        PO.Station currentStation;

        /// <summary>
        /// Delivery btn content
        /// </summary>
        string[] deliveryButtonOptionalContent = { "Send To Delivery", "Pick Up Parcel", "Which Package Delivery" };

        #region the closing button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        /// <summary>
        /// Ctor display the add a station Form
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        public StationWindow(BlApi.IBl blObject)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded;//The x button
            this.blObject = blObject;
            currentStation = new PO.Station();
            visibleAddForm.Visibility = Visibility.Visible;
            visibleUpdateForm.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Ctor display the update/see info a specific station Form.
        /// </summary>
        /// <param name="blObject">Instance of interface Ibl</param>
        /// <param name="station">The station to update/see info</param>
        public StationWindow(BlApi.IBl blObject, BO.Station station)
        {
            InitializeComponent();
            Loaded += ToolWindowLoaded; 
            this.blObject = blObject;
            currentStation = new PO.Station(station);
            AddDroneDisplay.DataContext = currentStation;
            if (station.DronesCharging.Count() == 0)
                ChargingDronesInStationListView.Visibility = Visibility.Hidden;
            else
                NoDronesInCharge.Visibility = Visibility.Hidden;
            visibleAddForm.Visibility = Visibility.Hidden;
            visibleUpdateForm.Visibility = Visibility.Visible;
        }

        #region ToolWindowLoaded
        /// <summary>
        /// Code to remove close box from window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion

        /// <summary>
        /// Add a station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addStationBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.Station newStation = new BO.Station()
                {
                    Id = int.Parse(IdTextBox.Text),
                    Name = NameTextBox.Text,
                    DroneChargeAvailble = int.Parse(ChargingSlotsTextBox.Text),
                    StationPosition = new BO.Position()
                    {
                        Latitude = double.Parse(StationLatitudeTextBox.Text),
                        Longitude = double.Parse(StationLongitudeTextBox.Text)
                    }
                };
                try
                {
                    blObject.AddStation(newStation);
                }
                catch (Exceptions.DataChanged e1) { PLFunctions.messageBoxResponseFromServer("Add a Station", e1.Message); }

                new StationListWindow(blObject).Show();
                this.Close();
            }

            #region catch exeptions
            catch (BO.Exceptions.ObjExistException e1)
            {
                PLFunctions.messageBoxResponseFromServer("Add a Station", e1.Message);
            }
            //catch (ArgumentNullException)
            //{
            //    MessageBox.Show("== ERROR receiving data ==\nPlease try again");
            //}
            //catch (FormatException)
            //{
            //    MessageBox.Show("== ERROR receiving data ==\nPlease try again");
            //}
            //catch (OverflowException)
            //{
            //    MessageBox.Show("== ERROR receiving data ==\nPlease try again");
            //}
            //catch (NullReferenceException)
            //{
            //    MessageBox.Show("== ERROR receiving data ==\nPlease try again");
            //}
            catch (Exception)
            {
                PLFunctions.messageBoxResponseFromServer("Cann't add a station", "== ERROR receiving data ==\nPlease try again");
            }
            #endregion 
        }

        /// <summary>
        /// Reastart = clear form text boxes from text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickRestart(object sender, RoutedEventArgs e)
        {
            PLFunctions.ClearFormTextBox(IdTextBox, NameTextBox,ChargingSlotsTextBox, StationLatitudeTextBox, StationLongitudeTextBox);
        }

        /// <summary>
        /// Return to StationListWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickReturnToPageStationListWindow(object sender, RoutedEventArgs e)
        {
            new StationListWindow(blObject).Show();
            this.Close();
        }

        private void DroneChargeSelection(object sender, MouseButtonEventArgs e)
        {
            ChargingDrone chargingDrone = ((ChargingDrone)ChargingDronesInStationListView.SelectedItem);
            Drone drone = blObject.GetDroneById(chargingDrone.Id);
            new DroneWindow(blObject, drone).Show();
            this.Close();
        }

        /// <summary>
        /// Try to send a stations update info to a func.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                blObject.changeStationInfo(currentStation.Id, NameTextBox.Text, int.Parse(ChargingSlotsAvailbleTextBox.Text));
                new StationListWindow(blObject).Show();
                this.Close();
            }
            catch (Exceptions.ObjNotAvailableException ee) { PLFunctions.messageBoxResponseFromServer("Change Station information", ee.Message); }
            catch (Exception e1) { PLFunctions.messageBoxResponseFromServer("Change Station information", e1.Message); }
        }


        /// <summary>
        /// Try to send the station to be removed = not active.
        /// Occurding to instuctions the station will be removed and no drones can be sent.
        /// The charging Drones will be able to stay till they are freed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                blObject.RemoveStation(currentStation.BO());
                new StationListWindow(blObject).Show();
                this.Close();
            }
            catch (BO.Exceptions.ObjExistException e1)
            {
                PLFunctions.messageBoxResponseFromServer("Remove Station", e1.Message);
            }
            catch (BO.Exceptions.ObjNotAvailableException e2)
            {
                PLFunctions.messageBoxResponseFromServer("Remove Station", e2.Message);
            }
        }
        
        
        #region TextBox OnlyNumbers PreviewKeyDown function
        private void TextBox_OnlyNumbers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PLFunctions.TextBox_OnlyNumbers_PreviewKeyDown(sender, e);
        }
        #endregion
    }
}
