using CsvHelper.Configuration.Attributes;
using EburyMPIsoFilesLibrary.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Models.Ebury
{
    public class MassPaymentFileModel
    {
        private string beneficiaryName;
        private string beneficiaryAddress1;
        private string beneficiaryCity;
        private string bankName;
        private string bankAddress1;
        private string bankCity;
        private string remitterName;
        private string remitterAddressLine1;
        private string remitterAddressLine2;
        private string beneficiaryReference;
        private string paymentReference;

        public string Direction { get; set; }
        public string Product { get; set; }
        public string BeneficiaryName { get => beneficiaryName.ToAsciiChars(); set => beneficiaryName = value; }
        public string BeneficiaryAddress1 { get => beneficiaryAddress1.ToAsciiChars(); set => beneficiaryAddress1 = value; }
        public string BeneficiaryCity { get => beneficiaryCity.ToAsciiChars(); set => beneficiaryCity = value; }
        public string BeneficiaryCountry { get; set; }
        public string PaymentCurrency { get; set; }
        public decimal PaymentAmount { get; set; }
        public string SettlementCurrency { get; set; }
        public string BankName { get => bankName.ToAsciiChars(); set => bankName = value; }
        public string BankAddress1 { get => bankAddress1.ToAsciiChars(); set => bankAddress1 = value; }
        public string BankCity { get => bankCity.ToAsciiChars(); set => bankCity = value; }
        public string BankCountry { get; set; }
        public string AccountNo { get; set; }
        public string IBAN { get; set; }
        public string SwiftCode { get; set; }
        public string PaymentReference { get => paymentReference.ToAsciiChars(); set => paymentReference = value; }
        public string BankCode { get; set; }
        public DateTime ValueDate { get; set; }
        public string RemitterName { get => remitterName.ToAsciiChars(); set => remitterName = value; }
        public string RemitterAddressLine1 { get => remitterAddressLine1.ToAsciiChars(); set => remitterAddressLine1 = value; }
        public string RemitterAddressLine2 { get => remitterAddressLine2.ToAsciiChars(); set => remitterAddressLine2 = value; }
        public string RemitterAccountNumber { get; set; }
        public string RemitterCountry { get; set; }
        [Name("Reason for trade")]
        public string ReasonForPayment { get; set; }
        public string BeneficiaryReference { get => beneficiaryReference.ToAsciiChars(); set => beneficiaryReference = value; }
        public string PurposeCode { get; set; }
        public string INN { get; set; }
        public string VO { get; set; }
        public string KIO { get; set; }
        public string RussianCentralBankAccount { get; set; }
    }

    public enum DirectionCode
    {
        BUY,
        SELL
    }


}
