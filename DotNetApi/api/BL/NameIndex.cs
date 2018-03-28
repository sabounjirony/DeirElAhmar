using System;
using BL.Setup;
using Tools.Utilities;

namespace BL
{
    [BlAspect(AspectPriority = 2)]
    public static class NameIndex
    {
        public static string GetNameIndex(long userId, string first, string last, string middle = "", int year = 0)
        {
            string toRet = null;
            var tokens = new[] { first, middle, last };
            var tokenKeys = new[] { "|&F:", "|&M:", "|&L:" };

            var j = 0;
            foreach (var token in tokens)
            {
                var itemRet = "";
                if (CheckEmpty.String(token) != "")
                {
                    var arToken = token.Split(new[] { " " }, StringSplitOptions.None);
                    for (var i = 0; i < arToken.Length; i++)
                    {
                        arToken[i] = FilterString(userId, arToken[i]);
                        itemRet += tokenKeys[j] + arToken[i];
                    }
                    toRet += itemRet;
                }
                j += 1;
            }

            if (CheckEmpty.Numeric(year) != 0) toRet += "|&Y:" + year;

            return toRet ?? "";
        }

        public static string[] GetNameIndexForQuery(long userId, string first, string last, string middle = "", int year = 0)
        {
            var wi = GetNameIndex(userId, first, last, middle, year);
            var toRet = wi.Replace("|&F:", "|").Replace("|&M:", "|").Replace("|&L:", "|").Split(new[] { "|" }, StringSplitOptions.None);
            return toRet;
        }

        #region Private methods

        private static string FilterString(long userId, string value)
        {

            //Replace noise characters with empty
            var noiseCharacter = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "{", "}", "|", "[", "]", "\\", ":", ";", "'", "<", ">", "?", ",", ".", "-" };
            for (var i = 0; i < noiseCharacter.Length; i++)
            {
                var noise = noiseCharacter[i];
                value = value.Replace(noise, "");
            }

            var noiseStrings = new[] { "/", " AND ", " OR ", " ET ", " OU ", "-", "_" };
            for (var i = 0; i < noiseStrings.Length; i++)
            {
                var noise = noiseStrings[i];
                value = value.Replace(noise, " ");
            }

            var originalValue = new[] { "Y", "EE", "K", "SCH", "SH", "PH" };
            var substituteValue = new[] { "I", "I", "C", "CH", "CH", "F" };
            for (var i = 0; i < originalValue.Length; i++)
            {
                value = value.Replace(originalValue[i], substituteValue[i]);
            }

            return GetReplacement(userId, value);
        }

        private static string GetReplacement(long userId, string token)
        {
            if (token.Length < 3) return "";
            var antiDict = BlAntiDict.GetAntiDict(userId, token);
            return CheckEmpty.String(antiDict) == "" ? token : antiDict;
        }

        #endregion Methods
    }
}