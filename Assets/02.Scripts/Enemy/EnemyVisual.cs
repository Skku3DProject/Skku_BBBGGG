using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private Renderer[] _allRenderers;
    private MaterialPropertyBlock _propBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor"); // URP용 컬러 ID
    private Color _originalColor = Color.white;

    private void Awake()
    {
        _allRenderers =  GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        // 첫 번째 유효한 머티리얼에서 초기 색상 저장
        foreach (var r in _allRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            if (r.sharedMaterial.HasProperty(EmissionColorID))
            {
                _originalColor = r.sharedMaterial.GetColor(EmissionColorID);
               
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
            if (r.sharedMaterial.HasProperty(EmissionColorID))
            {
                _propBlock.SetColor(EmissionColorID, Color.white);
                r.SetPropertyBlock(_propBlock);
                Debug.Log(_propBlock);
            }
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var r in _allRenderers)
        {
            r.GetPropertyBlock(_propBlock);
            if (r.sharedMaterial.HasProperty(EmissionColorID))
            {
                _propBlock.SetColor(EmissionColorID, _originalColor);
                r.SetPropertyBlock(_propBlock);
            }
        }
    }
}
