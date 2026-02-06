using System.Collections.Generic;
using UnityEngine;

public sealed class ParticleManager : MonoBehaviour
{
    public static ParticleManager I { get; private set; }

    public enum FXType
    {
        Hit_Punch,
        Hit_Hook
    }

    [System.Serializable]
    public class FXEntry
    {
        public FXType type;
        public ParticleSystem prefab;
        [Min(0)] public int preload = 10;
        [Min(0), Tooltip("풀 최대 개수. 0이면 무제한(비추천)")] public int maxCount = 30;
    }

    [Header("FX Database")]
    [SerializeField] private FXEntry[] fxEntries;
    [Header("Runtime")]
    [SerializeField, Tooltip("활성 파티클 종료 체크 주기(초). 0.1이면 1초에 10번만 체크")] private float pollInterval = 0.10f;
    [SerializeField, Tooltip("이 시간 지나도 살아있으면 강제 종료 후 회수(무한생존/렉 방지)")] private float hardTimeout = 5f;

    // 배열 기반 풀/프리팹/카운트
    private Queue<ParticleSystem>[] pools;
    private ParticleSystem[] prefabs;
    private int[] totalCounts;
    private int[] maxCounts;

    // 활성 파티클 정보
    private struct ActiveEntry
    {
        public ParticleSystem ps;
        public float startTime;
        public int typeIdx;
    }
    private List<ActiveEntry> activeList;

    float nextPollTime;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Build();
    }

    void Build()
    {
        int typeCount = System.Enum.GetValues(typeof(FXType)).Length;
        pools      = new Queue<ParticleSystem>[typeCount];
        prefabs    = new ParticleSystem[typeCount];
        totalCounts= new int[typeCount];
        maxCounts  = new int[typeCount];
        activeList = new List<ActiveEntry>(256);

        if (fxEntries == null) return;
        foreach (var e in fxEntries)
        {
            if (e == null || e.prefab == null)
            {
                Debug.LogWarning("[ParticleManager] FXEntry prefab is null.");
                continue;
            }
            int idx = (int)e.type;
            prefabs[idx] = e.prefab;
            pools[idx] = new Queue<ParticleSystem>(Mathf.Max(1, e.preload));
            totalCounts[idx] = 0;
            maxCounts[idx] = e.maxCount;
            for (int i = 0; i < e.preload; i++)
                CreateAndEnqueue(idx);
        }
    }

    bool CreateAndEnqueue(int typeIdx)
    {
        if (prefabs == null || typeIdx < 0 || typeIdx >= prefabs.Length) return false;
        var pf = prefabs[typeIdx];
        if (!pf) return false;
        int max   = maxCounts[typeIdx];
        int total = totalCounts[typeIdx];
        if (max > 0 && total >= max) return false;
        var ps = Instantiate(pf, transform);
        ps.gameObject.SetActive(false);
        totalCounts[typeIdx] = total + 1;
        pools[typeIdx].Enqueue(ps);
        return true;
    }

    public ParticleSystem Play(FXType type, Vector3 pos, Quaternion rot)
    {
        if (pools == null) return null;
        int idx = (int)type;
        if (idx < 0 || idx >= pools.Length || pools[idx] == null)
        {
            Debug.LogWarning($"[ParticleManager] Pool not registered: {type}");
            return null;
        }
        var q = pools[idx];
        if (q.Count == 0)
        {
            if (!CreateAndEnqueue(idx) || q.Count == 0) return null;
        }
        var ps = q.Dequeue();
        ps.transform.SetPositionAndRotation(pos, rot);
        ps.gameObject.SetActive(true);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
        if (activeList == null) activeList = new List<ActiveEntry>(64);
        activeList.Add(new ActiveEntry { ps = ps, startTime = Time.time, typeIdx = idx });
        return ps;
    }

    void Update()
    {
        if (activeList == null || activeList.Count == 0) return;
        if (Time.time < nextPollTime) return;
        nextPollTime = Time.time + pollInterval;
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            var entry = activeList[i];
            var ps = entry.ps;
            if (!ps)
            {
                activeList.RemoveAt(i);
                continue;
            }
            if (hardTimeout > 0f && (Time.time - entry.startTime) >= hardTimeout)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Return(i);
                continue;
            }
            if (!ps.IsAlive(true)) Return(i);
        }
    }

    void Return(int activeIndex)
    {
        if (activeList == null || activeIndex < 0 || activeIndex >= activeList.Count) return;
        var entry = activeList[activeIndex];
        activeList.RemoveAt(activeIndex);
        var ps = entry.ps;
        int typeIdx = entry.typeIdx;
        if (!ps || pools == null || typeIdx < 0 || typeIdx >= pools.Length) return;
        ps.transform.SetParent(transform, false);
        ps.gameObject.SetActive(false);
        pools[typeIdx].Enqueue(ps);
    }

    public void StopAllAndReturn()
    {
        if (activeList == null) return;
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            var entry = activeList[i];
            var ps = entry.ps;
            if (!ps) continue;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Return(i);
        }
    }
}