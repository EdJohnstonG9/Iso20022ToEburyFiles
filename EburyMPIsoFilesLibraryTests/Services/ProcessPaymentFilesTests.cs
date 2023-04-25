using Xunit;
using EburyMPIsoFilesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using EburyMPIsoFilesLibrary.Models;
using System.IO;
using EburyMPIsoFilesLibrary.Models.ApplyFinancials;
using System.Net;
using EburyMPIsoFilesLibraryTests;

namespace EburyMPIsoFilesLibrary.Services.Tests
{
    public class ProcessPaymentFilesTests : ApplyFixture
    {
        InputTypeModel inputType;
        string root = @"G:\Shared drives\MP - High Wycombe - Data\";
        bool runTest;

        ApplyFinancialsService _apply;
        public ProcessPaymentFilesTests()
        {
            _apply = new ApplyFinancialsService(applyConfig());
            runTest = new DirectoryInfo(root).Exists;
        }


        private ApplyConfiguration applyConfig()
        {
            return _applyConfig;
        }
        private InputTypeModel inputTypeModel()
        {
            inputType = new InputTypeModel();
            inputType.InputType = InputTypeModel.InputFileType.AirswiftText;
            inputType.InputExt = ".TXT";
            inputType.OuputExt = ".csv";
            return inputType;
        }

        [Fact()]
        public ProcessPaymentFiles ProcessPaymentFilesTest()
        {
            _apply = new ApplyFinancialsService(applyConfig());

            var result = new ProcessPaymentFiles(_apply);
            result.InputType = inputTypeModel();
            return result;
        }

        [Fact()]
        public void ReadInputFilesAirswiftTest()
        {
            if (runTest)
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
        }
        [Fact()]
        public void ReadInputFilesISOTest()
        {
            if (runTest)
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
}