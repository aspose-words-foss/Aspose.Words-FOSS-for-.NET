// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/09/2024 by Edward Voronov

namespace Aspose
{
    /// <summary>
    /// The lightweight version of the <see cref="System.Uri"/> class.
    /// </summary>
    public class URI
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="URI"/> class
        /// with address and subaddress extracted from specified uri.
        /// </summary>
        public URI(string uri)
            : this(UriUtil.GetAddress(uri), UriUtil.GetSubAddress(uri))
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="URI"/> class with specified address and subaddress.
        /// </summary>
        public URI(string address, string subAddress)
        {
            Address = address ?? string.Empty;
            SubAddress = subAddress ?? string.Empty;
        }

        /// <summary>
        /// Gets the address part (before "#") of the current uri.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Gets the subaddress part (after "#") of the current uri.
        /// </summary>
        public string SubAddress { get; }

        /// <summary>
        /// Gets the value indicating the current uri has only subaddress part (after "#").
        /// </summary>
        public bool IsSubAddressOnly
        {
            get { return string.IsNullOrEmpty(Address); }
        }

        /// <summary>
        /// Gets the value indicating the current uri has scheme.
        /// </summary>
        public bool IsHrefWithScheme
        {
            get { return UriUtil.IsHrefWithScheme(Address); }
        }

        /// <summary>
        /// Gets the value indicating the current is an absolute local file path or UNC path.
        /// </summary>
        public bool IsFilePath
        {
            get { return UriUtil.IsFilePath(Address); }
        }

        /// <summary>
        /// Returns full uri path from provided base uri and the current relative uri.
        /// </summary>
        public URI ToAbsoluteUri(string baseUri)
        {
            return new URI(UriUtil.ConstructAbsoluteUri(baseUri, Address), SubAddress);
        }

        /// <summary>
        /// Returns current uri with the file scheme prefix "file:///".
        /// </summary>
        public URI WithFileSchemePrefix()
        {
            return new URI(UriUtil.AddFileSchemePrefix(Address), SubAddress);
        }

        /// <summary>
        /// Returns concatenated address and subaddress with optional escaping.
        /// </summary>
        public string ToString(bool escape)
        {
            return escape
                ? FullUriEscaped
                : FullUri;
        }

        private string FullUri
        {
            get
            {
                return mFullUri ?? (mFullUri = UriUtil.AppendSubAddress(Address, SubAddress));
            }
        }

        private string FullUriEscaped
        {
            get
            {
                if (mFullUriEscaped == null)
                {
                    if (UriUtil.IsHrefThatNeedsEscaping(Address) || UriUtil.IsHrefThatNeedsEscaping(SubAddress))
                    {
                        mFullUriEscaped = UriUtil.AppendSubAddress(
                            UriUtil.EscapeHrefAnyway(Address),
                            UriUtil.EscapeHrefAnyway(SubAddress));
                    }
                    else
                    {
                        mFullUriEscaped = FullUri;
                    }
                }

                return mFullUriEscaped;
            }
        }

        private string mFullUri;
        private string mFullUriEscaped;
    }
}
