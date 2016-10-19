using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIPReportMot.DataManager;

namespace WIPReportMot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //begin monitoring for execution time
                var watch = System.Diagnostics.Stopwatch.StartNew();

                string fileName = null;

                bool hasError = false;
                string errorMessage = null;
                string emailMessage = "*** This is an automatically generated email, please do not reply ***\n\n";

                #region create Excel file and save it
                ExcelManager ex = new ExcelManager();
                ex.GetExcelSheet(ref fileName, ref errorMessage);


                #endregion

                #region rename the file before send the email
                string filePath = Configuration.Config.Instance.GetFileSavedPath();
                string subject = "Claims_File_Data_Feed-Futuretel-CA.xlsx";
                string newFileName = filePath + subject;
                System.IO.File.Copy(fileName, newFileName); //Copy("oldfilename", "newfilename")
                Console.WriteLine("{0} has been copy and renamed in {1}", fileName, newFileName);
                #endregion

                #region Email Service
                string recipient = "";

                emailMessage += errorMessage;
                Console.WriteLine(emailMessage);
                EmailService.Email es = new EmailService.Email();

                if (String.IsNullOrEmpty(errorMessage))
                {
                    hasError = false;
                }
                else
                {
                    hasError = true;
                }

                if (hasError)
                {
                    Console.WriteLine("Has Errors!!!");
                    recipient = Configuration.Config.Instance.GetRecipientAdmin();
                    es.SendEmailMethod(newFileName, recipient, subject.Remove(subject.Length - 5), emailMessage);
                }
                else
                {
                    Console.WriteLine("No Errors.");
                    recipient = Configuration.Config.Instance.GetRecipient();
                    es.SendEmailMethod(newFileName, recipient, subject.Remove(subject.Length - 5), "");
                }

                Console.WriteLine("Email Send Success");
                #endregion

                #region after send email, delete the new file
                Console.WriteLine("Deleting the new file: {0}", newFileName);

                Thread.Sleep(3000);
                System.IO.File.Delete(newFileName);
                Console.WriteLine("The new file has been deleted!");
                #endregion

                //stop monitoring execution time
                #region Calculate Execution Time
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                //display execution time
                Console.WriteLine("Execution time: " + elapsedMs / 1000 + " seconds");
                #endregion
            }
            catch (Exception er)
            {
                Console.WriteLine("Error:\n" + er);
            }


            Console.WriteLine("Application will exit within 5 seconds.");
            //delay 5 seconds for user to read detailed info
            Thread.Sleep(5000);
        }
    }
}
