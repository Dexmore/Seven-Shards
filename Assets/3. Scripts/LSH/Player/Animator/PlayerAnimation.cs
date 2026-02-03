using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float speed01)
    {
        anim.SetFloat(SpeedHash, speed01, 0.12f, Time.deltaTime);
    }

    public void SetAir(bool grounded, float yVel)
    {
        anim.SetBool(IsGroundedHash, grounded);
        anim.SetFloat(YVelocityHash, yVel);
    }
}
