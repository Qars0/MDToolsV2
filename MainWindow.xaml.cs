using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDTools.Models;
using MetadataExtractor;
using ExifLibrary;
using System.ComponentModel;


namespace MDTools
{
    public partial class MainWindow : Window
    {
        // Коллекция для хранения метаданных файлов
        public ObservableCollection<FileMetadata> Files { get; } = new();
        
        // Файл который выбран в данный момент
        private FileMetadata _selectedFile;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            FilesList.ItemsSource = Files;

            // Обработчики событий DragAndDrop
            FilesList.Drop += FilesList_Drop;                           // Обработчик перетаскивания
            FilesList.DragOver += FilesList_DragOver;                   // Обработчик наведения на область перетаскивания
            FilesList.SelectionChanged += FilesList_SelectionChanged;   // Обработчик изменения выбранного файла
        }

        // Определение обработчика наведения на область перетаскивания
        private void FilesList_DragOver(object sender, DragEventArgs e)
        {
            // Проверка на содержание файлов
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy  // Разрешаем копирование если есть файлы
                : DragDropEffects.None; // Запрещяем если фалов нет
        }

        // Обработчик перетаскивания
        private void FilesList_Drop(object sender, DragEventArgs e)
        {
            // Берем массив перетащеных файлов
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                // Проходим по массиву и читаем каждый файл
                foreach (var file in files)
                {
                    // В переменной file содержится адресс до этого файла

                    if (File.Exists(file))  // Проверка, является ли файлом
                    {
                        ReadFile(file);     // Начинаем обработку этого файла
                    }
                }
            }
        }

        // Обработчик изменения выбранного файла
        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFile = FilesList.SelectedItem as FileMetadata;
            MetadataGrid.ItemsSource = _selectedFile?.Metadata;
        }

        private void SaveMetadataToFile(FileMetadata fileMetadata)
        {
            // TODO
        }

        // Обработка чтения файла
        private void ReadFile(string filePath)
        {
            // Создаем класс FileMetadata для текущего файла
            var fileMetadata = new FileMetadata { FilePath = filePath };

            try
            {
                // Считываем метаданные из файла и записываем в переменную metadata
                var metadata = ImageMetadataReader.ReadMetadata(filePath);
                
                // Если количество метаданных в файле больше нуля начинаем проход
                if (metadata.Count > 0)
                {
                    // Помечаем файл содержащий EXIF
                    fileMetadata.HasExif = true;
                    
                    // Перебор директорий метаданных (EXIF, IPTC и т.д.)
                    foreach (var directory in metadata)
                    {
                        // Перебираем теги в каждой директории
                        foreach (var tag in directory.Tags)
                        {
                            // Добавляем метаданные в ранее созданный fileMetadata
                            fileMetadata.Metadata.Add(new MetadataItem
                            {
                                Name = tag.Name,            // Присваиваем имя 
                                Value = tag.Description,    // Присваиваем значение
                                tagEXIF = tag.TagName       // Присваиваем тег
                                
                            });
                        }
                    }
                }

                Files.Add(fileMetadata); // Добавляем этот файл в коллекцию
            }
            catch (Exception ex)
            {
                // Выводим ошибку, если она произошла при обработке файла
                MessageBox.Show($"Ошибка обработки файла: {ex.Message}\nПуть до файла: {filePath}");
            }
        }

        // Обработчик нажатия на кнопку сохранить
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            if (_selectedFile != null)
            {
                MessageBox.Show($"Сохранено: {_selectedFile.FileName}");
                // Сообщение является заглушкой
            }
        }

        // Обработчик нажатия на кнопу сохранить все
        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            MessageBox.Show($"Сохранено {Files.Count} файлов"); 
            // Сообщение является заглушкой
        }

        // Обработчик нажатия на кнопку удалить выбранное
        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            while(MetadataGrid.SelectedItems.Count > 0)
            {   // Запускаем цикл пока выбранных строк в таблице не останится 
                if (_selectedFile != null && MetadataGrid.SelectedItem is MetadataItem item)
                {
                    // Удаляем последнюю выбранную строку
                    _selectedFile.Metadata.Remove(item);
                }
            }
        }

        // Обработчик нажатия на кнопку удалить все
        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем выбран ли файл
            if (_selectedFile != null)
            {
                // Удаляем все строки
                _selectedFile.Metadata.Clear();
            }
        }

        // Обработчик нажатия на кнопку добавить строку с метаданными
        private void AddMetadata_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем выбран ли файл
            if (_selectedFile != null)
            {
                // Создаем новую строку для хранения метаданных
                var MDLine = new MetadataItem 
                {
                    Name = "MDUserData",
                    tagEXIF = "37510",
                    Value = ""
                };
                // Добавляем ее в выбранный файл
                _selectedFile.Metadata.Add(MDLine);
            }
        }
    }
}