using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using JetBrains.Annotations;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;
using NetCore.Utils.Extensions;
using NetCore.Utils.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TransferWorker.Models;
using OperatingSystem = TransferWorker.Models.OperatingSystem;
using Microsoft.Azure;
using System.Globalization;
using TransferWorker.Utility;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs;
using System.Text;
using System.Security.Cryptography;

namespace TransferWorker.Helpers
{
    public class AzureBlobHelper : IAzureBlobHelper
    {
        private string _connectionString = string.Empty;
        private string _containerName = string.Empty;
        private string _localFolderPath = string.Empty;
        private string fileName = string.Empty;
        private List<string> lstFilne = null;
        private readonly FolderConfig _config;
        private readonly AppSetting _appSettings;
        public BlobContainerClient _blobContainer { get; set; }
        private List<FileSyncInfo> cloudFiles;
        public AzureBlobHelper(AppSetting options, FolderConfig config)
        {
            _appSettings = options;
            _config = config;
            _connectionString = _appSettings.StorageConnectionString;
            _containerName = config.ContainerName;
            _localFolderPath = config.LocalFolderPath;
            if (OperatingSystem.IsWindows() && _localFolderPath.LastOrDefault() != '\\')
            {
                if (config.IsFolder == true) _localFolderPath = _localFolderPath + '\\';
                //_localFolderPath = _localFolderPath + '\\';
            }
            else if (OperatingSystem.IsWindows() && _localFolderPath.LastOrDefault() != '/')
            {
                if (config.IsFolder == true) _localFolderPath = _localFolderPath + '/';
                //_localFolderPath = _localFolderPath + '/';
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

        public async Task Compare()
        {
            // _logs = new MainUtility().LoadLog();
            try
            {
                NLogManager.LogInfo("Compare");
                lstFilne = new List<string>();
                // lấy danh sách file ở local
                var localFiles = GetListFileFromLocal();
                //var localChangedFiles = GetListFileChangedFromLocal(localFolderPath);
                //// xóa những file ở cloud nằm trong ds file đã update ở local
                //foreach (var fi in localChangedFiles)
                //{
                //    DeleteFile(fi.FileName);
                //}
                cloudFiles = await GetListFileFromCloud();
                //var cloudFilesLocal = await GetLog(_containerName + _appSettings.AccountName);
                var lstCloudClient = cloudFiles.Join(localFiles,
                                      p => p.FileName, c => c.FileName,
                                      (p, c) => new FileSyncInfo
                                      {
                                          FileName = p.FileName,
                                          Hash = p.Hash,
                                          CreatedTime = p.CreatedTime,
                                          LastModified = p.LastModified,
                                          Length = p.Hash
                                      }).ToList();

                // 3. Compare Files
                // những file có độ dài thay đổi hoặc chưa có
                var excepts = localFiles.Except(cloudFiles).ToList();
                // List<FileSyncInfo> cloudExcepts = cloudFilesLocal != null ? cloudFilesLocal : new List<FileSyncInfo>();
                //if (cloudFilesLocal != null)
                //{
                //    cloudExcepts = cloudFilesLocal.Except(cloudFiles).ToList();
                //}
                //Check xem đường dẫn là file hay folder
                if (_config.IsFolder == false)
                {
                    var pathfile = Path.GetDirectoryName(_config.LocalFolderPath);
                    try
                    {
                        var namefile = _config.LocalFolderPath.ReplaceFirst(pathfile, "").Replace("\\", "");
                        FileInfo info = new FileInfo(_config.LocalFolderPath.Replace(@"\\", @"\"));
                        //if (cloudExcepts != null && cloudExcepts.Any(c => c.FileName == namefile && c.Hash == hass))
                        //{
                        //    return;
                        //}
                        System.IO.FileInfo localfile = new System.IO.FileInfo(_config.LocalFolderPath);
                        if (localfile.Exists == false)
                        {
                            
                        }
                        else
                        {
                            var hass = GetHash(namefile + info.Length);
                            var fileCloude = cloudFiles.FirstOrDefault(x => x.FileName == namefile);
                            if (fileCloude == null)
                            {
                                fileName = namefile;
                                NLogManager.LogMessage("Updating in file: " + _config.LocalFolderPath);
                                // upload file
                                using var concurrencySemaphore = new SemaphoreSlim(_config.MaxConcurrency);
                                var downloadTask = new List<Task>();
                                //   GetFileLastVersion(namefile);
                                concurrencySemaphore.Wait();
                                var t = Task.Run(async () =>
                                {
                                    try
                                    {
                                        var fltt = cloudFiles.FirstOrDefault(x => x.FileName == namefile);
                                        if (fltt != null)
                                        {
                                            MoveBlobBetweenContainers(namefile);
                                        }
                                        //  TransferLocalFileDetailToAzureBlob(namefile);
                                        upload_ToBlob(namefile, _config.LocalFolderPath);
                                        new MainUtility().AddLogContentAsync(_config.JobName, namefile, 1);
                                        await new SendMail().SendMailTSG(_config.Email, _config.JobName, namefile, true);
                                    }
                                    catch (Exception e)
                                    {
                                        new MainUtility().AddLogContentAsync(_config.JobName, e.ToString(), 0);
                                        NLogManager.LogError(e.ToString());
                                        await new SendMail().SendMailTSG(_config.Email, _config.JobName, namefile, false);
                                    }
                                    finally
                                    {
                                        //WriteMessage($"DEBUG: Release concurrencySemaphore", ConsoleColor.DarkYellow);
                                        concurrencySemaphore.Release();
                                    }
                                });
                                downloadTask.Add(t);
                                await Task.WhenAll(downloadTask);
                            }
                        }
                        
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                else
                {
                    NLogManager.LogMessage("Updating in folder " + _config.LocalFolderPath);
                    if (excepts.Any())
                    {
                        try
                        {
                            using var concurrencySemaphore = new SemaphoreSlim(_config.MaxConcurrency);
                            var downloadTask = new List<Task>();
                            foreach (var fileSyncInfo in excepts)
                            {
                                // kiếm tra xem file này có phải xóa trên cloud không?
                                //if (cloudExcepts != null && cloudExcepts.Any(c => c.FileName == fileSyncInfo.FileName && c.Hash == fileSyncInfo.Hash))
                                //{
                                //    continue;
                                //}
                                if (fileSyncInfo.FileName.Substring(0, 10) == "[DelCloud]")
                                {
                                    continue;
                                }
                                concurrencySemaphore.Wait();
                                //WriteMessage($"DEBUG: Concurrency Semaphore {concurrencySemaphore.CurrentCount} / {Options.MaxConcurrency}");
                                lstFilne.Add(fileSyncInfo.FileName);
                                var t = Task.Run(async () =>
                                {
                                    try
                                    {
                                        var fltt = cloudFiles.FirstOrDefault(x => x.FileName == fileSyncInfo.FileName);
                                        if (fltt != null)
                                        {
                                            MoveBlobBetweenContainers(fileSyncInfo.FileName);
                                            //CopyBlobtoBlob(fileSyncInfo.FileName, fltt.LastModified);
                                        }
                                        string localFilePath = Path.Combine(_localFolderPath, fileSyncInfo.FileName).Replace("/", "\\");
                                        await upload_ToBlob(fileSyncInfo.FileName, localFilePath);
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
                            string combindedString = string.Join(",", lstFilne);
                            new MainUtility().AddLogContentAsync(_config.JobName, combindedString, 1);
                            await Task.WhenAll(downloadTask);
                            new SendMail().SendMailTSG(_config.Email, _config.JobName, combindedString, true);
                        }
                        catch (Exception ex)
                        {
                            NLogManager.LogError("Lỗi Update:" + ex.ToString());
                            new SendMail().SendMailTSG(_config.Email, _config.JobName, "", false);
                            new MainUtility().AddLogContentAsync(_config.JobName, ex.ToString(), 0);
                        }
                    }
                    else
                    {
                        NLogManager.LogMessage("No new files need to be downloaded.");
                        //new SendMail().SendMaikTSG(_config.Email, _config.JobName, excepts.ToString(), true);
                    }
                }
                //GET lại all file cloude để xét tier
                cloudFiles = await GetListFileFromCloud();
                //chuyển tier Tạm thời đóng chức năng này lại 
    //            if (_config.CoolTier >= 0 || _config.ArchiveTier >= 0)
    //            {
    //                try
    //                {
    //                    List<Task> listOfTasks = new List<Task>();
    //                    var coolList = new List<FileSyncInfo>();
    //                    //var archiveList = new List<FileSyncInfo>();
    //                    if (_config.CoolTier >= 0)
    //                    {
    //                        if (fileName.Length > 0)
    //                        {
    //                            var fileCloude = cloudFiles.FirstOrDefault(x => x.FileName == fileName);
    //                            var tier = AccessTier.Cool;
    //                            if (fileCloude.CreatedTime.AddDays(_config.CoolTier) < DateTime.Now.ToUniversalTime())
    //                            {
    //                                coolList.Add(fileCloude);
    //                                listOfTasks.Add(ChangeTier(fileCloude.FileName, tier));
    //                            }
    //                        }
    //                        foreach (var f in lstCloudClient)
    //                        {
    //                            var tier = AccessTier.Cool;
    //                            if (f.CreatedTime.AddDays(_config.CoolTier) < DateTime.Now.ToUniversalTime())
    //                            {
    //                                coolList.Add(f);
    //                                listOfTasks.Add(ChangeTier(f.FileName, tier));
    //                            }
    //                        }
    //                    }
    //                    if (_config.ArchiveTier >= 0)
    //                    {
    //                        if (fileName.Length > 0)
    //                        {
    //                            var fileCloude = cloudFiles.FirstOrDefault(x => x.FileName == fileName);
    //                            var tier = AccessTier.Archive;
    //                            //var cool = coolList.FirstOrDefault(x => x.FileName == fileName);
    //                            if (fileCloude.CreatedTime.AddDays(_config.CoolTier
    //+ _config.ArchiveTier) < DateTime.Now.ToUniversalTime())
    //                            {
    //                                listOfTasks.Add(ChangeTier(fileCloude.FileName, tier));
    //                            }
    //                        }
    //                        foreach (var f in lstCloudClient)
    //                        {
    //                            var tier = AccessTier.Archive;
    //                            if (f.CreatedTime.AddDays(_config.CoolTier
    //+ _config.ArchiveTier) < DateTime.Now.ToUniversalTime())
    //                            {
    //                                listOfTasks.Add(ChangeTier(f.FileName, tier));
    //                            }
    //                        }
    //                    }
    //                    await Task.WhenAll(listOfTasks);
    //                }
    //                catch (Exception ex)
    //                {
    //                    NLogManager.LogError("Chuyển tier lỗi : " + ex.ToString());
    //                }
    //            }
                NLogManager.LogInfo("DELETE TIME : " + _config.DeleteTimer);
                //xóa file theo cấu hình có trước
                if (_config.DeleteTimer > 0)
                {
                    NLogManager.LogInfo("Delele");
                    List<Task> listOfTasks = new List<Task>();
                    if (fileName.Length > 0)
                    {
                        var fileCloude = cloudFiles.FirstOrDefault(x => x.FileName == fileName);
                        if (fileCloude.LastModified.AddDays(_config.DeleteTimer) < DateTime.Now.ToUniversalTime())
                        {
                            listOfTasks.Add(DeleteFile(fileCloude.FileName));
                            //cloudExcepts.Add(fileCloude);
                        }
                    }
                    foreach (var f in lstCloudClient)
                    {
                        if (f.LastModified.AddDays(_config.DeleteTimer) < DateTime.Now.ToUniversalTime())
                        {
                            listOfTasks.Add(DeleteFile(f.FileName));
                            //cloudExcepts.Add(f);
                        }
                    }
                    await Task.WhenAll(listOfTasks);
                }
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
                //cloudFiles = await GetListFileFromCloud();
                //await WriteLog(_containerName + _appSettings.AccountName, cloudExcepts);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void ReNameFile(string fileNames)
        {
            // Source file to be renamed  
            //string sourceFile = @"C:\Temp\MaheshChand.jpg";
            // Create a FileInfo  
            string localFilePath = Path.Combine(_localFolderPath, fileNames);
            string localFileSub = "";
            if (_config.IsFolder == false)
            {
                localFilePath = _localFolderPath;
                localFileSub = Path.GetDirectoryName(_config.LocalFolderPath);
            }
            System.IO.FileInfo fi = new System.IO.FileInfo(localFilePath);
            // Check if file is there  
            if (fi.Exists)
            {
                // var slts = localFilePath.Split("\\");
                string newFileName = "[DelCloud]_" + fileNames;
                string localFilePathnew = Path.Combine(_config.IsFolder == true ? _localFolderPath : localFileSub, newFileName);
                // Move file with a new name. Hence renamed.  
                fi.MoveTo(localFilePathnew);
            }
        }
        private async Task WriteLog(string fileName, List<FileSyncInfo> content)
        {
            string localFilePath = string.Empty;
            try
            {
                if (!Directory.Exists("Data"))
                {
                    Directory.CreateDirectory("Data");
                }
                localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            + "\\Data\\" + fileName + ".txt";
                FileAttributes attributes = File.GetAttributes(localFilePath);
                attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                File.SetAttributes(localFilePath, attributes);
                var contentold = File.ReadAllText(localFilePath);

                var len = contentold.Length;

                var old = JsonConvert.DeserializeObject<List<FileSyncInfo>>(contentold);
                if (content.Count > 0)
                {
                    //var logExcepts = old.Except(content).ToList();
                    old.AddRange(content);
                }
                //  string localFilePath = Path.Combine("Data", fileName + ".txt");
                await File.WriteAllTextAsync(localFilePath, JsonConvert.SerializeObject(old));
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
            }
            finally
            {
                File.SetAttributes(localFilePath, File.GetAttributes(localFilePath) | FileAttributes.Hidden);
            }
        }
        private Task ChangeTier(string fileName, AccessTier tier)
        {
            BlobClient blob = _blobContainer.GetBlobClient(fileName);
            return blob.SetAccessTierAsync(tier);
        }

        private async Task<List<FileSyncInfo>> GetLog(string fileName)
        {
            NLogManager.LogInfo("GetLog " + fileName);
            var content = string.Empty;
            string localFilePath = string.Empty;
            try
            {
                //  string localFilePath = Path.Combine("Data", fileName + ".txt");
                localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
           + "\\Data\\" + fileName + ".txt";
                FileAttributes attributes = File.GetAttributes(localFilePath);
                attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                File.SetAttributes(localFilePath, attributes);
                content = File.ReadAllText(localFilePath);

                return JsonConvert.DeserializeObject<List<FileSyncInfo>>(content);
            }
            catch (Exception ex)
            {
                File.WriteAllText(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            + "\\Data\\" + fileName + ".txt", "[]");
                content = File.ReadAllText(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            + "\\Data\\" + fileName + ".txt");
                return JsonConvert.DeserializeObject<List<FileSyncInfo>>(content);
            }
            finally
            {
                File.SetAttributes(localFilePath, File.GetAttributes(localFilePath) | FileAttributes.Hidden);
            }
            return new List<FileSyncInfo>();
        }
        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
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

            using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                long fLength = stream.Length / 20 + 1;
                byte[] bytes = new byte[fLength];

                await UploadFileToBlobAsync(fileName, bytes, contentType);
            }

            var uploadFileStream = File.ReadAllBytes(localFilePath);
            await UploadFileToBlobAsync(fileName, uploadFileStream, contentType);
        }

        public async Task TransferLocalFileToAzureBlob(string fileName)
        {
            NLogManager.LogInfo("Uploading " + fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
            container.CreateIfNotExistsAsync().Wait();

            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
            string localFilePath = Path.Combine(_localFolderPath, fileName).Replace("/", "\\");
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            await TransferManager.UploadAsync(localFilePath, blob);
            // ExecuteChoice(account);
            NLogManager.LogInfo("Upload Success  " + fileName);
        }

        /// <summary>
        /// upload file lên container
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="fileData"></param>
        /// <param name="fileMimeType"></param>
        /// <returns></returns>
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
        public Task DeleteFile(string fileName)
        {
            NLogManager.LogInfo("Delete " + fileName);
            ReNameFile(fileName);
            BlobClient blob = _blobContainer.GetBlobClient(fileName);
            return blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// thông tin file từ cloud
        /// </summary>
        /// <returns></returns>
        private async Task<List<FileSyncInfo>> GetListFileFromCloud()
        {
            NLogManager.LogInfo("GetListFileFromCloud");
            var cloudFiles = new List<FileSyncInfo>();
            try
            {

                await foreach (var blobItem in _blobContainer.GetBlobsAsync())
                {
                    var fsi = new FileSyncInfo
                    {
                        Hash = GetHash(blobItem.Name + blobItem.Properties.ContentLength),
                        FileName = blobItem.Name,
                        Length = blobItem.Properties.ContentLength,
                        LastModified = blobItem.Properties.LastModified != null ? blobItem.Properties.LastModified.Value.DateTime : new DateTime(1970, 1, 1),
                        CreatedTime = blobItem.Properties.CreatedOn != null ? blobItem.Properties.CreatedOn.Value.DateTime : new DateTime(1970, 1, 1),
                    };
                    cloudFiles.Add(fsi);
                }
                NLogManager.LogInfo(JsonConvert.SerializeObject(cloudFiles));
                return cloudFiles;
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo(ex);
                return cloudFiles;
            }
        }
        /// <summary>
        /// thông tin file từ local
        /// </summary>
        /// <returns></returns>
        private List<FileSyncInfo> GetListFileFromLocal()
        {
            NLogManager.LogInfo("GetListFileFromLocal");

            if (!Directory.Exists(_localFolderPath))
            {
                return new List<FileSyncInfo>();
            }
            var localFilePaths = Directory.GetFiles(_localFolderPath, "*.*", SearchOption.AllDirectories);
            var localFiles = localFilePaths.Select(filePath => new FileInfo(filePath)).ToList();
            NLogManager.LogInfo(JsonConvert.SerializeObject(localFilePaths));

            var reLocalFiles = localFiles.Select(fi => new FileSyncInfo
            {
                FileName = fi.FullName.ReplaceFirst(_localFolderPath, "").Replace("\\", "/"),
                Length = fi.Length,
                LastModified = Directory.GetLastWriteTimeUtc(fi.FullName),
                CreatedTime = Directory.GetCreationTimeUtc(fi.FullName),
                Hash = GetHash(fi.FullName.ReplaceFirst(_localFolderPath, "").Replace("\\", "/") + fi.Length)
            }).ToList();

            return reLocalFiles;
        }

        public static DirectoryTransferContext GetDirectoryTransferContext(TransferCheckpoint checkpoint)
        {
            DirectoryTransferContext context = new DirectoryTransferContext(checkpoint);
            return context;
        }
        public async Task TransferLocalDirectoryToAzureBlobDirectory(string localDirectoryPath)
        {
            NLogManager.LogInfo("Updating folder");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
            container.CreateIfNotExistsAsync().Wait();

            CloudBlobDirectory blobDirectory = container.GetDirectoryReference("");

            TransferCheckpoint checkpoint = null;
            DirectoryTransferContext context = GetDirectoryTransferContext(checkpoint);
            CancellationTokenSource cancellationSource = new CancellationTokenSource();

            Stopwatch stopWatch = Stopwatch.StartNew();
            Task task;
            UploadDirectoryOptions options = new UploadDirectoryOptions()
            {
                Recursive = true
            };
            try
            {
                task = TransferManager.UploadDirectoryAsync(localDirectoryPath, blobDirectory, options, context, cancellationSource.Token);
                await task;
            }
            catch (Exception e)
            {
                NLogManager.LogInfo("Updating folder Error: " + e);
            }
            // stopWatch.Stop();
        }
        /// <summary>
        /// MoveBlobBetweenContainers
        /// Move a blob from source to destination container
        /// </summary>
        /// <param name="srcBlob"></param>
        /// <param name="destContainer"></param>
        private void MoveBlobBetweenContainers(string fileName)
        {
            NLogManager.LogInfo("MoveBlobBetweenContainers " + fileName);
            var sourceBlobContainerReference = GetBlobContainerReference(_connectionString, _containerName);
            var destinationBlobContainerReference = GetBlobContainerReference(_connectionString, _containerName);

            var sourceBlob = sourceBlobContainerReference.GetBlockBlobReference(fileName);
            CloudBlockBlob destBlob;

            //Copy source blob to destination container
            using (MemoryStream memoryStream = new MemoryStream())
            {
                sourceBlob.DownloadToStreamAsync(memoryStream);

                //put the time stamp

                var newBlobName = ".LastVersion/" + sourceBlob.Name.Split('.')[0] + "/" + sourceBlob.Name;

                destBlob = destinationBlobContainerReference.GetBlockBlobReference(newBlobName);
                //copy source blob content to destination blob
                destBlob.StartCopyAsync(sourceBlob);

            }
            //remove source blob after copy is done.
            // sourceBlob.DeleteAsync();
        }

        /// <summary>
        /// GetBlobContainerReference
        /// Gets a reference to the CloudBlobContainer
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static CloudBlobContainer GetBlobContainerReference(string connectionString, string containerName)
        {
            //Get the Microsoft Azure Storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //Create the Blob service client.
            CloudBlobClient blobclient = storageAccount.CreateCloudBlobClient();

            //Returns a reference to a Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer object with the specified name.
            CloudBlobContainer blobcontainer = blobclient.GetContainerReference(containerName);


            return blobcontainer;
        }

        public async Task upload_ToBlob(string fileName, string localPath)
        {
            NLogManager.LogInfo("upload_ToBlob " + localPath);
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            blobClient.Upload(localPath, overwrite: true);
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
    }
}