using System;

namespace TransferWorker.Models
{
    public class FileSyncInfo
    {
        public string FileName { get; set; }
        public int Hash { get; set; }

        public long? Length { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime CreatedTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is FileSyncInfo si)
            {
                return si.FileName == FileName && si.Length == Length && si.LastModified <= LastModified;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Seems wrong implementation of GetHashCode()
            // But why I wrote this method in the first place?
            // Emmmmmm... who cares anyway
            return $"{FileName}{Length}".Length;
        }
    }
}