using EburyMPIsoFilesLibrary.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Xunit;

namespace EburyMPIsoFilesLibraryTests.Services
{
    public  class EMPFilesCompleteBICTest
    {

        NetworkCredential _credential = new NetworkCredential("mpoperations@ebury.com", "MpEb0427!");
        
        public EburyMassPaymentsFile ReadPaymentsFileTest(string fileName)
        {
            var eburyFile = new EburyMassPaymentsFile();

            var result = eburyFile.ReadPaymentsFile(fileName);

            return eburyFile;
        }

        [Theory]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\MCIE May Live.xlsx - MCIE.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\MCIGroup May Live.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\MLDN May Live csv.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\MOUK - May Live.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Geant.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Tullow Domestic Live File .csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Tullow Expat Live File .csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Tullow Inpat Live File .csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Orix Mod Live File for SWIFT.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Orix UK Live File for SWIFT.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Orix US Live File for SWIFT.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\File for SWIFT Data.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Jadestone Jun Sal live file for payment.csv")]
        //[InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Copy of SWIFT Data.csv")]
        [InlineData(@"G:\Shared drives\MP - High Wycombe - Data\VialtoCompleteBIC\Rubrik Data 220809.csv")]
        public void ReadFileAndCompleteApplyAsync(string fileName)
        {
            var eburyFile = ReadPaymentsFileTest(fileName);

            ApplyFinancialsService service = new ApplyFinancialsService();

            var auth = service.Authenticate(_credential);

            Assert.True(auth == HttpStatusCode.OK);
            Assert.True(service.Token != "");

            var result = eburyFile.CompleteBenePaymentList(service);

            Assert.Equal(eburyFile.Payments.Count, result);
            var fi = new FileInfo(fileName);
            string outDir = fi.DirectoryName;
            string outFile = fi.Name;
            string outFileName = Path.Combine(outDir, "Upd-" + outFile);
            eburyFile.WriteMassPaymentsFile(outFileName);
        }

    }
}
