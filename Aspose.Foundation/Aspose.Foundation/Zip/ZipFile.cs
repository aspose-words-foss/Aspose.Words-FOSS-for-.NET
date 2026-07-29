// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// ZipFile.cs
//
// Copyright (c) 2006, 2007, 2008, 2009 Dino Chiesa and Microsoft Corporation.
// All rights reserved.
//
// This module is part of DotNetZip, a zipfile class library. 
// The class library reads and writes zip files, according to the format
// described by PKware, at:
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
// This implementation was originally based on the
// System.IO.Compression.DeflateStream base class in the .NET Framework
// v2.0 base class library, but now includes a managed-code port of Zlib.
//
// There are other Zip class libraries available.  For example, it is
// possible to read and write zip files within .NET via the J# runtime.
// But some people don't like to install the extra DLL.  Also, there is
// a 3rd party LGPL-based (or is it GPL?) library called SharpZipLib,
// which works, in both .NET 1.1 and .NET 2.0.  But some people don't
// like the GPL, and some people say it's complicated and slow. 
// Finally, there are commercial tools (From ComponentOne,
// XCeed, etc).  But some people don't want to incur the cost.
//
// This alternative implementation is not GPL licensed, is free of cost,
// and does not require J#. It does require .NET 2.0 .
// 
// This code is released under the Microsoft Public License . 
// See the License.txt for details.  
//
// Bugs:
// 1. no support for reading or writing multi-disk zip archives
// 2. no support for asynchronous operation
// 
// Fri, 31 Mar 2006  14:43
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.IO;
using Aspose.JavaAttributes;


// SonarIgnoreFile
namespace Aspose.Zip
{
    /// <summary>
    /// The ZipFile type represents a zip archive file.  This is the main type in the
    /// DotNetZip class library.  This class reads and writes zip files, as defined in
    /// the format for zip described by PKWare.  The compression for this implementation
    /// was, at one time, based on the System.IO.Compression.DeflateStream base class in
    /// the .NET Framework base class library, available in v2.0 and later of the .NET
    /// Framework. As of v1.7 of DotNetZip, the compression is provided by a
    /// managed-code version of Zlib, included with DotNetZip.
    /// </summary>
    [JavaDelete("For ZIP on Java we use a completely different implementation.")]
    public class ZipFile : IEnumerable<ZipEntry>, IDisposable
    {

        #region public properties

            /// <summary>
            /// Indicates whether to perform a full scan of the zip file when reading it. 
            /// </summary>
            ///
            /// <remarks>
            ///
            /// <para>
            /// When reading a zip file, if this flag is <c>true</c> (<c>True</c> in
            /// VB), the entire zip archive will be scanned and searched for entries.
            /// For large archives, this can take a very, long time. The much more
            /// efficient default behavior is to read the zip directory, at the end of
            /// the zip file. However, in some cases the directory is corrupted and it
            /// is desirable to perform a full scan of the zip file to determine the
            /// contents of the zip file.
            /// </para>
            ///
            /// <para>
            /// If you want to track progress, you can set the ReadProgress event. 
            /// </para>
            ///
            /// <para>
            /// This flag is effective only when calling Initialize.  The Initialize
            /// method may take a long time to run for large zip files, when
            /// <c>Fullscan</c> is true.
            /// </para>
            ///
            /// </remarks>
            ///
            /// <example>
            /// This example shows how to read a zip file using the full scan approach,
            /// and then save it, thereby producing a corrected zip file. 
            /// <code lang="C#">
            /// using (var zip = new ZipFile())
            /// {
            ///     zip.FullScan = true;
            ///     zip.Initialize(zipFileName);
            ///     zip.Save(newName);
            /// }
            /// </code>
            ///
            /// <code lang="VB">
            /// Using zip As New ZipFile
            ///     zip.FullScan = True
            ///     zip.Initialize(zipFileName)
            ///     zip.Save(newName)
            /// End Using
            /// </code>
            /// </example>
            ///
            public bool FullScan
        {
            get { return _FullScan; }
            set { _FullScan = value; }
        }

        private bool _FullScan;
        
        /// <summary>
        /// Size of the IO buffer used while saving.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        /// First, let me say that you really don't need to bother with this.  It is
        /// here to allow for optimizations that you probably won't make! It will work
        /// fine if you don't set or get this property at all. Ok?
        /// </para>
        ///
        /// <para>
        /// Now that we have <em>that</em> out of the way, the fine print: This
        /// property affects the size of the buffer that is used for I/O for each entry
        /// contained in the zip file. When a file is read in to be compressed, it uses
        /// a buffer given by the size here.  When you update a zip file, the data for
        /// unmodified entries is copied from the first zip file to the other, through a
        /// buffer given by the size here.
        /// </para>
        ///
        /// <para>
        /// Changing the buffer size affects a few things: first, for larger buffer
        /// sizes, the memory used by the <c>ZipFile</c>, obviously, will be larger
        /// during I/O operations.  This may make operations faster for very much larger
        /// files.  Last, for any given entry, when you use a larger buffer there will be
        /// fewer progress events during I/O operations, because there's one progress
        /// event generated for each time the buffer is filled and then emptied.
        /// </para>
        ///
        /// <para>
        /// The default buffer size is 8k.  Increasing the buffer size may speed things
        /// up as you compress larger files.  But there are no hard-and-fast rules here,
        /// eh?  You won't know til you test it.  And there will be a limit where ever
        /// larger buffers actually slow things down.  So as I said in the beginning,
        /// it's probably best if you don't set or get this property at all.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// This example shows how you might set a large buffer size for efficiency when
        /// dealing with zip entries that are larger than 1gb. 
        /// <code lang="C#">
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.SaveProgress += this.zip1_SaveProgress;
        ///     zip.AddDirectory(directoryToZip, "");
        ///     zip.UseZip64WhenSaving = Zip64Option.Always;
        ///     zip.BufferSize = 65536*8; // 65536 * 8 = 512k
        ///     zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        /// </example>
        
        public int BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; }
        }

        /// <summary>
        /// Size of the work buffer to use for the ZLIB codec during compression.
        /// </summary>
        public int CodecBufferSize
        {
            get { return _CodecBufferSize; }
            set { _CodecBufferSize = value; }
        }

        private int _CodecBufferSize;

        /// <summary>
        /// Indicates whether extracted files should keep their paths as
        /// stored in the zip archive. 
        /// </summary>
        public bool FlattenFoldersOnExtract
        {
            get { return _FlattenFoldersOnExtract; }
            set { _FlattenFoldersOnExtract = value; }
        }

        private bool _FlattenFoldersOnExtract;
    
        /// <summary>
        /// The compression strategy to use for all entries.
        /// </summary>
        ///
        /// <remarks>
        /// This refers to the Strategy used by the ZLIB-compatible compressor. Different
        /// compression strategies work better on different sorts of data. The strategy parameter
        /// can affect the compression ratio and the speed of compression but not the correctness
        /// of the compresssion.  For more information see <see
        /// cref="CompressionStrategy "/>.
        /// </remarks>
        public Aspose.Zip.CompressionStrategy Strategy
        {
            get { return _Strategy; }
            set { _Strategy = value; }
        }

        /// <summary>
        /// The name of the <c>ZipFile</c>, on disk.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// When the <c>ZipFile</c> instance was created by reading an archive using one
        /// of the <c>ZipFile.Read</c> methods, this property represents the name of the
        /// zip file that was read.  When the <c>ZipFile</c> instance was created by
        /// using the no-argument constructor, this value is <c>null</c> (<c>Nothing</c>
        /// in VB).
        /// </para>
        ///
        /// <para>
        /// If you use the no-argument constructor, and you then explicitly set this
        /// property, when you call <see cref="ZipFile.Save()"/>, this name will specify
        /// the name of the zip file created.  Doing so is equivalent to calling <see
        /// cref="ZipFile.Save(String)"/>.  When instantiating a ZipFile by reading from
        /// a stream or byte array, the Name property remains <c>null</c>.  When saving
        /// to a stream, the Name property is implicitly set to <c>null</c>.
        /// </para>
        /// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// Sets the compression level to be used for entries when saving the zip archive.
        /// </summary>
        /// <remarks>
        /// The compression level setting is used at the time of <c>Save()</c>. The same
        /// level is applied to all <c>ZipEntry</c> instances contained in the
        /// <c>ZipFile</c> during the save.  If you do not set this property, the
        /// default compression level is used, which normally gives a good balance of
        /// compression efficiency and compression speed.  In some tests, using
        /// <c>BestCompression</c> can double the time it takes to compress, while
        /// delivering just a small increase in compression efficiency.  This behavior
        /// will vary with the type of data you compress.  If you are in doubt, just
        /// leave this setting alone, and accept the default.
        /// </remarks>
        public Aspose.Zip.CompressionLevel CompressionLevel
        {
            get { return _CompressionLevel; }
            set { _CompressionLevel = value; }
        }

        private Aspose.Zip.CompressionLevel _CompressionLevel;

        /// <summary>
        /// A comment attached to the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// This property is read/write. It allows the application to specify a comment
        /// for the <c>ZipFile</c>, or read the comment for the <c>ZipFile</c>.  After
        /// setting this property, changes are only made permanent when you call a
        /// <c>Save()</c> method.
        /// </para>
        ///
        /// <para>
        /// According to <see
        /// href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's zip
        /// specification</see>, the comment is not encrypted, even if there is a
        /// password set on the zip file.
        /// </para>
        ///
        /// <para>
        /// The zip spec does not describe how to encode the comment string in a code
        /// page other than IBM437.  Therefore, for "compliant" zip tools and libraries,
        /// comments will use IBM437.  However, there are situations where you want an
        /// encoded Comment, for example using code page 950 "Big-5 Chinese".  DotNetZip
        /// will encode the comment in the code page specified by <see
        /// cref="ProvisionalAlternateEncoding"/>, at the time of the call to
        /// ZipFile.Save().
        /// </para>
        ///
        /// <para>
        /// When creating a zip archive using this library, it is possible to change the
        /// value of <see cref="ProvisionalAlternateEncoding" /> between each entry you
        /// add, and between adding entries and the call to Save(). Don't do this.  It
        /// will likely result in a zipfile that is not readable by any tool or
        /// application.  For best interoperability, leave <see
        /// cref="ProvisionalAlternateEncoding" /> alone, or specify it only once,
        /// before adding any entries to the <c>ZipFile</c> instance.
        /// </para>
        ///
        /// </remarks>
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                _contentsChanged = true;
            }
        }

        /// <summary>
        /// Specifies whether the Creation, Access, and Modified times
        /// for entries added to the zip file will be emitted in "Unix(tm)
        /// format" when the zip archive is saved.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// An application creating a zip archive can use this flag to explicitly
        /// specify that the file times for the entries should or should not be stored
        /// in the zip archive in the format used by Unix. By default this flag is
        /// <c>false</c>.
        /// </para>
        ///
        /// <para>
        /// When adding an entry from a file or directory, the Creation (<see
        /// cref="ZipEntry.CreationTime"/>), Access (<see cref="ZipEntry.AccessedTime"/>),
        /// and Modified (<see cref="ZipEntry.ModifiedTime"/>) times for the given entry are
        /// automatically set from the filesystem values. When adding an entry from a stream
        /// or string, all three values are implicitly set to DateTime.Now.  Applications
        /// can also explicitly set those times by calling <see
        /// cref="ZipEntry.SetEntryTimes(DateTime, DateTime, DateTime)"/>.
        /// </para>
        ///
        /// <para>
        /// <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
        /// zip specification</see> describes multiple ways to format these times in a
        /// zip file. One is the format Windows applications normally use: 100ns ticks
        /// since Jan 1, 1601 UTC.  The other is a format Unix applications typically
        /// use: seconds since Jan 1, 1970 UTC.  Each format can be stored in an "extra
        /// field" in the zip entry when saving the zip archive. The former uses an
        /// extra field with a Header Id of 0x000A, while the latter uses a header ID of
        /// 0x5455.
        /// </para>
        ///
        /// <para>
        /// Not all tools and libraries can interpret these fields.  Windows compressed
        /// folders is one that can read the Windows Format timestamps, while I believe
        /// the <see href="http://www.info-zip.org/">Infozip</see> tools can read the Unix
        /// format timestamps. Some tools and libraries may be able to read only one or
        /// the other.
        /// </para>
        ///
        /// <para>
        /// The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
        /// cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
        /// </para>
        ///
        /// <para>
        /// The value set here applies to all entries subsequently added to the
        /// <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        /// This property is not mutually exclusive of the <see
        /// cref="EmitTimesInUnixFormatWhenSaving" /> property.  It is possible and
        /// legal and valid to produce a zip file that contains timestamps encoded in
        /// the Unix format as well as in the Windows format.  I haven't got a complete
        /// list of tools and which sort of timestamps they can use and will
        /// tolerate. You'll have to test it yourself.  If you get any good information
        /// and would like to pass it on, please do so and I will include that
        /// information in this documentation.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// This example shows how to save a zip file that contains file timestamps
        /// in a format normally used by Unix.
        /// <code lang="C#">
        /// using (var zip = new ZipFile())
        /// {
        ///     zip.EmitTimesInWindowsFormatWhenSaving = false;
        ///     zip.EmitTimesInUnixFormatWhenSaving = true;
        ///     zip.AddDirectory(directoryToZip, "files");
        ///     zip.Save(outputFile);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     zip.EmitTimesInWindowsFormatWhenSaving = False
        ///     zip.EmitTimesInUnixFormatWhenSaving = True
        ///     zip.AddDirectory(directoryToZip, "files")
        ///     zip.Save(outputFile)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="ZipEntry.EmitTimesInWindowsFormatWhenSaving" />
        /// <seealso cref="EmitTimesInUnixFormatWhenSaving" />
        public bool EmitTimesInWindowsFormatWhenSaving
        {
            get
            {
                return _emitNtfsTimes;
            }
            set
            {
                _emitNtfsTimes= value;
            }
        }


        /// <summary>
        /// Specifies whether the Creation, Access, and Modified times
        /// for entries added to the zip file will be emitted in "Unix(tm)
        /// format" when the zip archive is saved.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// An application creating a zip archive can use this flag to explicitly
        /// specify that the file times for the entries should or should not be stored
        /// in the zip archive in the format used by Unix. By default this flag is
        /// <c>false</c>.
        /// </para>
        ///
        /// <para>
        /// When adding an entry from a file or directory, the Creation (<see
        /// cref="ZipEntry.CreationTime"/>), Access (<see cref="ZipEntry.AccessedTime"/>), and
        /// Modified (<see cref="ZipEntry.ModifiedTime"/>) times for the given entry are
        /// automatically set from the filesystem values. When adding an entry from a
        /// stream or string, all three values are implicitly set to DateTime.Now.
        /// Applications can also explicitly set those times by calling <see
        /// cref="ZipEntry.SetEntryTimes(DateTime, DateTime, DateTime)"/>.
        /// </para>
        ///
        /// <para>
        /// <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
        /// zip specification</see> describes multiple ways to format these times in a
        /// zip file. One is the format Windows applications normally use: 100ns ticks
        /// since Jan 1, 1601 UTC.  The other is a format Unix applications typically
        /// use: seconds since Jan 1, 1970 UTC.  Each format can be stored in an "extra
        /// field" in the zip entry when saving the zip archive. The former uses an
        /// extra field with a Header Id of 0x000A, while the latter uses a header ID of
        /// 0x5455.
        /// </para>
        ///
        /// <para>
        /// Not all tools and libraries can interpret these fields.  Windows compressed
        /// folders is one that can read the Windows Format timestamps, while I believe
        /// the <see href="http://www.info-zip.org/">Infozip</see> tools can read the Unix
        /// format timestamps. Some tools and libraries may be able to read only one or
        /// the other.
        /// </para>
        ///
        /// <para>
        /// The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
        /// cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
        /// </para>
        ///
        /// <para>
        /// This property is not mutually exclusive of the <see
        /// cref="EmitTimesInWindowsFormatWhenSaving" /> property.  It is possible and
        /// legal and valid to produce a zip file that contains timestamps encoded in
        /// the Unix format as well as in the Windows format.  I haven't got a complete
        /// list of tools and which sort of timestamps they can use and will
        /// tolerate. You'll have to test it yourself.  If you get any good information
        /// and would like to pass it on, please do so and I will include that
        /// information in this documentation.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="ZipEntry.EmitTimesInUnixFormatWhenSaving" />
        /// <seealso cref="EmitTimesInWindowsFormatWhenSaving" />
        public bool EmitTimesInUnixFormatWhenSaving
        {
            get
            {
                return _emitUnixTimes;
            }
            set
            {
                _emitUnixTimes= value;
            }
        }


                
        #if LEGACY
        /// <summary>
            /// When this is set, any volume name (eg C:) is trimmed 
            /// from fully-qualified pathnames on any ZipEntry, before writing the 
            /// ZipEntry into the <c>ZipFile</c>. 
            /// </summary>
            ///
            /// <remarks>
            /// <para>
            /// The default value is <c>true</c>. This setting must be true to allow 
            /// Windows Explorer to read the zip archives properly. It's also required to be 
            /// true if you want to read the generated zip files on any other non-Windows OS. 
            /// </para>
            /// 
            /// <para>
            /// The property is included for backwards compatibility only.  You'll 
            /// almost never need or want to set this to false.
            /// </para>
            ///
            /// </remarks>
            private bool TrimVolumeFromFullyQualifiedPaths
            {
                get { return _TrimVolumeFromFullyQualifiedPaths; }
                set { _TrimVolumeFromFullyQualifiedPaths = value; }
            }
        #endif

        /// <summary>
        /// Indicates whether verbose output is sent to the StatusMessageTextWriter during
        /// <c>AddXxx()</c> and <c>ReadXxx()</c> operations.
        /// </summary>
        ///
        /// <remarks>
        /// This is a synthetic property.  It returns true if the <see
        /// cref="StatusMessageTextWriter"/> is non-null.
        /// </remarks>
        internal bool Verbose
        {
            get { return (_StatusMessageTextWriter != null); }
        }


        /// <summary>
        /// Indicates whether to perform case-sensitive matching on the filename when
        /// retrieving entries in the zipfile via the string-based indexer.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>false</c>, which means DON'T do case-sensitive
        /// matching. In other words, retrieving zip["ReadMe.Txt"] is the same as
        /// zip["readme.txt"].  It really makes sense to set this to <c>true</c> only if
        /// you are not running on Windows, which has case-insensitive filenames. But
        /// since this library is not built for non-Windows platforms, in most cases you
        /// should just leave this property alone.
        /// </remarks>
        public bool CaseSensitiveRetrieval
        {
            get { return _CaseSensitiveRetrieval; }
            set { _CaseSensitiveRetrieval = value; }
        }


        /// <summary>
        /// Indicates whether to encode entry filenames and entry comments using Unicode 
        /// (UTF-8).
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">The
        /// PKWare zip specification</see> provides for encoding file names and file
        /// comments in either the IBM437 code page, or in UTF-8.  This flag selects the
        /// encoding according to that specification.  By default, this flag is false,
        /// and filenames and comments are encoded into the zip file in the IBM437
        /// codepage.  Setting this flag to true will specify that filenames and
        /// comments that cannot be encoded with IBM437 will be encoded with UTF-8.
        /// </para>
        ///
        /// <para>
        /// Zip files created with strict adherence to the PKWare specification with
        /// respect to UTF-8 encoding can contain entries with filenames containing any
        /// combination of Unicode characters, including the full range of characters
        /// from Chinese, Latin, Hebrew, Greek, Cyrillic, and many other alphabets.
        /// However, because at this time, the UTF-8 portion of the PKWare specification
        /// is not broadly supported by other zip libraries and utilities, such zip
        /// files may not be readable by your favorite zip tool or archiver. In other
        /// words, interoperability will decrease if you set this flag to true.
        /// </para>
        ///
        /// <para>
        /// In particular, Zip files created with strict adherence to the PKWare
        /// specification with respect to UTF-8 encoding will not work well with
        /// Explorer in Windows XP or Windows Vista, because Windows compressed folders,
        /// as far as I know, do not support UTF-8 in zip files.  Vista can read the zip
        /// files, but shows the filenames incorrectly. Unpacking from Windows Vista
        /// Explorer will result in filenames that have rubbish characters in place of
        /// the high-order UTF-8 bytes.
        /// </para>
        ///
        /// <para>
        /// Also, zip files that use UTF-8 encoding will not work well with Java
        /// applications that use the java.util.zip classes, as of v5.0 of the Java
        /// runtime. The Java runtime does not correctly implement the PKWare
        /// specification in this regard.
        /// </para>
        ///
        /// <para>
        /// As a result, we have the unfortunate situation that "correct" behavior by the
        /// DotNetZip library with regard to Unicode encoding of filenames during zip
        /// creation will result in zip files that are readable by strictly compliant and
        /// current tools (for example the most recent release of the commercial WinZip
        /// tool); but these zip files will not be readable by various other tools or
        /// libraries, including Windows Explorer.
        /// </para>
        ///
        /// <para>
        /// The DotNetZip library can read and write zip files with UTF8-encoded
        /// entries, according to the PKware spec.  If you use DotNetZip for both
        /// creating and reading the zip file, and you use UTF-8, there will be no loss
        /// of information in the filenames. For example, using a self-extractor created
        /// by this library will allow you to unpack files correctly with no loss of
        /// information in the filenames.
        /// </para>
        ///
        /// <para>
        /// If you do not set this flag, it will remain false.  If this flag is false,
        /// your ZipFile will encode all filenames and comments using the IBM437
        /// codepage.  This can cause "loss of information" on some filenames, but the
        /// resulting zipfile will be more interoperable with other utilities. As an
        /// example of the loss of information, diacritics can be lost.  The o-tilde
        /// character will be down-coded to plain o.  The c with a cedilla (Unicode
        /// 0xE7) used in Portugese will be downcoded to a c.  Likewise, the O-stroke
        /// character (Unicode 248), used in Danish and Norwegian, will be down-coded to
        /// plain o. Chinese characters cannot be represented in codepage IBM437; when
        /// using the default encoding, Chinese characters in filenames will be
        /// represented as ?. These are all examples of "information loss".
        /// </para>
        ///
        /// <para>
        /// The loss of information associated to the use of the IBM437 encoding is
        /// inconvenient, and can also lead to runtime errors. For example, using
        /// IBM437, any sequence of 4 Chinese characters will be encoded as ????.  If
        /// your application creates a ZipFile, then adds two files, each with names of
        /// four Chinese characters each, this will result in a duplicate filename
        /// exception.  In the case where you add a single file with a name containing
        /// four Chinese characters, calling Extract() on the entry that has question
        /// marks in the filename will result in an exception, because the question mark
        /// is not legal for use within filenames on Windows.  These are just a few
        /// examples of the problems associated to loss of information.
        /// </para>
        ///
        /// <para>
        /// This flag is independent of the encoding of the content within the entries
        /// in the zip file. Think of the zip file as a container - it supports an
        /// encoding.  Within the container are other "containers" - the file entries
        /// themselves.  The encoding within those entries is independent of the
        /// encoding of the zip archive container for those entries.
        /// </para>
        ///
        /// <para>
        /// Rather than specify the encoding in a binary fashion using this flag, an
        /// application can specify an arbitrary encoding via the <see
        /// cref="ProvisionalAlternateEncoding"/> property.  Setting the encoding
        /// explicitly when creating zip archives will result in non-compliant zip files
        /// that, curiously, are fairly interoperable.  The challenge is, the PKWare
        /// specification does not provide for a way to specify that an entry in a zip
        /// archive uses a code page that is neither IBM437 nor UTF-8.  Therefore if you
        /// set the encoding explicitly when creating a zip archive, you must take care
        /// upon reading the zip archive to use the same code page.  If you get it
        /// wrong, the behavior is undefined and may result in incorrect filenames,
        /// exceptions, stomach upset, hair loss, and acne.
        /// </para>
        /// </remarks>
        /// <seealso cref="ProvisionalAlternateEncoding"/>
        public bool UseUnicodeAsNecessary
        {
            get
            {
                return _provisionalAlternateEncoding == System.Text.Encoding.GetEncoding("UTF-8");
            }
            set
            {
                _provisionalAlternateEncoding = (value) ? System.Text.Encoding.GetEncoding("UTF-8") : DefaultEncoding;
            }
        }


        /// <summary>
        /// Specify whether to use ZIP64 extensions when saving a zip archive. 
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        /// Designed many years ago, the <see
        /// href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">original zip
        /// specification from PKWARE</see> allowed for 32-bit quantities for the
        /// compressed and uncompressed sizes of zip entries, as well as a 32-bit
        /// quantity for specifying the length of the zip archive itself, and a maximum
        /// of 65535 entries.  These limits are now regularly exceeded in many backup
        /// and archival scenarios.  Recently, PKWare added extensions to the original
        /// zip spec, called "ZIP64 extensions", to raise those limitations.  This
        /// property governs whether the <c>ZipFile</c> instance will use those
        /// extensions when writing zip archives within a call to one of the Save()
        /// methods.  The use of these extensions is optional and explicit in DotNetZip
        /// because, despite the status of ZIP64 as a bona fide standard, many other zip
        /// tools and libraries do not support ZIP64, and therefore a zip file saved
        /// with ZIP64 extensions may be unreadable by some of those other tools.
        /// </para>
        /// 
        /// <para>
        /// Set this property to <see cref="Zip64Option.Always"/> to always use ZIP64
        /// extensions when saving, regardless of whether your zip archive needs it.
        /// Suppose you add 5 files, each under 100k, to a ZipFile.  If you specify
        /// Always for this flag before calling the Save() method, you will get a ZIP64
        /// archive, though you do not need to use ZIP64 because none of the original
        /// zip limits had been exceeded.
        /// </para>
        ///
        /// <para>
        /// Set this property to <see cref="Zip64Option.Never"/> to tell the DotNetZip
        /// library to never use ZIP64 extensions.  This is useful for maximum
        /// compatibility and interoperability, at the expense of the capability of
        /// handling large files or large archives.  NB: Windows Explorer in Windows XP
        /// and Windows Vista cannot currently extract files from a zip64 archive, so if
        /// you want to guarantee that a zip archive produced by this library will work
        /// in Windows Explorer, use <c>Never</c>. If you set this property to <see
        /// cref="Zip64Option.Never"/>, and your application creates a zip that would
        /// exceed one of the ZIP limits, the library will throw an exception during the
        /// Save().
        /// </para>
        ///
        /// <para>
        /// Set this property to <see cref="Zip64Option.AsNecessary"/> to tell the
        /// DotNetZip library to use the zip64 extensions when required by the
        /// entry. After the file is compressed, the original and compressed sizes are
        /// checked, and if they exceed the limits described above, then zip64 can be
        /// used. That is the general idea, but there is an additional wrinkle when
        /// saving to a non-seekable device, like the ASP.NET
        /// <c>Response.OutputStream</c>, or <c>Console.Out</c>.  When using
        /// non-seekable streams for output, the entry header - which indicates whether
        /// zip64 is in use - is emitted before it is known if zip64 is necessary.  It
        /// is only after all entries have been saved that it can be known if ZIP64 will
        /// be required.  On seekable output streams, after saving all entries, the
        /// library can seek backward and re-emit the zip file header to be consistent
        /// with the actual ZIP64 requirement.  But using a non-seekable output stream,
        /// the library cannot seek backward, so the header can never be changed. In
        /// other words, the archive's use of ZIP64 extensions is not alterable after
        /// the header is emitted.  Therefore, when saving to non-seekable streams,
        /// using <see cref="Zip64Option.AsNecessary"/> is the same as using <see
        /// cref="Zip64Option.Always"/>: it will always produce a zip archive that uses
        /// zip64 extensions.
        /// </para>
        ///
        /// <para>
        /// The default value for the property is <see cref="Zip64Option.Never"/>. <see
        /// cref="Zip64Option.AsNecessary"/> is safest, in the sense that you will not
        /// get an Exception if a pre-ZIP64 limit is exceeded.
        /// </para>
        ///
        /// <para>
        /// You may set the property at any time before calling Save(). 
        /// </para>
        ///
        /// <para>
        /// The <c>Zipfile.Read()</c> method will properly read ZIP64-endowed zip
        /// archives, regardless of the value of this property.  DotNetZip will always
        /// read ZIP64 archives.  This property governs whether DotNetZip will write
        /// them. Therefore, when updating archives, be careful about setting this
        /// property after reading an archive that may use ZIP64 extensions.
        /// </para>
        ///
        /// <para>
        /// An interesting question is, if you have set this property to
        /// <c>AsNecessary</c>, and then successfully saved, does the resulting archive
        /// use ZIP64 extensions or not?  To learn this, check the <see
        /// cref="OutputUsedZip64"/> property, after calling Save().
        /// </para>
        ///
        /// <para>
        /// Have you thought about
        /// <see href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">donating</see>?
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="RequiresZip64"/>
        public Zip64Option UseZip64WhenSaving
        {
            get
            {
                return _zip64;
            }
            set
            {
                _zip64 = value;
            }
        }



        /// <summary>
        /// Indicates whether the archive requires ZIP64 extensions.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        /// This property is <c>null</c> (or <c>Nothing</c> in VB) if the archive has not been
        /// saved, and there are fewer than 65334 ZipEntry items contained in the archive.
        /// </para>
        ///
        /// <para>
        /// The <c>Value</c> is true if any of the following four conditions holds: the
        /// uncompressed size of any entry is larger than 0xFFFFFFFF; the compressed
        /// size of any entry is larger than 0xFFFFFFFF; the relative offset of any
        /// entry within the zip archive is larger than 0xFFFFFFFF; or there are more
        /// than 65534 entries in the archive.  (0xFFFFFFFF = 4,294,967,295).  The
        /// result may not be known until a Save() is attempted on the zip archive.  The
        /// Value of this <see cref="System.Nullable"/> property may be set only AFTER
        /// one of the Save() methods has been called.
        /// </para>
        ///
        /// <para>
        /// If none of the four conditions holds, and the archive has been saved, then
        /// the Value is false.
        /// </para>
        ///
        /// <para>
        /// A <c>Value</c> of false does not indicate that the zip archive, as saved,
        /// does not use ZIP64.  It merely indicates that ZIP64 is not required.  An
        /// archive may use ZIP64 even when not required if the <see
        /// cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
        /// cref="Zip64Option.Always"/>, or if the <see
        /// cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
        /// cref="Zip64Option.AsNecessary"/> and the output stream was not seekable. Use
        /// the <see cref="OutputUsedZip64"/> property to determine if the most recent
        /// <c>Save()</c> method resulted in an archive that utilized the ZIP64
        /// extensions.
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="UseZip64WhenSaving"/>
        /// <seealso cref="OutputUsedZip64"/>
        public NullableBool RequiresZip64
        {
            get
            {
                if (_entries.Count > 65534)
                    return NullableBool.True;

                // If the <c>ZipFile</c> has not been saved or if the contents have changed, then
                // it is not known if ZIP64 is required.
                if (!_hasBeenSaved || _contentsChanged) return NullableBool.NotDefined;

                // Whether ZIP64 is required is knowable.
                foreach (ZipEntry e in _entries)
                {
                    if (e.RequiresZip64 == NullableBool.True) return NullableBool.True;
                }

                return NullableBool.False;
            }
        }


        /// <summary>
        /// Describes whether the most recent <c>Save()</c> operation used ZIP64 extensions.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The use of ZIP64 extensions within an archive is not always necessary, and for
        /// interoperability concerns, it may be desired to NOT use ZIP64 if possible.  The
        /// <see cref="ZipFile.UseZip64WhenSaving"/> property can be set to use ZIP64
        /// extensions only when necessary.  In those cases, Sometimes applications want to
        /// know whether a Save() actually used ZIP64 extensions.  Applications can query
        /// this read-only property to learn whether ZIP64 has been used in a just-saved
        /// <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        /// The value is <c>null</c> (or <c>Nothing</c> in VB) if the archive has not
        /// been saved.
        /// </para>
        ///
        /// <para>
        /// Non-null values (<c>HasValue</c> is true) indicate whether ZIP64 extensions
        /// were used during the most recent <c>Save()</c> operation.  The ZIP64
        /// extensions may have been used as required by any particular entry because of
        /// its uncompressed or compressed size, or because the archive is larger than
        /// 4294967295 bytes, or because there are more than 65534 entries in the
        /// archive, or because the <c>UseZip64WhenSaving</c> property was set to <see
        /// cref="Zip64Option.Always"/>, or because the <c>UseZip64WhenSaving</c>
        /// property was set to <see cref="Zip64Option.AsNecessary"/> and the output
        /// stream was not seekable.  The value of this property does not indicate the
        /// reason the ZIP64 extensions were used.
        /// </para>
        /// </remarks>
        /// <seealso cref="UseZip64WhenSaving"/>
        /// <seealso cref="RequiresZip64"/>
        public NullableBool OutputUsedZip64
        {
            get
            {
                return _OutputUsesZip64;
            }
        }


        /// <summary>
        /// The text encoding to use when writing new entries to the <c>ZipFile</c>, for
        /// those entries that cannot be encoded with the default (IBM437) encoding; or,
        /// the text encoding that was used when reading the entries from the
        /// <c>ZipFile</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// In <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">its
        /// zip specification</see>, PKWare describes two options for encoding filenames
        /// and comments: using IBM437 or UTF-8.  But, some archiving tools or libraries
        /// do not follow the specification, and instead encode characters using the
        /// system default code page.  For example, WinRAR when run on a machine in
        /// Shanghai may encode filenames with the Big-5 Chinese (950) code page.  This
        /// behavior is contrary to the Zip specification, but it occurs anyway.
        /// </para>
        ///
        /// <para>
        /// When using DotNetZip to write zip archives that will be read by one of these
        /// other archivers, set this property to specify the code page to use when
        /// encoding the <see cref="ZipEntry.FileName"/> and <see
        /// cref="ZipEntry.Comment"/> for each ZipEntry in the zip file, for values that
        /// cannot be encoded with the default codepage for zip files, IBM437.  This is
        /// why this property is "provisional".  In all cases, IBM437 is used where
        /// possible, in other words, where no loss of data would result. It is
        /// possible, therefore, to have a given entry with a Comment encoded in IBM437
        /// and a FileName encoded with the specified "provisional" codepage.
        /// </para>
        ///
        /// <para>
        /// Be aware that a zip file created after you've explicitly set the <see
        /// cref="ProvisionalAlternateEncoding" /> property to a value other than IBM437
        /// may not be compliant to the PKWare specification, and may not be readable by
        /// compliant archivers.  On the other hand, many (most?) archivers are
        /// non-compliant and can read zip files created in arbitrary code pages.  The
        /// trick is to use or specify the proper codepage when reading the zip.
        /// </para>
        ///
        /// <para>
        /// When creating a zip archive using this library, it is possible to change the
        /// value of <see cref="ProvisionalAlternateEncoding" /> between each entry you
        /// add, and between adding entries and the call to Save(). Don't do this. It
        /// will likely result in a zipfile that is not readable.  For best
        /// interoperability, either leave <see cref="ProvisionalAlternateEncoding" />
        /// alone, or specify it only once, before adding any entries to the
        /// <c>ZipFile</c> instance.  There is one exception to this recommendation,
        /// described later.
        /// </para>
        ///
        /// <para>
        /// When using an arbitrary, non-UTF8 code page for encoding, there is no
        /// standard way for the creator application - whether DotNetZip, WinZip,
        /// WinRar, or something else - to formally specify in the zip file which
        /// codepage has been used for the entries. As a result, readers of zip files
        /// are not able to inspect the zip file and determine the codepage that was
        /// used for the entries contained within it.  It is left to the application or
        /// user to determine the necessary codepage when reading zip files encoded this
        /// way.  If you use an incorrect codepage when reading a zipfile, you will get
        /// entries with filenames that are incorrect, and the incorrect filenames may
        /// even contain characters that are not legal for use within filenames in
        /// Windows. Extracting entries with illegal characters in the filenames will
        /// lead to exceptions. It's too bad, but this is just the way things are with
        /// code pages in zip files. Caveat Emptor.
        /// </para>
        ///
        /// <para>
        /// When using DotNetZip to read a zip archive, and the zip archive uses an
        /// arbitrary code page, you must specify the encoding to use before or when the
        /// <c>Zipfile</c> is READ.  This means you must use a <c>ZipFile.Read()</c>
        /// method that allows you to specify a System.Text.Encoding parameter.  Setting
        /// the ProvisionalAlternateEncoding property after your application has read in
        /// the zip archive will not affect the entry names of entries that have already
        /// been read in, and is probably not what you want.
        /// </para>
        ///     
        /// <para>
        /// And now, the exception to the rule described above.  One strategy for
        /// specifying the code page for a given zip file is to describe the code page
        /// in a human-readable form in the Zip comment. For example, the comment may
        /// read "Entries in this archive are encoded in the Big5 code page".  For
        /// maximum interoperability, the zip comment in this case should be encoded in
        /// the default, IBM437 code page.  In this case, the zip comment is encoded
        /// using a different page than the filenames.  To do this, Specify
        /// <c>ProvisionalAlternateEncoding</c> to your desired region-specific code
        /// page, once before adding any entries, and then reset
        /// <c>ProvisionalAlternateEncoding</c> to IBM437 before setting the <see
        /// cref="Comment"/> property and calling Save().
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// This example shows how to read a zip file using the Big-5 Chinese code page
        /// (950), and extract each entry in the zip file.  For this code to work as
        /// desired, the <c>Zipfile</c> must have been created using the big5 code page
        /// (CP950). This is typical, for example, when using WinRar on a machine with
        /// CP950 set as the default code page.  In that case, the names of entries
        /// within the Zip archive will be stored in that code page, and reading the zip
        /// archive must be done using that code page.  If the application did not use
        /// the correct code page in ZipFile.Read(), then names of entries within the
        /// zip archive would not be correctly retrieved.
        /// <code>
        /// using (var zip = ZipFile.Read(zipFileName, System.Text.Encoding.GetEncoding("big5")))
        /// {
        ///     // retrieve and extract an entry using a name encoded with CP950
        ///     zip[MyDesiredEntry].Extract("unpack");
        /// }
        /// </code>
        ///
        /// <code Lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(ZipToExtract, System.Text.Encoding.fileGetencoding(950))
        ///     ' retrieve and extract an entry using a name encoded with CP950
        ///     zip(MyDesiredEntry).Extract("unpack")
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.DefaultEncoding">DefaultEncoding</seealso>
        public System.Text.Encoding ProvisionalAlternateEncoding
        {
            get
            {
                return _provisionalAlternateEncoding;
            }
            set
            {
                _provisionalAlternateEncoding = value;
            }
        }

        /// <summary>
        /// The default text encoding used in zip archives.  It is numeric 437, also 
        /// known as IBM437.
        /// </summary>
        /// <seealso cref="Aspose.Zip.ZipFile.ProvisionalAlternateEncoding"/>
        public static readonly System.Text.Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("IBM437");


        /// <summary>
        /// Gets or sets the <c>TextWriter</c> to which status messages are delivered 
        /// for the instance. 
        /// </summary>
        ///
        /// <remarks>
        /// If the TextWriter is set to a non-null value, then verbose output is sent to the
        /// <c>TextWriter</c> during <c>Add</c><c>, Read</c><c>, Save</c> and <c>Extract</c>
        /// operations.  Typically, console applications might use <c>Console.Out</c>
        /// and graphical or headless applications might use a
        /// <c>System.IO.StringWriter</c>. The output of this is suitable for viewing by
        /// humans.
        /// </remarks>
        ///
        /// <example>
        /// <para>
        /// In this example, a console application instantiates a ZipFile, then sets
        /// the StatusMessageTextWriter to Console.Out.  At that point, all verbose
        /// status messages for that ZipFile are sent to the console. 
        /// </para>
        ///
        /// <code lang="C#">
        /// using (ZipFile zip= ZipFile.Read(FilePath))
        /// {
        ///   zip.StatusMessageTextWriter= System.Console.Out;
        ///   // messages are sent to the console during extraction
        ///   zip.ExtractAll();
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(FilePath)
        ///   zip.StatusMessageTextWriter= System.Console.Out
        ///   'Status Messages will be sent to the console during extraction
        ///   zip.ExtractAll()
        /// End Using
        /// </code>
        /// </example>
        public TextWriter StatusMessageTextWriter
        {
            get { return _StatusMessageTextWriter; }
            set { _StatusMessageTextWriter = value; }
        }

        /// <summary>
        /// Gets or sets the flag that indicates whether the <c>ZipFile</c> should use
        /// compression for subsequently added entries in the <c>ZipFile</c> instance.
        /// </summary>
        ///
        /// <remarks>
        /// <para> When saving an entry into a zip archive, the DotNetZip by default
        /// compresses the file. That's what a ZIP archive is all about, isn't it?  For
        /// files that are already compressed, like MP3's or JPGs, the deflate algorithm
        /// can actually slightly expand the size of the data.  Setting this property to
        /// trye allows you to specify that compression should not be used.  The default
        /// value is false.  </para>
        ///
        /// <para>
        /// Do not construe setting this flag to false as "Force Compression".  Setting
        /// it to false merely does NOT force No compression.  If you want to force the
        /// use of the deflate algorithm when storing each entry into the zip archive,
        /// define a <see cref="WillReadTwiceOnInflation"/> callback, which always
        /// returns false, and a <see cref="WantCompression" /> callback that always
        /// returns true.  This is probably the wrong thing to do, but you could do
        /// it. Forcing the use of the Deflate algorithm when storing an entry does not
        /// guarantee that the data size will get smaller. It could increase, as
        /// described above.
        /// </para>
        ///
        /// <para>
        /// Changes to this flag apply to all entries subsequently added to the archive. 
        /// The application can also set the <see cref="ZipEntry.CompressionMethod"/>
        /// property on each ZipEntry, for more granular control of this capability.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="ZipEntry.CompressionMethod"/>
        /// <seealso cref="Aspose.Zip.ZipFile.CompressionLevel"/>
        /// <seealso cref="Aspose.Zip.ZipFile.WantCompression"/>
        ///
        /// <example>
        /// This example shows how to specify that Compression will not be used when
        /// adding files to the zip archive. None of the files added to the archive in
        /// this example will use compression.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   zip.ForceNoCompression = true;
        ///   zip.AddDirectory(@"c:\reports\January");
        ///   zip.Comment = "All files in this archive will be uncompressed.";
        ///   zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile()
        ///   zip.ForceNoCompression = true
        ///   zip.AddDirectory("c:\reports\January")
        ///   zip.Comment = "All files in this archive will be uncompressed."
        ///   zip.Save(ZipFileToCreate)
        /// End Using
        /// </code>
        ///
        /// </example>
        public bool ForceNoCompression
        {
            get { return _ForceNoCompression; }
            set { _ForceNoCompression = value; }
        }


        /// <summary>
        /// Gets or sets the name for the folder to store the temporary file
        /// this library writes when saving a zip archive. 
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This library will create a temporary file when saving a Zip archive to a file.
        /// This file is written when calling one of the <c>Save()</c> methods that does
        /// not save to a stream, or one of the <c>SaveSelfExtractor()</c> methods.  
        /// <para>
        ///
        /// </para>
        /// By default, the library will create the temporary file in the directory
        /// specified for the file itself, via the <see cref="Name"/> property or via the
        /// <see cref="ZipFile.Save(String)"/> method.
        /// </para>
        ///
        /// <para>
        /// Setting this property allows applications to override this default behavior,
        /// so that the library will create the temporary file in the specified
        /// folder. For example, to have the library create the temporary file in the
        /// current working directory, regardless where the <c>ZipFile</c> is saved,
        /// specfy ".".  To revert to the default behavior, set this property to
        /// <c>null</c> (<c>Nothing</c> in VB).
        /// </para>
        ///
        /// <para>
        /// When setting the property to a non-null value, the folder specified must exist;
        /// if it does not an exception is thrown.  The application should have write and
        /// delete permissions on the folder.  The permissions are not explicitly checked
        /// ahead of time; if the application does not have the appropriate rights, an
        /// exception will be thrown at the time <c>Save()</c> is called.
        /// </para>
        ///
        /// <para>
        /// There is no temporary file created when reading a zip archive.  When saving
        /// to a Stream, there is no temporary file created.  For example, if the
        /// application is an ASP.NET application and calls <c>Save()</c> specifying the
        /// <c>Response.OutputStream</c> as the output stream, there is no temporary
        /// file created.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown when setting the property if the directory does not exist. 
        /// </exception>
        ///
        public String TempFileFolder
        {
            get { return _TempFileFolder; }

            set
            {
                _TempFileFolder = value;
                if (value == null) return;

                if (!Directory.Exists(value))
                    throw new FileNotFoundException(String.Format("That directory ({0}) does not exist.", value));

            }
        }

        /// <summary>
        /// Sets the password to be used on the <c>ZipFile</c> instance.
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// When writing a zip archive, this password is applied to the entries, not to
        /// the zip archive itself. It applies to any ZipEntry subsequently added to the
        /// <c>ZipFile</c>, using one of the <c>AddFile</c>, <c>AddDirectory</c>,
        /// <c>AddEntry</c>, or <c>AddItem</c> methods, etc.  When reading a zip
        /// archive, this property applies to any entry subsequently extracted from the
        /// <c>ZipFile</c> using one of the Extract methods on the <c>ZipFile</c> class.
        /// </para>
        /// 
        /// <para>
        /// When writing a zip archive, keep this in mind: though the password is set on the
        /// ZipFile object, according to the Zip spec, the "directory" of the archive - in
        /// other words the list of entries contained in the archive - is not encrypted with
        /// the password, or protected in any way.  if you set the Password property, the
        /// password actually applies to individual entries that are added to the archive,
        /// subsequent to the setting of this property.  The list of filenames in the
        /// archive that is eventually created will appear in clear text, but the contents
        /// of the individual files are encrypted.  This is how Zip encryption works.
        /// </para>
        /// 
        /// <para>
        /// If you set the password on the zip archive, and then add a set of files to the
        /// archive, then each entry is encrypted with that password.  You may also want to
        /// change the password between adding different entries. If you set the password,
        /// add an entry, then set the password to <c>null</c> (<c>Nothing</c> in VB), and
        /// add another entry, the first entry is encrypted and the second is not.  If you
        /// call <c>AddFile()</c>, then set the <c>Password</c> property, then call
        /// <c>ZipFile.Save</c>, the file added will not be password-protected, and no
        /// warning will be generated.
        /// </para>
        /// 
        /// <para>
        /// When setting the Password, you may also want to explicitly set the <see
        /// cref="Encryption"/> property, to specify how to encrypt the entries added to
        /// the ZipFile.  If you set the Password to a non-null value and do not set
        /// <see cref="Encryption"/>, then PKZip 2.0 ("Weak") encryption is used.  This
        /// encryption is relatively weak but is very interoperable. If you set the
        /// password to a <c>null</c> value (<c>Nothing</c> in VB), Encryption is reset
        /// to None.
        /// </para>
        ///
        /// <para>
        /// All of the preceding applies to writing zip archives, in other words when
        /// you use one of the Save methods.  To use this property when reading or an
        /// existing ZipFile, do the following: set the Password property on the
        /// <c>ZipFile</c>, then call one of the Extract() overloads on the <see
        /// cref="ZipEntry" />. In this case, the entry is extracted using the
        /// <c>Password</c> that is specified on the <c>ZipFile</c> instance. If you
        /// have not set the <c>Password</c> property, then the password is <c>null</c>,
        /// and the entry is extracted with no password.
        /// </para>
        ///
        /// <para>
        /// If you set the Password property on the <c>ZipFile</c>, then call Extract()
        /// an entry that has not been encrypted with a password, the password is not
        /// used for that entry, and the <c>ZipEntry</c> is extracted as normal. In
        /// other words, the password is used only if necessary.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="ZipEntry"/> class also has a <see
        /// cref="ZipEntry.Password">Password</see> property.  It takes precedence over
        /// this property on the <c>ZipFile</c>.  Typically, you would use the per-entry
        /// Password when most entries in the zip archive use one password, and a few
        /// entries use a different password.  If all entries in the zip file use the
        /// same password, then it is simpler to just set this property on the
        /// <c>ZipFile</c> itself, whether creating a zip archive or extracting a zip
        /// archive.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <example>
        /// <para>
        /// This example creates a zip file, using password protection for the entries,
        /// and then extracts the entries from the zip file.  When creating the zip
        /// file, the Readme.txt file is not protected with a password, but the other
        /// two are password-protected as they are saved. During extraction, each file
        /// is extracted with the appropriate password.
        /// </para>
        /// <code>
        /// // create a file with encryption
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.AddFile("ReadMe.txt");
        ///     zip.Password= "!Secret1";
        ///     zip.AddFile("MapToTheSite-7440-N49th.png");
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf");
        ///     zip.Save("EncryptedArchive.zip");
        /// }
        /// 
        /// // extract entries that use encryption
        /// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
        /// {
        ///     zip.Password= "!Secret1";
        ///     zip.ExtractAll("extractDir");
        /// }
        /// 
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     zip.AddFile("ReadMe.txt")
        ///     zip.Password = "123456!"
        ///     zip.AddFile("MapToTheSite-7440-N49th.png")
        ///     zip.Password= "!Secret1";
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf")
        ///     zip.Save("EncryptedArchive.zip")
        /// End Using
        ///
        ///
        /// ' extract entries that use encryption
        /// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
        ///     zip.Password= "!Secret1"
        ///     zip.ExtractAll("extractDir")
        /// End Using
        /// 
        /// </code>
        ///
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.Encryption">ZipFile.Encryption</seealso>
        /// <seealso cref="ZipEntry.Password">ZipEntry.Password</seealso>
#pragma warning disable CA1044 // Properties should not be write only
        public String Password
#pragma warning restore CA1044 // Properties should not be write only
        {
            set
            {
                _Password = value;
                if (_Password == null)
                {
                    Encryption = EncryptionAlgorithm.None;
                }
                else if (Encryption == EncryptionAlgorithm.None)
                {
                    Encryption = EncryptionAlgorithm.PkzipWeak;
                }
            }
        }





        /// <summary>
        /// The action the library should take when extracting a file that already exists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property affects the behavior of the Extract methods (one of the
        /// <c>Extract()</c> or <c>ExtractWithPassword()</c> overloads), when extraction
        /// would overwrite an existing filesystem file. If you do not set this
        /// property, the library throws an exception when extracting an entry would
        /// overwrite an existing file.
        /// </para>
        ///
        /// <para>
        /// This property has no effect when extracting to a stream, or when the file to
        /// be extracted does not already exist.
        /// </para>
        /// </remarks>
        /// <seealso cref="ZipEntry.ExtractExistingFile"/>
        public ExtractExistingFileAction ExtractExistingFile
        {
            get { return _ExtractExistingFile; }
            set { _ExtractExistingFile = value; }
        }

        private ExtractExistingFileAction _ExtractExistingFile;

        /// <summary>
        ///   The action the library should take when an error is encountered while
        ///   opening or reading files as they are added to a zip archive. 
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///     In some cases an error will occur when DotNetZip tries to open a file to be
        ///     added to the zip archive.  In other cases, an error might occur after the
        ///     file has been successfully opened, while DotNetZip is reading the file.
        ///  </para>
        /// 
        ///  <para>
        ///    The first problem might occur when calling Adddirectory() on a directory
        ///    that contains a Clipper .dbf file; the file is locked by Clipper and
        ///    cannot be opened bby another process. An example of the second problem is
        ///    the ERROR_LOCK_VIOLATION that results when a file is opened by another
        ///    process, but not locked, and a range lock has been taken on the file.
        ///    Microsoft Outlook takes range locks on .PST files.
        ///  </para>
        ///
        /// </remarks>
        /// <seealso cref="ZipEntry.ZipErrorAction"/>
        public ZipErrorAction ZipErrorAction
        {
            get { return _ZipErrorAction; }
            set { _ZipErrorAction = value; }
        }

        private ZipErrorAction _ZipErrorAction;

        /// <summary>
        /// The Encryption to use for entries added to the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Set this when creating a zip archive, or when updating a zip archive. The
        /// specified Encryption is applied to the entries subsequently added to the
        /// <c>ZipFile</c> instance.  Applications do not need to set <c>Encryption</c>
        /// when reading or extracting a zip archive.
        /// </para>
        /// 
        /// <para>
        /// If you set this to something other than EncryptionAlgorithm.None, you will also
        /// need to set the <see cref="Password"/>.
        /// </para>
        ///
        /// <para>
        /// As with other properties (like <see cref="Password"/> and <see
        /// cref="ForceNoCompression"/>), setting this property a <c>ZipFile</c>
        /// instance will cause that <c>EncryptionAlgorithm</c> to be used on all <see
        /// cref="ZipEntry"/> items that are subsequently added to the <c>ZipFile</c>
        /// instance. In other words, if you set this property after you have added
        /// items to the <c>ZipFile</c>, but before you have called <c>Save()</c>, those
        /// items will not be encrypted or protected with a password in the resulting
        /// zip archive. To get a zip archive with encrypted entries, set this property,
        /// along with the <see cref="Password"/> property, before calling
        /// <c>AddFile</c>, <c>AddItem</c>, or <c>AddDirectory</c> (etc.) on
        /// the <c>ZipFile</c> instance.
        /// </para>
        ///
        /// <para>
        /// Some comments on updating archives: If you read a <c>ZipFile</c>, you cannot
        /// modify the Encryption on any encrypted entry, except by extracting the entry
        /// with the original password (if any), removing the original entry via <see
        /// cref="ZipFile.RemoveEntry(ZipEntry)"/>, and then adding a new entry with a
        /// new Password and Encryption setting.
        /// </para>
        ///
        /// <para>
        /// For example, suppose you read a <c>ZipFile</c>, and there is an encrypted
        /// entry.  Setting the Encryption property on that <c>ZipFile</c> and then
        /// calling <c>Save()</c> on the <c>ZipFile</c> does not update the Encryption
        /// used for the entries in the archive.  Neither is an exception
        /// thrown. Instead, what happens during the <c>Save()</c> is that all
        /// previously existing entries are copied through to the new zip archive, with
        /// whatever encryption and password that was used when originally creating the
        /// zip archive. Upon re-reading that archive, to extract entries, applications
        /// should use the original password or passwords, if any.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        /// This example creates a zip archive that uses encryption, and then extracts
        /// entries from the archive.  When creating the zip archive, the ReadMe.txt
        /// file is zipped without using a password or encryption.  The other files use
        /// encryption.
        /// </para>
        ///
        /// <code>
        /// // Create a zip archive with AES Encryption.
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.AddFile("ReadMe.txt");
        ///     zip.Encryption= EncryptionAlgorithm.WinZipAes256;
        ///     zip.Password= "Top.Secret.No.Peeking!";
        ///     zip.AddFile("7440-N49th.png");
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf");
        ///     zip.Save("EncryptedArchive.zip");
        /// }
        /// 
        /// // Extract a zip archive that uses AES Encryption.
        /// // You do not need to specify the algorithm during extraction.
        /// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
        /// {
        ///     zip.Password= "Top.Secret.No.Peeking!";
        ///     zip.ExtractAll("extractDirectory");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// ' Create a zip that uses Encryption.
        /// Using zip As New ZipFile()
        ///     zip.Encryption= EncryptionAlgorithm.WinZipAes256
        ///     zip.Password= "Top.Secret.No.Peeking!"
        ///     zip.AddFile("ReadMe.txt")
        ///     zip.AddFile("7440-N49th.png")
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf")
        ///     zip.Save("EncryptedArchive.zip")
        /// End Using
        /// 
        /// ' Extract a zip archive that uses AES Encryption.
        /// ' You do not need to specify the algorithm during extraction.
        /// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
        ///     zip.Password= "Top.Secret.No.Peeking!"
        ///     zip.ExtractAll("extractDirectory")
        /// End Using
        /// </code>
        ///
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.Password">ZipFile.Password</seealso>
        /// <seealso cref="ZipEntry.Encryption">ZipEntry.Encryption</seealso>
        public EncryptionAlgorithm Encryption
        {
            get
            {
                return _Encryption;
            }
            set
            {
                if (value == EncryptionAlgorithm.Unsupported)
                    throw new InvalidOperationException("You may not set Encryption to that value.");
                _Encryption= value;
            }
        }



        /// <summary>
        /// A callback that allows the application to specify whether multiple reads of the
        /// stream should be performed, in the case that a compression operation actually
        /// inflates the size of the file data.  
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// In some cases, applying the Deflate compression algorithm in
        /// <c>DeflateStream</c> can result an increase in the size of the data.  This
        /// "inflation" can happen with previously compressed files, such as a zip, jpg,
        /// png, mp3, and so on.  In a few tests, inflation on zip files can be as large
        /// as 60%!  Inflation can also happen with very small files.  In these cases,
        /// by default, the DotNetZip library discards the compressed bytes, and stores
        /// the uncompressed file data into the zip archive.  This is an optimization
        /// where smaller size is preferred over longer run times.
        /// </para>
        ///
        /// <para>
        /// The application can specify that compression is not even tried, by setting the
        /// ForceNoCompression flag.  In this case, the compress-and-check-sizes process as
        /// decribed above, is not done.
        /// </para>
        ///
        /// <para>
        /// In some cases, neither choice is optimal.  The application wants compression,
        /// but in some cases also wants to avoid reading the stream more than once.  This
        /// may happen when the stream is very large, or when the read is very expensive, or
        /// when the difference between the compressed and uncompressed sizes is not
        /// significant.
        /// </para>
        ///
        /// <para>
        /// To satisfy these applications, this delegate allows the DotNetZip library to ask
        /// the application to for approval for re-reading the stream, in the case where
        /// inflation occurs.  The callback is invoked only in the case of inflation; that
        /// is to say when the uncompressed stream is smaller than the compressed stream.
        /// </para>
        ///
        /// <para>
        /// As with other properties (like <see cref="Password"/> and <see
        /// cref="ForceNoCompression"/>), setting the corresponding delegate on a
        /// <c>ZipFile</c> instance will caused it to be applied to all ZipEntry items
        /// that are subsequently added to the <c>ZipFile</c> instance. In other words,
        /// if you set this callback after you have added files to the <c>ZipFile</c>,
        /// but before you have called Save(), those items will not be governed by the
        /// callback when you do call Save(). Your best bet is to set this callback
        /// before adding any entries.
        /// </para>
        ///
        /// <para>
        /// Of course, if you want to have different callbacks for different entries,
        /// you may do so.
        /// </para>
        ///
        /// </remarks>
        /// <example>
        /// <para>
        /// In this example, the application callback checks to see if the difference
        /// between the compressed and uncompressed data is greater than 25%.  If it is,
        /// then the callback returns true, and the application tells the library to
        /// re-read the stream.  If not, then the callback returns false, and the
        /// library just keeps the "inflated" file data.
        /// </para>
        ///
        /// <code>
        ///
        /// public bool ReadTwiceCallback(long uncompressed, long compressed, string filename)
        /// {
        ///     return ((uncompressed * 1.0/compressed) > 1.25);
        /// }
        /// 
        /// public void CreateTheZip()
        /// {
        ///     using (ZipFile zip = new ZipFile())
        ///     {
        ///         // set the callback before adding files to the zip
        ///         zip2.WillReadTwiceOnInflation = ReadTwiceCallback;
        ///         zip2.AddFile(filename1);
        ///         zip2.AddFile(filename2);
        ///         zip2.Save(ZipFileToCreate);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="Aspose.Zip.ZipFile.WantCompression"/>
        /// <seealso cref="Aspose.Zip.WantCompressionCallback"/>
        /// <seealso cref="ZipEntry.WillReadTwiceOnInflation"/>
        public ReReadApprovalCallback WillReadTwiceOnInflation
        {
            get { return _WillReadTwiceOnInflation; }
            set { _WillReadTwiceOnInflation = value; }
        }

        private ReReadApprovalCallback _WillReadTwiceOnInflation;

        /// <summary>
        /// A callback that allows the application to specify whether compression should
        /// be used for entries subsequently added to the zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// In some cases, applying the Deflate compression algorithm to an entry *may*
        /// result a slight increase in the size of the data.  This "inflation" can
        /// happen with previously compressed files, such as a zip, jpg, png, mp3, and
        /// so on; it results from adding DEFLATE framing data around incompressible data.
        /// Inflation can also happen with very small files. Applications may wish to
        /// avoid the use of compression in these cases. As well, applications may wish
        /// to avoid compression to save time.
        /// </para>
        ///
        /// <para>
        /// By default, the DotNetZip library takes this approach to decide whether to
        /// apply compression: first it applies a heuristic, to determine whether it
        /// should try to compress a file or not.  The library checks the extension of
        /// the entry, and if it is one of a known list of uncompressible file types
        /// (mp3, zip, docx, and others), the library will not attempt to compress the
        /// entry.  The library does not actually check the content of the entry.  If
        /// you name a text file "Text.mp3", and then attempt to add it to a zip
        /// archive, this library will, by default, not attempt to compress the entry,
        /// based on the extension of the filename.
        /// </para>
        ///
        /// <para>
        /// If this default behavior is not satisfactory, there are two options. First,
        /// the application can override it by setting this <see
        /// cref="ZipFile.WantCompression"/> callback.  This affords maximum control to
        /// the application.  With this callback, the application can supply its own
        /// logic for determining whether to apply the Deflate algorithm or not.  For
        /// example, an application may desire that files over 40mb in size are never
        /// compressed, or always compressed.  An application may desire that the first
        /// 7 entries added to an archive are compressed, and the remaining ones are
        /// not.  The WantCompression callback allows the application full control, on
        /// an entry-by-entry basis.
        /// </para>
        ///
        /// <para>
        /// The second option for overriding the default logic regarding whether to
        /// apply compression is the ForceNoCompression flag.  If this flag is set to
        /// true, the compress-and-check-sizes process as decribed above, is not done,
        /// nor is the callback invoked.  In other words, if you set ForceNoCompression
        /// to true, andalso set the WantCompression callback, only the
        /// ForceNoCompression flag is considered.
        /// </para>
        ///
        /// <para>
        /// This is how the library determines whether compression will be attempted for
        /// an entry.  If it is to be attempted, the library reads the entry, runs it
        /// through the deflate algorithm, and then checks the size of the result.  If
        /// applying the Deflate algorithm increases the size of the data, then the
        /// library discards the compressed bytes, re-reads the raw entry data, and
        /// stores the uncompressed file data into the zip archive, in compliance with
        /// the zip spec.  This is an optimization where smaller size is preferred over
        /// longer run times. The re-reading is gated on the <see
        /// cref="WillReadTwiceOnInflation"/> callback, if it is set. This callback
        /// applies independently of the WantCompression callback.
        /// </para>
        ///
        /// <para>
        /// If by the logic described above, compression is not to be attempted for an
        /// entry, the library reads the entry, and simply stores the entry data
        /// uncompressed.
        /// </para>
        ///
        /// <para>
        /// And, if you have read this far, I would like to point out that a single
        /// person wrote all the code that does what is described above, and also wrote
        /// the description.  Isn't it about time you <see
        /// href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">donated $5 in
        /// appreciation?</see> The money goes to a charity.
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="Aspose.Zip.ZipFile.WillReadTwiceOnInflation"/>
        public WantCompressionCallback WantCompression
        {
            get { return _WantCompression; }
            set { _WantCompression = value; }
        }

        private WantCompressionCallback _WantCompression;

        /// <summary>Provides a string representation of the instance.</summary>
        /// <returns>a string representation of the instance.</returns>
        public override String ToString()
        {
            return String.Format ("ZipFile/{0}", Name);
        }

        

        /// <summary>
        /// Returns the version number on the DotNetZip assembly.
        /// </summary>
        ///
        /// <remarks>
        /// This property is exposed as a convenience.  Callers
        /// could also get the version value by retrieving  GetName().Version 
        /// on the System.Reflection.Assembly object pointing to the
        /// DotNetZip assembly. But sometimes it is not clear which
        /// assembly is being loaded.  This property makes it clear. 
        /// </remarks>
        public static Version LibraryVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }


        internal void NotifyEntryChanged()
        {
            _contentsChanged = true;
        }



        internal Stream ReadStream
        {
            get
            {
                if (_readstream == null)
                {
                    if (_name != null)
                    {
                        try
                        {
                            _readstream = File.OpenRead(_name);
                            _ReadStreamIsOurs = true;
                        }
                        catch (IOException ioe)
                        {
                            throw new ZipException("Error opening the file", ioe);
                        }
                    }
                }
                return _readstream;
            }
        }



        // called by ZipEntry in ZipEntry.Extract(), when there is no stream set for the
        // ZipEntry.
        internal void Reset()
        {
            if (_JustSaved)
            {
                // read in the just-saved zip archive
                ZipFile x = new ZipFile();
                x._name = this._name;
                x.ProvisionalAlternateEncoding = this.ProvisionalAlternateEncoding;
                ReadIntoInstance(x);
                // copy the contents of the entries.
                // cannot just replace the entries - the app may be holding them
                foreach (ZipEntry e1 in x)
                {
                    foreach (ZipEntry e2 in this)
                    {
                        if (e1.FileName == e2.FileName)
                        {
                            e2.CopyMetaData(e1);
                        }
                    }
                }
                _JustSaved = false;
            }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ZipFile</c> instance, using the specified filename. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Applications can use this constructor to create a new ZipFile for writing, 
        /// or to slurp in an existing zip archive for read and update purposes. 
        /// </para>
        /// 
        /// <para>
        /// To create a new zip archive, an application can call this constructor,
        /// passing the name of a file that does not exist.  The name may be a fully
        /// qualified path. Then the application can add directories or files to the
        /// <c>ZipFile</c> via <c>AddDirectory()</c>, <c>AddFile()</c>, <c>AddItem()</c>
        /// and then write the zip archive to the disk by calling <c>Save()</c>. The zip
        /// file is not actually opened and written to the disk until the application
        /// calls <c>ZipFile.Save()</c>.  At that point the new zip file with the given
        /// name is created.
        /// </para>
        /// 
        /// <para>
        /// If you won't know the name of the <c>Zipfile</c> until the time you call
        /// <c>ZipFile.Save()</c>, or if you plan to save to a stream (which has no
        /// name), then you should use the no-argument constructor.
        /// </para>
        /// 
        /// <para>
        /// The application can also call this constructor to read an existing zip
        /// archive.  passing the name of a valid zip file that does exist. But, it's
        /// better form to use the static <see cref="ZipFile.Read(String)"/> method,
        /// passing the name of the zip file, because using <c>ZipFile.Read()</c> in
        /// your code communicates very clearly what you are doing.  In either case, the
        /// file is then read into the <c>ZipFile</c> instance.  The app can then
        /// enumerate the entries or can modify the zip file, for example adding
        /// entries, removing entries, changing comments, and so on.
        /// </para>
        /// 
        /// <para>
        /// One advantage to this parameterized constructor: it allows applications to
        /// use the same code to add items to a zip archive, regardless of whether the
        /// zip file exists.
        /// </para>
        /// 
        /// <para>
        /// Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        /// not party on a single instance with multiple threads.  You may have multiple
        /// threads that each use a distinct <c>ZipFile</c> instance, or you can
        /// synchronize multi-thread access to a single instance.
        /// </para>
        /// 
        /// <para>
        /// By the way, since DotNetZip is so easy to use, don't you think <see
        /// href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">you should donate
        /// $5 or $10</see>?
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="ZipException">
        /// Thrown if name refers to an existing file that is not a valid zip file. 
        /// </exception>
        ///
        /// <example>
        /// This example shows how to create a zipfile, and add a few files into it. 
        /// <code>
        /// String ZipFileToCreate = "archive1.zip";
        /// String DirectoryToZip  = "c:\\reports";
        /// using (ZipFile zip = new ZipFile())
        /// { 
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames, "files");
        ///   zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        /// Dim ZipFileToCreate As String = "archive1.zip"
        /// Dim DirectoryToZip As String = "c:\reports"
        /// Using zip As ZipFile = New ZipFile()
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames, "files")
        ///     zip.Save(ZipFileToCreate)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        ///
        public ZipFile(string fileName)
        {
            try
            {
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }


        /// <summary>
        /// Creates a new <c>ZipFile</c> instance, using the specified name for the
        /// filename, and the specified Encoding.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// See the documentation on the <see cref="ZipFile(String)">ZipFile constructor
        /// that accepts a single string argument</see> for basic information on all the
        /// <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// <para>
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// This is equivalent to setting the <see cref="ProvisionalAlternateEncoding"/>
        /// property on the <c>ZipFile</c> instance after construction.
        /// </para>
        ///
        /// <para>
        /// Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        /// not party on a single instance with multiple threads.  You may have multiple
        /// threads that each use a distinct <c>ZipFile</c> instance, or you can
        /// synchronize multi-thread access to a single instance.
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <exception cref="ZipException">
        /// Thrown if name refers to an existing file that is not a valid zip file. 
        /// </exception>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="encoding">The Encoding is used as the default alternate 
        /// encoding for entries with filenames or comments that cannot be encoded 
        /// with the IBM437 code page. </param>
        public ZipFile(string fileName, System.Text.Encoding encoding)
        {
            try
            {
                _InitInstance(fileName, null);
                ProvisionalAlternateEncoding = encoding;
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }

        /// <summary>
        /// Create a zip file, without specifying a target filename or stream to save to. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// See the documentation on the <see cref="ZipFile(String)">ZipFile constructor
        /// that accepts a single string argument</see> for basic information on all the
        /// <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// <para> After instantiating with this constructor and adding entries to the
        /// archive, the application should call <see cref="ZipFile.Save(String)"/> or
        /// <see cref="ZipFile.Save(System.IO.Stream)"/> to save to a file or a stream,
        /// respectively.  The application can also set the <see cref="Name"/> property
        /// and then call the no-argument <see cref="Save()"/> method.  (This is the
        /// preferred approach for applications that use the library through COM
        /// interop.)  If you call the no-argument <see cref="Save()"/> method without
        /// having set the <c>Name</c> of the <c>ZipFile</c>, either through the
        /// parameterized constructor or through the explicit property , the Save() will
        /// throw, because there is no place to save the file.  </para>
        ///
        /// <para>
        /// Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        /// have multiple threads that each use a distinct <c>ZipFile</c> instance, or
        /// you can synchronize multi-thread access to a single instance.  </para>
        /// 
        /// </remarks>
        /// 
        /// <example>
        /// This example creates a Zip archive called Backup.zip, containing all the files
        /// in the directory DirectoryToZip. Files within subdirectories are not zipped up.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// { 
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   // note: this code does not recurse subdirectories!
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames, "files");
        ///   zip.Save("Backup.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     ' note: this code does not recurse subdirectories!
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames, "files")
        ///     zip.Save("Backup.zip")
        /// End Using
        /// </code>
        /// </example>
        public ZipFile()
        {
            _InitInstance(null, null);
        }


        /// <summary>
        /// Create a zip file, specifying a text Encoding, but without specifying a target
        /// filename or stream to save to.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// See the documentation on the <see cref="ZipFile(String)">ZipFile constructor
        /// that accepts a single string argument</see> for basic information on all the
        /// <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="encoding">
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// </param>
        public ZipFile(System.Text.Encoding encoding)
        {
            _InitInstance(null, null);
            ProvisionalAlternateEncoding = encoding;
        }


        /// <summary>
        /// Creates a new <c>ZipFile</c> instance, using the specified name for the
        /// filename, and the specified status message writer.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// See the documentation on the <see cref="ZipFile(String)">ZipFile constructor
        /// that accepts a single string argument</see> for basic information on all the
        /// <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// <para>
        /// This version of the constructor allows the caller to pass in a TextWriter,
        /// to which verbose messages will be written during extraction or creation of
        /// the zip archive.  A console application may wish to pass System.Console.Out
        /// to get messages on the Console. A graphical or headless application may wish
        /// to capture the messages in a different <c>TextWriter</c>, for example, a
        /// <c>StringWriter</c>, and then display the messages in a TextBox, or generate
        /// an audit log of ZipFile operations.
        /// </para>
        /// 
        /// <para>
        /// To encrypt the data for the files added to the <c>ZipFile</c> instance, set
        /// the Password property after creating the <c>ZipFile</c> instance.
        /// </para>
        /// 
        /// <para>
        /// Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        /// not party on a single instance with multiple threads.  You may have multiple
        /// threads that each use a distinct <c>ZipFile</c> instance, or you can
        /// synchronize multi-thread access to a single instance.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <exception cref="ZipException">
        /// Thrown if name refers to an existing file that is not a valid zip file. 
        /// </exception>
        ///
        /// <example>
        /// <code>
        /// using (ZipFile zip = new ZipFile("Backup.zip", Console.Out))
        /// { 
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   // note: this code does not recurse subdirectories!
        ///   // Status messages will be written to Console.Out
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames);
        ///   zip.Save();
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile("Backup.zip", Console.Out)
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     ' note: this code does not recurse subdirectories!
        ///     ' Status messages will be written to Console.Out
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames)
        ///     zip.Save()
        /// End Using
        /// </code>
        /// </example>
        /// 
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="statusMessageWriter">A TextWriter to use for writing 
        /// verbose status messages.</param>
        public ZipFile(string fileName, TextWriter statusMessageWriter)
        {
            try
            {
                _InitInstance(fileName, statusMessageWriter);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }


        /// <summary>
        /// Creates a new <c>ZipFile</c> instance, using the specified name for the
        /// filename, the specified status message writer, and the specified Encoding.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This constructor works like the <see cref="ZipFile(String)">ZipFile
        /// constructor that accepts a single string argument.</see> See that reference
        /// for detail on what this constructor does.
        /// </para>
        ///
        /// <para>
        /// This version of the constructor allows the caller to pass in a TextWriter,
        /// and an Encoding.  The TextWriter will collect verbose messages that are
        /// generated by the library during extraction or creation of the zip archive.
        /// A console application may wish to pass System.Console.Out to get messages on
        /// the Console. A graphical or headless application may wish to capture the
        /// messages in a different <c>TextWriter</c>, for example, a
        /// <c>StringWriter</c>, and then display the messages in a TextBox, or generate
        /// an audit log of ZipFile operations.
        /// </para>
        /// 
        /// <para>
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// This is a equivalent to setting the <see
        /// cref="ProvisionalAlternateEncoding"/> property on the <c>ZipFile</c>
        /// instance after construction.
        /// </para>
        /// 
        /// <para>
        /// To encrypt the data for the files added to the <c>ZipFile</c> instance, set
        /// the Password property after creating the <c>ZipFile</c> instance.
        /// </para>
        /// 
        /// <para>
        /// Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        /// not party on a single instance with multiple threads.  You may have multiple
        /// threads that each use a distinct ZipFile instance, or you can synchronize
        /// multi-thread access to a single instance.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <exception cref="ZipException">
        /// Thrown if name refers to an existing file that is not a valid zip file. 
        /// </exception>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="statusMessageWriter">A TextWriter to use for writing verbose 
        /// status messages.</param>
        /// <param name="encoding">
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// </param>
        public ZipFile(string fileName, TextWriter statusMessageWriter,
                       System.Text.Encoding encoding)
        {
            try
            {
                _InitInstance(fileName, statusMessageWriter);
                ProvisionalAlternateEncoding = encoding;
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }




        /// <summary>
        /// Initialize a <c>ZipFile</c> instance by reading in a zip file.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        /// This method is primarily useful from COM Automation environments, when
        /// reading or extracting zip files. In COM, it is not possible to invoke
        /// parameterized constructors for a class. A COM Automation application can
        /// update a zip file by using the default (no argument) constructor, then
        /// calling Initialize() to read the contents of an on-disk zip archive into the
        /// <c>ZipFile</c> instance.
        /// </para>
        ///
        /// <para>
        /// .NET applications are encouraged to use the <c>ZipFile.Read()</c> methods for
        /// better clarity.
        /// </para>
        ///
        /// </remarks>
        /// <param name="fileName">the name of the existing zip file to read in.</param>
        public void Initialize(string fileName)
        {
            try
            {
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }


        private void _InitInstance(string zipFileName, TextWriter statusMessageWriter)
        {
            // create a new zipfile
            _name = zipFileName;
            _StatusMessageTextWriter = statusMessageWriter;
            _contentsChanged = true;
            CompressionLevel = CompressionLevel.Default;
            // workitem 7685
            _entries = new List<ZipEntry>();
            if (File.Exists(_name))
            {
                if (FullScan)
                    ReadIntoInstance_Orig(this);
                else
                    ReadIntoInstance(this);
                _fileAlreadyExists = true;
            }

            return;
        }
        #endregion



        #region Indexers and Collections


        /// <summary>
        /// This is an integer indexer into the Zip archive.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This property is read-write. But don't get too excited: When setting the
        /// value, the only legal value is <c>null</c> (<c>Nothing</c> in VB). If you
        /// assign a non-null value, the setter will throw an exception.
        /// </para>
        ///
        /// <para>
        /// Setting the value to <c>null</c> is equivalent to calling <see
        /// cref="ZipFile.RemoveEntry(String)"/> with the filename for the given entry.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentException">
        /// Thrown if the caller attempts to assign a non-null value to the indexer, 
        /// or if the caller uses an out-of-range index value.
        /// </exception>
        ///
        /// <param name="ix">
        /// The index value.
        /// </param>
        /// 
        /// <returns>
        /// The <c>ZipEntry</c> within the Zip archive at the specified index. If the 
        /// entry does not exist in the archive, this indexer throws.
        /// </returns>
        /// 
        public ZipEntry this[int ix]
        {
            // workitem 6402
            get
            {
                return (ZipEntry) _entries[ix];
            }

            set
            {
                if (value != null)
                    throw new ZipException("You may not set this to a non-null ZipEntry value.",
                                           new ArgumentException("this[int]"));
                RemoveEntry((ZipEntry) _entries[ix]);
            }
        }


        /// <summary>
        /// This is a name-based indexer into the Zip archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Retrieval by the string-based indexer is done on a case-insensitive basis,
        /// by default.  Set the <see cref="CaseSensitiveRetrieval"/> property to use
        /// case-sensitive comparisons.
        /// </para>
        ///
        /// <para>
        /// This property is read-write. When setting the value, the only legal value is
        /// <c>null</c> (<c>Nothing</c> in VB). Setting the value to <c>null</c> is
        /// equivalent to calling <see cref="ZipFile.RemoveEntry(String)"/> with the
        /// filename.
        /// </para>
        ///
        /// <para>
        /// If you assign a non-null value, the setter will throw an exception.
        /// </para>
        ///
        /// <para>
        /// It is can be true that <c>this[value].FileName == value</c>, but not
        /// always. In other words, the <c>FileName</c> property of the <c>ZipEntry</c>
        /// you retrieve with this indexer, can be equal to the index value, but not
        /// always.  In the case of directory entries in the archive, you may retrieve
        /// them with the name of the directory with no trailing slash, even though in
        /// the entry itself, the actual <see cref="ZipEntry.FileName"/> property may
        /// include a trailing slash.  In other words, for a directory entry named
        /// "dir1", you may find <c>zip["dir1"].FileName == "dir1/"</c>. Also, for any
        /// entry with slashes, they are stored in the zip file as forward slashes, but
        /// you may retrieve them with either forward or backslashes.  So,
        /// <c>zip["dir1\\entry1.txt"].FileName == "dir1/entry.txt"</c>.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// This example extracts only the entries in a zip file that are .txt files.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
        /// {
        ///   foreach (string s1 in zip.EntryFilenames)
        ///   {
        ///     if (s1.EndsWith(".txt"))
        ///       zip[s1].Extract("textfiles");
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip")
        ///       Dim s1 As String
        ///       For Each s1 In zip.EntryFilenames
        ///           If s1.EndsWith(".txt") Then
        ///               zip(s1).Extract("textfiles")
        ///           End If
        ///       Next
        ///   End Using
        /// </code>
        /// </example>
        /// <seealso cref="Aspose.Zip.ZipFile.RemoveEntry(string)"/>
        ///
        /// <exception cref="System.ArgumentException">
        /// Thrown if the caller attempts to assign a non-null value to the indexer.
        /// </exception>
        ///
        /// <param name="fileName">
        /// The name of the file, including any directory path, to retrieve from the zip. 
        /// The filename match is not case-sensitive by default; you can use the
        /// <see cref="CaseSensitiveRetrieval"/> property to change this behavior. The
        /// pathname can use forward-slashes or backward slashes.
        /// </param>
        /// 
        /// <returns>
        /// The <c>ZipEntry</c> within the Zip archive, given by the specified
        /// filename. If the named entry does not exist in the archive, this indexer
        /// returns <c>null</c> (<c>Nothing</c> in VB).
        /// </returns>
        /// 
        public ZipEntry this[String fileName]
        {
            get
            {
                foreach (ZipEntry e in _entries)
                {
                    if (CaseSensitiveRetrieval)
                    {
                        // check for the file match with a case-sensitive comparison.
                        if (e.FileName == fileName) return e;
                        // also check for equivalence
                        if (fileName.Replace("\\", "/") == e.FileName) return e;
                        if (e.FileName.Replace("\\", "/") == fileName) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/", StringComparison.Ordinal))
                        {
                            string fileNameNoSlash = e.FileName.Trim("/".ToCharArray());
                            if (fileNameNoSlash == fileName) return e;
                            // also check for equivalence
                            if (fileName.Replace("\\", "/") == fileNameNoSlash) return e;
                            if (fileNameNoSlash.Replace("\\", "/") == fileName) return e;
                        }

                    }
                    else
                    {
                        // check for the file match in a case-insensitive manner.
                        if (StringUtil.EqualsIgnoreCase(e.FileName, fileName)) return e;
                        // also check for equivalence
                        if (StringUtil.EqualsIgnoreCase(fileName.Replace("\\", "/"), e.FileName)) return e;
                        if (StringUtil.EqualsIgnoreCase(e.FileName.Replace("\\", "/"), fileName)) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/", StringComparison.Ordinal))
                        {
                            string fileNameNoSlash = e.FileName.Trim("/".ToCharArray());

                            if (StringUtil.EqualsIgnoreCase(fileNameNoSlash, fileName)) return e;
                            // also check for equivalence
                            if (StringUtil.EqualsIgnoreCase(fileName.Replace("\\", "/"), fileNameNoSlash)) return e;
                            if (StringUtil.EqualsIgnoreCase(fileNameNoSlash.Replace("\\", "/"), fileName)) return e;

                        }

                    }

                }
                return null;
            }

            set
            {
                if (value != null)
                    throw new ArgumentException("You may not set this to a non-null ZipEntry value.");
                RemoveEntry(fileName);
            }
        }

        /// <summary>
        /// The list of filenames for the entries contained within the zip archive.  
        /// </summary>
        ///
        /// <remarks>
        /// According to the ZIP specification, the names of the entries use forward
        /// slashes in pathnames.  If you are scanning through the list, you may have to
        /// swap forward slashes for backslashes.
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.this[string]"/>
        ///
        /// <example>
        /// This example shows one way to test if a filename is already contained within 
        /// a zip archive.
        /// <code>
        /// String ZipFileToRead= "PackedDocuments.zip";
        /// string Candidate = "DatedMaterial.xps";
        /// using (ZipFile zip = new ZipFile(ZipFileToRead))
        /// {
        ///   if (zip.EntryFilenames.Contains(Candidate))
        ///     Console.WriteLine("The file '{0}' exists in the zip archive '{1}'",
        ///                       Candidate,
        ///                       ZipFileName);
        ///   else
        ///     Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'",
        ///                       Candidate,
        ///                       ZipFileName);
        ///   Console.WriteLine();
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Dim ZipFileToRead As String = "PackedDocuments.zip"
        ///   Dim Candidate As String = "DatedMaterial.xps"
        ///   Using zip As New ZipFile(ZipFileToRead)
        ///       If zip.EntryFilenames.Contains(Candidate) Then
        ///           Console.WriteLine("The file '{0}' exists in the zip archive '{1}'", _
        ///                       Candidate, _
        ///                       ZipFileName)
        ///       Else
        ///         Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'", _
        ///                       Candidate, _
        ///                       ZipFileName)
        ///       End If
        ///       Console.WriteLine
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <returns>
        /// The list of strings for the filenames contained within the Zip archive.
        /// </returns>
        /// 
        public List<string> EntryFileNames
        {
            get
            {
                List<string> foo = new List<string>();
                foreach (ZipEntry e in _entries)
                    foo.Add(e.FileName);
                return foo;
            }
        }


        /// <summary>
        /// Returns the readonly collection of entries in the Zip archive.
        /// </summary>
        /// <remarks>
        /// If there are no entries in the current ZipFile, the value returned is a
        /// non-null zero-element collection.
        /// </remarks>
        public List<ZipEntry> Entries
        {
            get
            {
                return _entries;
            }
        }


        /// <summary>
        /// Returns the number of entries in the Zip archive.
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }



        /// <summary>
        /// Removes the given ZipEntry from the zip archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
        /// make the changes permanent.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="System.ArgumentException">
        /// Thrown if the specified ZipEntry does not exist in the <c>ZipFile</c>.
        /// </exception>
        ///
        /// <example>
        /// In this example, all entries in the zip archive dating from before December
        /// 31st, 2007, are removed from the archive.  This is actually much easier if
        /// you use the RemoveSelectedEntries method.  But I needed an example for
        /// RemoveEntry, so here it is.
        /// <code>
        /// String ZipFileToRead = "ArchiveToModify.zip";
        /// System.DateTime Threshold = new System.DateTime(2007,12,31);
        /// using (ZipFile zip = ZipFile.Read(ZipFileToRead))
        /// {
        ///   var EntriesToRemove = new System.Collections.Generic.List&lt;ZipEntry&gt;();
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (e.LastModified &lt; Threshold)
        ///     {
        ///       // We cannot remove the entry from the list, within the context of 
        ///       // an enumeration of said list.
        ///       // So we add the doomed entry to a list to be removed later.
        ///       EntriesToRemove.Add(e);
        ///     }
        ///   }
        ///   
        ///   // actually remove the doomed entries. 
        ///   foreach (ZipEntry zombie in EntriesToRemove)
        ///     zip.RemoveEntry(zombie);
        ///   
        ///   zip.Comment= String.Format("This zip archive was updated at {0}.", 
        ///                              System.DateTime.Now.ToString("G"));
        ///
        ///   // save with a different name
        ///   zip.Save("Archive-Updated.zip");
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        ///   Dim ZipFileToRead As String = "ArchiveToModify.zip"
        ///   Dim Threshold As New DateTime(2007, 12, 31)
        ///   Using zip As ZipFile = ZipFile.Read(ZipFileToRead)
        ///       Dim EntriesToRemove As New System.Collections.Generic.List(Of ZipEntry)
        ///       Dim e As ZipEntry
        ///       For Each e In zip
        ///           If (e.LastModified &lt; Threshold) Then
        ///               ' We cannot remove the entry from the list, within the context of 
        ///               ' an enumeration of said list.
        ///               ' So we add the doomed entry to a list to be removed later.
        ///               EntriesToRemove.Add(e)
        ///           End If
        ///       Next
        ///   
        ///       ' actually remove the doomed entries. 
        ///       Dim zombie As ZipEntry
        ///       For Each zombie In EntriesToRemove
        ///           zip.RemoveEntry(zombie)
        ///       Next
        ///       zip.Comment = String.Format("This zip archive was updated at {0}.", DateTime.Now.ToString("G"))
        ///       'save as a different name
        ///       zip.Save("Archive-Updated.zip")
        ///   End Using
        /// </code>
        /// </example>
        /// 
        /// <param name="entry">
        /// The <c>ZipEntry</c> to remove from the zip. 
        /// </param>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.RemoveSelectedEntries(string)"/>
        ///
        public void RemoveEntry(ZipEntry entry)
        {
            if (!_entries.Contains(entry))
                throw new ArgumentException("The entry you specified does not exist in the zip archive.");

            _entries.Remove(entry);

#if NOTNEEDED
            if (_direntries != null)
            {
                bool FoundAndRemovedDirEntry = false;
                foreach (ZipDirEntry de1 in _direntries)
                {
                    if (entry.FileName == de1.FileName)
                    {
                        _direntries.Remove(de1);
                        FoundAndRemovedDirEntry = true;
                        break;
                    }
                }

                if (!FoundAndRemovedDirEntry)
                    throw new BadStateException("The entry to be removed was not found in the directory.");
            }
#endif
            _contentsChanged = true;
        }




        /// <summary>
        /// Removes the <c>ZipEntry</c> with the given filename from the zip archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
        /// make the changes permanent.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the <c>ZipFile</c> is not updatable. 
        /// </exception>
        ///
        /// <exception cref="System.ArgumentException">
        /// Thrown if a ZipEntry with the specified filename does not exist in the <c>ZipFile</c>.
        /// </exception>
        ///
        /// <example>
        /// This example shows one way to remove an entry with a given filename from an 
        /// existing zip archive.
        /// <code>
        /// String ZipFileToRead= "PackedDocuments.zip";
        /// string Candidate = "DatedMaterial.xps";
        /// using (ZipFile zip = new ZipFile(ZipFileToRead))
        /// {
        ///   if (zip.EntryFilenames.Contains(Candidate))
        ///   {
        ///     zip.RemoveEntry(Candidate);
        ///     zip.Comment= String.Format("The file '{0}' has been removed from this archive.", 
        ///                                Candidate);
        ///     zip.Save();
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Dim ZipFileToRead As String = "PackedDocuments.zip"
        ///   Dim Candidate As String = "DatedMaterial.xps"
        ///   Using zip As ZipFile = New ZipFile(ZipFileToRead)
        ///       If zip.EntryFilenames.Contains(Candidate) Then
        ///           zip.RemoveEntry(Candidate)
        ///           zip.Comment = String.Format("The file '{0}' has been removed from this archive.", Candidate)
        ///           zip.Save
        ///       End If
        ///   End Using
        /// </code>
        /// </example>
        /// 
        /// <param name="fileName">
        /// The name of the file, including any directory path, to remove from the zip. 
        /// The filename match is not case-sensitive by default; you can use the
        /// <c>CaseSensitiveRetrieval</c> property to change this behavior. The
        /// pathname can use forward-slashes or backward slashes.
        /// </param>
        /// 
        public void RemoveEntry(String fileName)
        {
            string modifiedName = ZipEntry.NameInArchive(fileName, null);
            ZipEntry e = this[modifiedName];
            if (e == null)
                throw new ArgumentException("The entry you specified was not found in the zip archive.");

            RemoveEntry(e);
        }


        #endregion

        #region Destructors and Disposers

        /// <summary>
        /// This is the class Destructor, which gets called implicitly when the instance
        /// is destroyed.  Because the <c>ZipFile</c> type implements IDisposable, this
        /// method calls Dispose(false).
        /// </summary>
        ~ZipFile()
        {
            // call Dispose with false.  Since we're in the
            // destructor call, the managed resources will be
            // disposed of anyways.
            Dispose(false);
        }

        /// <summary>
        /// Handles closing of the read and write streams associated
        /// to the <c>ZipFile</c>, if necessary.  
        /// </summary>
        ///
        /// <remarks>
        /// The Dispose() method is generally 
        /// employed implicitly, via a using() {} statement. (Using...End Using in VB)
        /// Always use a using statement, or always insure that you are calling Dispose() 
        /// explicitly.
        /// </remarks>
        ///
        /// <example>
        /// This example extracts an entry selected by name, from the Zip file to the
        /// Console.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(zipfile))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (WantThisEntry(e.FileName)) 
        ///       zip.Extract(e.FileName, Console.OpenStandardOutput());
        ///   }
        /// } // Dispose() is called implicitly here.
        /// </code>
        /// 
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(zipfile)
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///       If WantThisEntry(e.FileName) Then
        ///           zip.Extract(e.FileName, Console.OpenStandardOutput())
        ///       End If
        ///     Next
        /// End Using ' Dispose is implicity called here
        /// </code>
        /// </example>
        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The Dispose() method.  It disposes any managed resources, 
        /// if the flag is set, then marks the instance disposed.
        /// This method is typically not called from application code.
        /// </summary>
        /// <param name="disposeManagedResources">indicates whether the
        /// method should dispose streams or not.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources
                    if (_ReadStreamIsOurs)
                    {
                        if (_readstream != null)
                        {
                            // workitem 7704
                            _readstream.Close();
                            _readstream = null;
                        }
                    }
                    // only dispose the writestream if there is a backing file 
                    //(_temporaryFileName is not null)
                    if ((_temporaryFileName != null) && (_name != null))
                        if (_writestream != null)
                        {
                            // workitem 7704
                            _writestream.Close();
                            _writestream = null;
                        }
                }
                _disposed = true;
            }
        }
        #endregion


        #region private properties

        private Stream WriteStream
        {
            get
            {
                if (_writestream == null)
                {
                    if (_name != null)
                    {

                        if (TempFileFolder == ".")
                            _temporaryFileName = SharedUtilities.GetTempFilename();
                        else if (TempFileFolder != null)
                            _temporaryFileName = Path.Combine(TempFileFolder, SharedUtilities.GetTempFilename());
                        else // null
                        {
                            string d = Path.GetDirectoryName(_name);
                            _temporaryFileName = Path.Combine(d, SharedUtilities.GetTempFilename());
                        }
                        _writestream = new FileStream(_temporaryFileName, FileMode.CreateNew);
                    }
                }
                return _writestream;
            }
            set
            {
                if (value != null)
                    throw new ZipException("Whoa!", new ArgumentException("Cannot set the stream to a non-null value.", "value"));
                _writestream = null;
            }
        }
        #endregion

        #region private fields
        private TextWriter _StatusMessageTextWriter;
        private bool _CaseSensitiveRetrieval;
        private Stream _readstream;
        private Stream _writestream;
        private bool _disposed;
        private List<ZipEntry> _entries;
        private bool _ForceNoCompression;
        private string _name;
        private string _Comment;
        internal string _Password;
        private bool _emitNtfsTimes = true;
        private bool _emitUnixTimes;
        private Aspose.Zip.CompressionStrategy _Strategy = CompressionStrategy.Default;
        private long _originPosition; 
        private bool _fileAlreadyExists;
        private string _temporaryFileName;
        private bool _contentsChanged;
        private bool _hasBeenSaved;
        private String _TempFileFolder;
        private bool _ReadStreamIsOurs = true;
        private object LOCK = new object();
        private bool _saveOperationCanceled;
        private bool _extractOperationCanceled;
        private EncryptionAlgorithm _Encryption;
        private bool _JustSaved;
        private bool _NeedZip64CentralDirectory;
        private long _locEndOfCDS = -1;
        private NullableBool _OutputUsesZip64;
        internal bool _inExtractAll;
        private System.Text.Encoding _provisionalAlternateEncoding = System.Text.Encoding.GetEncoding("IBM437"); // default = IBM437

        private int _BufferSize = 8192;
        
        internal Zip64Option _zip64 = Zip64Option.Default;

        #endregion

        #region AddUpdate

        /// <summary>
        /// Adds an item, either a file or a directory, to a zip file archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is handy if you are adding things to zip archive and don't want
        /// to bother distinguishing between directories or files.  Any files are added
        /// as single entries.  A directory added through this method is added
        /// recursively: all files and subdirectories contained within the directory are
        /// added to the <c>ZipFile</c>.
        /// </para>
        /// 
        /// <para>
        /// The name of the item may be a relative path or a fully-qualified
        /// path. Remember, the items contained in <c>ZipFile</c> instance get written
        /// to the disk only when you call ZipFile.Save() or a similar save method.
        /// </para>
        ///
        /// <para>
        /// The directory name used for the file within the archive is the same as the
        /// directory name (potentially a relative path) specified in the
        /// fileOrDirectoryName.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>,
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <overloads>This method has two overloads.</overloads>
        /// <param name="fileOrDirectoryName">
        /// the name of the file or directory to add.</param>
        /// 
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddItem(string fileOrDirectoryName)
        {
            return AddItem(fileOrDirectoryName, null);
        }


        /// <summary>
        /// Adds an item, either a file or a directory, to a zip file archive, 
        /// explicitly specifying the directory path to be used in the archive. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// If adding a directory, the add is recursive on all files and subdirectories 
        /// contained within it. 
        /// </para>
        /// <para>
        /// The name of the item may be a relative path or a fully-qualified path.
        /// The item added by this call to the <c>ZipFile</c> is not written to the zip file
        /// archive until the application calls Save() on the <c>ZipFile</c>. 
        /// </para>
        /// 
        /// <para>
        /// This version of the method allows the caller to explicitly specify the 
        /// directory path to be used in the archive, which would override the 
        /// "natural" path of the filesystem file.
        /// </para>
        /// 
        /// <para>
        /// Encryption will be used on the file data if the Password
        /// has been set on the <c>ZipFile</c> object, prior to calling this method.
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see cref="Password"/>,
        /// <see cref="WantCompression"/>, <see cref="ProvisionalAlternateEncoding"/>, 
        /// <see cref="ExtractExistingFile"/>, <see cref="ZipErrorAction"/>, and <see
        /// cref="ForceNoCompression"/>, their respective values at the time of this call will be
        /// applied to the <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if the file or directory passed in does not exist. 
        /// </exception>
        ///
        /// <param name="fileOrDirectoryName">the name of the file or directory to add.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// The name of the directory path to use within the zip archive.  This path
        /// need not refer to an extant directory in the current filesystem.  If the
        /// files within the zip are later extracted, this is the path used for the
        /// extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the
        /// path on the fileOrDirectoryName.  Passing the empty string ("") will insert
        /// the item at the root path within the archive.
        /// </param>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string, string)"/>
        ///
        /// <example>
        /// This example shows how to zip up a set of files into a flat hierarchy,
        /// regardless of where in the filesystem the files originated. The resulting
        /// zip archive will contain a toplevel directory named "flat", which itself
        /// will contain files Readme.txt, MyProposal.docx, and Image1.jpg.  A
        /// subdirectory under "flat" called SupportFiles will contain all the files in
        /// the "c:\SupportFiles" directory on disk.
        /// 
        /// <code>
        /// String[] itemnames= { 
        ///   "c:\\fixedContent\\Readme.txt",
        ///   "MyProposal.docx",
        ///   "c:\\SupportFiles",  // a directory
        ///   "images\\Image1.jpg"
        /// };
        ///
        /// try
        /// {
        ///   using (ZipFile zip = new ZipFile())
        ///   {
        ///     for (int i = 1; i &lt; itemnames.Length; i++)
        ///     {
        ///       // will add Files or Dirs, recurses and flattens subdirectories
        ///       zip.AddItem(itemnames[i],"flat"); 
        ///     }
        ///     zip.Save(ZipToCreate);
        ///   }
        /// }
        /// catch (System.Exception ex1)
        /// {
        ///   System.Console.Error.WriteLine("exception: {0}", ex1);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        ///   Dim itemnames As String() = _
        ///     New String() { "c:\fixedContent\Readme.txt", _
        ///                    "MyProposal.docx", _
        ///                    "SupportFiles", _
        ///                    "images\Image1.jpg" }
        ///   Try 
        ///       Using zip As New ZipFile
        ///           Dim i As Integer
        ///           For i = 1 To itemnames.Length - 1
        ///               ' will add Files or Dirs, recursing and flattening subdirectories.
        ///               zip.AddItem(itemnames(i), "flat")
        ///           Next i
        ///           zip.Save(ZipToCreate)
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1.ToString())
        ///   End Try
        /// </code>
        /// </example>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddItem(String fileOrDirectoryName, String directoryPathInArchive)
        {
            if (File.Exists(fileOrDirectoryName))
                return AddFile(fileOrDirectoryName, directoryPathInArchive);
            
            if (Directory.Exists(fileOrDirectoryName))
                return AddDirectory(fileOrDirectoryName, directoryPathInArchive);

            throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!",
                                                          fileOrDirectoryName));
        }

        /// <summary>
        /// Adds a File to a Zip file archive. 
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        /// The file added by this call to the <c>ZipFile</c> is not written to the zip
        /// file archive until the application calls Save() on the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        /// This method will throw an Exception if an entry with the same name already
        /// exists in the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <example>
        /// <para>
        /// In this example, three files are added to a Zip archive. The ReadMe.txt file
        /// will be placed in the root of the archive. The .png file will be placed in a
        /// folder within the zip called photos\personal.  The pdf file will be included
        /// into a folder within the zip called Desktop.
        /// </para>
        /// <code>
        ///    try
        ///    {
        ///      using (ZipFile zip = new ZipFile())
        ///      {
        ///        zip.AddFile("c:\\photos\\personal\\7440-N49th.png");
        ///        zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf");
        ///        zip.AddFile("ReadMe.txt");
        ///
        ///        zip.Save("Package.zip");
        ///      }
        ///    }
        ///    catch (System.Exception ex1)
        ///    {
        ///      System.Console.Error.WriteLine("exception: " + ex1);
        ///    }
        /// </code>
        /// 
        /// <code lang="VB">
        ///  Try 
        ///       Using zip As ZipFile = New ZipFile
        ///           zip.AddFile("c:\photos\personal\7440-N49th.png")
        ///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf")
        ///           zip.AddFile("ReadMe.txt")
        ///           zip.Save("Package.zip")
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1.ToString)
        ///   End Try
        /// </code>
        /// </example>
        /// 
        /// <overloads>This method has two overloads.</overloads>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string)"/>
        ///
        /// <param name="fileName">
        /// The name of the file to add. It should refer to a file in the filesystem.  
        /// The name of the file may be a relative path or a fully-qualified path. 
        /// </param>
        /// <returns>The <c>ZipEntry</c> corresponding to the File added.</returns>
        public ZipEntry AddFile(string fileName)
        {
            return AddFile(fileName, null);
        }





        /// <summary>
        /// Adds a File to a Zip file archive, potentially overriding the path to be used
        /// within the zip archive.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The file added by this call to the <c>ZipFile</c> is not written to the zip file
        /// archive until the application calls Save() on the <c>ZipFile</c>. 
        /// </para>
        /// 
        /// <para>
        /// This method will throw an Exception if an entry with the same name already exists
        /// in the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        /// This version of the method allows the caller to explicitly specify the 
        /// directory path to be used in the archive. 
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see cref="Password"/>,
        /// <see cref="WantCompression"/>, <see cref="ProvisionalAlternateEncoding"/>, 
        /// <see cref="ExtractExistingFile"/>, <see cref="ZipErrorAction"/>, and <see
        /// cref="ForceNoCompression"/>, their respective values at the time of this call will be
        /// applied to the <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <example>
        /// <para>
        /// In this example, three files are added to a Zip archive. The ReadMe.txt file
        /// will be placed in the root of the archive. The .png file will be placed in a
        /// folder within the zip called images.  The pdf file will be included into a
        /// folder within the zip called files\docs, and will be encrypted with the
        /// given password.
        /// </para>
        /// <code>
        /// try
        /// {
        ///   using (ZipFile zip = new ZipFile())
        ///   {
        ///     // the following entry will be inserted at the root in the archive.
        ///     zip.AddFile("c:\\datafiles\\ReadMe.txt", "");
        ///     // this image file will be inserted into the "images" directory in the archive.
        ///     zip.AddFile("c:\\photos\\personal\\7440-N49th.png", "images");
        ///     // the following will result in a password-protected file called 
        ///     // files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
        ///     zip.Password = "EncryptMe!";
        ///     zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf", "files\\docs");
        ///     zip.Save("Archive.zip");
        ///   }
        /// }
        /// catch (System.Exception ex1)
        /// {
        ///   System.Console.Error.WriteLine("exception: {0}", ex1);
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        ///   Try 
        ///       Using zip As ZipFile = New ZipFile
        ///           ' the following entry will be inserted at the root in the archive.
        ///           zip.AddFile("c:\datafiles\ReadMe.txt", "")
        ///           ' this image file will be inserted into the "images" directory in the archive.
        ///           zip.AddFile("c:\photos\personal\7440-N49th.png", "images")
        ///           ' the following will result in a password-protected file called 
        ///           ' files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
        ///           zip.Password = "EncryptMe!"
        ///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf", "files\documents")
        ///           zip.Save("Archive.zip")
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1)
        ///   End Try
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string,string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string,string)"/>
        ///
        /// <param name="fileName">
        /// The name of the file to add.  The name of the file may be a relative path or 
        /// a fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the fileName.  This path
        /// may, or may not, correspond to a real directory in the current filesystem.  If the
        /// files within the zip are later extracted, this is the path used for the extracted
        /// file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        /// fileName, if any.  Passing the empty string ("") will insert the item at the root
        /// path within the archive.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> corresponding to the file added.</returns>
        public ZipEntry AddFile(string fileName, String directoryPathInArchive)
        {
            string nameInArchive = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            ZipEntry ze = ZipEntry.Create(fileName, nameInArchive);
            ze.ForceNoCompression = ForceNoCompression;
            ze.ExtractExistingFile = ExtractExistingFile;
            ze.ZipErrorAction = ZipErrorAction;
            ze.WillReadTwiceOnInflation = WillReadTwiceOnInflation;
            ze.WantCompression = WantCompression;
            ze.ProvisionalAlternateEncoding = ProvisionalAlternateEncoding;
            ze._zipfile = this;
            ze.Encryption = Encryption;
            ze.Password = _Password;
            ze.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
            ze.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
            InsureUniqueEntry(ze);
            _entries.Add(ze);
            AfterAddEntry(ze);
            _contentsChanged = true;
            return ze;
        }




        /// <summary>
        /// This method adds a set of files to the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Use this method to add a set of files to the zip archive, in one call.  
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        /// The collection of names of the files to add. Each string should refer to a
        /// file in the filesystem. The name of the file may be a relative path or a
        /// fully-qualified path.
        /// </param>
        ///
        /// <example>
        /// This example shows how to create a zipfile, and add a few files into it. 
        /// <code>
        /// String ZipFileToCreate = "archive1.zip";
        /// String DirectoryToZip = "c:\\reports";
        /// using (ZipFile zip = new ZipFile())
        /// { 
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames);
        ///   zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        /// Dim ZipFileToCreate As String = "archive1.zip"
        /// Dim DirectoryToZip As String = "c:\reports"
        /// Using zip As ZipFile = New ZipFile
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames)
        ///     zip.Save(ZipFileToCreate)
        /// End Using
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public void AddFiles(List<string> fileNames)
        {
            AddFiles(fileNames, null);
        }


        /// <summary>
        /// Adds or updates a set of files in the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Any files that already exist in the archive are updated. Any files that
        /// don't yet exist in the archive are added.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        /// The collection of names of the files to update. Each string should refer to a file in 
        /// the filesystem. The name of the file may be a relative path or a fully-qualified path. 
        /// </param>
        ///
        public void UpdateFiles(List<string> fileNames)
        {
            UpdateFiles(fileNames, null);
        }


        /// <summary>
        /// Adds a set of files to the <c>ZipFile</c>, using the specified directory path 
        /// in the archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Any directory structure that may be present in the filenames contained in
        /// the list is "flattened" in the archive.  Each file in the list is added to
        /// the archive in the specified top-level directory.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        /// The names of the files to add. Each string should refer to a file in the
        /// filesystem.  The name of the file may be a relative path or a
        /// fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the file name.
        /// This path may, or may not, correspond to a real directory in the current
        /// filesystem.  If the files within the zip are later extracted, this is the
        /// path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        /// VB) will use the path on each of the <c>fileNames</c>, if any.  Passing the
        /// empty string ("") will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public void AddFiles(List<string> fileNames, String directoryPathInArchive)
        {
            AddFiles(fileNames, false, directoryPathInArchive);
        }



        /// <summary>
        /// Adds a set of files to the <c>ZipFile</c>, using the specified directory
        /// path in the archive, and preserving the full directory structure in the
        /// filenames.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// If preserveDirHierarchy is true, any directory structure present in the
        /// filenames contained in the list is preserved in the archive.  On the other
        /// hand, if preserveDirHierarchy is false, any directory structure that may be
        /// present in the filenames contained in the list is "flattened" in the
        /// archive; Each file in the list is added to the archive in the specified
        /// top-level directory.
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="fileNames">
        /// The names of the files to add. Each string should refer to a file in the filesystem.  
        /// The name of the file may be a relative path or a fully-qualified path. 
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the file name.
        /// This path may, or may not, correspond to a real directory in the current
        /// filesystem.  If the files within the zip are later extracted, this is the
        /// path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        /// VB) will use the path on each of the <c>fileNames</c>, if any.  Passing the
        /// empty string ("") will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <param name="preserveDirHierarchy">
        /// whether the entries in the zip archive will reflect the dir hierarchy that
        /// is present in each filename.
        /// </param>
        /// <seealso cref="Aspose.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public void AddFiles(List<string> fileNames,
                             bool preserveDirHierarchy,
                             String directoryPathInArchive)
        {
            OnAddStarted();
            if (preserveDirHierarchy)
            {
                foreach (string f in fileNames)
                {
                    if (directoryPathInArchive != null)
                    {
                        string s = SharedUtilities.NormalizePath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
                        AddFile(f, s);
                    }
                    else
                        AddFile(f, null);
                }
            }
            else
            {
                foreach (string f in fileNames)
                    AddFile(f, directoryPathInArchive);

            }
            OnAddCompleted();
        }


        /// <summary>
        /// Adds or updates a set of files to the <c>ZipFile</c>, using the specified
        /// directory path in the archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// Any files that already exist in the archive are updated. Any files that
        /// don't yet exist in the archive are added.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        /// The names of the files to add or update. Each string should refer to a file
        /// in the filesystem.  The name of the file may be a relative path or a
        /// fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the file name.
        /// This path may, or may not, correspond to a real directory in the current
        /// filesystem.  If the files within the zip are later extracted, this is the
        /// path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        /// VB) will use the path on each of the <c>fileNames</c>, if any.  Passing the
        /// empty string ("") will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public void UpdateFiles(List<string> fileNames, String directoryPathInArchive)
        {
            OnAddStarted();
            foreach (string f in fileNames)
                UpdateFile(f, directoryPathInArchive);
            OnAddCompleted();
        }




        /// <summary>
        /// Adds or Updates a File in a Zip file archive.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method adds a file to a zip archive, or, if the file already exists in
        /// the zip archive, this method Updates the content of that given filename in
        /// the zip archive.  The <c>UpdateFile</c> method might more accurately be
        /// called "AddOrUpdateFile".
        /// </para>
        ///
        /// <para>
        /// Upon success, there is no way for the application to learn whether the file
        /// was added versus updated.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// This example shows how to Update an existing entry in a zipfile. The first
        /// call to UpdateFile adds the file to the newly-created zip archive.  The
        /// second call to UpdateFile updates the content for that file in the zip
        /// archive.
        /// <code>
        /// using (ZipFile zip1 = new ZipFile())
        /// {
        ///   // UpdateFile might more accurately be called "AddOrUpdateFile"
        ///   zip1.UpdateFile("MyDocuments\\Readme.txt", "");
        ///   zip1.UpdateFile("CustomerList.csv", "");
        ///   zip1.Comment = "This zip archive has been created.";
        ///   zip1.Save("Content.zip");
        /// }
        /// 
        /// using (ZipFile zip2 = ZipFile.Read("Content.zip"))
        /// {
        ///   zip2.UpdateFile("Updates\\Readme.txt", "");
        ///   zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed.";
        ///   zip2.Save();
        /// }
        ///
        /// </code>
        /// <code lang="VB">
        ///   Using zip1 As New ZipFile
        ///       ' UpdateFile might more accurately be called "AddOrUpdateFile"
        ///       zip1.UpdateFile("MyDocuments\Readme.txt", "")
        ///       zip1.UpdateFile("CustomerList.csv", "")
        ///       zip1.Comment = "This zip archive has been created."
        ///       zip1.Save("Content.zip")
        ///   End Using
        ///
        ///   Using zip2 As ZipFile = ZipFile.Read("Content.zip")
        ///       zip2.UpdateFile("Updates\Readme.txt", "")
        ///       zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed."
        ///       zip2.Save
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <param name="fileName">
        /// The name of the file to add or update. It should refer to a file in the
        /// filesystem.  The name of the file may be a relative path or a
        /// fully-qualified path.
        /// </param>
        ///
        /// <returns>
        /// The <c>ZipEntry</c> corresponding to the File that was added or updated.
        /// </returns>
        public ZipEntry UpdateFile(string fileName)
        {
            return UpdateFile(fileName, null);
        }



        /// <summary>
        /// Adds or Updates a File in a Zip file archive.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method adds a file to a zip archive, or, if the file already exists in
        /// the zip archive, this method Updates the content of that given filename in
        /// the zip archive.
        /// </para>
        /// 
        /// <para>
        /// This version of the method allows the caller to explicitly specify the
        /// directory path to be used in the archive.  The entry to be added or updated
        /// is found by using the specified directory path, combined with the basename
        /// of the specified filename.
        /// </para>
        /// 
        /// <para>
        /// Upon success, there is no way for the application to learn if the file was
        /// added versus updated.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string,string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string,string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string,string)"/>
        ///
        /// <param name="fileName">
        /// The name of the file to add or update. It should refer to a file in the filesystem.  
        /// The name of the file may be a relative path or a fully-qualified path. 
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the
        /// <c>fileName</c>.  This path may, or may not, correspond to a real directory
        /// in the current filesystem.  If the files within the zip are later extracted,
        /// this is the path used for the extracted file.  Passing <c>null</c>
        /// (<c>Nothing</c> in VB) will use the path on the <c>fileName</c>, if any.
        /// Passing the empty string ("") will insert the item at the root path within
        /// the archive.
        /// </param>
        ///
        /// <returns>
        /// The <c>ZipEntry</c> corresponding to the File that was added or updated.
        /// </returns>
        public ZipEntry UpdateFile(string fileName, String directoryPathInArchive)
        {
            // ideally this would all be transactional!
            string key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);
            return AddFile(fileName, directoryPathInArchive);
        }

        /// <summary>
        /// Add or update a directory in a zip archive.  
        /// </summary>
        ///
        /// <remarks>
        /// If the specified directory does not exist in the archive, then this method
        /// is equivalent to calling AddDirectory().  If the specified directory already
        /// exists in the archive, then this method updates any existing entries, and
        /// adds any new entries. Any entries that are in the zip archive but not in the
        /// specified directory, are left alone.  In other words, the contents of the
        /// zip file will be a union of the previous contents and the new files.
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <param name="directoryName">
        /// The path to the directory to be added to the zip archive, 
        /// or updated in the zip archive.
        /// </param>
        /// 
        /// <returns>
        /// The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
        /// </returns>
        public ZipEntry UpdateDirectory(string directoryName)
        {
            return UpdateDirectory(directoryName, null);
        }


        /// <summary>
        /// Add or update a directory in the zip archive at the specified root directory
        /// in the archive.
        /// </summary>
        ///
        /// <remarks>
        /// If the specified directory does not exist in the archive, then this method
        /// is equivalent to calling AddDirectory().  If the specified directory already
        /// exists in the archive, then this method updates any existing entries, and
        /// adds any new entries. Any entries that are in the zip archive but not in the
        /// specified directory, are left alone.  In other words, the contents of the
        /// zip file will be a union of the previous contents and the new files.
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string,string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string,string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateItem(string,string)"/>
        ///
        /// <param name="directoryName">
        /// The path to the directory to be added to the zip archive, or updated in the
        /// zip archive.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the
        /// <c>directoryName</c>.  This path may, or may not, correspond to a real
        /// directory in the current filesystem.  If the files within the zip are later
        /// extracted, this is the path used for the extracted file.  Passing
        /// <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        /// <c>directoryName</c>, if any.  Passing the empty string ("") will insert the
        /// item at the root path within the archive.
        /// </param>
        /// 
        /// <returns>
        /// The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
        /// </returns>
        public ZipEntry UpdateDirectory(string directoryName, String directoryPathInArchive)
        {
            return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOrUpdate);
        }

        /// <summary>
        /// Add or update a file or directory in the zip archive. 
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This is useful when the application is not sure or does not care if the item
        /// to be added is a file or directory, and does not know or does not care if
        /// the item already exists in the <c>ZipFile</c>. Calling this method is
        /// equivalent to calling <c>RemoveEntry()</c> if an entry by the same name
        /// already exists, followed calling by <c>AddItem()</c>.
        /// </para>
        ///
        /// <para>
        /// For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string)"/>
        ///
        /// <param name="itemName">the path to the file or directory to be added or updated.</param>
        public void UpdateItem(string itemName)
        {
            UpdateItem(itemName, null);
        }


        /// <summary>
        /// Add or update a file or directory.  
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method is useful when the application is not sure or does not care if
        /// the item to be added is a file or directory, and does not know or does not
        /// care if the item already exists in the <c>ZipFile</c>. Calling this method is
        /// equivalent to calling <c>RemoveEntry()</c>, if an entry by that name exists,
        /// and then calling <c>AddItem()</c>.
        /// </para>
        /// 
        /// <para>
        /// This version of the method allows the caller to explicitly specify the
        /// directory path to be used for the item being added to the archive.  The
        /// entry or entries that are added or updated will use the specified
        /// <c>DirectoryPathInArchive</c>. Extracting the entry from the archive will
        /// result in a file stored in that directory path.
        /// </para>
        ///
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateFile(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string, string)"/>
        ///
        /// <param name="itemName">The path for the File or Directory to be added or updated.</param>
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the
        /// <c>itemName</c>.  This path may, or may not, correspond to a real directory
        /// in the current filesystem.  If the files within the zip are later extracted,
        /// this is the path used for the extracted file.  Passing <c>null</c>
        /// (<c>Nothing</c> in VB) will use the path on the <c>itemName</c>, if any.
        /// Passing the empty string ("") will insert the item at the root path within
        /// the archive.
        /// </param>
        public void UpdateItem(string itemName, string directoryPathInArchive)
        {
            if (File.Exists(itemName))
                UpdateFile(itemName, directoryPathInArchive);

            else if (Directory.Exists(itemName))
                UpdateDirectory(itemName, directoryPathInArchive);

            else
                throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!", itemName));
        }

        /// <summary>
        /// Adds a named entry into the zip archive, taking content for the entry
        /// from a string.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this method creates an entry using the given fileName and directory
        /// path within the archive.  There is no need for a file by the given name to
        /// exist in the filesystem; the name is used within the zip archive only. The
        /// content for the entry is encoded using the default text encoding (<see
        /// cref="System.Text.Encoding.Default"/>).
        /// </remarks>
        ///
        /// <param name="content">The content of the file, should it be extracted from
        /// the zip.</param>
        ///
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the fileName.
        /// This path need not correspond to a real directory in the current filesystem
        /// when creating the zip file.  If the files within the zip are later
        /// extracted, this is the path used for the extracted file.  Passing
        /// <c>null</c> (<c>Nothing</c> in VB) will use the path on the fileName, if
        /// any.  Passing the empty string ("") will insert the item at the root path
        /// within the archive.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        /// 
        /// <example>
        ///
        /// This example shows how to add an entry to the zipfile, using a string as
        /// content for that entry.
        ///
        /// <code lang="C#">
        /// string Content = "This string will be the content of the Readme.txt file in the zip archive.";
        /// using (ZipFile zip1 = new ZipFile())
        /// {
        ///   zip1.AddFile("MyDocuments\\Resume.doc", "files");
        ///   zip1.AddEntry("Readme.txt", "", Content); 
        ///   zip1.Comment = "This zip file was created at " + System.DateTime.Now.ToString("G");
        ///   zip1.Save("Content.zip");
        /// }
        /// 
        /// </code>
        /// <code lang="VB">
        /// Public Sub Run()
        ///   Dim Content As String = "This string will be the content of the Readme.txt file in the zip archive."
        ///   Using zip1 As ZipFile = New ZipFile
        ///     zip1.AddEntry("Readme.txt", "", Content)
        ///     zip1.AddFile("MyDocuments\Resume.doc", "files")
        ///     zip1.Comment = ("This zip file was created at " &amp; DateTime.Now.ToString("G"))
        ///     zip1.Save("Content.zip")
        ///   End Using
        /// End Sub
        /// </code>
        /// </example>
        public ZipEntry AddEntry(string fileName, string directoryPathInArchive, string content)
        {
            return AddEntry(fileName, directoryPathInArchive, content,
                                     System.Text.Encoding.Default);
        }



        /// <summary>
        /// Adds a named entry into the zip archive, taking content for the entry
        /// from a string.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>Calling this method creates an entry using the given fileName and
        /// directory path within the archive.  There is no need for a file by the given
        /// name to exist in the filesystem; the name is used within the zip archive
        /// only. </para>
        /// 
        /// <para> The content for the entry is encoded using the given text
        /// encoding. No Byte-order-mark (BOM) is emitted into the file. </para>
        ///
        /// <para>If you wish to create within a zip file a file entry with
        /// Unicode-encoded content that includes a byte-order-mark, you can convert
        /// your string to a byte array using the appropriate <see
        /// cref="System.Text.Encoding.GetBytes(String)"/> method, then prepend to that byte
        /// array the output of <see cref="System.Text.Encoding.GetPreamble()"/>, and use the
        /// <c>AddEntry(string,string,byte[])</c> method, to add the entry.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the <c>fileName</c>.
        /// This path need not correspond to a real directory in the current filesystem when
        /// creating the zip file.  If the files within the zip are later extracted, this is
        /// the path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB)
        /// will use the path on the <c>fileName</c>, if any.  Passing the empty string ("")
        /// will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <param name="content">The content of the file, should it be extracted from
        /// the zip.</param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when encoding the string. Be aware: This is
        /// distinct from the text encoding used to encode the fileName, as specified in <see
        /// cref="ProvisionalAlternateEncoding" />.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        /// 
        public ZipEntry AddEntry(string fileName, string directoryPathInArchive, string content,
            System.Text.Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, encoding);

            sw.Write(content);
            sw.Flush();

            // reset to allow reading later
            ms.Seek(0, SeekOrigin.Begin);

            return AddEntry(fileName, directoryPathInArchive, ms);
        }


        /// <summary>
        /// Create an entry in the <c>ZipFile</c> using the given Stream as input.  The
        /// entry will have the given filename and given directory path.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// The application can provide an open, readable stream; in this case it will
        /// be read during the call to <see cref="ZipFile.Save()"/> or one of its
        /// overloads.
        /// </para>
        ///
        /// <para>
        /// In cases where a large number of streams will be added to the
        /// <c>ZipFile</c>, the application may wish to avoid maintaining all of the
        /// streams open simultaneously.  To handle this situation, the application can
        /// provide a <c>null</c> value (<c>Nothing</c> in VB) for the stream, and
        /// provide a handler for the <see cref="ZipFile.SaveProgress"/> event.  Later,
        /// during the call to <c>ZipFile.Save</c>, DotNetZip will invoke the
        /// SaveProgress event handler, and within that handler, when the <see
        /// cref="ZipProgressEventArgs.EventType">e.EventType</see> is
        /// <c>ZipProgressEventType.Saving_BeforeWriteEntry</c>, the application can
        /// dispense the stream for each entry on a just-in-time basis by setting the
        /// <see cref="ZipEntry.InputStream"/> property.  The application can close or
        /// dispose the stream for each entry in a similar manner, when the
        /// <c>e.EventType</c> is
        /// <c>ZipProgressEventType.Saving_AfterWriteEntry</c>. Check the documentation
        /// of <see cref="ZipEntry.InputStream"/> for more information and a code
        /// sample.
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <example>
        /// <para>
        /// This example adds a single entry to a ZipFile via a stream. 
        /// </para>
        ///
        /// <code lang="C#">
        /// String ZipToCreate = "Content.zip";
        /// String FileNameInArchive = "Content-From-Stream.bin";
        /// using (System.IO.Stream StreamToRead = MyStreamOpener())
        /// {
        ///   using (ZipFile zip = new ZipFile())
        ///   {
        ///     ZipEntry entry= zip.AddEntry(FileNameInArchive, "basedirectory", StreamToRead);
        ///     entry.Comment = "The content for this entry in the zip file was obtained from a stream";
        ///     zip.AddFile("Readme.txt");
        ///     zip.Save(ZipToCreate);
        ///   }
        /// }
        /// 
        /// </code>
        /// <code lang="VB">
        /// Dim ZipToCreate As String = "Content.zip"
        /// Dim FileNameInArchive As String = "Content-From-Stream.bin"
        /// Using StreamToRead as System.IO.Stream = MyStreamOpener()
        ///   Using zip As ZipFile = New ZipFile()
        ///     Dim entry as ZipEntry = zip.AddEntry(FileNameInArchive, "basedirectory", StreamToRead)
        ///     entry.Comment = "The content for this entry in the zip file was obtained from a stream"
        ///     zip.AddFile("Readme.txt")
        ///     zip.Save(ZipToCreate)
        ///   End Using
        /// End Using
        /// </code>
        /// </example>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateEntry(string, string, System.IO.Stream)"/>
        ///
        /// <param name="fileName">the name which is shown in the zip file for the added entry.</param>
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the <c>itemName</c>.
        /// This path may, or may not, correspond to a real directory in the current
        /// filesystem.  If the files within the zip are later extracted, this is the path used
        /// for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the
        /// path on the <c>fileName</c>, if any.  Passing the empty string ("") will insert the
        /// item at the root path within the archive.
        /// </param>
        /// <param name="stream">the input stream from which to grab content for the file</param>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddEntry(string fileName, string directoryPathInArchive, Stream stream)
        {
            string n = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            ZipEntry ze = ZipEntry.Create(fileName, n, true, stream);
            ze.ForceNoCompression = ForceNoCompression;
            ze.ExtractExistingFile = ExtractExistingFile;
            ze.ZipErrorAction = ZipErrorAction;
            ze.WillReadTwiceOnInflation = WillReadTwiceOnInflation;
            ze.WantCompression = WantCompression;
            ze.ProvisionalAlternateEncoding = ProvisionalAlternateEncoding;
            ze._zipfile = this;
            ze.Encryption = Encryption;
            ze.Password = _Password;
            ze.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
            ze.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;
            if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
            InsureUniqueEntry(ze);
            _entries.Add(ze);
            AfterAddEntry(ze);
            _contentsChanged = true;
            return ze;
        }

        /// <summary>
        /// Updates the given entry in the <c>ZipFile</c>, using the given string as input.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// Calling this method is equivalent to removing the <c>ZipEntry</c> for the
        /// given file name and directory path, if it exists, and then calling <see
        /// cref="AddEntry(String,String,String)" />.  See the documentation
        /// for that method for further explanation. </para>
        /// 
        /// </remarks>
        ///
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the <c>fileName</c>.
        /// This path need not correspond to a real directory in the current filesystem when
        /// creating the zip file.  If the files within the zip are later extracted, this is
        /// the path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB)
        /// will use the path on the <c>fileName</c>, if any.  Passing the empty string ("")
        /// will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <param name="content">
        /// The content of the file, should it be extracted from the zip.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        /// 
        public ZipEntry UpdateEntry(string fileName, string directoryPathInArchive,
                                    string content)
        {
            return UpdateEntry(fileName, directoryPathInArchive,
                               content, System.Text.Encoding.Default);
        }


        /// <summary>
        /// Updates the given entry in the <c>ZipFile</c>, using the given string as content
        /// for the <c>ZipEntry</c>. 
        /// </summary>
        ///
        /// <remarks>Calling this method is equivalent to removing the <c>ZipEntry</c> for the
        /// given file name and directory path, if it exists, and then calling <see
        /// cref="AddEntry(String,String,String, System.Text.Encoding)" />.
        /// See the documentation for that method for further explanation. </remarks>
        ///
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the fileName.
        /// This path need not correspond to a real directory in the current filesystem
        /// when creating the zip file.  If the files within the zip are later
        /// extracted, this is the path used for the extracted file.  Passing
        /// <c>null</c> (<c>Nothing</c> in VB) will use the path on the <c>fileName</c>,
        /// if any.  Passing the empty string ("") will insert the item at the root path
        /// within the archive.
        /// </param>
        ///
        /// <param name="content">
        /// The content of the file, should it be extracted from the zip.
        /// </param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when encoding the string. Be aware: This is
        /// distinct from the text encoding used to encode the filename. See <see
        /// cref="ProvisionalAlternateEncoding" />.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        /// 
        public ZipEntry UpdateEntry(string fileName, string directoryPathInArchive,
                                    string content,
                                    System.Text.Encoding encoding)
        {
            string key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);

            return AddEntry(fileName, directoryPathInArchive, content, encoding);
        }


        /// <summary>
        /// Updates the given entry in the <c>ZipFile</c>, using the given stream as
        /// input, and the given filename and given directory Path.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Calling the method is equivalent to calling RemoveEntry() if an entry by the
        /// same name already exists, and then calling AddEntry() with the given
        /// <c>fileName</c> and stream.
        /// </para>
        ///
        /// <para>
        /// The stream must be open and readable during the call to 
        /// <c>ZipFile.Save</c>.  You can dispense the stream on a just-in-time basis using
        /// the <see cref="ZipEntry.InputStream"/> property. Check the documentation of that
        /// property for more information. 
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to the <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddEntry(string, string, System.IO.Stream)"/>
        /// <seealso cref="ZipEntry.InputStream"/>
        ///
        /// <param name="fileName">the name associated to the entry in the zip archive.</param>
        /// <param name="directoryPathInArchive">
        /// The root path to be used in the zip archive, 
        /// for the entry added from the stream.</param>
        /// <param name="stream">The input stream from which to read file data.</param>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry UpdateEntry(string fileName, string directoryPathInArchive, Stream stream)
        {
            string key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);

            return AddEntry(fileName, directoryPathInArchive, stream);
        }

        /// <summary>
        /// Add an entry into the zip archive using the given filename and directory
        /// path within the archive, and the given content for the file. No file is
        /// created in the filesystem.
        /// </summary>
        ///
        /// <param name="byteContent">The data to use for the entry.</param>
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use for the entry.  This path may, or may not,
        /// correspond to a real directory in the current filesystem.  If the files
        /// within the zip are later extracted, this is the path used for the extracted
        /// file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        /// <c>fileName</c>, if any. Passing the empty string ("") will insert the item
        /// at the root path within the archive.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddEntry(string fileName, string directoryPathInArchive, byte[] byteContent)
        {
            if (byteContent == null) throw new ArgumentException("bad argument", "byteContent");
            MemoryStream ms = new MemoryStream(byteContent);
            return AddEntry(fileName, directoryPathInArchive, ms);
        }

        
        /// <summary>
        /// Updates the given entry in the <c>ZipFile</c>, using the given byte array as
        /// content for the entry.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this method is equivalent to removing the <c>ZipEntry</c> for the
        /// given filename and directory path, if it exists, and then calling <see
        /// cref="AddEntry(String,String,String, System.Text.Encoding)" />.
        /// See the documentation for that method for further explanation.
        /// </remarks>
        ///
        /// <param name="fileName">The filename to use within the archive.</param>
        ///
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the <c>fileName</c>.
        /// This path need not correspond to a real directory in the current filesystem when
        /// creating the zip file.  If the files within the zip are later extracted, this is
        /// the path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB)
        /// will use the path on the <c>fileName</c>, if any.  Passing the empty string ("")
        /// will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <param name="byteContent">The content to use for the <c>ZipEntry</c>.</param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        /// 
        public ZipEntry UpdateEntry(string fileName, string directoryPathInArchive,
                                    byte[] byteContent)
        {
            string key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
            if (this[key] != null)
                RemoveEntry(key);

            return AddEntry(fileName, directoryPathInArchive, byteContent);
        }

        
        private void InsureUniqueEntry(ZipEntry ze1)
        {
            foreach (ZipEntry ze2 in _entries)
            {
                if (SharedUtilities.TrimVolumeAndSwapSlashes(ze1.FileName) == ze2.FileName)
                    throw new ArgumentException(String.Format("The entry '{0}' already exists in the zip archive.", ze1.FileName));
            }
        }

        /// <summary>
        /// Adds the contents of a filesystem directory to a Zip file archive. 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// The name of the directory may be a relative path or a fully-qualified
        /// path. Any files within the named directory are added to the archive.  Any
        /// subdirectories within the named directory are also added to the archive,
        /// recursively.
        /// </para>
        /// 
        /// <para>
        /// Top-level entries in the named directory will appear as top-level 
        /// entries in the zip archive.  Entries in subdirectories in the named 
        /// directory will result in entries in subdirectories in the zip archive.
        /// </para>
        /// 
        /// <para>
        /// If you want the entries to appear in a containing directory in the zip
        /// archive itself, then you should call the AddDirectory() overload that allows
        /// you to explicitly specify a directory path for use in the archive.
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddDirectory(string, string)"/>
        ///
        /// <overloads>This method has 2 overloads.</overloads>
        /// 
        /// <param name="directoryName">The name of the directory to add.</param>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddDirectory(string directoryName)
        {
            return AddDirectory(directoryName, null);
        }


        /// <summary>
        /// Adds the contents of a filesystem directory to a Zip file archive, 
        /// overriding the path to be used for entries in the archive. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The name of the directory may be a relative path or a fully-qualified
        /// path. The add operation is recursive, so that any files or subdirectories
        /// within the name directory are also added to the archive.
        /// </para>
        /// 
        /// <para>
        /// Top-level entries in the named directory will appear as top-level 
        /// entries in the zip archive.  Entries in subdirectories in the named 
        /// directory will result in entries in subdirectories in the zip archive.
        /// </para>
        /// 
        /// <para>
        /// For ZipFile properties including <see cref="Encryption"/>, <see
        /// cref="Password"/>, <see cref="WantCompression"/>, <see
        /// cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        /// <see cref="ZipErrorAction"/>, 
        /// and <see cref="ForceNoCompression"/>, their respective values at the time of
        /// this call will be applied to each ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <example>
        /// <para>
        /// In this code, calling the ZipUp() method with a value of "c:\reports" for the
        /// directory parameter will result in a zip file structure in which all entries
        /// are contained in a toplevel "reports" directory.
        /// </para>
        ///
        /// <code lang="C#">
        /// public void ZipUp(string targetZip, string directory)
        /// {
        ///   using (var zip = new ZipFile())
        ///   {
        ///     zip.AddDirectory(directory, System.IO.Path.GetFileName(directory));
        ///     zip.Save(targetZip);
        ///   }
        /// }
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.AddItem(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.AddFile(string, string)"/>
        /// <seealso cref="Aspose.Zip.ZipFile.UpdateDirectory(string, string)"/>
        ///
        /// <param name="directoryName">The name of the directory to add.</param>
        /// 
        /// <param name="directoryPathInArchive">
        /// Specifies a directory path to use to override any path in the DirectoryName.
        /// This path may, or may not, correspond to a real directory in the current
        /// filesystem.  If the zip is later extracted, this is the path used for the
        /// extracted file or directory.  Passing <c>null</c> (<c>Nothing</c> in VB) or
        /// the empty string ("") will insert the items at the root path within the
        /// archive.
        /// </param>
        /// 
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddDirectory(string directoryName, string directoryPathInArchive)
        {
            return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOnly);
        }


        /// <summary>
        /// Creates a directory in the zip archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// Use this when you want to create a directory in the archive but there is no
        /// corresponding filesystem representation for that directory.
        /// </para>
        ///
        /// <para>
        /// You will probably not need to do this in your code. One of the only times
        /// you will want to do this is if you want an empty directory in the zip
        /// archive.  The reason: if you add a file to a zip archive that is stored within a
        /// multi-level directory, all of the directory tree is implicitly created in
        /// the zip archive.  
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <param name="directoryNameInArchive">
        /// The name of the directory to create in the archive.
        /// </param>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public ZipEntry AddDirectoryByName(string directoryNameInArchive)
        {
            // add the directory itself.
            ZipEntry baseDir = ZipEntry.Create(directoryNameInArchive, directoryNameInArchive);
            baseDir.MarkAsDirectory();
            baseDir._Source = ZipEntrySource.Stream;
            baseDir._zipfile = this;
            InsureUniqueEntry(baseDir);
            _entries.Add(baseDir);
            AfterAddEntry(baseDir);
            _contentsChanged = true;
            return baseDir;
        }



        private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action)
        {
            if (rootDirectoryPathInArchive == null)
            {
                rootDirectoryPathInArchive = "";
            }

            return AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, 0);
        }



        private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action, int level)
        {
            if (Verbose) StatusMessageTextWriter.WriteLine("{0} {1}...",
                                                           (action == AddOrUpdateAction.AddOnly) ? "adding" : "Adding or updating", directoryName);

            if (level == 0)
                OnAddStarted();
            
            string dirForEntries = rootDirectoryPathInArchive;
            ZipEntry baseDir = null;

            if (level > 0)
            {
                int f = directoryName.Length;
                for (int i = level; i > 0; i--)
                    f = directoryName.LastIndexOfAny("/\\".ToCharArray(), f - 1, f - 1);

                dirForEntries = directoryName.Substring(f + 1);
                dirForEntries = Path.Combine(rootDirectoryPathInArchive, dirForEntries);
            }

            // if not top level, or if the root is non-empty, then explicitly add the directory
            if (level > 0 || rootDirectoryPathInArchive != "")
            {
                baseDir = ZipEntry.Create(directoryName, dirForEntries);
                baseDir.ProvisionalAlternateEncoding = ProvisionalAlternateEncoding;  // workitem 6410
                baseDir.MarkAsDirectory();
                baseDir._zipfile = this;


                // check for uniqueness:
                ZipEntry e = this[baseDir.FileName];
                if (e == null)
                {
                    _entries.Add(baseDir);
                    _contentsChanged = true;
                }
                dirForEntries = baseDir.FileName;
            }

            String[] filenames = Directory.GetFiles(directoryName);

            // add the files: 
            foreach (String filename in filenames)
            {
                if (action == AddOrUpdateAction.AddOnly)
                    AddFile(filename, dirForEntries);
                else
                    UpdateFile(filename, dirForEntries);
            }

            // add the subdirectories:
            String[] dirnames = Directory.GetDirectories(directoryName);
            foreach (String dir in dirnames)
            {
                AddOrUpdateDirectoryImpl(dir, rootDirectoryPathInArchive, action, level + 1);
            }

            if (level == 0)
                OnAddCompleted();
            
            return baseDir;
        }

#endregion

        #region Check

        /// <summary>
        /// Checks a zip file to see if its directory is consistent.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para> In cases of data error, the directory within a zip file can get out of
        ///     synch with the entries in the zip file.  This method checks the given
        ///     zip file and returns true if this has occurred.  </para>
        ///
        /// <para> This method may take a long time to run for large zip files.  </para>
        ///
        /// <para>
        /// This method is not supported in the Reduced or Compact
        /// Framework versions of DotNetZip.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <returns>true if the named zip file checks OK. Otherwise, false. </returns>
        ///
        /// <seealso cref="FixZipDirectory(string)"/>
        /// <seealso cref="CheckZip(string,bool,out System.Collections.ObjectModel.ReadOnlyCollection&lt;String&gt;)"/>
        public static bool CheckZip(string zipFileName)
        {
            List<string> ignoredMessages;
            return CheckZip(zipFileName, false, out ignoredMessages);
        }


        /// <summary>
        /// Checks a zip file to see if its directory is consistent, 
        /// and optionally fixes the directory if necessary. 
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para> In cases of data error, the directory within a zip file can get out of
        ///     synch with the entries in the zip file.  This method checks the given
        ///     zip file, and returns true if this has occurred. It also optionally
        ///     fixes the zipfile, saving the fixed copy in <em>Name</em>_Fixed.zip.</para>
        ///
        /// <para> This method may take a long time to run for large zip files.  It will
        ///     take even longer if the file actually needs to be fixed, and if
        ///     <c>fixIfNecessary</c> is true.  </para>
        ///
        /// <para>
        /// This method is not supported in the Reduced or Compact
        /// Framework versions of DotNetZip.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <param name="fixIfNecessary">If true, the method will fix the zip file if
        ///     necessary.</param>
        /// 
        /// <param name="messages">
        /// a collection of messages generated while checking, indicating any problems that are found. 
        /// </param>
        /// 
        /// <returns>true if the named zip is OK; false if the file needs to be fixed.</returns>
        ///
        /// <seealso cref="CheckZip(string)"/>
        /// <seealso cref="FixZipDirectory(string)"/>
        public static bool CheckZip(string zipFileName, bool fixIfNecessary, out List<string> messages)
        {
            List<string> notes = new List<string>();
            ZipFile zip1 = null;
            ZipFile zip2 = null;
            bool isOk = true;
            try
            {
                zip1 = new ZipFile();
                zip1.FullScan = true;
                zip1.Initialize(zipFileName);

                zip2 = ZipFile.Read(zipFileName);

                foreach (ZipEntry e1 in zip1)
                {
                    foreach (ZipEntry e2 in zip2)
                    {
                        if (e1.FileName == e2.FileName)
                        {
                            if (e1._RelativeOffsetOfLocalHeader != e2._RelativeOffsetOfLocalHeader)
                            {
                                isOk = false;
                                notes.Add(String.Format("{0}: mismatch in RelativeOffsetOfLocalHeader  (0x{1:X16} != 0x{2:X16})",
                                                        e1.FileName, e1._RelativeOffsetOfLocalHeader,
                                                        e2._RelativeOffsetOfLocalHeader));
                            }
                            if (e1._CompressedSize != e2._CompressedSize)
                            {
                                isOk = false;
                                notes.Add(String.Format("{0}: mismatch in CompressedSize  (0x{1:X16} != 0x{2:X16})",
                                                        e1.FileName, e1._CompressedSize,
                                                        e2._CompressedSize));
                            }
                            if (e1._UncompressedSize != e2._UncompressedSize)
                            {
                                isOk = false;
                                notes.Add(String.Format("{0}: mismatch in UncompressedSize  (0x{1:X16} != 0x{2:X16})",
                                                        e1.FileName, e1._UncompressedSize,
                                                        e2._UncompressedSize));
                            }
                            if (e1.CompressionMethod != e2.CompressionMethod)
                            {
                                isOk = false;
                                notes.Add(String.Format("{0}: mismatch in CompressionMethod  (0x{1:X4} != 0x{2:X4})",
                                                        e1.FileName, e1.CompressionMethod,
                                                        e2.CompressionMethod));
                            }
                            if (e1.Crc != e2.Crc)
                            {
                                isOk = false;
                                notes.Add(String.Format("{0}: mismatch in Crc32  (0x{1:X4} != 0x{2:X4})",
                                                        e1.FileName, e1.Crc,
                                                        e2.Crc));
                            }

                            // found a match, so stop the inside loop
                            break;
                        }
                    }
                }

                zip2.Dispose();
                zip2 = null;

                if (!isOk && fixIfNecessary)
                {
                    string newFileName = Path.GetFileNameWithoutExtension(zipFileName);
                    newFileName = String.Format("{0}_fixed.zip", newFileName);
                    zip1.Save(newFileName);
                }
            }
            finally
            {
                if (zip1 != null) zip1.Dispose();
                if (zip2 != null) zip2.Dispose();
            }
            messages = notes; // may or may not be empty
            return isOk;
        }



        /// <summary>
        /// Rewrite the directory within a zipfile.
        /// </summary>
        /// 
        /// <remarks>
        ///
        /// <para> In cases of data error, the directory in a zip file can get out of
        ///     synch with the entries in the zip file.  This method returns true if
        ///     this has occurred.  </para>
        ///
        /// <para> This can take a long time for large zip files. </para>
        ///
        /// <para>
        /// This method is not supported in the Reduced or Compact
        /// Framework versions of DotNetZip.
        /// </para>
        /// 
        /// </remarks>
        ///
        /// <seealso cref="CheckZip(string)"/>
        /// <seealso cref="CheckZip(string,bool,out System.Collections.ObjectModel.ReadOnlyCollection&lt;String&gt;)"/>
        public static void FixZipDirectory(string zipFileName)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.FullScan = true;
                zip.Initialize(zipFileName);
                zip.Save(zipFileName);
            }
        }

#endregion

        #region Events

        private string ArchiveNameForEvent
        {
            get
            {
                return (_name != null) ? _name : "(stream)";
            }
        }


        #region Save

        /// <summary>
        /// An event handler invoked when a Save() starts, before and after each entry has been
        /// written to the archive, when a Save() completes, and during other Save events.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Depending on the particular event, different properties on the
        /// SaveProgressEventArgs parameter are set.  The following table 
        /// summarizes the available EventTypes and the conditions under which this 
        /// event handler is invoked with a SaveProgressEventArgs with the given EventType.
        /// </para>
        /// 
        /// <list type="table">
        /// <listheader>
        /// <term>value of EntryType</term>
        /// <description>Meaning and conditions</description>
        /// </listheader>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_Started</term>
        /// <description>Fired when ZipFile.Save() begins. 
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_BeforeSaveEntry</term>
        /// <description>Fired within ZipFile.Save(), just before writing data for each particular entry. 
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_AfterSaveEntry</term>
        /// <description>Fired within ZipFile.Save(), just after having finished writing data for each 
        /// particular entry. 
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_Completed</term>
        /// <description>Fired when ZipFile.Save() has completed. 
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_AfterSaveTempArchive</term>
        /// <description>Fired after the temporary file has been created.  This happens only
        /// when saving to a disk file.  This event will not be invoked when saving to a stream.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_BeforeRenameTempArchive</term>
        /// <description>Fired just before renaming the temporary file to the permanent location.  This 
        /// happens only when saving to a disk file.  This event will not be invoked when saving to a stream.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_AfterRenameTempArchive</term>
        /// <description>Fired just after renaming the temporary file to the permanent location.  This 
        /// happens only when saving to a disk file.  This event will not be invoked when saving to a stream.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_AfterCompileSelfExtractor</term>
        /// <description>Fired after a self-extracting archive has finished compiling. 
        /// This EventType is used only within SaveSelfExtractor().
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Saving_BytesRead</term>
        /// <description>Set during the save of a particular entry, to update progress of the Save(). 
        /// When this EventType is set, the BytesTransferred is the number of bytes that have been read from the 
        /// source stream.  The TotalBytesToTransfer is the number of bytes in the uncompressed file.
        /// </description>
        /// </item>
        /// 
        /// </list>
        /// </remarks>
        ///
        /// <example>
        /// <code lang="C#">
        /// static bool justHadByteUpdate= false;
        /// public static void SaveProgress(object sender, SaveProgressEventArgs e)
        /// {
        ///     if (e.EventType == ZipProgressEventType.Saving_Started)
        ///         Console.WriteLine("Saving: {0}", e.ArchiveName);
        /// 
        ///     else if (e.EventType == ZipProgressEventType.Saving_Completed)
        ///     {
        ///         justHadByteUpdate= false; 
        ///         Console.WriteLine();
        ///         Console.WriteLine("Done: {0}", e.ArchiveName);
        ///     }
        /// 
        ///     else if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
        ///     {
        ///         if (justHadByteUpdate) 
        ///             Console.WriteLine();
        ///         Console.WriteLine("  Writing: {0} ({1}/{2})",  
        ///                           e.CurrentEntry.FileName, e.EntriesSaved, e.EntriesTotal);
        ///         justHadByteUpdate= false;
        ///     }
        /// 
        ///     else if (e.EventType == ZipProgressEventType.Saving_EntryBytesRead)
        ///     {
        ///         if (justHadByteUpdate)
        ///             Console.SetCursorPosition(0, Console.CursorTop);
        ///          Console.Write("     {0}/{1} ({2}%)", e.BytesTransferred, e.TotalBytesToTransfer,
        ///                       e.BytesTransferred / (0.01 * e.TotalBytesToTransfer ));
        ///         justHadByteUpdate= true;
        ///     }
        /// }
        /// 
        /// public static ZipUp(string targetZip, string directory)
        /// {
        ///   using (var zip = new ZipFile()) {
        ///     zip.SaveProgress += SaveProgress; 
        ///     zip.AddDirectory(directory);
        ///     zip.Save(targetZip);
        ///   }
        /// }
        ///
        /// </code>
        ///
        /// <code lang="VB">
        /// Public Sub ZipUp(ByVal targetZip As String, ByVal directory As String)
        ///     Try 
        ///         Using zip As ZipFile = New ZipFile
        ///             AddHandler zip.SaveProgress, AddressOf MySaveProgress
        ///             zip.AddDirectory(directory)
        ///             zip.Save(targetZip)
        ///         End Using
        ///     Catch ex1 As Exception
        ///         Console.Error.WriteLine(("exception: " &amp; ex1.ToString))
        ///     End Try
        /// End Sub
        /// 
        /// Private Shared justHadByteUpdate As Boolean = False
        /// 
        /// Public Shared Sub MySaveProgress(ByVal sender As Object, ByVal e As SaveProgressEventArgs)
        ///     If (e.EventType Is ZipProgressEventType.Saving_Started) Then
        ///         Console.WriteLine("Saving: {0}", e.ArchiveName)
        /// 
        ///     ElseIf (e.EventType Is ZipProgressEventType.Saving_Completed) Then
        ///         justHadByteUpdate = False
        ///         Console.WriteLine
        ///         Console.WriteLine("Done: {0}", e.ArchiveName)
        /// 
        ///     ElseIf (e.EventType Is ZipProgressEventType.Saving_BeforeWriteEntry) Then
        ///         If justHadByteUpdate Then
        ///             Console.WriteLine
        ///         End If
        ///         Console.WriteLine("  Writing: {0} ({1}/{2})", e.CurrentEntry.FileName, e.EntriesSaved, e.EntriesTotal)
        ///         justHadByteUpdate = False
        /// 
        ///     ElseIf (e.EventType Is ZipProgressEventType.Saving_EntryBytesRead) Then
        ///         If justHadByteUpdate Then
        ///             Console.SetCursorPosition(0, Console.CursorTop)
        ///         End If
        ///         Console.Write("     {0}/{1} ({2}%)", e.BytesTransferred, _
        ///                       e.TotalBytesToTransfer, _
        ///                       (CDbl(e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)))
        ///         justHadByteUpdate = True
        ///     End If
        /// End Sub
        /// </code>
        ///
        /// <para>
        /// This is an example of using the SaveProgress events in a WinForms app.
        /// </para>
        /// <code>
        /// delegate void SaveEntryProgress(SaveProgressEventArgs e);
        /// delegate void ButtonClick(object sender, EventArgs e);
        ///
        /// public class WorkerOptions
        /// {
        ///     public string ZipName;
        ///     public string Folder;
        ///     public string Encoding;
        ///     public string Comment;
        ///     public int ZipFlavor;
        ///     public Zip64Option Zip64;
        /// }
        ///
        /// private int _progress2MaxFactor;
        /// private bool _saveCanceled;
        /// private long _totalBytesBeforeCompress;
        /// private long _totalBytesAfterCompress;
        /// private Thread _workerThread;
        ///
        ///
        /// private void btnZipup_Click(object sender, EventArgs e)
        /// {
        ///     KickoffZipup();
        /// }
        ///
        /// private void btnCancel_Click(object sender, EventArgs e)
        /// {
        ///     if (this.lblStatus.InvokeRequired)
        ///     {
        ///         this.lblStatus.Invoke(new ButtonClick(this.btnCancel_Click), new object[] { sender, e });
        ///     }
        ///     else
        ///     {
        ///         _saveCanceled = true;
        ///         lblStatus.Text = "Canceled...";
        ///         ResetState();
        ///     }
        /// }
        ///
        /// private void KickoffZipup()
        /// {
        ///     _folderName = tbDirName.Text;
        ///
        ///     if (_folderName == null || _folderName == "") return;
        ///     if (this.tbZipName.Text == null || this.tbZipName.Text == "") return;
        ///
        ///     // check for existence of the zip file:
        ///     if (System.IO.File.Exists(this.tbZipName.Text))
        ///     {
        ///         var dlgResult = MessageBox.Show(String.Format("The file you have specified ({0}) already exists." + 
        ///                                                       "  Do you want to overwrite this file?", this.tbZipName.Text), 
        ///                                         "Confirmation is Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        ///         if (dlgResult != DialogResult.Yes) return;
        ///         System.IO.File.Delete(this.tbZipName.Text);
        ///     }
        ///
        ///      _saveCanceled = false;
        ///     _nFilesCompleted = 0;
        ///     _totalBytesAfterCompress = 0;
        ///     _totalBytesBeforeCompress = 0;
        ///     this.btnOk.Enabled = false;
        ///     this.btnOk.Text = "Zipping...";
        ///     this.btnCancel.Enabled = true;
        ///     lblStatus.Text = "Zipping...";
        ///
        ///     var options = new WorkerOptions
        ///     {
        ///         ZipName = this.tbZipName.Text,
        ///         Folder = _folderName,
        ///         Encoding = "ibm437"
        ///     };
        ///
        ///     if (this.comboBox1.SelectedIndex != 0)
        ///     {
        ///         options.Encoding = this.comboBox1.SelectedItem.ToString();
        ///     }
        ///
        ///     if (this.radioFlavorSfxCmd.Checked)
        ///         options.ZipFlavor = 2;
        ///     else if (this.radioFlavorSfxGui.Checked)
        ///         options.ZipFlavor = 1;
        ///     else options.ZipFlavor = 0;
        ///
        ///     if (this.radioZip64AsNecessary.Checked)
        ///         options.Zip64 = Zip64Option.AsNecessary;
        ///     else if (this.radioZip64Always.Checked)
        ///         options.Zip64 = Zip64Option.Always;
        ///     else options.Zip64 = Zip64Option.Never;
        ///
        ///     options.Comment = String.Format("Encoding:{0} || Flavor:{1} || ZIP64:{2}\r\nCreated at {3} || {4}\r\n",
        ///                 options.Encoding,
        ///                 FlavorToString(options.ZipFlavor),
        ///                 options.Zip64.ToString(),
        ///                 System.DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
        ///                 this.Text);
        ///
        ///     if (this.tbComment.Text != TB_COMMENT_NOTE)
        ///         options.Comment += this.tbComment.Text;
        ///
        ///     _workerThread = new Thread(this.DoSave);
        ///     _workerThread.Name = "Zip Saver thread";
        ///     _workerThread.Start(options);
        ///     this.Cursor = Cursors.WaitCursor;
        ///  }
        ///
        ///
        /// private void DoSave(Object p)
        /// {
        ///     WorkerOptions options = p as WorkerOptions;
        ///     try
        ///     {
        ///         using (var zip1 = new ZipFile())
        ///         {
        ///             zip1.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding(options.Encoding);
        ///             zip1.Comment = options.Comment;
        ///             zip1.AddDirectory(options.Folder);
        ///             _entriesToZip = zip1.EntryFileNames.Count;
        ///             SetProgressBars();
        ///             zip1.SaveProgress += this.zip1_SaveProgress;
        ///
        ///             zip1.UseZip64WhenSaving = options.Zip64;
        ///
        ///             if (options.ZipFlavor == 1)
        ///                 zip1.SaveSelfExtractor(options.ZipName, SelfExtractorFlavor.WinFormsApplication);
        ///             else if (options.ZipFlavor == 2)
        ///                 zip1.SaveSelfExtractor(options.ZipName, SelfExtractorFlavor.ConsoleApplication);
        ///             else
        ///                 zip1.Save(options.ZipName);
        ///         }
        ///     }
        ///     catch (System.Exception exc1)
        ///     {
        ///         MessageBox.Show(String.Format("Exception while zipping: {0}", exc1.Message));
        ///         btnCancel_Click(null, null);
        ///     }
        /// }
        ///
        ///
        ///
        /// void zip1_SaveProgress(object sender, SaveProgressEventArgs e)
        /// {
        ///     switch (e.EventType)
        ///     {
        ///         case ZipProgressEventType.Saving_AfterWriteEntry:
        ///             StepArchiveProgress(e);
        ///             break;
        ///         case ZipProgressEventType.Saving_EntryBytesRead:
        ///             StepEntryProgress(e);
        ///             break;
        ///         case ZipProgressEventType.Saving_Completed:
        ///             SaveCompleted();
        ///             break;
        ///         case ZipProgressEventType.Saving_AfterSaveTempArchive:
        ///             // this event only occurs when saving an SFX file
        ///             TempArchiveSaved();
        ///             break;
        ///     }
        ///     if (_saveCanceled)
        ///         e.Cancel = true;
        /// }
        ///
        ///
        ///
        /// private void StepArchiveProgress(SaveProgressEventArgs e)
        /// {
        ///     if (this.progressBar1.InvokeRequired)
        ///     {
        ///         this.progressBar1.Invoke(new SaveEntryProgress(this.StepArchiveProgress), new object[] { e });
        ///     }
        ///     else
        ///     {
        ///         if (!_saveCanceled)
        ///         {
        ///             _nFilesCompleted++;
        ///             this.progressBar1.PerformStep();
        ///             _totalBytesAfterCompress += e.CurrentEntry.CompressedSize;
        ///             _totalBytesBeforeCompress += e.CurrentEntry.UncompressedSize;
        ///
        ///             // reset the progress bar for the entry:
        ///             this.progressBar2.Value = this.progressBar2.Maximum = 1;
        ///
        ///             this.Update();
        ///         }
        ///     }
        /// }
        ///
        ///
        /// private void StepEntryProgress(SaveProgressEventArgs e)
        /// {
        ///     if (this.progressBar2.InvokeRequired)
        ///     {
        ///         this.progressBar2.Invoke(new SaveEntryProgress(this.StepEntryProgress), new object[] { e });
        ///     }
        ///     else
        ///     {
        ///         if (!_saveCanceled)
        ///         {
        ///             if (this.progressBar2.Maximum == 1)
        ///             {
        ///                 // reset
        ///                 Int64 max = e.TotalBytesToTransfer;
        ///                 _progress2MaxFactor = 0;
        ///                 while (max > System.Int32.MaxValue)
        ///                 {
        ///                     max /= 2;
        ///                     _progress2MaxFactor++;
        ///                 }
        ///                 this.progressBar2.Maximum = (int)max;
        ///                 lblStatus.Text = String.Format("{0} of {1} files...({2})",
        ///                     _nFilesCompleted + 1, _entriesToZip, e.CurrentEntry.FileName);
        ///             }
        ///
        ///              int xferred = e.BytesTransferred >> _progress2MaxFactor;
        ///
        ///              this.progressBar2.Value = (xferred >= this.progressBar2.Maximum)
        ///                 ? this.progressBar2.Maximum
        ///                 : xferred;
        ///
        ///              this.Update();
        ///         }
        ///     }
        /// }
        ///
        /// private void SaveCompleted()
        /// {
        ///     if (this.lblStatus.InvokeRequired)
        ///     {
        ///         this.lblStatus.Invoke(new MethodInvoker(this.SaveCompleted));
        ///     }
        ///     else
        ///     {
        ///         lblStatus.Text = String.Format("Done, Compressed {0} files, {1:N0}% of original.",
        ///             _nFilesCompleted, (100.00 * _totalBytesAfterCompress) / _totalBytesBeforeCompress);
        ///          ResetState();
        ///     }
        /// }
        ///
        /// private void ResetState()
        /// {
        ///     this.btnCancel.Enabled = false;
        ///     this.btnOk.Enabled = true;
        ///     this.btnOk.Text = "Zip it!";
        ///     this.progressBar1.Value = 0;
        ///     this.progressBar2.Value = 0;
        ///     this.Cursor = Cursors.Default;
        ///     if (!_workerThread.IsAlive)
        ///         _workerThread.Join();
        /// }
        /// </code>
        ///
        /// </example>
        public event EventHandler SaveProgress;

        internal bool OnSaveBlock(ZipEntry entry, Int64 bytesXferred, Int64 totalBytesToXfer)
        {
            if (SaveProgress != null)
            {
                lock (LOCK)
                {
                    SaveProgressEventArgs e = SaveProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
                                  bytesXferred, totalBytesToXfer);
                    SaveProgress(this, e);
                    if (e.Cancel)
                        _saveOperationCanceled = true;
                }
            }
            return _saveOperationCanceled;
        }

        private void OnSaveEntry(int current, ZipEntry entry, bool before)
        {
            if (SaveProgress != null)
            {
                lock (LOCK)
                {
                    SaveProgressEventArgs e = new SaveProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, entry);
                    SaveProgress(this, e);
                    if (e.Cancel)
                        _saveOperationCanceled = true;
                }
            }
        }

        private void OnSaveEvent(ZipProgressEventType eventFlavor)
        {
            if (SaveProgress != null)
            {
                lock (LOCK)
                {
                    SaveProgressEventArgs e = new SaveProgressEventArgs(ArchiveNameForEvent, eventFlavor);
                    SaveProgress(this, e);
                    if (e.Cancel)
                        _saveOperationCanceled = true;
                }
            }
        }

        private void OnSaveStarted()
        {
            if (SaveProgress != null)
            {
                lock (LOCK)
                {
                    SaveProgressEventArgs e = SaveProgressEventArgs.Started(ArchiveNameForEvent);
                    SaveProgress(this, e);
                }
            }
        }
        private void OnSaveCompleted()
        {
            if (SaveProgress != null)
            {
                lock (LOCK)
                {
                    SaveProgressEventArgs e = SaveProgressEventArgs.Completed(ArchiveNameForEvent);
                    SaveProgress(this, e);
                }
            }
        }
        #endregion


        #region Read
        /// <summary>
        /// An event handler invoked before, during, and after the reading of a zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Depending on the particular event being signaled, different properties on the
        /// ReadProgressEventArgs parameter are set.  The following table 
        /// summarizes the available EventTypes and the conditions under which this 
        /// event handler is invoked with a ReadProgressEventArgs with the given EventType.
        /// </para>
        /// 
        /// <list type="table">
        /// <listheader>
        /// <term>value of EntryType</term>
        /// <description>Meaning and conditions</description>
        /// </listheader>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Reading_Started</term>
        /// <description>Fired just as ZipFile.Read() begins. Meaningful properties: ArchiveName.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Reading_Completed</term>
        /// <description>Fired when ZipFile.Read() has completed. Meaningful properties: ArchiveName.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Reading_ArchiveBytesRead</term>
        /// <description>Fired while reading, updates the number of bytes read for the entire archive. 
        /// Meaningful properties: ArchiveName, CurrentEntry, BytesTransferred, TotalBytesToTransfer.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Reading_BeforeReadEntry</term>
        /// <description>Indicates an entry is about to be read from the archive.
        /// Meaningful properties: ArchiveName, EntriesTotal.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Reading_AfterReadEntry</term>
        /// <description>Indicates an entry has just been read from the archive.
        /// Meaningful properties: ArchiveName, EntriesTotal, CurrentEntry.
        /// </description>
        /// </item>
        ///
        /// </list>
        /// </remarks>
        public event EventHandler ReadProgress;

        private void OnReadStarted()
        {
            if (ReadProgress != null)
            {
                lock (LOCK)
                {
                    ReadProgressEventArgs e = ReadProgressEventArgs.Started(ArchiveNameForEvent);
                    ReadProgress(this, e);
                }
            }
        }

        private void OnReadCompleted()
        {
            if (ReadProgress != null)
            {
                lock (LOCK)
                {
                    ReadProgressEventArgs e = ReadProgressEventArgs.Completed(ArchiveNameForEvent);
                    ReadProgress(this, e);
                }
            }
        }

        internal void OnReadBytes(ZipEntry entry)
        {
            if (ReadProgress != null)
            {
                lock (LOCK)
                {
                    ReadProgressEventArgs e = ReadProgressEventArgs.ByteUpdate(ArchiveNameForEvent,
                                        entry,
                                        ReadStream.Position,
                                        LengthOfReadStream);
                    ReadProgress(this, e);
                }
            }
        }

        internal void OnReadEntry(bool before, ZipEntry entry)
        {
            if (ReadProgress != null)
            {
                lock (LOCK)
                {
                    ReadProgressEventArgs e = (before)
                    ? ReadProgressEventArgs.Before(ArchiveNameForEvent, _entries.Count)
                    : ReadProgressEventArgs.After(ArchiveNameForEvent, entry, _entries.Count);
                    ReadProgress(this, e);
                }
            }
        }

        private Int64 _lengthOfReadStream = -99;
        private Int64 LengthOfReadStream
        {
            get
            {
                if (_lengthOfReadStream == -99)
                {
                    if (_ReadStreamIsOurs)
                    {
                        FileInfo fi = new FileInfo(_name);
                        _lengthOfReadStream = fi.Length;
                    }
                    else _lengthOfReadStream = -1;
                }
                return _lengthOfReadStream;
            }
        }
        #endregion


        #region Extract
        /// <summary>
        /// An event handler invoked before, during, and after extraction of entries 
        /// in the zip archive. 
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Depending on the particular event, different properties on the
        /// ExtractProgressEventArgs parameter are set.  The following table 
        /// summarizes the available EventTypes and the conditions under which this 
        /// event handler is invoked with a ExtractProgressEventArgs with the given EventType.
        /// </para>
        /// 
        /// <list type="table">
        /// <listheader>
        /// <term>value of EntryType</term>
        /// <description>Meaning and conditions</description>
        /// </listheader>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Extracting_BeforeExtractAll</term>
        /// <description>Set when ExtractAll() begins.  The ArchiveName, Overwrite,
        /// and ExtractLocation properties are meaningful.</description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Extracting_AfterExtractAll</term>
        /// <description>Set when ExtractAll() has completed.  The ArchiveName, 
        /// Overwrite, and ExtractLocation properties are meaningful.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Extracting_BeforeExtractEntry</term>
        /// <description>Set when an Extract() on an entry in the ZipFile has begun.  
        /// Properties that are meaningful:  ArchiveName, EntriesTotal, CurrentEntry, Overwrite, 
        /// ExtractLocation, EntriesExtracted.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Extracting_AfterExtractEntry</term>
        /// <description>Set when an Extract() on an entry in the ZipFile has completed.  
        /// Properties that are meaningful:  ArchiveName, EntriesTotal, CurrentEntry, Overwrite, 
        /// ExtractLocation, EntriesExtracted.
        /// </description>
        /// </item>
        /// 
        /// <item>
        /// <term>ZipProgressEventType.Extracting_EntryBytesWritten</term>
        /// <description>Set within a call to Extract() on an entry in the ZipFile, as
        /// data is extracted for the entry.  Properties that are meaningful:  ArchiveName, 
        /// CurrentEntry, BytesTransferred, TotalBytesToTransfer. 
        /// </description>
        /// </item>
        ///
        /// <item>
        /// <term>ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite</term>
        /// <description>Set within a call to Extract() on an entry in the ZipFile, when the
        /// extraction would overwrite an existing file. This event type is used only when
        /// <c>ExtractExistingFileAction</c> on the <c>ZipFile</c> or <c>ZipEntry</c> is set
        /// to <c>InvokeExtractProgressEvent</c>.
        /// </description>
        /// </item>
        /// 
        /// </list>
        /// 
        /// </remarks>
        ///
        /// <example>
        /// <code>
        /// private static bool justHadByteUpdate = false;
        /// public static void ExtractProgress(object sender, ExtractProgressEventArgs e)
        /// {
        ///   if(e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
        ///   {
        ///     if (justHadByteUpdate)
        ///       Console.SetCursorPosition(0, Console.CursorTop);
        ///
        ///     Console.Write("   {0}/{1} ({2}%)", e.BytesTransferred, e.TotalBytesToTransfer,
        ///                   e.BytesTransferred / (0.01 * e.TotalBytesToTransfer ));
        ///     justHadByteUpdate = true;
        ///   }
        ///   else if(e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
        ///   {
        ///     if (justHadByteUpdate) 
        ///       Console.WriteLine();
        ///     Console.WriteLine("Extracting: {0}", e.CurrentEntry.FileName);
        ///     justHadByteUpdate= false;
        ///   }
        /// }
        ///
        /// public static ExtractZip(string zipToExtract, string directory)
        /// {
        ///   string TargetDirectory= "extract";
        ///   using (var zip = ZipFile.Read(zipToExtract)) {
        ///     zip.ExtractProgress += ExtractProgress; 
        ///     foreach (var e in zip1)
        ///     {
        ///       e.Extract(TargetDirectory, true);
        ///     }
        ///   }
        /// }
        ///
        /// </code>
        /// <code lang="VB">
        /// Public Shared Sub Main(ByVal args As String())
        ///     Dim ZipToUnpack As String = "C1P3SML.zip"
        ///     Dim TargetDir As String = "ExtractTest_Extract"
        ///     Console.WriteLine("Extracting file {0} to {1}", ZipToUnpack, TargetDir)
        ///     Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
        ///         AddHandler zip1.ExtractProgress, AddressOf MyExtractProgress
        ///         Dim e As ZipEntry
        ///         For Each e In zip1
        ///             e.Extract(TargetDir, True)
        ///         Next
        ///     End Using
        /// End Sub
        /// 
        /// Private Shared justHadByteUpdate As Boolean = False
        /// 
        /// Public Shared Sub MyExtractProgress(ByVal sender As Object, ByVal e As ExtractProgressEventArgs)
        ///     If (e.EventType = ZipProgressEventType.Extracting_EntryBytesWritten) Then
        ///         If ExtractTest.justHadByteUpdate Then
        ///             Console.SetCursorPosition(0, Console.CursorTop)
        ///         End If
        ///         Console.Write("   {0}/{1} ({2}%)", e.BytesTransferred, e.TotalBytesToTransfer, (CDbl(e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)))
        ///         ExtractTest.justHadByteUpdate = True
        ///     ElseIf (e.EventType = ZipProgressEventType.Extracting_BeforeExtractEntry) Then
        ///         If ExtractTest.justHadByteUpdate Then
        ///             Console.WriteLine
        ///         End If
        ///         Console.WriteLine("Extracting: {0}", e.CurrentEntry.FileName)
        ///         ExtractTest.justHadByteUpdate = False
        ///     End If
        /// End Sub
        /// </code>
        /// </example>
        public event EventHandler ExtractProgress;



        private void OnExtractEntry(int current, bool before, ZipEntry currentEntry, string path)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = new ExtractProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, currentEntry, path);
                    ExtractProgress(this, e);
                    if (e.Cancel)
                        _extractOperationCanceled = true;
                }
            }
        }


        // Can be called from within ZipEntry._ExtractOne.
        internal bool OnExtractBlock(ZipEntry entry, Int64 bytesWritten, Int64 totalBytesToWrite)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = ExtractProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
                                bytesWritten, totalBytesToWrite);
                    ExtractProgress(this, e);
                    if (e.Cancel)
                        _extractOperationCanceled = true;
                }
            }
            return _extractOperationCanceled;
        }


        // Can be called from within ZipEntry.InternalExtract.
        internal bool OnSingleEntryExtract(ZipEntry entry, string path, bool before)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = (before)
            ? ExtractProgressEventArgs.BeforeExtractEntry(ArchiveNameForEvent, entry, path)
            : ExtractProgressEventArgs.AfterExtractEntry(ArchiveNameForEvent, entry, path);
                    ExtractProgress(this, e);
                    if (e.Cancel)
                        _extractOperationCanceled = true;
                }
            }
            return _extractOperationCanceled;
        }

        internal bool OnExtractExisting(ZipEntry entry, string path)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = ExtractProgressEventArgs.ExtractExisting(ArchiveNameForEvent, entry, path);
                    ExtractProgress(this, e);
                    if (e.Cancel)
                        _extractOperationCanceled = true;
                }
            }
            return _extractOperationCanceled;
        }


        private void OnExtractAllCompleted(string path)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = ExtractProgressEventArgs.ExtractAllCompleted(ArchiveNameForEvent,
                         path );
                    ExtractProgress(this, e);
                }
            }
        }


        private void OnExtractAllStarted(string path)
        {
            if (ExtractProgress != null)
            {
                lock (LOCK)
                {
                    ExtractProgressEventArgs e = ExtractProgressEventArgs.ExtractAllStarted(ArchiveNameForEvent,
                         path );
                    ExtractProgress(this, e);
                }
            }
        }


        #endregion



        #region Add
        /// <summary>
        /// An event handler invoked before, during, and after Adding entries to a zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///     Adding a large number of entries to a zip file can take a long
        ///     time.  For example, when calling <see cref="AddDirectory(string)"/> on a
        ///     directory that contains 50,000 files, it could take 3 minutes or so.
        ///     This event handler allws an application to track the progress of the Add
        ///     operation.
        /// </remarks>
        ///
        /// <example>
        /// <code lang="C#">
        ///
        /// int _numEntriesToAdd= 0;
        /// int _numEntriesAdded= 0;
        /// void AddProgressHandler(object sender, AddProgressEventArgs e)
        /// {
        ///     switch (e.EventType)
        ///     {
        ///         case ZipProgressEventType.Adding_Started:
        ///             Console.WriteLine("Adding files to the zip...");
        ///             break;
        ///          case ZipProgressEventType.Adding_AfterAddEntry:
        ///             _numEntriesAdded++;
        ///             Console.WriteLine(String.Format("Adding file {0}/{1} :: {2}",
        ///                                      _numEntriesAdded, _numEntriesToAdd, e.CurrentEntry.FileName));
        ///             break;
        ///             
        ///         case ZipProgressEventType.Adding_Completed:
        ///             Console.WriteLine("Added all files");
        ///             break;
        ///     }
        /// }
        ///    
        /// void CreateTheZip()
        /// {
        ///     using (ZipFile zip = new ZipFile())
        ///     {
        ///         zip.AddProgress += AddProgressHandler;
        ///         zip.AddDirectory(System.IO.Path.GetFileName(DirToZip));
        ///         zip.BufferSize = 4096;
        ///         zip.SaveProgress += SaveProgressHandler;
        ///         zip.Save(ZipFileToCreate);
        ///     }
        /// }
        ///     
        /// </code>
        /// </example>
        public event EventHandler AddProgress;

        private void OnAddStarted()
        {
            if (AddProgress != null)
            {
                lock (LOCK)
                {
                    AddProgressEventArgs e = AddProgressEventArgs.Started(ArchiveNameForEvent);
                    AddProgress(this, e);
                }
            }
        }

        private void OnAddCompleted()
        {
            if (AddProgress != null)
            {
                lock (LOCK)
                {
                    AddProgressEventArgs e = AddProgressEventArgs.Completed(ArchiveNameForEvent);
                    AddProgress(this, e);
                }
            }
        }

        internal void AfterAddEntry(ZipEntry entry)
        {
            if (AddProgress != null)
            {
                lock (LOCK)
                {
                    AddProgressEventArgs e = AddProgressEventArgs.AfterEntry(ArchiveNameForEvent, entry, _entries.Count);
                    AddProgress(this, e);
                }
            }
        }

        #endregion


        
        #region Error
        /// <summary>
        /// An event handler invoked when an error occurs during open or read of files
        /// while saving a zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///     In some cases an error will occur when a file to be added to the zip
        ///     archive is opened.  In other cases, an error might occur after the file
        ///     has been successfully opened, while reading the file.
        ///  </para>
        /// 
        ///  <para>
        ///    The first problem might occur when calling Adddirectory() on a directory
        ///    that contains a Clipper .dbf file; the file is locked by Clipper and
        ///    cannot be opened bby another process. An example of the second problem is
        ///    the ERROR_LOCK_VIOLATION that results when a file is opened by another
        ///    process, but not locked, and a range lock has been taken on the file.
        ///    Microsoft Outlook takes range locks on .PST files.
        ///  </para>
        ///
        /// </remarks>
        ///
        public event EventHandler ZipError;

        internal bool OnZipErrorSaving(ZipEntry entry, Exception exc)
        {
            if (ZipError != null)
            {
                lock (LOCK)
                {
                    ZipErrorEventArgs e = ZipErrorEventArgs.Saving(Name, entry, exc);
                    ZipError(this, e);
                    if (e.Cancel)
                        _saveOperationCanceled = true;
                }
            }
            return _saveOperationCanceled;
        }
        #endregion

#endregion

        #region Extract

        /// <summary>
        /// Extracts all of the items in the zip archive, to the specified path in the
        /// filesystem.  The path can be relative or fully-qualified.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method will extract all entries in the <c>ZipFile</c> to the specified path. 
        /// </para>
        ///
        /// <para>
        /// If an extraction of a file from the zip archive would overwrite an existing
        /// file in the filesystem, the action taken is dictated by the
        /// ExtractExistingFile property, which overrides any setting you may have made
        /// on individual ZipEntry instances.  By default, if you have not set that
        /// property on the <c>ZipFile</c> instance, the entry will not be extracted,
        /// the existing file will not be overwritten and an exception will be
        /// thrown. To change this, set the property, or use the <see
        /// cref="ZipFile.ExtractAll(string, ExtractExistingFileAction)" />
        /// overload that allows you to specify an ExtractExistingFileAction parameter.
        /// </para>
        ///
        /// <para>
        /// The action to take when an extract would overwrite an existing file applies
        /// to all entries.  If you want to set this on a per-entry basis, then you must
        /// use one of the <see cef="ZipEntry.Extract" >ZipEntry.Extract</see> methods.
        /// </para>
        ///
        /// <para>
        /// This method will send verbose output messages to the
        /// StatusMessageTextWriter, if it is set on the <c>ZipFile</c> instance.
        /// </para>
        ///
        /// <para>
        /// You may wish to take advantage of the <c>ExtractProgress</c> event.
        /// </para>
        ///
        /// <para>
        /// About Timestamps: When extracting a file entry from a zip archive, the
        /// extracted file gets the last modified time of the entry as stored in the
        /// archive. The archive may also store extended file timestamp information,
        /// including last accessed and created times. If these are present in the
        /// ZipEntry, then the extracted file will also get these times.
        /// </para>
        ///
        /// <para>
        /// A Directory entry is somewhat different. It will get the times as described
        /// for a file entry, but, if there are file entries in the zip archive that,
        /// when extracted, appear in the just-created directory, then when those file
        /// entries are extracted, the last modified and last accessed times of the
        /// directory will change, as a side effect.  The result is that after an
        /// extraction of a directory and a number of files within the directory, the
        /// last modified and last accessed timestamps on the directory will reflect the
        /// time that the last file was extracted into the directory, rather than the
        /// time stored in the zip archive for the directory.
        /// </para>
        ///
        /// <para>
        /// To compensate, when extracting an archive with <c>ExtractAll</c>, DotNetZip
        /// will extract all the file and directory entries as described above, but it
        /// will then make a second pass on the directories, and reset the times on the
        /// directories to reflect what is stored in the zip archive.
        /// </para>
        ///
        /// <para>
        /// This compensation is performed only within the context of an
        /// <c>ExtractAll</c>. If you call <c>ZipEntry.Extract</c> on a directory entry,
        /// the timestamps on directory in the filesystem will reflect the times stored
        /// in the zip.  If you then call <c>ZipEntry.Extract</c> on a file entry, which
        /// is extracted into the directory, the timestamps on the directory will be
        /// updated to the current time.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// This example extracts all the entries in a zip archive file, to the
        /// specified target directory.  The extraction will overwrite any existing
        /// files silently.
        /// <code>
        /// String TargetDirectory= "unpack";
        /// using(ZipFile zip= ZipFile.Read(ZipFileToExtract))
        /// {
        ///     zip.ExtractExistingFile= ExtractExistingFileAction.OverwriteSilently;
        ///     zip.ExtractAll(TargetDirectory);
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        /// Dim TargetDirectory As String = "unpack"
        /// Using zip As ZipFile = ZipFile.Read(ZipFileToExtract)
        ///     zip.ExtractExistingFile= ExtractExistingFileAction.OverwriteSilently
        ///     zip.ExtractAll(TargetDirectory)
        /// End Using
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Aspose.Zip.ZipFile.ExtractProgress"/>
        /// <seealso cref="Aspose.Zip.ZipFile.ExtractExistingFile"/>
        ///
        /// <param name="path">
        /// The path to which the contents of the zipfile will be extracted.
        /// The path can be relative or fully-qualified. 
        /// </param>
        ///
        public void ExtractAll(string path)
        {
            _InternalExtractAll(path, true);
        }

        /// <summary>
        /// Extracts all of the items in the zip archive, to the specified path in the
        /// filesystem, using the specified behavior when extraction would overwrite an
        /// existing file.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// This method will extract all entries in the <c>ZipFile</c> to the specified
        /// path.  For an extraction that would overwrite an existing file, the behavior
        /// is dictated by the extractExistingFile parameter, which overrides any
        /// setting you may have made on individual ZipEntry instances.
        /// </para>
        ///
        /// <para>
        /// The action to take when an extract would overwrite an existing file applies
        /// to all entries.  If you want to set this on a per-entry basis, then you must
        /// use one of the <see cef="ZipEntry.Extract" /> methods.
        /// </para>
        ///
        /// <para>
        /// Calling this method is equivalent to setting the <see
        /// cref="ExtractExistingFile"/> property and then calling <see
        /// cref="ExtractAll(String)"/>.
        /// </para>
        ///
        /// <para>
        /// This method will send verbose output messages to the
        /// StatusMessageTextWriter, if it is set on the <c>ZipFile</c> instance.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// This example extracts all the entries in a zip archive file, to the
        /// specified target directory.  It does not overwrite any existing files.
        /// <code>
        /// String TargetDirectory= "c:\\unpack";
        /// using(ZipFile zip= ZipFile.Read(ZipFileToExtract))
        /// {
        ///   zip.ExtractAll(TargetDirectory, ExtractExistingFileAction.DontOverwrite);
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        /// Dim TargetDirectory As String = "c:\unpack"
        /// Using zip As ZipFile = ZipFile.Read(ZipFileToExtract)
        ///     zip.ExtractAll(TargetDirectory, ExtractExistingFileAction.DontOverwrite)
        /// End Using
        /// </code>
        /// </example>
        /// 
        /// <param name="path">
        /// The path to which the contents of the zipfile will be extracted.
        /// The path can be relative or fully-qualified. 
        /// </param>
        ///
        /// <param name="extractExistingFile">
        /// The action to take if extraction would overwrite an existing file.
        /// </param>
        /// <seealso cref="ExtractSelectedEntries(String,ExtractExistingFileAction)"/>
        public void ExtractAll(string path, ExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            _InternalExtractAll(path, true);
        }


        private void _InternalExtractAll(string path, bool overrideExtractExistingProperty)
        {
            bool header = Verbose;
            _inExtractAll = true;
            try
            {
                OnExtractAllStarted(path);

                int n = 0;
                foreach (ZipEntry e in _entries)
                {
                    if (header)
                    {
                        StatusMessageTextWriter.WriteLine("\n{1,-22} {2,-8} {3,4}   {4,-8}  {0}",
                                  "Name", "Modified", "Size", "Ratio", "Packed");
                        StatusMessageTextWriter.WriteLine(new String('-', 72));
                        header = false;
                    }
                    if (Verbose)
                    {
                        StatusMessageTextWriter.WriteLine("{1,-22} {2,-8} {3,4:F0}%   {4,-8} {0}",
                                  e.FileName,
                                  e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                                  e.UncompressedSize,
                                  e.CompressionRatio,
                                  e.CompressedSize);
                        if (e.Comment != null && e.Comment.Length > 0)
                            StatusMessageTextWriter.WriteLine("  Comment: {0}", e.Comment);
                    }
                    e.Password = _Password;  // this may be null
                    OnExtractEntry(n, true, e, path);
                    if (overrideExtractExistingProperty)
                        e.ExtractExistingFile = ExtractExistingFile;
                    e.Extract(path);
                    n++;
                    OnExtractEntry(n, false, e, path);
                    if (_extractOperationCanceled)
                        break;

                }

                // workitem 8264: 
                // now, set times on directory entries, again.
                // The problem is, extracting a file changes the times on the parent
                // directory.  So after all files have been extracted, we have to
                // run through the directories again. 
                foreach (ZipEntry e in _entries)
                {
                    // check if it is a directory
                    if ((e.IsDirectory) || (e.FileName.EndsWith("/", StringComparison.Ordinal)))
                    {
                        string outputFile = (e.FileName.StartsWith("/", StringComparison.Ordinal))
                            ? Path.Combine(path, e.FileName.Substring(1))
                            : Path.Combine(path, e.FileName);
                        
                        e._SetTimes(outputFile, false);
                    }
                }
                
                OnExtractAllCompleted(path);
            }
            finally
            {

                _inExtractAll = false;
            }
        }

#endregion

        #region Read

        /// <summary>
        /// Reads a zip file archive and returns the instance.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The stream is read using the default <c>System.Text.Encoding</c>, which is the
        /// <c>IBM437</c> codepage.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="System.Exception">
        /// Thrown if the <c>ZipFile</c> cannot be read. The implementation of this method
        /// relies on <c>System.IO.File.OpenRead</c>, which can throw a variety of exceptions,
        /// including specific exceptions if a file is not found, an unauthorized access
        /// exception, exceptions for poorly formatted filenames, and so on.
        /// </exception>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  This can be a fully-qualified or relative
        /// pathname.
        /// </param>
        /// 
        /// <overloads>This method has a bunch of interesting overloads. They are all
        /// static (Shared in VB).  One of them is bound to be right for you.  The
        /// reason there are so many is that there are a few properties on the
        /// <c>ZipFile</c> class that must be set before you read the zipfile in, for
        /// them to be useful.  The set of overloads covers the most interesting cases.
        /// Probably there are still too many, though.</overloads>
        ///
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName)
        {
            return Read(fileName, null, DefaultEncoding);
        }

        /// <summary>
        /// Reads a zip file archive and returns the instance, using the specified
        /// ReadProgress event handler.  
        /// </summary>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName, EventHandler readProgress)
        {
            return Read(fileName, null, DefaultEncoding, readProgress);
        }

        /// <summary>
        /// Reads a zip file archive using the specified text encoding, and returns the
        /// instance.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This version of the method allows the caller to pass in a <c>TextWriter</c>.  
        /// The ZipFile is read in using the default IBM437 encoding for entries where UTF-8 
        /// encoding is not explicitly specified.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// <code lang="C#">
        /// var sw = new System.IO.StringWriter();
        /// using (ZipFile zip =  ZipFile.Read("PackedDocuments.zip", sw))
        /// {
        ///   var Threshold = new DateTime(2007,7,4);
        ///   // We cannot remove the entry from the list, within the context of 
        ///   // an enumeration of said list.
        ///   // So we add the doomed entry to a list to be removed later.
        ///   // pass 1: mark the entries for removal
        ///   var MarkedEntries = new System.Collections.Generic.List&lt;ZipEntry&gt;();
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (e.LastModified &lt; Threshold)
        ///       MarkedEntries.Add(e);
        ///   }
        ///   // pass 2: actually remove the entry. 
        ///   foreach (ZipEntry zombie in MarkedEntries)
        ///      zip.RemoveEntry(zombie);
        ///   zip.Comment = "This archive has been updated.";
        ///   zip.Save();
        /// }
        /// // can now use contents of sw, eg store in an audit log
        /// </code>
        ///
        /// <code lang="VB">
        ///   Dim sw As New System.IO.StringWriter
        ///   Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip", sw)
        ///       Dim Threshold As New DateTime(2007, 7, 4)
        ///       ' We cannot remove the entry from the list, within the context of 
        ///       ' an enumeration of said list.
        ///       ' So we add the doomed entry to a list to be removed later.
        ///       ' pass 1: mark the entries for removal
        ///       Dim MarkedEntries As New System.Collections.Generic.List(Of ZipEntry)
        ///       Dim e As ZipEntry
        ///       For Each e In zip
        ///           If (e.LastModified &lt; Threshold) Then
        ///               MarkedEntries.Add(e)
        ///           End If
        ///       Next
        ///       ' pass 2: actually remove the entry. 
        ///       Dim zombie As ZipEntry
        ///       For Each zombie In MarkedEntries
        ///           zip.RemoveEntry(zombie)
        ///       Next
        ///       zip.Comment = "This archive has been updated."
        ///       zip.Save
        ///   End Using
        ///   ' can now use contents of sw, eg store in an audit log
        /// </code>
        /// </example>
        /// 
        /// <exception cref="System.Exception">
        /// Thrown if the zipfile cannot be read. The implementation of this 
        /// method relies on <c>System.IO.File.OpenRead</c>, which can throw
        /// a variety of exceptions, including specific exceptions if a file
        /// is not found, an unauthorized access exception, exceptions for
        /// poorly formatted filenames, and so on. 
        /// </exception>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
        /// during operations on the zip archive.  A console application may wish to
        /// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
        /// or headless application may wish to capture the messages in a different
        /// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
        /// </param>
        /// 
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName, TextWriter statusMessageWriter)
        {
            return Read(fileName, statusMessageWriter, DefaultEncoding);
        }


        /// <summary>
        /// Reads a zip file archive using the specified text encoding, and the
        /// specified ReadProgress event handler, and returns the instance.  
        /// </summary>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
        /// during operations on the zip archive.  A console application may wish to
        /// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
        /// or headless application may wish to capture the messages in a different
        /// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
        /// </param>
        /// 
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName,
                   TextWriter statusMessageWriter,
                   EventHandler readProgress)
        {
            return Read(fileName, statusMessageWriter, DefaultEncoding, readProgress);
        }

        /// <summary>
        /// Reads a zip file archive using the specified text encoding, and returns the instance.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This version of the method allows the caller to pass in an <c>Encoding</c>.  
        /// The ZipFile is read in using the specified encoding for entries where UTF-8
        /// encoding is not explicitly specified.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// This example shows how to read a zip file using the Big-5 Chinese code page
        /// (950), and extract each entry in the zip file.  For this code to work as
        /// desired, the zipfile must have been created using the big5 code page
        /// (CP950). This is typical, for example, when using WinRar on a machine with
        /// CP950 set as the default code page.  In that case, the names of entries
        /// within the Zip archive will be stored in that code page, and reading the zip
        /// archive must be done using that code page.  If the application did not use
        /// the correct code page in ZipFile.Read(), then names of entries within the
        /// zip archive would not be correctly retrieved.
        /// <code lang="C#">
        /// using (ZipFile zip = ZipFile.Read(ZipToExtract,
        ///                                   System.Text.Encoding.GetEncoding(950)))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///      e.Extract(extractDirectory);
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(ZipToExtract, System.Text.Encoding.GetEncoding(950))
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///      e.Extract(extractDirectory)
        ///     Next
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <exception cref="System.Exception">
        /// Thrown if the zipfile cannot be read. The implementation of this 
        /// method relies on <c>System.IO.File.OpenRead</c>, which can throw
        /// a variety of exceptions, including specific exceptions if a file
        /// is not found, an unauthorized access exception, exceptions for
        /// poorly formatted filenames, and so on. 
        /// </exception>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="encoding">
        /// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
        /// careful specifying the encoding.  If the value you use here is not the same
        /// as the Encoding used when the zip archive was created (possibly by a
        /// different archiver) you will get unexpected results and possibly exceptions.
        /// </param>
        /// 
        /// <seealso cref="ProvisionalAlternateEncoding"/>.
        ///
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName, System.Text.Encoding encoding)
        {
            return Read(fileName, null, encoding);
        }


        /// <summary>
        /// Reads a zip file archive using the specified text encoding and ReadProgress
        /// event handler, and returns the instance.  
        /// </summary>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <param name="encoding">
        /// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
        /// careful specifying the encoding.  If the value you use here is not the same
        /// as the Encoding used when the zip archive was created (possibly by a
        /// different archiver) you will get unexpected results and possibly exceptions.
        /// </param>
        /// 
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string fileName,
                                   System.Text.Encoding encoding,
                                   EventHandler readProgress)
        {
            return Read(fileName, null, encoding, readProgress);
        }


        /// <summary>
        /// Reads a zip file archive using the specified text encoding and the specified
        /// TextWriter for status messages, and returns the instance.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This version of the method allows the caller to pass in a <c>TextWriter</c>
        /// and an <c>Encoding</c>.  The ZipFile is read in using the specified encoding
        /// for entries where UTF-8 encoding is not explicitly specified.
        /// </para>
        /// </remarks>
        /// 
        /// 
        /// <example>
        /// This example shows how to read a zip file using the Big-5 Chinese code page
        /// (950), and extract each entry in the zip file, while sending status messages
        /// out to the Console.
        /// <code lang="C#">
        /// using (ZipFile zip = ZipFile.Read(ZipToExtract,
        ///                                   System.Console.Out,
        ///                                   System.Text.Encoding.GetEncoding(950)))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///      e.Extract(extractDirectory);
        ///   }
        /// }
        /// </code>
        /// </example>
        ///
        /// <exception cref="System.Exception">
        /// Thrown if the zipfile cannot be read. The implementation of this 
        /// method relies on <c>System.IO.File.OpenRead</c>, which can throw
        /// a variety of exceptions, including specific exceptions if a file
        /// is not found, an unauthorized access exception, exceptions for
        /// poorly formatted filenames, and so on. 
        /// </exception>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
        /// during operations on the zip archive.  A console application may wish to
        /// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
        /// or headless application may wish to capture the messages in a different
        /// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
        /// </param>
        /// 
        /// <param name="encoding">
        /// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
        /// careful specifying the encoding.  If the value you use here is not the same
        /// as the Encoding used when the zip archive was created (possibly by a
        /// different archiver) you will get unexpected results and possibly exceptions.
        /// </param>
        /// 
        /// <seealso cref="ProvisionalAlternateEncoding"/>
        ///
        /// <returns>The instance read from the zip archive.</returns>
        /// 
        public static ZipFile Read(string fileName,
                                   TextWriter statusMessageWriter,
                                   System.Text.Encoding encoding)
        {
            return Read(fileName, statusMessageWriter, encoding, null);
        }

        /// <summary>
        /// Reads a zip file archive using the specified text encoding,  the specified
        /// TextWriter for status messages, and the specified ReadProgress event handler, 
        /// and returns the instance.  
        /// </summary>
        /// 
        /// <param name="fileName">
        /// The name of the zip archive to open.  
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
        /// during operations on the zip archive.  A console application may wish to
        /// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
        /// or headless application may wish to capture the messages in a different
        /// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
        /// </param>
        /// 
        /// <param name="encoding">
        /// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
        /// careful specifying the encoding.  If the value you use here is not the same
        /// as the Encoding used when the zip archive was created (possibly by a
        /// different archiver) you will get unexpected results and possibly exceptions.
        /// </param>
        /// 
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string fileName,
                                   TextWriter statusMessageWriter,
                                   System.Text.Encoding encoding,
                                   EventHandler readProgress)
        {
            ZipFile zf = new ZipFile();
            zf.ProvisionalAlternateEncoding = encoding;
            zf._StatusMessageTextWriter = statusMessageWriter;
            zf._name = fileName;
            if (readProgress != null)
                zf.ReadProgress = readProgress;

            if (zf.Verbose) zf._StatusMessageTextWriter.WriteLine("reading from {0}...", fileName);

            try
            {
                ReadIntoInstance(zf);
                zf._fileAlreadyExists = true;
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} could not be read", fileName), e1);
            }
            return zf;
        }

        /// <summary>
        /// Reads a zip archive from a stream.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This is useful when the zip archive content is available from an
        /// already-open stream. The stream must be open and readable when calling this
        /// method.  The stream is left open when the reading is completed.
        /// </para>
        ///
        /// <para>
        /// Using this overload, the stream is read using the default
        /// <c>System.Text.Encoding</c>, which is the <c>IBM437</c> codepage. If you
        /// want to specify the encoding to use when reading the zipfile content, check
        /// out the other overloads of the ZipFile constructor.
        /// </para>
        ///
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <para>
        /// This example shows how to Read zip content from a stream, and extract one
        /// entry into a different stream. In this example, the filename
        /// "NameOfEntryInArchive.doc", refers only to the name of the entry within the
        /// zip archive.  A file by that name is not created in the filesystem.  The I/O
        /// is done strictly with the given streams.
        /// </para>
        /// 
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(InputStream))
        /// {
        ///    zip.Extract("NameOfEntryInArchive.doc", OutputStream);
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using zip as ZipFile = ZipFile.Read(InputStream)
        ///    zip.Extract("NameOfEntryInArchive.doc", OutputStream)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream)
        {
            return Read(zipStream, null, DefaultEncoding);
        }

        /// <summary>
        /// Reads a zip archive from a stream, with a given ReadProgress event handler.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// When opening large zip archives, you may want to display a progress bar or
        /// other indicator of status progress while reading.  This Read() method allows
        /// you to specify a ReadProgress Event Handler directly.  The stream is read
        /// using the default encoding (IBM437).  
        /// </para>
        ///
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile corresponding to the stream being read.</returns>
        public static ZipFile Read(Stream zipStream,
                                   EventHandler readProgress)
        {
            return Read(zipStream, null, DefaultEncoding, readProgress);
        }


        /// <summary>
        /// Reads a zip archive from a stream, using the specified TextWriter for status
        /// messages.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is useful when the zip archive content is available from 
        /// an already-open stream. The stream must be open and readable when calling this
        /// method.  The stream is left open when the reading is completed. 
        /// </para>
        /// 
        /// <para>
        /// The stream is read using the default <c>System.Text.Encoding</c>, which is
        /// the <c>IBM437</c> codepage.  For more information on the encoding, see the
        /// <see cref="ProvisionalAlternateEncoding"/> property.
        /// </para>
        /// 
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <exception cref="ZipException">
        /// Thrown if zipStream is <c>null</c> (<c>Nothing</c> in VB).
        /// In this case, the inner exception is an ArgumentException.
        /// </exception>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream, TextWriter statusMessageWriter)
        {
            return Read(zipStream, statusMessageWriter, DefaultEncoding);
        }


        /// <summary>
        /// Reads a zip archive from a stream, using the specified TextWriter for status
        /// messages, and the specified ReadProgress event handler.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The stream is read using the default <c>System.Text.Encoding</c>, which is
        /// the <c>IBM437</c> codepage.  For more information on the encoding, see the
        /// <see cref="ProvisionalAlternateEncoding"/> property.
        /// </para>
        /// 
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        /// 
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream,
                                   TextWriter statusMessageWriter,
                                   EventHandler readProgress)
        {
            return Read(zipStream, statusMessageWriter, DefaultEncoding, readProgress);
        }

        /// <summary>
        /// Reads a zip archive from a stream, using the specified encoding.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is useful when the zip archive content is available from 
        /// an already-open stream. The stream must be open and readable when calling this
        /// method.  The stream is left open when the reading is completed. 
        /// </para>
        ///
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="ZipException">
        /// Thrown if zipStream is <c>null</c> (<c>Nothing</c> in VB).
        /// In this case, the inner exception is an ArgumentException.
        /// </exception>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        /// 
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8
        /// encoding bit set.  Be careful specifying the encoding.  If the value you use
        /// here is not the same as the Encoding used when the zip archive was created
        /// (possibly by a different archiver) you will get unexpected results and
        /// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream, System.Text.Encoding encoding)
        {
            return Read(zipStream, null, encoding);
        }

        /// <summary>
        /// Reads a zip archive from a stream, using the specified encoding, and
        /// and the specified ReadProgress event handler.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        /// 
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8
        /// encoding bit set.  Be careful specifying the encoding.  If the value you use
        /// here is not the same as the Encoding used when the zip archive was created
        /// (possibly by a different archiver) you will get unexpected results and
        /// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream,
                                   System.Text.Encoding encoding,
                                   EventHandler readProgress)
        {
            return Read(zipStream, null, encoding, readProgress);
        }

        /// <summary>
        /// Reads a zip archive from a stream, using the specified text Encoding and the 
        /// specified TextWriter for status messages.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method is useful when the zip archive content is available from an
        /// already-open stream. The stream must be open and readable when calling this
        /// method.  The stream is left open when the reading is completed.
        /// </para>
        ///
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="ZipException">
        /// Thrown if zipStream is <c>null</c> (<c>Nothing</c> in VB).
        /// In this case, the inner exception is an ArgumentException.
        /// </exception>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8
        /// encoding bit set.  Be careful specifying the encoding.  If the value you use
        /// here is not the same as the Encoding used when the zip archive was created
        /// (possibly by a different archiver) you will get unexpected results and
        /// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream,
                   TextWriter statusMessageWriter,
                   System.Text.Encoding encoding)
        {
            return Read(zipStream, statusMessageWriter, encoding, null);
        }


        /// <summary>
        /// Reads a zip archive from a stream, using the specified text Encoding, the 
        /// specified TextWriter for status messages, 
        /// and the specified ReadProgress event handler.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Reading of zip content begins at the current position in the stream.  This
        /// means if you have a stream that concatenates regular data and zip data, if
        /// you position the open, readable stream at the start of the zip data, you
        /// will be able to read the zip archive using this constructor, or any of the
        /// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
        /// input. Some examples of where this might be useful: the zip content is
        /// concatenated at the end of a regular EXE file, as some self-extracting
        /// archives do.  (Note: SFX files produced by DotNetZip do not work this
        /// way). Another example might be a stream being read from a database, where
        /// the zip content is embedded within an aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8
        /// encoding bit set.  Be careful specifying the encoding.  If the value you use
        /// here is not the same as the Encoding used when the zip archive was created
        /// (possibly by a different archiver) you will get unexpected results and
        /// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <param name="readProgress">
        /// An event handler for Read operations.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(Stream zipStream,
                                   TextWriter statusMessageWriter,
                                   System.Text.Encoding encoding,
                                   EventHandler readProgress)
        {
            if (zipStream == null)
                throw new ZipException("Cannot read.", new ArgumentException("The stream must be non-null", "zipStream"));

            ZipFile zf = new ZipFile();
            zf._provisionalAlternateEncoding = encoding;
            if (readProgress != null)
                zf.ReadProgress += readProgress;
            zf._StatusMessageTextWriter = statusMessageWriter;
            zf._readstream = zipStream;
            zf._ReadStreamIsOurs = false;
            if (zf.Verbose) zf._StatusMessageTextWriter.WriteLine("reading from stream...");

            ReadIntoInstance(zf);
            return zf;
        }


        /// <summary>
        /// Reads a zip archive from a byte array.
        /// </summary>
        /// 
        /// <remarks>
        /// This is useful when the data for the zipfile is contained in a byte array, 
        /// for example, downloaded from an FTP server without being saved to a
        /// filesystem. 
        /// </remarks>
        /// 
        /// <param name="buffer">
        /// The byte array containing the zip data.  
        /// (I don't know why, but sometimes the compiled helpfile (.chm) indicates a 2d 
        /// array when it is just one-dimensional.  This is a one-dimensional array.)
        /// </param>
        /// 
        /// <returns>
        /// an instance of ZipFile. The name on the <c>ZipFile</c> will be <c>null</c>
        /// (<c>Nothing</c> in VB).
        /// </returns>
        ///
        /// <seealso cref="ZipFile.Read(System.IO.Stream)" />
        public static ZipFile Read(byte[] buffer)
        {
            return Read(buffer, null, DefaultEncoding);
        }


        /// <summary>
        /// Reads a zip archive from a byte array, using the given StatusMessageWriter.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is useful when the data for the zipfile is contained in a byte
        /// array, for example when retrieving the data from a database or other
        /// non-filesystem store.  The default Text Encoding (IBM437) is used to read
        /// the zipfile data.
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <param name="buffer">the byte array containing the zip data.</param>
        ///
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        /// 
        /// <returns>
        /// an instance of ZipFile. The name is set to <c>null</c> (<c>Nothing</c> in VB).
        /// </returns>
        /// 
        public static ZipFile Read(byte[] buffer, TextWriter statusMessageWriter)
        {
            return Read(buffer, statusMessageWriter, DefaultEncoding);
        }


        /// <summary>
        /// Reads a zip archive from a byte array, using the given StatusMessageWriter and text Encoding.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is useful when the data for the zipfile is contained in a byte
        /// array, for example when retrieving the data from a database or other
        /// non-filesystem store.
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <param name="buffer">the byte array containing the zip data.</param>
        ///
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to which verbose status messages are written
        /// during operations on the <c>ZipFile</c>.  For example, in a console
        /// application, System.Console.Out works, and will get a message for each entry
        /// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
        /// are written.
        /// </param>
        /// 
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8
        /// encoding bit set.  Be careful specifying the encoding.  If the value you use
        /// here is not the same as the Encoding used when the zip archive was created
        /// (possibly by a different archiver) you will get unexpected results and
        /// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <returns>
        /// an instance of ZipFile. The name is set to <c>null</c> (<c>Nothing</c> in VB).
        /// </returns>
        /// 
        public static ZipFile Read(byte[] buffer, TextWriter statusMessageWriter, System.Text.Encoding encoding)
        {
            ZipFile zf = new ZipFile();
            zf._StatusMessageTextWriter = statusMessageWriter;
            zf._provisionalAlternateEncoding = encoding;
            zf._readstream = new MemoryStream(buffer);
            zf._ReadStreamIsOurs = true;
            if (zf.Verbose) zf._StatusMessageTextWriter.WriteLine("reading from byte[]...");

            ReadIntoInstance(zf);
            return zf;
        }

        private static void ReadIntoInstance(ZipFile zf)
        {
            Stream s = zf.ReadStream;
            try
            {
                if (!s.CanSeek)
                {
                    ReadIntoInstance_Orig(zf);
                    return;
                }

                zf.OnReadStarted();

                // change for workitem 8098
                zf._originPosition = s.Position;

                // Try reading the central directory, rather than scanning the file. 

                uint datum = VerifyBeginningOfZipFile(s);

                if (datum == ZipConstants.EndOfCentralDirectorySignature)
                    return;


                // start at the end of the file...
                // seek backwards a bit, then look for the EoCD signature. 
                int nTries = 0;
                bool success = false;

                // The size of the end-of-central-directory-footer plus 2 bytes is 18.
                // This implies an archive comment length of 0.  We'll add a margin of
                // safety and start "in front" of that, when looking for the
                // EndOfCentralDirectorySignature
                long posn = s.Length - 64;
                long maxSeekback = System.Math.Max(s.Length - 0x4000, 10);
                do
                {
                    s.Seek(posn, SeekOrigin.Begin);
                    long bytesRead = SharedUtilities.FindSignature(s, (int)ZipConstants.EndOfCentralDirectorySignature);
                    if (bytesRead != -1)
                        success = true;
                    else
                    {
                        nTries++;
                        // Weird: with NETCF, negative offsets from SeekOrigin.End DO
                        // NOT WORK. So rather than seek a negative offset, we seek
                        // from SeekOrigin.Begin using a smaller number.
                        posn -= (32 * (nTries + 1) * nTries); // increasingly larger
                        if (posn < 0) posn = 0;  // BOF
                    }
                }
                while (!success && posn > maxSeekback);

                if (success)
                {
                    // workitem 8299
                    zf._locEndOfCDS = s.Position - 4;
                    byte[] block = new byte[16];
                    StreamUtil.Read(zf.ReadStream, block, 0, block.Length);

                    int i = 12;

                    uint offset32 = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                    if (offset32 == 0xFFFFFFFF)
                    {
                        Zip64SeekToCentralDirectory(zf);
                    }
                    else
                    {
                        zf.SeekFromOrigin(offset32);
                    }

                    ReadCentralDirectory(zf);
                }
                else
                {
                    // Could not find the central directory.
                    // Fallback to the old method.
                    // workitem 8098: ok
                    s.Seek(zf._originPosition, SeekOrigin.Begin);
                    ReadIntoInstance_Orig(zf);
                }
            }
            catch //(Exception e1)
            {
                if (zf._ReadStreamIsOurs && zf._readstream != null)
                {
                    zf._readstream.Close();
                    zf._readstream = null;
                }

                throw;
            }

            // the instance has been read in
            zf._contentsChanged = false;
        }



        private static void Zip64SeekToCentralDirectory(ZipFile zf)
        {
            Stream s = zf.ReadStream;

            byte[] block = new byte[16];

            // seek back to find the ZIP64 EoCD
            // I think this might not work for .NET CF ? 
            s.Seek(-40, SeekOrigin.Current);

            // Read first 16 bytes with proper read handling
            StreamUtil.Read(s, block, 0, 16);

            Int64 Offset64 = BitConverter.ToInt64(block, 8);
            zf.SeekFromOrigin(Offset64);

            uint datum = (uint)SharedUtilities.ReadInt(s);
            if (datum != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
                throw new BadReadException(String.Format("  ZipFile::Read(): Bad signature (0x{0:X8}) looking for ZIP64 EoCD Record at position 0x{1:X8}", datum, s.Position));

            // Read 8 bytes with proper read handling
            StreamUtil.Read(s, block, 0, 8);
            Int64 Size = BitConverter.ToInt64(block, 0);

            block = new byte[Size];

            // Read Size bytes with proper read handling
            StreamUtil.Read(s, block, 0, block.Length);

            Offset64 = BitConverter.ToInt64(block, 36);
            zf.SeekFromOrigin(Offset64);
        }

        private static uint VerifyBeginningOfZipFile(Stream s)
        {
            uint datum = (uint)SharedUtilities.ReadInt(s);
            return datum;
        }



        private static void ReadCentralDirectory(ZipFile zf)
        {
            ZipEntry de;
            while ((de = ZipEntry.ReadDirEntry(zf)) != null)
            {
                de.ResetDirEntry();
                zf.OnReadEntry(true, null);

                if (zf.Verbose)
                    zf.StatusMessageTextWriter.WriteLine("entry {0}", de.FileName);

                zf._entries.Add(de);
            }

            // workitem 8299
            if (zf._locEndOfCDS > 0)
                zf.SeekFromOrigin(zf._locEndOfCDS);
            ReadCentralDirectoryFooter(zf);

            if (zf.Verbose && zf.Comment != null && zf.Comment.Length > 0)
                zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);

            // We keep the read stream open after reading. 

            if (zf.Verbose)
                zf.StatusMessageTextWriter.WriteLine("read in {0} entries.", zf._entries.Count);

            zf.OnReadCompleted();
        }

        // build the TOC by reading each entry in the file.
        private static void ReadIntoInstance_Orig(ZipFile zf)
        {
            zf.OnReadStarted();
            zf._entries = new List<ZipEntry>();
            ZipEntry e;
            if (zf.Verbose)
                if (zf.Name == null)
                    zf.StatusMessageTextWriter.WriteLine("Reading zip from stream...");
                else
                    zf.StatusMessageTextWriter.WriteLine("Reading zip {0}...", zf.Name);

            // work item 6647:  PK00 (packed to removable disk)
            bool firstEntry = true;
            while ((e = ZipEntry.Read(zf, firstEntry)) != null)
            {
                if (zf.Verbose)
                    zf.StatusMessageTextWriter.WriteLine("  {0}", e.FileName);

                zf._entries.Add(e);
                firstEntry = false;
            }

            ZipEntry de;
            while ((de = ZipEntry.ReadDirEntry(zf)) != null)
            {
                // Housekeeping: Since ZipFile exposes ZipEntry elements in the enumerator, 
                // we need to copy the comment that we grab from the ZipDirEntry
                // into the ZipEntry, so the application can access the comment. 
                // Also since ZipEntry is used to Write zip files, we need to copy the 
                // file attributes to the ZipEntry as appropriate. 
                foreach (ZipEntry e1 in zf._entries)
                {
                    if (e1.FileName == de.FileName)
                    {
                        e1._Comment = de.Comment;
                        if (de.AttributesIndicateDirectory) e1.MarkAsDirectory();
                        break;
                    }
                }
            }

            // workitem 8299
            if (zf._locEndOfCDS > 0)
                zf.SeekFromOrigin(zf._locEndOfCDS);

            ReadCentralDirectoryFooter(zf);

            if (zf.Verbose && zf.Comment != null && zf.Comment.Length > 0)
                zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);

            zf.OnReadCompleted();
        }




        private static void ReadCentralDirectoryFooter(ZipFile zf)
        {
            Stream s = zf.ReadStream;
            int signature = SharedUtilities.ReadSignature(s);

            byte[] block;

            if (signature == ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
            {
                // We have a ZIP64 EOCD
                // This data block is 4 bytes sig, 8 bytes size, 44 bytes fixed data, 
                // followed by a variable-sized extension block.  We have read the sig already.
                // 8 - datasize (64 bits)
                // 2 - version made by
                // 2 - version needed to extract
                // 4 - number of this disk
                // 4 - number of the disk with the start of the CD
                // 8 - total number of entries in the CD on this disk
                // 8 - total number of entries in the CD 
                // 8 - size of the CD
                // 8 - offset of the CD
                // -----------------------
                // 52 bytes

                block = new byte[8 + 44];
                StreamUtil.Read(s, block, 0, block.Length);

                Int64 DataSize = BitConverter.ToInt64(block, 0);  // == 44 + the variable length

                if (DataSize < 44)
                    throw new ZipException("Bad DataSize in the ZIP64 Central Directory.");

                block = new byte[DataSize - 44];
                StreamUtil.Read(s, block, 0, block.Length);
                // discard the result

                signature = SharedUtilities.ReadSignature(s);
                if (signature != ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature)
                    throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");

                block = new byte[16];
                StreamUtil.Read(s, block, 0, block.Length);
                // discard the result

                signature = SharedUtilities.ReadSignature(s);
            }

            // Throw if this is not a signature for "end of central directory record"
            // This is a sanity check.
            if (signature != ZipConstants.EndOfCentralDirectorySignature)
            {
                s.Seek(-4, SeekOrigin.Current);
                throw new BadReadException(String.Format("  ZipFile::Read(): Bad signature ({0:X8}) at position 0x{1:X8}",
                                                         signature, s.Position));
            }

            // read a bunch of metadata for supporting multi-disk archives, which this library does not do.
            block = new byte[16];
            StreamUtil.Read(zf.ReadStream, block, 0, block.Length); // discard result

            // read the comment here
            ReadZipFileComment(zf);
        }



        private static void ReadZipFileComment(ZipFile zf)
        {
            // read the comment here
            byte[] block = new byte[2];
            StreamUtil.Read(zf.ReadStream, block, 0, block.Length);

            Int16 commentLength = (short)(block[0] + block[1] * 256);
            if (commentLength > 0)
            {
                block = new byte[commentLength];
                StreamUtil.Read(zf.ReadStream, block, 0, block.Length);

                // workitem 6513 - only use UTF8 as necessary
                // test reflexivity
                string s1 = DefaultEncoding.GetString(block, 0, block.Length);
                byte[] b2 = DefaultEncoding.GetBytes(s1);
                if (BlocksAreEqual(block, b2))
                {
                    zf.Comment = s1;
                }
                else
                {
                    // need alternate (non IBM437) encoding
                    // workitem 6415
                    // use UTF8 if the caller hasn't already set a non-default encoding
                    System.Text.Encoding e = (zf._provisionalAlternateEncoding.CodePage == 437)
                        ? System.Text.Encoding.UTF8
                        : zf._provisionalAlternateEncoding;
                    zf.Comment = e.GetString(block, 0, block.Length);
                }
            }
        }


        private static bool BlocksAreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }


        // workitem 8098
        internal void SeekFromOrigin(long position)
        {
            ReadStream.Seek(position + _originPosition, SeekOrigin.Begin);
        }

        internal long RelativeOffset
        {
            get
            {
                return ReadStream.Position - _originPosition;
            }
        }




        /// <summary>
        /// Checks the given file to see if it appears to be a valid zip file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calling this method is equivalent to calling <see cref="IsZipFile(string,
        /// bool)"/> with the testExtract parameter set to false.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="fileName">The file to check.</param>
        /// <returns>true if the file appears to be a zip file.</returns>
        public static bool IsZipFile(string fileName)
        {
            return IsZipFile(fileName, false);
        }


        /// <summary>
        /// Checks a file to see if it is a valid zip file.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method opens the specified zip file, reads in the zip archive,
        /// verifying the ZIP metadata as it reads.  Then, if testExtract is true, this
        /// method extracts each entry in the archive, dumping all the bits.
        /// </para>
        /// 
        /// <para>
        /// If everything succeeds, then the method returns true.  If anything fails -
        /// for example if an incorrect signature or CRC is found, indicating a corrupt
        /// file, the  method returns false.  This method also returns false for a
        /// file that does not exist.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, this method reads in the content for each
        /// entry, expands it, and checks CRCs.  This provides an additional check
        /// beyond verifying the zip header data.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, and if any of the zip entries are protected
        /// with a password, this method will return false.  If you want to verify a
        /// ZipFile that has entries which are protected with a password, you will need
        /// to do that manually.
        /// </para>
        /// </remarks>
        /// <param name="fileName">The zip file to check.</param>
        /// <param name="testExtract">true if the caller wants to extract each entry.</param>
        /// <returns>true if the file contains a valid zip file.</returns>
        public static bool IsZipFile(string fileName, bool testExtract)
        {
            bool result = false;
            try
            {
                if (!File.Exists(fileName)) return false;

                using (FileStream s = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    result = IsZipFile(s, testExtract);
                }
            }
            catch
            {
                // RK This is not our code, let's keep it like it is.
            }
            return result;
        }


        /// <summary>
        /// Checks a stream to see if it contains a valid zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method reads the zip archive contained in the specified stream, verifying
        /// the ZIP metadata as it reads.  If testExtract is true, this method also extracts 
        /// each entry in the archive, dumping all the bits into <see cref="Stream.Null"/>.
        /// </para>
        /// 
        /// <para>
        /// If everything succeeds, then the method returns true.  If anything fails -
        /// for example if an incorrect signature or CRC is found, indicating a corrupt
        /// file, the method returns false.  This method also returns false for a
        /// file that does not exist.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, this method reads in the content for each
        /// entry, expands it, and checks CRCs.  This provides an additional check
        /// beyond verifying the zip header data.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, and if any of the zip entries are protected
        /// with a password, this method will return false.  If you want to verify a
        /// ZipFile that has entries which are protected with a password, you will need
        /// to do that manually.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="IsZipFile(string, bool)"/>
        ///
        /// <param name="stream">The stream to check.</param>
        /// <param name="testExtract">true if the caller wants to extract each entry.</param>
        /// <returns>true if the stream contains a valid zip archive.</returns>
        public static bool IsZipFile(Stream stream, bool testExtract)
        {
            bool result = false;
            try
            {
                if (!stream.CanRead) return false;

                Stream bitBucket = Stream.Null;

                using (ZipFile zip1 = Read(stream, null, System.Text.Encoding.GetEncoding("IBM437")))
                {
                    if (testExtract)
                    {
                        foreach (ZipEntry e in zip1)
                        {
                            if (!e.IsDirectory)
                            {
                                e.Extract(bitBucket);
                            }
                        }
                    }
                }
                result = true;
            }
            catch
            {
                // RK This is not our code, let's keep it like it is.
            }
            return result;
        }

#endregion

        #region Save

        /// <summary>
        /// Saves the Zip archive to a file, specified by the Name property of the <c>ZipFile</c>. 
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The <c>ZipFile</c> instance is written to storage, typically a zip file in a
        /// filesystem, only when the caller calls <c>Save</c>.  The Save operation writes
        /// the zip content to a temporary file, and then renames the temporary file
        /// to the desired name. If necessary, this method will delete a pre-existing file
        /// before the rename.
        /// </para>
        ///
        /// <para> The <see cref="ZipFile.Name"/> property is specified either
        /// explicitly, or implicitly using one of the parameterized ZipFile
        /// constructors.  For COM Automation clients, the <c>Name</c> property must be
        /// set explicitly, because COM Automation clients cannot call parameterized
        /// constructors.  </para>
        ///
        /// <para>
        /// When using a filesystem file for the Zip output, it is possible to call
        /// <c>Save</c> multiple times on the <c>ZipFile</c> instance. With each call the zip
        /// content is re-written to the same output file.
        /// </para>
        ///
        /// <para>
        /// Data for entries that have been added to the <c>ZipFile</c> instance is written
        /// to the output when the <c>Save</c> method is called. This means that the input
        /// streams for those entries must be available at the time the application calls
        /// <c>Save</c>.  If, for example, the application adds entries with <c>AddEntry</c>
        /// using a dynamically-allocated <c>MemoryStream</c>, the memory stream must not
        /// have been disposed before the call to <c>Save</c>. See the <see
        /// cref="ZipEntry.InputStream"/> property for more discussion of the availability
        /// requirements of the input stream for an entry, and an approach for providing
        /// just-in-time stream lifecycle management.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="Aspose.Zip.ZipFile.AddEntry(String, String, System.IO.Stream)"/>
        ///
        /// <exception cref="Aspose.Zip.BadStateException">
        /// Thrown if you haven't specified a location or stream for saving the zip,
        /// either in the constructor or by setting the Name property, or if you try to
        /// save a regular zip archive to a filename with a .exe extension.
        /// </exception>
        ///
        public void Save()
        {
            try
            {
                bool thisSaveUsedZip64 = false;
                _saveOperationCanceled = false;
                OnSaveStarted();

                if (WriteStream == null)
                    throw new BadStateException("You haven't specified where to save the zip.");

                if (_name != null && _name.EndsWith(".exe", StringComparison.Ordinal))
                    throw new BadStateException("You specified an EXE for a plain zip file.");
                
                // check if modified, before saving. 
                if (!_contentsChanged) return;
                
                if (Verbose) StatusMessageTextWriter.WriteLine("saving....");

                // validate the number of entries
                if (_entries.Count >= 0xFFFF && _zip64 == Zip64Option.Never)
                    throw new ZipException("The number of entries is 65535 or greater. Consider setting the Zip64Mode save option.");

                {
                    // write an entry in the zip for each file
                    int n = 0;
                    foreach (ZipEntry e in _entries)
                    {
                        OnSaveEntry(n, e, true);
                        e.Write(WriteStream);
                        if (_saveOperationCanceled)
                            break;
                        e._zipfile = this;
                        n++;
                        OnSaveEntry(n, e, false);
                        if (_saveOperationCanceled)
                            break;

                        if (e.IncludedInMostRecentSave)
                            thisSaveUsedZip64 |= e.OutputUsedZip64 == NullableBool.True;
                    }
                }


                if (_saveOperationCanceled)
                    return;

                WriteCentralDirectoryStructure(WriteStream);

                OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);

                _hasBeenSaved = true;
                _contentsChanged = false;

                thisSaveUsedZip64 |= _NeedZip64CentralDirectory;
                _OutputUsesZip64 = (thisSaveUsedZip64) ? NullableBool.True : NullableBool.False;

                // do the rename as necessary
                if (_temporaryFileName != null && _name != null)
                {
                    // _temporaryFileName may remain null if we are writing to a stream.
                    // only close the stream if there is a file behind it. 
                    WriteStream.Close();
                    WriteStream = null;

                    if (_saveOperationCanceled)
                        return;

                    if ((_fileAlreadyExists) && (_readstream != null))
                    {
                        // This means we opened and read a zip file. 
                        // If we are now saving to the same file, we need to close the
                        // orig file, first.
                        _readstream.Close();
                        _readstream = null;
                    }

                    if (_fileAlreadyExists)
                    {
                        // We do not just call File.Replace() here because 
                        // there is a possibility that the TEMP volume is different 
                        // that the volume for the final file (c:\ vs d:\).
                        // So we need to do a Delete+Move pair. 
                        //
                        // Ideally this would be transactional. 
                        // 
                        // It's possible that the delete succeeds and the move fails.  
                        // in that case, we're hosed, and we'll throw.
                        //
                        // Could make this more complicated by moving (renaming) the first file, then
                        // moving the second, then deleting the first file. But the
                        // error handling and unwrap logic just gets more complicated.
                        //
                        // Better to just keep it simple. 
                        File.Delete(_name);
                        OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
                        File.Move(_temporaryFileName, _name);
                        OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);
                    }
                    else
                        File.Move(_temporaryFileName, _name);

                    _fileAlreadyExists = true;
                }

                OnSaveCompleted();
                _JustSaved = true;

            }

            // workitem 5043
            finally
            {
                CleanupAfterSaveOperation();
            }

            return;
        }



        private void RemoveTempFile()
        {
            try
            {
                if (File.Exists(_temporaryFileName))
                {
                    File.Delete(_temporaryFileName);
                }
            }
            catch (Exception ex1)
            {
                if (Verbose)
                    StatusMessageTextWriter.WriteLine("ZipFile::Save: could not delete temp file: {0}.", ex1.Message);
            }
        }


        private void CleanupAfterSaveOperation()
        {
            if ((_temporaryFileName != null) && (_name != null))
            {
                // only close the stream if there is a file behind it. 
                if (_writestream != null)
                {
                    try
                    {
                        _writestream.Close();
                    }
                    catch
                    {
                        // RK This is not our code, let's keep it like it is.
                    }
                    try
                    {
                        // workitem 7704
                        _writestream.Close();
                    }
                    catch
                    {
                        // RK This is not our code, let's keep it like it is.
                    }
                }
                _writestream = null;
                RemoveTempFile();
                _temporaryFileName = null;
            }
        }


        /// <summary>
        /// Save the file to a new zipfile, with the given name. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method allows the application to explicitly specify the name of the zip
        /// file when saving. Use this when creating a new zip file, or when 
        /// updating a zip archive.  
        /// </para>
        /// 
        /// <para>
        /// An application can also save a zip archive in several places by calling this
        /// method multiple times in succession, with different filenames.
        /// </para>
        ///
        /// <para>
        /// The <c>ZipFile</c> instance is written to storage, typically a zip file in a
        /// filesystem, only when the caller calls <c>Save</c>.  The Save operation writes
        /// the zip content to a temporary file, and then renames the temporary file
        /// to the desired name. If necessary, this method will delete a pre-existing file
        /// before the rename.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentException">
        /// Thrown if you specify a directory for the filename.
        /// </exception>
        ///
        /// <param name="fileName">
        /// The name of the zip archive to save to. Existing files will 
        /// be overwritten with great prejudice.
        /// </param>
        ///
        /// <example>
        /// This example shows how to create and Save a zip file.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   zip.AddDirectory(@"c:\reports\January");
        ///   zip.Save("January.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile()
        ///   zip.AddDirectory("c:\reports\January")
        ///   zip.Save("January.zip")
        /// End Using
        /// </code>
        ///
        /// </example>
        ///
        /// <example>
        /// This example shows how to update a zip file.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read("ExistingArchive.zip"))
        /// {
        ///   zip.AddFile("NewData.csv");
        ///   zip.Save("UpdatedArchive.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read("ExistingArchive.zip")
        ///   zip.AddFile("NewData.csv")
        ///   zip.Save("UpdatedArchive.zip")
        /// End Using
        /// </code>
        ///
        /// </example>
        public void Save(String fileName)
        {
            // Check for the case where we are re-saving a zip archive 
            // that was originally instantiated with a stream.  In that case, 
            // the _name will be null. If so, we set _writestream to null, 
            // which insures that we'll cons up a new WriteStream (with a filesystem
            // file backing it) in the Save() method.
            if (_name == null)
                _writestream = null;

            _name = fileName;
            if (Directory.Exists(_name))
                throw new ZipException("Bad Directory", new ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));
            _contentsChanged = true;
            _fileAlreadyExists = File.Exists(_name);
            Save();
        }


        /// <summary>
        /// Save the zip archive to the specified stream.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The <c>ZipFile</c> instance is written to storage - typically a zip file in a
        /// filesystem, but using this overload, the storage can eb anything accessible via
        /// a writable stream - only when the caller calls <c>Save</c>.
        /// </para>
        ///
        /// <para>
        /// Use this method to save the zip content to a stream directly.  A common
        /// scenario is an ASP.NET application that dynamically generates a zip file and
        /// allows the browser to download it. The application can call
        /// <c>Save(Response.OutputStream)</c> to write a zipfile directly to the output
        /// stream, without creating a zip file on the disk on the ASP.NET server.
        /// </para>
        ///
        /// </remarks>
        /// 
        /// <param name="outputStream">
        /// The <c>System.IO.Stream</c> to write to. It must be writable.
        /// </param>
        public void Save(Stream outputStream)
        {
            if (!outputStream.CanWrite)
                throw new ArgumentException("The outputStream must be a writable stream.");

            // if we had a filename to save to, we are now obliterating it. 
            _name = null;

            _writestream = new CountingStream(outputStream);

            _contentsChanged = true;
            _fileAlreadyExists = false;
            Save();
        }




        private void WriteCentralDirectoryStructure(Stream s)
        {
            // We need to keep track of the start and
            // Finish of the Central Directory Structure.

            // Cannot always use WriteStream.Length or Position; some streams do not 
            // support these. (eg, ASP.NET Response.OutputStream)
            // In those cases we have a CountingStream.

            CountingStream output = s as CountingStream;
            long Start = (output != null) ? output.BytesWritten : s.Position;

            foreach (ZipEntry e in _entries)
            {
                if (e.IncludedInMostRecentSave)
                    e.WriteCentralDirectoryEntry(s);  // this writes a ZipDirEntry corresponding to the ZipEntry
            }

            long Finish = (output != null) ? output.BytesWritten : s.Position;

            Int64 SizeOfCentralDirectory = Finish - Start;

            _NeedZip64CentralDirectory =
        _zip64 == Zip64Option.Always ||
        CountEntries() >= 0xFFFF ||
        SizeOfCentralDirectory > 0xFFFFFFFF ||
        Start > 0xFFFFFFFF;

            // emit ZIP64 extensions as required
            if (_NeedZip64CentralDirectory)
            {
                if (_zip64 == Zip64Option.Never)
                    throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the Zip64Mode save option.");

                WriteZip64EndOfCentralDirectory(s, Start, Finish);
            }

            // now, the footer
            WriteCentralDirectoryFooter(s, Start, Finish);
        }



        private int CountEntries()
        {
            // cannot just emit _entries.Count, because some of the entries
            // may have been skipped.
            int count = 0;
            foreach (ZipEntry entry in _entries)
                if (entry.IncludedInMostRecentSave) count++;
            return count;
        }

        
        private void WriteZip64EndOfCentralDirectory(Stream s,
                                                     long StartOfCentralDirectory,
                                                     long EndOfCentralDirectory)
        {
            const int bufferLength = 12 + 44 + 20;

            byte[] bytes = new byte[bufferLength];

            int i = 0;
            // signature
            byte[] sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryRecordSignature);
            Array.Copy(sig, 0, bytes, i, 4);
            i+=4;
            
            // There is a possibility to include "Extensible" data in the zip64
            // end-of-central-dir record.  I cannot figure out what it might be used to
            // store, so the size of this record is always fixed.  Maybe it is used for
            // strong encryption data?  That is for another day.
            long DataSize = 44;
            Array.Copy(BitConverter.GetBytes(DataSize), 0, bytes, i, 8);
            i += 8;

            // VersionMadeBy is 45
            bytes[i++] = 45;
            bytes[i++] = 0x00;

            // VersionNeededToExtract is 45
            bytes[i++] = 45;
            bytes[i++] = 0x00;

            // number of the disk, and the disk with the start of the central dir.  Always zero.
            for (int j = 0; j < 8; j++)
                bytes[i++] = 0x00;

            long numberOfEntries = CountEntries();
            Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, bytes, i, 8);
            i += 8;
            Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, bytes, i, 8);
            i += 8;

            Int64 SizeofCentraldirectory = EndOfCentralDirectory - StartOfCentralDirectory;
            Array.Copy(BitConverter.GetBytes(SizeofCentraldirectory), 0, bytes, i, 8);
            i += 8;
            Array.Copy(BitConverter.GetBytes(StartOfCentralDirectory), 0, bytes, i, 8);
            i += 8;

            // now, the locator
            // signature
            sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature);
            Array.Copy(sig, 0, bytes, i, 4);
            i+=4;
            
            // number of the disk with the zip64 eocd
            bytes[i++] = 0x00;
            bytes[i++] = 0x00;
            bytes[i++] = 0x00;
            bytes[i++] = 0x00;

            // relative offset of the zip64 eocd
            Array.Copy(BitConverter.GetBytes(EndOfCentralDirectory), 0, bytes, i, 8);
            i += 8;

            // total number of disks
            bytes[i++] = 0x01;
            bytes[i++] = 0x00;
            bytes[i++] = 0x00;
            bytes[i++] = 0x00;

            s.Write(bytes, 0, i);
        }




        private void WriteCentralDirectoryFooter(Stream s,
                                                 long StartOfCentralDirectory,
                                                 long EndOfCentralDirectory)
        {
            int j = 0;
            int bufferLength = 24;
            byte[] block = null;
            Int16 commentLength = 0;
            if ((Comment != null) && (Comment.Length != 0))
            {
                block = ProvisionalAlternateEncoding.GetBytes(Comment);
                commentLength = (Int16)block.Length;
            }
            bufferLength += commentLength;
            byte[] bytes = new byte[bufferLength];

            int i = 0;
            // signature
            byte[] sig = BitConverter.GetBytes(ZipConstants.EndOfCentralDirectorySignature);
            Array.Copy(sig, 0, bytes, i, 4);
            i+=4;
            
            // number of this disk
            bytes[i++] = 0;
            bytes[i++] = 0;

            // number of the disk with the start of the central directory
            bytes[i++] = 0;
            bytes[i++] = 0;

            // handle ZIP64 extensions for the end-of-central-directory 
            if (CountEntries() >= 0xFFFF || _zip64 == Zip64Option.Always)
            {
                // the ZIP64 version.
                for (j = 0; j < 4; j++)
                    bytes[i++] = 0xFF;
            }
            else
            {
                int c = CountEntries();
                // the standard version.
                // total number of entries in the central dir on this disk
                bytes[i++] = (byte)(c & 0x00FF);
                bytes[i++] = (byte)((c & 0xFF00) >> 8);

                // total number of entries in the central directory
                bytes[i++] = (byte)(c & 0x00FF);
                bytes[i++] = (byte)((c & 0xFF00) >> 8);
            }

            // size of the central directory
            Int64 SizeOfCentralDirectory = EndOfCentralDirectory - StartOfCentralDirectory;

            if (SizeOfCentralDirectory >= 0xFFFFFFFF || StartOfCentralDirectory >= 0xFFFFFFFF)
            {
                // The actual data is in the ZIP64 central directory structure
                for (j = 0; j < 8; j++)
                    bytes[i++] = 0xFF;
            }
            else
            {
                // size of the central directory (we just get the low 4 bytes)
                bytes[i++] = (byte)(SizeOfCentralDirectory & 0x000000FF);
                bytes[i++] = (byte)((SizeOfCentralDirectory & 0x0000FF00) >> 8);
                bytes[i++] = (byte)((SizeOfCentralDirectory & 0x00FF0000) >> 16);
                bytes[i++] = (byte)((SizeOfCentralDirectory & 0xFF000000) >> 24);

                // offset of the start of the central directory (we just get the low 4 bytes)
                bytes[i++] = (byte)(StartOfCentralDirectory & 0x000000FF);
                bytes[i++] = (byte)((StartOfCentralDirectory & 0x0000FF00) >> 8);
                bytes[i++] = (byte)((StartOfCentralDirectory & 0x00FF0000) >> 16);
                bytes[i++] = (byte)((StartOfCentralDirectory & 0xFF000000) >> 24);
            }


            // zip archive comment 
            if ((Comment == null) || (Comment.Length == 0))
            {
                // no comment!
                bytes[i++] = (byte)0;
                bytes[i++] = (byte)0;
            }
            else
            {
                // the size of our buffer defines the max length of the comment we can write
                if (commentLength + i + 2 > bytes.Length) commentLength = (Int16)(bytes.Length - i - 2);
                bytes[i++] = (byte)(commentLength & 0x00FF);
                bytes[i++] = (byte)((commentLength & 0xFF00) >> 8);

                if (commentLength != 0)
                {
                    // now actually write the comment itself into the byte buffer
                    for (j = 0; (j < commentLength) && (i + j < bytes.Length); j++)
                    {
                        bytes[i + j] = block[j];
                    }
                    i += j;
                }
            }

            s.Write(bytes, 0, i);
        }

#endregion

       #region Selector

#endregion

       #region IEnumerable

        IEnumerator<ZipEntry> IEnumerable<ZipEntry>.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        /// <summary>
        /// IEnumerator support, for use of a ZipFile in a foreach construct.  
        /// </summary>
        ///
        /// <remarks>
        /// This method is included for COM support.  An application generally does not call
        /// this method directly.  It is called implicitly by COM clients when enumerating
        /// the entries in the ZipFile instance.  In VBScript, this is done with a <c>For Each</c>
        /// statement.  In Javascript, this is done with <c>new Enumerator(zipfile)</c>.
        /// </remarks>
        ///
        /// <returns>
        /// The IEnumerator over the entries in the ZipFile. 
        /// </returns>
        public IEnumerator GetNewEnum()          // the name of this method is not significant
        {
            return _entries.GetEnumerator();
        }

#endregion

    }
}


// ==================================================================
//
// Information on the ZIP format:
//
// From
// http://www.pkware.com/documents/casestudies/APPNOTE.TXT
//
//  Overall .ZIP file format:
//
//     [local file header 1]
//     [file data 1]
//     [data descriptor 1]  ** sometimes
//     . 
//     .
//     .
//     [local file header n]
//     [file data n]
//     [data descriptor n]   ** sometimes
//     [archive decryption header] 
//     [archive extra data record] 
//     [central directory]
//     [zip64 end of central directory record]
//     [zip64 end of central directory locator] 
//     [end of central directory record]
//
// Local File Header format:
//         local file header signature ... 4 bytes  (0x04034b50)
//         version needed to extract ..... 2 bytes
//         general purpose bit field ..... 2 bytes
//         compression method ............ 2 bytes
//         last mod file time ............ 2 bytes
//         last mod file date............. 2 bytes
//         crc-32 ........................ 4 bytes
//         compressed size................ 4 bytes
//         uncompressed size.............. 4 bytes
//         file name length............... 2 bytes
//         extra field length ............ 2 bytes
//         file name                       varies
//         extra field                     varies
//
//
// Data descriptor:  (used only when bit 3 of the general purpose bitfield is set)
//         (although, I have found zip files where bit 3 is not set, yet this descriptor is present!)
//         local file header signature     4 bytes  (0x08074b50)  ** sometimes!!! Not always
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//
//
//   Central directory structure:
//
//       [file header 1]
//       .
//       .
//       . 
//       [file header n]
//       [digital signature] 
//
//
//       File header:  (This is a ZipDirEntry)
//         central file header signature   4 bytes  (0x02014b50)
//         version made by                 2 bytes
//         version needed to extract       2 bytes
//         general purpose bit flag        2 bytes
//         compression method              2 bytes
//         last mod file time              2 bytes
//         last mod file date              2 bytes
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//         file name length                2 bytes
//         extra field length              2 bytes
//         file comment length             2 bytes
//         disk number start               2 bytes
//         internal file attributes **     2 bytes
//         external file attributes ***    4 bytes
//         relative offset of local header 4 bytes
//         file name (variable size)
//         extra field (variable size)
//         file comment (variable size)
//
// ** The internal file attributes, near as I can tell, 
// uses 0x01 for a file and a 0x00 for a directory. 
//
// ***The external file attributes follows the MS-DOS file attribute byte, described here:
// at http://support.microsoft.com/kb/q125019/
// 0x0010 => directory
// 0x0020 => file 
//
//
// End of central directory record:
//
//         end of central dir signature    4 bytes  (0x06054b50)
//         number of this disk             2 bytes
//         number of the disk with the
//         start of the central directory  2 bytes
//         total number of entries in the
//         central directory on this disk  2 bytes
//         total number of entries in
//         the central directory           2 bytes
//         size of the central directory   4 bytes
//         offset of start of central
//         directory with respect to
//         the starting disk number        4 bytes
//         .ZIP file comment length        2 bytes
//         .ZIP file comment       (variable size)
//
// date and time are packed values, as MSDOS did them
// time: bits 0-4 : seconds (divided by 2)
//            5-10: minute
//            11-15: hour
// date  bits 0-4 : day
//            5-8: month
//            9-15 year (since 1980)
//
// see http://msdn.microsoft.com/en-us/library/ms724274(VS.85).aspx

