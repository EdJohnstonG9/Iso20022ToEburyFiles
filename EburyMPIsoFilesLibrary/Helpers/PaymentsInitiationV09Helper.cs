using EburyApiWrapper.Beneficiaries;
using EburyApiWrapper.Payments;

using EburyMassFilesWPF.Core.Models.ISO20022.pain_001_001_09;

using EburyMPIsoFilesLibrary.Models.Ebury;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class PaymentsInitiationV09Helper
    {

        public static NewBenePaymentModel GetBeneFromIso(this CreditTransferTransaction34 creditTransfer, PaymentInstruction30 paymentInstruction)//, PaymentInstruction30 paymentInstruction, GroupHeader32 groupHeader)
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
                creditTransfer.CreditAmt(), "NoBene", false, paymentInstruction.ExecutionDate(), creditTransfer.PaymentReference());

            outBene.FundingCcy = paymentInstruction.SettlementCurrency(creditTransfer.CreditCcy());
            if (string.IsNullOrEmpty(outBene.PostCode))
                outBene.PostCode = ".";
            //outBene.PurposeOfPayment = creditTransfer.ReasonForPayment();

            //outBene.CompleteSwift();

            return outBene;
        }

        public static MassPaymentFileModel GetPaymentFromISO(this CreditTransferTransaction34 creditTransfer, PaymentInstruction30 paymentInstruction)
        {
            MassPaymentFileModel output = new MassPaymentFileModel();
            output.Direction = "BUY";
            output.Product = "SPOT";

            output.ValueDate = paymentInstruction.ExecutionDate();
            output.PaymentAmount = creditTransfer.CreditAmt();
            output.PaymentCurrency = creditTransfer.CreditCcy();
            output.PaymentReference = creditTransfer.PaymentReference();
            //infOut.ReasonForPayment = reasonForPayment(creditTransfer);

            output.SwiftCode = creditTransfer.CdtrAgt?.FinInstnId.BICFI;
            output.BankName = creditTransfer.CdtrAgt?.FinInstnId.Nm;
            output.IBAN = creditTransfer.Iban();
            output.AccountNo = (string)creditTransfer.CdtrAcct.Tp?.Item;
            output.BankCountry = creditTransfer.BankCountry();

            output.BeneficiaryName = creditTransfer.Cdtr.Nm;
            //output.BeneficiaryAddress1 = creditTransfer.BeneficiaryAddress1();
            //output.BeneficiaryStreetName = creditTransfer.BeneficiaryAddress1
            //output.BeneficiaryCity = creditTransfer.Cdtr.PstlAdr?.TwnNm;
            output.MapOutputAddresses(creditTransfer);
            output.BeneficiaryCountry = creditTransfer.BeneficiaryCountry();

            //BOS does not accept ISO purpose codes
            //output.PurposeCode = creditTransfer.GetPurpopseCode(paymentInstruction);

            output.SettlementCurrency = paymentInstruction.SettlementCurrency(output.PaymentCurrency);

            //ToDo Remitter Info
            //ToDo CNY Payment Purpose Code
            //ToDo RUR Central Banking Codes

            //infOut.CompleteSwift();

            return output;
        }

        #region simple functions

        private static string GetPurpopseCode(this CreditTransferTransaction34 creditTransfer, PaymentInstruction30 paymentInstruction)
        {
            var output = creditTransfer.Purp?.Item
                ?? paymentInstruction.PmtTpInf.CtgyPurp?.Item;

            return output;
        }

        private static void MapOutputAddresses(this MassPaymentFileModel output, CreditTransferTransaction34 creditTransfer)
        {
            var postalAddr = creditTransfer.Cdtr.PstlAdr;
            if (postalAddr == null) return;

            output.BeneficiaryStreetName = postalAddr.StrtNm;
            output.BeneficiaryBuildingNumber = postalAddr.BldgNb;
            //output.BeneficiaryFloor = postalAddr.Flr;
            //output.BeneficiaryApartmentOffice = postalAddr.Room;
            output.BeneficiaryPostCode = postalAddr.PstCd;
            output.BeneficiaryCountrySubDivision = postalAddr.CtrySubDvsn;
            output.BeneficiaryCity = postalAddr.TwnNm;
            output.BeneficiaryCountry = postalAddr.Ctry;

            if (postalAddr.AdrLine?.Any() == true)
            {
                Debug.Print($"AddrLines:\n{string.Join(Environment.NewLine, postalAddr.AdrLine)}");
            }
        }

        public static decimal CreditAmt(this CreditTransferTransaction34 creditTransfer)
        {
            return ((ActiveOrHistoricCurrencyAndAmount)creditTransfer.Amt.Item).Value;
        }

        public static string CreditCcy(this CreditTransferTransaction34 creditTransfer)
        {
            return ((ActiveOrHistoricCurrencyAndAmount)creditTransfer.Amt.Item).Ccy;
        }

        public static string SettlementCurrency(this PaymentInstruction30 paymentInstruction, string PaymentCcy)
        {
            var output = paymentInstruction.DbtrAcct.Ccy;//or same as payment ccy
            if (string.IsNullOrEmpty(output))
            {
                output = PaymentCcy;
            }
            return output;
        }

        public static string PaymentReference(this CreditTransferTransaction34 creditTransfer)
        {
            string output = "";
            try
            {
                if (creditTransfer.RmtInf.Ustrd?.Length > 0)
                {
                    foreach (var item in creditTransfer.RmtInf.Ustrd)
                    {
                        output += item.Trim();
                        output += Environment.NewLine;
                    }
                    output.Trim();
                    //output = creditTransfer.PmtId.EndToEndId + Environment.NewLine + output;
                }
                else if (creditTransfer.RmtInf.Strd?.Length > 0)
                {//Covering the options for reference information
                    foreach (var structured in creditTransfer.RmtInf.Strd)
                    {
                        output += addtlRmtInf(structured.AddtlRmtInf);
                        output += cdtrRef(structured.CdtrRefInf);
                        output += structured.Invcee == null ? "" : $"Invcee: {structured.Invcee.Nm} ";
                        output += structured.Invcr == null ? "" : $"Invcr: {structured.Invcr.Nm} ";
                        //infOut += structured.RfrdDocAmt == null ? "" : $"RfrdDocAmt: {structured.RfrdDocAmt}";
                        output += structured.RfrdDocInf == null ? "" : $"RfrdDocInf: {structured.RfrdDocInf[0].Nb} ";
                        output += Environment.NewLine;
                    }
                }

                //remove multispace
                output = Regex.Replace(output, @"\s+", " ", RegexOptions.Multiline).Trim();
                return output;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"{nameof(PaymentReference)}\tCould not find Payment Reference in Remittance Information for Bene: {creditTransfer.Cdtr.Nm}", ex);
            }

            // Multi-line Remitter Info
            string addtlRmtInf(string[] inf)
            {
                if (inf == null)
                    return string.Empty;
                string infOut = "";
                foreach (string item in inf)
                {
                    infOut += item + Environment.NewLine;
                }
                return infOut.Trim();
            }
            //Creditor Ref Info handling
            string cdtrRef(CreditorReferenceInformation2 refInf)
            {
                if (refInf == null)
                    return string.Empty;

                string itemVal = refInf.Tp?.CdOrPrtry?.Item.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(itemVal))
                    itemVal += ": ";

                //return itemVal + refInf?.Ref;
                return refInf?.Ref;
            }
        }
        public static string AccountNo(this CreditTransferTransaction34 creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Tp?.Item;
        }
        public static string BankIdentifier(this CreditTransferTransaction34 creditTransfer)
        {
            return (string)creditTransfer.CdtrAcct.Id?.Item.ToString();
        }

        public static DateTime ExecutionDate(this PaymentInstruction30 paymentInstruction)
        {
            return paymentInstruction.ReqdExctnDt.Item;
        }

        public static string PurposeOfPayment(this CreditTransferTransaction34 creditTransfer)
        {
            //ToDo this should return an enum ankAccountCoreData.PurposeOfPaymentEnum
            return creditTransfer.Purp?.Item;
        }

        public static string ReasonForTrade(this CreditTransferTransaction34 creditTransfer)
        {
            string output = "";
            string purp = creditTransfer.Purp?.Item;
            if (purp.ToUpper().Contains("SALA"))
            {
                output = "Salary/Payroll";
            }
            return output;
        }

        public static string BeneficiaryCountry(this CreditTransferTransaction34 creditTransfer)
        {
            string output = creditTransfer.Cdtr.CtryOfRes;
            if (string.IsNullOrEmpty(output))
            {
                output = BankCountry(creditTransfer);
            }
            return output;
        }

        public static string BeneficiaryAddress1(this CreditTransferTransaction34 creditTransfer)
        {
            string output = creditTransfer.Cdtr.PstlAdr?.AdrLine?[0];
            if (string.IsNullOrEmpty(output))
            {
                output = "-";
            }
            return output;
        }

        public static List<string> CreditorEmailAddress(this CreditTransferTransaction34 creditTransfer)
        {
            List<string> outEmail = new List<string>();
            outEmail.Add(creditTransfer.Cdtr.CtctDtls?.EmailAdr);
            if (string.IsNullOrEmpty(outEmail[0]))
                outEmail[0] = "No.Email@Nowhere.com";
            return outEmail;
        }

        public static string BankCountry(this CreditTransferTransaction34 creditTransfer)
        {
            var output = creditTransfer.CdtrAgt?.FinInstnId.PstlAdr?.Ctry;
            if (string.IsNullOrEmpty(output))
            {
                output = Iban(creditTransfer)?.Substring(0, 2);
            }
            return output;
        }

        public static string Iban(this CreditTransferTransaction34 creditTransfer)
        {
            string output;
            if (creditTransfer.CdtrAcct.Id?.Item is GenericAccountIdentification1 generic)
                output = generic.Id;
            else
                output = (string)creditTransfer.CdtrAcct.Id?.Item;
            return output;
        }

        public static string AccountName(this CreditTransferTransaction34 creditTransfer)
        {
            var output = (string)creditTransfer.CdtrAcct.Nm;
            return output;
        }

        public static string Bic(this CreditTransferTransaction34 creditTransfer)
        {
            return creditTransfer.CdtrAgt?.FinInstnId?.BICFI;
        }
        #endregion
    }
}
