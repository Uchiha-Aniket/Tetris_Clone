using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public bool musicEnabled;
    [SerializeField, Range(0, 1)] private float _musicVolume = 1f;

    [Space]

    [SerializeField] private bool _fxEnabled;
    [SerializeField, Range(0, 1)] private float _fxVolume = 1f;

    [Header("Sound Fx")]
    public AudioClip _clearRowSound;
    public AudioClip _moveSound;
    public AudioClip _dropSound;
    public AudioClip _gameOverSound;
    public AudioClip _errorSound;
    public AudioClip _holdSound;

    [Header("Background Music")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip[] _backgroundMusics;

    [Header("Vocals")]
    public AudioClip gameOverVocal;
    public AudioClip levelUpVocal;
    [SerializeField] private AudioClip[] _clearRowVocals;

    [Space]
    [SerializeField] private IconToggle _musicToggle;
    [SerializeField] private IconToggle _fxToggle;

    private Camera _camera;

    // ----

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    // ----

    public void ToggleBackgroundMusic()
    {
        musicEnabled = !musicEnabled;
        UpdateBackgroundMusic();

        _musicToggle.Toggle(musicEnabled);
    }

    public void VolumeDownBackgroundMusic(float volume)
    {
        _musicSource.DOFade(volume, 1f);
    }

    private void UpdateBackgroundMusic()
    {
        if (_musicSource.volume == 0f) PlayBackgroundMusic();
        else _musicSource.DOFade(0, 1f);
    }

    private void PlayBackgroundMusic()
    {
        if (!musicEnabled || _backgroundMusics.Length <= 0 || !_musicSource) return;

        _musicSource.Stop();
        _musicSource.clip = GetRandomBackgroundMusicClip();
        //_musicSource.volume = _musicVolume;
        _musicSource.loop = true;
        _musicSource.Play();
        _musicSource.DOFade(_musicVolume, 1f).From(0);
    }

    private AudioClip GetRandomBackgroundMusicClip()
    {
        var index = Random.Range(0, _backgroundMusics.Length);
        return _backgroundMusics[index];
    }

    public void ToggleSoundFx()
    {
        _fxEnabled = !_fxEnabled;
        _fxToggle.Toggle(_fxEnabled);
    }

    public void PlaySoundFx(AudioClip clip, float volume = 1f)
    {
        if (_fxEnabled) 
        {
            volume = Mathf.Clamp(volume, .05f, 1f);
            AudioSource.PlayClipAtPoint(clip, _camera.transform.position, volume); 
        }
    }

    public void PlayRowClearingVocal()
    {
        if (!_fxEnabled) return;

        var index = Random.Range(0, _clearRowVocals.Length);
        PlaySoundFx(_clearRowVocals[index]);
    }
}
