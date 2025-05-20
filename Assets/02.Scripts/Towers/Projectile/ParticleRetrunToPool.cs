using UnityEngine;

public class ParticleRetrunToPool : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private bool _isReturning = false;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        _isReturning = false;
    }

    private void Update()
    {
        if (_particleSystem == null || _isReturning) return;

        // ��� ��ƼŬ�� �� �����ٸ� Ǯ�� ��ȯ
        if (!_particleSystem.IsAlive(true))
        {
            _isReturning = true;

            // Ǯ�� ��ȯ (������Ʈ Ǯ ������ ���� ���� �ʿ�)
            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }
}
