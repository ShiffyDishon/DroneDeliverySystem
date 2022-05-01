using System;
using System.ComponentModel;
using System.Windows;
using BO;
using DO;

namespace PL
{
    public partial class DroneWindow : Window
    {
        /// <summary>
        /// BackgroundWorker for simulation
        /// </summary>
        BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// If Simulator is asked to stop but operation is not Completed yet = true/false.
        /// For the progress bar.
        /// </summary>
        bool simIsAskedToStop = false;

        /// <summary>
        /// Enum of drone status in simulation.
        /// </summary>
        public DroneStatusInSim droneCase { get; set; }

        /// <summary>
        /// Saves the distence of drone from it's destination when drone.Status == DroneStatus.Delivery;
        /// </summary>
        public double droneDisFromDes { get; set; }

        /// <summary>
        /// Initialize obj worker for the simolator of drone
        /// And when manual btn is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializeWorker(object sender, RoutedEventArgs e)
        {
            if (isSimulationWorking && simIsAskedToStop) 
                return;

            if (isSimulationWorking)
            {
                simIsAskedToStop = true;
                worker.CancelAsync();
                ProgressBarForSimulation.Visibility = Visibility.Visible;
                return;
            }

            if (!isSimulationWorking)
                worker = new BackgroundWorker();

            droneCase = 0;
            droneDisFromDes = 0;
            AutomationBtn.Content = "Manual";
            changeVisibilityOfUpdateBtn(Visibility.Hidden);
            isSimulationWorking = true;
            worker.DoWork += new DoWorkEventHandler(DoWork); 
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync(worker);
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
        }

        /// <summary>
        /// DoWork of BackgroundWorker worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            blObject.StartSimulation(
                tempDrone, 
                (i, des) =>
                {
                    worker.ReportProgress((int)i);
                    droneCase = i;
                    droneDisFromDes = des;
                },
                () => worker.CancellationPending);
        }

        /// <summary>
        /// RunWorkerCompleted of BackgroundWorker worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.CancelAsync();

            if (isReturnBtnClick)
                this.Close();

            AutomationBtn.Content = "Start Automation";
            ProgressBarForSimulation.Visibility = Visibility.Hidden;
            setChargeBtn();
            setRemoveBtn();
            setDeliveryBtn();
            simIsAskedToStop = false;
            isSimulationWorking = false;
        }

        /// <summary>
        /// ProgressChanged of BackgroundWorker worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            currentDrone.Update(tempDrone);
            blObject.DroneListChangeAction(tempDrone, false, false);
            StatusTextBoxLabelSimulation.Visibility = Visibility.Visible;
            DisDroneFromDes.Visibility = Visibility.Hidden;
            switch (droneCase)
            {
                case DroneStatusInSim.ToPickUp:
                    parcelInDeliveryVisibility(Visibility.Visible);
                    StatusTextBoxLabelSimulation.Content = "Destination\nSender Customer";
                    break;
                case DroneStatusInSim.PickUp:
                    parcelInDeliveryVisibility(Visibility.Visible);
                    StatusTextBoxLabelSimulation.Content = "Picking up parcel";
                    break;
                case DroneStatusInSim.ToDelivery:
                    parcelInDeliveryVisibility(Visibility.Visible);
                    StatusTextBoxLabelSimulation.Content = "Destination\nReceiving Customer";
                    break;
                case DroneStatusInSim.Delivery:
                    parcelInDeliveryVisibility(Visibility.Hidden);
                    StatusTextBoxLabelSimulation.Content = "Delivering parcel";
                    break;
                case DroneStatusInSim.ToCharge:
                    DisDroneFromDes.Visibility = Visibility.Hidden;
                    StatusTextBoxLabelSimulation.Content = "Destination\nStation";
                    break;
                case DroneStatusInSim.NoAvailbleChargingSlots:
                    StatusTextBoxLabelSimulation.Content = "No charging slots";
                    DisDroneFromDes.Visibility = Visibility.Hidden;
                    break;
                case DroneStatusInSim.NotEnoughBatteryForDelivery:
                    StatusTextBoxLabelSimulation.Visibility = Visibility.Hidden;
                    DisDroneFromDes.Visibility = Visibility.Hidden;
                    break;
                case DroneStatusInSim.DisFromDestination:
                    DisDroneFromDes.Visibility = Visibility.Visible;
                    if (droneDisFromDes >= 0)
                        DisDroneFromDes.Content = $"Distance from\ndestination: {Math.Round(droneDisFromDes, 1)}";
                    else
                        DisDroneFromDes.Visibility = Visibility.Hidden;
                    break;
                case DroneStatusInSim.HideTextBlock:
                    StatusTextBoxLabelSimulation.Visibility = Visibility.Hidden;
                    DisDroneFromDes.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Parcel in delivery status visibility
        /// </summary>
        /// <param name="visibility"></param>
        private void parcelInDeliveryVisibility(Visibility visibility)
        {
            ParcelIdIdTextBox.Text = visibility == Visibility.Visible ? $"{currentDrone.ParcelInTransfer.Id}" : "";
            ParcelIdIdTextBox.Visibility = visibility;
            ParcelTextBoxLabel.Visibility = visibility;
        }
    }
}
