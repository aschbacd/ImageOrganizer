using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // worker
        BackgroundWorker worker = new BackgroundWorker();

        // scanned directories
        List<Dir> directories = new List<Dir>();

        // file handler
        FileHandler fileHandler;

        bool IsLoading = false;


        // folders
        string sourceFolder;
        string destinationFolder;
        string errorFolder;

        public MainWindow()
        {
            InitializeComponent();

            // set progress bar
            progressBarScan.Minimum = 0;
            progressBarScan.Maximum = 100;

            // set worker
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.WorkerSupportsCancellation = true;
        }

        private void buttonBrowseSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            // browse source folder
            System.Windows.Forms.FolderBrowserDialog sourceFolder = new System.Windows.Forms.FolderBrowserDialog();
            sourceFolder.ShowDialog();
            textBoxSourceFolder.Text = sourceFolder.SelectedPath;
        }

        private void buttonBrowseDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            // browse destination folder
            System.Windows.Forms.FolderBrowserDialog destinationFolder = new System.Windows.Forms.FolderBrowserDialog();
            destinationFolder.ShowDialog();
            textBoxDestinationFolder.Text = destinationFolder.SelectedPath;
        }

        private void buttonBrowseErrorFolder_Click(object sender, RoutedEventArgs e)
        {
            // browse error folder
            System.Windows.Forms.FolderBrowserDialog errorFolder = new System.Windows.Forms.FolderBrowserDialog();
            errorFolder.ShowDialog();
            textBoxErrorFolder.Text = errorFolder.SelectedPath;
        }

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoading)
            {
                worker.CancelAsync();
            }
            else
            {
                if (Check.CheckIfEmpty(textBoxSourceFolder.Text) && Check.CheckIfEmpty(textBoxDestinationFolder.Text) && Check.CheckIfEmpty(textBoxErrorFolder.Text))
                {
                    if (Check.CheckIfDirExists(textBoxSourceFolder.Text) && Check.CheckIfDirExists(textBoxDestinationFolder.Text) && Check.CheckIfDirExists(textBoxErrorFolder.Text))
                    {
                        // is loading
                        IsLoading = true;

                        listBoxScan.ItemsSource = new List<Dir>();

                        // set worker
                        worker.RunWorkerAsync();

                        // set file handler
                        string duplicationExtra = "none";
                        if (radioButtonChecksumMD5.IsChecked == true)
                            duplicationExtra = "md5";
                        else if (radioButtonChecksumSHA1.IsChecked == true)
                            duplicationExtra = "sha1";
                        else if (radioButtonChecksumSHA256.IsChecked == true)
                            duplicationExtra = "sha256";
                        else if (radioButtonChecksumSHA512.IsChecked == true)
                            duplicationExtra = "sha512";
                        fileHandler = new FileHandler(worker, sourceFolder, destinationFolder, errorFolder, duplicationExtra);

                        // disable input
                        buttonBrowseSourceFolder.IsEnabled = false;
                        buttonBrowseDestinationFolder.IsEnabled = false;
                        buttonBrowseErrorFolder.IsEnabled = false;
                        textBoxSourceFolder.IsEnabled = false;
                        textBoxDestinationFolder.IsEnabled = false;
                        textBoxErrorFolder.IsEnabled = false;
                        buttonStart.IsEnabled = false;
                        buttonScan.Content = "Cancel";

                        // set folders
                        sourceFolder = textBoxSourceFolder.Text;
                        destinationFolder = textBoxDestinationFolder.Text;
                        errorFolder = textBoxErrorFolder.Text;
                    }
                    else
                        MessageBox.Show("Please enter valid path for source, destination and error folder!");
                }
                else
                    MessageBox.Show("Please enter source, destination and error folder!");
            }

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // set progress bar value
            progressBarScan.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0);

            string str = "";

            this.Dispatcher.Invoke(() =>
            {
                str = textBoxSourceFolder.Text;
            });

            directories = fileHandler.Scan(str);

            if (directories.Count == 0)
                worker.ReportProgress(0);
            else
                worker.ReportProgress(100);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBoxScan.ItemsSource = directories;
            labelCount.Content = directories.Count();
            IsLoading = false;
            progressBarScan.Value = 0;

            if (checkBoxAutoStart.IsChecked == true && directories.Count > 0)
            {
                string duplicationExtra = "none";
                if (radioButtonChecksumMD5.IsChecked == true)
                    duplicationExtra = "md5";
                else if (radioButtonChecksumSHA1.IsChecked == true)
                    duplicationExtra = "sha1";
                else if (radioButtonChecksumSHA256.IsChecked == true)
                    duplicationExtra = "sha256";
                else if (radioButtonChecksumSHA512.IsChecked == true)
                    duplicationExtra = "sha512";
                Organizing organizing = new Organizing(sourceFolder, destinationFolder, errorFolder, directories, fileHandler.maximum, duplicationExtra);
                organizing.ShowDialog();
            }

            // enable input
            buttonBrowseSourceFolder.IsEnabled = true;
            buttonBrowseDestinationFolder.IsEnabled = true;
            buttonBrowseErrorFolder.IsEnabled = true;
            textBoxSourceFolder.IsEnabled = true;
            textBoxDestinationFolder.IsEnabled = true;
            textBoxErrorFolder.IsEnabled = true;
            buttonStart.IsEnabled = true;
            buttonScan.Content = "Scan";
            checkBoxAutoStart.IsChecked = false;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (Check.CheckIfEmpty(textBoxSourceFolder.Text) && Check.CheckIfEmpty(textBoxDestinationFolder.Text) && Check.CheckIfEmpty(textBoxErrorFolder.Text))
            {
                if (Check.CheckIfDirExists(textBoxSourceFolder.Text) && Check.CheckIfDirExists(textBoxDestinationFolder.Text) && Check.CheckIfDirExists(textBoxErrorFolder.Text))
                {
                    if (directories.Count > 0)
                    {
                        string duplicationExtra = "none";
                        if (radioButtonChecksumMD5.IsChecked == true)
                            duplicationExtra = "md5";
                        else if (radioButtonChecksumSHA1.IsChecked == true)
                            duplicationExtra = "sha1";
                        else if (radioButtonChecksumSHA256.IsChecked == true)
                            duplicationExtra = "sha256";
                        else if (radioButtonChecksumSHA512.IsChecked == true)
                            duplicationExtra = "sha512";
                        Organizing organizing = new Organizing(sourceFolder, textBoxDestinationFolder.Text, textBoxErrorFolder.Text, directories, fileHandler.maximum, duplicationExtra);
                        organizing.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Please search for files first or use a folder that contains files!");
                    }
                }
                else
                    MessageBox.Show("Please enter valid path for source, destination and error folder!");
            }
            else
                MessageBox.Show("Please enter source, destination and error folder!");
        }

        private void checkBoxAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;
        }

        private void checkBoxAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = true;
        }
    }
}
