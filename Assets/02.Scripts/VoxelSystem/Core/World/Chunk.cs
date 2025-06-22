using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 64;

    public VoxelType[,,] Blocks = new VoxelType[CHUNK_WIDTH + 2, CHUNK_HEIGHT, CHUNK_WIDTH + 2];

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Vector2Int _coordinate;

    private List<Vector3> _vertices = new List<Vector3>(2048);
    private List<int> _triangles = new List<int>(6144);
    private List<Vector2> _uvs = new List<Vector2>(2048);

    private static readonly Vector3[][] FaceVertices = new Vector3[][]
    {
        // Top
        new Vector3[] { new Vector3(0,1,0), new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,1,0) },
        // Bottom  
        new Vector3[] { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(0,0,1) },
        // Front
        new Vector3[] { new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(1,0,0) },
        // Back 
        new Vector3[] { new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(0,1,1), new Vector3(0,0,1) },
        // Left 
        new Vector3[] { new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(0,1,0), new Vector3(0,0,0) },
        // Right 
        new Vector3[] { new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(1,0,1) }
    };

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        int gridX = Mathf.FloorToInt(transform.position.x / CHUNK_WIDTH);
        int gridZ = Mathf.FloorToInt(transform.position.z / CHUNK_WIDTH);
        _coordinate = new Vector2Int(gridX, gridZ);

        BlockManager.RegisterChunk(_coordinate, this);
    }

    private void OnDestroy()
    {
        BlockManager.UnregisterChunk(_coordinate);
    }

    // 외부에서 블록 설정
    public void SetBlock(Vector3Int localPos, VoxelType type, bool rebuildMesh = true)
    {
        Blocks[localPos.x, localPos.y, localPos.z] = type;

        if(rebuildMesh)
            BuildMesh();
    }

    public VoxelType GetBlock(Vector3Int localPos)
    {
        return Blocks[localPos.x, localPos.y, localPos.z];
    }

    public void BuildMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();

        for (int x = 1; x <= CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= CHUNK_WIDTH; z++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    var type = Blocks[x, y, z];
                    if (type == VoxelType.Air) continue;

                    Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                    var voxel = Voxel.blocks[type];

                    AddFaceIfVisible(0, blockPos, x, y + 1, z, voxel.TopPos);
                    AddFaceIfVisible(1, blockPos, x, y - 1, z, voxel.BottomPos);
                    AddFaceIfVisible(2, blockPos, x, y, z - 1, voxel.SidePos);
                    AddFaceIfVisible(3, blockPos, x, y, z + 1, voxel.SidePos);
                    AddFaceIfVisible(4, blockPos, x - 1, y, z, voxel.SidePos);
                    AddFaceIfVisible(5, blockPos, x + 1, y, z, voxel.SidePos);
                }
            }
        }

        if (_vertices.Count > 0)
        {
            var mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();

            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }
        else
        {
            _meshFilter.mesh = null;
            _meshCollider.sharedMesh = null;
        }
    }

    private void AddFaceIfVisible(int faceIndex, Vector3 blockPos, int checkX, int checkY, int checkZ, TilePos tilePos)
    {
        bool shouldRender = (checkY < 0 || checkY >= CHUNK_HEIGHT) ||
                           (checkX < 0 || checkX >= CHUNK_WIDTH + 2) ||
                           (checkZ < 0 || checkZ >= CHUNK_WIDTH + 2) ||
                           Blocks[checkX, checkY, checkZ] == VoxelType.Air;

        if (shouldRender)
        {
            var faceVerts = FaceVertices[faceIndex];
            int startIndex = _vertices.Count;

            for (int i = 0; i < 4; i++)
            {
                _vertices.Add(blockPos + faceVerts[i]);
            }

            _uvs.AddRange(tilePos.GetUVs());

            _triangles.AddRange(new int[]
            {
                startIndex, startIndex + 1, startIndex + 2,
                startIndex, startIndex + 2, startIndex + 3
            });
        }
    }
}
