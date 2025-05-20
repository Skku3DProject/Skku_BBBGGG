using UnityEngine;

public class AttackCheckEvent : MonoBehaviour
{
    // 근거리

    public CapsuleCollider Collider;
    public GameObject Prefab;
    public void MeleeAttackEvent()
    {
        Collider.enabled = true;
    }

    // 원거리
    public void RangedAttackEvent()
    {
        Instantiate(Prefab);
    }

    public void EndAttackEvent()
    {
        Collider.enabled = false;
    }
    
}
