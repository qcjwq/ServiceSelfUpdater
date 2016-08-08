using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.ServiceProcess;

namespace ServiceUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check for an update
            string updateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Update");
            if (Directory.Exists(updateDirectory) && Directory.GetFileSystemEntries(updateDirectory).Count() > 0)
            {
                foreach (string filepath in Directory.GetFileSystemEntries(updateDirectory))
                {
                    if (Path.GetExtension(filepath) == ".zip")
                    {
                        Console.WriteLine("File is a zip: " + filepath);
                        ServiceController service = new ServiceController("MyWindowsService");
                        try
                        {
                            if (service.Status != ServiceControllerStatus.Stopped && service.Status != ServiceControllerStatus.StopPending)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped);
                            }
                            using (ZipFile zip = new ZipFile(filepath))
                            {
                                zip.ExtractAll(Path.Combine(Directory.GetCurrentDirectory(), @"F:\Projects\BlogPosts\ServiceSelfUpdater\MyWindowsService\bin\Debug\"), 
                                    ExtractExistingFileAction.OverwriteSilently);
                            }
                            string archiveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Archive");
                            if (!Directory.Exists(archiveDirectory)) Directory.CreateDirectory(archiveDirectory);
                            if (File.Exists(Path.Combine(archiveDirectory, Path.GetFileName(filepath)))) File.Delete(Path.Combine(archiveDirectory, Path.GetFileName(filepath)));
                            File.Move(filepath, Path.Combine(archiveDirectory, Path.GetFileName(filepath)));
                            if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                            {
                                service.Start();
                                service.WaitForStatus(ServiceControllerStatus.Running);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
