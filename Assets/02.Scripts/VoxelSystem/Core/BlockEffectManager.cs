using UnityEngine;

public class BlockEffectManager : MonoBehaviour
{
    public static BlockEffectManager Instance { get; private set; }

    [Header("Block Damage Effect")]
    public GameObject DamageEffectPrefab;

    [Header("Block Break Effect")]
    public GameObject BreakEffectPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayDamageEffect(Vector3Int worldPos)
    {
        if (DamageEffectPrefab == null)
            return;
        Vector3 spawnPos = worldPos + new Vector3(0.5f, 0.5f, 0.5f);
        ObjectPool.Instance.GetObject(DamageEffectPrefab, spawnPos, Quaternion.identity);

        //Object.Instantiate(DamageEffectPrefab, spawnPos, Quaternion.identity);
    }

    public void PlayBreakEffect(Vector3Int worldPos)
    {
        if (BreakEffectPrefab == null)
            return;
        Vector3 spawnPos = worldPos + new Vector3(0.5f, 0.5f, 0.5f);
        //Object.Instantiate(BreakEffectPrefab, spawnPos, Quaternion.identity);
        ObjectPool.Instance.GetObject(BreakEffectPrefab, spawnPos, Quaternion.identity);
    }
}
