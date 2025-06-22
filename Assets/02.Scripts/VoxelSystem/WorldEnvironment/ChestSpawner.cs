using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public static ChestSpawner Instance { get; private set; }

    [Header("보물상자 설정")]
    public GameObject ChestPrefab;
    public int MaxChestCount = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlaceChests(List<Vector3> hillPositions, Transform parent)
    {
        int chestCount = Mathf.Min(MaxChestCount, hillPositions.Count);
        if (chestCount == 0 || ChestPrefab == null) return;

        var shuffledPositions = new List<Vector3>(hillPositions);
        ShuffleList(shuffledPositions);

        for (int i = 0; i < chestCount; i++)
        {
            Instantiate(ChestPrefab, shuffledPositions[i], Quaternion.identity, parent);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
