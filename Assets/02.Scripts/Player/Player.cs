using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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

    private void Start()
    {
        _currnetWeapon = Weapons[_currentWeaponIndex];
        Debug.Log($"현재 무기 : {_currnetWeapon.WeaponName}");


    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q키 눌림");
            _currentWeaponIndex = (_currentWeaponIndex + 1) % Weapons.Count;
            _currnetWeapon = Weapons[_currentWeaponIndex];
            Debug.Log($"무기 변경: {_currnetWeapon.WeaponName}");
        }
    }
}
