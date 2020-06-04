using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Configures options for the wiki web client.
    /// </summary>
    public static class WikiWebConfig
    {
        /// <summary>
        /// The relative URL of the <see cref="SignalR.IWikiTalkHub"/>.
        /// </summary>
        public const string WikiTalkHubRoute = "/wikiTalkHub";

        private const string DefaultLayoutPath = "/Views/Wiki/_DefaultWikiMainLayout.cshtml";
        private const string DefaultLoginPath = "/Pages/Account/Login.cshtml";

        private static List<string>? _AdminNamespaces;
        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by non-admin
        /// users.
        /// </para>
        /// <para>
        /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
        /// </para>
        /// </summary>
        public static IEnumerable<string> AdminNamespaces => (_AdminNamespaces ?? Enumerable.Empty<string>())
            .Concat(new[] { SystemNamespace });

        /// <summary>
        /// <para>
        /// The title of the main about page.
        /// </para>
        /// <para>
        /// Default is "About"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the about page.
        /// </para>
        /// </summary>
        public static string? AboutPageTitle { get; set; } = "About";

        private static string? _CompactLayoutPath;
        /// <summary>
        /// <para>
        /// The layout used by wiki pages in compact view.
        /// </para>
        /// <para>
        /// Default is "/Views/Wiki/_DefaultWikiMainLayout.cshtml"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? CompactLayoutPath
        {
            get => _CompactLayoutPath ?? DefaultLayoutPath;
            set => _CompactLayoutPath = value;
        }

        /// <summary>
        /// <para>
        /// The title of the main contact page.
        /// </para>
        /// <para>
        /// Default is "Contact"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the contact page.
        /// </para>
        /// </summary>
        public static string? ContactPageTitle { get; set; } = "Contact";

        /// <summary>
        /// <para>
        /// The title of the main contents page.
        /// </para>
        /// <para>
        /// Default is "Contents"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the contents page.
        /// </para>
        /// </summary>
        public static string? ContentsPageTitle { get; set; } = "Contents";

        /// <summary>
        /// <para>
        /// The title of the main copyright page.
        /// </para>
        /// <para>
        /// Default is "Copyright"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the copyright page and the copyright notice on pages.
        /// </para>
        /// <para>
        /// Consider carefully before omitting this special page, unless you supply an alternate
        /// copyright notice on your wiki.
        /// </para>
        /// </summary>
        public static string? CopyrightPageTitle { get; set; } = "Copyright";

        /// <summary>
        /// <para>
        /// The title of the main help page.
        /// </para>
        /// <para>
        /// Default is "Help"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the help page.
        /// </para>
        /// </summary>
        public static string? HelpPageTitle { get; set; } = "Help";

        private static string? _LoginPath;
        /// <summary>
        /// <para>
        /// The path to the login page.
        /// </para>
        /// <para>
        /// Default is "/Pages/Account/Login.cshtml"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? LoginPath
        {
            get => _LoginPath ?? DefaultLoginPath;
            set => _LoginPath = value;
        }

        private static string? _MainLayoutPath;
        /// <summary>
        /// <para>
        /// The layout used by wiki pages.
        /// </para>
        /// <para>
        /// Default is "/Views/Wiki/_DefaultWikiMainLayout.cshtml"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? MainLayoutPath
        {
            get => _MainLayoutPath ?? DefaultLayoutPath;
            set => _MainLayoutPath = value;
        }

        /// <summary>
        /// <para>
        /// The maximum size (in bytes) of uploaded files.
        /// </para>
        /// <para>
        /// Setting this to a value less than or equal to zero effectively prevents file uploads.
        /// </para>
        /// </summary>
        public static int MaxFileSize { get; set; } = 5000000; // 5 MB

        /// <summary>
        /// Gets a string representing the <see cref="MaxFileSize"/> in a reasonable unit (GB for
        /// large sizes, down to bytes for small ones).
        /// </summary>
        public static string MaxFileSizeString
        {
            get
            {
                if (MaxFileSize >= 1000000000)
                {
                    return $"{MaxFileSize / 1000000000.0:N3} GB";
                }
                else if (MaxFileSize >= 1000000)
                {
                    return $"{MaxFileSize / 1000000.0:N3} MB";
                }
                else if (MaxFileSize >= 1000)
                {
                    return $"{MaxFileSize / 1000.0:G} KB";
                }
                else
                {
                    return $"{MaxFileSize} bytes";
                }
            }
        }

        /// <summary>
        /// <para>
        /// The title of the main policy page.
        /// </para>
        /// <para>
        /// Default is "Policies"
        /// </para>
        /// <para>
        /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
        /// the policy page.
        /// </para>
        /// </summary>
        public static string? PolicyPageTitle { get; set; } = "Policies";

        private const string SystemNamespaceDefault = "System";
        private static string _SystemNamespace = SystemNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the system namespace.
        /// </para>
        /// <para>
        /// Default is "System"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? SystemNamespace
        {
            get => _SystemNamespace;
            set
            {
                _SystemNamespace = string.IsNullOrWhiteSpace(value)
                    ? SystemNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// <para>
        /// The API key to be used for Tenor GIF integration.
        /// </para>
        /// <para>
        /// Leave <see langword="null"/> (the default) to omit GIF functionality.
        /// </para>
        /// </summary>
        public static string? TenorAPIKey { get; set; }

        private const string UserNamespaceDefault = "Users";
        private static string _UserNamespace = UserNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the user namespace.
        /// </para>
        /// <para>
        /// Default is "Users"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? UserNamespace
        {
            get => _UserNamespace;
            set
            {
                _UserNamespace = string.IsNullOrWhiteSpace(value)
                    ? UserNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// Adds one or more namespaces to the list of reserved names which may not be assigned to
        /// pages by non-admin users.
        /// </summary>
        /// <param name="namespaces"></param>
        public static void AddAdminNamespace(params string[] namespaces)
        {
            for (var i = 0; i < namespaces.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(namespaces[i]))
                {
                    (_AdminNamespaces ??= new List<string>()).Add(namespaces[i]);
                }
            }
        }
    }
}
