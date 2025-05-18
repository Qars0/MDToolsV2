using ExifLibrary;
using System.Collections.ObjectModel;

namespace MDTools.Models
{
    // Определение класса для использования метаданных в таблице
    public class MetadataItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int tagEXIF { get; set; }
    }

    // Оперделение класса для общего использования метаданных файла
    public class FileMetadata
    {
        public string FilePath { get; set; }
        public string FileName => System.IO.Path.GetFileName(FilePath);
        public ObservableCollection<MetadataItem> Metadata { get; set; } = new();
        public bool HasExif { get; set; }
        public bool IsModified { get; set; }
    }
}








