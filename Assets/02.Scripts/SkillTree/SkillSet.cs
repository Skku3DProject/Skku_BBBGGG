using UnityEngine;

public class SkillSet : MonoBehaviour
{
    public SkillNodeSO skillNode;

    public void OnClickLevelUp()
    {
        Debug.Log("OnClickLevelUp");
        SkillManager.instance.OnClickLevelUp(skillNode.Name);
    }
}
