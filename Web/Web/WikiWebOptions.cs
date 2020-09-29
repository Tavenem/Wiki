using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Options for the wiki web client.
    /// </summary>
    public class WikiWebOptions : IWikiWebOptions
    {
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
        public string? AboutPageTitle { get; set; } = "About";

        private const string AdminGroupNameDefault = "Wiki Admins";
        private string _adminGroupName = AdminGroupNameDefault;
        /// <summary>
        /// <para>
        /// The name of the admin user group.
        /// </para>
        /// <para>
        /// If omitted "Wiki Admins" is used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? AdminGroupName
        {
            get => _adminGroupName;
            set
            {
                _adminGroupName = string.IsNullOrWhiteSpace(value)
                    ? AdminGroupNameDefault
                    : value;
            }
        }

        private List<string>? _adminNamespaces;
        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by non-admin
        /// users.
        /// </para>
        /// <para>
        /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
        /// </para>
        /// </summary>
        public IEnumerable<string> AdminNamespaces => (_adminNamespaces ?? Enumerable.Empty<string>())
            .Concat(new[] { SystemNamespace });

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
        public string? ContactPageTitle { get; set; } = "Contact";

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
        public string? ContentsPageTitle { get; set; } = "Contents";

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
        public string? CopyrightPageTitle { get; set; } = "Copyright";

        private const string GroupNamespaceDefault = "Group";
        private string _groupNamespace = GroupNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the user group namespace.
        /// </para>
        /// <para>
        /// If omitted "Group" is used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? GroupNamespace
        {
            get => _groupNamespace;
            set
            {
                _groupNamespace = string.IsNullOrWhiteSpace(value)
                    ? GroupNamespaceDefault
                    : value;
            }
        }

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
        public string? HelpPageTitle { get; set; } = "Help";

        /// <summary>
        /// <para>
        /// The maximum size (in bytes) of uploaded files.
        /// </para>
        /// <para>
        /// Setting this to a value less than or equal to zero effectively prevents file uploads.
        /// </para>
        /// </summary>
        public int MaxFileSize { get; set; } = 5000000; // 5 MB

        /// <summary>
        /// Gets a string representing the <see cref="MaxFileSize"/> in a reasonable unit (GB for
        /// large sizes, down to bytes for small ones).
        /// </summary>
        public string MaxFileSizeString
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
        public string? PolicyPageTitle { get; set; } = "Policies";

        private const string SystemNamespaceDefault = "System";
        private string _systemNamespace = SystemNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the system namespace.
        /// </para>
        /// <para>
        /// If omitted "System" is used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? SystemNamespace
        {
            get => _systemNamespace;
            set
            {
                _systemNamespace = string.IsNullOrWhiteSpace(value)
                    ? SystemNamespaceDefault
                    : value;
            }
        }

        private const string UserNamespaceDefault = "User";
        private string _userNamespace = UserNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the user namespace.
        /// </para>
        /// <para>
        /// If omitted "User" is used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? UserNamespace
        {
            get => _userNamespace;
            set
            {
                _userNamespace = string.IsNullOrWhiteSpace(value)
                    ? UserNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// Adds one or more namespaces to the list of reserved names which may not be assigned to
        /// pages by non-admin users.
        /// </summary>
        /// <param name="namespaces"></param>
        /// <returns>This instance.</returns>
        public WikiWebOptions AddAdminNamespace(params string[] namespaces)
        {
            for (var i = 0; i < namespaces.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(namespaces[i]))
                {
                    (_adminNamespaces ??= new List<string>()).Add(namespaces[i]);
                }
            }
            return this;
        }
    }
}
