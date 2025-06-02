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

    [Header("���� Ŭ���� (enum ������ �°�)")]
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

        // 3D ���� ����
        _audioSource.spatialBlend = 1.0f;
        _audioSource.minDistance = 2f;
        _audioSource.maxDistance = 20f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.dopplerLevel = 0f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        // BGM�� AudioSource �߰�
        GameObject bgmObj = new GameObject("BGM Audio");
        bgmObj.transform.SetParent(transform);
        _bgmSource = bgmObj.AddComponent<AudioSource>();
        _bgmSource.spatialBlend = 0f; // 2D ����
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = 0.5f; // �ʿ�� ����


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
    /// enum ��� ���� ���
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
    /// ��ġ ��� ���� ��� (�ܺ� ��� ����)
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
    /// ������ ���带 ������ ��� ���
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
    /// ���� ��� ���� ���� ���带 ����
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
    /// BGM ���
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume = 0.2f)
    {
        if (clip == null) return;

        _bgmSource.clip = clip;
        _bgmSource.volume = volume;
        _bgmSource.Play();
    }

    /// <summary>
    /// BGM ����
    /// </summary>
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    /// <summary>
    /// ��� BGM ��ü �� ���
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
