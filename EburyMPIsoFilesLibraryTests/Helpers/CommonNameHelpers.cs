using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace EburyMPIsoFilesLibraryTests.Helpers
{

    public class CommonNameHelpers
    {

        [Theory]
        [InlineData(
            @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Multi-",
                @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Arbeitnehmer_2023_04 (1).XML",
                @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Finanzamt_2023_04.XML",
                @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Krankenkassen_2023_04 (1).XML"
            )]
        [InlineData(
            @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Arbeitnehmer_2023_04 (1).XML",
                @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES\TestData321081_Zahlungsdatei_Arbeitnehmer_2023_04 (1).XML"
            )]
        public void FilesNameTest(string expected, params string[] files)
        {
            if (files == null || files.Length == 0)
                throw new ArgumentNullException($"{nameof(files)} must contain 1 or more names");
            var fi = new FileInfo(files.First());
            var ext = fi.Extension;
            var path = fi.DirectoryName;
            string common = fi.Name;
            foreach (var file in files)
            {
                var fiName = new FileInfo(file).Name;
                common = string.Concat(common.TakeWhile((c, i) => c == fiName[i]));
                Debug.Print(common);
            }

            string output = fi.Name;
            if (output != common)
            {
                output = $"{path}\\{common}Multi-{DateTimeOffset.Now:yyMMdd-hhmmss}{ext}";
            }
            else
            {
                output = fi.FullName;
            }
            Assert.Contains(expected, output);
        }


    }
}
