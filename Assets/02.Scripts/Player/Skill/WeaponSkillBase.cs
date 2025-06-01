using UnityEngine;

public abstract class WeaponSkillBase : MonoBehaviour
{
   public abstract bool IsUsingSkill { get; protected set; }
    [Header("��Ÿ�� ����")]
    public float cooldownTime = 5f;
    protected float cooldownTimer = 0f;

    // ��Ÿ�� ���� �ð� Ȯ��
    public virtual bool IsCooldown => cooldownTimer > 0f;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer / cooldownTime); // 0~1 ����

    //��ų �ߵ�
    public abstract void UseSkill();

    //������ ���� ����
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 hitDirection);

    //����Ʈ ��� ����
    public abstract void OnSkillEffectPlay();

    //�ִϸ��̼��� ������ ����
    public abstract void OnSkillAnimationEnd();

    //��Ÿ�� üũ
   // public virtual bool IsSkillAvailable() => true;


    public virtual void ResetState() { }

    public virtual void Tick()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // UI ���� ��ġ ����
        // UpdateSkillCooldownUI(CooldownRemaining);
    }
}
