using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scriptable Objects/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    public float MaxHealth;
    public float MoveSpeed;
    public float JumpPower;
    public float Stamina;
    public float Exp;
    public int Level;

}
