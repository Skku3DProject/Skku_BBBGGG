using UnityEngine;
public enum PlayerSoundType
{
    SwoardAttack,
    SwoardSKill1,
    SwoardSkill2,
    BowAttack,
    BowSkill1,
    BowSkill2Charge,
    BowSkill2Shoot,
    WandAttack,
    WandSkill1,
    WandSKill2,
    Block,
    FootStep,
    Jump,
    JumpLanding,
    Hit
}
public class PlayerSoundController : MonoBehaviour
{
    public static PlayerSoundController Instance;

    [Header("사운드 클립들 (enum 순서에 맞게)")]
    public AudioClip[] soundClips;
    public AudioClip[] bgmClips;

    private AudioSource _audioSource;
    private AudioSource _bgmSource;

    private Transform _listener;

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
        _audioSource.spatialBlend = 1.0f;
        _audioSource.minDistance = 2f;
        _audioSource.maxDistance = 20f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.dopplerLevel = 0f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        // BGM용 AudioSource 추가
        GameObject bgmObj = new GameObject("BGM Audio");
        bgmObj.transform.SetParent(transform);
        _bgmSource = bgmObj.AddComponent<AudioSource>();
        _bgmSource.spatialBlend = 0f; // 2D 사운드
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = 0.5f; // 필요시 조정


        _listener = Camera.main?.transform;
    }
    private void Start()
    {
        PlayerSoundController.Instance.PlayBGM(bgmClips[0], 0.4f);

    }

    private void Update()
    {
        if (_listener == null) return;

        float dist = Vector3.Distance(transform.position, _listener.position);
        _audioSource.priority = Mathf.Clamp((int)(dist * 8), 0, 256);
    }

    /// <summary>
    /// enum 기반 사운드 재생
    /// </summary>
    public void PlaySound(PlayerSoundType type)
    {
        int index = (int)type;
        if (index >= 0 && index < soundClips.Length && soundClips[index] != null)
        {
            _audioSource.PlayOneShot(soundClips[index]);
        }
    }

    /// <summary>
    /// 위치 기반 사운드 재생 (외부 사용 가능)
    /// </summary>
    public void PlaySoundAtPosition(PlayerSoundType type, Vector3 position)
    {
        int index = (int)type;
        if (index >= 0 && index < soundClips.Length && soundClips[index] != null)
        {
            AudioSource.PlayClipAtPoint(soundClips[index], position);
        }
    }

    /// <summary>
    /// 지정된 사운드를 루프로 계속 재생
    /// </summary>
    public void PlayLoopSound(PlayerSoundType type)
    {
        int index = (int)type;
        if (index >= 0 && index < soundClips.Length && soundClips[index] != null)
        {
            if (_audioSource.isPlaying && _audioSource.clip == soundClips[index]) return;

            _audioSource.clip = soundClips[index];
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    /// <summary>
    /// 현재 재생 중인 루프 사운드를 멈춤
    /// </summary>
    public void StopLoopSound()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.loop = false;
        }
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume = 0.2f)
    {
        if (clip == null) return;

        _bgmSource.clip = clip;
        _bgmSource.volume = volume;
        _bgmSource.Play();
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    /// <summary>
    /// 즉시 BGM 교체 및 재생
    /// </summary>
    public void ChangeBGM(AudioClip newClip, float volume = 0.2f)
    {
        if (newClip == null || _bgmSource == null) return;

        if (_bgmSource.isPlaying)
            _bgmSource.Stop();

        _bgmSource.clip = newClip;
        _bgmSource.volume = volume;
        _bgmSource.Play();
    }
}
