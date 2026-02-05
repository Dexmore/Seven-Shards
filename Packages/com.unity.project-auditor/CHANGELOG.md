# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.1] - 2025-02-06

### Changed
* Don't suggest switching Physics 2D update mode to Script for now.

### Fixed
* Fixed an issue where Project Auditor exhibited compilation issues on certain versions of the Editor.
* Fixed link to help pages.
* Fix for PROFB-273; Change summary screen message when an area is not analyzed.
* Fix for PROFB-274; Added dividers between the foldouts on the shader variant page.
* Fix for PROFB-277; change text for "1 ignored are hidden".
* Fix for PROFB-278; 1 items changed to 1 item(s) in the table.
* Fix for PROFB-279; Build steps all show as info until you hit refresh.
* Fix for PROFB-280; Removed Path option from the Build Size context menu.
* Fix for PROFB-281; Unable to sort columns on Build - Build Size screens.
* Fix for PROFB-284; change references to Texture Streaming when it's now called Mipmap Streaming.
* Fix for PROFB-285; fix for not being able to hide info compiler messages in the compiler messages view.
* Fix for PROFB-286; fix performance issues when selecting lots of table items.
* Fix for PROFB-287; improve documentation for installation instructions.
* Fix for PROFB-289; exclude shaders that are Hidden from asset issues.
* Fix for PROFB-290; fix settings links for Physics/Physics 2D.
* Fix for PROFB-291; issue where rendering path recommendations would show for the wrong pipeline.
* Fix for PROFB-300; file extensions would sometimes throw because a file didn't have one.
* Fix for PROFB-310; fix for Analyzing assets causing shader issues to vanish.
* Fix for PROFB-314; speculative attempt to fix out of memory crash reported on Discussions.

## [1.0.0] - 2025-02-05

### Changed
* Version number bumped to 1.0.0 for release.
* Added QAReport.md.

## [1.0.0-pre.7] - 2025-01-31

### Fixed
* Fix for COPT-3333; have the Assembly view always deal in milliseconds for time values to help with eg. consistency in exported data.
* Fix for COPT-3376; don't look at compiled assemblies if this is a different project.
* Fix for COPT-3401; clear the search strings when starting/loading an analysis.
* Fix for COPT-3412; avoid issues with zoom slider for now by hiding it.
* Fix for COPT-3415; show message instead of NaN for Assembly Compile Time when Compilation Mode is set to Editor.
* Fix for COPT-3418; optimize performance when selecting to group by in large tables.
* Fix for COPT-3428; guard against issues from analysing empty sprite atlases.
* Fix for COPT-3440; remove the asset category from the shader module so it doesn't get analyzed with shaders.
* Fix for COPT-3452; audio clips runtime size is often wrong.

## [1.0.0-pre.6] - 2024-11-29

### Changed
* Changed the colours on the summary so the icons and charts match those from style guides

### Fixed
* Fix for COPT-3240; welcome screen text layout breaks.
* Fix for COPT-3241; don't allow sorting on the Build Steps page.
* Fix for COPT-3246; build steps - clicking on an item with long text removes ability to scroll the table.
* Fix for COPT-3250; fill out the project revision field on the summary page.
* Fix for COPT-3252; make sorting by name use alphanumeric comparison, and ignore case
* Fix for COPT-3254; grouped animation clip length now sorts by value rather than alphabetically
* Fix for COPT-3257; make enough room for the close bracket.
* Fix for COPT-3259; stop the shader variant checkbox from vanishing when you tick it.
* Fix for COPT-3260; fix the shader variant UI path to match it's location in Unity 6.
* Fix for COPT-3262; issue where summary page could be broken when performing a new analysis after loading an existing one.
* Fix for COPT-3263; remove the clear and apply buttons from the domain reload filter dialog.
* Fix for COPT-3276; loading a report in a different project results in errors when opening issues.
* Fix for COPT-3277; give user an option to enable all analysis categories if none are selected when starting a new analysis.
* Fix for COPT-3279; when exporting csv after loading/serializing we could have the same column repeated.
* Fix for COPT-3280; export button can be hard to click, changed to a dropdown.
* Fix for COPT-3281; blank project produces over 7800 issues.
* Fix for COPT-3283; summary updates and shows information about ignored items.
* Fix for COPT-3286; make the group by section in the table header wider.
* Fix for COPT-3288; shader compiler messages platform column contains numbers.
* Fix for COPT-3291; remove page for compute shaders for now because it's broken.
* Fix for COPT-3293; set a maximum width for the inspect buttons in the summary section.
* Fix for COPT-3295; fix for clicking 'more details' in the top ten producing errors when clicked
* Fix for COPT-3296; don't show the filter option on the context menu if multiple items are selected, and change not to include the item name.
* Fix for COPT-3299; avoid Area filters affecting analysis views that don't support them.
* Fix for COPT-3300; include/exclude ignored items button is confusing from a UI perspective.
* Fix for COPT-3302; severity levels could be unset after loading in an old analysis.
* Fix for COPT-3303; 'Unknown' severity could be shown if severity levels were unset.
* Fix for COPT-3304; show a dialog to tell the user this report doesn't match the project and disable analysis on areas.
* Fix for COPT-3305; rename 'Num Variants' to 'Max Variants' to clarify the column's meaning.
* Fix for COPT-3309; improved UX around Quick Fix.
* Fix for COPT-3314; don't show a filtering context menu if the active view doesn't support the "Filters" section.
* Fix for COPT-3318; null reference when CSV exporting the build size data.
* Fix for COPT-3319; fix for the status bar being cut off the bottom of the window.
* Fix for COPT-3324; fix audio clip module name from "AudioClips" to "Audio Clips"
* Fix for COPT-3330; exporting doesn't work on startup with the window open.
* Fix for COPT-3334; unhide the packages Name column to be consistent with the right click menu.
* Fix for COPT-3336; remove question marks on the end of some fields.
* Fix for COPT-3337; re-enable the domain reload analysis.
* Fix for COPT-3341; change the package description.
* Fix for COPT-3343; fix for ignored issues not being exported in CSV.
* Fix for COPT-3345; fix project areas preference not applying correctly.
* Fix for COPT-3351; don't cut Windows computer names short, and fix Mac platform naming.
* Fix for COPT-3355; fail build on issues option re-worded.
* Fix for COPT-3370; correct sorting in package name column.
* Fix for COPT-3373; fix for warning about a missing layout.
* Fix for COPT-3374; add support for SpriteAtlas analysis.
* Fix for COPT-3375; some old reports would not load correctly due to invalid layout infomration.
* Fix for COPT-3395; gave project auditor project settings panel a UX pass.
* Fix for COPT-3377; improve handling for loading random JSON files.
* Fix for COPT-3378; update summary name title based on report's filename.
* Fix for COPT-3380; change the wording on the "not been analyzed" message.
* Fix for COPT-3389; project setup -> settings in the text
* Fix for COPT-3391; match platform strings with Build Profiles menu.
* Fix for COPT-3394; swap the save and load buttons to match other profiling tools.
* Fix for COPT-3399; sanitise project name when using it as the default save name for a report.
* Fix for COPT-3405; group heading can clip with next column in table.
* Fix for COPT-3409; have desktop platforms grouped for project settings.

## [0.11.0] - 2023-05-03

### Added
* Autosave feature keeps current report persistent upon closing Project Auditor
* Summary UI shows a report's display name with star indicator for an unsaved report
* Requester when a report is unsaved and user clicks "New Analysis" button
* Display/Ignore All buttons
* Copy-to-Clipboard buttons to Details and Recommendations
* Vertical scrollbar to Details and Recommendations
* CompilationMode selection to Welcome page
* Shader view reports total property count and texture property count for Unity 2019.3 and above
* Compute shader variant view reports kernel thread count for Unity 2021.2 and above
* View-specific descriptions
* AudioClip diagnostics
* Material list view in shader module
* AnimatorController reporting
* AnimationClip reporting
* Avatar reporting
* AvatarMask reporting
* Domain Reload Roslyn analyzer
* New Area category for IterationTime issues
* Code/Domain Reload view, showing any issues raised by the Domain Reload Roslyn analyzer
* Domain Reload diagnostic issues in the Settings view if Domain Reload is enabled
* Diagnostics for unsupported APIs on WebGL target
* Diagnostic for Static Batching being enabled whilst Entities Graphics is installed
* Support for cancelling analysis

### Changed
* UI redesign of Summary showing issue breakdown, list of top ten issues, and buttons to jump to additional insights/views
* Improved issue description and suggestion strings, to make them clearer and more consistent
* Solid Color Texture analyzer now works for all texture types (2D, 2DArray, 3D, Cube) in Unity 2019.2 and above
* Upgrade com.unity.nuget.mono-cecil to 1.11.4
* Visualization of the Horizontal Stacked Bar and Legend Item
* Improved AudioClip asset table
* Clear table selection on unmuting issues
* Move some data that should be user-configurable to UserPreferences
* Report serialization files changed so that saved JSON is <= half the size it was before
* Configuration is now handled via a ProjectAuditorSettings class/asset
* Refactored and documented public API to allow CI/CD integration
* Refactored, exposed and documented API to allow the creation of custom ModuleAnalyzers
* Table views automatically resize columns to fit the data being displayed
* Almost complete rewrite of the package documentation

### Removed
* Actions section and Mute/Unmute buttons
* Settings from ProjectAuditorConfig
* Support for Unity 2018 and 2019.
* BuildReports are no longer saved into the project's Assets folder, and are instead cached in callbacks or read directly from the Library folder.

### Fixed
* Analysis never completes if an exception is thrown
* Compilation if both URP and HDRP are installed
* UI default column sorting
* UI sorting criteria persistence after domain reload
* Config changes not saved to corresponding asset
* Ignoring texture streaming issues
* Texture size reporting on Unity 2022.2 and above is now accurate
* Unusually-formatted compiler warnings (such as the one generated by including mcs.rsp in the project) no longer cause all code compilation and analysis to silently fail.

## [0.10.0] - 2023-05-03

### Changed
* Bumped analytics events version number from v1 to v2
* Made many APIs internal rather than public
* Added properly-formatted comments to enable API documentation for all remaining public types and methods
* Changes to documentation in .md files to ensure standards compliance
* Ignore SpriteAtlas test failures

### Removed
* Automated tests from package

## [0.9.4-preview] - 2023-03-27

### Added
* SRP Asset Settings analyzer
* Shader SRP Batcher analyzer
* Solid Color Texture analyzer
* Texture anisotropic level analyzer
* Universal Render Pipeline analyzer

### Changed
* CHANGELOG.md format to ensure it adheres to Unity standards
* Asset diagnostics IDs.

## [0.9.3-preview.3] - 2023-02-28

### Fixed
* Lines and bars drawing
* Missing "Read/Write" diagnostic recommendations

## [0.9.3-preview.2] - 2023-02-21

### Added
* Texture mipmaps streaming analyzer

### Fixed
* View switching cancellation

## [0.9.3-preview.1] - 2023-02-14

### Added
* Percentage formatting support
* Individual asset size percentage to Build Report
* Test utility classes to package

## [0.9.2-preview] - 2023-02-07

### Added
* Added Fog shader variant stripping analyzer
* Added IL2CPP Compiler Configuration analyzer

### Fixed
* Backwards compatibility
* Reporting of shader variants if not compiled for analysis platform
* Displaying of large values of total shader variants
* _Copy to Clipboard_ support of issue property
* Table sorting

## [0.9.1-preview] - 2023-01-24

### Added
* _UnityEngine.Object.FindObjectOfType_ usage detection
* _Settings_ asset for configuring analyzers
* _Severity_ information to diagnostics UI

### Fixed
* Names of build-generated assets in Build Report
* Parsing of unnamed shader passes in Unity 2021.2.14+
* _UnityEngine.AudioSettings_ speaker mode diagnostic

## [0.9.0-preview] - 2022-12-01

### Added
* Diagnostic area _Quality_, _Support_ and _Requirement_
* _documentation_ support to descriptor
* Issue _fixer_ support to descriptor
* Package diagnostics
* On-demand _Texture_, _AudioClip_, _Mesh_ modules
* Compute Shader Variants support

### Fixed
* Over-reporting of built shader variants count
* Export of filtered/selected non-diagnostic issues
* Build Report object name
* Text alignment and wrapping issues
* Build report steps text wrapping
* Diagnostics _critical_ property persistence after domain reload
* Improved text search to match custom properties

## [0.8.4-preview] - 2022-09-27

### Added
* Packages module to report installed packages and dependencies

### Fixed
* Analysis platform on incremental audit
* Compilation error due to newer com.unity.nuget.mono-cecil

## [0.8.3-preview] - 2022-09-05

### Added
* HTML export support
* _Packages_ module as _Experimental_
* _params_ array allocation diagnostic

### Fixed
* *NullReferenceException* on Draw2D shader not being found

## [0.8.2-preview] - 2022-07-25

### Added
* User preferences
* Group size/time properties
* Support for analyzing all compiled Editor assemblies
* Platform selection to _Home_ screen

### Changed
* Descriptor ID type from _int_ to _string_

### Fixed
* Diagnostic Rules serialization
* *Home* page *NullReferenceException* on Build
* *NullReferenceException* on export of non-diagnostic issues
* Improved issue creation code-readability by using *ProjectIssueBuilder*

## [0.8.1-preview] - 2022-06-24

### Added
* *ProjectAuditorConfig* option to enable/disable Roslyn analyzers
* *ProjectAuditorParams* option for compiling selected assemblies
* Discard button to toolbar
* Modules selection to _Home_ screen
* Support for reporting precompiled assemblies

### Changed
* Renamed asynchronous *ProjectAuditor.Audit* to *AuditAsync*

### Fixed
* Compatibility with Unity 2022
* Improved code analysis performance by caching "resolved" types

## [0.8.0-preview] - 2022-05-20

### Added
* *ProjectAuditor.NumCategories* API
* Module-specific incremental analysis support
* Support to disable a module by default
* 'Clear Selection' and 'Filter by Description' options to context menu
* *SavePath* to configuration asset
* [Graphics Tier](https://docs.unity3d.com/ScriptReference/Rendering.GraphicsTier.html) information to reported Shader Variants
* Diagnostic message formatting support
* Dependencies panel to assembly view
* *ImporterType* to Build File properties

### Changed
* Default compilation mode to Non-Development
* Replaced *AnalyzeEditorCode* with *CompilationMode* setting

### Fixed
* Reporting of assemblies not compiled due to dependencies
* Improved code diagnostic messages
* Improved UI groups to support arbitrary grouping criteria

### Removed
* Removed the need to have a Descriptor associated with non-diagnostic issues

## [0.7.6-preview] - 2022-04-22

### Fixed
* Build Report analysis 'Illegal characters in path' exception
* Shaders analysis 'Illegal characters in path' exception
* Compilation warnings
* Export of variants with no keywords

## [0.7.5-preview] - 2022-04-20

### Added
* Groups support to Shaders view
* Support for exporting Shader Variants as [Shader Variant Collection](https://docs.unity3d.com/ScriptReference/ShaderVariantCollection.html)

### Changed
* Optimized call tree building and visualization

## [0.7.4-preview] - 2022-03-25

### Added
* *OnRenderObject* and *OnWillRenderObject* to list of MonoBehavior critical contexts
* Compilation Time property to Assemblies view
* Public API to get float/double custom property
* Context menu item to open selected issue

### Changed
* Optimized viewing and sorting UI performance

### Fixed
* Closure allocation diagnostic message
* Sorting of call hierarchy nodes

## [0.7.3-preview] - 2022-03-01
### Added
* *UnityEngine.Object.name* code diagnostic
* *Severity* filters support

### Fixed
* Unreported assemblies that failed to compile
* View switching if any module is unsupported
* Database of API usage descriptors

### Removed
* Redundant API usage descriptors

## [0.7.2-preview] - 2022-01-21

### Added
* Shader *Size*, *Source Asset* and *Always Included* info to Shaders view
* Shader *Severity* column to indicate any compiler message
* *Stage*, *Pass Type* and *Platform Keywords* to Shader Variants view
* Shader Variants view right scrollable panels
* Shader Compiler Messages reporting

### Fixed
* Usage of deprecated shader API
* Shader compilation log parsing in 2021 or newer
* Cleanup of Shader Variants builds data in 2021 or newer

## [0.7.1-preview] - 2021-12-15

### Added
* Option to enable creation of BuildReport asset after each build

### Fixed
* UWP compilation issues
* *ArgumentException* on table Page Up/Down
* *InvalidOperationException* due failure to resolve asmdef
* *NullReferenceException* due to null compiler message
* *NullReferenceException* on empty table
* *ShaderCompilerData* parsing in 2021.2.0a16 or newer
* Disabling of unsupported modules
* Unreported output files from the same source asset
* Automatic creation of last BuildReport asset after build

## [0.7.0-preview] - 2021-11-29

### Added
* Documentation pages
* UI Button to open documentation page based on active view
* BuildReport Viewer UI
* Runtime Type property to BuildReport size items
* *OnAnimatorIK* and *OnAnimatorMove* to *MonoBehaviour* hot-paths

### Fixed
* *NullReferenceException* on projects with multiple dll with same name
* Variants view *ShaderRequirements* information
* Window opening after each build

## [0.6.6-preview] - 2021-10-14

### Fixed
* *ProjectReport.ExportToCSV* filtering

## [0.6.5-preview] - 2021-08-04

### Fixed
* Mono.Cecil package dependency

## [0.6.4-preview] - 2021-07-26

### Added
* *ProjectReport.ExportToCSV* to *public* API

### Fixed
* "No graphic device is available" error in batchmode

## [0.6.3-preview] - 2021-07-05

### Fixed
* *NullReferenceException* when searching Call Tree on Resources view
* *OverflowException* on reporting build sizes
* Player.log parsing if a shader name contains commas
* Persistent "Analysis in progress..." message

## [0.6.2-preview] - 2021-05-25

### Added
* Assemblies view (experimental)
* Build Report Steps view
* Overview stats to Build Report Size view

### Fixed
* Detection of HDRP mixed LitShaderMode

## [0.6.1-preview] - 2021-05-11

### Added
* HDRP settings analyzer

### Fixed
* Build Report *Build Name*
* Empty MonoBehaviour event detection
* *Graphics Tier Settings* misreporting
* Failed/cancelled report loading from file
* Improved Shader Variants analysis workflow

## [0.6.0-preview] - 2021-04-26

### Added
* Build Report support
* Compiler Messages support
* Generic types instantiation analysis
* Summary view
* Save&Load support
* *Log Shader Compilation* option to Shader Variants view
* Shaders view shortcut to Shader Variants view

### Changed
* Compilation pipeline to use AssemblyBuilder
* Shader Variants Window to simple view

### Fixed
* Shader Variants persistence in UI after Domain Reload
* Shader Variants *Compiled* column initial state
* Code Diagnostics view sorting
* Improved main documentation page

## [0.5.0-preview] - 2021-03-11

### Added
* *System.DateTime.Now* usage detection
* Descriptor's minimum/maximum version
* Splash-screen setting detection
* Zoom slider

### Changed
* Replaced tabs-like view selection with toolbar dropdown list
* Changed *Export* feature to be view-specific

### Removed
* *experimental* label from Allocation issues

### Fixed
* Reporting of issues affecting multiple areas
* Background analysis that results in code issues with empty filenames
* Android player.log parsing
* *GraphicsSettings.logWhenShaderIsCompiled* compilation error on early 2018.4.x releases
* Reduced UI managed allocations

## [0.4.2-preview] - 2021-02-01

### Added
* *SRP Batcher* column to Shader tab
* Support for parsing *Player.log* to identify which shader variants are compiled (or not-compiled) at runtime
* Shader errors/warnings reporting via Shader 'severity' icon
* [Shader Requirements](https://docs.unity3d.com/ScriptReference/Rendering.ShaderRequirements.html) column to Shader tab

### Fixed
* Detection of API calls using a derived type
* Reporting of *Editor Default Resources* shaders
* *ReflectionTypeLoadException*
* Exception when switching focus from Area/Assembly window
* *NullReferenceException* on invalid shader or vfx shader
* *NullReferenceException* when building AssetBundles
* Shader variants reporting due to *OnPreprocessBuild* callback default order

## [0.4.1-preview] - 2020-12-14

### Added
* Support for analyzing Editor only code-paths
* *reuseCollisionCallbacks* physics API diagnostic

### Changed
* Improved Shaders auditing to report both shaders and variants in their respective tables

### Fixed
* Assembly-CSharp-firstpass asmdef warning
* Backwards compatibility

## [0.4.0-preview] - 2020-11-24

### Added
* Shader variants auditing
* "Collapse/Expand All" buttons

### Changed
* Refactoring and code quality improvements

## [0.3.1-preview] - 2020-10-23

### Added
* Dependencies view to Assets tab
* Double-click on an asset selects it in the Project Window
* CI information to documentation

### Changed
* Move call tree to the bottom of the window
* Case-sensitive string search to be optional

### Fixed
* Page up/down key bug fixes
* Unity 2017 compatibility
* Default selected assemblies
* Area names filtering
* Call-tree serialization

## [0.3.0-preview] - 2020-10-07

### Added
* Auditing of assets in Resources folders
* Shader warmup issues

### Changed
* Reorganized UI filters and mute/unmute buttons in separate foldouts
* Better names for project settings issues

### Fixed
* Issues sorting within a group
* ExportToCSV improvements

## [0.2.1-preview] - 2020-05-22

### Changed
* Improved text search UX
* Improved test coverage
* Updated documentation

### Fixed
* Background assembly analysis
* Lost issue location after domain reload
* Tree view selection when background analysis is enabled
* Yamato configuration

## [0.2.0-preview] - 2020-04-27

### Added
* Boxing allocation analyzer
* Empty *MonoBehaviour* method analyzer
* *GameObject.tag* issue type to built-in analyzer
* *StaticBatchingAndHybridPackage* analyzer
* *Object.Instantiate* and *GameObject.AddComponent* issue types to built-in analyzer
* *String.Concat* issue type to built-in analyzer
* "experimental" allocation analyzer
* Performance critical context analysis
* Detect *MonoBehaviour.Update/LateUpdate/FixedUpdate* as perf critical contexts
* Detect *ComponentSystem/JobComponentSystem.OnUpdate* as perf critical contexts
* Critical-only UI filter
* Profiler markers
* Background analysis support

### Changed
* Optimized UI refresh performance and Assembly analysis

## [0.1.0-preview] - 2019-11-20

### Added
* Config asset support
* Mute/Unmute buttons
* Assembly column

### Changed
* Replaced Filters checkboxes with Popups

## [0.0.4-preview] - 2019-10-11

### Added
* Calling Method information
* Grouped view to Script issues

### Removed
* "Resolved" checkboxes

### Fixed
* Lots of bug fixes

## [0.0.3-preview] - 2019-09-04

### Added
* Progress bar
* Package whitelist
* Tooltips

### Fixed
* Unity 2017.x backwards compatibility

## [0.0.2-preview] - 2019-08-22

### First usable version

*Replaced placeholder database with real issues to look for*. This version also allows the user to Resolve issues.

## [0.0.1-preview] - 2019-07-23

### This is the first release of *Project Auditor*

*Proof of concept, mostly developed during Hackweek 2019*.
