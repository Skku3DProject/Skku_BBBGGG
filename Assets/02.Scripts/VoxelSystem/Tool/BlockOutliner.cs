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

        // ��� ����/�ܺ� �Ǵ��ϴ� ���� ������
        Vector3Int block = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
        Vector3 n = hit.normal;
        Vector3[] c = new Vector3[4];

        // ����
        if (n == Vector3.up)
        {
            float y = block.y + 1f;
            c[0] = new Vector3(block.x, y, block.z);
            c[1] = new Vector3(block.x + 1, y, block.z);
            c[2] = new Vector3(block.x + 1, y, block.z + 1);
            c[3] = new Vector3(block.x, y, block.z + 1);
        }
        // �Ʒ���
        else if (n == Vector3.down)
        {
            float y = block.y;
            c[0] = new Vector3(block.x, y, block.z + 1);
            c[1] = new Vector3(block.x + 1, y, block.z + 1);
            c[2] = new Vector3(block.x + 1, y, block.z);
            c[3] = new Vector3(block.x, y, block.z);
        }
        // �ո�(+Z)
        else if (n == Vector3.forward)
        {
            float z = block.z + 1f;
            c[0] = new Vector3(block.x, block.y, z);
            c[1] = new Vector3(block.x + 1, block.y, z);
            c[2] = new Vector3(block.x + 1, block.y + 1, z);
            c[3] = new Vector3(block.x, block.y + 1, z);
        }
        // �޸�(-Z)
        else if (n == Vector3.back)
        {
            float z = block.z;
            c[0] = new Vector3(block.x + 1, block.y, z);
            c[1] = new Vector3(block.x, block.y, z);
            c[2] = new Vector3(block.x, block.y + 1, z);
            c[3] = new Vector3(block.x + 1, block.y + 1, z);
        }
        // ����(-X)
        else if (n == Vector3.left)
        {
            float x = block.x;
            c[0] = new Vector3(x, block.y, block.z);
            c[1] = new Vector3(x, block.y, block.z + 1);
            c[2] = new Vector3(x, block.y + 1, block.z + 1);
            c[3] = new Vector3(x, block.y + 1, block.z);
        }
        // ������(+X)
        else if (n == Vector3.right)
        {
            float x = block.x + 1f;
            c[0] = new Vector3(x, block.y, block.z + 1);
            c[1] = new Vector3(x, block.y, block.z);
            c[2] = new Vector3(x, block.y + 1, block.z);
            c[3] = new Vector3(x, block.y + 1, block.z + 1);
        }

        // LineRenderer�� ���
        for (int i = 0; i < 4; i++)
            lr.SetPosition(i, c[i]);
        lr.SetPosition(4, c[0]);
        lr.enabled = true;
    }
}
