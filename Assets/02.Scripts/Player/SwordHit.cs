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

        if(other.CompareTag("Enemy") && PlayerAttack.IsAttacking)
        {
            PlayerAttack.TryDamageEnemy(other.gameObject);
            Debug.Log("�� ����");
        }
    }
}
