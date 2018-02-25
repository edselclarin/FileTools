using System;
using System.Collections.Generic;
using System.IO;

namespace FileTools
{
    /// <summary>
    /// Represents the parameters needed for backing up file(s).
    /// </summary>
    public class FileBackupParameters
    {
        /// <summary>
        /// List of files to backup.
        /// </summary>
        public List<string> FileList { get; set; } = new List<string>();
        /// <summary>
        /// Where to save the backup file.
        /// </summary>
        public string BackupPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Backup");
        /// <summary>
        /// How often to create a backup.
        /// </summary>
        public TimeSpan BackupFrequency { get; set; } = TimeSpan.FromDays(1);
        /// <summary>
        /// Prefix for the backup filename.
        /// </summary>
        public string BackupName { get; set; } = "Backup";
        /// <summary>
        /// How long backup files will live before being deleted.
        /// </summary>
        public TimeSpan Expiry { get; set; } = TimeSpan.FromDays(30);
    }
}
