// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System.Data;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// The main purpose of this class is to provide data for Java's tests.
    /// This class will be moved into new project Aspose.Ms.Tests when it gets created.
    /// </summary>
    [TestFixture]
    public class TestDataColumn
    {
        [Test]
        public void TestDataColumnType()
        {
            DataColumn dc = new DataColumn("Column1");
            Assert.That(dc.DataType == typeof(string), Is.True);

            dc = new DataColumn("Column1", typeof(int));
            Assert.That(dc.DataType == typeof(int), Is.True);

            dc = new DataColumn("Column1", typeof(double));
            Assert.That(dc.DataType == typeof(double), Is.True);

            dc = new DataColumn("Column1", typeof(byte));
            Assert.That(dc.DataType == typeof(byte), Is.True);

            dc = new DataColumn("Column1", typeof(ushort));
            Assert.That(dc.DataType == typeof(ushort), Is.True);

            dc = new DataColumn("Column1", typeof(uint));
            Assert.That(dc.DataType == typeof(uint), Is.True);
        }

        [Test]
        [JavaThrows(true)]
        public void TestDataColumnTypeRowValue()
        {
            DataTable table = new DataTable("testTable");
            table.Columns.Add("PriceDefaultType");
            table.Columns.Add("PriceDoubleType", typeof(double));
            table.Columns.Add("PriceByteType", typeof(byte));
            table.Columns.Add("PriceUshortType", typeof(ushort));
            table.Columns.Add("PriceUintType", typeof(uint));

            table.Rows.Add("30000", 18000.0, 1, 1 , 1);
            table.Rows.Add("50000", 20000.0, 253, 0xFFF0, 0xFFFFFFF0);

            string resultString = string.Empty;
            double resultDouble = 0;
            byte resultByte = 0;
            ushort resultUshort = 0;
            uint resultUint = 0;
            foreach (DataRow dataRow in table.Rows)
            {
                resultString += (string)dataRow[0]; //no System.InvalidCastException 
                resultDouble += (double)dataRow[1]; //no System.InvalidCastException 
                resultByte += (byte)dataRow[2]; //no System.InvalidCastException 
                resultUshort += (ushort)dataRow[3]; //no System.InvalidCastException 
                resultUint += (uint)dataRow[4]; //no System.InvalidCastException 
            }

            Assert.That(resultString, Is.EqualTo("3000050000"));
            Assert.That(resultDouble, Is.EqualTo(38000));
            Assert.That(resultByte, Is.EqualTo(254));
            Assert.That(resultUshort, Is.EqualTo((ushort)65521));
            Assert.That(resultUint, Is.EqualTo((uint)4294967281));
        }
    }
}
