using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySo EnemyData;

    private GameObject _target;
    public GameObject Target => _target;

    private GameObject _player;
    public GameObject Player => _player;

    public CharacterController CharacterController;

    public Animator Animator;
    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    public void Initialize()
    {
        _target = GameObject.FindGameObjectWithTag("BaseTower");
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    public bool TryFindTarget()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < EnemyData.FindDistance)
        {
            return true;
        }
        return false;
    }

    public bool TryAttack()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < EnemyData.AttackDistance)
        {
            return true;
        }
        return false;
    }

}
