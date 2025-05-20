using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;


public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;

    

    //ĳ���� ������ ����
    private float _gravity = -9.81f;
    private float _currentHealth;
    private Vector3 _velocity;
    private bool _isGrounded;
    private CharacterController _characterController;
    

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    // �ִϸ��̼�
    private Animator _playerAnimator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();

       
    }

    private void Update()
    {
        // �ٴ� Ȯ��
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // �̵� �Է�
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        // ����
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }

        // �߷� ����
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        
    }

    

}
