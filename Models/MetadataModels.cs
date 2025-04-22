using System.Collections.ObjectModel;

namespace MetadataAnalyzer.Models
{
    public class MetadataItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string tagEXIF { get; set; }
    }

    public class FileMetadata
    {
        public string FilePath { get; set; }
        public string FileName => System.IO.Path.GetFileName(FilePath);
        public ObservableCollection<MetadataItem> Metadata { get; set; } = new();
        public bool HasExif { get; set; }
        public bool IsModified { get; set; }
    }
}