using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing; // Для System.Drawing.Icon

namespace MDToolsV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private void AddFileToPanel(string filePath)
        {
            var fileItem = new Border
            {
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(5),
                CornerRadius = new CornerRadius(3),
                Tag = filePath
            };

            // Создаем иконку на основе расширения файла
            var icon = new Image
            {
                Source = new DrawingImage(),
                Width = 16,
                Height = 16,
                Stretch = Stretch.Uniform
            };

            fileItem.Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
        {
            icon,
            new TextBlock
            {
                Text = System.IO.Path.GetFileName(filePath),
                TextTrimming = TextTrimming.CharacterEllipsis,
                ToolTip = filePath,
                Margin = new Thickness(5, 0, 0, 0)
            }
        }
            };

            FilesStackPanel.Children.Add(fileItem);
        }

        private ObservableCollection<string> _droppedFiles = new ObservableCollection<string>();
        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            // Подсветка при входе файла в область
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }
 
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            // Подтверждение, что это файлы (а не текст или что-то еще)
            e.Handled = true;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    if (!FilesStackPanel.Children.OfType<Border>()
                        .Any(b => b.Tag.ToString().Equals(file, StringComparison.OrdinalIgnoreCase)))
                    {
                        AddFileToPanel(file);
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //FilesStackPanel.ItemSourse = _droppedFiles;
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}