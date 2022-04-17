using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace Parser
{
    /// <summary>
    /// Логика взаимодействия для Show.xaml
    /// </summary>
    public partial class Show : Window
    {
        bool isShort;
        private int PageSize = 25;
        private int TotalPage = 0;
        private DataTable DataSource;
        private int _CurrentPage = 1;

        private void btnPrevPage_Click(object sender, System.EventArgs e)
        {
            if (_CurrentPage == 1)
            {
                MessageBox.Show("Вы уже на первой странице.");
            }
            else
            {
                _CurrentPage -= 1;
                dataTable.ItemsSource = new DataView(ShowData(_CurrentPage));
            }
        }

        private void btnNxtPage_Click(object sender, System.EventArgs e)
        {
            if (_CurrentPage == TotalPage)
            {
                MessageBox.Show("Вы уже на последней странице.");
            }
            else
            {
                _CurrentPage += 1;
                dataTable.ItemsSource = new DataView(ShowData(_CurrentPage));
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!isShort)
            {
                DataGridRow temp = (DataGridRow)sender;
                DataRowView item = dataTable.Items[temp.GetIndex()] as DataRowView;
                int index = Int32.Parse(item.Row.ItemArray[0].ToString().Substring(4));
                new Show(index).Show();
            }
        }

        public void DataBind(DataTable dt)
        {
            DataSource = dt;
            dataTable.ItemsSource = new DataView(ShowData(1));
        }

        private DataTable ShowData(int pageNumber)
        {
            DataTable dt = new DataTable();
            TotalPage = (DataSource.Rows.Count / PageSize) + 1;
            int startIndex = PageSize * (pageNumber - 1);
            var result = DataSource.AsEnumerable().Where((s, k) => (k >= startIndex && k < (startIndex + PageSize)));

            foreach (DataColumn colunm in DataSource.Columns)
            {
                dt.Columns.Add(colunm.ColumnName);
            }

            foreach (var item in result)
            {
                dt.ImportRow(item);
            }
            txtPaging.Text = string.Format("Страница {0} из {1}", pageNumber, TotalPage);
            return dt;
        }

        public Show()
        {
            InitializeComponent();
            labelNotify.Content = "Для просмотра полной информации кликните два раза по строке";
            DataBind(Data.DataSourceShort);
        }

        public Show(int index)
        {
            Data.DataCreateFullTable("thrlist_formatted");
            Data.DataCreateShortTable();
            InitializeComponent();
            isShort = true;
            DataTable dt1 = Data.DataSourceFull.Clone();
            foreach (DataRow item in Data.DataSourceFull.Rows)
            {
                if (item["Идентификатор УБИ"].ToString() == index.ToString())
                {
                    int index2 = Data.DataSourceFull.Rows.IndexOf(item);
                    dt1.Rows.Add(Data.DataSourceFull.Rows[index2].ItemArray);
                }
            }
            //dt1.Rows.Add(Data.DataSourceFull.Rows[index].ItemArray);
            DataBind(dt1);
        }

    }
}
