using UnityEngine;

public abstract class WeaponSkillBase : MonoBehaviour
{
   public abstract bool IsUsingSkill { get; protected set; }

    //스킬 발동
    public abstract void UseSkill();

    //적에게 피해 적용
    public abstract void TryDamageEnemy(GameObject enemy, Vector3 hitDirection);

    //이펙트 재생 시점
    public abstract void OnSkillEffectPlay();

    //애니메이션이 끝나는 시점
    public abstract void OnSkillAnimationEnd();

    //쿨타임 체크
    public virtual bool IsSkillAvailable() => true;

    //스킬 쿨타임 관리
    public virtual void Tick() { }
}
