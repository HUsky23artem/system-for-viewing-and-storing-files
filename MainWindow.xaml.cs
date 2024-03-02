using Syncfusion.XPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using система_для_хранения_pdf_файлов.Models;
using System.Windows.Forms;
using Path = System.IO.Path;
namespace система_для_хранения_pdf_файлов
{
    public partial class MainWindow : Window
    {
        private Table_Files table_Files = new Table_Files();
       // private ObservableCollection<MyDataModel> dataItems = new ObservableCollection<MyDataModel>();
        public MainWindow()
        {
            InitializeComponent();
            new_file();

            TreeViewItem rootNodePdf = new TreeViewItem();
            rootNodePdf.Header = Pdf_File_Path;
            treeView_pdf.Items.Add(rootNodePdf);

            TreeViewItem rootNodeImage = new TreeViewItem();
            rootNodeImage.Header = Image_File_Path;
            treeView_image.Items.Add(rootNodeImage);

            // SaveToDatabase();
            //DataGrid_File_DB.ItemsSource = dataItems;
            //DataContext = table_Files;
            //DataGrid_File_DB.ItemsSource = Files_DBEntities.GetContext().Table_Files.ToString();
        }
        public string Pdf_File_Path = @"C:\Users\Пользователь\source\repos\система для хранения pdf файлов\система для хранения pdf файлов\PDF_Files";
        public string Image_File_Path = @"C:\Users\Пользователь\source\repos\система для хранения pdf файлов\система для хранения pdf файлов\Image_Files";

        private void SaveToDatabase()
        {
            string connectionString = "Data Source=-PC\\SQL_SERVE;Initial Catalog=Viewr_DB;Integrated Security=True;Encrypt=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Пример запроса на вставку данных в таблицу
                string query = "INSERT INTO Table_Files (Name_image, Link_image) VALUES (@Name_image,@Link_image)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name_image", "1");
                    command.Parameters.AddWithValue("@Link_image", "1");

                    command.ExecuteNonQuery();
                }
            }
        }
        private void new_file()
        {
            try
            {
                // Проверяем существование папки
                if (!Directory.Exists(Pdf_File_Path))
                {
                    // Создаем каталог для папки
                    Directory.CreateDirectory(Pdf_File_Path);
                }
                if (!Directory.Exists(Image_File_Path))
                {
                    // Создаем каталог для папки
                    Directory.CreateDirectory(Image_File_Path);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при создании каталога: " + ex.Message);
            }
        }
        private void treeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            TreeViewItem rootNode = new TreeViewItem();
            if (files.Length > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(files[0]).ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                {
                    foreach (string file in files)
                    {
                        string fileName = System.IO.Path.GetFileName(file);
                        string destinationPath = System.IO.Path.Combine(Image_File_Path, fileName);
                        System.IO.File.Copy(file, destinationPath, true);
                    }

                    rootNode.Header = Image_File_Path; //// Добавление корневого элемента в TreeView
                    treeView_image.Items.Add(rootNode); // Заполнение TreeView
                }
                else if (fileExtension == ".pdf")
                {
                    PdfView.Load(files[0]);
                    image.Visibility = Visibility.Hidden;
                    PdfView.Visibility = Visibility.Visible;

                    string new_Pdf_FilePath = System.IO.Path.Combine(Pdf_File_Path, System.IO.Path.GetFileName(files[0]));
                    System.IO.File.Move(files[0], new_Pdf_FilePath);
                }
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            //MyDataModel newItem = new MyDataModel();
            //dataItems.Add(newItem);


        }
        private object previousValue;
         private void pdf_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFile_pdf = pdf_List.SelectedItem as string; // отслеживание пути pdf файла
            if (selectedFile_pdf != null)
            {
                PdfView.Load(selectedFile_pdf); // вызов pdf файла
            }
        }

        private void image_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFile_image = image_List.SelectedItem as string; // отслеживание пути pdf файла
            if (selectedFile_image != null)
            {
                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFile_image);
                bitmap.EndInit();

                image.Source = bitmap;

            }
        }

        private void DataGrid_File_DB_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            previousValue = ((Table_PDF_Files)e.Row.Item).name_pdf;
            previousValue = ((Table_Image_Files)e.Row.Item).name_image;
        }

        private void DataGrid_File_DB_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Получаем измененное значение из редактируемой ячейки
                var textBox = e.EditingElement as System.Windows.Controls.TextBox;
                string newValue = textBox.Text;

                // Обновляем переменную с новым значением
                ((Table_PDF_Files)e.Row.Item).name_pdf = newValue; // Здесь YourDataObject - класс данных
                ((Table_Image_Files)e.Row.Item).name_image = newValue;
                // Теперь у вас в переменной newValue будет новое значение, которое вписали в ячейку
            }
        }
        public class MyDataModel
        {
            public string name_pdf { get; set; }
            public string link_pdf_file { get; set; }
            public string name_image { get; set; }
            public string link_image_files { get; set; }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid_File_DB.SelectedItem != null)
            {
                MyDataModel selectedData = (MyDataModel)DataGrid_File_DB.SelectedItem;

                // Далее вы можете использовать данные из selectedData для вашей логики
                string selectedPDFName = selectedData.name_pdf;
                string selectedPDFLink = selectedData.link_pdf_file;
                string selectedImageName = selectedData.name_image;
                string selectedImageLink = selectedData.link_image_files;

                OutputTextBox.Text = $"Selected PDF Name: {selectedPDFName}\nSelected PDF Link: {selectedPDFLink}\nSelected Image Name: {selectedImageName}\nSelected Image Link: {selectedImageLink}";
            }
        }

            private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] pdf_files = Directory.GetFiles(Pdf_File_Path, "*.pdf");
            pdf_List.ItemsSource = pdf_files; // загрузка списка .pdf файлов

            string[] image_files = Directory.GetFiles(Image_File_Path, "*.jpg");
            image_List.ItemsSource = image_files; // загрузка списка .jpg файлов
        }
    }
}
