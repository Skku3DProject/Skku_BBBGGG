using UnityEngine;

[CreateAssetMenu(fileName = "SO_Tutorial", menuName = "Scriptable Objects/SO_Tutorial")]
public class SO_Tutorial : ScriptableObject
{
    public string TutorialID;
    public string Discription;       // 튜토리얼 내용 소개
    public Currency currency;        // 튜토리얼 보상
    public TutorilaType TutoType;
    public int RequireReward;
    public int RequireAmount;
    
}
