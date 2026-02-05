# Compare issues and insights

You can save the [`Report`](xref:Unity.ProjectAuditor.Editor.Report) object produced by Project Auditor's analysis as a .projectauditor file, or examine it via its API. `Report` contains a `SessionInfo` object with information about the analysis session, including a copy of the `AnalysisParams` which Project Auditor used to configure the analysis. It also provides several methods to access the report's list of discovered `ReportItem` instances. Each `ReportItem` represents a single issue or insight: all the data for a single item in one of the tables that are available in the [Project Auditor window](project-auditor-window-reference.md).

The difference between issues and insights are that issues have a valid `DescriptorId` field and insights don't. There are a couple of ways to check this:

```c#
bool isIssue = reportItem.Id.IsValid();

// A slightly more readable alternative...
bool isIssue = reportItem.IsIssue();
```

If you have a valid `DescriptorId`, you can use it to get the corresponding `Descriptor`, which is the object that described a type of issue, including its details and recommendation strings.

```c#
var descriptor = reportItem.Id.GetDescriptor();
Debug.Log($"Id: {reportItem.Id.ToString()}");
Debug.Log($"Description: {descriptor.Description}");
Debug.Log($"Recommendation: {descriptor.Recommendation}");
```

Project Auditor also provides an API for creating custom analyzers tailored to the needs of your project. For more information, refer to [Create custom analyzers](custom-analyzers.md).

## Additional resources

* [Run Project Auditor from the command line](run-from-command-line.md)
* [Create custom analyzers](custom-analyzers.md)
* [`Report` API documentation](xref:Unity.ProjectAuditor.Editor.Report)