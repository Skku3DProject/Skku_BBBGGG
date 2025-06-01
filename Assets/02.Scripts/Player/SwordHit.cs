using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public PlayerAttack PlayerAttack;

    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�� ����");
        if (PlayerAttack == null)
        {
            Debug.Log("PlayerAttack ����");
        }

        if (other.CompareTag("Enemy"))
        {
            if(PlayerAttack.CurrentWeaponAttack.IsAttacking)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position); // �浹 ���� ����
                Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

                PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("�� ����");
            }
            else if (PlayerAttack.Skill1.IsUsingSkill)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position); // �浹 ���� ����
                Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

                PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("�� ����");
            }


        }
    }
}
