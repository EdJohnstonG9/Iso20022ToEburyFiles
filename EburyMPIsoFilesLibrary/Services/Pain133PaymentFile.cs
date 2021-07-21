using EburyApiWrapper.Beneficiaries;
using EburyMPIsoFilesLibrary.Helpers;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_001_03;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_003_03;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace EburyMPIsoFilesLibrary.Services
{
    public class Pain133PaymentFile : IInputPaymentFile<PaymentsInitiation001003v03>
    {
        const string pain_namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.003.03";
        Type inputType = typeof(PaymentsInitiation001003v03);
        private PaymentsInitiation001003v03 _isoPayments;
        public PaymentsInitiation001003v03 IsoPayments
        {
            get { return _isoPayments; }
            set { _isoPayments = value; }
        }

        public decimal ControlTotal { get { return getControlTotal(); } }

        private decimal getControlTotal()
        {
            decimal output;
            if (IsoPayments != null)
                output = IsoPayments.CstmrCdtTrfInitn.GrpHdr.CtrlSum;
            else
                output = 0;
            return output;
        }

        private XmlDocument getPaymentFile(string paymentFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            FileInfo fi = new FileInfo(paymentFile);
            if (!fi.Exists)
                throw new ApplicationException($"XML file could not be found: {paymentFile}");
            xmlDoc.Load(paymentFile);
            return xmlDoc;
        }
        private T getPaymentsInitiationSwift<T>(XmlDocument xmlDoc)
        {
            T payments = default;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
            payments = (T)reader.Deserialize(new System.IO.StringReader(xmlDoc.OuterXml));
            return payments;
        }
        /// <summary>
        /// Read the file of either 00100103 or 00100303 format
        /// Save to intneral property matching the type
        /// </summary>
        /// <param name="PaymentsFile"></param>
        /// <returns>Number of payments</returns>
        public int ReadPaymentsFile(string PaymentsFile)
        {
            int output = 0;
            IsoPayments = null;
            try
            {
                var xmlDoc = getPaymentFile(PaymentsFile);
                var xmlNamespace = xmlDoc.GetElementsByTagName("Document")[0]?.NamespaceURI;
                if (xmlNamespace == pain_namespace)
                {
                    IsoPayments = getPaymentsInitiationSwift<PaymentsInitiation001003v03>(xmlDoc);
                    foreach (var payInf in IsoPayments.CstmrCdtTrfInitn.PmtInf)
                    {
                        output += payInf.CdtTrfTxInf.Count();
                    }
                }
                else
                {
                    throw new ApplicationException($"{nameof(ReadPaymentsFile)}\tUnrecognised XML Document Type: {xmlNamespace}");
                }


            }
            catch (Exception)
            {
                IsoPayments = null;
                return 0;
            }

            return output;
        }

        public List<MassPaymentFileModel> MassPaymentFileList()
        {
            List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
            if (IsoPayments != null)
                outPayments = GetPaymentFileList(IsoPayments);
            return outPayments;
        }

        private List<MassPaymentFileModel> GetPaymentFileList<T>(T PaymentsIn)
        {
            List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
            if (typeof(T) == inputType)
            {
                var payments = PaymentsIn as PaymentsInitiation001003v03;
                foreach (var paymentInstruction in payments.CstmrCdtTrfInitn.PmtInf)
                {
                    foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                    {
                        outPayments.Add(creditTransfer.GetPaymentFromISO(paymentInstruction));
                    }
                }
            }
            return outPayments;
        }


        public List<NewBenePaymentModel> NewBeneficarisList<T>(T PaymentsIn)
        {
            if (typeof(T) == inputType)
            {
                var payments = PaymentsIn as PaymentsInitiation001003v03;
                List<NewBenePaymentModel> outPayments = new List<NewBenePaymentModel>();
                foreach (var paymentInstruction in payments.CstmrCdtTrfInitn.PmtInf)
                {
                    foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                    {
                        outPayments.Add(creditTransfer.GetBeneFromIso(paymentInstruction));
                    }
                }
                return outPayments;
            }
            else
            {
                return null;
            }
        }

    }
}
