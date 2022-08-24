﻿using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// <para>
/// Represents a group of users.
/// </para>
/// <para>
/// Groups may be assigned ownership, viewing, or editing permissions on wiki items, just like
/// individual users may.
/// </para>
/// </summary>
/// <remarks>
/// Note that it is not necessary to use this group object in your wiki implementation. You may use
/// any object which implements the <see cref="IWikiGroup"/> interface. This class is provided as a
/// convenient default implementation, if your use case does not require any additional information,
/// or if it is possible for you to inherit from this as a base class.
/// </remarks>
public class WikiGroup : IdItem, IWikiGroup
{
    /// <summary>
    /// A list of the <see cref="IdItem.Id"/> values of articles which users in this group has
    /// permission to edit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Article.AllowedEditorGroups"/> list. Rather, the lists are expected to be
    /// complementary. Articles may list groups with permission to edit them, and groups may also
    /// have a separate list of articles which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit an article, if either the article indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the article,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// </remarks>
    public virtual IList<string>? AllowedEditArticles { get; set; }

    /// <summary>
    /// A list of the <see cref="IdItem.Id"/> values of articles which users in this group has
    /// permission to view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Article.AllowedViewerGroups"/> list. Rather, the two lists are expected to be
    /// complementary. Articles may list groups with permission to view them, and groups may
    /// also have a separate list of articles which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view an article, if either the article indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the article,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// </remarks>
    public virtual IList<string>? AllowedViewArticles { get; set; }

    /// <summary>
    /// The display name for this group.
    /// </summary>
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string WikiGroupIdItemTypeName = ":WikiGroup:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => WikiGroupIdItemTypeName;

    /// <summary>
    /// The <see cref="IWikiOwner.Id"/> of the owner/administrator of this group.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the owner of a group may create, delete, or move its group page, set view and edit
    /// permission on its group page, invite or remove users, and change the group's ownership.
    /// </para>
    /// <para>
    /// Although this property is nullable to avoid causing trouble for initialization and/or
    /// (de)serialization processes, a group is considered invalid without a current owner. Such
    /// groups will be treated as not existing when encountered in most situations.
    /// </para>
    /// </remarks>
    public virtual string? OwnerId { get; set; }

    /// <summary>
    /// <para>
    /// The total number of kilobytes of uploaded files permitted for users in this group.
    /// </para>
    /// <para>
    /// A negative value indicates that users in the group may upload files without limit.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If a user is a member of a group with a configured upload limit, the effective upload limit
    /// is the greatest of the limits set for the user and all its groups.
    /// </remarks>
    public virtual int UploadLimit { get; set; }
}
