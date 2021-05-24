using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class AirswiftPaymentFileTests
    {
        string fileRoot = @"G:\Shared drives\MP - High Wycombe - Data\Airswift";
        [Fact()]
        public void ReadAirswiftFileTest()
        {
            var paymentFile = new AirswiftPaymentFile();
            var result = paymentFile.ReadPaymentsFile(Path.Combine(fileRoot, @"AirEnergi\0705_SBM LOCAL_SGD.TXT"));
            Assert.True(result);
        }
    }
}