using UnityEngine;

public abstract class WeaponAttackBase : MonoBehaviour
{
    public abstract void Attack();
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 dir);
    public abstract void OnAttackEffectPlay();
    public abstract void OnAttackAnimationEnd();
}
