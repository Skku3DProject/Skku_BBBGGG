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

                Debug.Log("�� �������� �� ����");
            }

            else if (SwordAttack.Instance.DashSkill.CurrentSwordDashSkill == true
                && SwordAttack.Instance.DashSkill.IsAttacking == true)
            {

                SwordAttack.Instance.DashSkill.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("�� �뽬�� �� ����");
            }

            else
            {
                SwordAttack.Instance.TryDamageEnemy(other.gameObject, directionToEnemy);
                Debug.Log("�Ϲ� ������ �� ����");
            }
        }

       /* Debug.Log("�� ����");
        if (PlayerAttack == null)
        {
            Debug.Log("PlayerAttack ����");
        }

        if(other.CompareTag("Enemy") && PlayerAttack.CurrentWeaponAttack.IsAttacking)
        {
            Vector3 hitPosition = other.ClosestPoint(transform.position); // �浹 ���� ����
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

            PlayerAttack.TryDamageEnemy(other.gameObject, directionToEnemy);
            Debug.Log("�� ����");
        }*/
    }
}
