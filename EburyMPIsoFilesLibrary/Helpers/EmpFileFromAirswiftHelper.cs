using EburyApiWrapper.Beneficiaries;
using EburyMPIsoFilesLibrary.Models.Airswift;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class EmpFileFromAirswiftHelper
    {
        public static NewBenePaymentModel GetBeneFromAirswift(this AirswiftPaymentModel airswift)
        {
            return new NewBenePaymentModel();
        }

        public static MassPaymentFileModel GetPaymentFromAirswift(this AirswiftPaymentModel airswift, IApplyFinancialsService apply)
        {
            bool valid = true;
            var applyRes = apply.Convert(airswift.SecPty.SecBankCtry, airswift.SecPty.SecBeneSwift, airswift.SecPty.Account);
            if (!applyRes.status.ToUpper().Contains("PASS"))
            {
                valid = false;
                System.Diagnostics.Debug.Print(applyRes.status);
            }
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";

            output.ValueDate = airswift.executionDate();
            output.PaymentAmount = airswift.CreditAmt();
            output.PaymentCurrency = airswift.BatHdr.BatCcy;
            output.PaymentReference = airswift.SecPty.SecPaymentRef;
            output.ReasonForPayment = airswift.reasonForPayment();

            output.SwiftCode = applyRes.recommendedBIC;
            output.BankName = applyRes.paymentBicDetails?.bankName;
            if (isIban(applyRes.recommendedAcct))
                output.IBAN = applyRes.recommendedAcct;
            else
                output.AccountNo = applyRes.recommendedAcct;
            output.BankCountry = applyRes.countryCode;
            output.BankCode = applyRes.recommendedNatId;

            output.BeneficiaryName = airswift.beneficiaryName(valid);
            output.BeneficiaryReference = airswift.SecPty.SecPaymentRef;
            output.BeneficiaryAddress1 = airswift.beneficiaryAddress(valid);
            //output.BeneficiaryCity = airswift.Cdtr.PstlAdr?.TwnNm;
            output.BeneficiaryCountry = airswift.SecPty.SecBankCtry;

            output.SettlementCurrency = airswift.BatHdr.BatCcy;

            return output;
        }

        #region converter functions

        private static bool isIban(string account)
        {
            if (string.IsNullOrEmpty(account))
                return false;
            else
            {
                var match = Regex.Match(account, @"^([A-Z]{2}[ \-]?[0-9]{2})(?=(?:[ \-]?[A-Z0-9]){9,30}$)((?:[ \-]?[A-Z0-9]{3,5}){2,7})([ \-]?[A-Z0-9]{1,3})?$");
                return match.Success;
            }
        }

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

        private static string beneficiaryName(this AirswiftPaymentModel airswift, bool valid)
        {
            string output = airswift.SecPty.BeneName;
            if (!valid)
                output += " REVIEW BANK DETAILS!!!";
            return output;
        }

        private static string beneficiaryAddress(this AirswiftPaymentModel airswift, bool valid)
        {
            if (valid)
                return airswift.BatHdr.BatBeneAddress;
            else
                return "";
        }
        #endregion
    }
}
