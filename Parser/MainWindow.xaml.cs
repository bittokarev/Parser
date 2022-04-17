using System;
using System.Windows;
using System.IO;
using System.Net;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Parser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist.xlsx";
            var baseFile2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist_formatted.xlsx";

            if (!File.Exists(baseFile2))
            {
                System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show
                    ("Базы данных не существует. Создать базу?", "Внимание!", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base");
                    try
                    {
                        new WebClient().DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", baseFile);
                        Data.DataCreateAndFormat("thrlist", "thrlist_formatted");
                        Data.DataCreateShortTable();
                        Data.DataCreateFullTable("thrlist_formatted");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Произошла ошибка. Попробуйте позднее...", "Внимание!");
                        Application.Current.Shutdown();
                    }

                }
                else
                {
                    Environment.Exit(0);
                }

            }
            else
            {
                Data.DataCreateShortTable();
                Data.DataCreateFullTable("thrlist_formatted");
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist.xlsx";
                new WebClient().DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", baseFile);
                Data.DataCreateAndFormat("thrlist", "thrlist_formatted_update");
                Data.DataCreateFullTable("thrlist_formatted_update");
                new Update();
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка. Попробуйте позднее...", "Внимание!");
            }


        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.DataCreateShortTable();
                Data.DataCreateFullTable("thrlist_formatted");
                new Show().Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка базы данных. Перезапустите программу.", "Внимание!");
                Application.Current.Shutdown();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist_formatted.xlsx";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.FileName = "База данных ФСТЭК";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    System.IO.File.Copy(baseFile, saveFileDialog.FileName);
                }
                catch
                {
                    System.IO.File.Delete(saveFileDialog.FileName);
                    System.IO.File.Copy(baseFile, saveFileDialog.FileName);
                }
            }
        }
    }
}
