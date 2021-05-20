using System.Threading.Tasks;

namespace TransferWorker.Helpers
{
    public interface IAzureBlobHelper
    {
        Task UploadFile(string fileName);
        Task TransferLocalFileToAzureBlob(string fileName);

        Task Compare();
    }
}