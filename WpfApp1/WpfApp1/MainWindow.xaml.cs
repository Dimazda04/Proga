using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using MessageBox = System.Windows.MessageBox;
using System.Windows.Shapes;

namespace Application_Form
{
    /// <summary>
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        private int maxImagesPerRow = 5;
        private int currentImageCount = 0;
        private string notesFolderMyapp = @"C:\MyAPP";
        private string notesFolderPath = @"C:\MyAPP\Note";
        private string notesFolderIMG = @"C:\MyAPP\Img";
        public Window3()
        {
            InitializeComponent();

            // Создание папки для заметок, если она не существует
            if (!Directory.Exists(notesFolderMyapp))
                Directory.CreateDirectory(notesFolderMyapp);

            InitializeComponent();

            // Создание папки для заметок, если она не существует
            if (!Directory.Exists(notesFolderPath))
                Directory.CreateDirectory(notesFolderPath);

            InitializeComponent();

            // Создание папки для заметок, если она не существует
            if (!Directory.Exists(notesFolderIMG))
                Directory.CreateDirectory(notesFolderIMG);
            // Проверка и вывод сохраненных заметок
            UpdateNotesList();

            // Проверка и вывод сохраненных фотографий в галерее
            LoadGalleryImages();
        }
        private void LoadGalleryImages()
        {
            try
            {
                // Получение пути к папке с изображениями галереи
                string[] imageFiles = Directory.GetFiles(notesFolderIMG, "*.*", SearchOption.AllDirectories);

                foreach (string imageFile in imageFiles)
                {
                    // Создание нового изображения и добавление его на форму
                    Image image = new Image();
                    image.Width = 100;
                    image.Height = 100;
                    image.Margin = new Thickness(5);
                    image.Source = new BitmapImage(new Uri(imageFile));
                    image.MouseUp += Image_MouseUp; // Добавление обработчика события MouseUp
                    ImagesWrapPanel.Children.Add(image);

                    // Увеличение счетчика изображений
                    currentImageCount++;

                    // Проверка, нужно ли перенести изображения на новый ряд
                    if (currentImageCount % maxImagesPerRow == 0)
                        ImagesWrapPanel.Children.Add(new TextBlock());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке изображений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image clickedImage = (Image)sender;
            string imagePath = clickedImage.Source.ToString();

            // Открытие выбранной фотографии в приложении "Фотографии" от Windows
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при открытии фотографии: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                // Определение нового пути для сохранения изображения в папке "Img"
                string newImagePath = System.IO.Path.Combine(notesFolderIMG, System.IO.Path.GetFileName(imagePath));

                try
                {
                    // Копирование изображения в папку "Img"
                    File.Copy(imagePath, newImagePath, true);

                    // Создание нового изображения и добавление его на форму
                    Image image = new Image();
                    image.Width = 100;
                    image.Height = 100;
                    image.Margin = new Thickness(5);
                    image.Source = new BitmapImage(new Uri(newImagePath)); // Используем новый путь для изображения
                    ImagesWrapPanel.Children.Add(image);

                    // Увеличение счетчика изображений
                    currentImageCount++;

                    // Проверка, нужно ли перенести изображения на новый ряд
                    if (currentImageCount % maxImagesPerRow == 0)
                        ImagesWrapPanel.Children.Add(new TextBlock());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при добавлении изображения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            string noteTitle = NoteTitleTextBox.Text;
            string noteText = NoteTextTextBox.Text;

            if (string.IsNullOrWhiteSpace(noteTitle) || string.IsNullOrWhiteSpace(noteText))
            {
                MessageBox.Show("Заполните название и текст заметки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string noteFilePath = System.IO.Path.Combine(notesFolderPath, noteTitle + ".txt");

                // Запись заметки в файл
                File.WriteAllText(noteFilePath, noteText);

                // Очистка полей ввода
                NoteTitleTextBox.Clear();
                NoteTextTextBox.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении заметки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedNote = (string)NotesListBox.SelectedItem;

            if (string.IsNullOrWhiteSpace(selectedNote))
            {
                MessageBox.Show("Выберите заметку для просмотра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string noteFilePath = System.IO.Path.Combine(notesFolderPath, selectedNote + ".txt");

                if (File.Exists(noteFilePath))
                {
                    // Открытие заметки в блокноте
                    System.Diagnostics.Process.Start("notepad.exe", noteFilePath);
                }
                else
                {
                    MessageBox.Show("Выбранная заметка не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при открытии заметки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProfilePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                // Определяем новый путь для сохранения изображения
                string newImagePath = System.IO.Path.Combine(notesFolderIMG, System.IO.Path.GetFileName(imagePath));

                try
                {
                    // Копируем изображение в новую папку
                    File.Copy(imagePath, newImagePath);

                    // Отображение выбранной фотографии
                    ProfileImage.Source = new BitmapImage(new Uri(newImagePath));

                    MessageBox.Show("Изображение успешно сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при сохранении изображения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateNotesList()
        {
            // Очистка списка
            NotesListBox.Items.Clear();

            // Получение заголовков заметок из папки
            string[] noteFiles = Directory.GetFiles(notesFolderPath, "*.txt");

            // Добавление заголовков заметок в список
            foreach (string noteFile in noteFiles)
            {
                string noteTitle = System.IO.Path.GetFileNameWithoutExtension(noteFile);
                NotesListBox.Items.Add(noteTitle);

            }
        }
        private void ChangeProfileImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

                // Создаем маску для обрезки изображения с учетом радиуса границы
                double borderRadius = 50;
                double targetWidth = 100;
                double targetHeight = 100;

                EllipseGeometry maskGeometry = new EllipseGeometry(new Point(targetWidth / 2, targetHeight / 2), borderRadius, borderRadius);

                // Создаем обрезанное изображение с примененной маской
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.PushClip(maskGeometry);
                    drawingContext.DrawImage(bitmapImage, new Rect(new Size(targetWidth, targetHeight)));
                }

                RenderTargetBitmap croppedBitmap = new RenderTargetBitmap((int)targetWidth, (int)targetHeight, 96, 96, PixelFormats.Pbgra32);
                croppedBitmap.Render(drawingVisual);

                // Устанавливаем обрезанное изображение в элемент Image
                ProfileImage.Source = croppedBitmap;
            }
        }


        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Сохранить изменения профиля (имя, фамилию, возраст)
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;

            // Действия по сохранению данных профиля

            MessageBox.Show("Профиль успешно сохранен!");
        }

        private void Button_Exit3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Min3_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}