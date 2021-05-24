using EburyApiWrapper.Beneficiaries;
using EburyMPIsoFilesLibrary.Models.Airswift;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class EmpFileFromAirswiftHelper
    {
        public static NewBenePaymentModel GetBeneFromAirswift(this AirswiftPaymentModel airswift)
        {
            return new NewBenePaymentModel();
        }

        public static MassPaymentFileModel GetPaymentFromAirswift(this AirswiftPaymentModel airswift)
        {
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";

            output.ValueDate = airswift.executionDate();
            output.PaymentAmount = airswift.CreditAmt();
            output.PaymentCurrency = airswift.BatHdr.BatCcy;
            output.PaymentReference = airswift.SecPty.SecPaymentRef;
            output.ReasonForPayment = airswift.reasonForPayment();

            output.SwiftCode = airswift.SecPty.SecBeneSwift;
            output.BankName = airswift.SecPty.SecBeneBank;
            //output.IBAN = airswift.Iban();
            output.AccountNo = airswift.SecPty.Account;
            output.BankCountry = airswift.SecPty.SecBankCtry;

            output.BeneficiaryName = airswift.SecPty.BeneName;
            output.BeneficiaryReference = airswift.SecPty.SecPaymentRef;
            output.BeneficiaryAddress1 = airswift.BatHdr.BatBeneAddress;
            //output.BeneficiaryCity = airswift.Cdtr.PstlAdr?.TwnNm;
            output.BeneficiaryCountry = airswift.SecPty.SecBankCtry;

            output.SettlementCurrency = airswift.BatHdr.BatCcy;

            return output;
        }

        #region converter functions

        private static DateTime executionDate(this AirswiftPaymentModel airswift)
        {
            string dueDate = airswift.SecPty.DueDate;
            DateTime output = DateTime.ParseExact(dueDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return output;
        }

        private static decimal CreditAmt(this AirswiftPaymentModel airswift)
        {
            string amt = airswift.SecPty.Amount;
            return decimal.Parse(amt);
        }

        private static string reasonForPayment(this AirswiftPaymentModel airswift)
        {
            string output = "";
            var payref = airswift.SecPty.SecPaymentRef;
            if (payref.ToUpper().Contains("SALA"))
            {
                output = "Salary/Payroll";
            }
            return output;
        }
        #endregion
    }
}
