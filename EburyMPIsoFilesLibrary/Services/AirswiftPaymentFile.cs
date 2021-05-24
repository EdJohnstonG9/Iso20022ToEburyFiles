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
    public class AirswiftPaymentFile
    {
        public AirswiftModel IfhRecord { get; set; }
        public List<AirswiftPaymentModel> AirswiftPaymentList { get; set; }
        //public AirswiftModel[] RawAirswiftData { get; set; }

        public AirswiftPaymentFile()
        {

        }

        public bool ReadPaymentsFile(string fullPathName)
        {
            AirswiftPaymentList = readAirswiftFile(fullPathName);
            if (AirswiftPaymentList.Count > 0 && IfhRecord != null)
                return true;
            else
                return false;
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
                throw ex;
            }
        }

        //private AirswiftPaymentModel newPayment(AirswiftPaymentModel payment, AirswiftModel record)
        //{
        //    bool returnNew = false; 
        //    if (payment == default(AirswiftPaymentModel))
        //    {
        //        //do nothing, already is new
        //    }
        //    else
        //    {
        //        if (record.RecType == "BATHDR") //First of 3
        //        {
        //            if (payment.BatHdr == null || payment.SecPty == null || payment.Adv == null)
        //                throw new ApplicationException("Bad Record Sequence");
        //            returnNew = true; ;
        //        }
        //    }
        //    if (returnNew)
        //        return new AirswiftPaymentModel();
        //    else
        //        return payment;
        //}
        private AirswiftPaymentModel savePayment(AirswiftPaymentModel payment, List<AirswiftPaymentModel> paymentList)
        {
            if (payment.BatHdr == null || payment.SecPty == null || payment.Adv == null)
                throw new ApplicationException("Bad Record Sequence");
            if (paymentList == null)
                paymentList = new List<AirswiftPaymentModel>(); 
            paymentList.Add(payment);
            return new AirswiftPaymentModel();
        }


        public List<MassPaymentFileModel> GetPaymentFileList()
        {
            var output = new List<MassPaymentFileModel>();
            foreach (var item in AirswiftPaymentList)
            {
                output.Add(item.GetPaymentFromAirswift());
            }
            return output;
        }
    }
}
