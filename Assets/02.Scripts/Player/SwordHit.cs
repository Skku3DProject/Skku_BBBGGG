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

        if(other.CompareTag("Enemy") && PlayerAttack.IsAttacking)
        {
            PlayerAttack.TryDamageEnemy(other.gameObject);
            Debug.Log("적 공격");
        }
    }
}
