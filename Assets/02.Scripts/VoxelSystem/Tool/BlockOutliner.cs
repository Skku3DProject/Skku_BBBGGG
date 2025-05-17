using UnityEngine;

public class BlockOutliner : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("LineRenderer Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    private LineRenderer lr;

    void Start()
    {
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
        var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 100f))
        {
            lr.enabled = false;
            return;
        }

        // 블록 내부/외부 판단하는 기존 오프셋
        Vector3Int block = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
        Vector3 n = hit.normal;
        Vector3[] c = new Vector3[4];

        // 윗면
        if (n == Vector3.up)
        {
            float y = block.y + 1f;
            c[0] = new Vector3(block.x, y, block.z);
            c[1] = new Vector3(block.x + 1, y, block.z);
            c[2] = new Vector3(block.x + 1, y, block.z + 1);
            c[3] = new Vector3(block.x, y, block.z + 1);
        }
        // 아랫면
        else if (n == Vector3.down)
        {
            float y = block.y;
            c[0] = new Vector3(block.x, y, block.z + 1);
            c[1] = new Vector3(block.x + 1, y, block.z + 1);
            c[2] = new Vector3(block.x + 1, y, block.z);
            c[3] = new Vector3(block.x, y, block.z);
        }
        // 앞면(+Z)
        else if (n == Vector3.forward)
        {
            float z = block.z + 1f;
            c[0] = new Vector3(block.x, block.y, z);
            c[1] = new Vector3(block.x + 1, block.y, z);
            c[2] = new Vector3(block.x + 1, block.y + 1, z);
            c[3] = new Vector3(block.x, block.y + 1, z);
        }
        // 뒷면(-Z)
        else if (n == Vector3.back)
        {
            float z = block.z;
            c[0] = new Vector3(block.x + 1, block.y, z);
            c[1] = new Vector3(block.x, block.y, z);
            c[2] = new Vector3(block.x, block.y + 1, z);
            c[3] = new Vector3(block.x + 1, block.y + 1, z);
        }
        // 왼쪽(-X)
        else if (n == Vector3.left)
        {
            float x = block.x;
            c[0] = new Vector3(x, block.y, block.z);
            c[1] = new Vector3(x, block.y, block.z + 1);
            c[2] = new Vector3(x, block.y + 1, block.z + 1);
            c[3] = new Vector3(x, block.y + 1, block.z);
        }
        // 오른쪽(+X)
        else if (n == Vector3.right)
        {
            float x = block.x + 1f;
            c[0] = new Vector3(x, block.y, block.z + 1);
            c[1] = new Vector3(x, block.y, block.z);
            c[2] = new Vector3(x, block.y + 1, block.z);
            c[3] = new Vector3(x, block.y + 1, block.z + 1);
        }

        // LineRenderer에 찍기
        for (int i = 0; i < 4; i++)
            lr.SetPosition(i, c[i]);
        lr.SetPosition(4, c[0]);
        lr.enabled = true;
    }
}
