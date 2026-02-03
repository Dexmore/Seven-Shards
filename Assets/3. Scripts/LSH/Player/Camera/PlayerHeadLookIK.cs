using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerHeadLookIK : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform aimOrigin;
    [SerializeField] PlayerMotor motor;

    [Header("Look At")]
    [Range(0f, 1f)] [SerializeField] float weight = 0.85f;
    [Range(0f, 1f)] [SerializeField] float headWeight = 1f;
    [Range(0f, 1f)] [SerializeField] float clampWeight = 0.6f;
    [SerializeField] float lookDistance = 10f;

    [Header("Performance")]
    [SerializeField] bool disableInAir = true;
    [SerializeField] float smooth = 14f;

    Animator anim;

    Vector3 lookTarget;
    float currentWeight;

    bool wasActiveLastFrame;

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!cameraTransform)
        {
            DisableIK();
            return;
        }

        bool active = !(disableInAir && motor && !motor.IsGrounded);

        if (!active)
        {
            DisableIK();
            wasActiveLastFrame = false;
            return;
        }

        Vector3 origin = aimOrigin
            ? aimOrigin.position
            : transform.position + Vector3.up * 1.6f;

        Vector3 desiredTarget =
            origin + cameraTransform.forward * lookDistance;

        float k = 1f - Mathf.Exp(-smooth * Time.deltaTime);

        lookTarget = Vector3.Lerp(lookTarget, desiredTarget, k);
        currentWeight = Mathf.Lerp(currentWeight, weight, k);

        if (!wasActiveLastFrame)
        {
            anim.SetLookAtWeight(currentWeight,0f, headWeight, 0f, clampWeight);
        }
        else
        {
            anim.SetLookAtWeight(currentWeight);
        }

        anim.SetLookAtPosition(lookTarget);
        wasActiveLastFrame = true;
    }

    void DisableIK()
    {
        if (currentWeight > 0f)
        {
            currentWeight = 0f;
            anim.SetLookAtWeight(0f);
        }
    }
}
