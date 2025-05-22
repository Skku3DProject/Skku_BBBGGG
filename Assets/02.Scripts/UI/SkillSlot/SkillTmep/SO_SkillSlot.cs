using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SO_SkillSlot", menuName = "Scriptable Objects/SO_SkillSlot")]
public class SO_SkillSlot : ScriptableObject
{
    public EquipmentType EEquipmentType;
    public float CoolDown;
    public Sprite Icon;
}
