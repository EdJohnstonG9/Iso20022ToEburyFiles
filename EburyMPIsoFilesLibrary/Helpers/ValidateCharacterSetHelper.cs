using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Helpers
{
    public static class ValidateCharacterSetHelper
    {
        static Dictionary<char, string> _toAscii = new Dictionary<char, string>
        {
            { (char)0xA0, " "},
            { (char)0xA1, "!"},
            { (char)0xA2, "c"},
            { (char)0xA3, "l"},
            { (char)0xA4, "o"},
            { (char)0xA5, "y"},
            { (char)0xA6, "|"},
            { (char)0xA7, "s"},
            { (char)0xA8, ""},
            { (char)0xA9, "c"},
            { (char)0xAA, "a"},
            { (char)0xAB, "<"},
            { (char)0xAC, "!"},
            { (char)0xAD, "-"},
            { (char)0xAE, "r"},
            { (char)0xAF, ""},
            { (char)0xB0, ""},
            { (char)0xB1, "+/-"},
            { (char)0xB2, "2"},
            { (char)0xB3, "3"},
            { (char)0xB4, ""},
            { (char)0xB5, "/"},
            { (char)0xB6, "P!"},
            { (char)0xB7, "."},
            { (char)0xB8, ""},
            { (char)0xB9, "1"},
            { (char)0xBA, "o"},
            { (char)0xBB, ">"},
            { (char)0xBC, ""},
            { (char)0xBD, ""},
            { (char)0xBE, ""},
            { (char)0xBF, "?"},
            { (char)0xC0, "A"},
            { (char)0xC1, "A"},
            { (char)0xC2, "A"},
            { (char)0xC3, "A"},
            { (char)0xC4, "A"},
            { (char)0xC5, "A"},
            { (char)0xC6, "AE"},
            { (char)0xC7, "C"},
            { (char)0xC8, "E"},
            { (char)0xC9, "E"},
            { (char)0xCA, "E"},
            { (char)0xCB, "E"},
            { (char)0xCC, "I"},
            { (char)0xCD, "I"},
            { (char)0xCE, "I"},
            { (char)0xCF, "I"},
            { (char)0xD0, "D"},
            { (char)0xD1, "N"},
            { (char)0xD2, "O"},
            { (char)0xD3, "O"},
            { (char)0xD4, "O"},
            { (char)0xD5, "O"},
            { (char)0xD6, "O"},
            { (char)0xD7, "x"},
            { (char)0xD8, "O"},
            { (char)0xD9, "U"},
            { (char)0xDA, "U"},
            { (char)0xDB, "U"},
            { (char)0xDC, "U"},
            { (char)0xDD, "Y"},
            { (char)0xDE, "P"},
            { (char)0xDF, "ss"},
            { (char)0xE0, "a"},
            { (char)0xE1, "a"},
            { (char)0xE2, "a"},
            { (char)0xE3, "a"},
            { (char)0xE4, "a"},
            { (char)0xE5, "a"},
            { (char)0xE6, "ae"},
            { (char)0xE7, "c"},
            { (char)0xE8, "e"},
            { (char)0xE9, "e"},
            { (char)0xEA, "e"},
            { (char)0xEB, "e"},
            { (char)0xEC, "i"},
            { (char)0xED, "i"},
            { (char)0xEE, "i"},
            { (char)0xEF, "i"},
            { (char)0xF0, "d"},
            { (char)0xF1, "n"},
            { (char)0xF2, "o"},
            { (char)0xF3, "o"},
            { (char)0xF4, "o"},
            { (char)0xF5, "o"},
            { (char)0xF6, "o"},
            { (char)0xF7, "/"},
            { (char)0xF8, "o"},
            { (char)0xF9, "u"},
            { (char)0xFA, "u"},
            { (char)0xFB, "u"},
            { (char)0xFC, "u"},
            { (char)0xFD, "y"},
            { (char)0xFE, "p"},
            { (char)0xFF, "y"}
        };
        public static string ToAsciiChars(this string input)
        {
            var writer = new StringWriter();
            if (input != null)
            {
                foreach (char c in input)
                {
                    if (_toAscii.TryGetValue(c, out string replace))
                    {
                        writer.Write(replace);
                    }
                    else
                    {
                        writer.Write(c);
                    }
                }
            }
            string output = writer.ToString();
            //output = Regex.Replace(output, @"[^\u0000-\u007F]+", string.Empty); //Catch any remaining non-Ascii
            output = Regex.Replace(output, @"[^a-zA-Z0-9\/\-\?:().,'+ \n]+", string.Empty); //Catch any non-Swift
            
            return output;
        }

    }
}
