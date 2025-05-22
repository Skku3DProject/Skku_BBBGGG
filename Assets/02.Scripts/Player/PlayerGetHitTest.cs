using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class PlayerGetHitTest : MonoBehaviour
{
    private Enemy _enemy;
    public CapsuleCollider _capsuleCollider;

    private float _lastHitTime = -999f;
    private float _hitCooldown = 0.5f; // 0.5초마다 한 번만 충돌 처리

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - _lastHitTime < _hitCooldown)
            return; // 쿨타임 내에는 충돌 무시

        Debug.Log("오크의 무기가 무언가에 닿음: " + other.name);

        if (other.TryGetComponent<IDamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            dmg.TakeDamage(new Damage(_enemy.EnemyData.Power, _enemy.gameObject, _enemy.EnemyData.KnockbackPower));
            _lastHitTime = Time.time; // 마지막 충돌 시간 갱신
        }
    }


}
