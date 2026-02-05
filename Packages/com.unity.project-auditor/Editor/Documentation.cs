namespace Unity.ProjectAuditor.Editor
{
    static class Documentation
    {
        internal const string baseURL = "https://docs.unity3d.com/Packages/com.unity.project-auditor@";
        internal const string subURL = "/manual/";
        internal const string endURL = ".html";

        internal static string GetPageUrl(string pageName)
        {
            return baseURL + ProjectAuditorPackage.VersionShort + subURL + pageName + endURL;
        }
    }
}
