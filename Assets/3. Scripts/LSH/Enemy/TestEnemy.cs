using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public float hp = 100f;
    public void TakeDamage(float damage)
    {
        hp -= damage;
    }
}
