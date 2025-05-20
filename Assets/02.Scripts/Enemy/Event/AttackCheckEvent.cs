using UnityEngine;

public class AttackCheckEvent : MonoBehaviour
{
    // �ٰŸ�

    public CapsuleCollider Collider;
    public GameObject Prefab;
    public void MeleeAttackEvent()
    {
        Collider.enabled = true;
    }

    // ���Ÿ�
    public void RangedAttackEvent()
    {
        Instantiate(Prefab);
    }

    public void EndAttackEvent()
    {
        Collider.enabled = false;
    }
    
}
