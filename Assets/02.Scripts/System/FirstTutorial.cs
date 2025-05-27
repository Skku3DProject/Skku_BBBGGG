using UnityEngine;

public class FirstTutorial : TutorialBase
{
    private int _collected;

    public FirstTutorial(SO_Tutorial data) : base(data)
    {
    }

    public override void OnProgress(TutorilaType type, int amount)
    {
        if (type != _tutorial.TutoType)
        {
            return;
        }
        _collected += amount;
    }
    protected override int GetProgress() => _collected;

    public override void OnComplete()
    {
        Debug.Log($"{_tutorial.currency}");
    }
}