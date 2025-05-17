using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;            // Ŭ���� ����� ī�޶�. ������ Transform.forward ���
    public LayerMask chunkLayer;           // ûũ Collider �� ���Ե� ���̾�
    public float maxDistance = 4f;         // Raycast �ִ� �Ÿ�

    [Header("Voxel Settings")]
    public VoxelType placeType = VoxelType.Grass;  // ��ġ�� �� Ÿ��

    // ûũ ��ǥ �� Chunk �ν��Ͻ� ��
    private static Dictionary<Vector2Int, Chunk> registeredChunks
        = new Dictionary<Vector2Int, Chunk>();

    void Update()
    {
        // ��Ŭ��(��ġ) �Ǵ� ��Ŭ��(�ı�) üũ
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (!leftClick && !rightClick) return;

        // Ray ���� (ī�޶� ������ ������ ��ũ�� ����, �ƴϸ� �� ����)
        Ray ray = playerCamera != null
            ? playerCamera.ScreenPointToRay(Input.mousePosition)
            : new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, chunkLayer))
            return;

        // Ŭ���� ���� ���� �������� ���� �ణ ������
        bool placing = leftClick;
        Vector3 offset = hit.normal * (placing ? 0.5f : -0.5f);
        Vector3 adjustedPoint = hit.point + offset;

        // ���� �� ��� ��ǥ (����)
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedPoint);

        // ûũ �׸��� �ε��� ���
        int cx = Mathf.FloorToInt(blockPos.x / (float)Chunk.chunkWidth);
        int cz = Mathf.FloorToInt(blockPos.z / (float)Chunk.chunkWidth);
        Vector2Int chunkCoord = new Vector2Int(cx, cz);

        if (!registeredChunks.TryGetValue(chunkCoord, out Chunk chunk))
            return;

        // �е�(+1) ó���� ��� �迭 �� ���� ��ǥ
        int localX = blockPos.x - cx * Chunk.chunkWidth + 1;
        int localY = blockPos.y;
        int localZ = blockPos.z - cz * Chunk.chunkWidth + 1;

        // ���� �˻�
        if (localX < 0 || localX >= Chunk.chunkWidth + 2 ||
            localY < 0 || localY >= Chunk.chunkHeight ||
            localZ < 0 || localZ >= Chunk.chunkWidth + 2)
            return;

        // ��ġ / ����
        chunk.blocks[localX, localY, localZ] = placing
            ? placeType
            : VoxelType.Air;

        // �޽� ����
        chunk.BuildMesh();
    }

    // WorldManager.CreateChunk ���� ȣ���ؼ� �ʿ� ���
    public static void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        registeredChunks[coord] = chunk;
    }

    public static void ClearRegistry()
    {
        registeredChunks.Clear();
    }
}
