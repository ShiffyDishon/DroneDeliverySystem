using System;
using System.Collections.Generic;
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
using BlApi;
using System.Collections.ObjectModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>

    public partial class DroneListWindow : Window
    {
        /// <summary>
        /// Instance of IBl interface.
        /// </summary>
        private IBl blObjectH;

        /// <summary>
        /// Current drone list
        /// </summary>
        private PO.Drones currentDroneList;

        #region the closing button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion
        public DroneListWindow(IBl blObject)
        {
            InitializeComponent();
            blObjectH = blObject;
            Loaded += ToolWindowLoaded;//The x button
            currentDroneList = new PO.Drones(blObjectH);
            DroneListView.DataContext = currentDroneList.DroneList;
            currentDroneList.getNewList(blObjectH.GetDronesToList());
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatus));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            ChosenStatus.Visibility = Visibility.Hidden;
            ChosenWeight.Visibility = Visibility.Hidden;
        }

        /// <summary>
        // Code to remove close box from window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        /// <summary>
        /// Display DroneToList occurding to both conditions: 
        /// StatusSelector.SelectedItem and WeightSelector.SelectedItem;
        /// if they are null = -1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusSelectorANDWeightSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            object status = StatusSelector.SelectedItem;
            object weight = WeightSelector.SelectedItem;
            if (weight != null)
            {
                weight = WeightSelector.SelectedItem;
                ChosenWeight.Visibility = Visibility.Visible;
                ChosenWeightText.Text = WeightSelector.SelectedItem.ToString();
            }
            else
            {
                weight = -1;
                ChosenWeight.Visibility = Visibility.Hidden;
            }
            if (status != null)
            {
                status = StatusSelector.SelectedItem;
                ChosenStatus.Visibility = Visibility.Visible;
                ChosenStatusText.Text = StatusSelector.SelectedItem.ToString();
            }
            else
            {
                status = -1;
                ChosenStatus.Visibility = Visibility.Hidden;
            }

            IEnumerable<DroneToList> b = blObjectH.GetDronesByConditions((int)weight, (int)status);
            currentDroneList.getNewList(b);
        }

        /// <summary>
        /// Close all windows that were open (drones).
        /// Open MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                App.Current.Windows[intCounter].Close();

            new MainWindow(blObjectH).Show();
            this.Close();
        }

        /// <summary>
        /// Open Add A drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDroneButtonClick(object sender, RoutedEventArgs e)
        {
            new DroneWindow(blObjectH).Show();
        }

        /// <summary>
        /// Open specific drone Window by selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneSelection(object sender, MouseButtonEventArgs e)
        {
            PO.DroneToList droneToList = (PO.DroneToList)DroneListView.SelectedItem;
            Drone drone = blObjectH.GetDroneById(droneToList.Id);
            new DroneWindow(blObjectH, drone).Show();
        }

        /// <summary>
        /// Clear status of drone selection to null
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeStatusToNull(object sender, MouseButtonEventArgs e)
        {
            StatusSelector.SelectedItem = null;
            ChosenStatus.Visibility = Visibility.Hidden;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatus));
        }

        /// <summary>
        /// Change status of drone slection to null
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeWeightToNull(object sender, MouseButtonEventArgs e)
        {
            WeightSelector.SelectedItem = null;
            ChosenWeight.Visibility = Visibility.Hidden;
            WeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }
    }
}