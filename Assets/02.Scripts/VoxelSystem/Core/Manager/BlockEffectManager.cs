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

        // 이벤트 구독으로 의존성 역전
        BlockManager.OnBlockDamaged += PlayDamageEffect;
        BlockManager.OnBlockBroken += PlayBreakEffect;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            BlockManager.OnBlockDamaged -= PlayDamageEffect;
            BlockManager.OnBlockBroken -= PlayBreakEffect;
        }
    }

    private void PlayDamageEffect(Vector3Int worldPos)
    {
        if (DamageEffectPrefab == null)
            return;
        Vector3 spawnPos = worldPos + new Vector3(0.5f, 0.5f, 0.5f);

        // ObjectPool 의존성도 제거 가능한 방법
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.GetObject(DamageEffectPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // fallback: 일반 Instantiate
            var effect = Instantiate(DamageEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2초 후 삭제
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
            var effect = Instantiate(BreakEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
}
