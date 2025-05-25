using UnityEngine;

public abstract class WeaponAttackBase : MonoBehaviour
{
    public abstract bool IsAttacking { get; protected set; }
    public abstract void Attack();
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 dir);
    public abstract void OnAttackEffectPlay();
    public abstract void OnAttackAnimationEnd();
    public abstract void EnableComboInput();

    // 매 프레임 호출되는 가상 메서드
    public virtual void Tick() { }
}
