using UnityEngine;

public abstract class WeaponSkillBase : MonoBehaviour
{
   public abstract bool IsUsingSkill { get; protected set; }

    //��ų �ߵ�
    public abstract void UseSkill();

    //������ ���� ����
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 hitDirection);

    //����Ʈ ��� ����
    public abstract void OnSkillEffectPlay();

    //�ִϸ��̼��� ������ ����
    public abstract void OnSkillAnimationEnd();

    //��Ÿ�� üũ
    public virtual bool IsSkillAvailable() => true;

    //��ų ��Ÿ�� ����
    public virtual void Tick() { }
}
