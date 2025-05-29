using System.Collections;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private Renderer[] _allRenderers;
    private MaterialPropertyBlock _propBlock;
    private static readonly int _ColorID = Shader.PropertyToID("_RimColor"); // URP용 컬러 ID
    private Color _originalColor = Color.black;
    private Color _hitColor = Color.white;
    public string ColorHexadecimal;

    private void Awake()
    {
        // FF5858
        _allRenderers = GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();

        // 코드로 색가져오기
        //  ColorUtility.TryParseHtmlString(ColorHexadecimal, out _hitColor);
        // 첫 번째 유효한 머티리얼에서 초기 색상 저장
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
