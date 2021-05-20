using Azure.Storage.Blobs;
using NetCore.Utils.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TransferWorker.UI.Utility
{
    public class MainUtility
    {
        private string filePath = @"C:\Program Files (x86)\ByteSave\ByteSave\appsettings.json";
        private string localFilePath = "";
        private decimal kk = 0;

        public AppJson LoadConfig()
        {
            try
            {
                //filePath = GetPath("aPathJsonLocal", 0);
                filePath = Path.GetFullPath("appsettings.json");
                var str = File.ReadAllText(filePath);
                var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
                return configs;
            }
            catch (Exception ex)
            {
                new MainUtility().save_log_agent(ex.ToString(), "MainWindowModel", 0, 0, "");
                return new AppJson();
            }

        }
        public void WriteConfig(Settings _settings)
        {
            filePath = Path.GetFullPath("appsettings.json");
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            configs.Settings = _settings;
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
        }
        public Historys LoadHistory()
        {
            try
            {
                //filePath = GetPath("aPathJsonLocal", 0);
                filePath = Path.GetFullPath("historys.json");
                var str = File.ReadAllText(filePath);
                var configs = System.Text.Json.JsonSerializer.Deserialize<Historys>(str);
                return configs;
            }
            catch (Exception)
            {
                return new Historys();
            }
        }
        public void WriteHistory(History _historys)
        {
            filePath = Path.GetFullPath("historys.json");
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<Historys>(str);
            configs.history = _historys;
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
        }
        public void AddHistory(string function, string log_content, int status)
        {
            var historys = LoadHistory();
            int date_now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            historys.history.history_bytesave.Add(
                new History_ByteSave
                {
                    function = function,
                    //Time = DateTime.Now,
                    time_log = date_now,
                    log_content = log_content,
                    status = status,
                }
            );
            WriteHistory(historys.history);
        }
        public void write_email(string email)
        {
            filePath = Path.GetFullPath("appsettings.json");
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            configs.email_loggin = email;
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
        }
        public void write_logout()
        {
            filePath = Path.GetFullPath("appsettings.json");
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            List<connect_bytesave> lst = new List<connect_bytesave>();
            List<backup_bytesave> lst_backup = new List<backup_bytesave>();
            var info = new Info();
            var setting = new setting_bytesave();
            configs.email_loggin = "";
            configs.Settings.backup_bytesaves = lst_backup;
            configs.Settings.connect_bytesaves = lst;
            configs.Settings.bytesave_info.information = info;
            configs.Settings.bytesave_setting.set_bytesave = setting;
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
        }
        public void WriteConfig_Loggin()
        {
            var configs = new AppJson();
            try
            {
                filePath = Path.GetFullPath("appsettings.json");
                var str = File.ReadAllText(filePath);
                configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
                get_info_to_server(configs);
                get_list_data_backup(configs);
                //var ex_lst_backup = list_backup.data.Except(configs.Settings.backup_bytesaves).ToList();

                get_setting_to_server(configs);
                get_list_data_connect(configs);



            }
            catch (Exception ex)
            {
                new MainUtility().save_log_agent(ex.ToString(), "MainWindowModel", 0, 0, configs.email_loggin);
            }
        }

        //public Logs LoadLog()
        //{
        //    filePath = GetPath("aPathJsonLocal", 0);
        //    var stream = File.Open(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        //    if (stream != null)
        //    {
        //        var str = File.ReadAllText(filePath);
        //        var configs = JsonSerializer.Deserialize<AppJson>(str);
        //        stream.Dispose();
        //        stream.Close();
        //        return configs.Logs;

        //    }
        //    stream.Dispose();
        //    stream.Close();
        //    return null;

        //}

        public AppJson LoadServerMail()
        {
            filePath = GetPath("aPathJsonLocal", 0);
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            return configs;
        }
        public string GetPath(string fileName, int type)
        {
            var content = string.Empty;
            try
            {
                localFilePath = Path.GetFullPath(fileName + ".txt");
                string contents = File.ReadAllText(localFilePath);
                return contents;
            }
            catch (Exception ex)
            {
                File.WriteAllText(fileName + ".txt", filePath);
                string contents = File.ReadAllText(localFilePath);
                return contents;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tittle">Tiêu đề</param>
        /// <param name="content">Nội dung</param>
        /// <param name="status">Trạng thái: 0: false  1: Success</param>
        public void AddLog(string tittle, string content, int status)
        {
            //var _logs = LoadConfig().Logs;
            //_logs.LogContents.Add(
            //    new LogContent
            //    {
            //        Tittle = tittle,
            //        //Time = DateTime.Now,
            //        TimeDisplay = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            //        Content = content,
            //        StatusSuccess = status == 1 ? "Visible" : "Hidden",
            //        StatusFalse = status == 1 ? "Hidden" : "Visible"
            //    }
            //);
            //WriteLog(_logs);
        }

        //public void WriteLog(Logs LogContents)
        //{
        //    filePath = GetPath("aPathJsonLocal", 0);
        //    var str = File.ReadAllText(filePath);
        //    var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
        //    configs.Logs = LogContents;
        //    File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Content">License or Isfirst</param>
        /// <param name="IsCheck">0:License --  1: Isfirst</param>
        public void WriteAppjson(string Content, int IsCheck)
        {
            //System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //{
            //    filePath = GetPath("aPathJsonLocal", 0);
            //    var str = File.ReadAllText(filePath);
            //    var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            //    if (IsCheck == 0)
            //    {
            //        configs.license = Content;
            //    }
            //    else configs.IsFirst = Content;
            //    File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
            //});
        }
        public void WriteServerMail(string Host, string Port, string Email, string Pwd, string Subject)
        {
            try
            {
                //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                //{
                //    filePath = GetPath("aPathJsonLocal", 0);
                //    var str = File.ReadAllText(filePath);
                //    var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
                //    configs.host = Host;
                //    configs.port = Port;
                //    configs.email = Email;
                //    configs.pwd = crypt(Pwd);
                //    configs.subject = Subject;
                //    File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
                //});
            }
            catch (Exception ex)
            {
                NLogManager.LogError("Error Add mail:  " + ex);
            }

        }
        //public void WriteLog(Logs _logs)
        //{
        //    filePath = GetPath("aPathJsonLocal", 1);
        //    var str = File.ReadAllText(filePath);
        //    var configs = JsonSerializer.Deserialize<AppJson>(str);
        //    configs.Logs = _logs;
        //    File.WriteAllText(filePath, JsonSerializer.Serialize(configs));
        //}

        public async Task<List<SizeFile>> GetListFileFromCloud(string storageConnectionString)
        {
            var cloudFiles = new List<SizeFile>();
            var containers = await AzureBlobHelper.ListContainersAsync(storageConnectionString);
            foreach (var item in containers)
            {
                var blobContainer = GetBlobContainerV2(item.Name, storageConnectionString).GetBlobsAsync();
                await foreach (var blobItem in blobContainer)
                {
                    var fsi = new SizeFile
                    {
                        Lenght = blobItem.Properties.ContentLength
                    };
                    cloudFiles.Add(fsi);
                }
            }

            return cloudFiles;
        }
        public async Task GetUseLevelAsync(string storageConnectionString)
        {
            var cloudFiles = await GetListFileFromCloud(storageConnectionString);
            decimal cum = decimal.Parse(cloudFiles.Sum(x => x.Lenght).ToString());
            kk = cum;
        }

        private BlobContainerClient GetBlobContainerV2(string nameContainer, string storageConnectionString)
        {
            var container = new BlobContainerClient(storageConnectionString, nameContainer);
            return container;
        }
        static string key { get; set; } = "khanh.trieu";
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public string crypt(string code)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(code);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length) + "tsg";
                    }
                }
            }
        }
        public string DecryptGenLicense(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        try
                        {
                            byte[] cipherBytes = Convert.FromBase64String(cipher.Substring(0, cipher.Length - 3));
                            byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                            return UTF8Encoding.UTF8.GetString(bytes);
                        }
                        catch (Exception ex)
                        {
                            return "";
                        }
                    }
                }
            }
        }
        public string GetMac()
        {
            string addr = "";
            foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (n.OperationalStatus == OperationalStatus.Up)
                {
                    addr += n.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return addr;
        }
        public int GetHash(string Textstring)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] messageBytes = ue.GetBytes(Textstring);
            //Create a new instance of the SHA256 class to create
            //the hash value.
            SHA256 shHash = SHA256.Create();
            //Create the hash value from the array of bytes.
            var hashvalue = shHash.ComputeHash(messageBytes);
            return BitConverter.ToInt32(hashvalue, 0);
        }
        public string Get_Serial_number()
        {
            string serial_number = "";
            try
            {
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
                ManagementObjectCollection collection = mSearcher.Get();
                foreach (ManagementObject obj in collection)
                {
                    serial_number = (string)obj["SerialNumber"];
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return serial_number;
        }
        public async Task get_list_data_backup(AppJson configs)
        {
            string api_backup_list = System.Configuration.ConfigurationSettings.AppSettings["api_backup_list"];
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
            try
            {
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_backup_list + Get_Serial_number() + "/" + configs.email_loggin);
                WebReq.Method = "GET";
                WebReq.Timeout = 10000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    dynamic d = JsonConvert.DeserializeObject(jsonString);
                    json_backup_bytesave json_backup_bytesaves = d.ToObject<json_backup_bytesave>();
                    //var msg = json["msg"].ToString();
                    if (json_backup_bytesaves.status == "true")
                    {
                        List<backup_bytesave> lst_backup = new List<backup_bytesave>();
                        if (json_backup_bytesaves.countdata > 0)
                        {
                            foreach (var item in json_backup_bytesaves.data)
                            {
                                lst_backup.Add(new backup_bytesave
                                {
                                    id = item.id,
                                    id_agent = item.id_agent,
                                    id_connect_bytesave = item.id_connect_bytesave,
                                    name = item.name,
                                    local_path = item.local_path,
                                    container_name = item.container_name,
                                    time = item.time,
                                    time_delete = item.time_delete,
                                    connect_bytesave_name = item.connect_bytesave_name,
                                    time_delete_file_in_LastVersion = item.time_delete_file_in_LastVersion,
                                    connect_bytesave_username_account = item.connect_bytesave_username_account,
                                    email = item.email,
                                    is_folder = item.is_folder,
                                    time_create_at = item.time_create_at,
                                    time_update_at = item.time_update_at,
                                });
                            }
                        }
                        configs.Settings.backup_bytesaves = lst_backup;
                        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));

                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[check_is_logged] -> " + BYTESAVE_API_PBL + api_backup_list + Get_Serial_number() + "/" + configs.email_loggin);
                NLogManager.LogError("[check_is_logged] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "get_list_data_backup", 0, 0, configs.email_loggin);
                configs.Settings.backup_bytesaves = new List<backup_bytesave>();
                File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
            }
        }
        public void get_list_data_connect(AppJson configs)
        {
            string api_connect_list = System.Configuration.ConfigurationSettings.AppSettings["api_connect_list"];
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
            try
            {
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_connect_list + Get_Serial_number() + "/" + configs.email_loggin);
                WebReq.Method = "GET";
                WebReq.Timeout = 10000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    dynamic d = JsonConvert.DeserializeObject(jsonString);
                    json_connect_bytesave json_connect_bytesaves = d.ToObject<json_connect_bytesave>();
                    //var msg = json["msg"].ToString();
                    if (json_connect_bytesaves.status == "true")
                    {
                        List<connect_bytesave> lst = new List<connect_bytesave>();
                        if (json_connect_bytesaves.countdata > 0)
                        {
                            foreach (var item in json_connect_bytesaves.data)
                            {
                                lst.Add(new connect_bytesave
                                {
                                    id = item.id,
                                    id_agent = item.id_agent,
                                    id_metric_service = item.id_metric_service,
                                    metric_service_max_storage = item.metric_service_max_storage,
                                    metric_service_information_connect = item.metric_service_information_connect,
                                    metric_service_username_account = item.metric_service_username_account,
                                    time_check_at = item.time_check_at,
                                    name = item.name,
                                    type_text = item.type_text,
                                    type = item.type,
                                    time_create_at = item.time_create_at,
                                    time_update_at = item.time_update_at,
                                });
                            }
                        }
                        configs.Settings.connect_bytesaves = lst;
                        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
                        //return json_connect_bytesaves;
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[check_is_logged] -> "+ BYTESAVE_API_PBL + api_connect_list + Get_Serial_number() + "/" + configs.email_loggin);
                NLogManager.LogError("[check_is_logged] -> " + ex );
                new MainUtility().save_log_agent(ex.ToString(), "get_list_data_connect", 0, 0, configs.email_loggin);
                configs.Settings.connect_bytesaves = new List<connect_bytesave>();
                File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
            }
            //return null;
        }
        public async Task create_connect_to_server(int id, string id_service, string information_connect, string max_storage, string username_account, string name, string email_loggin)
        {
            try
            {
                string api_connect_create_or_update = System.Configuration.ConfigurationSettings.AppSettings["api_connect_create_or_update"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_connect_create_or_update + id + "/" +
                                                                            id_service + "/" + Get_Serial_number() + "/" + email_loggin + "/" + information_connect + "/" +
                                                                            max_storage + "/" + username_account + "/" + name);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var status = json["status"].ToString();
                    if (status == "true")
                    {
                        //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[create_connect_to_server] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "create_connect_to_server", 0, 0, email_loggin);
            }
        }
        public bool check_connect(string name)
        {
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
            string api_connect_check = System.Configuration.ConfigurationSettings.AppSettings["api_connect_check"];
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_connect_check + new MainUtility().Get_Serial_number() + "/" + name);
            WebReq.Method = "GET";
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonString);
                var status = json["status"].ToString();
                if (status == "true")
                {
                    return true;
                    //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (status == "false")
                {
                    System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        public bool del_connect_from_agent(int id)
        {
            try
            {
                string api_connect_delete = System.Configuration.ConfigurationSettings.AppSettings["api_connect_delete"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_connect_delete + id);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var status = json["status"].ToString();
                    if (status == "true")
                    {
                        System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return true;
                        //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (status == "false")
                    {
                        System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[del_connect_from_agent] -> " + ex);
            }
            return false;
        }

        public bool check_backup(string name)
        {
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
            string api_connect_check = System.Configuration.ConfigurationSettings.AppSettings["api_backup_check"];
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_connect_check + new MainUtility().Get_Serial_number() + "/" + name);
            WebReq.Method = "GET";
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonString);
                var status = json["status"].ToString();
                if (status == "true")
                {
                    return true;
                    //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (status == "false")
                {
                    System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("api/products/1");
                if (response.IsSuccessStatusCode)
                {

                }
            }
        }
        public async Task create_backup_to_server(int id, int id_connect_bytesave, string name, string local_path, string container_name,
            string email, int time_delete, int time_delete_file_in_LastVersion, string time, int is_folder, string email_loggin)
        {
            try
            {
                string api_backup_create_or_update = System.Configuration.ConfigurationSettings.AppSettings["api_backup_create_or_update"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_backup_create_or_update + id + "/"
                    + Get_Serial_number() + "/" + email_loggin + "/" + id_connect_bytesave + "/" +
                                                                            name + "/" + local_path.Replace('\\', '-') + "/" + container_name + "/" + (email == "" ? "0" : email) + "/"
                                                                            + time_delete + "/" + time_delete_file_in_LastVersion +
                                                                            "/" + time + "/" + is_folder);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var status = json["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[create_backup_to_server] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "create_backup_to_server", 0, 0, email_loggin);
            }
        }
        public async Task del_backup_from_agent(int id)
        {
            try
            {
                string api_backup_delete = System.Configuration.ConfigurationSettings.AppSettings["api_backup_delete"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_backup_delete + id);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())//modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var status = json["status"].ToString();
                    //if (status == "true")
                    //{
                    //    System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //   turn true;
                    //    //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    //if (status == "false")
                    //{
                    //    System.Windows.Forms.MessageBox.Show(json["msg"].ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //}
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[del_backup_from_agent] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "del_backup_from_agent", 0, 0, "");
            }

        }

        public async Task get_setting_to_server(AppJson configs)
        {
            try
            {
                string api_setting = System.Configuration.ConfigurationSettings.AppSettings["api_setting"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_setting + "/" + Get_Serial_number() + "/" + configs.email_loggin);
                WebReq.Method = "GET";
                WebReq.Timeout = 10000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    dynamic d = JsonConvert.DeserializeObject(jsonString);
                    json_setting_bytesave json_setting_bytesaves = d.ToObject<json_setting_bytesave>();
                    //var msg = json["msg"].ToString();
                    if (json_setting_bytesaves.status == "true")
                    {
                        var item = new setting_bytesave();
                        if (json_setting_bytesaves.countdata > 0)
                        {
                            item = json_setting_bytesaves.data.First();
                        }
                        configs.Settings.bytesave_setting.set_bytesave = item;
                        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
                        //return json_setting_bytesaves;
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[get_setting_to_server] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "get_setting_to_server", 0, 0, configs.email_loggin);
                configs.Settings.bytesave_setting.set_bytesave = new setting_bytesave();
                File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
            }
            ///*return*/ null;
        }
        public async Task create_setting_to_server(int id, int id_agent, string email_loggin, string server_mail, string port, string mail_send, string mail_send_pwd, string subject)
        {
            try
            {
                string api_setting_create = System.Configuration.ConfigurationSettings.AppSettings["api_setting_create"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_setting_create + id + "/" + id_agent + "/" + Get_Serial_number() + "/" + email_loggin + "/" + server_mail + "/" + port + "/" + mail_send + "/" + mail_send_pwd + "/" + subject);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var status = json["status"].ToString();

                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[create_setting_to_server] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "create_setting_to_server", 0, 0, email_loggin);
            }
        }
        public void get_info_to_server(AppJson configs)
        {
            string api_info = System.Configuration.ConfigurationSettings.AppSettings["api_info"];
            //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
            try
            {
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_info + Get_Serial_number() + "/" + configs.email_loggin);
                WebReq.Method = "GET";
                WebReq.Timeout = 10000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    dynamic d = JsonConvert.DeserializeObject(jsonString);
                    json_info_bytesave json_info_bytesaves = d.ToObject<json_info_bytesave>();
                    //var msg = json["msg"].ToString();
                    if (json_info_bytesaves.status == "true")
                    {
                        var info = new Info();
                        if (json_info_bytesaves.countdata > 0)
                        {
                            var item = json_info_bytesaves.data[0];

                            info.name_version = item.name_version;
                            info.bytesave_expiration_date = item.bytesave_expiration_date;
                        }
                        configs.Settings.bytesave_info.information = info;
                        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[get_info_to_server] -> " + BYTESAVE_API_PBL + api_info + Get_Serial_number() + "/" + configs.email_loggin);
                NLogManager.LogError("[get_info_to_server] -> " + ex);
                new MainUtility().save_log_agent(ex.ToString(), "get_info_to_server", 0, 0, configs.email_loggin);
                configs.Settings.bytesave_info.information = new Info();
                File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(configs));
            }
        }
        public bool save_log_agent(string logcontent, string function, int status, int type, string email_loggin)
        {
            //type = 0: log
            //type =1: history
            try
            {
                string api_save_log = System.Configuration.ConfigurationSettings.AppSettings["api_save_log"];
                //string id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                var BYTESAVE_API_SAVE_LOG = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_SAVE_LOG"];

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_SAVE_LOG + api_save_log + Get_Serial_number() + "/ " + email_loggin + "/" + logcontent.Replace('/', '\\') + "/" + function + "/" + status + "/" + ConvertToTimestamp(DateTime.Now) + "/" + type);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[save_log_agent] -> " + ex);
            }
            return false;
        }
        private static double ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }

        public json_log_bytesave get_log_to_server(int type)
        {
            try
            {
                string api_get_log = System.Configuration.ConfigurationSettings.AppSettings["api_get_log"];
                var BYTESAVE_API_SAVE_LOG = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_SAVE_LOG"];

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_SAVE_LOG + api_get_log + "/" + Get_Serial_number() + "/" + type);
                WebReq.Method = "GET";
                WebReq.Timeout = 20000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    dynamic d = JsonConvert.DeserializeObject(jsonString);
                    json_log_bytesave json_info_bytesaves = d.ToObject<json_log_bytesave>();
                    return json_info_bytesaves;
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[get_info_to_server] -> " + ex);
            }
            return null;
        }

    }
}
