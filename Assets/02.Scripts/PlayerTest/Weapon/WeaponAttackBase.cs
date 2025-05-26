using UnityEngine;

public abstract class WeaponAttackBase : MonoBehaviour
{
    public abstract bool IsAttacking { get; protected set; }
    public abstract void Attack();
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 dir);
    public abstract void OnAttackEffectPlay();
    public abstract void OnAttackAnimationEnd();
    public abstract void EnableComboInput();

    // 각 무기별로 틱에서 독립적으로 입력과 상태를 관리해야함 아니면 복잡해짐
    public virtual void Tick() { }
}
