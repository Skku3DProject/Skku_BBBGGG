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

    private Transform _listener; // 카메라 위치

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _audioSource = GetComponent<AudioSource>();


        // 3D 사운드 설정
        _audioSource.spatialBlend = 1.0f;         // 3D 사운드
        _audioSource.minDistance = 5f;            // 가까이선 100% 볼륨
        _audioSource.maxDistance = 30f;           // 이 거리 이상이면 거의 무음
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.dopplerLevel = 0f;           // 도플러 제거
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        // AudioListener 찾기
        _listener = Camera.main?.transform;
    }
    private void Update()
    {
        if (_listener == null) return;

        // 거리 기반 우선순위 설정
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
