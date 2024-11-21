# Changelog

## 0.27.2-preview
### Fixed
- WikiLink Preview rendering

## 0.27.1-preview
### Changed
- Improve WikiLink handling
### Fixed
- Escape script variables

## 0.27.0-preview
### Updated
- Update to .NET 9
- Update dependencies

## 0.26.14-preview
### Fixed
- Do not encode transclusions

## 0.26.13-preview
### Fixed
- Anonymous transclusions

## 0.26.12-preview
### Fixed
- Missing references

## 0.26.11-preview
### Fixed
- Ignore correct properties for serialization

## 0.26.10-preview
### Changed
- `GetUserMaxUploadLimit` to `GetUserMaxUploadLimitAsync`

## 0.26.9-preview
### Added
- `GetUserMaxUploadLimit` to `WikiExtensions`
### Removed
- `UserMaxUploadLimit` from `IWikiGroupManager`

## 0.26.8-preview
### Changed
- Allow null parameters

## 0.26.7-preview
### Added
- `IWikiOwner` polymorphism

## 0.26.6-preview
### Fixed
- Category child lists
- Redirects

## 0.26.5-preview
### Fixed
- Invalid page creation

## 0.26.4-preview
### Fixed
- Ignore-missing link formatting

## 0.26.3-preview
### Fixed
- Wiki link parsing bug

## 0.26.2-preview
### Updated
- Update dependencies

## 0.26.1-preview
### Fixed
- Wiki link rendering

## 0.26.0-preview
### Changed
- Custom function syntax replaced by Handlebars templating
- Custom wiki link syntax largely replaced by CommonMark reference links, with some deviations
- Custom search replaced by Lucene

## 0.25.4-preview
### Added
- `PageTitle.ToString` overload which accepts a `WikiOptions` parameter, and uses the configured `MainPageTitle` when the title is empty

## 0.25.3-preview
### Fixed
- Allow build targets from `SmartComponents.LocalEmbeddings`

## 0.25.2-preview
### Added
- `WikiArchiveJsonSerializerOptions`: singleton `JsonSerializerOptions` for `Archive`

## 0.25.1-preview
### Changed
- Added more types to `WikiJsonSerializerContext`

## 0.25.0-preview
### Added
- `Text` property to `MarkdownItem` for plain text representation of the content
- Semantic search capabilities
### Changed
- Made `Html`, `MarkdownContent`, and `Preview` properties on `MarkdownItem` nullable
### Removed
- `WikiLinks` property on `MarkdownItem`

## 0.24.4-preview
### Added
- Allow restoring an archive to a new domain

## 0.24.3-preview
### Changed
- Remove unused return type for `OnRenamed`

## 0.24.2-preview
### Added
- `OnRenamed` function in `WikiOptions`

## 0.24.1-preview
### Fixed
- Topic serialization

## 0.24.0-preview
### Changed
- Update to .NET 8

## 0.23.0-preview
### Added
- `OwnerPage` subclass `Article`
- `GroupPage` and `UserPage` subclasses of `OwnerPage`
- `AllowedViewer/Editor(Group)Objects`, `CanRename`, `DisplayHtml`, `DisplayTitle`, `IsDiff`, `OwnerObject`, `Permission`, `RevisionHtml` properties to `Page`
- `Files`, `Pages`, `Subcategories` properties to `Category`
### Removed
- Most query record types; the additional properties these added to the main `Page` result were added directly to `Page` (see above)

## 0.22.2-preview
### Added
- `TitleRequest`
- `GetTitleAsync` to `WikiExtensions`

## 0.22.1-preview
### Updated
- Update dependencies

## 0.22.0-preview
### Changed
- Made `GetWikiLinks` in `MarkdownItem` public

## 0.21.0-preview
### Changed
- Update to .NET 8 preview

## 0.20.1-preview
### Added
- `SetContentAsync` to `MarkdownItem`

## 0.20.0-preview
### Added
- Headings

## 0.19.4-preview
### Fixed
- URL generation with route

## 0.19.3-preview
### Fixed
- Match main title as if missing

## 0.19.2-preview
### Fixed
- `PageTitle` parsing bug

## 0.19.1-preview
### Fixed
- Preserve page histories when restoring an archive

## 0.19.0-preview
### Changed
- Only pages which can be viewed by anyone can be transcluded

## 0.18.6-preview
### Fixed
- Creating archives

## 0.18.5-preview
### Fixed
- Retrieval of main page when title is explicit

## 0.18.4-preview
### Fixed
- URL generation for main titles

## 0.18.3-preview
### Fixed
- Implement `AllowedViewDomains` for groups

## 0.18.2-preview
### Changed
- De-duplicate wikilink lists

## 0.18.1-preview
### Changed
- Minimize archive serialization

## 0.18.0-preview
### Changed
- Do not archive topics

## 0.17.0-preview
### Changed
- Made `Topic.GetTitle` public

## 0.16.0-preview
### Changed
- Archive topics

## 0.15.0-preview
### Changed
- Revisions are now based on a wiki domain, namespace, and title (rather than an article ID), and
  have a `PageHistory` collection object for direct retrieval (without querying over a collection).
- Messages now have a `Topic` collection object for direct retrieval (without querying over a collection).

## 0.14.1-preview
### Changed
- Update to .NET 7

## 0.14.0-preview
### Changed
- Allow archiving non-domain content only

## 0.13.3-preview
### Added
- Archives

## 0.13.2-preview
### Fixed
- Double execution of scripts

## 0.13.1-preview
### Added
- Domain information to category query objects

## 0.13.0-preview
### Added
- Domain URLs
### Changed
- Domain delimiters changed from "\{ }" to "( )" for better URL compatibility

## 0.12.3-preview
### Added
- `GetWikiItemAsync` overload for ID

## 0.12.2-preview
### Added
- Default WikiUserManager and WikiGroupManager implementations

## 0.12.1-preview
### Changed
- Configurable default permissions

## 0.12.0-preview
### Changed
- Added domains

## 0.11.1-preview
### Changed
- Sort category content in query result

## 0.11.0-preview
### Changed
- Replaced `IList<IGrouping<string, T>>` with `Dictionary<string, List<T>>` in query classes, for (de)serialization support

## 0.10.0-preview
### Changed
- Default to camelCase for JSON source generation

## 0.9.0-preview
### Changed
- Simplified JSON serializer contexts to a single class: `WikiJsonSerializerContext`

## 0.8.10-preview
### Changed
- Added support for categories to `AddOrReviseWikiItemAsync`, made using `FileNamespace` throw an exception

## 0.8.9-preview
### Added
- `AddOrReviseWikiItemAsync` to extensions

## 0.8.8-preview
### Changed
- Nullability of query responses

## 0.8.7-preview
### Added
- Added overloads for `GetWikiItemAtTimeAsync` which accept a `long` timestamp

## 0.8.6-preview
### Added
- Added overloads to query methods which accept an `IWikiUser`

## 0.8.5-preview
### Fixed
- What links here paging

## 0.8.4-preview
### Changed
- Modified constructors to add support for group permissions.

## 0.8.3-preview
### Changed
- Group page edit permission based solely on group membership.

## 0.8.2-preview
### Changed
- Adjusted parameter order of `GetWikiPageUrl` overload to avoid ambiguity.

## 0.8.1-preview
### Added
- `GetWikiItemAtTimeAsync` - gets a `WikiItemInfo` record for the most recent revision of a wiki
  page at a given time.

## 0.8.0-preview
### Added
- Parent interface for `IWikiUser` and `IWikiGroup`: `IWikiOwner`
- Default implementation of `IWikiUser`: `WikiUser`
- Default implementation of `IWikiGroup`: `WikiGroup`
- Wiki query capabilities previously implemented in the client project(s) have been given base
  implementations in this library, to simplify the process of implementing a client. Most are
  implemented as extension methods, including:
    - `GetPermissionAsync` - calculates the permission a user has for a wiki item
    - `GetWikiItemAsync` - fetches an `Article`, `Category`, or `WikiFile`, or fetches a `WikiItemInfo` record with the underlying item, plus permission information
    - `GetWikiItemForEditingAsync` - fetches a `WikiEditInfo` record similar to a `WikiItemInfo` record, but with additional information suited to editing (such as detailed owner and allowed editor/viewer info)
    - `GetWikiItemDiffWithCurrentAsync`, `GetWikiItemDiffWithPreviousAsync`, and `GetWikiItemDiffAsync` for retrieving `WikiItemInfo` records for diffs
    - `GetCategoryAsync` - fetches a `CategoryInfo` record with information about the category and
      its content
    - `GetGroupPageAsync` - fetches a group page with information about the group and its members
    - `GetUserPageAsync` - fetches a user page with information about the user
    - `GetHistoryAsync` - gets paged revision information for a wiki item, along with editor
      information
    - `GetWhatLinksHereAsync` - gets a list of the pages which link to a wiki item
    - `GetSpecialListAsync` - gets a list of wiki items which fit the criteria of a member of the new `SpecialListType` enum
### Changed
- `WikiOptions` refactored to better support ASP.NET Options binding pattern
- `IWikiUser` and `IWikiGroup` now use `DisplayName` instead of `UserName` and `GroupName`
- Replaced various synchronous APIs with async
### Removed
- `IWikiOptions` interface removed (`WikiOptions` class is now used directly)

## 0.7.2-preview
### Updated
- Update dependencies

## 0.7.1-preview
### Changed
- Make property setters in `MarkdownItem` public for serialization support outside this library.

## 0.7.0-preview
### Added
- Simplify project structure by moving `IWikiGroup`, `IWikiGroupManager`, `IWikiUser`, `IWikiUserManager` from the `Tavenem.Wiki.Web` project into this one.
### Changed
- Simplify configuration process by merging `IWikiWebOptions` from the `Tavenem.Wiki.Web` project into the main `WikiOptions` object.

## 0.6.1-preview
### Updated
- Update dependencies

## 0.6.0-preview
### Changed
- Enable library trimming
- Add source generated serializer contexts.
### Fixed
- Indicate default namespace when entire title string is empty

## 0.5.0-preview
### Changed
- Update to .NET 7 preview

## 0.4.0-preview
### Changed
- Update to .NET 6 preview
- Update to C# 10 preview
### Removed
- Support for non-JSON serialization

## 0.3.2-preview
### Fixed
- Fix broken link update on article add/revise

## 0.3.1-preview
### Changed
- Change MarkdownItem property setter visibility to facilitate subclassing

## 0.3.0-preview
### Changed
- Changed MarkdownItem constructor, method visibility and property attributes to facilitate
  subclassing.
### Updated
- Updated Markdig dependency.

## 0.2.3-preview
### Fixed
- Fixed editor parameter signature in creation callback.

## 0.2.2-preview
### Added
- Add editor to creation callback.

## 0.2.1-preview
### Added
- Add missing setters to callbacks.

## 0.2.0-preview
### Added
- Add creation, deletion, and edit callbacks.

## 0.1.0-preview
### Added
- Initial preview release