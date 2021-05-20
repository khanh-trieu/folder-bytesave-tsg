using ReactiveUI;
using System;
using System.Reactive;

namespace TransferWorker.UI.Models
{
    public class FileSyncInfo
    {
        public string FileName { get; set; }
        public int Hash { get; set; }
        public long? Length { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedDisplay { get; set; }
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
    public class SizeFile
    {
        public long? Lenght { get; set; }
    }

    public class FileViewInfo : FileSyncInfo
    {
        public string OriginalPath { get; set; }
        public string IsFolder { get; set; }
        public string IsFile { get; set; }
        public bool isChecked { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public string Parent { get; set; }
        public int ParentId { get; set; }
        public ReactiveCommand<FileViewInfo, Unit> Select { get; set; }
       // public ReactiveCommand<FileViewInfo, Unit> Ticked { get; set; }
    }
}