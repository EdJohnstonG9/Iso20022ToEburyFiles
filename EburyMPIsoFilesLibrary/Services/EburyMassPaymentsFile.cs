﻿using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyApiWrapper.Beneficiaries;
using EburyMPIsoFilesLibrary.Helpers;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;

namespace EburyMPIsoFilesLibrary.Services
{
    /// <summary>
    /// Object to read, write and hold Ebury Mass Payment File data, exposing conversions to other formats
    /// </summary>
    public class EburyMassPaymentsFile
    {
        public List<MassPaymentFileModel> Payments { get; set; } = new List<MassPaymentFileModel>();

        public EburyMassPaymentsFile()
        {

        }

        public List<NewBenePaymentModel> GetNewBenePaymentList(List<MassPaymentFileModel> payments = null)
        {
            List<NewBenePaymentModel> output = new List<NewBenePaymentModel>();
            if (payments == null) payments = Payments;
            foreach (var payment in payments)
            {
                output.Add(payment.GetBenePaymentFromFile());
            }
            return output;
        }

        public List<NewBenePaymentModel> GetBenePaymentList()
        {
            var outList = new List<NewBenePaymentModel>();
            try
            {
                foreach (var item in Payments)
                {
                    outList.Add(item.GetBenePaymentFromFile());
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"{nameof(GetBenePaymentList)}\tCould not get BenePayment data from Ebury file", ex);
            }
            return outList;
        }

        public int CompleteBenePaymentList(ApplyFinancialsService service)
        {
            int output = 0;
            foreach (var payment in Payments)
            {
                ConvertResponse response = null;
                if (!string.IsNullOrEmpty(payment.BankCountry)
                    && (!string.IsNullOrEmpty(payment.BankCode))
                    && (!string.IsNullOrEmpty(payment.AccountNo)))
                {
                    response = service.Convert(payment.BankCountry, payment.BankCode, payment.AccountNo);
                }
                else if (!string.IsNullOrEmpty(payment.IBAN))
                {
                    response = service.Validate(payment.SwiftCode, payment.IBAN);
                }
                else
                {
                    payment.BeneficiaryReference = $"***MissingData*** {payment.BeneficiaryReference}";
                    System.Diagnostics.Debug.Print($"Missing Data:\t{payment.BeneficiaryName}\t{payment.SwiftCode}\tTo:\t{response?.recommendedBIC}\t{payment.IBAN}\tTo:\t{response?.recommendedAcct}");
                }

                if (response != null)
                {
                    if (payment.SwiftCode != response.recommendedBIC
                        || payment.IBAN != response.recommendedAcct)
                    {
                        System.Diagnostics.Debug.Print($"Account Details updated:\t{payment.BeneficiaryName}\t{payment.SwiftCode}\tTo:\t{response.recommendedBIC}\t{payment.IBAN}\tTo:\t{response.recommendedAcct}");
                    }
                    payment.SwiftCode = response.recommendedBIC;
                    if (!string.IsNullOrEmpty(response.recommendedAcct))
                        payment.IBAN = response.recommendedAcct;
                    output += 1;
                }
            }
            return output;
        }

        //public async Task<List<BicFromIbanModel>> CompleteBicFromIban(List<MassPaymentFileModel> payments = null)
        //{
        //    if (payments == null) payments = Payments;
        //    List<BicFromIbanModel> output = new List<BicFromIbanModel>();
        //    foreach (var benePayment in payments)
        //    {
        //        if (string.IsNullOrEmpty(benePayment.SwiftCode))
        //        {
        //            BicFromIbanModel bic = null;
        //            try
        //            {
        //                bic = await BicFromIban.ValidateIbanAsync(benePayment.IBAN);
        //                if (bic != null && bic?.valid == true && bic?.checkResults.bankCode == true)
        //                {
        //                    output.Add(bic);
        //                    benePayment.SwiftCode = bic.bankData.bic;
        //                    benePayment.BankName = bic.bankData.name;
        //                    benePayment.BankCode = bic.bankData.bankCode;
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ApplicationException($"{nameof(CompleteBicFromIban)}\tFault getting bank details\n{ex.Message}", ex);
        //            }
        //        }
        //    }
        //    return output;
        //}

        public bool WriteMassPaymentsFile(string FileName)
        {
            bool success = false;
            try
            {
                var fi = new FileInfo(FileName);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                if (fi.Exists) fi.Delete();
                var csvConfig = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture);
                csvConfig.PrepareHeaderForMatch = (header, index) => Regex.Replace(header, @"\s", string.Empty);
                csvConfig.MissingFieldFound = null;
                csvConfig.HeaderValidated = null;
                csvConfig.IgnoreBlankLines = true;
                csvConfig.ShouldSkipRecord = (records) => records[0].ToString() == "" && records[1].ToString() == "";
                csvConfig.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "dd/MM/yyyy" };

                using (var writer = new StreamWriter(FileName))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(Payments);
                }
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public int ReadPaymentsFile(string FileName)
        {
            List<MassPaymentFileModel> output = new List<MassPaymentFileModel>();
            try
            {
                var fi = new FileInfo(FileName);
                if (!fi.Exists)
                {
                    throw new FileNotFoundException($"Could not find file: {FileName}");
                }
                var csvConfig = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture);
                csvConfig.PrepareHeaderForMatch = (header, index) => Regex.Replace(header, @"\s", string.Empty);
                csvConfig.MissingFieldFound = null;
                csvConfig.HeaderValidated = null;
                csvConfig.IgnoreBlankLines = true;
                csvConfig.ShouldSkipRecord = (records) => records[0].ToString() == "" && records[1].ToString() == "";
                using (var reader = new StreamReader(FileName))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var records = csv.GetRecords<MassPaymentFileModel>();
                    output = records.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Could not read Mass Payment file: {FileName}", ex);
            }
            Payments = output;
            return output.Count;
        }
    }
}
