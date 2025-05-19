using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TickManager : MonoBehaviour
{
    private static TickManager _instance;
    private readonly List<ITickable> _tickables = new List<ITickable>();

    private int _nextIndex = 0;
    public float TickInterval = 0.1f;
    private float _timer;

    void Awake()
    {
        _instance = this;
        if(_instance)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= TickInterval)
        {
            _timer -= TickInterval;
            DistributeTicks();
        }
    }

    public static void Register(ITickable tickable) => _instance._tickables.Add(tickable);
    public static void Unregister(ITickable tickable) => _instance._tickables.Remove(tickable);

    private void DistributeTicks()
    {
        int count = _tickables.Count;
        if (count == 0) return;

        // 예: 한 틱에 최대 10개 객체만 처리
        int batchSize = Mathf.CeilToInt(count * (TickInterval / (1f / Application.targetFrameRate)));
        for (int i = 0; i < batchSize; i++)
        {
            _tickables[_nextIndex].Tick();
            _nextIndex = (_nextIndex + 1) % count;
        }
    }
}
