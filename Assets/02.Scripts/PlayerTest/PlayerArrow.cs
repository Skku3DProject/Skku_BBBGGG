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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.TryGetComponent<IDamageAble>(out var d))
            {
                d.TakeDamage(new Damage(_damage, gameObject, 10));
            }
            Destroy(gameObject);

        }
    }

    public void SetAttackPower(float power)
    {
        _damage = power;
    }
}
