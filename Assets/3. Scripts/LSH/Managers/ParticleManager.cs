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

        [Tooltip("풀 최대 개수. 0이면 무제한(비추천)")]
        [Min(0)] public int maxCount = 30;
    }

    [Header("FX Database")]
    [SerializeField] private FXEntry[] fxEntries;

    [Header("Runtime")]
    [Tooltip("활성 파티클 종료 체크 주기(초). 0.1이면 1초에 10번만 체크")]
    [SerializeField] private float pollInterval = 0.10f;

    [Tooltip("이 시간 지나도 살아있으면 강제 종료 후 회수(무한생존/렉 방지)")]
    [SerializeField] private float hardTimeout = 5f;

    // 풀/프리팹/카운트
    private readonly Dictionary<FXType, Queue<ParticleSystem>> pool = new();
    private readonly Dictionary<FXType, ParticleSystem> prefab = new();
    private readonly Dictionary<FXType, int> totalCount = new();
    private readonly Dictionary<FXType, int> maxCount = new();

    // 활성 추적(코루틴 없이 회수)
    private struct Active
    {
        public FXType type;
        public float startTime;
    }

    private readonly Dictionary<ParticleSystem, Active> activeMap = new(256);
    private readonly List<ParticleSystem> activeList = new(256);

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
        pool.Clear();
        prefab.Clear();
        totalCount.Clear();
        maxCount.Clear();
        activeMap.Clear();
        activeList.Clear();

        if (fxEntries == null) return;

        foreach (var e in fxEntries)
        {
            if (e == null || e.prefab == null)
            {
                Debug.LogWarning("[ParticleManager] FXEntry prefab is null.");
                continue;
            }

            prefab[e.type] = e.prefab;
            pool[e.type] = new Queue<ParticleSystem>(Mathf.Max(1, e.preload));

            totalCount[e.type] = 0;
            maxCount[e.type] = e.maxCount;

            for (int i = 0; i < e.preload; i++)
                CreateAndEnqueue(e.type);
        }
    }

    bool CreateAndEnqueue(FXType type)
    {
        if (!prefab.TryGetValue(type, out var pf) || pf == null) return false;

        int max = maxCount.TryGetValue(type, out var m) ? m : 0;
        int total = totalCount.TryGetValue(type, out var t) ? t : 0;

        // ✅ 무한 생성 방지
        if (max > 0 && total >= max) return false;

        var ps = Instantiate(pf, transform);
        ps.gameObject.SetActive(false);

        // ✅ 중요: 재사용 시 잔여 제거를 위해 Stop+Clear 가능한 상태 유지
        // (여기서 별도 stopAction/callback 필요 없음)

        totalCount[type] = total + 1;
        pool[type].Enqueue(ps);
        return true;
    }

    /// <summary>
    /// 반드시 풀에서 꺼내 재생. 풀 고갈이면 maxCount 내에서만 생성.
    /// </summary>
    public ParticleSystem Play(FXType type, Vector3 pos, Quaternion rot)
    {
        if (!pool.TryGetValue(type, out var q))
        {
            Debug.LogWarning($"[ParticleManager] Pool not registered: {type}");
            return null;
        }

        // ✅ 풀에 없으면: 생성(가능한 경우에만) 후 다시 시도
        if (q.Count == 0)
        {
            if (!CreateAndEnqueue(type) || q.Count == 0)
            {
                // maxCount 초과 등으로 생성 불가 → 그냥 스킵(렉 방지)
                return null;
            }
        }

        // ✅ 여기서 반드시 "프리로드로 만든 비활성 오브젝트"를 꺼내쓴다
        var ps = q.Dequeue();

        // 세팅
        ps.transform.SetPositionAndRotation(pos, rot);
        ps.gameObject.SetActive(true);

        // 잔여 제거 후 재생
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);

        // 활성 등록
        activeMap[ps] = new Active { type = type, startTime = Time.time };
        activeList.Add(ps);

        return ps;
    }

    void Update()
    {
        if (activeList.Count == 0) return;

        // ✅ 폴링은 pollInterval마다만 (코루틴 0)
        if (Time.time < nextPollTime) return;
        nextPollTime = Time.time + pollInterval;

        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            var ps = activeList[i];
            if (!ps)
            {
                activeList.RemoveAt(i);
                continue;
            }

            if (!activeMap.TryGetValue(ps, out var a))
            {
                activeList.RemoveAt(i);
                continue;
            }

            // ✅ 너무 오래 살아있으면 강제 종료 후 회수
            if (hardTimeout > 0f && (Time.time - a.startTime) >= hardTimeout)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Return(ps, a.type, i);
                continue;
            }

            // ✅ 자식 포함 완전 종료면 회수
            if (!ps.IsAlive(true))
            {
                Return(ps, a.type, i);
            }
        }
    }

    void Return(ParticleSystem ps, FXType type, int activeIndex)
    {
        activeMap.Remove(ps);
        activeList.RemoveAt(activeIndex);

        ps.transform.SetParent(transform, false);
        ps.gameObject.SetActive(false);

        pool[type].Enqueue(ps);
    }

    // 선택: 씬 전환/리셋 시 전체 회수
    public void StopAllAndReturn()
    {
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            var ps = activeList[i];
            if (!ps) continue;

            if (activeMap.TryGetValue(ps, out var a))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Return(ps, a.type, i);
            }
        }
    }
}