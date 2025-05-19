using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;//ü��, �̵� �ӵ�

    [Header("Weapon")]
    public List<WeaponAttackSO> Weapons = new List<WeaponAttackSO>();
    [SerializeField]
    private int _currentWeaponIndex = 0;//���� ���� �ε��� 0�� ��

    [SerializeField]
    private WeaponAttackSO _currnetWeapon;//���� ����


    private float _currentHealth;
    
    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _gravity = -9.81f;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    private void Start()
    {

        _characterController = GetComponent<CharacterController>();

        _currnetWeapon = Weapons[_currentWeaponIndex];
        Debug.Log($"���� ���� : {_currnetWeapon.WeaponName}");

    }


    private void Update()
    {
        //�ٴ� Ȯ��
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        //�Է�
        float x = Input.GetAxis("Horizontal");//��/��
        float z = Input.GetAxis("Vertical");//��/�Ʒ�

        Vector3 move = transform.right * x + transform.forward * z;

        //�̵�
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        //����
        if(Input.GetButtonDown("Jump") && _isGrounded)
        {
            Debug.Log("���� ������� ��������");
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }


        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("QŰ ����");
            _currentWeaponIndex = (_currentWeaponIndex + 1) % Weapons.Count;
            _currnetWeapon = Weapons[_currentWeaponIndex];
            Debug.Log($"���� ����: {_currnetWeapon.WeaponName}");
        }
    }
}
