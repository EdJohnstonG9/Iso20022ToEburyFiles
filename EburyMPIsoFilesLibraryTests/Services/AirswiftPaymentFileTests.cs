using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using System.Net;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class AirswiftPaymentFileTests
    {
        string fileRoot = @"\\engsyst-svr05\CommonData\Shared drives\MP - High Wycombe - Data\Airswift\";
        ApplyFinancialsService _apply;

        private ApplyConfiguration applyConfig()
        {
            //todo: get this information from new Setting page, persist privately
            var output = new ApplyConfiguration();
            output.BaseUrl = @"https://apps.applyfinancial.co.uk/validate-api/rest";
            output.Credentials = new NetworkCredential("mpoperations@ebury.com", "MpEb0427!");
            return output;
        }

        [Theory()]
        [InlineData(@"AirEnergi\0705_SBM LOCAL_SGD.TXT")]
        public AirswiftPaymentFile ReadAirswiftFileTest(string file)
        {
            _apply = new ApplyFinancialsService(applyConfig());

            var paymentFile = new AirswiftPaymentFile(_apply);
            var result = paymentFile.ReadPaymentsFile(Path.Combine(fileRoot, file));
            Assert.True(result > 0);
            return paymentFile;
        }

        [Theory()]
        [InlineData(@"AirEnergi\0705_SBM LOCAL_SGD - Copy.TXT")]
        [InlineData(@"AirEnergi\0705_FLOUR.TXT")]
        public void MassPaymentFileListTest(string file)
        {
            var paymentFile = ReadAirswiftFileTest(file);

            var restult = paymentFile.MassPaymentFileList();
            Assert.True(restult.Count > 0);
        }
    }
}