using UnityEngine;

public interface IDamageAble
{
    void TakeDamage(Damage damage);

}

public class Damage
{
    public float Value;
    public GameObject from;
    public float KnockbackPower;
 
    public Damage(float value, GameObject from, float knockbackPower = 0)
    {
        Value = value;
        this.from = from;
        KnockbackPower = knockbackPower;
    }
}
