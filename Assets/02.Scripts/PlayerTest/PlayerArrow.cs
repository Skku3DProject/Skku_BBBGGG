using Unity.VisualScripting;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    private float _damage;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);

            transform.Rotate(90f, 0f, 0f, Space.Self);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        /*if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.TryGetComponent<IDamageAble>(out var d))
            {
                d.TakeDamage(new Damage(_damage, gameObject, 10));
            }
            Destroy(gameObject);

        }*/

        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("적과 충돌");
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;


            if(BowAttack.Instance.TripleArrow.CurrentThreeArrowSkill == false 
                && BowAttack.Instance.FireArrow.CurrentArrowFireSkill == false)
             {
                BowAttack.Instance.TryDamageEnemy(other.gameObject, directionToEnemy);
            }

            else if(BowAttack.Instance.TripleArrow.CurrentThreeArrowSkill == true)
            {
                BowAttack.Instance.TripleArrow.TryDamageEnemy(other.gameObject, directionToEnemy);
            }

            else if (BowAttack.Instance.FireArrow.CurrentArrowFireSkill == true)
            {
                BowAttack.Instance.FireArrow.TryDamageEnemy(other.gameObject, directionToEnemy);
            }

            Destroy(gameObject);

        }

        else if (other.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("땅과 충돌");
            Destroy(gameObject);

        }

    }

    public void SetAttackPower(float power)
    {
        _damage = power;
    }
}
