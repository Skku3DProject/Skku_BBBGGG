using UnityEngine;
using System.Collections.Generic;
public enum TowerSoundType
{
    Place,
    Attack,
    Destroy
}
public class TowerSoundController : MonoBehaviour
{
    public static TowerSoundController Instance;

    [Header("Audio Clips (Order must match enum)")]
    public AudioClip[] soundClips;

    private AudioSource _audioSource;
    private Dictionary<TowerSoundType, AudioClip> _soundMap;

    private Transform _listener; // ī�޶� ��ġ

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _audioSource = GetComponent<AudioSource>();


        // 3D ���� ����
        _audioSource.spatialBlend = 1.0f;         // 3D ����
        _audioSource.minDistance = 5f;            // �����̼� 100% ����
        _audioSource.maxDistance = 30f;           // �� �Ÿ� �̻��̸� ���� ����
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.dopplerLevel = 0f;           // ���÷� ����
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        // AudioListener ã��
        _listener = Camera.main?.transform;
    }
    private void Update()
    {
        if (_listener == null) return;

        // �Ÿ� ��� �켱���� ����
        float dist = Vector3.Distance(transform.position, _listener.position);
        _audioSource.priority = Mathf.Clamp((int)(dist * 8), 0, 256);
    }

    public void PlaySound(TowerSoundType type)
    {
        int index = (int)type;
        if (index >= 0 && index < soundClips.Length && soundClips[index] != null)
        {
            _audioSource.PlayOneShot(soundClips[index]);
        }
    }
}
