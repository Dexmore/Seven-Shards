using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.ProjectAuditor.Editor.Core;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Unity.ProjectAuditor.Editor
{
    /// <summary>
    /// Contains information about the session in which a <see cref="Report"/> was created.
    /// </summary>
    [Serializable]
    public class SessionInfo : AnalysisParams
    {
        /// <summary>
        /// Default Constructor. For serialization purposes only.
        /// </summary>
        public SessionInfo() : base(false) {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serializedParams">AnalysisParams object which was passed to ProjectAuditor to create the Report</param>
        public SessionInfo(AnalysisParams serializedParams)
            : base(serializedParams)
        {}

        /// <summary>
        /// The version number of the Project Auditor package which was used.
        /// </summary>
        public string ProjectAuditorVersion;

        /// <summary>
        /// The version of Unity which was used.
        /// </summary>
        public string UnityVersion;

        /// <summary>
        /// The Company Name string in the project's Project Settings.
        /// </summary>
        public string CompanyName;

        /// <summary>
        /// The `Application.cloudProjectId` identifier for the project.
        /// </summary>
        public string ProjectId;

        /// <summary>
        /// The Product Name string in the project's Project Settings.
        /// </summary>
        public string ProjectName;

        /// <summary>
        /// The Product Version string in the project's Project Settings.
        /// </summary>
        public string ProjectRevision;

        /// <summary>
        /// The date and time at which the Report was created.
        /// </summary>
        public string DateTime;

        /// <summary>
        /// The `SystemInfo.deviceName` identifier for the device on which the Unity Editor was running.
        /// </summary>
        public string HostName;

        /// <summary>
        /// The `SystemInfo.operatingSystem` identifier for the operating system on which the Unity Editor was running.
        /// </summary>
        public string HostPlatform;

        /// <summary>
        /// True if the "Use Roslyn Analyzers" checkbox was ticked in Preferences > Project Auditor.
        /// </summary>
        public bool UseRoslynAnalyzers;
    }

    /// <summary>
    /// Report contains a list of all issues found by ProjectAuditor.
    /// </summary>
    [Serializable]
    public sealed class Report
    {
        const string k_CurrentVersion = "0.2";
        const string k_SaveFileHeader = "PROJECT_AUDITOR_REPORT";

        [JsonProperty("version")][SerializeField]
        string m_Version = k_CurrentVersion;

        /// <summary>
        /// File format version of the Report (read-only).
        /// </summary>
        [JsonIgnore]
        public string Version
        {
            get => m_Version;
            internal set => m_Version = value;
        }

        // stephenm TODO: ModuleInfo serializes to JSON but isn't accessible in any meaningful way if a script just has a Report object it wants to query. Figure out some API for this?
        // Keeping this internal for now. Exposing this means exposing IssueLayout, which means exposing PropertyDefinition, which to be useful means exposing every enum that can
        // be passed to PropertyTypeUtil.FromCustom() (basically one per view). I'd love to find a more elegant way to do this.
        [Serializable]
        class ModuleInfo
        {
            // stephenm TODO: Comment (for all these fields... Assuming we do what the above comment says and expose this via an API of some sort)
            public string name;

            // this is used by HasCategory
            public IssueCategory[] categories;
            public IssueLayout[] layouts;

            public string startTime;
            public string endTime;

            public AnalysisResult result;
        }

        /// <summary>
        /// Contains information about the session in which this Report was created.
        /// </summary>
        [JsonProperty("sessionInfo")][SerializeField]
        public SessionInfo SessionInfo;

        /// <summary>
        /// A name to display along with the Report, configurable by the user.
        /// </summary>
        public string DisplayName;

        [JsonProperty("needsSaving")][SerializeField]
        internal bool NeedsSaving;

        [JsonProperty("moduleMetadata")][SerializeField]
        List<ModuleInfo> m_ModuleInfos = new List<ModuleInfo>();

        [SerializeField]
        DescriptorLibrary m_DescriptorLibrary = new DescriptorLibrary();

        [JsonIgnore][SerializeField]
        List<ReportItem> m_Issues = new List<ReportItem>();

        static Mutex s_Mutex = new Mutex();

        [JsonProperty("insights")]
        internal ReportItem[] Insights
        {
            get
            {
                return m_Issues.Where(i => !i.IsIssue()).ToArray();
            }
            set => m_Issues.AddRange(value);
        }

        [JsonProperty("issues")]
        internal ReportItem[] UnfixedIssues
        {
            get
            {
                return m_Issues.Where(i => i.IsIssue() && !i.WasFixed).ToArray();
            }
            set => m_Issues.AddRange(value);
        }

        [JsonProperty("descriptors")]
        internal List<Descriptor> Descriptors
        {
            get
            {
                m_DescriptorLibrary.OnBeforeSerialize();
                return m_DescriptorLibrary.m_SerializedDescriptors;
            }
            set
            {
                m_DescriptorLibrary.m_SerializedDescriptors = value;
                m_DescriptorLibrary.OnAfterDeserialize();
            }
        }

        /// <summary>
        /// The total number of ProjectIssues included in this report.
        /// </summary>
        [JsonIgnore]
        public int NumTotalIssues => m_Issues.Count;

        // for serialization purposes only
        internal Report()
        {}

        // for internal use only
        internal Report(AnalysisParams analysisParams)
        {
            SessionInfo = new SessionInfo(analysisParams)
            {
                ProjectAuditorVersion = ProjectAuditorPackage.Version,

                ProjectId = PlayerSettings.productGUID.ToString(),
                ProjectName = Application.productName,
                ProjectRevision = Application.version,
                CompanyName = Application.companyName,
                UnityVersion = Application.unityVersion,

                DateTime = Utils.Json.SerializeDateTime(DateTime.Now),
                // Windows' SystemInfo.deviceName implementation is currently the NetBIOS name (limited to 15 characters), so use this instead.
                HostName = System.Net.Dns.GetHostName(),
                // It's not 2016 any more, but too many systems depend on operatingSystem thinking it is, so update mac's naming here
                HostPlatform = SystemInfo.operatingSystem.Replace("Mac OS X", "macOS"),

                UseRoslynAnalyzers = UserPreferences.UseRoslynAnalyzers
            };
        }

        /// <summary>
        /// Checks whether the Report includes analysis for a given IssueCategory.
        /// </summary>
        /// <param name="category">The IssuesCategory to check</param>
        /// <returns>True if ProjectAuditor ran one or more Modules that reports issues of the specified IssueCategory. Otherwise, returns false.</returns>
        public bool HasCategory(IssueCategory category)
        {
            if (category == IssueCategory.Metadata)
                return true;

            foreach (ModuleInfo moduleInfo in m_ModuleInfos)
                foreach (IssueCategory moduleCategory in moduleInfo.categories)
                    if (moduleCategory == category)
                        return true;

            return false;
        }

        /// <summary>
        /// Gets a read-only collection of all of the ProjectIssues included in the report.
        /// </summary>
        /// <returns>All the issues in the report</returns>
        public IReadOnlyCollection<ReportItem> GetAllIssues()
        {
            s_Mutex.WaitOne();
            var result = m_Issues.ToArray();
            s_Mutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// Get total number of issues for a specific IssueCategory.
        /// </summary>
        /// <param name="category"> Desired IssueCategory</param>
        /// <returns> Number of project issues</returns>
        public int GetNumIssues(IssueCategory category)
        {
            s_Mutex.WaitOne();
            var result = m_Issues.Count(i => i.Category == category);
            s_Mutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// After we load from json the layouts in the ModuleInfo are invalid because they aren't serialized out.  This
        /// function runs through them and re-initializes them as they were on an initial scan.
        /// </summary>
        /// <param name="modules">An array of all the Module classes that are used to get the SupportedLayouts.</param>
        internal void PostSerializeLayoutUpdate(Module[] modules)
        {
            foreach (ModuleInfo moduleInfo in m_ModuleInfos)
            {
                if (moduleInfo.layouts == null)
                {
                    foreach (Module module in modules)
                    {
                        if (module.Name == moduleInfo.name || (moduleInfo.name == "AudioClips" && module.Name == "Audio Clips"))
                        {
                            moduleInfo.layouts = module.SupportedLayouts.ToArray();
                        }
                    }
                }
            }
        }

        internal IssueLayout GetLayout(IssueCategory category)
        {
            foreach (ModuleInfo m in m_ModuleInfos)
            {
                if (m.layouts == null)
                    continue;

                foreach (IssueLayout l in m.layouts)
                {
                    if (l.Category == category)
                        return l;
                }
            }

            return null;
        }

        /// <summary>
        /// find all issues for a specific IssueCategory.
        /// </summary>
        /// <param name="category"> Desired IssueCategory</param>
        /// <returns> Array of project issues</returns>
        public IReadOnlyCollection<ReportItem> FindByCategory(IssueCategory category)
        {
            s_Mutex.WaitOne();
            var result = m_Issues.Where(i => i.Category == category).ToArray();
            s_Mutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// Find all Issues that match a specific ID.
        /// </summary>
        /// <param name="id"> Desired Descriptor ID</param>
        /// <returns> Array of project issues</returns>
        public IReadOnlyCollection<ReportItem> FindByDescriptorId(string id)
        {
            s_Mutex.WaitOne();
            var result = m_Issues.Where(i => i.Id.IsValid() && i.Id.Equals(id)).ToArray();
            s_Mutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// Clears all issues that match the specified IssueCategory from the report.
        /// </summary>
        /// <param name="Category">The IssueCategory of the issues to remove.</param>
        public void ClearIssues(IssueCategory Category)
        {
            s_Mutex.WaitOne();
            m_Issues.RemoveAll(issue => issue.Category == Category);
            foreach (var info in m_ModuleInfos)
            {
                var categories = info.categories.ToList();
                categories.RemoveAll(c => c == Category);
                info.categories = categories.ToArray();
            }
            m_ModuleInfos.RemoveAll(info => info.categories.Length == 0);
            s_Mutex.ReleaseMutex();
        }

        /// <summary>
        /// Check whether all issues in the report are valid.
        /// </summary>
        /// <returns>True is none of the issues in the report have a null description string. Otherwise returns false.</returns>
        public bool IsValid()
        {
            if (m_ModuleInfos.Count == 0)
                return false;

            foreach (IssueCategory ic in Enum.GetValues(typeof(IssueCategory)))
            {
                if (ic == IssueCategory.Metadata || ic == IssueCategory.SpriteAtlas)
                    continue;

                if (ic >= IssueCategory.FirstCustomCategory)
                    break;

                if (SessionInfo != null && SessionInfo.Categories != null && !SessionInfo.Categories.Contains(ic))
                    continue;

                IssueLayout il = GetLayout(ic);
                if (il == null)
                    return false;
            }

            return m_Issues.All(i => i.IsValid()) && m_ModuleInfos.All(m => m.result != AnalysisResult.Cancelled);
        }

        /// <summary>
        /// Is this Report for the currently loaded Project?
        /// </summary>
        /// <returns>True if this report is for the currently loaded Unity Project.</returns>
        public bool IsForCurrentProject()
        {
            return (SessionInfo.ProjectId == PlayerSettings.productGUID.ToString());
        }

        /// <summary>
        /// Save the Report as a JSON file.
        /// </summary>
        /// <param name="path">The file path at which to save the file</param>
        public void Save(string path)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine(k_SaveFileHeader);
            sw.WriteLine(k_CurrentVersion);
            string contents = JsonConvert.SerializeObject(this,
                UserPreferences.PrettifyJsonOutput ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new DescriptorJsonConverter() },
                    NullValueHandling = NullValueHandling.Ignore
                });

            File.WriteAllText(path, sw.ToString() + contents);
        }

        /// <summary>
        /// Load a Report from a JSON file at the specified path.
        /// </summary>
        /// <param name="path">File path of the report to load</param>
        /// <returns>A loaded Report object</returns>
        public static Report Load(string path)
        {
            string whole_file = File.ReadAllText(path);

            StringReader sr = new StringReader(whole_file);

            string header = sr.ReadLine();

            if (!header.Equals(k_SaveFileHeader))
                return null;

            string version = sr.ReadLine();
            if (!version.Equals(k_CurrentVersion))
                return null;

            return JsonConvert.DeserializeObject<Report>(sr.ReadToEnd(), new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
        }

        // Internal only: Data written by ProjectAuditor during analysis
        internal void RecordModuleInfo(Module module, DateTime startTime, DateTime endTime, AnalysisResult analysisResult)
        {
            var name = module.Name;
            var info = m_ModuleInfos.FirstOrDefault(m => m.name.Equals(name));
            if (info == null)
            {
                info = new ModuleInfo
                {
                    name = module.Name,
                    categories = module.Categories,
                    layouts = module.SupportedLayouts.ToArray(),
                };
                m_ModuleInfos.Add(info);
            }

            info.startTime = Utils.Json.SerializeDateTime(startTime);
            info.endTime = Utils.Json.SerializeDateTime(endTime);
            info.result = analysisResult;
        }

        // Internal only: Data written by ProjectAuditor during analysis
        internal void AddIssues(IEnumerable<ReportItem> issues)
        {
            s_Mutex.WaitOne();
            m_Issues.AddRange(issues);
            s_Mutex.ReleaseMutex();
        }
    }
}
