using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public float hp = 100f;
    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        hp -= amount;
        ParticleManager.I.Play(ParticleManager.FXType.Hit_Punch, hitPoint, Quaternion.LookRotation(hitNormal));
    }
}
