﻿using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A user of the wiki.
/// </summary>
/// <remarks>
/// Note that it is not necessary to use this user object in your wiki implementation. You may use
/// any object which implements the <see cref="IWikiUser"/> interface. This class is provided as a
/// convenient default implementation, if your use case does not require any additional information,
/// or if it is possible for you to inherit from this as a base class.
/// </remarks>
public class WikiUser : IdItem, IWikiUser
{
    /// <summary>
    /// A list of the pages which this user has permission to edit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Page.AllowedEditors"/> or <see cref="Page.AllowedEditorGroups"/> list. Rather, the
    /// lists are expected to be complementary. Pages may list entities with permission to edit
    /// them, and entities may also have a separate list of pages which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit a page, if either the page indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the page, then
    /// permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// </remarks>
    public virtual IList<PageTitle>? AllowedEditPages { get; set; }

    /// <summary>
    /// A list of the pages which this user has permission to view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Page.AllowedViewers"/> or <see cref="Page.AllowedViewerGroups"/> list. Rather, the two
    /// lists are expected to be complementary. Pages may list entities with permission to view
    /// them, and entities may also have a separate list of pages which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view a page, if either the page indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the page, then
    /// permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// </remarks>
    public virtual IList<PageTitle>? AllowedViewPages { get; set; }

    /// <summary>
    /// A list of the names of domains in which this user has permission to view articles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The user's effective permission is determined by the combination of this property, the <see
    /// cref="WikiGroup.AllowedViewDomains"/> property, and the <see
    /// cref="IPermissionManager.GetDomainPermissionAsync"/> function, as well as any access
    /// controls on the specific article, which override the general permissions for the domain, if
    /// present.
    /// </para>
    /// <para>
    /// Note that the default when no permission is specified is to be denied access (unlike the
    /// default for non-domain articles, which is to grant full access even to anonymous users).
    /// </para>
    /// </remarks>
    public virtual IList<string>? AllowedViewDomains { get; set; }

    /// <summary>
    /// The display name for this user.
    /// </summary>
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// A list of the group IDs to which this user belongs (if any).
    /// </summary>
    public virtual IList<string>? Groups { get; set; }

    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string WikiUserIdItemTypeName = ":WikiUser:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => WikiUserIdItemTypeName;

    /// <summary>
    /// Whether this user's account has been (soft) deleted.
    /// </summary>
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// Whether this user's account has been disabled.
    /// </summary>
    public virtual bool IsDisabled { get; set; }

    /// <summary>
    /// Whether this user is a wiki administrator.
    /// </summary>
    public virtual bool IsWikiAdmin { get; set; }

    /// <summary>
    /// <para>
    /// The total number of kilobytes of uploaded files permitted for this user.
    /// </para>
    /// <para>
    /// A negative value indicates that the user may upload files without limit.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If the user is a member of a group with a configured upload limit, the effective upload
    /// limit is the greatest of the limits set for the user and all its groups.
    /// </remarks>
    public virtual int UploadLimit { get; set; }
}
