using UnityEngine;

public abstract class WeaponAttackBase : MonoBehaviour
{
    public abstract bool IsAttacking { get; protected set; }
    public abstract void Attack();
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 dir);
    public abstract void OnAttackEffectPlay();
    public abstract void OnAttackAnimationEnd();
    public abstract void EnableComboInput();

    // �� ������ ȣ��Ǵ� ���� �޼���
    public virtual void Tick() { }
}
