﻿using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// Represents any entity who may own a wiki item: either an individual user, or a group of users.
/// </summary>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(IWikiUser), ":IWikiUser:")]
[JsonDerivedType(typeof(WikiUser), WikiUser.WikiUserIdItemTypeName)]
[JsonDerivedType(typeof(IWikiGroup), ":IWikiGroup:")]
[JsonDerivedType(typeof(WikiGroup), WikiGroup.WikiGroupIdItemTypeName)]
public interface IWikiOwner
{
    /// <summary>
    /// A list of the <see cref="IdItem.Id"/> values of articles which this entity has permission to
    /// edit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Article.AllowedEditors"/> or <see cref="Article.AllowedEditorGroups"/> list. Rather,
    /// the lists are expected to be complementary. Articles may list entities with permission to
    /// edit them, and entities may also have a separate list of articles which they may edit.
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
    IList<string>? AllowedEditArticles { get; set; }

    /// <summary>
    /// A list of the <see cref="IdItem.Id"/> values of articles which this entity has permission to
    /// view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="Article.AllowedViewers"/> or <see cref="Article.AllowedViewerGroups"/> list. Rather,
    /// the two lists are expected to be complementary. Articles may list entities with permission
    /// to view them, and entities may also have a separate list of articles which they may view.
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
    IList<string>? AllowedViewArticles { get; set; }

    /// <summary>
    /// A list of the names of domains in which this entity has permission to view articles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="WikiOptions.GetDomainPermission"/> function will also be invoked with this
    /// entity's <see cref="IdItem.Id"/>, if an implementation for the function is provided.
    /// </para>
    /// <para>
    /// The entity's effective permission is determined by the combination of this property and <see
    /// cref="WikiOptions.GetDomainPermission"/>, as well as any access controls on the specific
    /// article, which override the general permissions for the domain, if present.
    /// </para>
    /// <para>
    /// Note that the default when no permission is specified is to be denied access (unlike the
    /// default for non-domain articles, which is to grant full access even to anonymous users).
    /// </para>
    /// </remarks>
    IList<string>? AllowedViewDomains { get; set; }

    /// <summary>
    /// The display name for this entity.
    /// </summary>
    string? DisplayName { get; set; }

    /// <summary>
    /// The unique ID of this entity.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>
    /// The total number of kilobytes of uploaded files permitted for this entity.
    /// </para>
    /// <para>
    /// A negative value indicates that the entity may upload files without limit.
    /// </para>
    /// </summary>
    int UploadLimit { get; set; }
}
