using UnityEngine;

public class CollectTutorial : TutorialBase
{
    private float _collected;

    public CollectTutorial(SO_Tutorial data) : base(data)
    {
    }

    public override void OnProgress(TutorialType type, float amount)
    {
        if (type != _tutorial.TutoType)
        {
            return;
        }
        _collected += amount;
    }
    protected override float GetProgress() => _collected;

    public override void OnComplete()
    {
    }
}