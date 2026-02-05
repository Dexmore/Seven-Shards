# Project issues introduction

When you create a report, Project Auditor gives a high level indication of the types of issues that your project has. Those issues are divided into [project areas](project-auditor-window-reference.md#project-area-views) which you can get detailed information and suggestions for how to fix common issues.

## Managed memory issues

Because Unity uses a [managed memory system](xref:um-performance-managed-memory-introduction), to optimize your code for performance, you must avoid situations where your application triggers the garbage collector a lot. The information in the [Optimizing your code for managed memory](xref:um-performance-garbage-collection-best-practices) section of the Unity user manual covers troubleshooting common issues such as [avoiding boxing](xref:um-performance-reference-types) which causes high overhead.

## Managing assets

It's best practice to avoid using the Resources folder to store assets. You can avoid a lot of performance issues if you don't use this way of storing assets. For more information, refer to the Learn Tutorial on [Assets, Resources, and AssetBundles](https://learn.unity.com/tutorial/assets-resources-and-assetbundles#5c7f8528edbc2a002053b5a6).

## Additional resources

* [Domain reloading issues](domain-reloading-issues.md)