using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public PlayerAttack PlayerAttack;

    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;


           

            if (SwordAttack.Instance.SpinSkill.CurrentSwordSpinSkill == true
                && SwordAttack.Instance.SpinSkill.IsAttacking == true)
            {
                SwordAttack.Instance.SpinSkill.TryDamageEnemy(other.gameObject, directionToEnemy);

                Debug.Log("검 스핀으로 적 공격");
            }

            else if (SwordAttack.Instance.DashSkill.CurrentSwordDashSkill == true
                && SwordAttack.Instance.DashSkill.IsAttacking == true)
            {

                SwordAttack.Instance.DashSkill.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("검 대쉬로 적 공격");
            }

            else
            {
                SwordAttack.Instance.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("일반 검으로 적 공격");
            }
        }

       /* Debug.Log("적 공격");
        if (PlayerAttack == null)
        {
            Debug.Log("PlayerAttack 없음");
        }

        if(other.CompareTag("Enemy") && PlayerAttack.CurrentWeaponAttack.IsAttacking)
        {
            Vector3 hitPosition = other.ClosestPoint(transform.position); // 충돌 지점 추정
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

            PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
            Debug.Log("적 공격");
        }*/
    }
}
