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
            output.PaymentReference = "";// bacs.Description;
            output.ReasonForPayment = bacs.reasonForPayment();

            output.AccountNo = bacs.AccountNo;
            output.BankCode = bacs.SortCode;
            output.SwiftCode = apply?.recommendedBIC;
            output.BankName = apply?.branchDetails?[0].bankName;
            output.BankAddress1 = apply?.branchDetails?[0].branch;
            output.BankCity = apply?.branchDetails?[0].city;
            output.BankCountry = apply?.branchDetails?[0].country;
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
        private static string decodedString(string source)
        {
            var srcBytes = Encoding.GetEncoding(1252).GetBytes(source);
            var outBytes = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.ASCII, srcBytes);
            return Encoding.ASCII.GetString(outBytes).Replace("?"," ");
        }
    }
}
