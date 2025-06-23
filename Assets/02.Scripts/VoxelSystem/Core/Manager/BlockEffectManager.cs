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

        //BlockManager.OnBlockDamaged += PlayDamageEffect;
        //BlockManager.OnBlockBroken += PlayBreakEffect;
        VoxelEvents.OnBlockDamaged += PlayDamageEffect;
        VoxelEvents.OnBlockBroken += PlayBreakEffect;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            VoxelEvents.OnBlockDamaged -= PlayDamageEffect;
            VoxelEvents.OnBlockBroken -= PlayBreakEffect;
            //BlockManager.OnBlockDamaged -= PlayDamageEffect;
            //BlockManager.OnBlockBroken -= PlayBreakEffect;
        }
    }

    private void PlayDamageEffect(Vector3Int worldPos)
    {
        if (DamageEffectPrefab == null)
            return;
        Vector3 spawnPos = worldPos + new Vector3(0.5f, 0.5f, 0.5f);

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.GetObject(DamageEffectPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("오브젝트풀이 존재하지 않습니다");
        }
    }

    private void PlayBreakEffect(Vector3Int worldPos)
    {
        if (BreakEffectPrefab == null)
            return;
        Vector3 spawnPos = worldPos + new Vector3(0.5f, 0.5f, 0.5f);

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.GetObject(BreakEffectPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("오브젝트풀이 존재하지 않습니다");
        }
    }
}
