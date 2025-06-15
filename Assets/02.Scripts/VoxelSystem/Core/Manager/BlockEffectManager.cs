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

        // �̺�Ʈ �������� ������ ����
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

        // ObjectPool �������� ���� ������ ���
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.GetObject(DamageEffectPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // fallback: �Ϲ� Instantiate
            var effect = Instantiate(DamageEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2�� �� ����
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
