using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MetadataAnalyzer.Models;
using MetadataExtractor;
using ExifLibrary;
using System.ComponentModel;

namespace MetadataAnalyzer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<FileMetadata> Files { get; } = new();
        private FileMetadata _selectedFile;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            FilesList.ItemsSource = Files;
            FilesList.Drop += FilesList_Drop;
            FilesList.DragOver += FilesList_DragOver;
            FilesList.SelectionChanged += FilesList_SelectionChanged;
        }

        private void FilesList_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void FilesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        ProcessFile(file);
                    }
                }
            }
        }
        private void SaveMetadataToFile(FileMetadata fileMetadata)
        { 
            var imageFile = ImageFile.FromFile(fileMetadata.FilePath);

            imageFile.Properties.Clear();
            foreach (var item in fileMetadata.Metadata)
            {
                try
                {
                    ExifProperty property;
                    //ExifTag tag = item.Name;
                    property.Tag = item.Name;
                    property.Name = item.Name;
                    property.Value = item.Value;

                    if (property != null)
                    {
                        imageFile.Properties.Add(property);
                    }
                }
                catch
                {
                    
                }
            }
            imageFile.Save(fileMetadata.FilePath);
        }

        private void ProcessFile(string filePath)
        {
            var fileMetadata = new FileMetadata { FilePath = filePath };

            try
            {
                var metadata = ImageMetadataReader.ReadMetadata(filePath);
                if (metadata.Count > 0)
                {
                    fileMetadata.HasExif = true;
                    foreach (var directory in metadata)
                    {
                        foreach (var tag in directory.Tags)
                        {
                            fileMetadata.Metadata.Add(new MetadataItem
                            {
                                Name = tag.Name,
                                Value = tag.Description,
                                tagEXIF = tag.TagType.ToString()
                                
                            });
                            MessageBox.Show($"Вот такой тег: {tag.TagType}");
                        }
                    }
                }

                Files.Add(fileMetadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки файла: {ex.Message}");
                fileMetadata.HasExif = false;
                //Files.Add(fileMetadata);
            }
        }

        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFile = FilesList.SelectedItem as FileMetadata;
            MetadataGrid.ItemsSource = _selectedFile?.Metadata;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null)
            {
                MessageBox.Show($"Сохранено: {_selectedFile.FileName}");
            }
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Сохранено {Files.Count} файлов");
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null && MetadataGrid.SelectedItem is MetadataItem item)
            {
                _selectedFile.Metadata.Remove(item);
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile != null)
            {
                _selectedFile.Metadata.Clear();
            }
        }
    }
}