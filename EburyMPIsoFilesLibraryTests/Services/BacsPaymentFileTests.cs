using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class BacsPaymentFileTests
    {
        [Fact()]
        public void ReadPaymentsFileTest()
        {
            BacsPaymentFile service = new BacsPaymentFile();
            string filePath = @"G:\Shared drives\MP - High Wycombe - Data\BDOBacs";
            string fileName = "B2C2 BACSLIST.xls";
            service.XlPassword = "B2C2";
            service.ReadPaymentsFile(Path.Combine(filePath,fileName));
            Assert.True(service.BacsPayments.Count > 0);

            Assert.True(service.BacsPayments.Count == service.TotalCount);
            var totAmt = service.BacsPayments.Sum(x => x.Amount);
            Assert.True(service.TotalAmount == totAmt);
        }
    }
}