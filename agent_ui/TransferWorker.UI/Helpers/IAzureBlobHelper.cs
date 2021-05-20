using System.Collections.Generic;
using System.Threading.Tasks;
using TransferWorker.UI.Models;

namespace TransferWorker.UI.Helpers
{
    public interface IAzureBlobHelper
    {
        string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType);

        //void DeleteBlobData(string fileUrl);
        Task UploadFile(string fileName);

        Task Compare();
        Task Download(List<FileViewInfo> files,string path);
        Task<List<FileSyncInfo>> GetListFileFromCloud(string containerName, int idAppSetting, string connection_string);
        Task<List<FileSyncInfo>> GetListFileFromCloud();

        List<FileSyncInfo> GetListFileFromLocal();
    }
}