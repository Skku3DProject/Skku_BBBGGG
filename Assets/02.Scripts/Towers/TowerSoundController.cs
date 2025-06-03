using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
public enum TowerSoundType
{
    ArrowShoot,
    FireballShoot,
    IceballShoot,
    Collapse,

}
public class TowerSoundController : MonoBehaviour
{
    public static TowerSoundController Instance;

    [Header("Audio Clips (Order must match enum)")]
    public AudioClip[] soundClips;

    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private int poolSize = 10;
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

        InitAudioSourcePool();
        //AudioListener 찾기
        _listener = Camera.main?.transform;
    }
    private void InitAudioSourcePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject audioObj = new GameObject($"PooledAudioSource_{i}");
            audioObj.transform.parent = this.transform;

            AudioSource aSource = audioObj.AddComponent<AudioSource>();
            aSource.spatialBlend = 1.0f;
            aSource.minDistance = 5f;
            aSource.maxDistance = 100f;
            aSource.rolloffMode = AudioRolloffMode.Linear;
            aSource.dopplerLevel = 0f;
            aSource.playOnAwake = false;
            aSource.loop = false;

            audioObj.SetActive(false);
            audioSourcePool.Add(aSource);
        }
    }
    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null; // 풀 부족 시 null 반환
    }
    private IEnumerator DisableAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
    }
    public void PlaySoundAt(TowerSoundType type, Vector3 worldPosition)
    {
        AudioSource aSource = GetAvailableAudioSource();
        int index = (int)type;

        if (aSource != null && index >= 0 && index < soundClips.Length && soundClips[index] != null)
        {
            aSource.transform.position = worldPosition;
            aSource.clip = soundClips[index];
            aSource.gameObject.SetActive(true);
            aSource.Play();
            StartCoroutine(DisableAfterPlay(aSource, soundClips[index].length));
        }
    }
}
