using System;

namespace WIPReportMot.Configuration
{
    public class Connection
    {
        //Thread Safe Singleton without using locks and no lazy instantiation
        private static readonly Connection instance = new Connection();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Connection() { }
        private Connection() { }

        //connection string
        private static string ConnectionString { get; set; }
        //queries for all data
        private static string QueryAll { get; set; }
        //file name
        private static string FileName { get; set; }

        public static Connection Instance
        {
            get
            {
                //Config config = new Config();
                string dataSource = Config.Instance.GetDataSource();
                string initialCatalog = Config.Instance.GetInitialCatalog();
                string filePath = Config.Instance.GetFileSavedPath();
                string manufacturer = Config.Instance.GetManufacturer();
                
                ConnectionString = "Data Source=" + dataSource + ";Initial Catalog=" + initialCatalog + ";Integrated Security=True";

                QueryAll = @"SELECT R.RefNumber,DATEDIFF(day, R.DateIn, convert(date, GETDATE())) as Aging,
                        R.ESN,R.MSN,R.SVP,R.Type,R.TechID,R.MftrMSN,R.FuturetelMSN,R.FutureTelOTC,R.ManufacturerOTC,
                        R.Status,R.WorkCode1,R.WorkCode2,R.WorkCode3,R.Complaint1,R.Complaint2,R.Complaint3,
                        R.Problem1,R.Problem2,R.Problem3,R.Manufacturer,R.SpecialCaseNum,R.Warranty,R.Mot_Tran,
                        Format(R.DateIn, N'yyyy-MM-dd HH:mm:ss') as DateIn,Format(R.PurchaseDate, N'yyyy-MM-dd HH:mm:ss') as PurchaseDate,
			            Format(R.DateApproved, N'yyyy-MM-dd HH:mm:ss') as DateApproved,Format(R.DateReject, N'yyyy-MM-dd HH:mm:ss') as DateReject,
			            Format(R.DateEstimation, N'yyyy-MM-dd HH:mm:ss') as DateEstimation,Format(R.DateComplete, N'yyyy-MM-dd HH:mm:ss') as DateComplete,
                        R.DOAQualify,R.FuturetelLocation,R.ModelNumber,P.FaultCode,P.PartNumber,P.Qty,M.ProductCode,
                        N.repairResolutionCode1,N.repairResolutionCode2,N.repairResolutionCode3
                        FROM " + initialCatalog + @".dbo.tblRepair R
                        LEFT JOIN " + initialCatalog + @".dbo.tblRepairParts P
                        ON R.RefNumber = P.RefNumber
                        LEFT JOIN " + initialCatalog + @".dbo.tblModel M
                        ON R.ModelNumber = M.ModelNumber
                        LEFT JOIN " + initialCatalog + @".dbo.tblTelusRepairNew N
                        ON R.RefNumber = N.RefNumber
                        where R.Manufacturer = '" + manufacturer + @"' 
                        And R.DateDockOut is null
                        And R.Status != 'X' and R.Status!='C' and R.DealerID!='7430'
                        And (R.SVP !='TCC' and R.SVP !='KCC' and R.SVP !='TEXPRESS' and R.SVP !='KEXPRESS')
                        And R.FuturetelLocation != 'KATIE'
                        AND R.RefNumber in 
                        ( select R.RefNumber
                        FROM " + initialCatalog + @".dbo.tblRepair R
                        LEFT JOIN " + initialCatalog + @".dbo.tblRepairParts P
                        ON R.RefNumber = P.RefNumber
                        LEFT JOIN " + initialCatalog + @".dbo.tblModel M
                        ON R.ModelNumber = M.ModelNumber
                        group by R.RefNumber
                        Having COUNT(*)=1)
                        UNION
                        SELECT R.RefNumber,DATEDIFF(day, R.DateIn, convert(date, GETDATE())) as Aging,
                        R.ESN,R.MSN,R.SVP,R.Type,R.TechID,R.MftrMSN,R.FuturetelMSN,R.FutureTelOTC,R.ManufacturerOTC,
                        R.Status,R.WorkCode1,R.WorkCode2,R.WorkCode3,R.Complaint1,R.Complaint2,R.Complaint3,
                        R.Problem1,R.Problem2,R.Problem3,R.Manufacturer,R.SpecialCaseNum,R.Warranty,R.Mot_Tran,
                        Format(R.DateIn, N'yyyy-MM-dd HH:mm:ss') as DateIn,Format(R.PurchaseDate, N'yyyy-MM-dd HH:mm:ss') as PurchaseDate,
			            Format(R.DateApproved, N'yyyy-MM-dd HH:mm:ss') as DateApproved,Format(R.DateReject, N'yyyy-MM-dd HH:mm:ss') as DateReject,
			            Format(R.DateEstimation, N'yyyy-MM-dd HH:mm:ss') as DateEstimation,Format(R.DateComplete, N'yyyy-MM-dd HH:mm:ss') as DateComplete,
                        R.DOAQualify,R.FuturetelLocation,R.ModelNumber,P.FaultCode,P.PartNumber,P.Qty,M.ProductCode,
                        N.repairResolutionCode1,N.repairResolutionCode2,N.repairResolutionCode3
                        FROM " + initialCatalog + @".dbo.tblRepair R
                        LEFT JOIN " + initialCatalog + @".dbo.tblRepairParts P
                        ON R.RefNumber = P.RefNumber
                        LEFT JOIN " + initialCatalog + @".dbo.tblModel M
                        ON R.ModelNumber = M.ModelNumber
                        LEFT JOIN " + initialCatalog + @".dbo.tblTelusRepairNew N
                        ON R.RefNumber = N.RefNumber
                        where R.Manufacturer = '" + manufacturer + @"' 
                        And R.DateDockOut is null
                        And R.Status='C'
                        And (R.SVP !='TCC' and R.SVP !='KCC' and R.SVP !='TEXPRESS' and R.SVP !='KEXPRESS')
                        And (R.SVP !='TCC' and R.SVP !='TCHURN' and R.SVP !='KCHURN' and R.SVP !='KMT' and R.SVP !='RMT' 
                        and R.SVP !='TMT' and R.SVP !='PMT' and R.SVP !='FMT' and R.SVP !='WMT' and R.SVP !='PCMT' 
                        and R.SVP !='BMT' and R.SVP !='VMT' and R.SVP !='SMT' and R.SVP !='AMT' and R.SVP !='MMT' and R.SVP !='BSOLOMT')
                        And R.DateComplete >= (Format(GetDate(), N'yyyy-MM-dd')+' 07:00:00')
                        And R.FuturetelLocation != 'STOCK'
                        order by R.RefNumber";
                

                //current location
                string currentLocation = AppDomain.CurrentDomain.BaseDirectory;
                string currentDate = DateTime.Now.ToString("yyyyMMdd-HHmm");
                //file name
                FileName = @"" + filePath + manufacturer + " WIP " + currentDate + ".xlsx";

                return instance;
            }
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }

        public string GetQueryAll()
        {
            return QueryAll;
        }
        
        public string GetFileName()
        {
            return FileName;
        }
    }
}
