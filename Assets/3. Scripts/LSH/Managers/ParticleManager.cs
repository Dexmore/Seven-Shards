using System.Collections;
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
        public int preload = 10;
    }

    [Header("FX Database")]
    [SerializeField] FXEntry[] fxEntries;

    readonly Dictionary<FXType, Queue<ParticleSystem>> pools = new();
    readonly Dictionary<FXType, ParticleSystem> prefabMap = new();

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    void Initialize()
    {
        foreach (var entry in fxEntries)
        {
            prefabMap[entry.type] = entry.prefab;

            var q = new Queue<ParticleSystem>(entry.preload);
            pools[entry.type] = q;

            for (int i = 0; i < entry.preload; i++)
                Create(entry.type);
        }
    }

    void Create(FXType type)
    {
        var ps = Instantiate(prefabMap[type], transform);
        ps.gameObject.SetActive(false);
        pools[type].Enqueue(ps);
    }

    public void Play(FXType type, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(type, out var q))
            return;

        if (q.Count == 0)
            Create(type);

        var ps = q.Dequeue();
        ps.transform.SetPositionAndRotation(pos, rot);
        ps.gameObject.SetActive(true);
        ps.Play();

        StartCoroutine(ReturnAfter(type, ps, ps.main.duration));
    }

    IEnumerator ReturnAfter(FXType type, ParticleSystem ps, float t)
    {
        yield return new WaitForSeconds(t);
        ps.gameObject.SetActive(false);
        pools[type].Enqueue(ps);
    }
}
