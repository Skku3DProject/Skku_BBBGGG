using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    private static TickManager _instance = null;
    private readonly List<ITickable> _tickables = new List<ITickable>();

    private int _nextIndex = 0;
    public float TickInterval = 0.1f;
    private float _timer = 0;
    private float _carryover = 0f;

    private Transform _player;

    private readonly List<ITickable> _nearTickables = new();
    private readonly List<ITickable> _midTickables = new();
    private readonly List<ITickable> _farTickables = new();

    private readonly int[] _lodInterval = { 1, 2, 5 };
    private int _tickCount = 0;


    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _player = GameObject.FindWithTag("Player").transform;
        ReclassifyAll();
    }

    private void ReclassifyAll()
    {
        _nearTickables.Clear();
        _midTickables.Clear();
        _farTickables.Clear();

        foreach (var t in _tickables)
        {
            float dist = Vector3.Distance(t.position, _player.position);
            if (dist <= t.LodDistance) _nearTickables.Add(t);
            else if (dist <= t.LodDistance * 2) _midTickables.Add(t);
            else _farTickables.Add(t);
        }
    }

    void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
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
        _tickCount++;

        // Near �׷�: �� ƽ
        if (_tickCount % _lodInterval[0] == 0)
            ProcessGroup(_nearTickables);

        // Mid �׷�: 2ƽ����
        if (_tickCount % _lodInterval[1] == 0)
            ProcessGroup(_midTickables);

        // Far �׷�: 5ƽ����
        if (_tickCount % _lodInterval[2] == 0)
            ProcessGroup(_farTickables);
    }

    private void ProcessGroup(List<ITickable> group)
    {
        int count = group.Count;
        if (count == 0) return;

        // �� ƽ�� ó���� ��ü �� ��� (��: ��ü ��ȯ 1ȸ)
        float ideal = count;
        _carryover += ideal;
        int batchSize = Mathf.FloorToInt(_carryover);
        _carryover -= batchSize;

        for (int i = 0; i < batchSize; i++)
        {
            group[_nextIndex % count].Tick();
            _nextIndex++;
        }
    }
}
