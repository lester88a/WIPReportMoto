using System;
using System.ComponentModel.DataAnnotations;

namespace WIPReportMot.Models
{
    public class RepairModel
    {
        public int RefNumber { get; set; }
        public int Aging { get; set; }
        public string ESN { get; set; }
        public string MSN { get; set; }
        public string SVP { get; set; }
        public string Type { get; set; }
        public string TechID { get; set; }
        public string MftrMSN { get; set; }
        public string FuturetelMSN { get; set; }
        public string FutureTelOTC { get; set; }
        public string ManufacturerOTC { get; set; }
        public string Status { get; set; }
        public string WorkCode1 { get; set; }
        public string WorkCode2 { get; set; }
        public string WorkCode3 { get; set; }
        public string Complaint1 { get; set; }
        public string Complaint2 { get; set; }
        public string Complaint3 { get; set; }
        public string Problem1 { get; set; }
        public string Problem2 { get; set; }
        public string Problem3 { get; set; }
        public string Manufacturer { get; set; }
        public string SpecialCaseNum { get; set; }
        public bool Warranty { get; set; }
        public string Mot_Tran { get; set; }
        public string DateIn { get; set; }
        public string PurchaseDate { get; set; }
        public string DateApproved { get; set; }
        public string DateReject { get; set; }
        public string DateEstimation { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:00}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:00}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateComplete { get; set; }
        public string DOAQualify { get; set; }
        public string FuturetelLocation { get; set; }
        public string ModelNumber { get; set; }
        public string FaultCode { get; set; }
        public string PartNumber { get; set; }
        public short? Qty { get; set; }
        public string ProductCode { get; set; }
        public string repairResolutionCode1 { get; set; }
        public string repairResolutionCode2 { get; set; }
        public string repairResolutionCode3 { get; set; }
    }
}
