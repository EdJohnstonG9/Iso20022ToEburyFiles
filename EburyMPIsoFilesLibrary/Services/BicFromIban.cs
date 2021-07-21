using EburyApiWrapper.Beneficiaries;
using EburyApiWrapper.Clients;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EburyMPIsoFilesLibrary.Services
{

    public static class BicFromIban
    {
        const string basePath = @"https://openiban.com";
        const string reqPathIban = @"/validate/{iban}";
        const string reqPathBankId = @"/v2/calculate/{country}/{bankCode}/{accountNumber}";

        private static RestRequest getRequest(string iban)
        {
            var request = new RestRequest(reqPathIban, Method.GET);
            request.AddUrlSegment("iban", iban);
            request.AddQueryParameter("getBIC", "true");
            request.AddQueryParameter("validateBankCode", "true");
            return request;
        }
        private static RestRequest getRequest(string ctry, string sort, string acnt)
        {
            var request = new RestRequest(reqPathBankId, Method.GET);
            request.AddUrlSegment("country", ctry);
            request.AddUrlSegment("bankCode", sort);
            request.AddUrlSegment("accountNumber", acnt);
            return request;
        }

        public static BicFromIbanModel ValidateIban(string iban)
        {
            BicFromIbanModel output = null;
            if (!string.IsNullOrEmpty(iban))
            {
                var client = new RestClient(basePath);
                var response = client.Get(getRequest(iban));
                try
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        output = JsonConvert.DeserializeObject<BicFromIbanModel>(response.Content);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"{nameof(ValidateIban)}\tFault getting bank details\n{ex.Message}", ex);
                }
            }
            return output;
        }

        public async static Task<BicFromIbanModel> ValidateIbanAsync(string Iban)
        {
            BicFromIbanModel bic = null;
            if (!string.IsNullOrEmpty(Iban))
            {
                var client = new RestClient(basePath);
                try
                {
                    var response = await client.ExecuteAsync(getRequest(Iban));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        bic = JsonConvert.DeserializeObject<BicFromIbanModel>(response.Content);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"{nameof(ValidateIban)}\tFault getting bank details\n{ex.Message}", ex);
                }
            }
            return bic;
        }

        public async static Task<BicFromIbanModel> ValidateIbanAsync(NewBenePaymentModel benePayment, IProgress<NewBenePaymentModel> progress, CancellationToken cancel = new CancellationToken())
        {
            BicFromIbanModel bic = null;
            try
            {
                bic = await ValidateIbanAsync(benePayment.Iban);
                if (bic != null && bic?.valid == true)
                {
                    benePayment.SwiftCode = bic.bankData.bic;
                    benePayment.BankName = bic.bankData.name;
                    benePayment.BankIdentifier = bic.bankData.bankCode;
                    if (progress != null)
                        progress.Report(benePayment);
                    if (cancel.IsCancellationRequested)
                        throw new OperationCanceledException("CompleteSwift cancelled at operator request");
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"{nameof(ValidateIban)}\tFault getting bank details\n{ex.Message}", ex);
            }
            return bic;
        }

        public async static Task<BicFromIbanModel> ValidateBankIdAsync(string ctry, string sort, string acnt)
        {
            BicFromIbanModel output;
            if (string.IsNullOrEmpty(ctry)
                || string.IsNullOrEmpty(sort)
                || string.IsNullOrEmpty(acnt)) //Needs all params
            {
                output = null;
            }
            else
            {
                var client = new RestClient(basePath);
                try
                {
                    var response = await client.ExecuteAsync(getRequest(ctry, sort, acnt));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        output = JsonConvert.DeserializeObject<BicFromIbanModel>(response.Content);
                    }
                    else
                    {
                        output = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"{nameof(ValidateBankIdAsync)}\tFault getting bank details\n{ex.Message}", ex);
                }
            }
            return output;
        }

    }
}
