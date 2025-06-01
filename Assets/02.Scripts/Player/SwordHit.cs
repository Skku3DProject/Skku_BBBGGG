using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public PlayerAttack PlayerAttack;

    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("적 공격");
        if (PlayerAttack == null)
        {
            Debug.Log("PlayerAttack 없음");
        }

        if (other.CompareTag("Enemy"))
        {
            if(PlayerAttack.CurrentWeaponAttack.IsAttacking)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position); // 충돌 지점 추정
                Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

                PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("적 공격");
            }
            else if (PlayerAttack.Skill1.IsUsingSkill)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position); // 충돌 지점 추정
                Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

                PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("적 공격");
            }


        }
    }
}
