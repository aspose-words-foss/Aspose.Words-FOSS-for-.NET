// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System.Text;
using Aspose.Collections;
using Aspose.Common;

namespace Aspose.TestFx.GoldComparers
{
    public class ComparerParams
    {
        public StringToStringDictionary ToParamMap()
        {
            StringToStringDictionary map = new StringToStringDictionary();
            map.Add("Title", Title);
            map.Add("FileNameSrc", FileNameSrc);
            map.Add("FileNameOut", FileNameOut);
            map.Add("FileNameGold", FileNameGold);
            map.Add("FileNameOriginalGold", FileNameOriginalGold);
            map.Add("FileNameMs", FileNameMs);

            map.Add("PagesCount", PagesCount.ToString());
            map.Add("Pages", Join(Pages));
            map.Add("ComplianceOption", ComplianceOption);
            map.Add("UserPassword", UserPassword);
            map.Add("ErrorMessage", ErrorMessage);
            return map;
        }

        private static string Join(int[] arr)
        {
            if (arr == null || arr.Length == 0)
                return string.Empty;

            StringBuilder str = new StringBuilder();
            foreach (int i in arr)
                str.Append(i).Append(",");

            return str.Remove(str.Length - 1, 1).ToString();
        }

        public string Title { get; set; }
        public string FileNameSrc { get; set; }
        public string FileNameOut { get; set; }
        public string FileNameGold { get; set; }
        public string FileNameOriginalGold { get; set; }
        public string FileNameMs { get; set; }

        public string ErrorMessage { get; set; }

        public int PagesCount { get; set; }
        public int[] Pages { get; set; }
        public string ComplianceOption { get; set; }
        public string UserPassword { get; set; }
        public bool DialogTopMost { get; set; }
        public bool DialogRestorePosition { get; set; }
    }
}
