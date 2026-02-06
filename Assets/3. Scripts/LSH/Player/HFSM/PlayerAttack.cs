using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public sealed class PlayerAttack : MonoBehaviour
{
    public enum AttackMoveMode { Full, Slow, Lock }

    [Header("Animator")]
    [SerializeField] int attackLayer = 1;
    [SerializeField] string punchingStateName = "Punching";
    [SerializeField] string hookStateName = "HookPunch";

    [Header("Input / Combo")]
    [SerializeField] float bufferTime = 0.20f;
    [SerializeField, Range(0f, 1f)] float comboOpen  = 0.55f;
    [SerializeField, Range(0f, 1f)] float comboClose = 0.90f;

    [Header("Move While Attacking")]
    [SerializeField] AttackMoveMode moveMode = AttackMoveMode.Lock;
    [SerializeField, Range(0f, 1f)] float slowMoveScale = 0.35f;

    [Header("Hit Origins")]
    [SerializeField] Transform leftHandOrigin;
    [SerializeField] Transform rightHandOrigin;

    [Header("Hit Detection")]
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Punch")]
    [SerializeField, Range(0f, 1f)] float punchHitOpen  = 0.20f;
    [SerializeField, Range(0f, 1f)] float punchHitClose = 0.35f;
    [SerializeField] float punchRange = 1.2f;
    [SerializeField] float punchRadius = 0.35f;
    [SerializeField] float punchDamage = 10f;

    [Header("Hook")]
    [SerializeField, Range(0f, 1f)] float hookHitOpen  = 0.25f;
    [SerializeField, Range(0f, 1f)] float hookHitClose = 0.45f;
    [SerializeField] float hookRange = 1.35f;
    [SerializeField] float hookRadius = 0.40f;
    [SerializeField] float hookDamage = 14f;

    [Header("Hit FX")]
    [SerializeField] ParticleManager.FXType punchFX = ParticleManager.FXType.Hit_Punch;
    [SerializeField] ParticleManager.FXType hookFX  = ParticleManager.FXType.Hit_Hook;
    [SerializeField] bool spawnFXOnHit = true;

    [Header("Buffers")]
    [SerializeField] int hitBufferSize = 32;
    [SerializeField] int damageableCacheCapacity = 128;

    Animator anim;
    PlayerController pc;

    int punchHash, hookHash;
    static readonly int AttackTrig = Animator.StringToHash("Attack");
    static readonly int ComboTrig  = Animator.StringToHash("Combo");

    float bufferedUntil;
    bool comboSent;
    bool wasAttacking;

    bool hitConsumed;
    int lastStateHash;

    Collider[] hitCols;
    HashSet<int> hitIds;

    struct DmgEntry { public IDamageable dmg; public bool has; }
    Dictionary<int, DmgEntry> dmgCache;

    const float EPS = 0.0001f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        pc   = GetComponent<PlayerController>();
        punchHash = Animator.StringToHash(punchingStateName);
        hookHash  = Animator.StringToHash(hookStateName);
        hitCols  = new Collider[Mathf.Max(8, hitBufferSize)];
        hitIds   = new HashSet<int>(Mathf.Max(16, hitBufferSize));
        dmgCache = new Dictionary<int, DmgEntry>(damageableCacheCapacity);
    }

    void Update()
    {
        AnimatorStateInfo st = anim.IsInTransition(attackLayer)
            ? anim.GetNextAnimatorStateInfo(attackLayer)
            : anim.GetCurrentAnimatorStateInfo(attackLayer);
        int stateHash = st.shortNameHash;
        float t = st.normalizedTime;
        float time01 = t - Mathf.Floor(t);
        bool isPunching = stateHash == punchHash;
        bool isHook     = stateHash == hookHash;
        bool attacking  = isPunching || isHook;
        if (attacking != wasAttacking)
        {
            wasAttacking = attacking;
            hitConsumed = false;
            hitIds.Clear();
            lastStateHash = 0;
            if (attacking)
            {
                pc.motor.SetMoveScale(
                    moveMode == AttackMoveMode.Full ? 1f :
                    moveMode == AttackMoveMode.Slow ? slowMoveScale : 0f);
                if (moveMode == AttackMoveMode.Lock)
                    pc.motor.SetMoveInput(Vector3.zero);
            }
            else
            {
                pc.motor.SetMoveScale(1f);
                bufferedUntil = 0f;
                comboSent = false;
                return;
            }
        }
        if (pc.AttackPressedThisFrame)
        {
            if (isPunching)
                bufferedUntil = Time.time + bufferTime;
            else if (!attacking)
            {
                anim.ResetTrigger(ComboTrig);
                anim.SetTrigger(AttackTrig);
            }
        }
        if (isPunching && !comboSent && Time.time <= bufferedUntil)
        {
            if (time01 >= comboOpen && time01 <= comboClose)
            {
                anim.SetTrigger(ComboTrig);
                comboSent = true;
                bufferedUntil = 0f;
            }
        }
        if (stateHash != lastStateHash)
        {
            lastStateHash = stateHash;
            hitConsumed = false;
            hitIds.Clear();
        }
        if (hitConsumed) return;
        if (isPunching)
            TryHit(time01, punchHitOpen, punchHitClose, punchRange, punchRadius, punchDamage, leftHandOrigin, punchFX);
        else if (isHook)
            TryHit(time01, hookHitOpen, hookHitClose, hookRange, hookRadius, hookDamage, rightHandOrigin, hookFX);
    }

    void TryHit(float t01, float open, float close, float range, float radius, float dmg,
                Transform originTr, ParticleManager.FXType fxType)
    {
        if (t01 + EPS < open || t01 - EPS > close) return;
        hitConsumed = true;
        hitIds.Clear();
        Vector3 origin = originTr ? originTr.position : transform.position;
        Vector3 center = origin + transform.forward * range;
        int count;
        while (true)
        {
            count = Physics.OverlapSphereNonAlloc(center, radius, hitCols, hitMask, triggerInteraction);
            if (count < hitCols.Length || hitCols.Length >= 256) break;
            hitCols = new Collider[hitCols.Length * 2];
        }
        for (int i = 0; i < count; i++)
        {
            Collider col = hitCols[i];
            if (!col) continue;
            Transform root = col.attachedRigidbody ? col.attachedRigidbody.transform.root : col.transform.root;
            int id = root.GetInstanceID();
            if (!hitIds.Add(id)) continue;
            if (TryGetDmg(root, id, out var d))
            {
                Vector3 hitPos = col.ClosestPoint(origin);
                Vector3 hitNormal = hitPos - origin;
                if (hitNormal.sqrMagnitude < 0.0001f)
                    hitNormal = -transform.forward;
                else
                    hitNormal.Normalize();
                d.TakeDamage(dmg, hitPos, hitNormal);
                if (HitFeedbackManager.I != null)
                {
                    HitFeedbackManager.I.OnHit(
                        fxType == ParticleManager.FXType.Hit_Punch ? HitFeedbackManager.HitKind.Punch : HitFeedbackManager.HitKind.Hook,
                        hitPos);
                }
                if (spawnFXOnHit && ParticleManager.I != null)
                {
                    ParticleManager.I.Play(fxType, hitPos, Quaternion.LookRotation(hitNormal));
                }
                AudioManager.I.PlaySFX(
                    fxType == ParticleManager.FXType.Hit_Punch ? AudioManager.SFXType.PunchHit : AudioManager.SFXType.HookHit,
                    hitPos);
            }
        }
    }

    bool TryGetDmg(Transform root, int id, out IDamageable dmg)
    {
        if (dmgCache.TryGetValue(id, out var e))
        {
            dmg = e.dmg;
            return e.has && dmg != null;
        }
        dmg = root.GetComponentInChildren<IDamageable>();
        dmgCache[id] = new DmgEntry { dmg = dmg, has = dmg != null };
        return dmg != null;
    }

    public void ClearDamageableCache() => dmgCache.Clear();
}