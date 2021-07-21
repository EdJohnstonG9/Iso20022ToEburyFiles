using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Models
{
    public class InputTypeModel
    {
        public enum InputFileType
        {
            ISOPain113,
            ISOPain133,
            AirswiftText,
            BacsXls
        }
        public InputFileType InputType { get; set; }
        public string FileMask { get; set; }
        public string InputExt { get; set; }
        public string OuputExt { get; set; }
        public string DisplayTitle { get; set; }
    }
}
