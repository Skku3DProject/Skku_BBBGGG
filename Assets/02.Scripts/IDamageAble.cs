using UnityEngine;

public interface IDamageAble
{
    void TakeDamage(Damage damage);

}

public class Damage
{
    public float Value;
    public GameObject From;
    public Vector3 Direction;
    public float KnockbackPower;

    public Damage(float value, GameObject from, float knockbackPower = 0, Vector3 direction = new Vector3())
    {
        Value = value;
        this.From = from;
        KnockbackPower = knockbackPower;
        Direction = direction;
    }
}
