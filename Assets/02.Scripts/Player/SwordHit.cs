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
            Debug.Log("검 공격해서 적과 닿은... 거니?");
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
