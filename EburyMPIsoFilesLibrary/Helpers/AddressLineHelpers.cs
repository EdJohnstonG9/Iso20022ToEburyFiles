using EburyMPIsoFilesLibrary.Models.Ebury;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class AddressLineHelpers
    {
        const string newline = ", ";
        public static string GetOnelineAddress(this MassPaymentFileModel bene)
        {
            string output = bene.GetAddressLine1() + newline;
            bene.SetUsState();
            output = output.AddAddressItem(bene.BeneficiaryCity);
            output = output.AddAddressItem(bene.BeneficiaryCountrySubDivision);
            output = output.AddAddressItem(bene.BeneficiaryPostCode);

            output = string.IsNullOrEmpty(output)
                ? string.Empty
                : output.TrimEnd(newline.ToCharArray());
            return output;
        }
        public static string GetAddressLine1(this MassPaymentFileModel bene)
        {
            string output = string.Empty;
            output = output.AddAddressItem(bene.BeneficiaryStreetName);
            output = output.AddAddressItem(bene.BeneficiaryBuildingNumber);
            output = output.AddAddressItem(bene.BeneficiaryFloor);
            output = output.AddAddressItem(bene.BeneficiaryApartmentOffice);

            output = string.IsNullOrEmpty(output)
                ? string.Empty
                : output.TrimEnd(newline.ToCharArray());
            return output;
        }

        public static string GetOnelineCityRegionPostcode(this MassPaymentFileModel bene)
        {
            string output = string.Empty;
            bene.SetUsState();
            output = output.AddAddressItem(bene.BeneficiaryCity);
            output = output.AddAddressItem(bene.BeneficiaryCountrySubDivision);
            output = output.AddAddressItem(bene.BeneficiaryPostCode);
            return output.TrimEnd(newline.ToCharArray());
        }
        public static void SetUsState(this MassPaymentFileModel bene)
        {
            if (bene.BeneficiaryCountry == "USA"
                && string.IsNullOrEmpty(bene.BeneficiaryCountrySubDivision)
                && !string.IsNullOrEmpty(bene.BeneficiaryPostCode))
            {
                var match = Regex.Match(bene.BeneficiaryPostCode.Trim(), @"^([A-Z]{2})[, -]([\d-]{4,})$");
                if (match.Success)
                {
                    bene.BeneficiaryCountrySubDivision = match.Groups[1].Value;
                }
            }
        }

        private static string AddAddressItem(this string input, string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return input;
            }
            else
            {
                return input + item + newline;
            }
        }
    }
}
