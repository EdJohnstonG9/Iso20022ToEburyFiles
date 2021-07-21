using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using EburyMPIsoFilesLibrary.Models;
using System.IO;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class ProcessPaymentFilesTests
    {
        InputTypeModel inputType;
        string root = @"G:\Shared drives\MP - High Wycombe - Data\";

        public ProcessPaymentFilesTests()
        {
            inputType = new InputTypeModel();
            inputType.InputType = InputTypeModel.InputFileType.AirswiftText;
            inputType.InputExt = ".TXT";
            inputType.OuputExt = ".csv";
        }

        [Fact()]
        public ProcessPaymentFiles ProcessPaymentFilesTest()
        {
            inputType = new InputTypeModel();
            inputType.InputType = InputTypeModel.InputFileType.AirswiftText;
            inputType.InputExt = ".TXT";
            inputType.OuputExt = ".csv";

            var result = new ProcessPaymentFiles(inputType);
            return result;
        }

        [Fact()]
        public void ReadInputFilesAirswiftTest()
        {
            var service = ProcessPaymentFilesTest();
            string path = @"Airswift\AirEnergi\";
            List<String> inFiles = new List<string>()
            {
                Path.Combine(root,path,"0705_FLOUR.TXT"),
                Path.Combine(root,path,"0705_OTHERSFINAL.TXT")
            };
            var actual = service.ReadInputFiles(inFiles, root, root);
            Assert.Contains("Success", actual);
            Assert.DoesNotContain("FAILED)", actual);
        }
        [Fact()]
        public void ReadInputFilesISOTest()
        {
            var service = ProcessPaymentFilesTest();
            string path = @"XML FILE EXAMPLES\AirBnB\";
            inputType.InputExt = ".xml";
            inputType.InputType = InputTypeModel.InputFileType.ISOPain113;
            List<String> inFiles = new List<string>()
            {
                Path.Combine(root,path,"payments_employees_AIR041_122020.xml"),
                Path.Combine(root,path,"Social insurance_AIR041_122020.xml")
            };
            var actual = service.ReadInputFiles(inFiles, root, root);
            Assert.Contains("Success", actual);
            Assert.DoesNotContain("FAILED)", actual);
        }
    }
}