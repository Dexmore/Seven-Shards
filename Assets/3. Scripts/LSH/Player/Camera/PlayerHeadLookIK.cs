using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerHeadLookIK : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraTransform;
    public Transform aimOrigin;

    [Header("Look At")]
    public float lookDistance = 10f;
    [Range(0f, 1f)] public float weight = 0.8f;
    [Range(0f, 1f)] public float bodyWeight = 0.15f;
    [Range(0f, 1f)] public float headWeight = 0.85f;
    [Range(0f, 1f)] public float eyesWeight = 1.0f;
    [Range(0f, 1f)] public float clampWeight = 0.6f;

    [Header("Smoothing")]
    public float smooth = 12f;

    public bool disableInAir = false;

    private Animator anim;
    private Vector3 _targetPosSmoothed;
    private float _wSmoothed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        _targetPosSmoothed = transform.position + transform.forward * lookDistance;
        _wSmoothed = 0f;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (cameraTransform == null)
        {
            anim.SetLookAtWeight(0f);
            return;
        }

        Vector3 origin = aimOrigin ? aimOrigin.position : (transform.position + Vector3.up * 1.6f);
        Vector3 desired = origin + cameraTransform.forward * lookDistance;

        float dt = Time.deltaTime;
        float k = 1f - Mathf.Exp(-smooth * dt);

        _targetPosSmoothed = Vector3.Lerp(_targetPosSmoothed, desired, k);
        _wSmoothed = Mathf.Lerp(_wSmoothed, weight, k);

        anim.SetLookAtWeight(_wSmoothed, bodyWeight, headWeight, eyesWeight, clampWeight);
        anim.SetLookAtPosition(_targetPosSmoothed);
    }
}
