# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- Handle Unity Gui Skin typo change from "ToolbarSeachCancelButton" to "ToolbarSearchCancelButton".

## [0.1.1]

### Added
- Introduced `SketchDependsOverrideAttribute`, allowing for more specific dependency injection for subclasses of sketches. This provides greater flexibility when extending abstract sketches.
- Removed the requirement for the ".Sketches" assembly naming convention. Users can now create and run sketches without adhering to this convention, allowing for more flexibility in less structured projects.

### Changed
- While it was previously required that the sketch class exists inside of an assembly with a name ending in ".Sketches", this requirement has been removed to accommodate less structured projects. However, maintaining a clear structure in your project is still recommended when possible.

## [0.1.0]

### Changed

- Changed the top level namespace (This is a breaking change). Please update your imports accordingly.
- Added a logo to the README file for improved project branding. 

## [0.0.2]

### Added

- Runtime asm, containing all attributes.
- Sketches can now inherit from ScriptableObject.
- Button to select the Sketch asset.
- Sample - Set-up sketch assembly definition with empty sketch included.
- Sample - Simple examples of Sketch usage.

### Changed

- Sketch window now uses Name, not FullyQualifiedName.
- Flume is now an optional dependency; adding additional function, but not required.

### Fixed

- Error when a sketch search resulted in no results.
- SketchRunner correctly returns to the previous scene when stopping.

## [0.0.1]

### Added

- Initial public release
