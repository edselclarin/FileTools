using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace FileTools
{
    /// <summary>
    /// Creates a backup of the specified list of files at a specified interval, 
    /// and deletes backups that meet the specified expiry.
    /// </summary>
    public class FileBackup
    {
        protected const int threadIntervalMilliseconds = 1000;
        AutoResetEvent exitEvent;
        Thread thread;
        DateTimeOffset lastBackupAt;

        /// <summary>
        /// Backup context.
        /// </summary>
        public FileBackupParameters Parameters { get; set; }

        /// <summary>
        /// FileBackup constructor.
        /// </summary>
        /// <param name="backupParameters">Required for backup.</param>
        public FileBackup(FileBackupParameters backupParameters)
        {
            Parameters = backupParameters;

            if (!Directory.Exists(backupParameters.BackupPath))
                Directory.CreateDirectory(backupParameters.BackupPath);

            exitEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Run the backup routine.
        /// </summary>
        /// <exception cref="System.ArgumentException">description</exception> 
        public void Start()
        {
            if (Parameters.FileList.Count == 0)
                throw new ArgumentException("Need at least one file to create a backup file.");

            Stop();

            thread = new Thread(new ThreadStart(BackupThread));
            thread.Start();
        }

        /// <summary>
        /// Terminates the backup routine.
        /// </summary>
        public void Stop()
        {
            if (thread != null)
            {
                // Signal thread to exit.
                exitEvent.Set();

                // Wait for thread.
                thread.Join();
            }
        }

        void Backup(DateTime backupTime)
        {
            string timestamp = String.Format($"{backupTime.Year}{backupTime.Month:00}{backupTime.Day:00}_{backupTime.Hour:00}{backupTime.Minute:00}{backupTime.Second:00}");
            string backupFile = Path.Combine(Parameters.BackupPath, String.Format($"{Parameters.BackupName}_{timestamp}.zip"));
            using (var zipArch = ZipFile.Open(backupFile, ZipArchiveMode.Create))
            {
                foreach (var file in Parameters.FileList)
                {
                    if (File.Exists(file))
                        zipArch.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }

        void DeleteExpiredBackups()
        {
            var filelist = Directory.EnumerateFiles(Parameters.BackupPath);
            IEnumerable<string> query = from file in filelist
                                        where ((DateTimeOffset.Now - new DateTimeOffset(File.GetCreationTime(file))) >= Parameters.Expiry)
                                        select file;
            foreach (var file in query)
                File.Delete(file);
        }

        void BackupThread()
        {
            lastBackupAt = DateTime.Now;

            while (!exitEvent.WaitOne(threadIntervalMilliseconds))
            {
                var now = DateTimeOffset.Now;
                if ((now - lastBackupAt) >= Parameters.BackupFrequency)
                {
                    Backup(now.DateTime);
                    lastBackupAt = DateTimeOffset.Now;
                }

                DeleteExpiredBackups();
            }
        }
    }
}
