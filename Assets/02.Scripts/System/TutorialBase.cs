using UnityEngine;

public abstract class TutorialBase
{
    protected SO_Tutorial _tutorial;

    public string TutorialID => _tutorial.TutorialID;
    public string Discription => _tutorial.Discription;
    public TutorilaType Type => _tutorial.TutoType;
    public Currency currency => _tutorial.currency;
    public bool IsCompleted => GetProgress() >= _tutorial.RequireAmount;
    
    public TutorialBase(SO_Tutorial data)
    {
        _tutorial= data;
    }

    public virtual void Start()
    {
        Debug.Log($"튜토리얼 시작: {_tutorial.Discription}");
    }

    public abstract void OnProgress(TutorilaType type, int amount);
    protected abstract int GetProgress();
    public abstract void OnComplete();
}
