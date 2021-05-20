using System;
using System.Collections.Generic;

namespace TransferWorker.UI.Models
{
    public class AppJson
    {
        public Settings Settings { get; set; }
        // public List<LogContent> LogContents { get; set; }
        public int IsFirst { get; set; }
        public bool IsRunNow { get; set; }
        public string email_loggin { get; set; }
        public int id_agent { get; set; }

    }
    public class Historys
    {
        public History history { get; set; }
    }
    public class History
    {
        public List<History_ByteSave> history_bytesave { get; set; }
    }
    public class History_ByteSave
    {
        public string log_content { get; set; }
        public float time_log { get; set; }
        public string function { get; set; }
        public int status { get; set; }
    }
    public class Settings
    {
        public Logging Logging { get; set; }
        public sets_bytesave bytesave_setting { get; set; }
        public Information bytesave_info { get; set; }
        public List<connect_bytesave> connect_bytesaves { get; set; }
        public List<backup_bytesave> backup_bytesaves { get; set; }
    }

    public class Information
    {
       public Info information { get; set; }
    }
    public class Info
    {
        public string name_version { get; set; }
        public int bytesave_expiration_date { get; set; }
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
        public string Time { get; set; }
        public string Content { get; set; }
    }
    public class Logs
    {
        public List<LogContent> LogContents { get; set; }
    }
    public class Companys
    {
        public string MACAddress { get; set; }
        public string License { get; set; }
        public string ExpirationTime { get; set; }
        public int SoLuong { get; set; }
        public int DaDung { get; set; }
    }

    public class backup_bytesave
    {
        public int id { get; set; }
        public int id_agent { get; set; }
        public int id_connect_bytesave { get; set; }
        public string name { get; set; }
        public string local_path { get; set; }
        public string container_name { get; set; }
        public string time { get; set; }
        public int time_delete { get; set; }
        public int time_delete_file_in_LastVersion { get; set; }
        public string connect_bytesave_name { get; set; }
        public string connect_bytesave_username_account { get; set; }
        public string email { get; set; }
        public int is_folder { get; set; }
        public int time_create_at { get; set; }
        public int time_update_at { get; set; }
    }
    public class connect_bytesave
    {
        public int id { get; set; }
        public int id_agent { get; set; }
        public int id_metric_service { get; set; }
        public string metric_service_max_storage { get; set; }
        public string metric_service_information_connect { get; set; }
        public string metric_service_username_account { get; set; }
        public int time_check_at { get; set; }
        public string name { get; set; }
        public string type_text { get; set; }
        public int type { get; set; }
        public int time_create_at { get; set; }
        public int time_update_at { get; set; }
    }
    public class sets_bytesave
    {
        public setting_bytesave set_bytesave { get; set; }
    }
    public class setting_bytesave
    {
        public int id { get; set; }
        public int id_agent { get; set; }
        public string server_mail { get; set; }
        public string port { get; set; }
        public string mail_send { get; set; }
        public string mail_send_pwd { get; set; }
        public string subject { get; set; }
        public int is_ssl { get; set; }
        public string time_create_at { get; set; }
        public string time_update_at { get; set; }
    }
    public class log_agent_bytesave
    {
        public string serial_number { get; set; }
        public string customer_name { get; set; }
        public string log_content { get; set; }
        public string function { get; set; }
        public string time_log { get; set; }
        public string type { get; set; }
        public string status { get; set; }
    }
    public class info_bytesave
    {
        public string item { get; set; }
        public string name_version { get; set; }
        public int bytesave_expiration_date { get; set; }
    }
    public class json_backup_bytesave
    {
        public string status { get; set; }
        public int countdata { get; set; }
        public string error { get; set; }
        public List<backup_bytesave> data { get; set; }
    }
    public class json_connect_bytesave
    {
        public string status { get; set; }
        public int countdata { get; set; }
        public List<connect_bytesave> data { get; set; }
    }
    public class json_setting_bytesave
    {
        public string status { get; set; }
        public int countdata { get; set; }
        public List<setting_bytesave> data { get; set; }
    }
    public class json_info_bytesave
    {
        public string status { get; set; }
        public int countdata { get; set; }
        public List<Info> data { get; set; }
    }
    public class json_log_bytesave
    {
        public List<log_agent_bytesave> data { get; set; }
    }
    
}