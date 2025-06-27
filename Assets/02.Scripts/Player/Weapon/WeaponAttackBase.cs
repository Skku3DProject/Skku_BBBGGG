using UnityEngine;

public abstract class WeaponAttackBase : MonoBehaviour
{
    public bool IsAttacking;
    public abstract void Attack();
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 dir);
    public abstract void OnAttackEffectPlay();
    public abstract void OnAttackAnimationEnd(EEquipmentType currentType);
    public abstract void EnableComboInput();

    // �� ���⺰�� ƽ���� ���������� �Է°� ���¸� �����ؾ��� �ƴϸ� ��������
    public virtual void Tick() { }
}
