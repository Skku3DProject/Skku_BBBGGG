using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;//체력, 이동 속도

    [Header("Weapon")]
    public List<WeaponAttackSO> Weapons = new List<WeaponAttackSO>();
    [SerializeField]
    private int _currentWeaponIndex = 0;//현재 무기 인덱스 0이 검

    [SerializeField]
    private WeaponAttackSO _currnetWeapon;//현재 무기


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
        Debug.Log($"현재 무기 : {_currnetWeapon.WeaponName}");

    }


    private void Update()
    {
        //바닥 확인
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        //입력
        float x = Input.GetAxis("Horizontal");//좌/우
        float z = Input.GetAxis("Vertical");//위/아래

        Vector3 move = transform.right * x + transform.forward * z;

        //이동
        _characterController.Move(move * PlayerStats.MoveSpeed * Time.deltaTime);

        //점프
        if(Input.GetButtonDown("Jump") && _isGrounded)
        {
            Debug.Log("땅에 닿았으니 점프가능");
            _velocity.y = Mathf.Sqrt(PlayerStats.JumpPower * -2f * _gravity);
        }


        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q키 눌림");
            _currentWeaponIndex = (_currentWeaponIndex + 1) % Weapons.Count;
            _currnetWeapon = Weapons[_currentWeaponIndex];
            Debug.Log($"무기 변경: {_currnetWeapon.WeaponName}");
        }
    }
}
