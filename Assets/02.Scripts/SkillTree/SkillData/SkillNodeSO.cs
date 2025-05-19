using UnityEngine;

[CreateAssetMenu(fileName = "SkillNodeSO", menuName = "Scriptable Objects/SkillNodeSO")]
public class SkillNodeSO : ScriptableObject
{
    public SkillType Type;
    public string Name;
    public int MaxLevel;
}
