using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float speed01)
    {
        anim.SetFloat(SpeedHash, speed01, 0.12f, Time.deltaTime);
    }
}
