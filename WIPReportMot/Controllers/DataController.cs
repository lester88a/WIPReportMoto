using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQL = System.Data;
using WIPReportMot.Configuration;
using WIPReportMot.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace WIPReportMot.Controllers
{
    public class DataController
    {
        // This method is used to convert datatable to json string
        public ObservableCollection<RepairModel> GetAllRepairData()
        {
            //Connection connection = new Connection();
            string connectionString = Connection.Instance.GetConnectionString();
            string queryAll = Connection.Instance.GetQueryAll();

            SQL.DataTable dt = new SQL.DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(queryAll, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;

                    foreach (SQL.DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (SQL.DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    //conver json string to json obejct
                    var items = JsonConvert.DeserializeObject<ObservableCollection<RepairModel>>(JsonConvert.SerializeObject(rows));
                    System.Console.WriteLine("DataController items.Count: " + items.Count);
                    return items;
                }
            }
        }

        
    }
}
