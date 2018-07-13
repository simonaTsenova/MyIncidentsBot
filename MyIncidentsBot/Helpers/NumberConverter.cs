using System;

namespace MyIncidentsBot.Helpers
{
    public static class NumberConverter
    {
        public static int ConvertWordToNumber(string numberInWords)
        {
            string[] words = numberInWords.ToLower().Split(new char[] { ' ', '-', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var numberWords = new
            {
                ones = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" },
                teens = new[] { "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" },
                tens = new[] { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" }
            };

            var result = 0;
            foreach (var word in words)
            {
                var n = 0;
                if ((n = Array.IndexOf(numberWords.ones, word) + 1) > 0)
                {
                    result += n;
                }
                else if ((n = Array.IndexOf(numberWords.teens, word) + 1) > 0)
                {
                    result += n + 10;
                }
                else if ((n = Array.IndexOf(numberWords.tens, word) + 1) > 0)
                {
                    result += n * 10;
                }
                else if (word == "hundred" && result == 1)
                {
                    result *= 100;
                }
                else if (word != "and")
                {
                    throw new ArgumentException("Word must be valid number in the range 1-100.");
                }
            }

            return result;
        }
    }
}