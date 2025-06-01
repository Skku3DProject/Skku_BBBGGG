using UnityEngine;

public abstract class WeaponSkillBase : MonoBehaviour
{
   public abstract bool IsUsingSkill { get; protected set; }
    [Header("쿨타임 설정")]
    public float cooldownTime = 5f;
    protected float cooldownTimer = 0f;

    // 쿨타임 남은 시간 확인
    public virtual bool IsCooldown => cooldownTimer > 0f;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer / cooldownTime); // 0~1 비율

    //스킬 발동
    public abstract void UseSkill();

    //적에게 피해 적용
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 hitDirection);

    //이펙트 재생 시점
    public abstract void OnSkillEffectPlay();

    //애니메이션이 끝나는 시점
    public abstract void OnSkillAnimationEnd();

    //쿨타임 체크
   // public virtual bool IsSkillAvailable() => true;


    public virtual void ResetState() { }

    public virtual void Tick()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // UI 연동 위치 예시
        // UpdateSkillCooldownUI(CooldownRemaining);
    }
}
