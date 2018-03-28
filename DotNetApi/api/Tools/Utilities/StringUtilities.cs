using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Tools.Utilities
{
    public static class StringUtilities
    {
        public static string Left(this string s, int left)
        {
            return s.Substring(0, left);
        }

        public static string Right(this string s, int right)
        {
            return s.Substring(s.Length - right);
        }

        public static string EncodeForXml(string val)
        {
            if (val == null) return string.Empty;

            val = val.Replace("&", "&amp;");
            val = val.Replace("<", "&lt;");
            val = val.Replace(">", "&gt;");
            val = val.Replace("\"", "&quot;");
            val = val.Replace("'", "&apos;");

            return val;

        }

        public static string ToUiString(this int val)
        {
            return val.ToString(CultureInfo.CurrentCulture);
        }

        public static string ToUiString(this long val)
        {
            return val.ToString(CultureInfo.CurrentCulture);
        }

        public static string ManageTextLength(this string val, int maxLength)
        {
            var toRet = val;
            if (val.Length > maxLength)
            {
                toRet = toRet.Left(maxLength - 4) + " ...";
            }
            return toRet;
        }

        public static bool IsHtmlEncoded(string text)
        {
            return (HttpUtility.HtmlDecode(text) != text);
        }

        #region Array strings

        public static long CountEntries(string list, string delimiter)
        {
            list = list.Replace("-", " ");
            list = list.Replace("_", " ");
            int counter;
            long listLength = list.Length;
            long countEntries = 1;
            for (counter = 1; counter <= listLength; counter++)
            {
                if (list.Substring(counter, 1) != delimiter) continue;
                if (counter != listLength)
                {
                    countEntries = countEntries + 1;
                }
            }
            var toRet = countEntries;
            return toRet;
        }

        //public static string Entry(long entryPosition, string list, string delimiter = ",")
        //{
        //    string functionReturnValue;
        //    var returnEntry = "";

        //    if (entryPosition == 0) return "";

        //    var numEntries = CountEntries(list, delimiter);
        //    if (numEntries == 1)
        //    {
        //        functionReturnValue = list.Replace(delimiter, "");
        //        if (functionReturnValue == null)
        //            functionReturnValue = "";
        //        return functionReturnValue;
        //    }
        //    long lengthList = list.Length;
        //    numEntries = 1;
        //    long counter = 0;
        //    for (counter = 1; counter <= lengthList; counter++)
        //    {
        //        returnEntry = returnEntry + Strings.Mid(list, counter, 1);
        //        if (Strings.Mid(list, counter, 1) == delimiter | counter == lengthList)
        //        {
        //            if (counter != lengthList)
        //            {
        //                numEntries = numEntries + 1;
        //            }
        //            else
        //            {
        //                if (numEntries + 1 == entryPosition + 1)
        //                {
        //                    functionReturnValue = returnEntry.Replace(delimiter, "");
        //                    if ((functionReturnValue == null))
        //                        functionReturnValue = "";
        //                    return functionReturnValue;
        //                }

        //            }
        //            if (numEntries == entryPosition + 1)
        //            {
        //                functionReturnValue = returnEntry.Replace(delimiter, "");
        //                if ((functionReturnValue == null))
        //                    functionReturnValue = "";
        //                return functionReturnValue;
        //            }
        //            else
        //            {
        //                returnEntry = "";
        //            }
        //        }
        //    }
        //    return "";

        //    return functionReturnValue;
        //}

        //public static string SetEntry(string value, string listValues, string defaultValue, string delimiter)
        //{
        //    int li1;
        //    var lcEq = "=";
        //    var lcComa = ",";

        //    if ((delimiter.Trim().Length == 2))
        //    {
        //        lcEq = delimiter.Substring(0, 1);
        //        lcComa = delimiter.Substring(1, 1);
        //    }

        //    var li2 = CountEntries(listValues, lcComa);
        //    var llFound = false;

        //    for (li1 = 1; li1 <= li2; li1++)
        //    {
        //        var lcEntry = Entry(li1, listValues, lcComa).Trim();
        //        if ((lcEntry.StartsWith(value + " " + lcEq)) | (lcEntry.StartsWith(value + lcEq)))
        //        {
        //            SpecialSetEntry(2, ref lcEntry, defaultValue, lcEq);
        //            SpecialSetEntry(li1, ref listValues, lcEntry, lcComa);
        //            llFound = true;
        //            break; // TODO: might not be correct. Was : Exit For
        //        }
        //    }

        //    if (!llFound)
        //    { listValues = value + lcEq + defaultValue + lcComa + listValues; }

        //    return listValues;

        //}

        //public static void SpecialSetEntry(long tokenNumber, ref string str, object value, string delimiter = ",")
        //{
        //    int i;
        //    //Was empty before instead of nothing
        //    if (tokenNumber <= 0 || str.Length == 0) return;

        //    string[] laStr = str.Split(delimiter, -1, Constants.vbTextCompare);
        //    if (laStr.Length + 1 < tokenNumber) return;

        //    laStr[tokenNumber - 1] = value;
        //    var lcWork = "";
        //    for (i = 0; i <= laStr.Length; i++)
        //    { lcWork = lcWork + delimiter + laStr[i]; }

        //    lcWork = Strings.Mid(lcWork, delimiter.Length + 1);
        //    if (lcWork.Length > 0)
        //    { str = lcWork; }
        //}

        public static string Quotify(string list)
        {
            if (CheckEmpty.String(list) == "") return "''";
            var listArray = new List<string>(list.Split(','));
            for (var i = 0; i <= listArray.Count - 1; i++)
            {
                listArray[i] = "'" + listArray[i] + "'";
            }
            return string.Join(",", listArray);
        }

        //public static string LookUp(string value, string list, string defaultValue, string delimiter)
        //{
        //    int LI_1;
        //    int LI_2;
        //    string LC_ENTRY;
        //    string LC_RETURN;
        //    string LC_EQ;
        //    string LC_COMA;
        //    bool LL_FOUND = false;

        //    LC_EQ = "=";
        //    LC_COMA = ",";

        //    if ((Strings.Trim(delimiter).Length == 2))
        //    {
        //        LC_EQ = delimiter.Substring(0, 1);
        //        LC_COMA = delimiter.Substring(1, 1);
        //    }

        //    LI_2 = CountEntries(list, LC_COMA);
        //    LC_RETURN = defaultValue;

        //    for (LI_1 = 1; LI_1 <= LI_2; LI_1++)
        //    {
        //        LC_ENTRY = Strings.Trim(Entry(LI_1, list, LC_COMA));
        //        if ((LC_ENTRY.StartsWith(value + " " + LC_EQ, true, null)) | (LC_ENTRY.StartsWith(value + LC_EQ, true, null)))
        //        {
        //            LC_RETURN = Entry(2, LC_ENTRY, LC_EQ);
        //            LL_FOUND = true;
        //            break; // TODO: might not be correct. Was : Exit For
        //        }
        //    }

        //    return Strings.Trim(LC_RETURN);
        //}

        //public static long LookUp(string stringToSearch, string stringToSearchIn, string delimiter = ",")
        //{
        //    long toRet;

        //    if (stringToSearch.Length == 0 || stringToSearchIn.Length == 0) return 0;

        //    var lst1 = stringToSearchIn.Replace(" " + delimiter, delimiter);
        //    lst1 = lst1.Replace(delimiter + " ", delimiter);
        //    lst1 = delimiter + lst1 + delimiter;
        //    int i = Strings.InStr(1, lst1, delimiter + stringToSearch + delimiter, Constants.vbTextCompare);
        //    if (i == 0)
        //    {
        //        toRet = 0;
        //        return toRet;
        //    }
        //    if (i == 1)
        //    {
        //        toRet = 1;
        //        return toRet;
        //    }

        //    lst1 = lst1.Left(i - 1);
        //    dynamic lar2 = lst1.Split(delimiter, -1, Constants.vbTextCompare);
        //    toRet = lar2.Length + 1;
        //    return toRet;
        //}

        #endregion Array strings

        public static string[] Tokens(this string s, string seperator = " ")
        {
            return s.Split(new [] { seperator }, StringSplitOptions.None);
        }

        public static Enumerations.TokenType GetTokenType(this string val)
        {
            long toRetlong;
            if (long.TryParse(val, out toRetlong)) return Enumerations.TokenType.Long;

            double toRetDouble;
            if (double.TryParse(val, out toRetDouble)) return Enumerations.TokenType.Double;

            int toRetInt;
            if (int.TryParse(val, out toRetInt)) return Enumerations.TokenType.Integer;

            DateTime toRetDate;
            if (DateTime.TryParse(val, out toRetDate)) return Enumerations.TokenType.Date;

            return Enumerations.TokenType.String;
        }
    }
}