using UnityEngine;
using Unity.Cinemachine;

public sealed class HitFeedbackManager : MonoBehaviour
{
    public static HitFeedbackManager I { get; private set; }
    public enum HitKind { Punch, Hook }
    [Header("HitStop")]
    [SerializeField, Range(0.01f, 1f)] float hitstopScale = 0.1f;
    [SerializeField, Range(0.01f, 0.2f)] float hitstopDuration = 0.06f;
    [SerializeField, Range(0f, 0.2f)] float hitstopCooldown = 0.03f;
    [Header("Cinemachine Impulse (Camera Shake)")]
    [SerializeField] CinemachineImpulseSource punchImpulse;
    [SerializeField] CinemachineImpulseSource hookImpulse;
    [SerializeField] Vector3 impulseDir = new Vector3(0, 0, -1);
    [SerializeField, Range(0f, 0.2f)] float shakeCooldown = 0.02f;
    float defaultFixedDelta;
    float stopUntilUnscaled;
    float lastStopUnscaled;
    float lastShakeUnscaled;
    Coroutine hitStopCoroutine;
    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        defaultFixedDelta = Time.fixedDeltaTime;
    }
    public void OnHit(HitKind kind, Vector3 hitWorldPos)
    {
        TryHitStop();
        TryShake(kind, hitWorldPos);
    }
    void TryHitStop()
    {
        float now = Time.unscaledTime;
        if (now - lastStopUnscaled < hitstopCooldown) return;
        lastStopUnscaled = now;
        float targetUntil = now + hitstopDuration;
        if (targetUntil > stopUntilUnscaled) stopUntilUnscaled = targetUntil;
        Time.timeScale = hitstopScale;
        Time.fixedDeltaTime = defaultFixedDelta * hitstopScale;
        // 코루틴이 없으면 시작. 이미 돌고 있다면 stopUntilUnscaled만 갱신
        if (hitStopCoroutine == null)
            hitStopCoroutine = StartCoroutine(HitStopRoutine());
    }
    void TryShake(HitKind kind, Vector3 hitWorldPos)
    {
        float now = Time.unscaledTime;
        if (now - lastShakeUnscaled < shakeCooldown) return;
        lastShakeUnscaled = now;
        var src = kind == HitKind.Punch ? punchImpulse : hookImpulse;
        if (!src) return;
        src.transform.position = hitWorldPos;
        Vector3 dir = impulseDir.sqrMagnitude > 0.001f ? impulseDir.normalized : Vector3.back;
        src.GenerateImpulse(dir);
    }
    System.Collections.IEnumerator HitStopRoutine()
    {
        while (Time.unscaledTime < stopUntilUnscaled)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        hitStopCoroutine = null;
    }
#if UNITY_EDITOR
    void OnDisable()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
    }
#endif
}