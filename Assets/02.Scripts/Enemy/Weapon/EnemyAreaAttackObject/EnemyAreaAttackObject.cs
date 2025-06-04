using UnityEngine;

public class EnemyAreaAttackObject : MonoBehaviour, IEnemyPoolable
{
    public So_EnemyProjectile ProjectileData;
    private Damage _damage;

    private Collider[] hits = new Collider[100];

    public void OnSpawn()
    {
        CheckGroundHit();
    }
    public void OnDespawn()
    {

    }

    private void Update()
    {
        CheckGroundHit();
    }
    private void Awake()
    {
        if (ProjectileData != null)
        {
            _damage = new Damage(ProjectileData.Damage, this.gameObject, ProjectileData.KnockbackPower);
        }
        else
        {
            Debug.LogError($"ProjectileData is null on {gameObject.name}");
        }
    }


    private void CheckGroundHit()
    {
        if (ProjectileData == null) return;

        Vector3 down = Vector3.down;

        if (Physics.Raycast(transform.position, down, out RaycastHit hit, 10, ProjectileData.GroundMask))
        {
            if(hit.collider.TryGetComponent<IDamageAble>(out var damage))
            {
                damage.TakeDamage(_damage);
            }
            Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
            BlockSystem.DamageBlocksInRadius(blockPos, ProjectileData.AreaRange, (int)_damage.Value);
            EnemyParticlePoolManger.Instance.GetObject(ProjectileData.HitVfxKey, blockPos);
            EnemyObjectPoolManger.Instance.ReturnObject(ProjectileData.Key, gameObject);
        }
    }
}
