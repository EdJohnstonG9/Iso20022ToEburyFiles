using EburyApiWrapper.Beneficiaries;

using EburyMassFilesWPF.Core.Models.ISO20022.pain_001_001_09;

using EburyMPIsoFilesLibrary.Helpers;
using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_001_03;
using EburyMPIsoFilesLibrary.Models.ISO20022.pain_001_003_03;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;

namespace EburyMPIsoFilesLibrary.Services
{
    public class IsoPaymentFile
    {
        const string pain_001_001_03 = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03";
        const string pain_001_001_09 = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09";
        const string pain_001_003_03 = "urn:iso:std:iso:20022:tech:xsd:pain.001.003.03";
        const string pain_001_001_03_ch = @"http://www.six-interbank-clearing.com/de/pain.001.001.03.ch.02.xsd";
        public PaymentsInitiation001001v03 Payments001001v03 { get; private set; }
        public PaymentsInitiation001003v03 Payments001003v03 { get; private set; }
        public PaymentsInitiation001001v09 Payments001001v09 { get; private set; }
        public object PaymentsFile { get; private set; }
        public decimal ControlTotal { get { return getControlTotal(); } }

        public IsoPaymentFile()
        {
        }

        private decimal getControlTotal()
        {
            decimal output;
            if (Payments001001v03 != null)
                output = Payments001001v03.CstmrCdtTrfInitn.GrpHdr.CtrlSum;
            else if (Payments001003v03 != null)
                output = Payments001003v03.CstmrCdtTrfInitn.GrpHdr.CtrlSum;
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
        private PaymentsInitiation001001v09 getPaymentsInitiationV09(XmlDocument xmlDoc)
        {
            PaymentsInitiation001001v09 payments = null;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PaymentsInitiation001001v09));
            payments = (PaymentsInitiation001001v09)reader.Deserialize(new System.IO.StringReader(xmlDoc.OuterXml));
            return payments;
        }
        private T GetPaymentsFromXml<T>(XmlDocument xmlDoc) where T : class
        {
            T payments = null;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
            payments = (T)reader.Deserialize(new System.IO.StringReader(xmlDoc.OuterXml));
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
        /// <param name="PaymentsContent"></param>
        /// <returns>Number of payments</returns>
        public int ReadPaymentsFile(string PaymentsContent)
        {
            int output = 0;
            Payments001001v03 = null;
            Payments001003v03 = null;
            try
            {
                var xmlDoc = getPaymentFile(PaymentsContent);
                string xmlNamespace = GetNamespaceUri(xmlDoc);

                if (xmlNamespace == pain_001_001_03_ch)
                {
                    xmlDoc = replaceNamespace(xmlDoc, pain_001_001_03);
                    xmlNamespace = GetNamespaceUri(xmlDoc);
                }

                if (xmlNamespace == pain_001_001_03)
                {
                    Payments001001v03 = getPaymentsInitiationSwift(xmlDoc);
                    PaymentsFile = GetPaymentsFromXml<PaymentsInitiation001001v03>(xmlDoc);
                    foreach (var payInf in Payments001001v03.CstmrCdtTrfInitn.PmtInf)
                    {
                        output += payInf.CdtTrfTxInf.Count();
                    }
                }
                else if (xmlNamespace == pain_001_001_09)
                {
                    Payments001001v09 = getPaymentsInitiationV09(xmlDoc);
                    PaymentsFile = GetPaymentsFromXml<PaymentsInitiation001001v09>(xmlDoc);
                    foreach (var payInf in Payments001001v09.CstmrCdtTrfInitn.PmtInf)
                    {
                        output += payInf.CdtTrfTxInf.Count();
                    }
                }
                else if (xmlNamespace == pain_001_003_03)
                {
                    Payments001003v03 = getPaymentsInitiationSepa(xmlDoc);
                    PaymentsFile = GetPaymentsFromXml<PaymentsInitiation001003v03>(xmlDoc);
                    foreach (var payInf in Payments001003v03.CstmrCdtTrfInitn.PmtInf)
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

        //public List<MassPaymentFileModel> GetPaymentFileList<T>(T input)
        //{
        //    if (input is PaymentsInitiation001001v03 input00100103)
        //    {
        //        return GetPaymentFileList(input00100103);
        //    }
        //    List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
        //    if (Payments001001v03 != null)
        //        outPayments = GetPaymentFileList(Payments001001v03);
        //    else if (Payments001003v03 != null)
        //        outPayments = GetPaymentFileList(Payments001003v03);
        //    return outPayments;
        //}

        public List<MassPaymentFileModel> GetPaymentFileList<T>(T PaymentsIn)
        {
            List<MassPaymentFileModel> outPayments = new List<MassPaymentFileModel>();
            if (PaymentsIn is PaymentsInitiation001001v03 payments113)
            {
                foreach (var paymentInstruction in payments113.CstmrCdtTrfInitn.PmtInf)
                {
                    foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                    {
                        outPayments.Add(creditTransfer.GetPaymentFromISO(paymentInstruction));
                    }
                }
            }
            else if(PaymentsIn is PaymentsInitiation001001v09 payments119)
            {
                foreach (var paymentInstruction in payments119.CstmrCdtTrfInitn.PmtInf)
                {
                    foreach (var creditTransfer in paymentInstruction.CdtTrfTxInf)
                    {
                        outPayments.Add(creditTransfer.GetPaymentFromISO(paymentInstruction));
                    }
                }
            }
            else if (PaymentsIn is PaymentsInitiation001003v03 payments133)
            {
                foreach (var paymentInstruction in payments133.CstmrCdtTrfInitn.PmtInf)
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
            if (PaymentsIn == null) PaymentsIn = Payments001001v03;
            //if (PaymentsIn == null) PaymentsIn = Payments001003v03;
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
