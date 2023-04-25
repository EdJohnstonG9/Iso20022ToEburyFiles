using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using System.Net;
using EburyMPIsoFilesLibraryTests;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class AirswiftPaymentFileTests : ApplyFixture
    {
        string fileRoot = @"\\engsyst-svr05\CommonData\Shared drives\MP - High Wycombe - Data\Airswift\";
        ApplyFinancialsService _apply;

        private ApplyConfiguration applyConfig()
        {
            return _applyConfig;
        }

        [Theory()]
        [InlineData(@"AirEnergi\0705_SBM LOCAL_SGD.TXT")]
        public AirswiftPaymentFile ReadAirswiftFileTest(string file)
        {
            _apply = new ApplyFinancialsService(applyConfig());

            var paymentFile = new AirswiftPaymentFile(_apply);
            var fi = new FileInfo(Path.Combine(fileRoot, file));
            if (fi.Exists)
            {
                var result = paymentFile.ReadPaymentsFile(fi.FullName);
                Assert.True(result > 0);
            }
            else
            {
                Console.WriteLine($"{nameof(ReadAirswiftFileTest)}\tTest not run for {fi.FullName}");
            }
            return paymentFile;
        }

        [Theory()]
        [InlineData(@"AirEnergi\0705_SBM LOCAL_SGD - Copy.TXT")]
        [InlineData(@"AirEnergi\0705_FLOUR.TXT")]
        public void MassPaymentFileListTest(string file)
        {
            var paymentFile = ReadAirswiftFileTest(file);

            if(paymentFile == null)
            {
                Console.WriteLine($"{nameof(MassPaymentFileListTest)}\tTest not run for {file}");
            }
            var restult = paymentFile.MassPaymentFileList();
            Assert.True(restult.Count > 0);
        }
    }
}