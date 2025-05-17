using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;            // 클릭에 사용할 카메라. 없으면 Transform.forward 사용
    public LayerMask chunkLayer;           // 청크 Collider 만 포함된 레이어
    public float maxDistance = 4f;         // Raycast 최대 거리

    [Header("Voxel Settings")]
    public VoxelType placeType = VoxelType.Grass;  // 설치할 블럭 타입

    // 청크 좌표 → Chunk 인스턴스 맵
    private static Dictionary<Vector2Int, Chunk> registeredChunks
        = new Dictionary<Vector2Int, Chunk>();

    void Update()
    {
        // 좌클릭(설치) 또는 우클릭(파괴) 체크
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (!leftClick && !rightClick) return;

        // Ray 생성 (카메라가 지정돼 있으면 스크린 레이, 아니면 앞 방향)
        Ray ray = playerCamera != null
            ? playerCamera.ScreenPointToRay(Input.mousePosition)
            : new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, chunkLayer))
            return;

        // 클릭한 면의 법선 기준으로 아주 약간 오프셋
        bool placing = leftClick;
        Vector3 offset = hit.normal * (placing ? 0.5f : -0.5f);
        Vector3 adjustedPoint = hit.point + offset;

        // 월드 → 블록 좌표 (정수)
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedPoint);

        // 청크 그리드 인덱스 계산
        int cx = Mathf.FloorToInt(blockPos.x / (float)Chunk.chunkWidth);
        int cz = Mathf.FloorToInt(blockPos.z / (float)Chunk.chunkWidth);
        Vector2Int chunkCoord = new Vector2Int(cx, cz);

        if (!registeredChunks.TryGetValue(chunkCoord, out Chunk chunk))
            return;

        // 패딩(+1) 처리된 블록 배열 내 로컬 좌표
        int localX = blockPos.x - cx * Chunk.chunkWidth + 1;
        int localY = blockPos.y;
        int localZ = blockPos.z - cz * Chunk.chunkWidth + 1;

        // 범위 검사
        if (localX < 0 || localX >= Chunk.chunkWidth + 2 ||
            localY < 0 || localY >= Chunk.chunkHeight ||
            localZ < 0 || localZ >= Chunk.chunkWidth + 2)
            return;

        // 설치 / 제거
        chunk.blocks[localX, localY, localZ] = placing
            ? placeType
            : VoxelType.Air;

        // 메쉬 갱신
        chunk.BuildMesh();
    }

    // WorldManager.CreateChunk 에서 호출해서 맵에 등록
    public static void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        registeredChunks[coord] = chunk;
    }

    public static void ClearRegistry()
    {
        registeredChunks.Clear();
    }
}
