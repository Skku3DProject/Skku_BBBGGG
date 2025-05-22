using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SkillNodeSO skillNode;
    public Image SkillIcon;
    public float Cooldown;
    
    // 변경되는 스킬 아이콘들
    public Image CooltimeSlider;
    public float Cooltimer;

    private void OnEnable()
    {
        
    }
    // 무기가 바뀌면 스킬 창에 위치한 스킬들이 변경된다.
    public void Setup(SkillNodeSO node)
    {
               
        SkillIcon.sprite = node.Icon;
        Cooldown = node.Cooldown;
        
    }
}
