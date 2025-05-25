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

        if(other.CompareTag("Enemy") && PlayerAttack.CurrentWeaponAttack.IsAttacking)
        {
            Vector3 hitPosition = other.ClosestPoint(transform.position); // �浹 ���� ����
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;
            Vector3 oppositeDirection = -directionToEnemy;

            PlayerAttack.TryDamageEnemy(other.gameObject, oppositeDirection);
            Debug.Log("�� ����");
        }
    }
}
