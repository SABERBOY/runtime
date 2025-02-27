// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace System.Net
{
    public partial class WebProxy : IWebProxy, ISerializable
    {
        private ArrayList? _bypassList;
        private Regex[]? _regexBypassList;

        public WebProxy() : this((Uri?)null, false, null, null) { }

        public WebProxy(Uri? Address) : this(Address, false, null, null) { }

        public WebProxy(Uri? Address, bool BypassOnLocal) : this(Address, BypassOnLocal, null, null) { }

        public WebProxy(Uri? Address, bool BypassOnLocal, [StringSyntax(StringSyntaxAttribute.Regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)] string[]? BypassList) : this(Address, BypassOnLocal, BypassList, null) { }

        public WebProxy(Uri? Address, bool BypassOnLocal, [StringSyntax(StringSyntaxAttribute.Regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)] string[]? BypassList, ICredentials? Credentials)
        {
            this.Address = Address;
            this.Credentials = Credentials;
            this.BypassProxyOnLocal = BypassOnLocal;
            if (BypassList != null)
            {
                _bypassList = new ArrayList(BypassList);
                UpdateRegexList(true);
            }
        }

        public WebProxy(string Host, int Port)
            : this(CreateProxyUri(Host, Port), false, null, null)
        {
        }

        public WebProxy(string? Address)
            : this(CreateProxyUri(Address), false, null, null)
        {
        }

        public WebProxy(string? Address, bool BypassOnLocal)
            : this(CreateProxyUri(Address), BypassOnLocal, null, null)
        {
        }

        public WebProxy(string? Address, bool BypassOnLocal, [StringSyntax(StringSyntaxAttribute.Regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)] string[]? BypassList)
            : this(CreateProxyUri(Address), BypassOnLocal, BypassList, null)
        {
        }

        public WebProxy(string? Address, bool BypassOnLocal, [StringSyntax(StringSyntaxAttribute.Regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)] string[]? BypassList, ICredentials? Credentials)
            : this(CreateProxyUri(Address), BypassOnLocal, BypassList, Credentials)
        {
        }

        public Uri? Address { get; set; }

        public bool BypassProxyOnLocal { get; set; }

        [AllowNull]
        public string[] BypassList
        {
            get
            {
                if (_bypassList == null)
                    return Array.Empty<string>();

                var bypassList = new string[_bypassList.Count];
                _bypassList.CopyTo(bypassList);
                return bypassList;
            }
            set
            {
                _bypassList = value != null ? new ArrayList(value) : null;
                UpdateRegexList(true);
            }
        }

        public ArrayList BypassArrayList => _bypassList ??= new ArrayList();

        public ICredentials? Credentials { get; set; }

        public bool UseDefaultCredentials
        {
            get => Credentials == CredentialCache.DefaultCredentials;
            set => Credentials = value ? CredentialCache.DefaultCredentials : null;
        }

        public Uri? GetProxy(Uri destination)
        {
            ArgumentNullException.ThrowIfNull(destination);

            return IsBypassed(destination) ? destination : Address;
        }

        private static Uri? CreateProxyUri(string? address, int? port = null)
        {
            if (address is null)
            {
                return null;
            }

            if (!address.Contains("://", StringComparison.Ordinal))
            {
                address = "http://" + address;
            }

            var proxyUri = new Uri(address);

            if (port.HasValue && proxyUri.IsAbsoluteUri)
            {
                proxyUri = new UriBuilder(proxyUri) { Port = port.Value }.Uri;
            }

            return proxyUri;
        }

        private void UpdateRegexList(bool canThrow)
        {
            Regex[]? regexBypassList = null;
            ArrayList? bypassList = _bypassList;
            try
            {
                if (bypassList != null && bypassList.Count > 0)
                {
                    regexBypassList = new Regex[bypassList.Count];
                    for (int i = 0; i < bypassList.Count; i++)
                    {
                        regexBypassList[i] = new Regex((string)bypassList[i]!, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    }
                }
            }
            catch
            {
                if (!canThrow)
                {
                    _regexBypassList = null;
                    return;
                }
                throw;
            }

            // Update field here, as it could throw earlier in the loop
            _regexBypassList = regexBypassList;
        }

        private bool IsMatchInBypassList(Uri input)
        {
            UpdateRegexList(canThrow: false);

            if (_regexBypassList is Regex[] bypassList)
            {
                bool isDefaultPort = input.IsDefaultPort;
                int lengthRequired = input.Scheme.Length + 3 + input.Host.Length;
                if (!isDefaultPort)
                {
                    lengthRequired += 1 + 5; // 1 for ':' and 5 for max formatted length of a port (16 bit value)
                }

                int charsWritten;
                Span<char> url = lengthRequired <= 256 ? stackalloc char[256] : new char[lengthRequired];
                bool formatted = isDefaultPort ?
                    url.TryWrite($"{input.Scheme}://{input.Host}", out charsWritten) :
                    url.TryWrite($"{input.Scheme}://{input.Host}:{(uint)input.Port}", out charsWritten);
                Debug.Assert(formatted);
                url = url.Slice(0, charsWritten);

                foreach (Regex r in bypassList)
                {
                    if (r.IsMatch(url))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsBypassed(Uri host)
        {
            ArgumentNullException.ThrowIfNull(host);

            return
                Address == null ||
                (BypassProxyOnLocal && IsLocal(host)) ||
                IsMatchInBypassList(host);
        }

        protected WebProxy(SerializationInfo serializationInfo, StreamingContext streamingContext) =>
            throw new PlatformNotSupportedException();

        void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext) =>
            throw new PlatformNotSupportedException();

        protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext) =>
            throw new PlatformNotSupportedException();

        [Obsolete("WebProxy.GetDefaultProxy has been deprecated. Use the proxy selected for you by default.")]
        public static WebProxy GetDefaultProxy() =>
            // The .NET Framework here returns a proxy that fetches IE settings and
            // executes JavaScript to determine the correct proxy.
            throw new PlatformNotSupportedException();
    }
}
