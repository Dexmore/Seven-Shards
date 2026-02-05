# Run Project Auditor from the command line

You can execute Project Auditor's analysis from command line by launching the Unity Editor [in batch mode](xref:um-cli-batchmode-coroutines). This requires an Editor script that creates a `ProjectAuditor` instance and runs the analysis. The following is an example of such a script:

```c#
using Unity.ProjectAuditor.Editor;
using UnityEngine;

public static class ProjectAuditorCI
{
    public static void AuditAndExport()
    {
        string reportPath = "C:/Dev/MyProject/my-report.projectauditor";
        var projectAuditor = new ProjectAuditor();
        var report = projectAuditor.Audit();
        report.Save(reportPath);

        var codeIssues = report.FindByCategory(IssueCategory.Code);
        Debug.Log($"Project Auditor found {codeIssues.Count} code issues");
    }
}
```

This can be useful for performing automated analysis in a CI/CD environment. You can use the following command line call to launch Unity, run the script, and then exit:

```
path/to/unity/executable -batchmode -quit -projectPath path/to/your/project -executeMethod ProjectAuditorCI.AuditAndExport
```

For more information on how to run the Unity Editor via command line, refer to [Unity Editor command line arguments](xref:um-editor-command-line-arguments).

## Configure Project Auditor

The [`ProjectAuditor`](xref:Unity.ProjectAuditor.Editor.ProjectAuditor) class provides the interface for running project analysis, via its `Audit` and `AuditAsync` methods, which return a `Report` object. In the previous code example, `Audit` doesn't take any configuration parameters, which means it creates and uses an `AnalysisParams` object with default values. This results in a full analysis of the project, targeting the currently-selected build platform and performing a Player code build.

To configure analysis differently, or to specify callbacks for some stages in the analysis process, create an`AnalysisParams` object and configure it as required, then pass it as a parameter into a `ProjectAuditor` Audit method.

For example, the following code performs asynchronous analysis of a project's code (ignoring other areas such as Assets and Project Settings) on the default player assembly, compiled in debug mode for Android devices. Callbacks are declared to count and log the number of issues.

```c#
int foundIssues = 0;
var analysisParams = new AnalysisParams
{
  Categories = new[] { IssueCategory.Code },
  AssemblyNames = new[] { "Assembly-CSharp" },
  Platform = BuildTarget.Android,
  CodeOptimization = CodeOptimization.Debug,
  OnIncomingIssues = issues => { foundIssues += issues.Count(); },
  OnCompleted = (report) =>
  {
    Debug.Log($"Found {foundIssues} code issues");
    report.Save(reportPath);
  }
};

projectAuditor.AuditAsync(analysisParams);
```

## Additional resources

* [Compare issues and insights](compare-issues.md)
* [Create custom analyzers](custom-analyzers.md)
* [`ProjectAuditor` API documentation](xref:Unity.ProjectAuditor.Editor.ProjectAuditor)