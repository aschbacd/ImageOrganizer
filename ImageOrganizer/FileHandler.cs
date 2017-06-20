using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageOrganizer
{
    class FileHandler
    {
        // ------------------------------ FIELDS ------------------------------ //
        private BackgroundWorker worker;
        private List<Dir> directories;

        public double maximum;
        private double progress;

        private double error = 0;
        private double duperror = 0;

        private bool cancelOrganizeEnumerate = false;

        string datenow;

        // ------------------------------ CONSTRUCTOR ------------------------------ //
        public FileHandler(BackgroundWorker worker, string s, string d, string e)
        {
            this.worker = worker;

            directories = new List<Dir>();

            maximum = 0;
            progress = 0;

            DateTime now = System.DateTime.Now;
            datenow = now.Year.ToString("d4") + now.Month.ToString("d2") + now.Day.ToString("d2") + "_" + now.Hour.ToString("d2") + now.Minute.ToString("d2") + now.Second.ToString("d2");

            if (Directory.Exists(e + "\\" + "Duplicates_" + datenow) || Directory.Exists(e + "\\" + "Errors_" + datenow))
            {
                datenow += now.Millisecond.ToString("d2");

                if (Directory.Exists(e + "\\" + "Duplicates_" + datenow) || Directory.Exists(e + "\\" + "Errors_" + datenow))
                {
                    datenow += "_";

                    while (Directory.Exists(e + "\\" + "Duplicates_" + datenow) || Directory.Exists(e + "\\" + "Errors_" + datenow))
                    {
                        datenow += new Random().Next(0, 9);
                    }

                }
            }
        }

        // ------------------------------ SCANNER ------------------------------ //
        public List<Dir> Scan(string source)
        {
            EnumerateFiles(source); // enumerate files --> maximum
            Organize(source); // scan files

            if (worker.CancellationPending)
                return new List<Dir>(); // canceled
            else
                return directories; // finished
        }

        // ------------------------------ ORGANIZER ------------------------------ //
        public void Organize(string source)
        {
            if (cancelOrganizeEnumerate == false) // do nothing if canceled
            {
                if (worker.CancellationPending)
                    cancelOrganizeEnumerate = true; // stop working if canceled

                try
                {

                    // directories in this directory
                    for (int i = 0; i < Directory.GetDirectories(source).Length; i++)
                    {
                        Organize(Directory.GetDirectories(source)[i]); // recoursive scan
                    }

                    // just files in this directory
                    if (Directory.GetFiles(source).Length > 0)
                    {
                        directories.Add(new Dir(source, Directory.GetFiles(source).Length));
                        
                        // progress bar change
                        progress += Directory.GetFiles(source).Length; // add filecount to progress

                        double num = (progress / maximum) * 100;
                        int rounded = (int)Math.Ceiling(num);

                        worker.ReportProgress(rounded);
                    }

                }
                catch(UnauthorizedAccessException) { }
            }
        }

        // ------------------------------ ENUMERATOR ------------------------------ //
        public void EnumerateFiles(string source)
        {
            if (cancelOrganizeEnumerate == false) // do nothing if canceled
            {
                if (worker.CancellationPending)
                    cancelOrganizeEnumerate = true; // stop working if canceled

                try
                {

                    // directories in this directory
                    for (int i = 0; i < Directory.GetDirectories(source).Length; i++)
                    {
                        EnumerateFiles(Directory.GetDirectories(source)[i]); // recoursive scan
                    }

                    // just files in this directory
                    if (Directory.GetFiles(source).Length > 0)
                    {
                        maximum += Directory.GetFiles(source).Length; // add filecount to maximum
                    }

                }
                catch (UnauthorizedAccessException) { }
            }
        }

        // ------------------------------ START ORGANIZING ------------------------------ //
        public void StartOrganizing(List<Dir> dir, string s, string d, string e, double filecount)
        {
            maximum = filecount;

            foreach(Dir item in dir)
            {
                if (worker.CancellationPending == false)
                    CopyFiles(item.path, item.count, s, d, e); // copy files
                else
                    break;
            }
        }

        // ------------------------------ COPY FILES ------------------------------ //
        public void CopyFiles(string source, int length, string sourceFolder, string destinationFolder, string errorFolder)
        {
            BitmapMetadata meta = null;

            // organize images
            for (int i = 0; i < length; i++)
            {
                if (worker.CancellationPending == false)
                {

                    string[] pathArr = Directory.GetFiles(source)[i].Split('\\'); // split path
                    string[] fileArr = pathArr.Last().Split('.'); // split filename

                    string fileName = fileArr.First().ToString(); // filename
                    string extension = fileArr.Last().ToString(); // extension

                    try
                    {
                        BitmapSource image = BitmapFrame.Create(new Uri(Directory.GetFiles(source)[i], UriKind.Relative)); // get bitmapmetadata of current image
                        meta = (BitmapMetadata)image.Metadata; // set meta to current image

                        DateTime date = DateTime.Parse(meta.DateTaken); // get date

                        // --------------- PROCESS DATE --------------- //
                        string year = date.Year.ToString("d4");
                        string month = date.Month.ToString("d2");
                        string day = date.Day.ToString("d2");
                        string hour = date.Hour.ToString("d2");
                        string minute = date.Minute.ToString("d2");
                        string second = date.Second.ToString("d2");

                        // --------------- FINAL PAHTS --------------- //
                        string finalDestPath = destinationFolder + "\\" + year + "\\" + month + " - " + date.ToString("MMMMMMMMMM"); // final path
                        string finalFileName = "IMG_" + year + month + day + "_" + hour + minute + second + "." + extension; // final filename

                        // --------------- COPYING --------------- //
                        string sourceFile = System.IO.Path.Combine(source, fileName + "." + extension);
                        string destFile = System.IO.Path.Combine(finalDestPath, finalFileName);

                        if (!System.IO.Directory.Exists(finalDestPath))
                            System.IO.Directory.CreateDirectory(finalDestPath);

                        if (!System.IO.File.Exists(destFile))
                            System.IO.File.Copy(sourceFile, destFile, true);

                        // --------------- DUPLICATES --------------- //
                        else
                        {
                            string errorPathDuplicate = errorFolder + "\\" + "Duplicates_" + datenow;

                            if (!System.IO.Directory.Exists(errorPathDuplicate))
                                System.IO.Directory.CreateDirectory(errorPathDuplicate);

                            string finalDestPath2 = errorPathDuplicate + "\\" + date.Year + "\\" + month + " - " + date.ToString("MMMMMMMMMM");
                            string finalFileName2 = "duplication_error_" + duperror + "_" + finalFileName;

                            duperror++;

                            string sourceFile2 = System.IO.Path.Combine(source, fileName + "." + extension);
                            string destFile2 = System.IO.Path.Combine(finalDestPath2, finalFileName2);

                            if (!System.IO.Directory.Exists(finalDestPath2))
                                System.IO.Directory.CreateDirectory(finalDestPath2);

                            System.IO.File.Copy(sourceFile2, destFile2, true);
                        }

                        // report progress --> progress bar
                        progress++;
                        double dnum = (progress / maximum) * 100;
                        int inum = (int)Math.Ceiling(dnum);
                        worker.ReportProgress(inum, sourceFile);
                    }
                    catch
                    {
                        ErrorHandling(Directory.GetFiles(source)[i], errorFolder);
                    }

                }
                else
                    break;

            }
        }

        // ------------------------------ ERROR HANDLING ------------------------------ //
        public void ErrorHandling(string source, string e)
        {
            string errorFolder = e + "\\" + "Errors_" + datenow;

            string[] pathArr = source.Split('\\'); // split path
            string[] fileArr = pathArr.Last().Split('.'); // split filename
            
            string fileName = fileArr.First().ToString(); // filename
            string extension = fileArr.Last().ToString(); // extension


            // --------------- COPYING --------------- //
            string errorFilename = "error_" + error + "." + extension;

            error++;

            if (!System.IO.Directory.Exists(errorFolder))
                System.IO.Directory.CreateDirectory(errorFolder);

            string errorPath = System.IO.Path.Combine(errorFolder, errorFilename);

            System.IO.File.Copy(source, errorPath, true);

            // report progress --> progress bar
            progress++;
            double dnum = (progress / maximum) * 100;
            int inum = (int)Math.Ceiling(dnum);
            worker.ReportProgress(inum, source);
        }
    }
}
