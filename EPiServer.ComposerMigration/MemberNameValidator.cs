#region Copyright (C) 2013 EPiServer AB
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class MemberNameValidator
    {
        private static readonly Dictionary<char, string> CharVisualReplacements = new Dictionary<char, string> { { 'Ð', "D" }, { 'ð', "d" }, { 'Ł', "L" }, { 'ł', "l" }, { 'ß', "sz" }, { 'Ø', "O" }, { 'ø', "o" }, { 'Æ', "AE" }, { 'æ', "ae" }, { 'Œ', "OE" }, { 'œ', "oe" } }; 
        private static readonly string[] UnitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] TensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public virtual string CreateIdentifier(string name)
        {
            var newName = new StringBuilder();
            bool uppercaseNext = true;
            var initialNumber = new Queue<char>();

            name = Normalize(name);

            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];

                var cat = char.GetUnicodeCategory(c);

                if (cat != UnicodeCategory.DecimalDigitNumber)
                {
                    ClearNumberQueue(initialNumber, newName);
                }

                switch (cat)
                {
                    case UnicodeCategory.UppercaseLetter:
                        newName.Append(c);
                        uppercaseNext = false;
                        break;
                    case UnicodeCategory.LowercaseLetter:
                        if (uppercaseNext)
                        {
                            c = char.ToUpperInvariant(c);
                            uppercaseNext = false;
                        }
                        newName.Append(c);
                        break;
                    case UnicodeCategory.DecimalDigitNumber:
                        if (newName.Length > 0)
                        {
                            newName.Append(c);
                        }
                        else
                        {
                            initialNumber.Enqueue(c);
                        }
                        uppercaseNext = true;
                        break;
                    case UnicodeCategory.ConnectorPunctuation:
                        if (c == '_')
                        {
                            newName.Append(c);
                        }
                        break;
                    case UnicodeCategory.NonSpacingMark:
                        break;
                    default:
                        uppercaseNext = true;
                        break;
                }
            }

            return newName.ToString();
        }

        private static string Normalize(string name)
        {
            name = name.Normalize(NormalizationForm.FormD);

            var normalized = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                string replacement;
                if (CharVisualReplacements.TryGetValue(c, out replacement))
                {
                    normalized.Append(replacement);
                }
                else
                {
                    normalized.Append(c);
                }
            }

            return normalized.ToString();
        }

        private void ClearNumberQueue(Queue<char> queue, StringBuilder output)
        {
            if (queue.Count == 0)
            {
                return;
            }

            int number;
            if (int.TryParse(new string(queue.ToArray()), out number))
            {
                output.Append(NumberToWords(number));
            }
            queue.Clear();
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + "Million";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + "Thousand";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + "Hundred";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and";

                if (number < 20)
                    words += UnitsMap[number];
                else
                {
                    words += TensMap[number / 10];
                    if ((number % 10) > 0)
                        words += UnitsMap[number % 10];
                }
            }

            return words;
        }
    }
}
