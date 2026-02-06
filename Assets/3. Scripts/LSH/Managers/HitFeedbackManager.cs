using UnityEngine;
using Unity.Cinemachine;

public sealed class HitFeedbackManager : MonoBehaviour
{
    public static HitFeedbackManager I { get; private set; }

    public enum HitKind { Punch, Hook }

    [Header("HitStop")]
    [Tooltip("히트스톱 시 Time.timeScale")]
    [SerializeField, Range(0.01f, 1f)] float hitstopScale = 0.1f;

    [Tooltip("히트스톱 지속 시간(초, unscaled 기준)")]
    [SerializeField, Range(0.01f, 0.2f)] float hitstopDuration = 0.06f;

    [Tooltip("연타 시 히트스톱 갱신 쿨다운(초, unscaled)")]
    [SerializeField, Range(0f, 0.2f)] float hitstopCooldown = 0.03f;

    [Header("Cinemachine Impulse (Camera Shake)")]
    [SerializeField] CinemachineImpulseSource punchImpulse;
    [SerializeField] CinemachineImpulseSource hookImpulse;

    [Tooltip("임펄스 방향. 보통 -forward(카메라 뒤로 튀는 느낌)")]
    [SerializeField] Vector3 impulseDir = new Vector3(0, 0, -1);

    [Tooltip("Shake 중복 방지용 최소 간격(초, unscaled)")]
    [SerializeField, Range(0f, 0.2f)] float shakeCooldown = 0.02f;

    float defaultFixedDelta;
    float stopUntilUnscaled;
    float lastStopUnscaled;
    float lastShakeUnscaled;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        defaultFixedDelta = Time.fixedDeltaTime;
    }

    void Update()
    {
        // 히트스톱 종료
        if (Time.timeScale < 1f && Time.unscaledTime >= stopUntilUnscaled)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDelta;
        }
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

        // 이미 히트스톱 중이면 더 길게만 갱신
        float targetUntil = now + hitstopDuration;
        if (targetUntil > stopUntilUnscaled)
            stopUntilUnscaled = targetUntil;

        Time.timeScale = hitstopScale;

        // 물리도 같이 스케일 맞추기(기본값 기반)
        Time.fixedDeltaTime = defaultFixedDelta * hitstopScale;
    }

    void TryShake(HitKind kind, Vector3 hitWorldPos)
    {
        float now = Time.unscaledTime;
        if (now - lastShakeUnscaled < shakeCooldown) return;
        lastShakeUnscaled = now;

        var src = kind == HitKind.Punch ? punchImpulse : hookImpulse;
        if (!src) return;

        // 임펄스는 위치 기반으로 줄 수도 있고(감쇠), 그냥 카메라 기준으로 줄 수도 있음.
        // 여기서는 "타격 지점에서 흔들림 발생" 느낌.
        src.transform.position = hitWorldPos;

        Vector3 dir = impulseDir.sqrMagnitude > 0.001f ? impulseDir.normalized : Vector3.back;
        src.GenerateImpulse(dir);
    }

#if UNITY_EDITOR
    void OnDisable()
    {
        // 플레이 중 정지 시 timeScale 원복 안전장치
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
    }
#endif
}
