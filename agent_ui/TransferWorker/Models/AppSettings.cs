using System;
using System.Collections.Generic;

namespace TransferWorker.Models
{
    //public class AppSettings
    //{
    //    public string StorageConnectionString { get; set; }
    //    //public string LocalFolderPath { get; set; }
    //    //public string ContainerName { get; set; }
    //    public int MaxConcurrency { get; set; }
    //    public int Timer { get; set; }
    //    //public int DeleteTimer { get; set; }
    //}
    public class AppJson
    {
        public Settings Settings { get; set; }
        public Logs Logs { get; set; }
        public string IsFirst { get; set; }
        public bool IsRunNow { get; set; }
        public string host { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
        public string subject { get; set; }
        public string port { get; set; }
        public string license { get; set; }
    }

    public class Settings
    {
        public Logging Logging { get; set; }
        public List<AppSetting> AppSettings { get; set; }
        public List<FolderConfig> Folders { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class AppSetting
    {
        public int IdAppSetting { get; set; }
        public string NameAppSetting { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string LastCheck { get; set; }
        public string AccountName { get; set; }
        public string StorageConnectionString { get; set; }
        public string CheckConnectTrue { get; set; }
        public string CheckConnectFalse { get; set; }
    }

    public class FolderConfig
    {
        public string JobName { get; set; }
        public string LocalFolderPath { get; set; }
        public string ContainerName { get; set; }
        public int Id { get; set; }
        public int IdAppSetting { get; set; }
        public int DeleteTimer { get; set; }
        public int CoolTier { get; set; }
        public int ArchiveTier { get; set; }
        public int MaxConcurrency { get; set; }
        public string NameAppSetting { get; set; }
        public int Timer { get; set; }
        public string TimerString { get; set; }
        public string Email { get; set; }
        public bool IsFolder { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string LastRunTime { get; set; }

    }
    public class FolderResult
    {
        public FolderConfig Folder { get; set; }
        public int Result { get; set; }          //0: xóa, 1: sửa
    }

    public class LogContent
    {
        public string Tittle { get; set; }
        public string StatusSuccess { get; set; }
        public string StatusFalse { get; set; }
        public string TimeDisplay { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
    public class Logs
    {
        public List<LogContent> LogContents { get; set; }
    }
}