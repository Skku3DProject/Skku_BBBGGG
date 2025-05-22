using UnityEngine;
[CreateAssetMenu(fileName = "SO_SkillSlot", menuName = "Scriptable Objects/SO_SkillSlot")]
public class SO_TempSkillSlot : ScriptableObject
{
    public EquipmentType EEquipmentType;
    public float CoolDown;
    public Sprite Icon;
}
