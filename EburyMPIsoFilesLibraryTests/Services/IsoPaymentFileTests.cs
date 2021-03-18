using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class IsoPaymentFileTests
    {
        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
            49)]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.wrong",
        //    49)]
        public IsoPaymentFile ReadPaymentsFileTest(string fileName, int items)
        {
            IsoPaymentFile payments = new IsoPaymentFile();

            int actual = payments.ReadPaymentsFile(fileName);
            Assert.Equal(items, actual);
            return payments;
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
             49)]
        public void PaymentBeneficarisListTest(string fileName, int items)
        {
            IsoPaymentFile payments = new IsoPaymentFile();

            int iRead = payments.ReadPaymentsFile(fileName);

            var actual = payments.NewBeneficarisList(payments.Payments001001);
            var total = actual.Sum(x => (decimal)x.Payment.Amount);
            Assert.True(actual.Count == items);
            Assert.Equal(payments.Payments001001.CstmrCdtTrfInitn.GrpHdr.CtrlSum, total);
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
             49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Visa Dec 2020\VIS046_1962_Bank_file_Payroll_tax__122020.xml",
             1)]
        public void GetPaymentFileListTest(string fileName, int items)
        {
            IsoPaymentFile payments = new IsoPaymentFile();

            int iRead = payments.ReadPaymentsFile(fileName);

            var actual = payments.GetPaymentFileList();
            var total = actual.Sum(x => (decimal)x.PaymentAmount);
            Assert.True(actual.Count == items);
            Assert.Equal(payments.ControlTotal, total);
        }
    }
}