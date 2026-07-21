// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2013 by Victor Chebotok

using System.Text;
using Aspose.Collections;
using Aspose.Common;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Utility class that converts XML numeric and named character references into corresponding Unicode characters.
    /// </summary>
    internal static class XmlEntities
    {
        static XmlEntities()
        {
            gNamedEntities = new StringToCharDictionary();
            gNamedEntities.Add("quot", '\u0022');
            gNamedEntities.Add("amp", '\u0026');
            gNamedEntities.Add("apos", '\u0027');
            gNamedEntities.Add("lt", '\u003C');
            gNamedEntities.Add("gt", '\u003E');
        }

        internal static string Expand(string s)
        {
            if (s == null)
            {
                return null;
            }

            if (s.Length == 0)
            {
                return s;
            }

            StringBuilder sb = new StringBuilder(s.Length);
            bool inEntity = false;
            StringBuilder entity = new StringBuilder(10);

            for (int i = 0; i < s.Length; i++)
            {
                if (!inEntity)
                {
                    switch (s[i])
                    {
                        case '&':
                            inEntity = true;
                            break;
                        default:
                            sb.Append(s[i]);
                            break;
                    }
                }
                else
                {
                    switch (s[i])
                    {
                        case ';':
                            if (entity.Length == 0)
                            {
                                sb.Append("&;");
                            }
                            else
                            {
                                if (entity[0] == '#')
                                {
                                    // It must be a numeric character reference.
                                    try
                                    {
                                        if ((entity[1] == 'x') || (entity[1] == 'X'))
                                        {
                                            // It must be a hex number.
                                            string hex = entity.ToString(2, entity.Length - 2);
                                            int code = FormatterPal.ParseHex(hex);
                                            sb.Append((char)code);
                                        }
                                        else
                                        {
                                            // It must be a decimal number.
                                            string dec = entity.ToString(1, entity.Length - 1);
                                            int code = FormatterPal.ParseInt(dec);
                                            sb.Append((char)code);
                                        }
                                    }
                                    catch
                                    {
                                        // This is executed in number cannot be parsed, just leave as is.
                                        sb.Append('&').Append(entity).Append(';');
                                    }
                                }
                                else
                                {
                                    // named entity?
                                    char entityChar = gNamedEntities[entity.ToString()];
                                    if (StringToCharDictionary.IsNullSubstitute(entityChar))
                                    {
                                        // nope
                                        sb.Append('&').Append(entity).Append(';');
                                    }
                                    else
                                    {
                                        // we found one
                                        sb.Append(entityChar);
                                    }
                                }
                                entity.Remove(0, entity.Length);
                            }
                            inEntity = false;
                            break;

                        case '&':
                            // new entity start without end, it was not an entity...
                            sb.Append('&').Append(entity);
                            entity.Length = 0;
                            break;

                        default:
                            entity.Append(s[i]);
                            const int maxEntitySize = 8 + 1; // we add the # char
                            if (entity.Length > maxEntitySize)
                            {
                                // unknown stuff, just don't touch it
                                inEntity = false;
                                sb.Append('&').Append(entity);
                                entity.Length = 0;
                            }
                            break;
                    }
                }
            }

            // finish the work
            if (inEntity)
            {
                sb.Append('&').Append(entity);
            }

            return sb.ToString();
        }

        private static readonly StringToCharDictionary gNamedEntities;
    }
}
