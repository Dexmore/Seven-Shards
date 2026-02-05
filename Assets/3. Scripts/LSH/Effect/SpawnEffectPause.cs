using System.Collections;
using UnityEngine;

public class SpawnEffectPause : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnDuration = 0.8f;
    [SerializeField] private float holdTime = 0.0f;

    [Header("Cutoff Range (잔상 방지로 넉넉하게)")]
    [SerializeField] private float cutoffTop = 3.0f;
    [SerializeField] private float cutoffBottom = -3.0f;

    [Header("Padding (잔상 제거용)")]
    [SerializeField] private float noiseStrength = 1.0f;
    [SerializeField] private float glowThickness = 0.08f;
    [SerializeField] private float extraPadding = 0.2f;

    [Header("End Behavior")]
    [Tooltip("소환 완료 후 라인/노이즈 흐름을 멈출지")]
    [SerializeField] private bool stopFlowAfterSpawn = true;

    private MaterialPropertyBlock mpb;
    private Coroutine routine;

    private static readonly int DirectionAndSpeedID = Shader.PropertyToID("_Direction_Speed");
    private static readonly int HightCutoffID      = Shader.PropertyToID("_Hight_Cutoff");

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        float padding = noiseStrength + glowThickness + extraPadding;

        SetCutoff(cutoffTop + padding);

        yield return AnimateCutoff(cutoffTop + padding, cutoffBottom - padding, spawnDuration);

        if (holdTime > 0f)
            yield return new WaitForSeconds(holdTime);

        SetCutoff(cutoffBottom - padding);

        if (stopFlowAfterSpawn)
        {
            targetRenderer.GetPropertyBlock(mpb);
            mpb.SetVector(DirectionAndSpeedID, Vector4.zero);
            targetRenderer.SetPropertyBlock(mpb);
        }

        routine = null;
    }

    private void SetCutoff(float cutoff)
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(HightCutoffID, cutoff);
        targetRenderer.SetPropertyBlock(mpb);
    }

    private IEnumerator AnimateCutoff(float from, float to, float duration)
    {
        targetRenderer.GetPropertyBlock(mpb);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.SmoothStep(0f, 1f, t / duration);

            mpb.SetFloat(HightCutoffID, Mathf.Lerp(from, to, a));
            targetRenderer.SetPropertyBlock(mpb);
            yield return null;
        }

        mpb.SetFloat(HightCutoffID, to);
        targetRenderer.SetPropertyBlock(mpb);
    }
}