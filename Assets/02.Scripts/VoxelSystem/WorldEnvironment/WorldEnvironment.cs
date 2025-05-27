using System.Collections.Generic;
using UnityEngine;

public class WorldEnvironment : MonoBehaviour
{
    [SerializeField] private Currency _currency;
    [SerializeField] private float _health = 30;

    private FractureExplosion fracture;
    private List<GameObject> fragments = new List<GameObject>();

    private void Awake()
    {
        fracture = GetComponent<FractureExplosion>();
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            CurrencyManager.instance.Add(_currency);
            // Æø¹ß ½ÇÇà
            fracture.Explode();
            Invoke(nameof(DisableFragments), 6f);
            gameObject.SetActive(false);
        }
    }
    private void DisableFragments()
    {
        fracture.ReturnFragmentsToPool();
    }
}
