namespace Tavenem.Wiki;

/// <summary>
/// The type of permission a user has for a wiki page.
/// </summary>
[Flags]
public enum WikiPermission
{
    /// <summary>
    /// No permission; access to the page should be denied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Read permission; the user may view the page's content.
    /// </summary>
    Read = 1 << 0,

    /// <summary>
    /// Write permission; the user may edit the page's content.
    /// </summary>
    Write = 1 << 1,

    /// <summary>
    /// Read-write permission; the user may view and edit the page's content.
    /// </summary>
    ReadWrite = Read | Write,

    /// <summary>
    /// Create permission; the user may create a page which does not yet exist.
    /// </summary>
    /// <remarks>
    /// Also covers permission to move or rename another page to the namespace and title in
    /// question, which effectively creates it.
    /// </remarks>
    Create = 1 << 2,

    /// <summary>
    /// Delete permission; the user may delete the page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Also covers permission to move or rename the page, which effectively delete it from its
    /// current namespace and title.
    /// </para>
    /// <para>
    /// Note that <see cref="Write"/> permission alone permits replacing a page's content with a
    /// redirect.
    /// </para>
    /// </remarks>
    Delete = 1 << 3,

    /// <summary>
    /// Permission to set permissions; the user may set the page's allowed viewers and editors.
    /// </summary>
    SetPermissions = 1 << 4,

    /// <summary>
    /// Permission to set the page's owner.
    /// </summary>
    SetOwner = 1 << 5,

    /// <summary>
    /// All permissions.
    /// </summary>
    All = ReadWrite | Create | Delete | SetPermissions | SetOwner,
}
