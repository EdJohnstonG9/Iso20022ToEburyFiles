using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using EburyMPIsoFilesLibrary.Models.Bacs;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class EmpFileFromBacsHelper
    {
        const string salaDescr = "NET PAY";
        const string bacsCcy = "GBP";

        public static MassPaymentFileModel GetPaymentFromBacs(this BacsModel bacs,
            DateTime executionDate,
            ConvertResponse apply)
        {
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";
            
            output.ValueDate = executionDate;
            output.PaymentAmount = bacs.Amount;
            output.PaymentCurrency = bacsCcy;
            output.PaymentReference = bacs.Description;
            output.ReasonForPayment = bacs.reasonForPayment();

            output.AccountNo = bacs.AccountNo;
            output.BankCode = bacs.sortCode();
            output.SwiftCode = apply?.recommendedBIC;
            output.BankName = apply?.paymentBicDetails?.bankName;
            output.BankAddress1 = apply?.paymentBicDetails?.branch;
            output.BankCity = apply?.paymentBicDetails?.city;
            output.BankCountry = apply.bankCountry();
            output.IBAN = apply?.recommendedAcct;

            output.BeneficiaryName = decodedString(bacs.EmployeeName);
            output.BeneficiaryReference = "";// bacs.Description;
            output.BeneficiaryAddress1 = ".";
            //output.BeneficiaryCity = airswift.Cdtr.PstlAdr?.TwnNm;
            output.BeneficiaryCountry = output.BankCountry;

            output.SettlementCurrency = bacsCcy;

            return output;
        }
        private static string reasonForPayment(this BacsModel bacs)
        {
            string output = "";
            var payref = bacs.Description;
            if (payref.ToUpper().Contains(salaDescr))
            {
                output = "Salary/Payroll";
            }

            return output;
        }
        private static string sortCode(this BacsModel bacs)
        {
            return bacs.SortCode.Replace("-", "");
        }
        private static string bankCountry (this ConvertResponse apply)
        {
            string country = apply?.paymentBicDetails?.country;
            return country == "UNITED KINGDOM" ? "GB" : "XX";
        }
        private static string decodedString(string source)
        {
            var srcBytes = Encoding.GetEncoding(1252).GetBytes(source);
            var outBytes = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.ASCII, srcBytes);
            return Encoding.ASCII.GetString(outBytes).Replace("?"," ");
        }
    }
}
