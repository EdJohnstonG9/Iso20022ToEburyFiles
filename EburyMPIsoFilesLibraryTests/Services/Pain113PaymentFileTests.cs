using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace EburyMPIsoFilesLibraryTests.Services
{
    public class Pain113PaymentFileTests
    {
        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
            49)]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.wrong",
        //    49)]
        public Pain113PaymentFile ReadPaymentsFileTest(string fileName, int items)
        {
            Pain113PaymentFile payments = new Pain113PaymentFile();

            if (new FileInfo(fileName).Exists)
            {
                var actual = payments.ReadPaymentsFile(fileName);
                Assert.Equal(items, payments.MassPaymentFileList().Count);
            }
                return payments; 
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
             49)]
        public void PaymentBeneficarisListTest(string fileName, int items)
        {
            Pain113PaymentFile payments = new Pain113PaymentFile();

            if (new FileInfo(fileName).Exists)
            {
                var resuult = payments.ReadPaymentsFile(fileName);

                var actual = payments.NewBeneficarisList(payments.IsoPayments);
                var total = actual.Sum(x => (decimal)x.Payment.Amount);
                Assert.True(actual.Count == items);
                Assert.Equal(payments.IsoPayments.CstmrCdtTrfInitn.GrpHdr.CtrlSum, total); 
            }
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
             49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\AirBnB\payments_employees_AIR041_122020.xml",
             29)]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Visa Dec 2020\VIS046_1962_Bank_file_Payroll_tax__122020.xml",
        //     1)]
        public void GetPaymentFileListTest(string fileName, int items)
        {
            Pain113PaymentFile payments = new Pain113PaymentFile();

            if (new FileInfo(fileName).Exists)
            {
                var result = payments.ReadPaymentsFile(fileName);

                var actual = payments.MassPaymentFileList();
                var total = actual.Sum(x => (decimal)x.PaymentAmount);
                Assert.True(actual.Count == items);
                Assert.Equal(payments.ControlTotal, total); 
            }
        }

        [Theory]
     //   [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
     //49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Visa Dec 2020\VIS046_1962_Bank_file_Payroll_tax__122020.xml",
             1)]
        public void GetPaymentFileList_wrontTypeTest(string fileName, int items)
        {
            Pain113PaymentFile payments = new Pain113PaymentFile();

            if (new FileInfo(fileName).Exists)
            {
                var result = payments.ReadPaymentsFile(fileName);

                var actual = payments.MassPaymentFileList();
                Assert.True(result == 0);
                Assert.Empty(actual); 
            }
        }
    }
}
