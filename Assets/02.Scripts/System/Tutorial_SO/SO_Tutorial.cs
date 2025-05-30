using UnityEngine;

[CreateAssetMenu(fileName = "SO_Tutorial", menuName = "Scriptable Objects/SO_Tutorial")]
public class SO_Tutorial : ScriptableObject
{
    public string MainTitle;         // 대제목
    public string SubTitle;          // 부제목
    public Currency currency;        // 튜토리얼 보상
    public TutorialType TutoType;
    public int RequireAmount;
    
}
