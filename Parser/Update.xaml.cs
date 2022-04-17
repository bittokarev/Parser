using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Data;

namespace Parser
{
    /// <summary>
    /// Логика взаимодействия для Update.xaml
    /// </summary>

    public partial class Update : Window
    {
        public Update()
        {
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist_formatted_update.xlsx";
            InitializeComponent();
            Data.DataCreateShortTable();
            Data.DataCreateFullTable("thrlist_formatted");
            IEnumerable<DataRow> addedTemp = Data.DataSourceFullUpdated.AsEnumerable().Except(Data.DataSourceFull.AsEnumerable(), DataRowComparer.Default);
            IEnumerable<DataRow> deletedTemp = Data.DataSourceFull.AsEnumerable().Except(Data.DataSourceFullUpdated.AsEnumerable(), DataRowComparer.Default);
            DataTable addedTable = Data.DataSourceFull.Clone();
            DataTable deletedTable = Data.DataSourceFull.Clone();
            DataTable changedBeforeTable = Data.DataSourceFull.Clone();
            DataTable changedAfterTable = Data.DataSourceFull.Clone();

            foreach (DataRow item in addedTemp)
            {
                foreach (DataRow item2 in deletedTemp)
                {
                    if (item["Идентификатор УБИ"].Equals(item2["Идентификатор УБИ"]))
                    {
                        changedBeforeTable.ImportRow(item2);
                        changedAfterTable.ImportRow(item);
                    }
                }
            }
            try
            {
                deletedTable = deletedTemp.AsEnumerable().Except(changedBeforeTable.AsEnumerable(), DataRowComparer.Default).CopyToDataTable();
            }
            catch (Exception) { }

            try
            {
                addedTable = addedTemp.AsEnumerable().Except(changedAfterTable.AsEnumerable(), DataRowComparer.Default).CopyToDataTable();
            }
            catch (Exception) { }

            if (changedAfterTable.Rows.Count == 0 && changedAfterTable.Rows.Count == 0 && !addedTemp.Any() && !deletedTemp.Any())
            {
                MessageBox.Show("Обновление не требуется. Локальная база данных содержит актуальную информацию.", "Ошибка");
                this.Close();
            }
            else
            {
                Data.DataCreateAndFormat("thrlist_formatted_update", "thrlist_formatted");
                changedBefore.ItemsSource = new DataView(changedBeforeTable);
                changedAfter.ItemsSource = new DataView(changedAfterTable);
                added.ItemsSource = new DataView(addedTable);
                deleted.ItemsSource = new DataView(deletedTable);
                addedLabel.Content += addedTable.Rows.Count.ToString();
                deletedLabel.Content += deletedTable.Rows.Count.ToString();
                changedLabel.Content += changedBeforeTable.Rows.Count.ToString();
                this.Show();
                MessageBox.Show("База данных обновлена!", "Успешно");
            }
            File.Delete(baseFile);
        }
    }
}
