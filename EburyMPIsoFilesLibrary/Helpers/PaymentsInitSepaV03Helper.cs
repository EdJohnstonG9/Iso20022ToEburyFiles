using EburyApiWrapper.Beneficiaries;
using EburyApiWrapper.Payments;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_003_03;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class PaymentsSepaV03Helper 
    {
        public static NewBenePaymentModel GetBeneFromIso(this CreditTransferTransactionInformationSCT creditTransfer, PaymentInstructionInformationSCT paymentInstruction)//, PaymentInstructionInformation3 paymentInstruction, GroupHeader32 groupHeader)
        {
            NewBenePaymentModel outBene = new NewBenePaymentModel(
                name: creditTransfer.Cdtr.Nm,
                emailAddresses: creditTransfer.CreditorEmailAddress(),
                emailNotification: false,
                addressLine1: creditTransfer.BeneficiaryAddress1(),
                postCode: "",//creditTransfer.Cdtr.PstlAdr?.PstCd,
                countryCode: creditTransfer.BeneficiaryCountry(),
                bankIdentifier: creditTransfer.BankIdentifier(),
                accountNumber: creditTransfer.AccountNo(),
                bankAddressLine1: "",//creditTransfer.CdtrAgt?.FinInstnId.PstlAdr?.AdrLine?[0],
                bankCountryCode: creditTransfer.BankCountry(),
                bankCurrencyCode: creditTransfer.CreditCcy(),
                bankName: creditTransfer.AccountName(), //Name of payee bank ac?
                correspondentAccount: "",//creditTransfer.CdtrAgtAcct?.Nm, //Should this be the creditor's bank our our own account
                correspondentSwiftCode: creditTransfer.CdtrAgt?.FinInstnId?.BIC,// .Id?.Item.ToString(),//Creditor or Our Swift??
                iban: creditTransfer.Iban(),//Repeat of bankIdentifier?
                reasonForTrade: "",//Nothing until otherwise advised
                                   //purposeOfPayment:  ToDo, lookup the best estimate based on Ref and stuff. creditTransfer.Purp.Item,
                referenceInformation: creditTransfer.PaymentReference(),
                beneficiaryReference: "",
                swiftCode: creditTransfer.Bic()
            ) ;
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

        public static MassPaymentFileModel GetPaymentFromISO(this CreditTransferTransactionInformationSCT creditTransfer, PaymentInstructionInformationSCT paymentInstruction)
        {
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";

            output.ValueDate = paymentInstruction.ExecutionDate();
            output.PaymentAmount = creditTransfer.CreditAmt();
            output.PaymentCurrency = creditTransfer.CreditCcy();
            output.PaymentReference = creditTransfer.PaymentReference();
            output.ReasonForPayment = ReasonForTrade(creditTransfer);

            output.SwiftCode = creditTransfer.CdtrAgt?.FinInstnId.BIC;
            output.BankName = "";//creditTransfer.CdtrAgt?.FinInstnId.Nm;
            output.IBAN = creditTransfer.Iban();
            output.AccountNo = "";// (string)creditTransfer.CdtrAcct.Tp?.Item;
            output.BankCountry = creditTransfer.BankCountry();

            output.BeneficiaryName = creditTransfer.Cdtr.Nm;
            output.BeneficiaryAddress1 = creditTransfer.BeneficiaryAddress1();
            output.BeneficiaryCity = "";// creditTransfer.BeneficiaryCountry();
            output.BeneficiaryCountry = creditTransfer.BeneficiaryCountry();

            output.SettlementCurrency = paymentInstruction.SettlementCurrency(output.PaymentCurrency);

            //ToDo Remitter Info
            //ToDo CNY Payment Purpose Code
            //ToDo RUR Central Banking Codes

            //output.CompleteSwift();

            return output;
        }

        #region simple functions

        public static decimal CreditAmt(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            return creditTransfer.Amt.InstdAmt.Value;
        }

        public static string CreditCcy(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            return creditTransfer.Amt.InstdAmt.Ccy.ToString();
        }

        public static string SettlementCurrency(this PaymentInstructionInformationSCT paymentInstruction, string PaymentCcy)
        {
            var output = paymentInstruction.DbtrAcct.Ccy;//or same as payment ccy
            if (string.IsNullOrEmpty(output))
            {
                output = PaymentCcy;
            }
            return output;
        }

        public static string PaymentReference(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            //string output = $"{creditTransfer.PurposeOfPayment()} {creditTransfer.RmtInf.Item.ToString()}";
            string output = $"{creditTransfer.RmtInf.Item.ToString()}";
            //remove multispace
            output = Regex.Replace(output, @"\s+", " ", RegexOptions.Multiline).Trim();
            return output;
        }
        public static string AccountNo(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Id.ToString();// .Tp?.Item;
        }
        public static string BankIdentifier(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Id.IBAN;// ?.Item.ToString();
        }

        public static DateTime ExecutionDate(this PaymentInstructionInformationSCT paymentInstruction)
        {
            return paymentInstruction.ReqdExctnDt;
        }

        public static string PurposeOfPayment(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            //ToDo this should return an enum ankAccountCoreData.PurposeOfPaymentEnum
            return creditTransfer.Purp?.Cd;// .Item;
        }

        public static string ReasonForTrade(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            string output = "";
            string purp = creditTransfer.Purp?.Cd;//.Item;
            if(purp.ToUpper().Contains("SALA"))
            {
                output = "Salary/Payroll";
            }
            else
            {
                //output = "Other";
                output = "";
            }
            return output;
        }

        public static string BeneficiaryCountry(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            string output = creditTransfer.Cdtr.PstlAdr?.Ctry;// .CtryOfRes;
            if (string.IsNullOrEmpty(output))
            {
                output = BankCountry(creditTransfer);
            }
            return output;
        }

        public static string BeneficiaryAddress1(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            string output = creditTransfer.Cdtr.PstlAdr?.AdrLine?[0];
            if (string.IsNullOrEmpty(output))
            {
                output = "-";
            }
            return output;
        }

        public static List<string> CreditorEmailAddress(this CreditTransferTransactionInformationSCT creditTransfer)
        {
             List<string> outEmail  = new List<string>();
            outEmail.Add("");// creditTransfer.Cdtr.CtctDtls?.EmailAdr);
            if (string.IsNullOrEmpty(outEmail[0]))
                outEmail[0] = "No.Email@Nowhere.com";
            return outEmail;
        }

        public static string BankCountry(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            var output = "";// creditTransfer.CdtrAgt?.FinInstnId.PstlAdr?.Ctry;
            if (string.IsNullOrEmpty(output))
            {
                output = Iban(creditTransfer)?.Substring(0, 2);
            }
            return output;
        }

        public static string Iban(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            var output = (string)creditTransfer.CdtrAcct.Id.IBAN;
            return output;
        }

        public static string AccountName(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            var output = "";// (string)creditTransfer.CdtrAcct.Id;
            return output;
        }

        public static string Bic(this CreditTransferTransactionInformationSCT creditTransfer)
        {
            return creditTransfer.CdtrAgt?.FinInstnId?.BIC;
        }
        #endregion
    }
}
