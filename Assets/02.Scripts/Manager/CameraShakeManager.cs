using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    private Vector3 _shakeOffset = Vector3.zero;
    private float _shakeDuration = 0f;
    private float _shakeStrength = 0.1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_shakeDuration > 0)
        {
            _shakeOffset = Random.insideUnitSphere * _shakeStrength;
            _shakeDuration -= Time.deltaTime;

            if (_shakeDuration <= 0)
                _shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float duration, float strength)
    {
        _shakeDuration = duration;
        _shakeStrength = strength;
    }

    public Vector3 GetShakeOffset()
    {
        return _shakeOffset;
    }
}
