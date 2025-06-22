using System.Collections.Generic;
using UnityEngine;

public class BlockHealthMap
{
    private readonly Dictionary<Vector3Int, int> _healthMap = new();

    public void SetHealth(Vector3Int pos, int hp)
    {
        _healthMap[pos] = hp;
    }

    public bool HasHealth(Vector3Int pos)
    {
        return _healthMap.ContainsKey(pos);
    }

    //데미지를 가하고 파괴여부를 반환
    public bool TryDestroyAfterDamage(Vector3Int pos, int dmg)
    {
        if (!_healthMap.TryGetValue(pos, out var hp))
            return false;

        hp -= dmg;
        if (hp <= 0)
        {
            _healthMap.Remove(pos);
            return true;
        }

        _healthMap[pos] = hp;
        return false;
    }

    public int GetHealth(Vector3Int pos)
    {
        return _healthMap.TryGetValue(pos, out var hp) ? hp : 0;
    }

    public void Clear()
    {
        _healthMap.Clear();
    }
}
