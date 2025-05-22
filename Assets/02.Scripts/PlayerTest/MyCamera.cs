using System;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private Transform _target;
    [SerializeField] private CameraOffset _thirdPersonOffset;
    [SerializeField] private float _rotationSpeed = 300f;

    private ThirdPersonCameraMode _thirdPerson;

    private float _rotationX;
    private float _rotationY;


    // 카메라 쉐이크용
    private Vector3 _shakeOffset = Vector3.zero;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _shakeOffset = Vector3.zero;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _thirdPerson = new ThirdPersonCameraMode(_thirdPersonOffset, ref _rotationX, ref _rotationY, _rotationSpeed);
    }

    private void Update()
    {
        _thirdPerson.UpdateCamera(transform, _target);

        // 쉐이크 오프셋 적용
        transform.position += _shakeOffset;
    }
    //public void Shake(float duration, float magnitude)
    //{
    //    // 이미 진행 중이면 멈추고 새로 시작
    //    if (_shakeCoroutine != null)
    //        StopCoroutine(_shakeCoroutine);
    //    _shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    //}

    //private IEnumerator ShakeCoroutine(float duration, float magnitude)
    //{
    //    Vector3 originalOffset = Vector3.zero;
    //    float elapsed = 0f;

    //    while (elapsed < duration)
    //    {
    //        // 매 프레임 랜덤 오프셋
    //        _shakeOffset = UnityEngine.Random.insideUnitSphere * magnitude;
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    // 끝나면 초기화
    //    _shakeOffset = Vector3.zero;
    //    _shakeCoroutine = null;
    //}
}
