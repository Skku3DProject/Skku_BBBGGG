using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    // WorldManager/BlockController 에서 16 단위로 나누고 있으므로 동일하게 맞춥니다.
    public const int chunkWidth = 16;
    public const int chunkHeight = 64;

    // X/Z 방향으로 +1씩 패딩이 필요하므로 총 +2
    public VoxelType[,,] blocks =
        new VoxelType[chunkWidth + 2, chunkHeight, chunkWidth + 2];

    MeshFilter mf;
    MeshCollider mc;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();

        // 이 청크의 월드 좌표를 그리드 인덱스로 변환해 등록
        int gx = Mathf.FloorToInt(transform.position.x / chunkWidth);
        int gz = Mathf.FloorToInt(transform.position.z / chunkWidth);
        Vector2Int coord = new Vector2Int(gx, gz);
        BlockController.RegisterChunk(coord, this);
    }

    public void BuildMesh()
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        // 실제 블록은 [1..chunkWidth] 범위
        for (int x = 1; x <= chunkWidth; x++)
            for (int z = 1; z <= chunkWidth; z++)
                for (int y = 0; y < chunkHeight; y++)
                {
                    var type = blocks[x, y, z];
                    if (type == VoxelType.Air) continue;

                    Vector3 bp = new Vector3(x - 1, y, z - 1);
                    int faceCount = 0;

                    // Top
                    if (y == chunkHeight - 1 || blocks[x, y + 1, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(0,1,0),
                    bp + new Vector3(0,1,1),
                    bp + new Vector3(1,1,1),
                    bp + new Vector3(1,1,0)
                });
                        uvs.AddRange(Voxel.blocks[type].topPos.GetUVs());
                        faceCount++;
                    }
                    // Bottom
                    if (y == 0 || blocks[x, y - 1, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(0,0,0),
                    bp + new Vector3(1,0,0),
                    bp + new Vector3(1,0,1),
                    bp + new Vector3(0,0,1)
                });
                        uvs.AddRange(Voxel.blocks[type].bottomPos.GetUVs());
                        faceCount++;
                    }
                    // Front (-Z)
                    if (blocks[x, y, z - 1] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(0,0,0),
                    bp + new Vector3(0,1,0),
                    bp + new Vector3(1,1,0),
                    bp + new Vector3(1,0,0)
                });
                        uvs.AddRange(Voxel.blocks[type].sidePos.GetUVs());
                        faceCount++;
                    }
                    // Back (+Z)
                    if (blocks[x, y, z + 1] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(1,0,1),
                    bp + new Vector3(1,1,1),
                    bp + new Vector3(0,1,1),
                    bp + new Vector3(0,0,1)
                });
                        uvs.AddRange(Voxel.blocks[type].sidePos.GetUVs());
                        faceCount++;
                    }
                    // Left (-X)
                    if (blocks[x - 1, y, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(0,0,1),
                    bp + new Vector3(0,1,1),
                    bp + new Vector3(0,1,0),
                    bp + new Vector3(0,0,0)
                });
                        uvs.AddRange(Voxel.blocks[type].sidePos.GetUVs());
                        faceCount++;
                    }
                    // Right (+X)
                    if (blocks[x + 1, y, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[]{
                    bp + new Vector3(1,0,0),
                    bp + new Vector3(1,1,0),
                    bp + new Vector3(1,1,1),
                    bp + new Vector3(1,0,1)
                });
                        uvs.AddRange(Voxel.blocks[type].sidePos.GetUVs());
                        faceCount++;
                    }

                    // 삼각형 인덱스 추가
                    int start = verts.Count - faceCount * 4;
                    for (int i = 0; i < faceCount; i++)
                    {
                        tris.AddRange(new[]{
                    start + i * 4,
                    start + i * 4 + 1,
                    start + i * 4 + 2,
                    start + i * 4,
                    start + i * 4 + 2,
                    start + i * 4 + 3
                });
                    }
                }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }


}
