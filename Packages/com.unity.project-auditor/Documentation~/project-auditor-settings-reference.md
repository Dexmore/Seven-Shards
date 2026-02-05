# Project Auditor settings reference

You can configure Project Auditor in the following places in the Unity Editor:

* [Preferences window](#preferences-reference): Set the preferences for your individual workspace.
* [Project settings](#project-settings-reference): Configure settings globally for the project.

## Preferences reference

To open the Project Auditor Preferences go to **Edit &gt; Preferences** (macOS: **Unity &gt; Settings**), and then select **Analysis &gt; Project Auditor**. The following preferences are available:

### Analysis

Configure how Project Auditor performs analysis of your project.

|**Setting**|**Description**|
|---|---|
|**Project Areas**| Select the [Project Areas](project-auditor-window-reference.md#project-area-views) of the project to be included in project analysis. By default, all areas are selected.|
|**Platform**|Specify the target platform for analysis from a list of all the currently supported platform modules included in your installed Unity Editor. By default, Project Auditor uses the platform that's targeted in the Build Settings window.|
|**Compilation Mode**|Specify which assemblies to compile for code analysis, and how to treat those compiled assemblies. The options are as follows:<br/><br/> - **Player** (default): Compile code for analysis for a non-development build for the specified target platform. Code inside `#if DEVELOPMENT_BUILD` is excluded from the analysis.<br/>- **Development**: Compile code for analysis for a development build for the specified target platform. Code inside `#if DEVELOPMENT_BUILD` is included in the analysis.<br/>- **Editor Play Mode**: Perform analysis on the assemblies which are used in Play mode. Project Auditor skips the compilation step for these assemblies because the Editor caches them, which speeds up analysis.<br/>- **Editor**: Perform analysis only on Editor code assemblies. Select this option to analyze custom Editor code, including the contents of packages.|
|**Use Roslyn Analyzers**|Enable Roslyn analyzers, including one that reports issues impacting domain reload times. Roslyn analyzers can slow down the code compilation step of the analysis process, so this option can be used to disable Roslyn support if you don't need it.|
|**Log timing information**|Enable to log information to the Console about how long each analyzer took to run.|
|**Analyze Package Contents For Issues**|Enable to include issues with packages in the Project Auditor report.|

### Build

Options to configure how Project Auditor interacts with the build process in your project.


|**Setting**|**Description**|
|---|---|
|**Log number of issues after Build**|Enable to log the number of issues that Project Auditor finds to the Console.|
|**Log Issues as Errors**|Only available if the **Log number of issues after Build** setting is enabled. Enable to log any issues that Project Auditor finds as errors.|

### Report

Configure how Project Auditor handles the reports it generates.

|**Setting**|**Description**|
|---|---|
|**Prettify saved .projectauditor files**|Enable to format project reports to make them easier to read. Disabling this setting reduces the file size of reports.|

## Project settings reference

To open the Project Settings window, go to **Edit &gt; Project Settings &gt; Project Auditor**.

You can set the threshold at which Project Auditor creates a warning for each of the items in this window, and for each platform available in your project.

|**Setting**|**Description**|
|---|---|
|**Empty Sprite Atlas use threshold**|Warns if the percentage of unused pixels in a Sprite Atlas is greater than this value. Set to 100 to disable Sprite Atlas analysis.|
|**Maximum non-streaming Texture size**|Creates an issue if a Texture is larger than this value in pixels.|
|**Streaming Audio Clip Threshold**|Creates an issue if a streaming Audio Clip is larger than this value in bytes.|
|**Decompressed Audio Clip Threshold**|Creates an issue if a decompressed Audio Clip is larger than this value in bytes.|
|**Compressed Audio Clip Threshold**|Creates an issue if a compressed Audio Clip is larger than this value in bytes.|
|**Load in Background Audio Clip Threshold**|Creates an issue if a load in background Audio Clip is larger than this value in bytes.|
|**StreamingAssets folder size limit**|Creates an issue if the StreamingAssets folder is larger than this value in MB.|

## Additional resources

* [Programming with Project Auditor](project-auditor-programming.md)
* [Analyze your project](analyze-project.md)
* [Project Auditor window reference overview](project-auditor-window-reference.md)