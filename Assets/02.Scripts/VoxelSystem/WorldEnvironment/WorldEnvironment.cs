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
                // ���� ����
                fracture?.Explode();
            }
            if(type == EnvirormentType.Grass)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null && playerObj.TryGetComponent(out ThirdPersonPlayer player))
                {
                    player.RecoveryStamina(10f); // ȸ������ ���ϴ� ������ ����
                }
            }
            if(type == EnvirormentType.Mashroom)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null && playerObj.TryGetComponent(out ThirdPersonPlayer player))
                {
                    player.RecoveryStamina(30f); // ȸ������ ���ϴ� ������ ����
                }
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
