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

    private AudioSource _audioSource;
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

        _listener = Camera.main?.transform;
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
}
