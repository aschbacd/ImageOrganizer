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
using System.Windows.Threading;

namespace ImageOrganizer
{
    /// <summary>
    /// Interaction logic for Organizing.xaml
    /// </summary>
    public partial class Organizing : Window
    {
        BackgroundWorker copyworker = new BackgroundWorker();
        DispatcherTimer timer = new DispatcherTimer();

        DateTime TimerStart;

        string sourceFolder;
        string destinationFolder;
        string errorFolder;

        double filecount;

        bool copyCanceled = false;

        List<Dir> directories = new List<Dir>();

        FileHandler fileHandler;

        public Organizing(string s, string d, string e, List<Dir> dir, double fc)
        {
            InitializeComponent();

            // initialize folders
            sourceFolder = s;
            destinationFolder = d;
            errorFolder = e;

            // filecount
            filecount = fc;
            labelFiles.Content = fc.ToString();

            // set directories
            directories = dir;

            // timer
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            // set worker properties
            copyworker.RunWorkerCompleted += worker_RunWorkerCompleted;
            copyworker.WorkerReportsProgress = true;
            copyworker.DoWork += worker_DoWork;
            copyworker.ProgressChanged += worker_ProgressChanged;
            copyworker.WorkerSupportsCancellation = true;

            // set progress bar
            progressBarCopy.Minimum = 0;
            progressBarCopy.Maximum = 100;

            // when loaded --> start processing
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => {
                // set worker
                copyworker.RunWorkerAsync();
                
            }));

            // start timer
            labelElapsedTime.Content = "00:00:00";
            TimerStart = DateTime.Now;
            timer.Start();

            // set fileHandler
            fileHandler = new FileHandler(copyworker, sourceFolder, destinationFolder, errorFolder);
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            var currentValue = DateTime.Now - this.TimerStart;
            if (currentValue.Days > 0)
                labelElapsedTime.Content = currentValue.Days.ToString("d2") + ":" + currentValue.Hours.ToString("d2") + ":" + currentValue.Minutes.ToString("d2") + ":" + currentValue.Seconds.ToString("d2");
            else
                labelElapsedTime.Content = currentValue.Hours.ToString("d2") + ":" + currentValue.Minutes.ToString("d2") + ":" + currentValue.Seconds.ToString("d2");
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarCopy.Value = e.ProgressPercentage;
            this.Title = e.ProgressPercentage.ToString() + "% Organizing...";
            labelCurrentFile.Content = (string)e.UserState;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var copyworker = sender as BackgroundWorker;
            copyworker.ReportProgress(0);

            fileHandler.StartOrganizing(directories, sourceFolder, destinationFolder, errorFolder, filecount);

            if (copyworker.CancellationPending)
                copyCanceled = true;
            else
                copyworker.ReportProgress(100);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            if (copyCanceled == false)
            {
                var currentValue = DateTime.Now - this.TimerStart;
                if (currentValue.Days > 0)
                    MessageBox.Show("Finished after " + currentValue.Days.ToString("d2") + ":" + currentValue.Hours.ToString("d2") + ":" + currentValue.Minutes.ToString("d2") + ":" + currentValue.Seconds.ToString("d2"));
                else
                    MessageBox.Show("Finished after " + currentValue.Hours.ToString("d2") + ":" + currentValue.Minutes.ToString("d2") + ":" + currentValue.Seconds.ToString("d2"));
            }
            this.Close();
        }

        private void buttonCancelCopy_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Organizing...", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                // yes
                copyworker.CancelAsync();
            }
        }
    }
}
