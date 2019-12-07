using System;

namespace RoomFinishes.Extensions
{
    public static class StringExtensions
    {
        public static string NthFromSplit(this string input, string delimiter, int nth)
        {
            if (nth < 1) throw new ArgumentException("Nth must start with 1");
            var split = input.Split(new[] {delimiter}, StringSplitOptions.None);
            return split.Length < nth ? "" : split[nth - 1];
        }
    }
}