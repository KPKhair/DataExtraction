namespace DataExtraction.Infrastructure.Helpers
{
    public static class ParserHelper
    {

        /// <summary>
        /// Extracts the value for a specific key from pipe separated string.
        /// </summary>
        /// <param name="inputString">The input string</param>
        /// <param name="key">The key to look for (e.g. "PriceMultiplier")</param>
        /// <param name="delimiter">Delimiters bank specific (e,g. "|;" or "|")</param>
        /// <param name="keyValueSeparator">Key value pair separator bank specific (e.g ":" or "=")</param>
        public static string? GetValueFromPipeSeparatedString(string inputString, string key, string delimiter, string keyValueSeparator)
        {
            // Define the delimiter for pairs and the key-value separator


            // Split the string into individual key-value pair strings
            // We use StringSplitOptions.RemoveEmptyEntries to handle the trailing "|".
            string[] pairs = inputString.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                // Find the index of the key-value separator
                int separatorIndex = pair.IndexOf(keyValueSeparator, StringComparison.Ordinal);

                if (separatorIndex > 0)
                {
                    // Extract the key part
                    string currentKey = pair.Substring(0, separatorIndex);

                    // If it matches the target key, extract the value part and return it
                    if (string.Equals(currentKey, key, StringComparison.OrdinalIgnoreCase))
                    {
                        string value = pair.Substring(separatorIndex + 1);
                        return value;
                    }
                }
            }

            return null;
        }


    }
}
