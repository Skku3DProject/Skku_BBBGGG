using UnityEngine;

public class WorldEnvironment : MonoBehaviour
{
    [SerializeField] private Currency _currency;
    [SerializeField] private float _health = 30;
    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            CurrencyManager.instance.Add(_currency);
            gameObject.SetActive(false);
        }
    }

}
