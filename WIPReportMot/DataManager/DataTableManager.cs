using System;
using SQL = System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using WIPReportMot.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WIPReportMot.DataManager
{
    public class DataTableManager
    {
        //public & private instance variables
        public string FileName { get; set; }
        private Excel.Application _Excel { get; set; }
        private Excel._Workbook _WorkBook { get; set; }
        private Excel._Worksheet WorkSheet { get; set; }

        //virtual method
        /// <summary>
        /// Create an Excel file
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void CreateExcel(ref string fileName)
        {
            //get fileName by connection
            this.FileName = Connection.Instance.GetFileName();
            fileName = this.FileName;
            /*-------------------------*/
            //Create Excel objects
            #region Create Excel objects

            _Excel = new Excel.Application();
            //hide excel windwow when generating
            _Excel.Visible = false;
            _Excel.DisplayAlerts = false;

            _WorkBook = _Excel.Workbooks.Add(Missing.Value);

            WorkSheet = _WorkBook.ActiveSheet;
            #endregion

        }

        //virtual method
        /// <summary>
        /// Create an wookSheet of the Excel
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="sheetName"></param>
        public virtual void CreatSheet(SQL.DataTable dataTable, string sheetName)
        {
            /*-------------------------*/
            //create work sheet name
            #region create a woork sheet by name
            WorkSheet = (Excel._Worksheet)_Excel.Worksheets.Add();
            WorkSheet.Name = sheetName;
            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine("Create a new work sheet[{0}]", sheetName);
            #endregion

            /*-------------------------*/
            //Add column names to excel sheet
            #region add sheet header and set sheet width and font
            string[] colNames = new string[dataTable.Columns.Count];
            int col = 0;
            //fetch column names from dtData
            foreach (SQL.DataColumn dc in dataTable.Columns)
            {
                colNames[col++] = dc.ColumnName;
            }

            //last column for english alphabet
            //char lastColumn = (char)(65 + dataTable.Columns.Count - 1);
            string lastColumnName = GetExcelColumnName(dataTable.Columns.Count);
            Console.WriteLine("lastColumn: " + lastColumnName);
            //assign data to column headers
            WorkSheet.get_Range("A1", lastColumnName + "1").Value2 = colNames;

            //set width
            WorkSheet.Columns.AutoFit();
            //set columns format to text format
            WorkSheet.Columns.NumberFormat = "@";

            //set column headers' font to bold
            //WorkSheet.get_Range("A1", lastColumnName + "1").Font.Bold = true;
            #endregion

            /*-------------------------*/
            // Add DataRows data to Excel
            #region New method for add DataRows data to Excel --- Slower

            Console.WriteLine("Generating data for Sheet[{0}]...", sheetName);

            string data = null;

            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dataTable.Columns.Count - 1; j++)
                {
                    data = dataTable.Rows[i].ItemArray[j].ToString();
                    WorkSheet.Cells[i + 2, j + 1] = data;
                }
            }
            #endregion

            Console.WriteLine("Sheet [{0}] created!", sheetName);
            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Save the excel file
        /// </summary>
        public virtual void SaveExcel()
        {
            /*-------------------------*/
            //Save Data Excel sheet
            #region Save Data to Excel sheet
            _Excel.Visible = false;
            _Excel.DisplayAlerts = false;

            //save without prompt
            _Excel.UserControl = true;
            _WorkBook.SaveAs(this.FileName, AccessMode: Excel.XlSaveAsAccessMode.xlExclusive);
            // Cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(WorkSheet);
            _WorkBook.Close();
            Marshal.ReleaseComObject(_WorkBook);
            _Excel.Quit();
            Marshal.ReleaseComObject(_Excel);
            Console.WriteLine("Success saved Excel to:" + FileName);
            #endregion
        }


        /// <summary>
        /// Convert a number to an Excel column name
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
