using ClosedXML.Excel;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Data
    {
        public static DataTable DataSourceShort;
        public static DataTable DataSourceFull;
        public static DataTable DataSourceFullUpdated;

        public static void DataCreateAndFormat(string name1, string name2)
        {
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\" + name1 + ".xlsx";
            using (var stream = File.Open(baseFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    if (name1 == "thrlist")
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true,
                                FilterColumn = (columnReader, columnIndex) =>
                                {
                                    string header = columnReader.GetString(columnIndex);
                                    return !(header == "Дата включения угрозы в БнД УБИ" || header == "Дата последнего изменения данных");
                                },
                                ReadHeaderRow = (rowReader) =>
                                {
                                    rowReader.Read();
                                }
                            }
                        });
                        DataTable dt = result.Tables[0];
                        XLWorkbook wb = new XLWorkbook();
                        wb.Worksheets.Add(dt, "Sheet");
                        wb.Worksheet("Sheet").ColumnWidth = 30;
                        wb.Worksheet("Sheet").RowHeight = 20;
                        wb.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\" + name2 + ".xlsx");
                    }
                    else if (name1 == "thrlist_formatted_update")
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        DataTable dt = result.Tables[0];
                        XLWorkbook wb = new XLWorkbook();
                        wb.Worksheets.Add(dt, "Sheet");
                        wb.Worksheet("Sheet").ColumnWidth = 30;
                        wb.Worksheet("Sheet").RowHeight = 20;
                        wb.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\" + name2 + ".xlsx");
                    }
                }
            }
            File.Delete(baseFile);
        }

        public static void DataCreateFullTable(string name)
        {
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\" + name + ".xlsx";
            using (var stream = File.Open(baseFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    if (name == "thrlist_formatted")
                    {
                        Data.DataSourceFull = result.Tables[0];
                    }
                    else if (name == "thrlist_formatted_update")
                    {
                        Data.DataSourceFullUpdated = result.Tables[0];
                    }
                }

            }
        }

        public static void DataCreateShortTable()
        {
            var baseFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base\\thrlist_formatted.xlsx";
            using (var stream = File.Open(baseFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {

                            UseHeaderRow = true,
                            FilterColumn = (columnReader, columnIndex) =>
                            {
                                string header = columnReader.GetString(columnIndex);
                                return (header == "Наименование УБИ" || header == "Идентификатор УБИ");
                            }
                        }
                    });
                    DataTable dt = result.Tables[0];
                    dt.Columns[0].ColumnName = "Идентификатор угрозы";
                    foreach (DataRow item in dt.Rows)
                    {
                        item["Идентификатор угрозы"] = $"УБИ.{item[0].ToString()}";
                    }
                    Data.DataSourceShort = dt;

                }

            }
        }
    }

}
