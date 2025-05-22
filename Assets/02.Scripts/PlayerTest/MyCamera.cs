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


    // ī�޶� ����ũ��
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

        // ����ũ ������ ����
        transform.position += _shakeOffset;
    }
    //public void Shake(float duration, float magnitude)
    //{
    //    // �̹� ���� ���̸� ���߰� ���� ����
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
    //        // �� ������ ���� ������
    //        _shakeOffset = UnityEngine.Random.insideUnitSphere * magnitude;
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    // ������ �ʱ�ȭ
    //    _shakeOffset = Vector3.zero;
    //    _shakeCoroutine = null;
    //}
}
