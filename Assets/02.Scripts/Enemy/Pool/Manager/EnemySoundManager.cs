using UnityEngine;
using System.Collections.Generic;

public class EnemySoundManager : MonoBehaviour
{
    public List<SoundContatiner> Sounds;
    private Dictionary<string,AudioClip> _audios;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // 3D 사운드 설정
        _audioSource.spatialBlend = 1.0f;
        _audioSource.minDistance = 2f;
        _audioSource.maxDistance = 20f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.dopplerLevel = 0f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        _audios = new Dictionary<string, AudioClip>(Sounds.Count);
        foreach (SoundContatiner sound in Sounds)
        {
            if (sound.AudioClip == null || sound.Key == null) continue;
            _audios.Add(sound.Key, sound.AudioClip);
        }
    }

    public void PlaySound(string key)
    {
        if(_audios.ContainsKey(key) == false)
        {
            return;
        }
        _audioSource.clip = _audios[key];
        _audioSource.Play();
    }
    
}
