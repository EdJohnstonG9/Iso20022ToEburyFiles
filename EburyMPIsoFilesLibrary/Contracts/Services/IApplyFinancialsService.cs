using EburyMPIsoFilesLibrary.Models.ApplyFinancials;

using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace EburyMPIsoFilesLibrary.Services
{
    public interface IApplyFinancialsService
    {
        string Token { get; }

        HttpStatusCode Authenticate(NetworkCredential credential = null);
        ConvertResponse Convert(string countryCode, string branchId, string accountNo);
        Task<ConvertResponse> ConvertAsync(string countryCode, string branchId, string accountNo);
        ConvertResponse Validate(string bic, string iban);
    }
}