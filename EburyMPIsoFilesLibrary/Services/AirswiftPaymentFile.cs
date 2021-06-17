using CsvHelper;
using CsvHelper.Configuration;
using EburyMPIsoFilesLibrary.Helpers;
using EburyMPIsoFilesLibrary.Models.Airswift;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EburyMPIsoFilesLibrary.Services
{
    public class AirswiftPaymentFile : IInputPaymentFile<AirswiftPaymentModel>
    {
        private AirswiftModel IfhRecord { get; set; }
        public List<AirswiftPaymentModel> InputPaymentList { get; set; }
        public string SettlementCcy { get; set; }
        //public AirswiftModel[] RawAirswiftData { get; set; }

        private IApplyFinancialsService _apply;
        public AirswiftPaymentFile(IApplyFinancialsService apply)
        {
            _apply = apply;
            _apply.Authenticate();
        }
        #region ReadPaymentsFile
        public int ReadPaymentsFile(string fullPathName)
        {
            InputPaymentList = readAirswiftFile(fullPathName);
            if (InputPaymentList.Count > 0 && IfhRecord != null)
                return InputPaymentList.Count;
            else
                return 0;
        }

        private List<AirswiftPaymentModel> readAirswiftFile(string fullPathName)
        {
            try
            {
                List<AirswiftPaymentModel> output = new List<AirswiftPaymentModel>();
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = (headerNames, index, context) =>
                    {
                    }
                };
                using (var reader = new StreamReader(fullPathName))
                {
                    using (CsvReader csvReader = new CsvReader(reader, config))
                    {
                        AirswiftPaymentModel payment = new AirswiftPaymentModel();
                        while (csvReader.Read())
                        {
                            var record = csvReader.GetRecord<AirswiftModel>();
                            switch (record.RecType)
                            {
                                case "IFH":
                                    if (IfhRecord != null)
                                        throw new ApplicationException("Bad Record Sequence, duplicate IFH records");
                                    IfhRecord = record;
                                    break;
                                case "BATHDR":
                                    if (payment.BatHdr != null)
                                        throw new ApplicationException("Bad Record Sequence, duplicate BATHDR records");
                                    payment.BatHdr = record;
                                    break;
                                case "SECPTY":
                                    if (payment.SecPty != null)
                                        throw new ApplicationException("Bad Record Sequence, duplicate SECPTY records");
                                    payment.SecPty = record;
                                    break;
                                case "ADV":
                                    if (payment.Adv != null)
                                        throw new ApplicationException("Bad Record Sequence, duplicate ADV records");
                                    payment.Adv = record;
                                    payment = savePayment(payment, output);
                                    break;
                                default:
                                    throw new ApplicationException("Bad Record Sequence, Unrecognised Record Type: " + record.RecType);
                                    break;
                            }
                        }
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                return new List<AirswiftPaymentModel>();
            }
        }

        private AirswiftPaymentModel savePayment(AirswiftPaymentModel payment, List<AirswiftPaymentModel> paymentList)
        {
            if (payment.BatHdr == null || payment.SecPty == null || payment.Adv == null)
                throw new ApplicationException("Bad Record Sequence");
            if (paymentList == null)
                paymentList = new List<AirswiftPaymentModel>();
            paymentList.Add(payment);
            return new AirswiftPaymentModel();
        }
        #endregion

        #region PassPaymentFile
        public List<MassPaymentFileModel> MassPaymentFileList()
        {
            var output = new List<MassPaymentFileModel>();
            foreach (var item in InputPaymentList)
            {
                output.Add(item.GetPaymentFromAirswift(SettlementCcy, _apply));
            }
            return output;
        }
        #endregion
    }
}
