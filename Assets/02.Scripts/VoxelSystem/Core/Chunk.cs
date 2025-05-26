using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 64;

    // X/Z 방향으로 +1씩 패딩이 필요하므로 총 +2
    public VoxelType[,,] Blocks = new VoxelType[CHUNK_WIDTH + 2, CHUNK_HEIGHT, CHUNK_WIDTH + 2];

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        int gridX = Mathf.FloorToInt(transform.position.x / CHUNK_WIDTH);
        int gridZ = Mathf.FloorToInt(transform.position.z / CHUNK_WIDTH);
        var coord = new Vector2Int(gridX, gridZ);
        BlockSystem.RegisterChunk(coord, this);
    }

    public void BuildMesh()
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        // 실제 블록은 [1..CHUNK_WIDTH] 범위
        for (int x = 1; x <= CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= CHUNK_WIDTH; z++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    var type = Blocks[x, y, z];
                    if (type == VoxelType.Air) continue;

                    Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                    int faceCount = 0;

                    // Top
                    if (y == CHUNK_HEIGHT - 1 || Blocks[x, y + 1, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(0,1,0),
                            blockPos + new Vector3(0,1,1),
                            blockPos + new Vector3(1,1,1),
                            blockPos + new Vector3(1,1,0)
                        });
                        uvs.AddRange(Voxel.blocks[type].TopPos.GetUVs());
                        faceCount++;
                    }
                    // Bottom
                    if (y == 0 || Blocks[x, y - 1, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(0,0,0),
                            blockPos + new Vector3(1,0,0),
                            blockPos + new Vector3(1,0,1),
                            blockPos + new Vector3(0,0,1)
                        });
                        uvs.AddRange(Voxel.blocks[type].BottomPos.GetUVs());
                        faceCount++;
                    }
                    // Front (-Z)
                    if (Blocks[x, y, z - 1] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(0,0,0),
                            blockPos + new Vector3(0,1,0),
                            blockPos + new Vector3(1,1,0),
                            blockPos + new Vector3(1,0,0)
                        });
                        uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
                        faceCount++;
                    }
                    // Back (+Z)
                    if (Blocks[x, y, z + 1] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(1,0,1),
                            blockPos + new Vector3(1,1,1),
                            blockPos + new Vector3(0,1,1),
                            blockPos + new Vector3(0,0,1)
                        });
                        uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
                        faceCount++;
                    }
                    // Left (-X)
                    if (Blocks[x - 1, y, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(0,0,1),
                            blockPos + new Vector3(0,1,1),
                            blockPos + new Vector3(0,1,0),
                            blockPos + new Vector3(0,0,0)
                        });
                        uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
                        faceCount++;
                    }
                    // Right (+X)
                    if (Blocks[x + 1, y, z] == VoxelType.Air)
                    {
                        verts.AddRange(new[] {
                            blockPos + new Vector3(1,0,0),
                            blockPos + new Vector3(1,1,0),
                            blockPos + new Vector3(1,1,1),
                            blockPos + new Vector3(1,0,1)
                        });
                        uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
                        faceCount++;
                    }

                    // Triangles
                    int startIndex = verts.Count - faceCount * 4;
                    for (int i = 0; i < faceCount; i++)
                    {
                        tris.AddRange(new[] {
                            startIndex + i * 4,
                            startIndex + i * 4 + 1,
                            startIndex + i * 4 + 2,
                            startIndex + i * 4,
                            startIndex + i * 4 + 2,
                            startIndex + i * 4 + 3
                        });
                    }
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    //public void BuildMesh()
    //{
    //    var mesh = new Mesh();
    //    var verts = new List<Vector3>();
    //    var tris = new List<int>();
    //    var uvs = new List<Vector2>();

    //    for (int x = 1; x <= CHUNK_WIDTH; x++)
    //    {
    //        for (int z = 1; z <= CHUNK_WIDTH; z++)
    //        {
    //            for (int y = 0; y < CHUNK_HEIGHT; y++)
    //            {
    //                var type = Blocks[x, y, z];
    //                if (type == VoxelType.Air)
    //                    continue;

    //                Vector3 blockPos = new Vector3(x - 1, y, z - 1);
    //                int faceCount = 0;

    //                // Top
    //                if (y == CHUNK_HEIGHT - 1 || Blocks[x, y + 1, z] == VoxelType.Air)
    //                {
    //                    verts.AddRange(new[] {
    //                    blockPos + new Vector3(0,1,0),
    //                    blockPos + new Vector3(0,1,1),
    //                    blockPos + new Vector3(1,1,1),
    //                    blockPos + new Vector3(1,1,0)
    //                });
    //                    uvs.AddRange(Voxel.blocks[type].TopPos.GetUVs());
    //                    faceCount++;
    //                }

    //                // Side 면 (경계 제외 & 옆이 Air일 때만)
    //                // Front (-Z)
    //                if (z > 1 && Blocks[x, y, z - 1] == VoxelType.Air)
    //                {
    //                    verts.AddRange(new[] {
    //                    blockPos + new Vector3(0,0,0),
    //                    blockPos + new Vector3(0,1,0),
    //                    blockPos + new Vector3(1,1,0),
    //                    blockPos + new Vector3(1,0,0)
    //                });
    //                    uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
    //                    faceCount++;
    //                }

    //                // Back (+Z)
    //                if (z < CHUNK_WIDTH && Blocks[x, y, z + 1] == VoxelType.Air)
    //                {
    //                    verts.AddRange(new[] {
    //                    blockPos + new Vector3(1,0,1),
    //                    blockPos + new Vector3(1,1,1),
    //                    blockPos + new Vector3(0,1,1),
    //                    blockPos + new Vector3(0,0,1)
    //                });
    //                    uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
    //                    faceCount++;
    //                }

    //                // Left (-X)
    //                if (x > 1 && Blocks[x - 1, y, z] == VoxelType.Air)
    //                {
    //                    verts.AddRange(new[] {
    //                    blockPos + new Vector3(0,0,1),
    //                    blockPos + new Vector3(0,1,1),
    //                    blockPos + new Vector3(0,1,0),
    //                    blockPos + new Vector3(0,0,0)
    //                });
    //                    uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
    //                    faceCount++;
    //                }

    //                // Right (+X)
    //                if (x < CHUNK_WIDTH && Blocks[x + 1, y, z] == VoxelType.Air)
    //                {
    //                    verts.AddRange(new[] {
    //                    blockPos + new Vector3(1,0,0),
    //                    blockPos + new Vector3(1,1,0),
    //                    blockPos + new Vector3(1,1,1),
    //                    blockPos + new Vector3(1,0,1)
    //                });
    //                    uvs.AddRange(Voxel.blocks[type].SidePos.GetUVs());
    //                    faceCount++;
    //                }

    //                // Triangles
    //                int startIndex = verts.Count - faceCount * 4;
    //                for (int i = 0; i < faceCount; i++)
    //                {
    //                    tris.AddRange(new[] {
    //                    startIndex + i * 4,
    //                    startIndex + i * 4 + 1,
    //                    startIndex + i * 4 + 2,
    //                    startIndex + i * 4,
    //                    startIndex + i * 4 + 2,
    //                    startIndex + i * 4 + 3
    //                });
    //                }
    //            }
    //        }
    //    }

    //    mesh.vertices = verts.ToArray();
    //    mesh.triangles = tris.ToArray();
    //    mesh.uv = uvs.ToArray();
    //    mesh.RecalculateNormals();

    //    _meshFilter.mesh = mesh;
    //    _meshCollider.sharedMesh = mesh;
    //}
}
