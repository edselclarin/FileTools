using System;
using System.IO;
using FileTools;

namespace FileToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Press any key to exit.");

                var fileBackup = new FileBackup(new FileBackupParameters
                {
                    BackupPath = Path.Combine(Environment.CurrentDirectory, "Backup"),
                    BackupFrequency = TimeSpan.FromSeconds(2),
                    BackupName = "BACKUP",
                    Expiry = TimeSpan.FromSeconds(30)
                });

                fileBackup.Parameters.FileList.Add("file1.txt");
                fileBackup.Parameters.FileList.Add("file2.txt");
                fileBackup.Parameters.FileList.Add("file3.txt");

                fileBackup.Start();

                Console.ReadKey(true);

                fileBackup.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
