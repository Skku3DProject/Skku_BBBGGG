using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class PlayerGetHitTest : MonoBehaviour
{
    private Enemy _enemy;
    public CapsuleCollider _capsuleCollider;

    private float _lastHitTime = -999f;
    private float _hitCooldown = 0.5f; // 0.5�ʸ��� �� ���� �浹 ó��

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - _lastHitTime < _hitCooldown)
            return; // ��Ÿ�� ������ �浹 ����

        Debug.Log("��ũ�� ���Ⱑ ���𰡿� ����: " + other.name);

        if (other.TryGetComponent<IDamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            dmg.TakeDamage(new Damage(_enemy.EnemyData.Power, _enemy.gameObject, _enemy.EnemyData.KnockbackPower));
            _lastHitTime = Time.time; // ������ �浹 �ð� ����
        }
    }


}
