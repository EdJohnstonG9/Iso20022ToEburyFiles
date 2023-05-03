using Xunit;
using EburyMPIsoFilesLibrary.Helpers;

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EburyMPIsoFilesLibrary.Helpers.Tests
{
    public class ValidateCharacterSetHelperTests
    {

        [Theory()]
        [InlineData("ÖGK Wien", "OGK Wien")]
        [InlineData("Alserbachstraße", "Alserbachstrasse")]
        [InlineData("Wienerbergstraße", "Wienerbergstrasse")]
        [InlineData("Peanut", "Peanut")]
        [InlineData("SERTGasdfg/-?:().,'+ Peanut", "SERTGasdfg/-?:().,'+ Peanut")]
        [InlineData("Pe.a$n%ut", "Pe.anut")]
        [InlineData("Pea$n%ut\nPeanut", "Peanut\nPeanut")]
        [InlineData("Peanut" + "\u0080", "Peanut")]
        //[InlineData("Peanut", "Peanut")]
        //[InlineData("Peanut", "Peanut")]
        //[InlineData("Peanut", "Peanut")]
        //[InlineData("Peanut", "Peanut")]
        public void ToAsciiCharsTest(string input, string expected)
        {
            var result = input.ToAsciiChars();
            Assert.Equal(expected, result);
        }
    }
}