using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Models.Bacs
{
    public class BacsModel
    {
        public int BacsRec { get; set; }
        public string EmployeeName { get; set; }
        public string SortCode { get; set; }
        public string AcType { get; set; }
        public string BuildingSoctRef { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

    }
}
