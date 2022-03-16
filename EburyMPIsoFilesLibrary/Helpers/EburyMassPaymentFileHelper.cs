using EburyApiWrapper.Beneficiaries;
using EburyApiWrapper.Payments;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System.Collections.Generic;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class EburyMassPaymentFileHelper
    {

        public static NewBenePaymentModel GetBenePaymentFromFile(this MassPaymentFileModel paymentIn)
        {
            NewBenePaymentModel outBene = new NewBenePaymentModel(
                name: paymentIn.BeneficiaryName,
                emailAddresses: new List<string>(),
                emailNotification: false,
                addressLine1: paymentIn.BeneficiaryAddress1,
                postCode: ".",
                countryCode: paymentIn.BeneficiaryCountry,

                bankName: paymentIn.BankName,
                bankAddressLine1: paymentIn.BankAddress1,
                bankCountryCode: paymentIn.BankCountry,
                bankIdentifier: paymentIn.BankCode,
                accountNumber: paymentIn.AccountNo,
                bankCurrencyCode: paymentIn.PaymentCurrency,
                iban: paymentIn.IBAN,
                swiftCode: paymentIn.SwiftCode,

                correspondentAccount: "", //Should this be the creditor's bank our our own account
                correspondentSwiftCode: "",//Creditor or Our Swift??
                reasonForTrade: "",//Nothing until otherwise advised
                                   //purposeOfPayment:  ToDo, lookup the best estimate based on Ref and stuff. creditTransfer.Purp.Item,
                referenceInformation: paymentIn.PaymentReference,
                beneficiaryReference: paymentIn.BeneficiaryReference
            );
            if (string.IsNullOrEmpty(outBene.ReferenceInformation))
                outBene.ReferenceInformation = outBene.BeneficiaryReference;
            outBene.Payment = new NewPaymentPayments(
                NewBenePaymentModel.NoBene,
                paymentIn.PaymentAmount, NewBenePaymentModel.NoBene, false, paymentIn.ValueDate, outBene.ReferenceInformation);

            outBene.FundingCcy = paymentIn.SettlementCurrency;

            //outBene.PurposeOfPayment = creditTransfer.ReasonForPayment();

            return outBene;

        }
    }
}
