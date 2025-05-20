using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;


public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;

    

    //캐릭터 움직임 관련
    private float _gravity = -9.81f;
    private float _currentHealth;
    private Vector3 _velocity;
    private bool _isGrounded;
    private CharacterController _characterController;
    

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    // 애니메이션
    private Animator _playerAnimator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();

       
    }

    private void Update()
    {
        // 바닥 확인
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 이동 입력
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        // 점프
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        
    }

    

}
