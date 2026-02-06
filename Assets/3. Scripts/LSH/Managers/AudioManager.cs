using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    public enum SFXType
    {
        PunchHit,
        HookHit
    }

    [System.Serializable]
    public class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;

        [Header("3D Settings")]
        [Range(0f, 1f)] public float spatialBlend = 1f; // 1 = 3D, 0 = 2D
        public float minDistance = 1f;
        public float maxDistance = 20f;

        [Header("Anti-Spam")]
        [Tooltip("같은 타입 SFX 최소 간격(초)")]
        public float cooldown = 0.03f;

        [Tooltip("이 타입이 동시에 울릴 수 있는 최대 수(0이면 전역 제한만 적용)")]
        public int maxVoicesForType = 4;
    }

    [Header("Database")]
    [SerializeField] private SFXEntry[] sfxEntries;

    [Header("Pooling")]
    [SerializeField] private int preloadSources = 16;
    [SerializeField] private int maxSources = 48;

    [Header("Global")]
    [SerializeField] private float masterSfxVolume = 1f;
    [SerializeField] private int maxVoicesGlobal = 24; // 동시에 울리는 소리 상한

    private readonly Dictionary<SFXType, SFXEntry> map = new();
    private readonly Dictionary<SFXType, float> lastPlayTime = new();

    private readonly Dictionary<SFXType, int> activeVoicesByType = new();
    private readonly List<AudioSource> activeSources = new();

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
        map.Clear();
        lastPlayTime.Clear();
        activeVoicesByType.Clear();

        if (sfxEntries == null) return;

        foreach (var e in sfxEntries)
        {
            if (e == null || e.clip == null) continue;
            map[e.type] = e;
            lastPlayTime[e.type] = -999f;
            activeVoicesByType[e.type] = 0;
        }
    }

    void Preload()
    {
        for (int i = 0; i < preloadSources; i++)
            CreateAndEnqueue();
    }

    AudioSource CreateAndEnqueue()
    {
        if (maxSources > 0 && totalSources >= maxSources)
            return null;

        var go = new GameObject("SFXSource");
        go.transform.SetParent(transform, false);

        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;

        pool.Enqueue(src);
        totalSources++;
        return src;
    }

    /// <summary>
    /// 3D 위치에서 SFX 재생
    /// </summary>
    public void PlaySFX(SFXType type, Vector3 pos)
    {
        if (!map.TryGetValue(type, out var e) || e.clip == null) return;

        // 쿨다운(도배 방지)
        float now = Time.unscaledTime;
        if (lastPlayTime.TryGetValue(type, out var last) && (now - last) < e.cooldown)
            return;

        // 전역 동시 재생 제한
        CleanupFinishedSources();
        if (activeSources.Count >= maxVoicesGlobal)
            return;

        // 타입별 동시 재생 제한
        // 현재 타입의 활성 보이스 수
        int activeForType = 0;
        activeVoicesByType.TryGetValue(type, out activeForType);

        // 타입별 동시 재생 제한
        if (e.maxVoicesForType > 0 && activeForType >= e.maxVoicesForType)
            return;

        var src = GetSource();
        if (!src) return;

        lastPlayTime[type] = now;
        activeVoicesByType[type] = activeForType + 1;


        // 세팅
        src.transform.position = pos;
        src.clip = e.clip;
        src.volume = e.volume * masterSfxVolume;
        src.pitch = Mathf.Clamp(e.pitch, 0.01f, 3f);
        src.spatialBlend = e.spatialBlend;
        src.minDistance = e.minDistance;
        src.maxDistance = e.maxDistance;
        src.rolloffMode = AudioRolloffMode.Logarithmic;

        src.Play();
        activeSources.Add(src);

        // 이 src가 어떤 타입인지 추적(끝나면 카운트 감소)
        src.gameObject.hideFlags = HideFlags.None;
        src.gameObject.name = $"SFXSource_{type}";
        src.gameObject.SetActive(true);

        // 타입 카운트 감소를 위해 메타 저장
        var meta = src.GetComponent<SFXMeta>();
        if (!meta) meta = src.gameObject.AddComponent<SFXMeta>();
        meta.type = type;
        meta.manager = this;
    }

    AudioSource GetSource()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        // 풀 고갈 시 maxSources 내에서만 생성
        return CreateAndEnqueue() != null ? pool.Dequeue() : null;
    }

    void CleanupFinishedSources()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var s = activeSources[i];
            if (!s) continue;

            // clip이 없으면 회수
            if (s.clip == null)
            {
                activeSources.RemoveAt(i);
                pool.Enqueue(s);
                continue;
            }

            // 실제로 끝났는지 확인 (더 안전)
            bool finished =
                !s.isPlaying &&
                (s.timeSamples == 0 || s.time >= Mathf.Max(0.01f, s.clip.length - 0.02f));

            if (!finished) continue;

            // 여기서 회수
            activeSources.RemoveAt(i);

            var meta = s.GetComponent<SFXMeta>();
            if (meta && meta.manager == this)
            {
                if (activeVoicesByType.TryGetValue(meta.type, out var c))
                    activeVoicesByType[meta.type] = Mathf.Max(0, c - 1);
            }

            s.clip = null;
            pool.Enqueue(s);

        }
    }

    void LateUpdate()
    {
        // 매 프레임 끝난 소스 회수(가벼움: active만 체크)
        CleanupFinishedSources();
    }

    // (선택) 마스터 볼륨 조절
    public void SetMasterSfxVolume(float v) => masterSfxVolume = Mathf.Clamp01(v);

    // 내부 메타
    private sealed class SFXMeta : MonoBehaviour
    {
        public SFXType type;
        public AudioManager manager;
    }
}