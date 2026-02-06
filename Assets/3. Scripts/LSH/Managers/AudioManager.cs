using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    public enum SFXType { PunchHit, HookHit }

    [System.Serializable]
    public class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
        [Header("3D Settings")] [Range(0f, 1f)] public float spatialBlend = 1f;
        public float minDistance = 1f;
        public float maxDistance = 20f;
        [Header("Anti-Spam")] public float cooldown = 0.03f;
        [Tooltip("이 타입이 동시에 울릴 수 있는 최대 수(0이면 전역 제한만 적용)")]
        public int maxVoicesForType = 4;
    }

    [Header("Database")] [SerializeField] private SFXEntry[] sfxEntries;
    [Header("Pooling")] [SerializeField] private int preloadSources = 16;
    [SerializeField] private int maxSources = 48;
    [Header("Global")] [SerializeField] private float masterSfxVolume = 1f;
    [SerializeField] private int maxVoicesGlobal = 24;

    // 배열 기반 데이터
    private SFXEntry[] entriesByType;
    private float[] lastPlayTimes;
    private int[] activeVoicesByType;

    private struct ActiveSourceEntry { public AudioSource src; public int typeIdx; }
    private readonly List<ActiveSourceEntry> activeSources = new();
    private readonly Queue<AudioSource> pool = new();
    private int totalSources;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        BuildDatabase();
        Preload();
    }

    void BuildDatabase()
    {
        int typeCount = System.Enum.GetValues(typeof(SFXType)).Length;
        entriesByType     = new SFXEntry[typeCount];
        lastPlayTimes     = new float[typeCount];
        activeVoicesByType = new int[typeCount];
        for (int i = 0; i < typeCount; i++)
        {
            entriesByType[i] = null;
            lastPlayTimes[i] = -999f;
            activeVoicesByType[i] = 0;
        }
        if (sfxEntries == null) return;
        foreach (var e in sfxEntries)
        {
            if (e == null || e.clip == null) continue;
            int idx = (int)e.type;
            entriesByType[idx] = e;
            lastPlayTimes[idx] = -999f;
            activeVoicesByType[idx] = 0;
        }
    }

    void Preload()
    {
        for (int i = 0; i < preloadSources; i++)
            CreateAndEnqueue();
    }

    AudioSource CreateAndEnqueue()
    {
        if (maxSources > 0 && totalSources >= maxSources) return null;
        var go = new GameObject("SFXSource");
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        pool.Enqueue(src);
        totalSources++;
        return src;
    }

    public void PlaySFX(SFXType type, Vector3 pos)
    {
        if (entriesByType == null) return;
        int idx = (int)type;
        if (idx < 0 || idx >= entriesByType.Length) return;
        var e = entriesByType[idx];
        if (e == null || e.clip == null) return;
        float now = Time.unscaledTime;
        float last = lastPlayTimes[idx];
        if ((now - last) < e.cooldown) return;
        CleanupFinishedSources();
        if (activeSources.Count >= maxVoicesGlobal) return;
        int activeForType = activeVoicesByType[idx];
        if (e.maxVoicesForType > 0 && activeForType >= e.maxVoicesForType) return;
        var src = GetSource();
        if (!src) return;
        lastPlayTimes[idx] = now;
        activeVoicesByType[idx] = activeForType + 1;
        src.transform.position = pos;
        src.clip = e.clip;
        src.volume = e.volume * masterSfxVolume;
        src.pitch = Mathf.Clamp(e.pitch, 0.01f, 3f);
        src.spatialBlend = e.spatialBlend;
        src.minDistance = e.minDistance;
        src.maxDistance = e.maxDistance;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.Play();
        activeSources.Add(new ActiveSourceEntry { src = src, typeIdx = idx });
        src.gameObject.hideFlags = HideFlags.None;
        src.gameObject.name = $"SFXSource_{type}";
        src.gameObject.SetActive(true);
    }

    AudioSource GetSource()
    {
        if (pool.Count > 0) return pool.Dequeue();
        return CreateAndEnqueue() != null ? pool.Dequeue() : null;
    }

    void CleanupFinishedSources()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var entry = activeSources[i];
            var s = entry.src;
            if (!s)
            {
                activeSources.RemoveAt(i);
                continue;
            }
            if (s.clip == null)
            {
                activeSources.RemoveAt(i);
                pool.Enqueue(s);
                continue;
            }
            bool finished = !s.isPlaying && (s.timeSamples == 0 || s.time >= Mathf.Max(0.01f, s.clip.length - 0.02f));
            if (!finished) continue;
            activeSources.RemoveAt(i);
            int idx = entry.typeIdx;
            if (idx >= 0 && idx < activeVoicesByType.Length)
            {
                int c = activeVoicesByType[idx];
                activeVoicesByType[idx] = Mathf.Max(0, c - 1);
            }
            s.clip = null;
            pool.Enqueue(s);
        }
    }

    void LateUpdate() => CleanupFinishedSources();
    public void SetMasterSfxVolume(float v) => masterSfxVolume = Mathf.Clamp01(v);
}