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
        public string Token { get; private set; }
        string basePath = @"https://apps.applyfinancial.co.uk/validate-api/rest";
        private NetworkCredential _credential;

        public ApplyFinancialsService()
        {

        }
        public ApplyFinancialsService(ApplyConfiguration config)
        {
            basePath = config.BaseUrl;
            Authenticate(config.Credentials);
            _credential = config.Credentials;
        }

        #region authenticate
        public HttpStatusCode Authenticate(NetworkCredential credential = null)
        {
            try
            {
                if (credential == null) credential = _credential;
                RestClient client = new RestClient(basePath);
                var request = getAuthenticateRequest(credential);
                var response = client.Post(request);
                Token = authToken(response);
                _credential = credential;
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                throw new AccessViolationException($"{nameof(Authenticate)}\tFailed to log in to ApplyFinancials", ex);
            }

        }
        private RestRequest getAuthenticateRequest(NetworkCredential credential)
        {
            string reqPath = @"/authenticate";
            Method method = Method.POST;

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
            if (string.IsNullOrEmpty(Token))
                throw new AccessViolationException($"{nameof(Convert)}\tMust authenticate before Conver");
            try
            {
                RestClient client = new RestClient(basePath);
                var request = getConvertRequest(countryCode, branchId, accountNo);
                var response = client.Get(request);
                var output = getConvert(response);
                return output;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"{nameof(Convert)}\tIssue when getting bank details\t{countryCode}\t{branchId}\t{accountNo}", ex);
            }

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
