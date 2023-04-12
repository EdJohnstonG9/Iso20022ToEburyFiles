using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class EburyMassPaymentsFileTests
    {
        NetworkCredential _credential = new NetworkCredential("mpoperations@ebury.com", "MpEb0427!");
        
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
        public EburyMassPaymentsFile ReadPaymentsFileTest(string fileName, int items)
        {
            var eburyFile = new EburyMassPaymentsFile();

            var result = eburyFile.ReadPaymentsFile(fileName);

            Assert.True(items == result || items == -1);

            return eburyFile;
        }

        [Theory]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\BDO Example 1\XML FILE - Example 1 - BDO - GetSwiftTest.csv",
            3)]
        //[InlineData(@"C:\Users\EdJohnston\Downloads\Bene file test.csv",
        //    -1)]
        public void ReadFileAndCompleteApplyAsync(string fileName, int items)  
        {
            var eburyFile = ReadPaymentsFileTest(fileName, items);

            ApplyFinancialsService service = new ApplyFinancialsService();

            var auth = service.Authenticate(_credential);

            Assert.True(auth == HttpStatusCode.OK);
            Assert.True(service.Token != "");

            var result = eburyFile.CompleteBenePaymentList(service);

            Assert.Equal(eburyFile.Payments.Count, result);

            string outFileName = fileName.Replace(".csv", "-Upd.csv");
            eburyFile.WriteMassPaymentsFile(outFileName);

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