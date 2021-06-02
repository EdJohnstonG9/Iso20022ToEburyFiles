using EburyMPIsoFilesLibrary.Helpers;
using EburyMPIsoFilesLibrary.Models.Bacs;
using EburyMPIsoFilesLibrary.Models.Ebury;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace EburyMPIsoFilesLibrary.Services
{
    public class BacsPaymentFile : IInputPaymentFile<BacsModel>
    {
        public string XlPassword { get; set; }
        public List<BacsModel> BacsPayments { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        private IApplyFinancialsService _apply;

        public BacsPaymentFile()
        {

        }
        public BacsPaymentFile(IApplyFinancialsService apply)
        {
            _apply = apply;
            //Required for the ReadExcel package ref: https://github.com/ExcelDataReader/ExcelDataReader#important-note-on-net-core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public List<MassPaymentFileModel> MassPaymentFileList()
        {
            List<MassPaymentFileModel> output = new List<MassPaymentFileModel>();
            
            foreach(var payment in BacsPayments)
            {
                var apply = _apply.Convert("GB", payment.SortCode, payment.AccountNo);
                var emp = payment.GetPaymentFromBacs(DateTime.Today.AddDays(1), apply);
                output.Add(emp);
            }
            return output;
        }

        public int ReadPaymentsFile(string fullPathName)
        {
            if (!new FileInfo(fullPathName).Exists)
            {
                throw new ApplicationException($"{nameof(ReadPaymentsFile)}\tFile does not exist: {fullPathName}");
            }

            try
            {
                var dataset = getExceToDataset(fullPathName);
                BacsPayments = new List<BacsModel>();
                foreach (var row in dataset.Tables[0].Rows)
                {
                    var payment = getBacsData((DataRow)row);
                    if (payment != null)
                        BacsPayments.Add(payment);
                }
                var totals = findTotals(dataset.Tables[0]);
                if (totals != null)
                {
                    TotalAmount = getValueDec(totals, 1);
                    TotalCount = getValueInt(totals, 2);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return 0;
        }

        private DataSet getExceToDataset(string fullPathName)
        {
            DataSet output;
            using (var fs = new FileStream(fullPathName, FileMode.Open, FileAccess.Read))
            {
                var xlConfig = new ExcelReaderConfiguration();
                xlConfig.FallbackEncoding = Encoding.GetEncoding(1252);
                if (XlPassword != null)
                    xlConfig.Password = XlPassword;
                xlConfig.LeaveOpen = false;
                using (var xlreader = ExcelReaderFactory.CreateReader(fs, xlConfig))
                {
                    output = xlreader.AsDataSet();
                }
            }
            return output;
        }

        private BacsModel getBacsData(DataRow drBacs)
        {
            try
            {
                BacsModel output = new BacsModel();

                output.BacsRec = getValueInt(drBacs, 0);
                output.EmployeeName = getValueStr(drBacs, 2);
                output.SortCode = getValueStr(drBacs, 3);
                output.AccountNo = getValueStr(drBacs, 4);
                output.AcType = getValueInt(drBacs, 5);
                output.BuildingSoctRef = drBacs[6].ToString();
                output.AccountName = getValueStr(drBacs, 8);
                output.Amount = getValueDec(drBacs, 9);
                output.Description = getValueStr(drBacs, 11);
                return output;
            }
            catch (Exception)
            {
                return null;
            }

        }
        private string getValueStr(DataRow drBacs, int col)
        {
            string res = drBacs[col].ToString().Trim();
            if (string.IsNullOrEmpty(res))
                throw new ArgumentException("String expected");
            return res;
        }
        private int getValueInt(DataRow drBacs, int col)
        {
            int iRes;
            if (!int.TryParse(drBacs[col].ToString(), out iRes))
                throw new ArgumentException("Int expected");
            return iRes;
        }
        private decimal getValueDec(DataRow drBacs, int col)
        {
            decimal res;
            if (!decimal.TryParse(drBacs[col].ToString(), out res))
                throw new ArgumentException("Int expected");
            return res;
        }

        private DataRow findTotals(DataTable bacs)
        {
            foreach (var row in bacs.Rows)
            {
                var dr = row as DataRow;
                if (dr[0].ToString().Contains("TOTAL BACS PAYMENTS"))
                {
                    return dr;
                }
            }
            return null;
        }
    }
}
