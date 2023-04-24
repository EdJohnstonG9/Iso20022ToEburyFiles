using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EburyMPIsoFilesLibrary.Models.ApplyFinancials
{

    public class ApplyConfiguration
    {
        public NetworkCredential Credentials { get; set; } = new NetworkCredential();
        public string BaseUrl { get; set; }
    }

    public class AuthenticateResponse
    {
        public string status { get; set; }
        public string comment { get; set; }
        public string token { get; set; }
    }

    public class ConvertResponse
    {
        public string countryCode { get; set; }
        public string nationalId { get; set; }
        public string accountNumber { get; set; }
        public string recommendedNatId { get; set; }
        public string recommendedAcct { get; set; }
        public string recommendedBIC { get; set; }
        public string status { get; set; }
        public string comment { get; set; }
        public string _ref { get; set; }
        public string group { get; set; }
        public Branchdetail[] branchDetails { get; set; }
        public Headofficedetails headOfficeDetails { get; set; }
        public Paymentbicdetails paymentBicDetails { get; set; }
        public string bic8 { get; set; }
        public string dataStore { get; set; }
        public string noBranch { get; set; }
        public string isoAddr { get; set; }
        public string payBranchType { get; set; }
        public string freeToken { get; set; }
    }

    public class Headofficedetails
    {
        public string bankName { get; set; }
        public string branch { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postZip { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public Codedetails codeDetails { get; set; }
        public Additionaldata additionalData { get; set; }
    }

    public class Codedetails
    {
        public string codeName1 { get; set; }
        public string codeValue1 { get; set; }
        public string codeName2 { get; set; }
        public string codeValue2 { get; set; }
        public string codeName3 { get; set; }
        public string codeValue3 { get; set; }
        public string codeName4 { get; set; }
        public string codeValue4 { get; set; }
    }

    public class Additionaldata
    {
        public string ssiAvailable { get; set; }
        public string payServiceAvailable { get; set; }
        public string contactsAvailable { get; set; }
        public string messageAvailable { get; set; }
        public string holidayAvailable { get; set; }
    }

    public class Paymentbicdetails
    {
        public string branchTypeLabel { get; set; }
        public string bankName { get; set; }
        public string branch { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postZip { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public Codedetails codeDetails { get; set; }
        public Additionaldata additionalData { get; set; }
    }

    public class Branchdetail
    {
        public string bankName { get; set; }
        public string branch { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postZip { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public Codedetails codeDetails { get; set; }
        public Sepadetails sepaDetails { get; set; }
        public Additionaldata additionalData { get; set; }
        public string bankToken { get; set; }
    }

    public class Sepadetails
    {
        public string ctStatus { get; set; }
        public string ddStatus { get; set; }
        public string bbStatus { get; set; }
    }

}
