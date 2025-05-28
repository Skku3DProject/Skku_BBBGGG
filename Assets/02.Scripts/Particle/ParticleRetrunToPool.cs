using UnityEngine;

public class ParticleRetrunToPool : MonoBehaviour
{
    private ParticleSystem[] _particles;

    void Awake()
    {
        _particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
    }

    void OnEnable()
    {
        foreach (var ps in _particles)
        {
            ps.Play();
        }

        StartCoroutine(CheckIfAllStopped());
    }

    private System.Collections.IEnumerator CheckIfAllStopped()
    {
        // ��� ��ƼŬ�� ���� ������ ���
        while (true)
        {
            bool allDead = true;

            foreach (var ps in _particles)
            {
                if (ps.IsAlive(true)) // �ڽ� ���� true
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
                break;

            yield return null;
        }

        ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
