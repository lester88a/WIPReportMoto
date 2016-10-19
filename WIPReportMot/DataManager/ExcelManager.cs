using SQL = System.Data;
using System.Collections.ObjectModel;
using WIPReportMot.Controllers;
using WIPReportMot.Models;

namespace WIPReportMot.DataManager
{
    public class ExcelManager:DataTableManager
    {
        private ObservableCollection<RepairModel> AllRepairData;
        private SQL.DataTable MotoDataTable;
        private string ErrorMessage;
        //constructor
        public ExcelManager()
        {
            AllRepairData = new ObservableCollection<RepairModel>();
            //get all data by DataController
            DataController dtController = new DataController();
            AllRepairData = dtController.GetAllRepairData();

            //get cat data by CatController
            MotoController motoController = new MotoController();
            MotoDataTable = motoController.GetCatDataTable(AllRepairData, ref ErrorMessage);



        }

        public void GetExcelSheet(ref string FileName, ref string errorMessage)
        {
            errorMessage = this.ErrorMessage;
            //set work sheet name
            #region Local variables for sheet name
            string sheetNameCat = "MOTOROLA";
            #endregion

            //create excel woork
            #region Create Excel
            string fileName = null;
            CreateExcel(ref fileName);
            FileName = fileName;
            #endregion

            //add work sheet by name
            #region Create WoorkSheet
            //call CreatSheet
            CreatSheet(MotoDataTable, sheetNameCat);

            #endregion

            //save excel sheet
            #region Save Excel
            SaveExcel();
            #endregion

        }


        //methods overried
        public override void CreateExcel(ref string fileName)
        {
            base.CreateExcel(ref fileName);
        }

        public override void CreatSheet(SQL.DataTable dataTable, string sheetName)
        {
            base.CreatSheet(dataTable, sheetName);
        }

        public override void SaveExcel()
        {
            base.SaveExcel();
        }
    }
}
