namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A user of the wiki. Your subclass of <see
    /// cref="Microsoft.AspNetCore.Identity.IdentityUser"/> should implement this interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that it isn't necessary to use the version of <see
    /// cref="Microsoft.AspNetCore.Identity.IdentityUser"/> which utilizes a <see cref="string"/>
    /// primary key. However, if a different type of key is used, your subclass must provide a
    /// unique <see cref="string"/> <see cref="Id"/> property. Perhaps the result of <c>ToString</c>
    /// for your actual key, or perhaps an independant value.
    /// </para>
    /// <para>
    /// You are not required to use ASP.NET Identity for your primary identity store, if you have
    /// other business requirements. A <see
    /// cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> is used to manage user access
    /// within this library, however. You could maintain a separate Identity user store for the
    /// wiki, and coordinate between the two sets of users via their IDs. Or you could manually
    /// supply a version of <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> to the
    /// dependency injection service which works with whichever alternate identity system you
    /// utilize.
    /// </para>
    /// </remarks>
    public interface IWikiUser
    {
        /// <summary>
        /// The unique ID of this user.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Whether this user may upload files.
        /// </summary>
        bool HasUploadPermission { get; }

        /// <summary>
        /// Whether this user's account has been (soft) deleted.
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Whether this user's account has been disabled.
        /// </summary>
        bool IsDisabled { get; }

        /// <summary>
        /// The user name for this user.
        /// </summary>
        string UserName { get; }
    }
}
