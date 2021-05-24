using Xunit;
using EburyMPIsoFilesLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using EburyMPIsoFilesLibrary.Services;
using System.IO;
using EburyMPIsoFilesLibrary.Models.Ebury;

namespace EburyMPIsoFilesLibrary.Helpers.Tests
{
    public class EmpFileFromAirswiftHelperTests
    {
        string fileRoot = @"G:\Shared drives\MP - High Wycombe - Data\Airswift";
        [Fact()]
        public void GetPaymentFromAirswiftTest()
        {
            var paymentFile = new AirswiftPaymentFile();
            string fileName = Path.Combine(fileRoot, @"AirEnergi\0705_SBM LOCAL_SGD.TXT");
            var readfile = paymentFile.ReadPaymentsFile(fileName);
            Assert.True(readfile);

            var eburyData = new List<MassPaymentFileModel>();
            foreach (var payment in paymentFile.AirswiftPaymentList)
            {
                var actual = payment.GetPaymentFromAirswift();
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