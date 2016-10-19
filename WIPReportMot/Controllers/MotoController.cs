using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SQL = System.Data;
using System.Threading.Tasks;
using WIPReportMot.Models;
using WIPReportMot.Configuration;
using FastMember;
using System.Globalization;

namespace WIPReportMot.Controllers
{
    public class MotoController
    {
        //instance variables
        private ObservableCollection<MotorolaModel> MotoDataModel;
        private SQL.DataTable MotoDataTable;
        //variables for error handling
        private string ErrorMessage = null;
        private string productCodeIn = "Error";
        private string repairStatusCode = "Error";

        //Get DataIn DataTable
        public SQL.DataTable GetCatDataTable(ObservableCollection<RepairModel> AllRepairData, ref string errorMessage)
        {
            //initialize objects
            MotoDataModel = new ObservableCollection<MotorolaModel>();
            MotoDataTable = new SQL.DataTable();

            //get all data
            var allData = AllRepairData;

            //date time: today's date at 7:00 AM
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);

            //LINQ query to get catData data from all data
            var motoData = from i in allData.AsEnumerable()
                          orderby i.Aging descending, i.RefNumber ascending
                          select i;

            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:00") + " +1:00";

            //assign data to connection
            foreach (var item in motoData)
            {
                //get Action Reason Code
                #region
                string actionReasonCode = "R";
                if ((new[] { "R0054", "R0055", "R0056", "R0057", "R0058", "R0060" }.Any(c => item.WorkCode1.Contains(c))) && item.Status == "C")
                {
                    actionReasonCode = "F";
                }
                if ((new[] { "R0054", "R0055", "R0056", "R0057", "R0058", "R0060" }.Any(c => item.WorkCode2.Contains(c))) && item.Status == "C")
                {
                    actionReasonCode = "F";
                }
                if ((new[] { "R0054", "R0055", "R0056", "R0057", "R0058", "R0060" }.Any(c => item.WorkCode3.Contains(c))) && item.Status == "C")
                {
                    actionReasonCode = "F";
                }
                #endregion

                //get Material Number
                #region
                string materialNumber = "";
                if (item.Status=="C")
                {
                    if (item.PartNumber == "===")
                    {
                        materialNumber = "NULL";
                    }
                    else if (String.IsNullOrEmpty(item.PartNumber))
                    {
                        materialNumber = "NULL";
                    }
                    else
                    {
                        materialNumber = item.PartNumber;
                    }
                }
                else
                {
                    materialNumber = "NULL";
                }
                #endregion

                //get Payer
                #region
                string payer = "ERROR";
                if (item.Status=="C")
                {
                    if (!String.IsNullOrEmpty(item.SpecialCaseNum))
                    {
                        payer = "5";
                    }
                    else if (item.Warranty == true)
                    {
                        payer = "10";
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(item.Mot_Tran))
                        {
                            payer = "30";
                        }
                        else if (!String.IsNullOrEmpty(item.repairResolutionCode1) &&(new[] { "BERSWAP", "TPWREPAIR", "TPWREPLACE" }.Any(c => item.repairResolutionCode1.Contains(c))))
                        {
                            payer = "20";
                        }
                        else if (!String.IsNullOrEmpty(item.repairResolutionCode2) && (new[] { "BERSWAP", "TPWREPAIR", "TPWREPLACE" }.Any(c => item.repairResolutionCode2.Contains(c))))
                        {
                            payer = "20";
                        }
                        else if (!String.IsNullOrEmpty(item.repairResolutionCode3) && (new[] { "BERSWAP", "TPWREPAIR", "TPWREPLACE" }.Any(c => item.repairResolutionCode3.Contains(c))))
                        {
                            payer = "20";
                        }
                        else
                        {
                            payer = "30";
                        }
                    }
                }
                else
                {
                    payer = "NULL";
                }

                
                #endregion

                //get Product Code In
                #region
                
                if (item.Type =="U" || item.Type == "T")
                {
                    productCodeIn = item.ProductCode;
                }
                if (item.Type =="A")
                {
                    productCodeIn = "ACC";
                }
                if (item.Type=="W" && String.IsNullOrEmpty(item.ProductCode) && item.ModelNumber=="360")
                {
                    productCodeIn = item.MSN.Substring(0,3);
                }
                #endregion

                //get Product Code Out
                #region
                string productCodeOut = "";
                if (item.Status=="C")
                {
                    if (new[] { "U", "T", "W" }.Any(c => item.Type.Contains(c)) && (!String.IsNullOrEmpty(item.FuturetelMSN) ))
                    {
                        productCodeOut = item.FuturetelMSN.Substring(0, 3);
                    }
                    else if (new[] { "U", "T", "W" }.Any(c => item.Type.Contains(c)) && (!String.IsNullOrEmpty(item.MftrMSN)))
                    {
                        productCodeOut = item.MftrMSN.Substring(0, 3);
                    }
                    else
                    {
                        productCodeOut = "NULL";
                    }
                }
                else
                {
                    productCodeOut = "NULL";
                }
                #endregion

                //get Quotation End Date
                #region
                string quotationEndDate;
                if (!String.IsNullOrEmpty(item.DateApproved) && String.IsNullOrEmpty(item.DateReject)) { quotationEndDate = item.DateApproved + " +1:00"; }
                else if (String.IsNullOrEmpty(item.DateApproved) && !String.IsNullOrEmpty(item.DateReject)) { quotationEndDate = item.DateReject + " +1:00"; }
                else { quotationEndDate = "NULL"; }
                #endregion

                //get RepairStatusCode
                #region
                
                if (item.Status == "C")
                {
                    repairStatusCode = "AWD";
                }
                else
                {
                    if (item.Status == "I")
                    {
                        repairStatusCode = "AWR";
                    }
                    else if (item.Status == "B" || item.Status == "R")
                    {
                        repairStatusCode = "INR";
                    }
                    else if (new[] { "E", "A", "J" }.Any(c => item.Status.Contains(c)))
                    {
                        repairStatusCode = "NWR";
                    }
                    else
                    {
                        repairStatusCode = "ERROR";
                    }
                }
                #endregion

                //get SerialNumberOut
                #region
                string serialNumberOut = "";
                if (item.Status=="C")
                {
                    if (!String.IsNullOrEmpty(item.FuturetelMSN))
                    {
                        serialNumberOut = item.FuturetelMSN;
                    }
                    else if (!String.IsNullOrEmpty(item.MftrMSN))
                    {
                        serialNumberOut = item.MftrMSN;
                    }
                    else
                    {
                        serialNumberOut = item.MSN;
                        if (String.IsNullOrEmpty(item.MSN))
                        {
                            serialNumberOut = "Error";
                        }
                    }
                }
                else
                {
                    serialNumberOut = "NULL";
                }
                #endregion
                
                //get shop id
                #region
                string shopId = "";
                if (!String.IsNullOrEmpty(item.DOAQualify))
                {
                    shopId = "CA9392";
                }
                else
                {
                    shopId = "CA0001";
                }
                #endregion

                //get TransactionCode
                #region
                string transactionCode = "";
                if (item.Status=="C")
                {
                    if (!String.IsNullOrEmpty(item.Mot_Tran)&&!String.IsNullOrWhiteSpace(item.Mot_Tran))
                    {
                        transactionCode = item.Mot_Tran;
                    }
                    else
                    {
                        transactionCode = "OOW";
                    }
                }
                else
                {
                    if (!new[] { "kmt", "rmt", "tmt", "pmt", "fmt", "wmt", "pcmt", "bmt", "vmt", "smt", "amt", "mmt", "bsolomt" }.Any(c => item.SVP.Contains(c)))
                    {
                       transactionCode = "RFP";
                    }
                    else if (item.Warranty == true && !String.IsNullOrEmpty(item.DOAQualify))
                    {
                        transactionCode = "OOB";
                    }
                    else if (item.Warranty == true)
                    {
                        transactionCode = "RE";
                    }
                    else
                    {
                        transactionCode = "OOW";
                    }
                    
                }

                #endregion

                //get WarrantyFlag
                #region
                string warrantyFlag = "NULL";
                if (item.Status=="C")
                {
                    warrantyFlag = "F" + item.RefNumber;
                }
                else
                {
                    warrantyFlag = "NULL";
                }
                #endregion


                //MotoDataModel.Add
                #region
                MotoDataModel.Add(new MotorolaModel
                {
                    AccidentDate = "NULL",
                    ActionCode = item.Status == "C" && item.DateComplete >= today ? item.WorkCode1 + ":" + item.WorkCode2 + ":" + item.WorkCode3 : "NULL",
                    ActionReasonCode = item.Status == "C" ? actionReasonCode : "NULL",
                    AWBIn = "NULL",
                    AWBOut = "NULL",
                    ClaimID = item.RefNumber,
                    CourierIn = "NULL",
                    CourierOut = "NULL",
                    CustomerComplaintCoddPrimary = item.Complaint1 + ":" + item.Complaint2 + ":" + item.Complaint3,
                    DateIn = item.DateIn + " +1:00",
                    DateOut = "NULL",
                    DeliveryDate = "NULL",
                    DeliveryNumber = "NULL",
                    FaultCode = item.Status == "C" ? item.FaultCode : "NULL",
                    FieldBulletinNumber = "NULL",
                    IMEI2In = "NULL",
                    IMEI2Out = "NULL",
                    IMEINumberIn = !String.IsNullOrEmpty(item.ESN) ? item.ESN : "NULL",
                    IMEINumberOut = item.Status == "C" ? (!String.IsNullOrEmpty(item.FutureTelOTC) || !String.IsNullOrEmpty(item.ManufacturerOTC) ? item.FutureTelOTC + item.ManufacturerOTC : "NULL") : "NULL",
                    InboundShipmentType = "NULL",
                    ItemCodeOut = "NULL",
                    JobCreationDate = item.DateIn + " +1:00",
                    Mandatory = item.Status == "C" ? "1" : "NULL",
                    ManufactureDate = item.Type == "A" ? item.PurchaseDate + " +1:00" : "NULL",
                    MaterialNumber = item.Status == "C" ? materialNumber : "NULL",
                    OEM = "MOTOROLA",
                    OutboundShipmentType = "NULL",
                    Payer = item.Status == "C" ? payer : "NULL",
                    PickupArrangedDate = "NULL",
                    PickupDate = "NULL",
                    POPDate = !String.IsNullOrEmpty(item.PurchaseDate) ? item.PurchaseDate + " +1:00" : "NULL",
                    POPSupplier = "NULL",
                    ProblemFoundCode = item.Status == "C" ? item.Problem1 + ":" + item.Problem2 + ":" + item.Problem3 : "NULL",
                    ProductCodeIn = !String.IsNullOrEmpty(productCodeIn) ? productCodeIn : "Error",
                    ProductCodeOut = productCodeOut,
                    ProductType = "NULL",
                    ProductVersionIn = "NULL",
                    ProductVersionOut = "NULL",
                    Project = "NULL",
                    ProviderCarrier = "NULL",
                    QuantityReplaced = item.Status == "C" ? (item.Qty != 0 && !String.IsNullOrEmpty(item.Qty.ToString())? item.Qty.ToString() : "NULL") : "NULL",
                    QuotationEndDate = quotationEndDate,
                    QuotationStartDate = !String.IsNullOrEmpty(item.DateEstimation) ? item.DateEstimation + " +1:00" : "NULL",
                    ReferenceDesignatorNumber = "NULL",
                    RepairServicePartnerID = !String.IsNullOrEmpty(item.DOAQualify) ? "CA9392" : "CA0001",
                    RepairStatusCode = repairStatusCode,
                    RepairTimeStamp = item.Status == "C" ? item.DateComplete.Value.ToString("yyyy-MM-dd HH:mm:00", CultureInfo.InvariantCulture) + " +1:00" : "NULL",
                    ReportDate = currentTime,
                    ReturnDate = "NULL",
                    SecondStatus = "NULL",
                    SerialNumberIn = !String.IsNullOrEmpty(item.MSN) ? item.MSN : "NULL",
                    SerialNumberOut = serialNumberOut,
                    ShopID = shopId,
                    SoftwareIn = "NULL",
                    SoftwareOut = "NULL",
                    SpecialProjectNumber = "NULL",
                    TechnicianID = item.Status == "C" ? (!String.IsNullOrEmpty(item.TechID) ? item.TechID : "NULL") : "NULL",
                    TransactionCode = transactionCode,
                    WarrantyFlag = warrantyFlag
                });
                #endregion

               
                
            }

            #region error handling
            //LINQ query to get motoData with distinct data from all data
            var result = allData.GroupBy(test => test.RefNumber)
               .Select(grp => grp.First())
               .ToList();
            foreach (var noDuplicates in result)
            {
                
                //Error handle after generate report
                #region

                //convert string to int
                long n;
                bool isNumeric = true;
                if (!String.IsNullOrEmpty(noDuplicates.ESN))
                {
                    //long number = Convert.ToInt64(item.ESN);
                    isNumeric = long.TryParse(noDuplicates.ESN, out n);
                }
                if (String.IsNullOrEmpty(noDuplicates.Complaint1))
                {
                    ErrorMessage += "\n\nErrorCode[1]\nComplaint1 cannot be empty, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (!isNumeric)
                {
                    ErrorMessage += "\n\nErrorCode[2]\nESN is not numerical, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (noDuplicates.Type == "A" && String.IsNullOrEmpty(noDuplicates.PurchaseDate))
                {
                    ErrorMessage += "\n\nErrorCode[3]\nType is A and PurchaseDate is empty, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (productCodeIn.Contains("ERROR"))
                {
                    ErrorMessage += "\n\nErrorCode[4]\nType is not equal to U,T,W,A, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (repairStatusCode.Contains("ERROR"))
                {
                    ErrorMessage += "\n\nErrorCode[5]\nStatus is not equal to I,B,R,E,A,J when status is not C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (!String.IsNullOrEmpty(noDuplicates.FutureTelOTC) && !String.IsNullOrEmpty(noDuplicates.ManufacturerOTC))
                {
                    ErrorMessage += "\n\nErrorCode[10]\nBoth ReplacementESN AND MftOTC fields cannot be populate on the same record, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }

                if (!String.IsNullOrEmpty(noDuplicates.FuturetelMSN) && !String.IsNullOrEmpty(noDuplicates.MftrMSN))
                {
                    ErrorMessage += "\n\nErrorCode[11]\nReplacementMSN AND MftMSN fields cannot be populate on the same record, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (noDuplicates.Type=="U"&&String.IsNullOrEmpty(noDuplicates.ProductCode))
                {
                    ErrorMessage += "\n\nErrorCode[14]\nProductCode cannot be empty, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (noDuplicates.Type == "T" && String.IsNullOrEmpty(noDuplicates.ProductCode))
                {
                    ErrorMessage += "\n\nErrorCode[14]\nProductCode cannot be empty, see RefNumber: " + noDuplicates.RefNumber + "\n";
                }
                if (noDuplicates.Status == "C")
                {
                    if (String.IsNullOrEmpty(noDuplicates.WorkCode1))
                    {
                        ErrorMessage += "\n\nErrorCode[6]\nWorkCode cannot be empty when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                    if (noDuplicates.WorkCode1[0] != 'R')
                    {
                        ErrorMessage += "\n\nErrorCode[7]\nWorkCode1 is not equal to starting with 'R' when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                    if (String.IsNullOrEmpty(noDuplicates.FaultCode))
                    {
                        ErrorMessage += "\n\nErrorCode[8]\nFaultCode cannot be empty when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                    if (String.IsNullOrEmpty(noDuplicates.Problem1))
                    {
                        ErrorMessage += "\n\nErrorCode[9]\nProblem1 cannot be empty when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                    if (String.IsNullOrEmpty(noDuplicates.TechID))
                    {
                        ErrorMessage += "\n\nnErrorCode[12]\nTechID should not be empty when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                    if (String.IsNullOrEmpty(noDuplicates.DateComplete.ToString()))
                    {
                        ErrorMessage += "\n\nnErrorCode[13]\nDateComplete should not be empty when status is C, see RefNumber: " + noDuplicates.RefNumber + "\n";
                    }
                }

                #endregion
            }
            //assign error message
            if (String.IsNullOrEmpty(this.ErrorMessage))
            {
                errorMessage = null;
            }
            else
            {
                string errorFileName = Connection.Instance.GetFileName();
                string msg = "\nError has been found on these records!\nPlease check the original file:\n" + errorFileName;
                msg += "\n\nNOTE: Avoid sending the error file to client!";
                errorMessage = msg + ErrorMessage;
            }
            #endregion


            //convert ObservableCollection to DataTable using FastMember reference
            using (var reader = ObjectReader.Create(MotoDataModel))
            {
                MotoDataTable.Load(reader);
            }

            //order the datatable's columns and set column names
            #region order the datatable's columns and set column names
            //CatDataTable.Columns["RefNumber"].SetOrdinal(0);
            MotoDataTable.Columns["AccidentDate"].ColumnName = "Accident Date";
            MotoDataTable.Columns["ActionCode"].ColumnName = "Action Code";
            MotoDataTable.Columns["ActionReasonCode"].ColumnName = "Action Reason Code";
            MotoDataTable.Columns["AWBIn"].ColumnName = "AWB In";
            MotoDataTable.Columns["AWBOut"].ColumnName = "AWB Out";
            MotoDataTable.Columns["ClaimID"].ColumnName = "Claim ID";
            MotoDataTable.Columns["CourierIn"].ColumnName = "Courier In";
            MotoDataTable.Columns["CourierOut"].ColumnName = "Courier Out";
            MotoDataTable.Columns["CustomerComplaintCoddPrimary"].ColumnName = "Customer Complaint Code - Primary";
            MotoDataTable.Columns["DateIn"].ColumnName = "Date In";
            MotoDataTable.Columns["DateOut"].ColumnName = "Date Out";
            MotoDataTable.Columns["DeliveryDate"].ColumnName = "Delivery Date";
            MotoDataTable.Columns["DeliveryNumber"].ColumnName = "Delivery Number";
            MotoDataTable.Columns["FaultCode"].ColumnName = "Fault Code";
            MotoDataTable.Columns["FieldBulletinNumber"].ColumnName = "Field Bulletin Number";
            MotoDataTable.Columns["IMEI2In"].ColumnName = "IMEI 2 In";
            MotoDataTable.Columns["IMEI2Out"].ColumnName = "IMEI 2 Out";
            MotoDataTable.Columns["IMEINumberIn"].ColumnName = "IMEI Number In";
            MotoDataTable.Columns["IMEINumberOut"].ColumnName = "IMEI Number Out";
            MotoDataTable.Columns["InboundShipmentType"].ColumnName = "Inbound Shipment Type";
            MotoDataTable.Columns["ItemCodeOut"].ColumnName = "Item code out";
            MotoDataTable.Columns["JobCreationDate"].ColumnName = "Job Creation Date";
            //CatDataTable.Columns["Mandatory"].ColumnName = "Mandatory";
            MotoDataTable.Columns["ManufactureDate"].ColumnName = "Manufacture date";
            MotoDataTable.Columns["MaterialNumber"].ColumnName = "Material Number";
            //CatDataTable.Columns["OEM"].ColumnName = "OEM";
            MotoDataTable.Columns["OutboundShipmentType"].ColumnName = "Outbound Shipment Type";
            //CatDataTable.Columns["Payer"].ColumnName = "Payer";
            MotoDataTable.Columns["PickupArrangedDate"].ColumnName = "Pickup Arranged Date";
            MotoDataTable.Columns["PickupDate"].ColumnName = "Pickup Date";
            MotoDataTable.Columns["POPDate"].ColumnName = "POP date";
            MotoDataTable.Columns["POPSupplier"].ColumnName = "POP supplier";
            MotoDataTable.Columns["ProblemFoundCode"].ColumnName = "Problem Found Code";
            MotoDataTable.Columns["ProductCodeIn"].ColumnName = "Product Code In";
            MotoDataTable.Columns["ProductCodeOut"].ColumnName = "Product Code Out";
            MotoDataTable.Columns["ProductType"].ColumnName = "Product Type";
            MotoDataTable.Columns["ProductVersionIn"].ColumnName = "Product version in";
            MotoDataTable.Columns["ProductVersionOut"].ColumnName = "Product version out";
            MotoDataTable.Columns["Project"].ColumnName = "Project";
            MotoDataTable.Columns["ProviderCarrier"].ColumnName = @"Provider/Carrier";
            MotoDataTable.Columns["QuantityReplaced"].ColumnName = "Quantity Replaced";
            MotoDataTable.Columns["QuotationEndDate"].ColumnName = "Quotation End Date";
            MotoDataTable.Columns["QuotationStartDate"].ColumnName = "Quotation Start Date";
            MotoDataTable.Columns["ReferenceDesignatorNumber"].ColumnName = "Reference designator number";
            MotoDataTable.Columns["RepairServicePartnerID"].ColumnName = "Repair Service Partner ID";
            MotoDataTable.Columns["RepairStatusCode"].ColumnName = "Repair Status Code";
            MotoDataTable.Columns["RepairTimeStamp"].ColumnName = "Repair Time Stamp";
            MotoDataTable.Columns["ReportDate"].ColumnName = "Report date";
            MotoDataTable.Columns["ReturnDate"].ColumnName = "Return Date";
            MotoDataTable.Columns["SecondStatus"].ColumnName = "Second Status";
            MotoDataTable.Columns["SerialNumberIn"].ColumnName = "Serial Number In";
            MotoDataTable.Columns["SerialNumberOut"].ColumnName = "Serial Number Out";
            MotoDataTable.Columns["ShopID"].ColumnName = "Shop ID";
            MotoDataTable.Columns["SoftwareIn"].ColumnName = "Software In";
            MotoDataTable.Columns["SoftwareOut"].ColumnName = "Software Out";
            MotoDataTable.Columns["SpecialProjectNumber"].ColumnName = "Special project number";
            MotoDataTable.Columns["TechnicianID"].ColumnName = "Technician ID";
            MotoDataTable.Columns["TransactionCode"].ColumnName = "Transaction Code";
            MotoDataTable.Columns["WarrantyFlag"].ColumnName = "Warranty Flag";
            #endregion


            return MotoDataTable;
        }

    }
}
