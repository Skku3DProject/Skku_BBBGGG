using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    private float _damage;
    private Rigidbody _rb;

    [Header("Fire Arrow Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask enemyLayer;

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
        Vector3 directionToTarget = (other.transform.position - transform.position).normalized;

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (BowAttack.Instance.FireArrow.CurrentArrowFireSkill)
            {
                Explode();
                Debug.Log("Æø¹ß È­»ì Àû¿¡ ´ê¾Æ¼­ Æø¹ß");
            }
            else if (BowAttack.Instance.TripleArrow.CurrentThreeArrowSkill)
            {
                BowAttack.Instance.TripleArrow.TryDamageEnemy(other.gameObject, directionToTarget);
            }
            else
            {
                BowAttack.Instance.TryDamageEnemy(other.gameObject, directionToTarget);
            }

            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            if (BowAttack.Instance.FireArrow.CurrentArrowFireSkill)
            {
                Explode();
            }

            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        // Æø¹ß ÀÌÆåÆ® »ý¼º
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Æø¹ß ¹üÀ§ ³» Àû °¨Áö
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (Collider col in hitEnemies)
        {
            if (col.CompareTag("Enemy")) // Àû¸¸ Å¸°ÙÆÃ
            {
                Vector3 hitDirection = (col.transform.position - transform.position).normalized;
                BowAttack.Instance.FireArrow.TryDamageEnemy(col.gameObject, hitDirection);
            }
        }
    }

    public void SetAttackPower(float power)
    {
        _damage = power;
    }
}

/*using Unity.VisualScripting;
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


        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Àû°ú Ãæµ¹");
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
            //Debug.Log("¶¥°ú Ãæµ¹");
            Destroy(gameObject);

        }

    }

    public void SetAttackPower(float power)
    {
        _damage = power;
    }
}
*/