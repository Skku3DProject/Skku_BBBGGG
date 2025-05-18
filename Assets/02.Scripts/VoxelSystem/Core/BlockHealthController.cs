using System.Collections.Generic;
using UnityEngine;

public class BlockHealthController : MonoBehaviour
{
    private static Dictionary<Vector3Int, int> _healthMap = new Dictionary<Vector3Int, int>();

    public static void SetHealth(Vector3Int pos, int hp)
    {
        _healthMap[pos] = hp;
    }
    public static bool HasHealth(Vector3Int pos)
    {
        return _healthMap.ContainsKey(pos);
    }
    public static bool Damage(Vector3Int pos, int dmg)
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

    public static int GetHealth(Vector3Int pos)
    {
        return _healthMap.TryGetValue(pos, out var hp) ? hp : 0;
    }
}
