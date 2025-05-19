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

    private void Start()
    {
        _currnetWeapon = Weapons[_currentWeaponIndex];
        Debug.Log($"���� ���� : {_currnetWeapon.WeaponName}");


    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("QŰ ����");
            _currentWeaponIndex = (_currentWeaponIndex + 1) % Weapons.Count;
            _currnetWeapon = Weapons[_currentWeaponIndex];
            Debug.Log($"���� ����: {_currnetWeapon.WeaponName}");
        }
    }
}
