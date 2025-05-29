using System.Collections;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private Renderer[] _allRenderers;
    private MaterialPropertyBlock _propBlock;
    private static readonly int _ColorID = Shader.PropertyToID("_RimColor"); // URP�� �÷� ID
    private Color _originalColor = Color.black;
    private Color _hitColor = Color.white;
    public string ColorHexadecimal;

    private void Awake()
    {
        // FF5858
        _allRenderers = GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();

        // �ڵ�� ����������
        //  ColorUtility.TryParseHtmlString(ColorHexadecimal, out _hitColor);
        // ù ��° ��ȿ�� ��Ƽ���󿡼� �ʱ� ���� ����
        foreach (var r in _allRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            if (r.sharedMaterial.HasProperty(_ColorID))
            {
                _originalColor = r.sharedMaterial.GetColor(_ColorID);

                break;
            }
        }
    }

    public void PlayHitFeedback(float waitTime)
    {
        StartCoroutine(HitColorFeedback(waitTime));
    }

    private IEnumerator HitColorFeedback(float waitTime)
    {
        foreach (var r in _allRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            if (r.sharedMaterial.HasProperty(_ColorID))
            {
                _propBlock.SetColor(_ColorID, _hitColor);
                r.SetPropertyBlock(_propBlock);
            }
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var r in _allRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            if (r.sharedMaterial.HasProperty(_ColorID))
            {
                _propBlock.SetColor(_ColorID, _originalColor);
                r.SetPropertyBlock(_propBlock);
            }
        }
    }
}
