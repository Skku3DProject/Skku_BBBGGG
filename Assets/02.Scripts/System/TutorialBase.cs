using UnityEngine;

public abstract class TutorialBase
{
    protected SO_Tutorial _tutorial;
    public string MainTitle => _tutorial.MainTitle;
    public string SubTitle => _tutorial.SubTitle;
    public TutorialType Type => _tutorial.TutoType;
    public Currency currency => _tutorial.currency;
    public bool IsCompleted => GetProgress() >= _tutorial.RequireAmount;
    public int RequiredAmount => _tutorial.RequireAmount;
    public TutorialBase(SO_Tutorial data)
    {
        _tutorial= data;
    }

    public virtual void Start()
    {
    }

    public abstract void OnProgress(TutorialType type, float amount);
    protected abstract float GetProgress();
    public abstract void OnComplete();
}
