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
                Debug.Log("���� ȭ�� ���� ��Ƽ� ����");
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
        // ���� ����Ʈ ����
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // ���� ���� �� �� ����
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (Collider col in hitEnemies)
        {
            if (col.CompareTag("Enemy")) // ���� Ÿ����
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
            //Debug.Log("���� �浹");
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
            //Debug.Log("���� �浹");
            Destroy(gameObject);

        }

    }

    public void SetAttackPower(float power)
    {
        _damage = power;
    }
}
*/