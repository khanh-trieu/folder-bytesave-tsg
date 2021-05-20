using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using NetCore.Utils.Extensions;
using NetCore.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransferWorker.UI.Models;
using Newtonsoft.Json;
using OperatingSystem = TransferWorker.UI.Models.OperatingSystem;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using TransferWorker.UI.Helpers;

namespace TransferWorker.UI.Helpers
{
    public class AzureBlobHelper : IAzureBlobHelper
    {
        private string filePath = @"D:\Code\Transfer_New\transferworker\TransferWorker\appsettings.json";
        private string localFilePath = "";
        private string _connectionString = string.Empty;
        private string _containerName = string.Empty;
        private string _localFolderPath = string.Empty;
        private readonly backup_bytesave _config;
        private readonly connect_bytesave _appSettings;
        private int count;
        public BlobContainerClient _blobContainer { get; set; }
        public AzureBlobHelper(connect_bytesave options, backup_bytesave config)
        {
            _appSettings = options;
            _config = config;
            _connectionString = _appSettings.metric_service_information_connect;
            _containerName = config != null ? config.container_name : "";
            _localFolderPath = config != null ? config.local_path : "";
            if (OperatingSystem.IsWindows() && _localFolderPath.LastOrDefault() != '\\')
            {
                _localFolderPath = _localFolderPath + '\\';
            }
            else if (OperatingSystem.IsWindows() && _localFolderPath.LastOrDefault() != '/')
            {
                _localFolderPath = _localFolderPath + '/';
            }
            _blobContainer = GetBlobContainer();
            if (null == _blobContainer)
            {
                NLogManager.LogError("ERROR: Can not get BlobContainer.");
            }
        }

        private BlobContainerClient GetBlobContainer()
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            return container;
        }

        public string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                var _task = Task.Run(() => this.UploadFileToBlobAsync(strFileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                BlobClient blob = _blobContainer.GetBlobClient(strFileName);
                // Why .NET Core doesn't have MimeMapping.GetMimeMapping()
                var blobHttpHeader = new BlobHttpHeaders();
                string extension = Path.GetExtension(blob.Uri.AbsoluteUri);

                blobHttpHeader.ContentType = fileMimeType;

                await using (var fileStream = new MemoryStream(fileData))
                {
                    var uploadedBlob = await blob.UploadAsync(fileStream, blobHttpHeader);
                }
                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //public async Task SyncData(string localFolderPath, string container)
        //{
        //}
        public async Task Compare()
        {
            try
            {
                NLogManager.LogInfo("Compare");

                // lấy danh sách file ở local
                var localFiles = GetListFileFromLocal();
                //var localChangedFiles = GetListFileChangedFromLocal(localFolderPath);
                //// xóa những file ở cloud nằm trong ds file đã update ở local
                //foreach (var fi in localChangedFiles)
                //{
                //    DeleteFile(fi.FileName);
                //}
                var cloudFiles = await GetListFileFromCloud();
                var cloudFilesLocal = await GetLog(_containerName);
                // 3. Compare Files
                // những file có độ dài thay đổi hoặc chưa có

                var excepts = localFiles.Except(cloudFiles).ToList();
                var cloudExcepts = cloudFilesLocal.Except(cloudFiles).ToList();
                if (excepts.Any())
                {
                    // upload file
                    using var concurrencySemaphore = new SemaphoreSlim(10);
                    var downloadTask = new List<Task>();
                    foreach (var fileSyncInfo in excepts)
                    {
                        // kiếm tra xem file này có phải xóa trên cloud không?
                        if (cloudExcepts.Any(c => c.FileName == fileSyncInfo.FileName))
                        {
                            continue;
                        }

                        concurrencySemaphore.Wait();
                        //WriteMessage($"DEBUG: Concurrency Semaphore {concurrencySemaphore.CurrentCount} / {Options.MaxConcurrency}");

                        var t = Task.Run(async () =>
                        {
                            try
                            {
                                await UploadFile(fileSyncInfo.FileName);
                            }
                            catch (Exception e)
                            {
                                NLogManager.PublishException(e);
                            }
                            finally
                            {
                                //WriteMessage($"DEBUG: Release concurrencySemaphore", ConsoleColor.DarkYellow);
                                concurrencySemaphore.Release();
                            }
                        });

                        // WriteMessage($"DEBUG: Added {fileSyncInfo.FileName} ({fileSyncInfo.Length} bytes) to download tasks.", ConsoleColor.DarkYellow);
                        downloadTask.Add(t);
                    }

                    await Task.WhenAll(downloadTask);
                }
                else
                {
                    NLogManager.LogMessage("No new files need to be downloaded.");
                }
                //chuyển tier
                //if (_config.CoolTier > 0 || _config.ArchiveTier > 0)
                //{
                //    List<Task> listOfTasks = new List<Task>();
                //    var coolList = new List<FileSyncInfo>();
                //    //var archiveList = new List<FileSyncInfo>();
                //    if (_config.CoolTier > 0)
                //    {
                //        foreach (var f in cloudFiles)
                //        {
                //            var tier = AccessTier.Cool;
                //            if (f.CreatedTime.AddDays(_config.CoolTier) < DateTime.Now.ToUniversalTime())
                //            {
                //                coolList.Add(f);
                //                listOfTasks.Add(ChangeTier(f.FileName, tier));
                //            }
                //        }
                //    }
                //    if (_config.ArchiveTier > 0)
                //    {
                //        foreach (var f in cloudFiles.Except(coolList))
                //        {
                //            var tier = AccessTier.Archive;
                //            if (f.CreatedTime.AddDays(_config.ArchiveTier) < DateTime.Now.ToUniversalTime())
                //            {
                //                listOfTasks.Add(ChangeTier(f.FileName, tier));
                //            }
                //        }
                //    }
                //    await Task.WhenAll(listOfTasks);
                //}
                //xóa file theo cấu hình có trước
                //if (_config.DeleteTimer > 0)
                //{
                //    List<Task> listOfTasks = new List<Task>();

                //    foreach (var f in cloudFiles)
                //    {
                //        if (f.CreatedTime.AddDays(_config.DeleteTimer) < DateTime.Now.ToUniversalTime())
                //        {
                //            listOfTasks.Add(DeleteFile(f.FileName));
                //        }
                //    }
                //    await Task.WhenAll(listOfTasks);
                //}
                // Ask Delete Old Files
                //var localExcepts = new List<FileSyncInfo>();
                //localExcepts = cloudFiles.Where(c => !localFiles.Any(l => l.FileName == c.FileName)).ToList();
                //var deleteCount = 0;
                //if (localExcepts.Any())
                //{
                //    NLogManager.LogInfo("Deleting local redundancy files...");
                //    foreach (var fi in localExcepts)
                //    {
                //        DeleteFile(fi.FileName);
                //        deleteCount++;
                //    }
                //}
                // lưu log file đã thay đổi trên cloud
                cloudFiles = await GetListFileFromCloud();
                await WriteLog(_containerName, JsonConvert.SerializeObject(cloudFiles));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
       
        public async Task Download(List<FileViewInfo> files,string path)
        {
            try
            {
                NLogManager.LogInfo("Download");
                //foreach (var fileSyncInfo in files.Where(f => f.IsFolder == "Visible"))
                //{
                //    try
                //    {
                //        await DownloadFile(fileSyncInfo, path);
                //    }
                //    catch (Exception e)
                //    {
                //        NLogManager.PublishException(e);
                //    }
                //}
                if (files.Where(f => f.IsFile == "Visible").Any())
                {
                    // upload file
                    using var concurrencySemaphore = new SemaphoreSlim(10);
                    var downloadTask = new List<Task>();
                    foreach (var fileSyncInfo in files)
                    {
                        concurrencySemaphore.Wait();
                        //WriteMessage($"DEBUG: Concurrency Semaphore {concurrencySemaphore.CurrentCount} / {Options.MaxConcurrency}");

                        var t = Task.Run(async () =>
                        {
                            try
                            {
                                await DownloadFile(fileSyncInfo, path);
                            }
                            catch (Exception e)
                            {
                                NLogManager.PublishException(e);
                            }
                            finally
                            {
                                //WriteMessage($"DEBUG: Release concurrencySemaphore", ConsoleColor.DarkYellow);
                                concurrencySemaphore.Release();
                            }
                        });

                        // WriteMessage($"DEBUG: Added {fileSyncInfo.FileName} ({fileSyncInfo.Length} bytes) to download tasks.", ConsoleColor.DarkYellow);
                        downloadTask.Add(t);
                    }

                    await Task.WhenAll(downloadTask);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private async Task WriteLog(string fileName, string content)
        {
            try
            {
                if (!Directory.Exists("Data"))
                {
                    Directory.CreateDirectory("Data");
                }
                string localFilePath = Path.Combine("Data", fileName + ".txt");
                await File.WriteAllTextAsync(localFilePath, content);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private Task ChangeTier(string fileName, AccessTier tier)
        {
            BlobClient blob = _blobContainer.GetBlobClient(fileName);
            return blob.SetAccessTierAsync(tier);
        }

        private async Task<List<FileSyncInfo>> GetLog(string fileName)
        {
            var content = string.Empty;
            try
            {
                string localFilePath = Path.Combine("Data", fileName + ".txt");
                content = await File.ReadAllTextAsync(localFilePath);
                return JsonConvert.DeserializeObject<List<FileSyncInfo>>(content);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new List<FileSyncInfo>();
        }
        public async Task UploadFile(string fileName)
        {
            NLogManager.LogInfo("Uploading " + fileName);
            string localFilePath = Path.Combine(_localFolderPath, fileName);
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            var uploadFileStream = File.ReadAllBytes(localFilePath);
            await UploadFileToBlobAsync(fileName, uploadFileStream, contentType);
        }

        public Task DeleteFile(string fileName)
        {
            BlobClient blob = _blobContainer.GetBlobClient(fileName);
            return blob.DeleteIfExistsAsync();
        }

        public async Task DownloadFile(FileViewInfo info,string path)
        {
            try
            {
                //if (info.IsFolder == "Visible")
                //{
                //    string folderNamePath = Path.GetDirectoryName(info.OriginalPath);
                //    if (!string.IsNullOrEmpty(folderNamePath))
                //    {
                //        //var strArray = folderNamePath.Split('\\');
                //        //for (int i = 0; i < strArray.Length; i++)
                //        //{
                //        //    if (lstFile.Exists(f => f.Level == i && f.FileName == strArray[i]))
                //        //    {
                //        //        continue;
                //        //    }
                //        //}
                //        if (!Directory.Exists(folderNamePath))
                //        {
                //            string pathString = System.IO.Path.Combine(path, folderNamePath);
                //            //var dirInfo = Directory.CreateDirectory(Path.Combine(path, folderNamePath));
                //            System.IO.Directory.CreateDirectory(pathString);
                //        }
                //    }
                //    return;
                //}
                BlobClient blob = _blobContainer.GetBlobClient(info.OriginalPath);
                string input = info.OriginalPath;
                string namefile = "";
                string pathfolder = "";
                var leng = input.Length;
                int index = input.LastIndexOf("/");
                if (index > 0)
                {
                    namefile = input.Substring(index + 1, leng - (index + 1));
                    pathfolder = input.Substring(0,(index + 1));
                }
                string pathString = System.IO.Path.Combine(path, pathfolder).Replace("/","\\");
                //var dirInfo = Directory.CreateDirectory(Path.Combine(path, folderNamePath));
                System.IO.Directory.CreateDirectory(pathString);

                Stream file = File.OpenWrite(Path.Combine(pathString, namefile.Length == 0 ? info.OriginalPath : namefile));
                await blob.DownloadToAsync(file);
                file.Close();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        public string GetPath(string fileName, int type)
        {
            var content = string.Empty;
            try
            {
                localFilePath = Path.GetFullPath(fileName + ".txt");
                string contents = File.ReadAllText(localFilePath);
                var links = contents.Split('|');
                return (links[type]);
            }
            catch (Exception ex)
            {
                File.WriteAllText(fileName + ".txt", filePath);
                string contents = File.ReadAllText(localFilePath);
                var links = contents.Split('|');
                return (links[type]);
            }
        }
        public Settings LoadConfig()
        {
            filePath = GetPath("aPathJsonLocal", 0);
            var str = File.ReadAllText(filePath);
            var configs = System.Text.Json.JsonSerializer.Deserialize<AppJson>(str);
            return configs.Settings;
        }
        private Settings _settings;
        private BlobContainerClient GetBlobContainerV2()
        {
            _connectionString = "";
            var container = new BlobContainerClient(_settings.connect_bytesaves[0].metric_service_information_connect, _settings.backup_bytesaves[0].container_name);
            return container;
        }
        private BlobContainerClient GetBlobContainerV2(string nameContainer, string StorageConnectionString)
        {
            //s.AppSettings.FirstOrDefault(x => x.IdAppSetting == idAppsetting);

            //if (storageConnect == null)
            //    storageConnect = _settings.AppSettings[0];
            var container = new BlobContainerClient(StorageConnectionString, nameContainer);
            return container;
        }
        public async Task<List<FileSyncInfo>> GetListFileFromCloud()
        {
            NLogManager.LogInfo("GetListFileFromCloud");
            var cloudFiles = new List<FileSyncInfo>();

            _settings = LoadConfig();
            _blobContainer = GetBlobContainerV2();
            await foreach (var blobItem in _blobContainer.GetBlobsAsync())
            {
                var fsi = new FileSyncInfo
                {
                    Hash = blobItem.GetHashCode(),
                    FileName = blobItem.Name,
                    Length = blobItem.Properties.ContentLength,
                    LastModified = blobItem.Properties.LastModified != null ? blobItem.Properties.LastModified.Value.DateTime : new DateTime(1970, 1, 1),
                    CreatedTime = blobItem.Properties.CreatedOn != null ? blobItem.Properties.CreatedOn.Value.DateTime : new DateTime(1970, 1, 1)
                };
                cloudFiles.Add(fsi);
            }
            NLogManager.LogInfo(JsonConvert.SerializeObject(cloudFiles));

            return cloudFiles;
        }
        public async Task<List<FileSyncInfo>> GetListFileFromCloud(string nameContainer, int idAppsetting,string connection_string)
        {
            NLogManager.LogInfo("GetListFileFromCloud");
            var cloudFiles = new List<FileSyncInfo>();

            //_settings = LoadConfig();
            //var storageConnect = _settings.AppSettings.FirstOrDefault(x => x.IdAppSetting == idAppsetting);


            //if (storageConnect == null)
            //    return null;
            var containers = await ListContainersAsync(connection_string);
            if (containers.Count() == 0)
            {
                return null;
            }
            _blobContainer = GetBlobContainerV2(nameContainer, connection_string);
            var blobContainer = _blobContainer.GetBlobsAsync();
            await foreach (var blobItem in blobContainer)
            {
                var fsi = new FileSyncInfo
                {
                    Hash = blobItem.GetHashCode(),
                    FileName = blobItem.Name,
                    Length = blobItem.Properties.ContentLength,
                    LastModified = blobItem.Properties.LastModified != null ? blobItem.Properties.LastModified.Value.DateTime.AddHours(7) : new DateTime(1970, 1, 1),
                    CreatedTime = blobItem.Properties.CreatedOn != null ? blobItem.Properties.CreatedOn.Value.DateTime.AddHours(7) : new DateTime(1970, 1, 1)
                };
                cloudFiles.Add(fsi);
            }
            NLogManager.LogInfo(JsonConvert.SerializeObject(cloudFiles));

            return cloudFiles;
        }

        public static async Task<IEnumerable<CloudBlobContainer>> ListContainersAsync(string connectionString)
        {
            CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount);
            if (storageAccount == null)
                return null;
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            if (storageAccount == null)
            {
                Console.WriteLine("Connection string did not work");
            }

            BlobContinuationToken continuationToken = null;
            var containers = new List<CloudBlobContainer>();
            do
            {
                ContainerResultSegment response = await cloudBlobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                containers.AddRange(response.Results);

            } while (continuationToken != null);

            return containers;
        }
        public List<FileSyncInfo> GetListFileFromLocal()
        {
            NLogManager.LogInfo("GetListFileFromLocal");
            if (!Directory.Exists(_localFolderPath))
            {
                return new List<FileSyncInfo>();
            }

            var localFilePaths = Directory.GetFiles(_localFolderPath, "*.*", SearchOption.AllDirectories);
            var localFiles = localFilePaths.Select(filePath => new FileInfo(filePath)).ToList();
            NLogManager.LogInfo(JsonConvert.SerializeObject(localFilePaths));

            return localFiles.Select(fi => new FileSyncInfo
            {
                FileName = fi.FullName.ReplaceFirst(_localFolderPath, "").Replace("\\", "/"),
                Length = fi.Length,
                LastModified = Directory.GetLastWriteTimeUtc(fi.FullName),
                CreatedTime = Directory.GetCreationTimeUtc(fi.FullName),
                Hash = fi.GetHashCode()
            }).ToList();
        }

        //private List<FileSyncInfo> GetListFileChangedFromLocal(string localFolderPath)
        //{
        //    if (!Directory.Exists(localFolderPath))
        //    {
        //        return new List<FileSyncInfo>();
        //    }

        //    var localFilePaths = Directory.GetFiles(localFolderPath, "*.*", SearchOption.AllDirectories)
        //        .Where(i => Directory.GetLastWriteTime(i) > DateTime.Now.Add(-new TimeSpan(0, _appSettings.Timer, 0)));
        //    var localFiles = localFilePaths.Select(filePath => new FileInfo(filePath)).ToList();
        //    return localFiles.Select(fi => new FileSyncInfo
        //    {
        //        FileName = fi.FullName.ReplaceFirst(localFolderPath, "").Replace("\\", "/"),
        //        Length = fi.Length,
        //    }).ToList();
        //}
        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }
    }
}