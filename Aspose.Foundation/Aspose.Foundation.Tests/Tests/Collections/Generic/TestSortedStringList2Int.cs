// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2018 by Alexey Noskov

using System.Text;
using Aspose.Collections.Generic;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestSortedStringList2Int
    {
        [Test]
        public void TestDefaultSorting()
        {
            mList = new SortedStringListGeneric<int>();
            AddAll(mList);
            //source key-values stored in default case-sensitive order.
            CheckSorting(mValues);
        }

        [Test]
        public void TestDefaultSortingCustomValues()
        {
            mList = new SortedStringListGeneric<int>();
            AddAllCustom(mList);
            CheckSorting(mCustomSensitive);
        }

        [Test]
        public void TestInsensitiveSorting()
        {
            mList = new SortedStringListGeneric<int>(false);
            //we have here duplicate keys from case-insensitive point of view - so using set instead of add.
            SetAll(mList);
            CheckSorting(mInSensitiveValues);
        }

        [Test]
        public void TestInsensitiveSortingCustomValues()
        {
            mList = new SortedStringListGeneric<int>(false);
            AddAllCustom(mList);
            CheckSorting(mCustomSensitive);
        }

        private void AddAll(SortedStringListGeneric<int> sl)
        {
            for (int i = 0; i < mKeys.Length; i++)
                sl.Add(mKeys[i], mValues[i]);
        }

        private void AddAllCustom(SortedStringListGeneric<int> sl)
        {
            for (int i = 0; i < mCustomKeys.Length; i++)
                sl.Add(mCustomKeys[i], mCustomValues[i]);
        }

        //added for insensitive comparers since add() throws on doubles.
        private void SetAll(SortedStringListGeneric<int> sl)
        {
            for (int i = 0; i < mKeys.Length; i++)
                sl[mKeys[i]] = mValues[i];
        }

        private int[] GetArrayOfValues()
        {
            int[] values = new int[mList.Count];
            for (int i = 0; i < mList.Count; i++)
                values[i] = (int)mList.GetByIndex(i);
            return values;
        }

        private void CheckSorting(int[] expectedValues)
        {
            int[] sortedValues = GetArrayOfValues();
            if (!ArrayUtil.IsArrayEqual(sortedValues, expectedValues))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\r\nsorted\t: expected\r\n");
                sb.Append("----------:---------\r\n");
                for (int i = 0; i < expectedValues.Length; i++)
                    sb.AppendFormat("{0} -> {1} : {2}\r\n", mList.GetKey(i), sortedValues[i], expectedValues[i]);

                Assert.Fail(sb.ToString());
            }
        }

        private SortedStringListGeneric<int> mList;
        private string[] mKeys = { "/ABC", "/Abc", "0abc", "0абв", "ABC", "Abc", "Bcd", "\\ABC", "\\Abc", "_abc", "_абв", "abc", "abc0", "abc_", "bcd", "bcd0", "АБВ", "Абв", "БВГ", "Бвг", "абв", "бвг" };
        private int[] mValues = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

        //case-insensitive values: doubles (from case-insensitive point of view) are removed by sorted list.
        private int[] mInSensitiveValues = { 1, 2, 3, 11, 12, 13, 14, 15, 8, 9, 10, 20, 21 };
        private string[] mCustomKeys = { "DN_Bank", "DN_BankKontoNr", "DN_BankRegNr", "DN_By", "DN_CVR", "DN_DNKundeKontaktPerson", "DN_DokumentNummer",
                                   "DN_EAN", "DN_KundeAdresse", "DN_KundeNavn", "DN_OprettetAfPrimærEmail", "DN_Version", "DNKontraktDato",
                                   "DNKundeKontaktMail", "DNSælger", "INKundeKontaktTlf", "INSælger", "www" };
        private int[] mCustomValues = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

        //default(case-sensitive) sorting of custom values
        private int[] mCustomSensitive = { 12, 13, 14, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 15, 16, 17 };
    }
}
