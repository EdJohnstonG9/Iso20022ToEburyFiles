using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class EburyMassPaymentsFileTests
    {
        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
            49)]
        public EburyMassPaymentsFile PaymentsFromISOTest(string fileName, int items)
        {
            IsoPaymentFile payments = new IsoPaymentFile();
            int actual = payments.ReadPaymentsFile(fileName);
            Assert.Equal(items, actual);

            var eburyFile = new EburyMassPaymentsFile();
            eburyFile.Payments = payments.GetPaymentFileList();
            actual = eburyFile.Payments.Count;

            Assert.Equal(items, actual);
            return eburyFile;
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
            49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 1 (Adyen)\Copy of SEPA_Finanzamt.xml",
            1)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 2 (DE Kraton Entity)\Copy of ARI038_2795_Bank_file_Other_102020.xml",
            15)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_2859_Zahlungsdatei_Sonstiges_102020 (1).xml",
            483)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_DE_10_20201014_14-42.xml",
            1)]
        public void WriteMassPaymentsFileTest(string fileName, int items)
        {
            var eburyFile = PaymentsFromISOTest(fileName, items);

            string outFileName = fileName.Replace(".xml", "test.csv");
            var actual = eburyFile.WriteMassPaymentsFile(outFileName);

            Assert.True(actual);
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020test.csv",
            49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 1 (Adyen)\Copy of SEPA_Finanzamttest.csv",
            1)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 2 (DE Kraton Entity)\Copy of ARI038_2795_Bank_file_Other_102020test.csv",
            15)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_2859_Zahlungsdatei_Sonstiges_102020 (1)test.csv",
            483)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_DE_10_20201014_14-42test.csv",
            1)]
        public void ReadPaymentsFileTest(string fileName, int items)
        {
            var eburyFile = new EburyMassPaymentsFile();

            var result = eburyFile.ReadPaymentsFile(fileName);

            Assert.Equal(items, result);
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\CSV File Examples\FXFMP031735.csv", 17)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\CSV File Examples\FXFMP031746.csv", 48)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\CSV File Examples\FXFMP031824.csv", 121)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\CSV File Examples\FXFMP032711.csv", 2)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\CSV File Examples\FXFMP032857.csv", 593)]
        public EburyMassPaymentsFile ReadPaymentsFileTest1(string fileName, int items)
        {
            var mpFile = new EburyMassPaymentsFile();

            var result = mpFile.ReadPaymentsFile(fileName);

            Assert.Equal(items, result);
            return mpFile;
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\Copy of 3623_27821 Tokio Marine_ salaries_10_2020.xml",
            49)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 1 (Adyen)\Copy of SEPA_Finanzamt.xml",
            1)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Cloudpay Example 2 (DE Kraton Entity)\Copy of ARI038_2795_Bank_file_Other_102020.xml",
            15)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_2859_Zahlungsdatei_Sonstiges_102020 (1).xml",
            483)]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\Zahlungsdatei\IFS034_DE_10_20201014_14-42.xml",
            1)]
        public async void CompleteBicFromIbanTest(string fileName, int items)
        {
            var mpFile = PaymentsFromISOTest(fileName, items);


            //No test as CompleteBicFromIban removed from the app.

            //var result = await mpFile.CompleteBicFromIban();

            //foreach (var pay in mpFile.Payments)
            //{
            //    Assert.NotEmpty(pay.SwiftCode);
            //}
        }
    }
}