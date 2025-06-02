using System.Collections.Generic;
using UnityEngine;



public class WorldEnvironment : MonoBehaviour
{
    private enum EnvirormentType
    {
        Tree,
        Stone,
        Grass,
        Mashroom
    }

    [SerializeField] private EnvirormentType type;
    [SerializeField] private Currency _currency;
    [SerializeField] private float _health = 30;
    [SerializeField] private GameObject HitPrefab;


    private FractureExplosion fracture;
    private List<GameObject> fragments = new List<GameObject>();

    private void Awake()
    {
        fracture = GetComponent<FractureExplosion>();
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;

        ObjectPool.Instance.GetObject(HitPrefab, gameObject.transform.position, Quaternion.identity);
        if (_health <= 0)
        {
            PlaySound();

            if (type == EnvirormentType.Tree || type == EnvirormentType.Stone)
            {
                CurrencyManager.instance.Add(_currency);
                // Æø¹ß ½ÇÇà
                fracture?.Explode();
            }
            gameObject.SetActive(false);
        }
    }

    private void PlaySound()
    {
        switch (type)
        {
            case EnvirormentType.Tree:
                PlayerSoundController.Instance.PlaySoundAtPosition(PlayerSoundType.TreeSound, gameObject.transform.position);
                break;
            case EnvirormentType.Stone:
                PlayerSoundController.Instance.PlaySoundAtPosition(PlayerSoundType.StoneSound, gameObject.transform.position);
                break;
        }

    }

}
