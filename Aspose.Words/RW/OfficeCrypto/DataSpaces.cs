// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Ss;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// This class allows to work with structured storage documents organized according to the Data Spaces
    /// format as specified in the [MS-OFFCRYPTO]: Office Document Cryptography Structure Specification.
    /// </summary>
    internal class DataSpaces
    {
        /// <summary>
        /// Creates a data spaces manager from an existing structured storage document.
        /// </summary>
        /// <param name="fs">The structured storage document must be a valid data spaces document.</param>
        /// <exception cref="InvalidOperationException">If some data is unexpected or invalid.</exception>
        internal DataSpaces(FileSystem fs)
        {
            Debug.Assert(fs != null);

            mFs = fs;
            mDataSpaces = fs.Root.GetStorageSafe("\x0006DataSpaces");

            if(mDataSpaces != null)
            {
                VerifyDataSpacesVersion();
                ReadDataSpaceMap();
            }
        }

        internal bool IsEncryptedEcma376()
        {
            if (mDataSpaces == null)
                return mFs.ContainsInRootStorage("EncryptedPackage") && mFs.ContainsInRootStorage("EncryptionInfo");

            // Verify data space map.
            if (mDataSpaceMap.Count != 1)
                return false;

            DataSpaceMapEntry entry = mDataSpaceMap[0];
            if (entry.DataSpaceName != "StrongEncryptionDataSpace")
                return false;

            if (entry.ReferenceComponents.Count != 1)
                return false;

            if (entry.ReferenceComponents[0] != "EncryptedPackage")
                return false;

            // I think this is enough to detect the ECMA-376 encryption.
            return true;
        }

        /// <summary>
        /// Writes "DataSpaces" data.
        /// </summary>
        /// <remarks>
        /// AM. These streams are subset of MS IRMDS and always the same for encryption purpose.
        /// I decided to get them from encrypted DOCX file produced by Word2007.
        /// </remarks>
        private static void InitDataSpaces(FileSystem fs)
        {
            MemoryStorage dataSpaceInfo = new MemoryStorage();
            dataSpaceInfo.Add("StrongEncryptionDataSpace", new MemoryStream(gStrongEncryptionDataSpace));

            MemoryStorage strongEncryptionTransform = new MemoryStorage();
            strongEncryptionTransform.Add("\x0006Primary", new MemoryStream(g0006Primary));
            MemoryStorage transformInfo = new MemoryStorage();
            transformInfo.Add("StrongEncryptionTransform", strongEncryptionTransform);

            MemoryStorage dataSpaces = new MemoryStorage();

            dataSpaces.Add("DataSpaceInfo", dataSpaceInfo);
            dataSpaces.Add("DataSpaceMap", new MemoryStream(gDataSpaceMap));
            dataSpaces.Add("Version", new MemoryStream(gVersion));
            dataSpaces.Add("TransformInfo", transformInfo);

            fs.Root.Add("\x0006DataSpaces", dataSpaces);
        }

        /// <summary>
        /// Encrypt stream with given password.
        /// </summary>
        internal static FileSystem Encrypt(Stream stream, string password)
        {
            FileSystem fs = new FileSystem(new Guid("00000000-0000-0000-0000-000000000000"));
            InitDataSpaces(fs);

            IEcma376Encryption encryption = new Ecma376StandardEncryption();

            MemoryStream encryptionInfo = new MemoryStream();
            MemoryStream encryptedPackage = new MemoryStream();
            encryption.Encrypt(encryptionInfo, stream, encryptedPackage, password);

            fs.Root.Add("EncryptionInfo", encryptionInfo);
            fs.Root.Add("EncryptedPackage", encryptedPackage);
            return fs;
        }

        /// <summary>
        /// Should be called when the document is ECMA-376 or Agile encrypted.
        /// At the moment supports only the Standard Encryption method.
        /// </summary>
        /// <exception cref="InvalidOperationException">If some data is unexpected or invalid.</exception>
        /// <exception cref="IncorrectPasswordException">If the password is incorrect.</exception>
        internal MemoryStream Decrypt(string password)
        {
            if(mDataSpaces != null)
                VerifyEcma376EncryptionTransform();

            Stream encryptionInfo = mFs.Root.FetchStream("EncryptionInfo");
            Stream encryptedPackage = mFs.Root.FetchStream("EncryptedPackage");

            IEcma376Encryption encryption = GetEncryption(encryptionInfo);

            MemoryStream decryptedData = new MemoryStream();
            encryption.Decrypt(encryptionInfo, encryptedPackage, decryptedData, password);
            return decryptedData;
        }

        /// <summary>
        /// Reads EncryptionVersionInfo and returns appropriate IEcma376Encryption class.
        /// </summary>
        private static IEcma376Encryption GetEncryption(Stream encryptionInfo)
        {
            BinaryReader reader = new BinaryReader(encryptionInfo);
            EncryptionVersionInfo versionInfo = (EncryptionVersionInfo)reader.ReadInt32();

            IEcma376Encryption encryption;
            switch (versionInfo)
            {
                case EncryptionVersionInfo.Standard:
                case EncryptionVersionInfo.Standard2010:
                    encryption = new Ecma376StandardEncryption();
                    break;

                case EncryptionVersionInfo.Extensible:
                case EncryptionVersionInfo.Extensible2010:
                    throw new UnsupportedFileFormatException(
                        "The document is encrypted using the Extensible Encryption and this is not currently supported.");

                case EncryptionVersionInfo.Agile:
                    encryption = new Ecma376AgileEncryption();
                    break;

                default:
                    throw new InvalidOperationException("Unexpected encryption version.");
            }

            encryptionInfo.Position = 0;
            return encryption;
        }

        private void VerifyEcma376EncryptionTransform()
        {
            // Verify data space info.
            string[] transformReferences = ReadTransformReferences("StrongEncryptionDataSpace");
            if ((transformReferences.Length != 1) || (transformReferences[0] != "StrongEncryptionTransform"))
                throw new InvalidOperationException("Unexpected transform references.");

            MemoryStorage transformInfo = mDataSpaces.FetchStorage("TransformInfo");
            MemoryStorage transformStorage = transformInfo.FetchStorage(transformReferences[0]);

            MemoryStream primaryStream = transformStorage.GetStreamZeroPositioned("\x0006Primary");
            if(primaryStream == null)
                primaryStream = transformStorage.FetchStream("Primary");

            BinaryReader reader = new BinaryReader(primaryStream);

            // Verify IRMDSTransformInfo
            reader.ReadInt32(); // int transformLength =

            int transformType = reader.ReadInt32();
            if (transformType != 1)
                throw new InvalidOperationException("Unexpected transform type.");

            // WORDSNET-22837 Seems Word does not verify rest (ID, name and reader/updater/writer versions).
            ReadUnicodeLPP4(reader);    // "{FF9A3F03-56EF-4613-BDD5-5A41C1D07246}"
            ReadUnicodeLPP4(reader);    // "Microsoft.Container.EncryptionTransform"

            if(StreamUtil.HasEnoughBytesToRead(reader, 4 * 3 /* 3 * Int32 */))
                VerifyReaderUpdaterWriter(reader);

            // 2.2.6 IRMDSTransformInfo Structure specification states that the transform info should contain
            // a 32-bit integer length followed by a UTF-8-LP-P4 structure with XML license.
            // But it looks like something is wrong.
            // I can read a string from here such as "AES 128" using readUtf8LPP4, and then a few more bytes,
            // but does not look like what the spec says. So let's ignore completely.

            // EncryptionTransformInfo is stored here, but from the spec it seems to have no authority.
        }

        /// <summary>
        /// Reads and returns transforms that are specified for the given data space definition.
        /// </summary>
        private string[] ReadTransformReferences(string dataSpaceName)
        {
            MemoryStorage dataSpaceInfo = mDataSpaces.FetchStorage("DataSpaceInfo");
            BinaryReader reader = new BinaryReader(dataSpaceInfo.FetchStream(dataSpaceName));

            int headerLength = reader.ReadInt32();
            if (headerLength != 8)
                throw new InvalidOperationException("Unexpected header length.");

            int transformCount = reader.ReadInt32();
            string[] result = new string[transformCount];
            for (int i = 0; i < transformCount; i++)
                result[i] = ReadUnicodeLPP4(reader);

            return result;
        }

        private void VerifyDataSpacesVersion()
        {
            // WORDSNET-22837 Seems Word does not check Version stream.
            if (mDataSpaces.ContainsKey(VersionStreamName))
            {
                BinaryReader reader = new BinaryReader(mDataSpaces.FetchStream(VersionStreamName));

                if (ReadUnicodeLPP4(reader) != "Microsoft.Container.DataSpaces")
                    throw new InvalidOperationException("Unexpected FeatureIdentifier.");

                VerifyReaderUpdaterWriter(reader);
            }
        }

        private static void VerifyReaderUpdaterWriter(BinaryReader reader)
        {
            int readerVersion = reader.ReadInt32();
            if (readerVersion != 0x00000001)
                throw new InvalidOperationException("Unexpected reader version.");

            int updatedVersion = reader.ReadInt32();
            if (updatedVersion != 0x00000001)
                throw new InvalidOperationException("Unexpected updated version.");

            int writerVersion = reader.ReadInt32();
            if (writerVersion != 0x00000001)
                throw new InvalidOperationException("Unexpected writer version.");
        }

        private static string ReadUnicodeLPP4(BinaryReader reader)
        {
            int lengthBytes = reader.ReadInt32();
            byte[] data = reader.ReadBytes(lengthBytes);
            StreamUtil.SeekToNextPage(reader.BaseStream, 4);
            return Encoding.Unicode.GetString(data);
        }

        /// <summary>
        /// Reads the stream that associates protected content with the data space definition used to transform it.
        /// </summary>
        private void ReadDataSpaceMap()
        {
            BinaryReader reader = new BinaryReader(mDataSpaces.FetchStream("DataSpaceMap"));

            int headerLength = reader.ReadInt32();
            if (headerLength != 0x00000008)
                throw new InvalidOperationException("Unexpected header length.");

            mDataSpaceMap = new List<DataSpaceMapEntry>();

            int entryCount = reader.ReadInt32();
            for (int i = 0; i < entryCount; i++)
            {
                DataSpaceMapEntry entry = ReadDataSpaceMapEntry(reader);
                mDataSpaceMap.Add(entry);
            }
        }

        private static DataSpaceMapEntry ReadDataSpaceMapEntry(BinaryReader reader)
        {
            DataSpaceMapEntry entry = new DataSpaceMapEntry();

            reader.ReadInt32(); // int entryLength =
            int componentCount = reader.ReadInt32();
            for (int i = 0; i < componentCount; i++)
            {
                reader.ReadInt32(); // Specifies whether stream or storage, I don't need this. // int componentType =
                string component = ReadUnicodeLPP4(reader);
                entry.ReferenceComponents.Add(component);
            }

            entry.DataSpaceName = ReadUnicodeLPP4(reader);

            return entry;
        }

        private readonly FileSystem mFs;
        private readonly MemoryStorage mDataSpaces;
        /// <summary>
        /// Items are <see cref="DataSpaceMapEntry"/>.
        /// </summary>
        private List<DataSpaceMapEntry> mDataSpaceMap;

        private const string VersionStreamName = "Version";

        #region "DataSpaces" content.
        /// <summary>
        /// See [MS-OFFCRYPTO] 2.3.4 ECMA-376 Document Encryption.
        /// </summary>
        private static readonly byte[] gVersion = new byte[]
        {
            0x3C, 0x00, 0x00, 0x00, 0x4D, 0x00, 0x69, 0x00, 0x63, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x73, 0x00, 0x6F, 0x00,
            0x66, 0x00, 0x74, 0x00, 0x2E, 0x00, 0x43, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x61, 0x00, 0x69, 0x00,
            0x6E, 0x00, 0x65, 0x00, 0x72, 0x00, 0x2E, 0x00, 0x44, 0x00, 0x61, 0x00, 0x74, 0x00, 0x61, 0x00, 0x53, 0x00,
            0x70, 0x00, 0x61, 0x00, 0x63, 0x00, 0x65, 0x00, 0x73, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00
        };

        /// <summary>
        /// See [MS-OFFCRYPTO] 2.3.4.1 \0x06DataSpaces\DataSpaceMap Stream.
        /// </summary>
        private static readonly byte[] gDataSpaceMap = new byte[]
        {
            0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x68, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x45, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x72, 0x00, 0x79, 0x00, 0x70, 0x00,
            0x74, 0x00, 0x65, 0x00, 0x64, 0x00, 0x50, 0x00, 0x61, 0x00, 0x63, 0x00, 0x6B, 0x00, 0x61, 0x00, 0x67, 0x00,
            0x65, 0x00, 0x32, 0x00, 0x00, 0x00, 0x53, 0x00, 0x74, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x67, 0x00,
            0x45, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x72, 0x00, 0x79, 0x00, 0x70, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00,
            0x6E, 0x00, 0x44, 0x00, 0x61, 0x00, 0x74, 0x00, 0x61, 0x00, 0x53, 0x00, 0x70, 0x00, 0x61, 0x00, 0x63, 0x00,
            0x65, 0x00,
            0x00, 0x00
        };

        /// <summary>
        /// See [MS-OFFCRYPTO] 2.3.4.2 \0x06DataSpaces\DataSpaceInfo Storage.
        /// </summary>
        private static readonly byte[] gStrongEncryptionDataSpace = new byte[]
        {
            0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x53, 0x00, 0x74, 0x00, 0x72, 0x00,
            0x6F, 0x00, 0x6E, 0x00, 0x67, 0x00, 0x45, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x72, 0x00, 0x79, 0x00, 0x70, 0x00,
            0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x54, 0x00, 0x72, 0x00, 0x61, 0x00, 0x6E, 0x00, 0x73, 0x00,
            0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x00, 0x00
        };

        /// <summary>
        /// See [MS-OFFCRYPTO] 2.3.4.3 \0x06DataSpaces\TransformInfo Storage.
        /// </summary>
        private static readonly byte[] g0006Primary = new byte[]
        {
            0x58, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x7B, 0x00, 0x46, 0x00, 0x46, 0x00,
            0x39, 0x00, 0x41, 0x00, 0x33, 0x00, 0x46, 0x00, 0x30, 0x00, 0x33, 0x00, 0x2D, 0x00, 0x35, 0x00, 0x36, 0x00,
            0x45, 0x00, 0x46, 0x00, 0x2D, 0x00, 0x34, 0x00, 0x36, 0x00, 0x31, 0x00, 0x33, 0x00, 0x2D, 0x00, 0x42, 0x00,
            0x44, 0x00, 0x44, 0x00, 0x35, 0x00, 0x2D, 0x00, 0x35, 0x00, 0x41, 0x00, 0x34, 0x00, 0x31, 0x00, 0x43, 0x00,
            0x31, 0x00, 0x44, 0x00, 0x30, 0x00, 0x37, 0x00, 0x32, 0x00, 0x34, 0x00, 0x36, 0x00, 0x7D, 0x00, 0x4E, 0x00,
            0x00, 0x00, 0x4D, 0x00, 0x69, 0x00, 0x63, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x73, 0x00, 0x6F, 0x00, 0x66, 0x00,
            0x74, 0x00, 0x2E, 0x00, 0x43, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x61, 0x00, 0x69, 0x00, 0x6E, 0x00,
            0x65, 0x00, 0x72, 0x00, 0x2E, 0x00, 0x45, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x72, 0x00, 0x79, 0x00, 0x70, 0x00,
            0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x54, 0x00, 0x72, 0x00, 0x61, 0x00, 0x6E, 0x00, 0x73, 0x00,
            0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x00, 0x00
        };
        #endregion
    }
}
