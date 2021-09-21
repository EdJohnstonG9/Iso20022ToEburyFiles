using EburyApiWrapper.Beneficiaries;
using EburyApiWrapper.Client;
using EburyApiWrapper.Payments;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_001_03;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class PaymentsInitiationV03Helper 
    {

        public static void CompleteSwift(this NewBenePaymentModel benePayment)
        {
            //if (string.IsNullOrEmpty(benePayment.SwiftCode) && !string.IsNullOrEmpty(benePayment.Iban))
            //{
            //    BicFromIbanModel bic = BicFromIban.ValidateIban(benePayment.Iban);
            //    if (bic != null && bic?.valid == true)
            //    {
            //        benePayment.SwiftCode = bic.bankData.bic;
            //        benePayment.BankName = bic.bankData.name;
            //        benePayment.BankIdentifier = bic.bankData.bankCode;
            //    }
            //}
        }
        public static void CompleteSwift(this MassPaymentFileModel payFile)
        {
            //if (string.IsNullOrEmpty(payFile.SwiftCode) && !string.IsNullOrEmpty(payFile.IBAN))
            //{
            //    BicFromIbanModel bic = BicFromIban.ValidateIban(payFile.IBAN);
            //    if (bic != null && bic?.valid == true)
            //    {
            //        payFile.SwiftCode = bic.bankData.bic;
            //        payFile.BankName = bic.bankData.name;
            //        payFile.BankCode = bic.bankData.bankCode;
            //    }
            //}
        }

        public static NewBenePaymentModel GetBeneFromIso(this CreditTransferTransactionInformation10 creditTransfer, PaymentInstructionInformation3 paymentInstruction)//, PaymentInstructionInformation3 paymentInstruction, GroupHeader32 groupHeader)
        {
            NewBenePaymentModel outBene = new NewBenePaymentModel(
                name: creditTransfer.Cdtr.Nm,
                emailAddresses: creditTransfer.CreditorEmailAddress(),
                emailNotification: false,
                addressLine1: creditTransfer.BeneficiaryAddress1(),
                postCode: creditTransfer.Cdtr.PstlAdr?.PstCd,
                countryCode: creditTransfer.BeneficiaryCountry(),
                bankIdentifier: creditTransfer.BankIdentifier(), 
                accountNumber: creditTransfer.AccountNo(),
                bankAddressLine1: creditTransfer.CdtrAgt?.FinInstnId.PstlAdr?.AdrLine?[0],
                bankCountryCode: creditTransfer.BankCountry(),
                bankCurrencyCode: creditTransfer.CreditCcy(),
                bankName: creditTransfer.AccountName(), //Name of payee bank ac?
                correspondentAccount: creditTransfer.CdtrAgtAcct?.Nm, //Should this be the creditor's bank our our own account
                correspondentSwiftCode: creditTransfer.CdtrAgtAcct?.Id?.Item.ToString(),//Creditor or Our Swift??
                iban: creditTransfer.Iban(),//Repeat of bankIdentifier?
                reasonForTrade: "",//Nothing until otherwise advised
                                   //purposeOfPayment:  ToDo, lookup the best estimate based on Ref and stuff. creditTransfer.Purp.Item,
                referenceInformation: creditTransfer.PaymentReference(),
                beneficiaryReference: "",
                swiftCode: creditTransfer.Bic()
            );
            outBene.Payment = new NewPaymentPayments(
                "NoAccount",
                (float)creditTransfer.CreditAmt(), "NoBene", false, paymentInstruction.ExecutionDate(), creditTransfer.PaymentReference());

            outBene.FundingCcy = paymentInstruction.SettlementCurrency(creditTransfer.CreditCcy());
            if (string.IsNullOrEmpty(outBene.PostCode))
                outBene.PostCode = ".";
            //outBene.PurposeOfPayment = creditTransfer.ReasonForPayment();

            //outBene.CompleteSwift();

            return outBene;
        }

        public static MassPaymentFileModel GetPaymentFromISO(this CreditTransferTransactionInformation10 creditTransfer, PaymentInstructionInformation3 paymentInstruction)
        {
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";

            output.ValueDate = paymentInstruction.ExecutionDate();
            output.PaymentAmount = creditTransfer.CreditAmt();
            output.PaymentCurrency = creditTransfer.CreditCcy();
            output.PaymentReference = creditTransfer.PaymentReference();
            //output.ReasonForPayment = reasonForPayment(creditTransfer);

            output.SwiftCode = creditTransfer.CdtrAgt?.FinInstnId.BIC;
            output.BankName = creditTransfer.CdtrAgt?.FinInstnId.Nm;
            output.IBAN = creditTransfer.Iban();
            output.AccountNo = (string)creditTransfer.CdtrAcct.Tp?.Item;
            output.BankCountry = creditTransfer.BankCountry();

            output.BeneficiaryName = creditTransfer.Cdtr.Nm;
            output.BeneficiaryAddress1 = creditTransfer.BeneficiaryAddress1();
            output.BeneficiaryCity = creditTransfer.Cdtr.PstlAdr?.TwnNm;
            output.BeneficiaryCountry = creditTransfer.BeneficiaryCountry();

            output.SettlementCurrency = paymentInstruction.SettlementCurrency(output.PaymentCurrency);

            //ToDo Remitter Info
            //ToDo CNY Payment Purpose Code
            //ToDo RUR Central Banking Codes

            //output.CompleteSwift();

            return output;
        }

        #region simple functions

        public static decimal CreditAmt(this CreditTransferTransactionInformation10 creditTransfer)
        {
            return ((ActiveOrHistoricCurrencyAndAmount)creditTransfer.Amt.Item).Value;
        }

        public static string CreditCcy(this CreditTransferTransactionInformation10 creditTransfer)
        {
            return ((ActiveOrHistoricCurrencyAndAmount)creditTransfer.Amt.Item).Ccy;
        }

        public static string SettlementCurrency(this PaymentInstructionInformation3 paymentInstruction, string PaymentCcy)
        {
            var output = paymentInstruction.DbtrAcct.Ccy;//or same as payment ccy
            if (string.IsNullOrEmpty(output))
            {
                output = PaymentCcy;
            }
            return output;
        }

        public static string PaymentReference(this CreditTransferTransactionInformation10 creditTransfer)
        {
            //string output = $"{creditTransfer.PurposeOfPayment()} {creditTransfer.RmtInf.Ustrd[0]}".Trim();
            string output = $"{creditTransfer.RmtInf?.Ustrd[0]}".Trim();
            //remove multispace
            output = Regex.Replace(output, @"\s+", " ", RegexOptions.Multiline).Trim();
            if (string.IsNullOrEmpty(output))
                throw new ApplicationException($"Blank Reference not allowed. Payment to {creditTransfer.Cdtr.Nm} {creditTransfer.Bic()} {creditTransfer.CreditAmt()}");
            return output;
        }
        public static string AccountNo(this CreditTransferTransactionInformation10 creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Tp?.Item;
        }
        public static string BankIdentifier(this CreditTransferTransactionInformation10 creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Id?.Item.ToString();
        }

        public static DateTime ExecutionDate(this PaymentInstructionInformation3 paymentInstruction)
        {
            return paymentInstruction.ReqdExctnDt;
        }

        public static string PurposeOfPayment(this CreditTransferTransactionInformation10 creditTransfer)
        {
            //ToDo this should return an enum ankAccountCoreData.PurposeOfPaymentEnum
            return creditTransfer.Purp?.Item;
        }

        public static string ReasonForTrade(this CreditTransferTransactionInformation10 creditTransfer)
        {
            string output = "";
            string purp = creditTransfer.Purp?.Item;
            if(purp.ToUpper().Contains("SALA"))
            {
                output = "Salary/Payroll";
            }
            return output;
        }

        public static string BeneficiaryCountry(this CreditTransferTransactionInformation10 creditTransfer)
        {
            string output = creditTransfer.Cdtr.CtryOfRes;
            if (string.IsNullOrEmpty(output))
            {
                output = BankCountry(creditTransfer);
            }
            return output;
        }

        public static string BeneficiaryAddress1(this CreditTransferTransactionInformation10 creditTransfer)
        {
            string output = creditTransfer.Cdtr.PstlAdr?.AdrLine?[0];
            if (string.IsNullOrEmpty(output))
            {
                output = "-";
            }
            return output;
        }

        public static List<string> CreditorEmailAddress(this CreditTransferTransactionInformation10 creditTransfer)
        {
             List<string> outEmail  = new List<string>();
             outEmail.Add(creditTransfer.Cdtr.CtctDtls?.EmailAdr);
            if (string.IsNullOrEmpty(outEmail[0]))
                outEmail[0] = "No.Email@Nowhere.com";
            return outEmail;
        }

        public static string BankCountry(this CreditTransferTransactionInformation10 creditTransfer)
        {
            var output = creditTransfer.CdtrAgt?.FinInstnId.PstlAdr?.Ctry;
            if (string.IsNullOrEmpty(output))
            {
                output = Iban(creditTransfer)?.Substring(0, 2);
            }
            return output;
        }

        public static string Iban(this CreditTransferTransactionInformation10 creditTransfer)
        {
            var output = (string)creditTransfer.CdtrAcct.Id?.Item;
            return output;
        }

        public static string AccountName(this CreditTransferTransactionInformation10 creditTransfer)
        {
            var output = (string)creditTransfer.CdtrAcct.Nm;
            return output;
        }

        public static string Bic(this CreditTransferTransactionInformation10 creditTransfer)
        {
            return creditTransfer.CdtrAgt?.FinInstnId?.BIC;
        }
        #endregion
    }
}
