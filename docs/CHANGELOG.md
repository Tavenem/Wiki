# Changelog

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
- Add creation, deleteion, and edit callbacks.

## 0.1.0-preview
### Added
- Initial preview release