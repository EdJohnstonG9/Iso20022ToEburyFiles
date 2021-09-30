using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class ApplyFinancialsServiceTests
    {
        NetworkCredential credential = new NetworkCredential("mpoperations@ebury.com", "MpEb0427!");
        [Fact()]
        public void AuthenticateTest()
        {
            ApplyFinancialsService service = new ApplyFinancialsService();

            var result = service.Authenticate(credential.UserName, credential.SecurePassword);

            Assert.True(result == HttpStatusCode.OK);
            Assert.True(service.Token != "");

            result = service.Authenticate("badUser", credential.SecurePassword);
            Assert.True(string.IsNullOrEmpty(service.Token));
        }

        [Theory]
        [InlineData("09-01-28", "87393708")]
        [InlineData("51-50-01", "64830063")]
        [InlineData("77-30-12", "4462560")]
        [InlineData("20-71-75", "13600572")]
        [InlineData("20-25-19", "90820784")]
        [InlineData("04-00-04", "18599107")]
        [InlineData("20-22-67", "80179310")]
        [InlineData("20-72-91", "53276449")]
        [InlineData("20-89-16", "93666743")]
        [InlineData("09-01-35", "8471583")]
        public void ConvertTest(string sort, string acno)
        {
            ApplyFinancialsService service = new ApplyFinancialsService();

            var result = service.Authenticate(credential.UserName, credential.SecurePassword);

            Assert.True(service.Token != "");

            var convert = service.Convert("GB", sort, acno);
            Assert.NotNull(convert);
            Assert.True(convert.recommendedBIC.Length > 0);
            Debug.Print($"Sort: {sort}\tAccount: {acno}\tBIC: {convert.recommendedBIC}\tIBAN: {convert.recommendedAcct}");
        }

        [Theory]
        [InlineData("CH", "CH8505881078055840002", "AHHBCH22XXX")]
        [InlineData("CH", "CH3981298000007119534", "RAIFCH22C98")]
        [InlineData("CH", "CH280025925911356640U", "CRESCHZZ80A")]
        [InlineData("PL", "PL53114020040000300269320685", "BREXPLPWMBK")]
        [InlineData("GB", "GB19HBUK40131051883291", "HBUKGB4102U")]
        [InlineData("GB", "GB27LOYD30802727682660", "LOYDGB2172")]
        public void ValidateTest(string ctry, string iban, string bic)
        {
            ApplyFinancialsService service = new ApplyFinancialsService();

            var result = service.Authenticate(credential.UserName, credential.SecurePassword);

            Assert.True(service.Token != "");

            var convert = service.Validate(bic, iban);
            Assert.NotNull(convert);
            Assert.True(convert.comment.Length > 0);
            Debug.Print($"Input:\t{bic}\tAccount:\t{iban}\nRec:\t{convert.recommendedBIC}\tIBAN:\t{convert.recommendedAcct}\n{convert.comment}");
        }

    }
}