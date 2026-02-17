using UnityEngine;
using System.Collections.Generic;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Audio manager for background music (BGM) and sound effects (SFX).
    /// Singleton MonoBehaviour with DontDestroyOnLoad.
    /// Loads audio clips from Resources/Audio/SFX/ and Resources/Audio/BGM/.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _bgmVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _sfxVolume = 1f;
        [SerializeField] private bool _isMuted;

        // Cache for loaded audio clips
        private Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

        // Fade coroutine tracking
        private Coroutine _fadeCoroutine;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
                LoadSavedSettings();
                Debug.Log("[AudioManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetupAudioSources()
        {
            // Create BGM AudioSource if not assigned
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
            }
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _bgmSource.priority = 0; // Highest priority for music

            // Create SFX AudioSource if not assigned
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
            }
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
            _sfxSource.priority = 128;

            ApplyVolumes();
        }

        // ===============================
        // SFX
        // ===============================

        /// <summary>
        /// Play a sound effect by name. Loaded from Resources/Audio/SFX/{sfxName}.
        /// </summary>
        public void PlaySFX(string sfxName)
        {
            if (_isMuted || string.IsNullOrEmpty(sfxName)) return;

            AudioClip clip = LoadClip("Audio/SFX/" + sfxName);
            if (clip != null)
            {
                _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
            }
        }

        /// <summary>
        /// Play a sound effect directly from an AudioClip reference.
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (_isMuted || clip == null) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
        }

        // ===============================
        // BGM
        // ===============================

        /// <summary>
        /// Play background music by name. Loaded from Resources/Audio/BGM/{bgmName}.
        /// If the same track is already playing, does nothing.
        /// </summary>
        public void PlayBGM(string bgmName)
        {
            if (string.IsNullOrEmpty(bgmName)) return;

            AudioClip clip = LoadClip("Audio/BGM/" + bgmName);
            if (clip == null) return;

            // Don't restart if same track is already playing
            if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

            _bgmSource.clip = clip;
            _bgmSource.volume = _isMuted ? 0f : _bgmVolume * _masterVolume;
            _bgmSource.Play();
            Debug.Log($"[AudioManager] 🎵 Now playing: {bgmName}");
        }

        /// <summary>
        /// Play BGM with a fade-in effect.
        /// </summary>
        public void PlayBGMWithFade(string bgmName, float fadeDuration = 1f)
        {
            if (string.IsNullOrEmpty(bgmName)) return;

            AudioClip clip = LoadClip("Audio/BGM/" + bgmName);
            if (clip == null) return;

            if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _bgmSource.clip = clip;
            _bgmSource.volume = 0f;
            _bgmSource.Play();
            _fadeCoroutine = StartCoroutine(FadeVolume(_bgmSource, 0f, _bgmVolume * _masterVolume, fadeDuration));
        }

        /// <summary>
        /// Stop BGM immediately.
        /// </summary>
        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        /// <summary>
        /// Stop BGM with a fade-out effect.
        /// </summary>
        public void StopBGMWithFade(float fadeDuration = 1f)
        {
            if (!_bgmSource.isPlaying) return;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeVolumeAndStop(_bgmSource, _bgmSource.volume, 0f, fadeDuration));
        }

        /// <summary>
        /// Pause BGM (can be resumed).
        /// </summary>
        public void PauseBGM()
        {
            _bgmSource.Pause();
        }

        /// <summary>
        /// Resume paused BGM.
        /// </summary>
        public void ResumeBGM()
        {
            _bgmSource.UnPause();
        }

        // ===============================
        // Volume Controls
        // ===============================

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Mathf.Clamp01(value);
                ApplyVolumes();
                SaveSettings();
            }
        }

        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = Mathf.Clamp01(value);
                ApplyVolumes();
                SaveSettings();
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                ApplyVolumes();
                SaveSettings();
            }
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                ApplyVolumes();
                SaveSettings();
            }
        }

        /// <summary>
        /// Toggle mute on/off.
        /// </summary>
        public void ToggleMute()
        {
            IsMuted = !_isMuted;
        }

        private void ApplyVolumes()
        {
            if (_bgmSource != null)
            {
                _bgmSource.volume = _isMuted ? 0f : _bgmVolume * _masterVolume;
            }
            // SFX volume is applied per-play via PlayOneShot
        }

        // ===============================
        // Settings Persistence (PlayerPrefs)
        // ===============================

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("audio_master_volume", _masterVolume);
            PlayerPrefs.SetFloat("audio_bgm_volume", _bgmVolume);
            PlayerPrefs.SetFloat("audio_sfx_volume", _sfxVolume);
            PlayerPrefs.SetInt("audio_muted", _isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadSavedSettings()
        {
            _masterVolume = PlayerPrefs.GetFloat("audio_master_volume", 1f);
            _bgmVolume = PlayerPrefs.GetFloat("audio_bgm_volume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("audio_sfx_volume", 1f);
            _isMuted = PlayerPrefs.GetInt("audio_muted", 0) == 1;
            ApplyVolumes();
        }

        // ===============================
        // Audio Clip Loading & Cache
        // ===============================

        private AudioClip LoadClip(string resourcePath)
        {
            if (_clipCache.TryGetValue(resourcePath, out AudioClip cached))
            {
                return cached;
            }

            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] Audio clip not found: Resources/{resourcePath}");
                return null;
            }

            _clipCache[resourcePath] = clip;
            return clip;
        }

        /// <summary>
        /// Clear the audio clip cache to free memory.
        /// </summary>
        public void ClearCache()
        {
            _clipCache.Clear();
        }

        // ===============================
        // Fade Coroutines
        // ===============================

        private System.Collections.IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            source.volume = to;
            _fadeCoroutine = null;
        }

        private System.Collections.IEnumerator FadeVolumeAndStop(AudioSource source, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            source.volume = to;
            source.Stop();
            _fadeCoroutine = null;
        }
    }
}
