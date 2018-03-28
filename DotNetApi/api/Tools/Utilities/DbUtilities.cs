using System;
using System.Configuration;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Diagnostics;

namespace Tools.Utilities
{
    public class DbUtilities
    {
        public static bool Backup(string dbName)
        {
            var sourceServerName = ConfigurationManager.AppSettings["DbSourceServer"];
            EventLog.WriteEntry("PPSM", sourceServerName, EventLogEntryType.Error);
            var sourceServerUserName = ConfigurationManager.AppSettings["DbSourceServerSQLLogin"];
            EventLog.WriteEntry("PPSM", sourceServerUserName, EventLogEntryType.Error);
            var sourceServerPassword = Cryptography.Decrypt(ConfigurationManager.AppSettings["DbSourceServerSQLPassword"], true);
            EventLog.WriteEntry("PPSM", sourceServerPassword, EventLogEntryType.Error);
            var sourceDbName = dbName;
            EventLog.WriteEntry("PPSM", dbName, EventLogEntryType.Error);
            var backUpName = sourceDbName + "_backup_" + DateUtilities.GetDateStamp(DateTime.Now, true) + ".bak";

            EventLog.WriteEntry("PPSM", backUpName, EventLogEntryType.Error);

            var backUpDestinationPath = ConfigurationManager.AppSettings["DbBackupPath"];
            //Create backup directory if does not exist
            if (!Directory.Exists(backUpDestinationPath)) Directory.CreateDirectory(backUpDestinationPath);

            backUpDestinationPath += backUpName;

            EventLog.WriteEntry("PPSM", backUpDestinationPath, EventLogEntryType.Error);

            var restoreSourcePath = ConfigurationManager.AppSettings["DbRestorePath"];
            //Create backup directory if does not exist
            if (!Directory.Exists(restoreSourcePath)) Directory.CreateDirectory(restoreSourcePath);

            restoreSourcePath += backUpName;

            EventLog.WriteEntry("PPSM", restoreSourcePath, EventLogEntryType.Error);

            var sqlBackup = new Backup
            {
                Action = BackupActionType.Database,
                BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString(),
                BackupSetName = "Archive",
                Database = sourceDbName
            };


            var deviceItem = new BackupDeviceItem(backUpDestinationPath, DeviceType.File);
            var sourceConn = new ServerConnection(sourceServerName, sourceServerUserName, sourceServerPassword);
            var sourceServer = new Server(sourceConn);

            EventLog.WriteEntry("PPSM", sourceConn.ConnectionString, EventLogEntryType.Error);

            sqlBackup.Initialize = true;
            sqlBackup.Checksum = true;
            sqlBackup.ContinueAfterError = true;

            sqlBackup.Devices.Add(deviceItem);
            sqlBackup.Incremental = false;

            EventLog.WriteEntry("PPSM", "4", EventLogEntryType.Error);

            sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
            sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;
            sqlBackup.FormatMedia = false;

            EventLog.WriteEntry("PPSM", "5", EventLogEntryType.Error);

            sqlBackup.SqlBackup(sourceServer);

            EventLog.WriteEntry("PPSM", "6", EventLogEntryType.Error);

            if (backUpDestinationPath != restoreSourcePath)
            { File.Copy(backUpDestinationPath, restoreSourcePath, true); }

            return true;
        }

        public static bool Restore(string dbName, string backUpSet)
        {
            var destServerName = ConfigurationManager.AppSettings["DbDestinationServer"];
            var destServerUserName = ConfigurationManager.AppSettings["DbDestinationServerSQLLogin"];
            var destServerPassword = Cryptography.Decrypt(ConfigurationManager.AppSettings["DbDestinationServerSQLPassword"], true);
            var restoreSourcePath = ConfigurationManager.AppSettings["DbRestorePath"];
            var dbLocalPath = ConfigurationManager.AppSettings["DbLocalPath"];

            var destDbName = dbName;

            var backUpSourcePath = backUpSet;

            var sqlRestore = new Restore();

            var deviceItem = new BackupDeviceItem(restoreSourcePath + backUpSourcePath, DeviceType.File);
            sqlRestore.Devices.Add(deviceItem);
            sqlRestore.Database = destDbName;

            var connection = new ServerConnection(destServerName, destServerUserName, destServerPassword);
            var sqlServer = new Server(connection);

            sqlRestore.Action = RestoreActionType.Database;
            var dataFileLocation = dbLocalPath + destDbName + "_Data.mdf";
            var logFileLocation = dbLocalPath + destDbName + "_Log.ldf";
            //var rf = new RelocateFile(destDbName, dataFileLocation);

            sqlRestore.RelocateFiles.Add(new RelocateFile(destDbName + "_Data", dataFileLocation));
            sqlRestore.RelocateFiles.Add(new RelocateFile(destDbName + "_log", logFileLocation));
            sqlRestore.ReplaceDatabase = true;

            sqlServer.Databases[dbName].ExecuteNonQuery("alter database " + dbName + " set single_user with rollback immediate");
            sqlRestore.SqlRestore(sqlServer);
            var db = sqlServer.Databases[destDbName];
            db.SetOnline();
            sqlServer.Databases[dbName].ExecuteNonQuery("alter database " + dbName + " set multi_user");
            sqlServer.Databases[dbName].ExecuteNonQuery("Exec PostCopyUAT");

            File.Delete(restoreSourcePath + backUpSourcePath);

            sqlServer.Refresh();
            return true;
        }
    }
}