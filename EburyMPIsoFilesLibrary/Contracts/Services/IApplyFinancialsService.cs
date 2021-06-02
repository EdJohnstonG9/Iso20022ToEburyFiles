using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using System.Net;
using System.Security;

namespace EburyMPIsoFilesLibrary.Services
{
    public interface IApplyFinancialsService
    {
        string Token { get; }

        HttpStatusCode Authenticate(NetworkCredential credential);
        ConvertResponse Convert(string countryCode, string branchId, string accountNo);
    }
}