using EburyMPIsoFilesLibrary.Models.ApplyFinancials;

using System.Net;
using System.Security;

namespace EburyMPIsoFilesLibrary.Services
{
    public interface IApplyFinancialsService
    {
        string Token { get; }

        HttpStatusCode Authenticate(NetworkCredential credential = null);
        ConvertResponse Convert(string countryCode, string branchId, string accountNo);
        ConvertResponse Validate(string bic, string iban);
    }
}