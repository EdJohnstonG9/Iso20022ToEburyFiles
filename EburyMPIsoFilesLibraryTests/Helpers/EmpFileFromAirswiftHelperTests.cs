using Xunit;
using EburyMPIsoFilesLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using EburyMPIsoFilesLibrary.Services;
using System.IO;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using System.Net;

namespace EburyMPIsoFilesLibrary.Helpers.Tests
{
    public class EmpFileFromAirswiftHelperTests
    {

        ApplyFinancialsService _apply;
        public EmpFileFromAirswiftHelperTests()
        {
            _apply = new ApplyFinancialsService(applyConfig());
        }


        private ApplyConfiguration applyConfig()
        {
            //todo: get this information from new Setting page, persist privately
            var output = new ApplyConfiguration();
            output.BaseUrl = @"https://apps.applyfinancial.co.uk/validate-api/rest";
            output.Credentials = new NetworkCredential("mpoperations@ebury.com", "MpEb0427!");
            return output;
        }

        string fileRoot = @"G:\Shared drives\MP - High Wycombe - Data\Airswift";

        [Fact()]
        public void GetPaymentFromAirswiftTest()
        {
            var paymentFile = new AirswiftPaymentFile(_apply);
            string fileName = Path.Combine(fileRoot, @"AirEnergi\0705_SBM LOCAL_SGD.TXT");
            var readfile = paymentFile.ReadPaymentsFile(fileName);
            Assert.True(readfile > 0);
            string settlementCcy = "USD";
            var eburyData = new List<MassPaymentFileModel>();
            foreach (var payment in paymentFile.InputPaymentList)
            {
                var actual = payment.GetPaymentFromAirswift(settlementCcy, _apply);
                Assert.True(actual != null);
                eburyData.Add(actual);
            }

            var eburyFile = new EburyMassPaymentsFile();
            eburyFile.Payments = eburyData;

            string outFileName = fileName.Replace(".TXT", "test.csv");
            var saveActual = eburyFile.WriteMassPaymentsFile(outFileName);

        }
    }
}