using UnityEngine;

public interface IDamageable
{
    void TakeDamage(string hitAnimationName);

    void TakeDamage(string hitAnimationName, Transform attacker);

    void TakeDamage(Transform attacker, float damage);
}
