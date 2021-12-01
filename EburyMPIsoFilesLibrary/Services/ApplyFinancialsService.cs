using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;

namespace EburyMPIsoFilesLibrary.Services
{
    //https://documents.applyfinancial.co.uk/
    public class ApplyFinancialsService : IApplyFinancialsService
    {
        const string basePath = @"https://apps.applyfinancial.co.uk/validate-api/rest";

        public string Token { get; private set; }

        #region authenticate
        public HttpStatusCode Authenticate(string username, SecureString password)
        {
            RestClient client = new RestClient(basePath);
            var request = getAuthenticateRequest(username, password);
            var response = client.Post(request);
            Token = authToken(response);
            return response.StatusCode;
        }
        private RestRequest getAuthenticateRequest(string username, SecureString password)
        {
            string reqPath = @"/authenticate";
            Method method = Method.POST;

            var credential = new NetworkCredential(username, password);
            var request = new RestRequest(reqPath, method);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("username", credential.UserName, "application/x-www-form-urlencoded", ParameterType.GetOrPost);
            request.AddParameter("password", credential.Password, "application/x-www-form-urlencoded", ParameterType.GetOrPost);

            return request;
        }
        private string authToken(IRestResponse response)
        {
            string output = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    AuthenticateResponse auth = JsonConvert.DeserializeObject<AuthenticateResponse>(response.Content);
                    output = auth.token;
                }
                catch (Exception)
                {
                }
            }
            return output;
        }
        #endregion

        #region convert
        public ConvertResponse Convert(string countryCode, string branchId, string accountNo)
        {
            RestClient client = new RestClient(basePath);
            var request = getConvertRequest(countryCode, branchId, accountNo);
            var response = client.Get(request);
            var output = getConvert(response);
            return output;
        }

        public ConvertResponse Validate(string bic, string iban)
        {
            RestClient client = new RestClient(basePath);

            RestRequest request = ValidateRequest(bic, iban);

            var response = client.Get(request);
            var output = getConvert(response);
            return output;
        }

        private RestRequest ValidateRequest(string bic, string iban)
        {
            string reqPath = @"/convert/1.0.1";
            Method method = Method.GET;
            string countryCode = bic.Substring(4, 2);
            RestRequest request = new RestRequest(reqPath, method);
            request.AddParameter("countryCode", countryCode, ParameterType.QueryString);
            request.AddParameter("nationalId", bic, ParameterType.QueryString);
            request.AddParameter("accountNumber", iban, ParameterType.QueryString);
            request.AddParameter("errorComment", "E", ParameterType.QueryString);
            request.AddParameter("token", Token, ParameterType.QueryString);
            return request;
        }

        private RestRequest getConvertRequest(string countryCode, string nationalId, string accountNumber)
        {

            string reqPath = @"/convert/1.0.1";
            Method method = Method.GET;

            var request = new RestRequest(reqPath, method);
            request.AddParameter("countryCode", countryCode, ParameterType.QueryString);
            request.AddParameter("nationalId", nationalId, ParameterType.QueryString);
            request.AddParameter("accountNumber", accountNumber, ParameterType.QueryString);
            request.AddParameter("token", Token, ParameterType.QueryString);

            return request;
        }
        private ConvertResponse getConvert(IRestResponse response)
        {
            ConvertResponse output = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    ConvertResponse convert = JsonConvert.DeserializeObject<ConvertResponse>(response.Content);
                    output = convert;
                }
                catch (Exception)
                {
                }
            }
            return output;
        }


        #endregion
    }
}
