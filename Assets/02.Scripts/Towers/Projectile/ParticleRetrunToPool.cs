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

        // 모든 파티클이 다 끝났다면 풀로 반환
        if (!_particleSystem.IsAlive(true))
        {
            _isReturning = true;

            // 풀로 반환 (오브젝트 풀 구현에 따라 수정 필요)
            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }
}
