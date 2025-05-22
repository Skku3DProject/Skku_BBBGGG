using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BlockOutliner : MonoBehaviour
{
    [Header("참조")]
    public Camera playerCamera;
    public LayerMask GroundLayer;
    private ThirdPersonPlayer _player;

    [Header("라인렌더러 세팅")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public float MaxDistance = 5f;

    private LineRenderer lr;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonPlayer>();

        var go = new GameObject("BlockHighlighterLine");
        lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.widthMultiplier = lineWidth;
        lr.positionCount = 5;
        lr.loop = true;
        lr.useWorldSpace = true;
    }

    void Update()
    {
        var mode = PlayerModeManager.Instance.CurrentMode;
        if (mode != EPlayerMode.Pickaxe && mode != EPlayerMode.Block)
        {
            lr.enabled = false;
            return;
        }

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f);
        var ray = playerCamera.ScreenPointToRay(screenCenter);
        if (!Physics.Raycast(ray, out var hit, 15f, GroundLayer))
        {
            lr.enabled = false;
            return;
        }

        Vector3Int block = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);

        if (!IsWithinReach(block))
        {
            lr.enabled = false;
            return;
        }

        Vector3 n = hit.normal;
        Vector3[] c = new Vector3[4];

        if (n == Vector3.up)
        {
            float y = block.y + 1f;
            c[0] = new Vector3(block.x, y, block.z);
            c[1] = new Vector3(block.x + 1, y, block.z);
            c[2] = new Vector3(block.x + 1, y, block.z + 1);
            c[3] = new Vector3(block.x, y, block.z + 1);
        }
        else if (n == Vector3.down)
        {
            float y = block.y;
            c[0] = new Vector3(block.x, y, block.z + 1);
            c[1] = new Vector3(block.x + 1, y, block.z + 1);
            c[2] = new Vector3(block.x + 1, y, block.z);
            c[3] = new Vector3(block.x, y, block.z);
        }
        else if (n == Vector3.forward)
        {
            float z = block.z + 1f;
            c[0] = new Vector3(block.x, block.y, z);
            c[1] = new Vector3(block.x + 1, block.y, z);
            c[2] = new Vector3(block.x + 1, block.y + 1, z);
            c[3] = new Vector3(block.x, block.y + 1, z);
        }
        else if (n == Vector3.back)
        {
            float z = block.z;
            c[0] = new Vector3(block.x + 1, block.y, z);
            c[1] = new Vector3(block.x, block.y, z);
            c[2] = new Vector3(block.x, block.y + 1, z);
            c[3] = new Vector3(block.x + 1, block.y + 1, z);
        }
        else if (n == Vector3.left)
        {
            float x = block.x;
            c[0] = new Vector3(x, block.y, block.z);
            c[1] = new Vector3(x, block.y, block.z + 1);
            c[2] = new Vector3(x, block.y + 1, block.z + 1);
            c[3] = new Vector3(x, block.y + 1, block.z);
        }
        else if (n == Vector3.right)
        {
            float x = block.x + 1f;
            c[0] = new Vector3(x, block.y, block.z + 1);
            c[1] = new Vector3(x, block.y, block.z);
            c[2] = new Vector3(x, block.y + 1, block.z);
            c[3] = new Vector3(x, block.y + 1, block.z + 1);
        }

        for (int i = 0; i < 4; i++)
            lr.SetPosition(i, c[i]);
        lr.SetPosition(4, c[0]);
        lr.enabled = true;
    }

    private bool IsWithinReach(Vector3Int blockPos)
    {
        Vector3 playerPos = _player.transform.position;
        float distance = Vector3.Distance(playerPos, blockPos + new Vector3(0.5f, 0.5f, 0.5f));
        return distance <= MaxDistance;
    }
}
