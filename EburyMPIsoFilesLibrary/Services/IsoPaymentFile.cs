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
    public class IsoPaymentFile 
    {
        const string pain_001_001_03 = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03";
        const string pain_001_003_03 = "urn:iso:std:iso:20022:tech:xsd:pain.001.003.03";
        const string pain_001_001_03_ch = @"http://www.six-interbank-clearing.com/de/pain.001.001.03.ch.02.xsd";
        public PaymentsInitiation001001v03 Payments001001 { get; private set; }
        public PaymentsInitiation001003v03 Payments001003 { get; private set; }
        public decimal ControlTotal { get { return getControlTotal(); } }

        public IsoPaymentFile()
        {
        }

        private decimal getControlTotal()
        {
            decimal output;
            if (Payments001001 != null)
                output = Payments001001.CstmrCdtTrfInitn.GrpHdr.CtrlSum;
            else if (Payments001003 != null)
                output = Payments001003.CstmrCdtTrfInitn.GrpHdr.CtrlSum;
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

        private XmlDocument replaceNamespace(XmlDocument docIn, string newNamespace)
        {
            var oldNamespace = GetNamespaceUri(docIn);
            var newXml = docIn.OuterXml.Replace(oldNamespace, newNamespace);
            var output = new XmlDocument();
            output.LoadXml(newXml);
            return output;
        }
        private PaymentsInitiation001001v03 getPaymentsInitiationSwift(XmlDocument xmlDoc)
        {
            PaymentsInitiation001001v03 payments = null;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PaymentsInitiation001001v03));
            payments = (PaymentsInitiation001001v03)reader.Deserialize(new System.IO.StringReader(xmlDoc.OuterXml));
            return payments;
        }
        private PaymentsInitiation001003v03 getPaymentsInitiationSepa(XmlDocument xmlDoc)
        {
            PaymentsInitiation001003v03 payments = null;
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PaymentsInitiation001003v03));
                payments = (PaymentsInitiation001003v03)reader.Deserialize(new System.IO.StringReader(xmlDoc.OuterXml));
            }
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
            Payments001001 = null;
            Payments001003 = null;
            try
            {
                var xmlDoc = getPaymentFile(PaymentsFile);
                string xmlNamespace = GetNamespaceUri(xmlDoc);

                if (xmlNamespace == pain_001_001_03_ch)
                {
                    xmlDoc = replaceNamespace(xmlDoc, pain_001_001_03);
                    xmlNamespace = GetNamespaceUri(xmlDoc);
                }

                if (xmlNamespace == pain_001_001_03)
                {
                    Payments001001 = getPaymentsInitiationSwift(xmlDoc);
                    foreach (var payInf in Payments001001.CstmrCdtTrfInitn.PmtInf)
                    {
                        output += payInf.CdtTrfTxInf.Count();
                    }
                }
                else if (xmlNamespace == pain_001_003_03)
                {
                    Payments001003 = getPaymentsInitiationSepa(xmlDoc);
                    foreach (var payInf in Payments001003.CstmrCdtTrfInitn.PmtInf)
                    {
                        output += payInf.CdtTrfTxInf.Count();
                    }
                }
                else
                {
                    throw new ApplicationException($"{nameof(ReadPaymentsFile)}\tUnrecognised XML Document Type: {xmlNamespace}");
                }


            }
            catch (Exception ex)
            {
                throw;
            }

            return output;
        }

        private static string GetNamespaceUri(XmlDocument xmlDoc)
        {
            return xmlDoc.GetElementsByTagName("Document")[0]?.NamespaceURI;
        }

        public List<MassPaymentFileModel> GetPaymentFileList()
        {
            List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
            if (Payments001001 != null)
                outPayments = GetPaymentFileList(Payments001001);
            else if (Payments001003 != null)
                outPayments = GetPaymentFileList(Payments001003);
            return outPayments;
        }

        private List<MassPaymentFileModel> GetPaymentFileList<T>(T PaymentsIn)
        {
            List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
            if (typeof(PaymentsInitiation001001v03) == typeof(T))
            {
                var payments = PaymentsIn as PaymentsInitiation001001v03;
                foreach (var paymentInstruction in payments.CstmrCdtTrfInitn.PmtInf)
                {
                    foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                    {
                        outPayments.Add(creditTransfer.GetPaymentFromISO(paymentInstruction));
                    }
                }
            }
            else if (typeof(PaymentsInitiation001003v03) == typeof(T))
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
            if (typeof(T) == typeof(PaymentsInitiation001001v03))
            {
                return newBeneficarisList(PaymentsIn as PaymentsInitiation001001v03);
            }
            else
            {
                return null;
            }
        }

        private List<NewBenePaymentModel> newBeneficarisList(PaymentsInitiation001001v03 PaymentsIn = null)
        {
            if (PaymentsIn == null) PaymentsIn = Payments001001;
            //if (PaymentsIn == null) PaymentsIn = Payments001003;
            List<NewBenePaymentModel> outPayments = new List<NewBenePaymentModel>();
            //var groupHeader = PaymentsIn.CstmrCdtTrfInitn.GrpHdr;
            foreach (var paymentInstruction in PaymentsIn.CstmrCdtTrfInitn.PmtInf)
            {
                foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                {
                    outPayments.Add(creditTransfer.GetBeneFromIso(paymentInstruction));
                }
            }
            return outPayments;
        }


    }
}
