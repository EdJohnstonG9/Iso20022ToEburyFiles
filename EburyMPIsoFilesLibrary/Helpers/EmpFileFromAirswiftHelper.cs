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
        const string fail = "FAIL";
        const string pass = "PASS";

        public static NewBenePaymentModel GetBeneFromAirswift(this AirswiftPaymentModel airswift)
        {
            return new NewBenePaymentModel();
        }

        public static MassPaymentFileModel GetPaymentFromAirswift(this AirswiftPaymentModel airswift, string settlementCcy, IApplyFinancialsService apply)
        {
            bool valid = true;
            var applyRes = apply.Convert(airswift.SecPty.SecBankCtry, airswift.SecPty.SecBeneSwift, airswift.SecPty.Account);
            if (!applyRes.status.ToUpper().Contains(pass))
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

            output.SwiftCode = airswift.bic(applyRes);
            output.BankName = applyRes.paymentBicDetails?.bankName;
            string account = airswift.account(applyRes);
            if (isIban(account))
            { 
                output.IBAN = account;
                output.AccountNo = accFromGBIban(account);
            }
            else
                output.AccountNo = account;
            output.BankCountry = applyRes.countryCode;
            output.BankCode = airswift.bankCode(applyRes);

            output.BeneficiaryName = airswift.beneficiaryName(applyRes);
            //output.BeneficiaryReference = airswift.SecPty.SecPaymentRef;
            output.BeneficiaryReference = airswift.BatHdr.BatBeneAddress;
            output.BeneficiaryAddress1 = airswift.beneficiaryAddress(valid);
            //output.BeneficiaryCity = airswift.Cdtr.PstlAdr?.TwnNm;
            output.BeneficiaryCountry = airswift.SecPty.SecBankCtry;

            output.SettlementCurrency = settlementCcy;

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

        private static string account(this AirswiftPaymentModel airswift, ConvertResponse applyRes)
        {
            string output;
            if (applyRes.status == fail)
            {
                output = airswift.SecPty.Account;
            }
            else
            {
                if (airswift.SecPty.Account != applyRes.recommendedAcct)
                    airswift.SecPty.BeneName = $"{airswift.SecPty.BeneName}\n{airswift.SecPty.Account} Recommend Change Acct to: {applyRes.recommendedAcct}";
                output = applyRes.recommendedAcct;
            }
            return output;
        }

        private static string accFromGBIban(string account)
        {
            if (account.Substring(0, 2) == "GB" && account.Length == 14+8)
            {
                return account.Substring(14, 8);
            }
            else
            {
                return "";
            }
        }

        private static string bic(this AirswiftPaymentModel airswift, ConvertResponse applyRes)
        {
            string output;
            if (applyRes.status == fail)
            {
                output = airswift.SecPty.SecBeneSwift;
            }
            else
            {
                if (!applyRes.recommendedBIC.Contains(airswift.SecPty.SecBeneSwift))
                    airswift.SecPty.BeneName = $"{airswift.SecPty.BeneName}\n{airswift.SecPty.SecBeneSwift} Recommend Change BIC to: {applyRes.recommendedBIC}";
                output = applyRes.recommendedBIC;
            }
            return output;
        }

        private static DateTime executionDate(this AirswiftPaymentModel airswift)
        {
            string dueDate = airswift.SecPty.DueDate;
            DateTime output = DateTime.ParseExact(dueDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return output;
        }

        private static string bankCode (this AirswiftPaymentModel airswift, ConvertResponse applyRes)
        {
            string output = applyRes.recommendedNatId;
            if (string.IsNullOrEmpty(output))
            {
                if (applyRes.branchDetails?[0].codeDetails.codeName4=="Sort Code")
                {
                    output = applyRes.branchDetails[0].codeDetails.codeValue4;
                }
            }
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

        private static string beneficiaryName(this AirswiftPaymentModel airswift, ConvertResponse applyRes)
        {
            string output = airswift.SecPty.BeneName;
            if (applyRes.status == fail)
                output = $"{output}\nBANK DETAILS!!! {applyRes.comment}";
            return output;
        }

        private static string beneficiaryAddress(this AirswiftPaymentModel airswift, bool valid)
        {
            if (valid)
                return ".";// airswift.BatHdr.BatBeneAddress; this is not valid address data
            else
                return "";
        }
        #endregion
    }
}
