using System.Collections.Generic;

namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Options for the wiki web client.
    /// </summary>
    public interface IWikiWebOptions
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
        string? AboutPageTitle { get; }

        /// <summary>
        /// <para>
        /// The name of the admin user group.
        /// </para>
        /// <para>
        /// If omitted "Wiki Admins" is used.
        /// </para>
        /// </summary>
        string AdminGroupName { get; }

        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by non-admin
        /// users.
        /// </para>
        /// <para>
        /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
        /// </para>
        /// </summary>
        IEnumerable<string> AdminNamespaces { get; }

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
        string? ContactPageTitle { get; }

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
        string? ContentsPageTitle { get; }

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
        string? CopyrightPageTitle { get; }

        /// <summary>
        /// <para>
        /// The name of the user group namespace.
        /// </para>
        /// <para>
        /// If omitted "Group" is used.
        /// </para>
        /// </summary>
        string GroupNamespace { get; }

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
        string? HelpPageTitle { get; }

        /// <summary>
        /// <para>
        /// The maximum size (in bytes) of uploaded files.
        /// </para>
        /// <para>
        /// Setting this to a value less than or equal to zero effectively prevents file uploads.
        /// </para>
        /// </summary>
        int MaxFileSize { get; }

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
        string? PolicyPageTitle { get; }

        /// <summary>
        /// <para>
        /// The name of the system namespace.
        /// </para>
        /// <para>
        /// If omitted "System" is used.
        /// </para>
        /// </summary>
        string SystemNamespace { get; }

        /// <summary>
        /// <para>
        /// The name of the user namespace.
        /// </para>
        /// <para>
        /// If omitted "User" is used.
        /// </para>
        /// </summary>
        string UserNamespace { get; }
    }
}