// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2013 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.TestFx
{
    [CppDeclareFriendClass("Aspose.TestFx.Pal.TestUtilPal")]
    public class TestSetting
    {
        internal static TestSetting Load(int index, string key, string value, params string[] attached)
        {
            // This happens when we try to set value for setting which was not loaded by default or read from configuration.
            // We want index for the TestSetting UI. If we do not provide it then settings will appear at random on the form.
            Debug.Assert(index >= 0);

            TestSetting result = new TestSetting(index, key, attached);

            Debug.Assert(result.Kind == KindEnum.Undefined);
            bool b;
            float f;
            result.Values = TokenizeListQuoteless(value);
            if (result.Values.Count > 1)
                result.Kind = KindEnum.Array;
            else if (TryParseBool(result.Values[0], out b))
                result.Kind = KindEnum.Bool;
            else if (TryParseFloat(result.Values[0], out f))
                result.Kind = KindEnum.Float;
            else
                result.Kind = KindEnum.String;
            return result;
        }

        internal static TestSetting Create(int index, string key, object value, params string[] attached)
        {
            Debug.Assert(index >= 0);

            TestSetting result = new TestSetting(index, key, attached);

            if (value is bool)
            {
                result.Kind = KindEnum.Bool;
                result.Values = new List<string> { (bool)value ? YES :  NO };
            }
            else if (value is string)
            {
                result.Kind = KindEnum.String;
                result.Values = new List<string> { value.ToString() };
            }
            else if (value is float)
            {
                result.Kind = KindEnum.Float;
                result.Values = new List<string> { value.ToString() };
            }
            else if (value is string[])
            {
                result.Kind = KindEnum.Array;
                result.Values = new List<string>((string[])value);
            }
            else
                throw new ArgumentException("value");

            return result;
        }

        [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
        internal enum KindEnum
        {
            Undefined,
            Bool,
            Float,
            String,
            Array,
        }

        internal KindEnum Kind { get; private set; }

        internal int Index {get; private set; }

        internal string Key {get; private set; }

        internal string[] Attached {get; private set; }

        internal bool AsBool
        {
            get
            {
                CheckThrowIfWrongKind(KindEnum.Bool);
                return ToBool(Values[0]);
            }
        }

        internal float AsFloat
        {
            get
            {
                CheckThrowIfWrongKind(KindEnum.Float);
                return float.Parse(Values[0]);
            }
        }

        internal string AsString
        {
            get
            {
                CheckThrowIfWrongKind(KindEnum.String);
                return Values[0].Replace("\\,", ",");
            }
        }

        internal string[] AsArray
        {
            get
            {
                CheckThrowIfWrongKind(KindEnum.Array);
                return Values.ToArray();
            }
        }

        internal bool Leader
        {
            get
            {
                // if any attached line is empty, then new group is started
                foreach (var a in Attached)
                    if (string.IsNullOrWhiteSpace(a))
                        return true;

                return false;
            }
        }

        internal string Description
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                // last attached line is a comment with description
                return  Attached.Length > 0 && Attached[Attached.Length-1].Length > 0
                    ? Attached[Attached.Length-1].Substring(1) : null;
            }
        }

        /// <summary>
        /// Updates value. Takes <paramref name="value"/> of the actual type.
        /// </summary>
        internal void UpdateValue(object value)
        {
            Debug.Assert(Kind != KindEnum.Undefined);

            // this happens when called from constructor
            bool init = Values == null;
            if (init)
                Values = new List<string>() { "" };

            switch (Kind)
            {
                case KindEnum.Bool:
                    Debug.Assert(value is bool);
                    Values[0] = (bool)value ? TestSetting.YES : TestSetting.NO;
                    break;
                case KindEnum.String:
                    Debug.Assert(value is string);
                    Values[0] = (string)value;
                    break;
                case KindEnum.Float:
                    Debug.Assert(value is float || value is int);
                    Values[0] = value.ToString();
                    break;
                case KindEnum.Array:
                    string[] input = (string[])value;
                    if (init)
                    {
                        Values = new List<string>(input);
                    }
                    else
                    {
                        int index = Values.IndexOf(input[0]);
                        Debug.Assert(index >= 0);
                        // the first item is the selected value
                        if (index > 0)
                        {
                            string temp = Values[0];
                            Values[0] = Values[index];
                            Values[index] = temp;
                        }
                        // extend values if passed contains new ones
                        // if passed has fewer values do not change existing ones
                        HashSetGeneric<string> existing = new HashSetGeneric<string>(Values);
                        foreach (var str in input)
                            if (!existing.Contains(str))
                                existing.Add(str);
                    }
                    break;
            }
        }

        /// <summary>
        /// This is used to serialize value into configuration file and build UI form.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal string GetSerializedValue()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // this implementation escapes ',' in the values, so that TokenizeListQuoteless can parse it back
            bool first = true;
            foreach (string v in Values)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append(v.Replace(",", "\\,"));
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}={1} ({2})", Key, GetSerializedValue(), Description);
        }

        /// <summary>
        /// Converts string of comma separated values into list of values.
        /// This version allows untrimmed values and escapes quotes.
        /// </summary>
        private static List<string> TokenizeListQuoted(string value)
        {
            if (value == null)
                value = string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<string> result = new List<string>();
            bool insideString = false;
            char last = ' ';
            foreach (var c in value.ToCharArray())
            {
                if (insideString)
                {
                    if (c == '"')
                    {
                        if (last == '\\')
                        {
                            sb[sb.Length-1] = c;
                        }
                        else
                        {
                            insideString = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (char.IsWhiteSpace(c))
                    {
                        // ignore
                    }
                    else if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb = new System.Text.StringBuilder();
                    }
                    else if (c == '"')
                    {
                        if (last == '\\')
                        {
                            sb[sb.Length-1] = '"';
                        }
                        else
                        {
                            insideString = true;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                last = c;
            }
            result.Add(sb.ToString());

            return result;
        }

        /// <summary>
        /// Converts string of comma separated values into list of values.
        /// This version trims values and escapes comma.
        /// </summary>
        private static List<string> TokenizeListQuoteless(string value)
        {
            if (value == null)
                value = string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<string> result = new List<string>();
            char last = ' ';
            foreach (var c in value.ToCharArray())
            {
                if (c == ',')
                {
                    if (last == '\\')
                    {
                        sb[sb.Length-1] = c;
                    }
                    else
                    {
                        result.Add(sb.ToString().Trim());
                        sb = new System.Text.StringBuilder();
                    }
                }
                else
                {
                    sb.Append(c);
                }
                last = c;
            }
            result.Add(sb.ToString().Trim());

            return result;
        }

        private static bool ToBool(string value)
        {
            bool result;
            if (!TryParseBool(value, out result))
                throw new InvalidOperationException("Cannot convert '" + value + "' to bool.");
            return result;
        }

        /// <summary>
        /// Converts "y", "yes", "true" and "1" and "n", "no", "false" and "0" to boolean.
        /// </summary>
        private static bool TryParseBool(string value, out bool result)
        {
            switch (value.ToLower())
            {
                case "yes":
                case "y":
                case "1":
                case "t":
                case "true":
                    result = true;
                    return true;
                case "no":
                case "n":
                case "f":
                case "false":
                case "0":
                    result = false;
                    return true;
                default:
                    result = false;
                    return false;
            }
        }

        /// <summary>
        /// Converts string to float.
        /// </summary>
        private static bool TryParseFloat(string value, out float result)
        {
            result = FormatterPal.TryParseFloatInvariant(value);
            return (!float.IsNaN(result));
        }

        [JavaThrows(true)]
        private void CheckThrowIfWrongKind(params KindEnum[] kinds)
        {
            foreach (var kind in kinds)
                if (Kind == kind)
                    return;
            throw new Exception("expected '" + Kind+ "' value");
        }

        private TestSetting(int index, string key, params string[] attached)
        {
            Index = index;
            Key = key;
            Attached = new string[attached.Length];
            attached.CopyTo(Attached, 0);
        }

        private List<string> Values;
        internal const string YES = "yes";
        internal const string NO = "no";
    }
}
