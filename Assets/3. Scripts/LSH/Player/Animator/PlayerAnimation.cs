using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    static int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float speed01)
    {
        anim.SetFloat(SpeedHash, speed01, 0.12f, Time.deltaTime);
    }
}
