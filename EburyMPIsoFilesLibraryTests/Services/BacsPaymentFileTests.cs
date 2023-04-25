using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System.Diagnostics;
using EburyMPIsoFilesLibraryTests;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class BacsPaymentFileTests :ApplyFixture
    {
        string testFullFile;
        string filePath = @"G:\Shared drives\MP - High Wycombe - Data\BDOBacs";
        string fileName = "B2C2 BACSLIST.xls";

        public BacsPaymentFileTests()
        {
            testFullFile = Path.Combine(filePath, fileName);
        }

        [Fact()]
        public void ReadPaymentsFileTest()
        {
            BacsPaymentFile service = new BacsPaymentFile(null);

            service.XlPassword = "B2C2";
            if (new FileInfo(testFullFile).Exists)
            {
                service.ReadPaymentsFile(testFullFile);
                Assert.True(service.BacsPayments.Count > 0);

                Assert.True(service.BacsPayments.Count == service.TotalCount);
                var totAmt = service.BacsPayments.Sum(x => x.Amount);
                Assert.True(service.TotalAmount == totAmt);
            }
            else
            {
                Console.WriteLine($"{nameof(ReadPaymentsFileTest)}\tTest not run for file: {testFullFile}");
            }
        }

        [Fact()]
        public List<MassPaymentFileModel> MassPaymentFileListTest()
        {
            ApplyFinancialsService apply = new ApplyFinancialsService(_applyConfig);
            apply.Authenticate();
            BacsPaymentFile service = new BacsPaymentFile(apply);
            service.XlPassword = "B2C2";
            service.ReadPaymentsFile(testFullFile);

            var result = service.MassPaymentFileList();
            Assert.True(result.Count == service.TotalCount);

            return result;
        }

        [Fact()]
        public void SaveResultTest()
        {
            var output = MassPaymentFileListTest();
            EburyMassPaymentsFile emp = new EburyMassPaymentsFile();
            emp.Payments = output;
            string outFile = testFullFile.Replace(".xls", ".csv");

            if(new FileInfo(outFile).Exists )
            {
                emp.WriteMassPaymentsFile(outFile);

                Assert.True(new FileInfo(outFile).Exists);
                Debug.Print(outFile);
            }
            else
            {
                Console.WriteLine($"{nameof(SaveResultTest)}\tTest not run for file {outFile}");
            }
        }
    }
}