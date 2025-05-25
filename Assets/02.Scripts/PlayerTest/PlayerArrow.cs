using Unity.VisualScripting;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.TryGetComponent<IDamageAble>(out var d))
            {
                d.TakeDamage(new Damage(10, gameObject, 10));
            }
            Destroy(gameObject);

        }
    }
}
