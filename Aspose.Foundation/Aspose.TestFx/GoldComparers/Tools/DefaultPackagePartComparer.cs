// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2016 by Ivan Lyagin

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Represents the default package part comparer used by <see cref="PackagePartComparisonInfo"/> while performing GOLD tests 
    /// when no custom package part comparer is provided.
    /// </summary>
    public class DefaultPackagePartComparer : IPackagePartComparer
    {
        private DefaultPackagePartComparer()
        {
            // Hide from using outside.
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // replaced with xdiff
        bool IPackagePartComparer.CompareBuffers(string partName, byte[] buffer1, byte[] buffer2, bool isPreferXmlDiff)
        {
            bool isXml = ComparisonInfo.IsXml(partName);
            if (isPreferXmlDiff && ComparisonInfo.IsXml(partName))
            {
                // This is a slow compare method and should only be used when comparing XML in OUT vs MS for display in the Test UI.
                return XmlComparer.CompareXmlBuffers(buffer1, buffer2);
            }
            else
            {
#if PLAIN_JAVA // XMLDIFF
                // On Java we compare XML using XmlDiff so indentation etc will pass on Java.
                // XmlComparer throws parse exception on empty files.
                try
                {
                    if (ComparisonInfo.isXml(partName) && buffer1.length > 4 && buffer2.length > 4)
                        return XmlComparer.compareXmlBuffers(buffer1, buffer2);
                }
                catch (Exception e)
                {
                    // This could happen if we are comparing XMLs which were encrypted. e.g. TestOdtEncryption.testEncryptSha256Aes()
                    // If XmlDiff fails to create XML documents from the buffers lets fall and do byte-by-byte comparision via TestUtil.compareBuffers()
                    System.err.println("XML content could be encrypted. So lets try and compare them byte by byte. Exception: " + e.getClass() + ". " + e.getMessage());
                }
#endif

                // This is the fast compare method that we use normally.
                return TestFxUtil.CompareBuffers(buffer1, buffer2);
            }
        }


        /// <summary>
        /// The single instance of the class.
        /// </summary>
        public static readonly IPackagePartComparer Instance = new DefaultPackagePartComparer();
    }
}
