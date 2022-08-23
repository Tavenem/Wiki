# Changelog

## 0.8.0-preview
### Changed
- WikiOptions refactored to better support ASP.NET Options binding pattern.
- Replaced various synchronous APIs with async.
### Removed
- IWikiOptions interface removed (WikiOptions class is used directly).

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