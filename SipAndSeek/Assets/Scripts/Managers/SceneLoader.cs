using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Handles scene loading with optional transition effects and progress tracking.
    /// Singleton MonoBehaviour with DontDestroyOnLoad.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float _minimumLoadTime = 0.5f; // Minimum time to show loading

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired during async loading with progress (0-1).</summary>
        public static event Action<float> OnLoadProgress;

        /// <summary>Fired when a scene starts loading.</summary>
        public static event Action<string> OnSceneLoadStarted;

        /// <summary>Fired when a scene finishes loading.</summary>
        public static event Action<string> OnSceneLoadCompleted;

        // ===============================
        // State
        // ===============================
        private bool _isLoading;
        public bool IsLoading => _isLoading;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[SceneLoader] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ===============================
        // Scene Loading
        // ===============================

        /// <summary>
        /// Load a scene by name (synchronous). Use for small/fast scenes.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] Already loading a scene. Ignoring request.");
                return;
            }

            Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
            OnSceneLoadStarted?.Invoke(sceneName);

            SceneManager.LoadScene(sceneName);

            OnSceneLoadCompleted?.Invoke(sceneName);
        }

        /// <summary>
        /// Load a scene asynchronously with progress tracking.
        /// Ideal for gameplay scenes with loading screens.
        /// </summary>
        public void LoadSceneAsync(string sceneName, Action onComplete = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] Already loading a scene. Ignoring request.");
                return;
            }

            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
        }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action onComplete)
        {
            _isLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            Debug.Log($"[SceneLoader] Async loading scene: {sceneName}");

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            float elapsedTime = 0f;

            // Track progress until 90% (Unity loads to 0.9, then waits for activation)
            while (!asyncOperation.isDone)
            {
                elapsedTime += Time.unscaledDeltaTime;

                // Unity's progress goes from 0 to 0.9 during loading
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                OnLoadProgress?.Invoke(progress);

                // Activate scene when loading is done AND minimum time has passed
                if (asyncOperation.progress >= 0.9f && elapsedTime >= _minimumLoadTime)
                {
                    OnLoadProgress?.Invoke(1f);
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            _isLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            onComplete?.Invoke();

            Debug.Log($"[SceneLoader] ✅ Scene loaded: {sceneName}");
        }

        // ===============================
        // Convenience Methods
        // ===============================

        /// <summary>
        /// Load the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            LoadSceneAsync("MainMenu");
        }

        /// <summary>
        /// Load the gameplay scene.
        /// </summary>
        public void LoadGameplay()
        {
            LoadSceneAsync("Gameplay");
        }

        /// <summary>
        /// Reload the current active scene (for retry).
        /// </summary>
        public void ReloadCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadSceneAsync(currentScene);
        }

        /// <summary>
        /// Get the name of the currently active scene.
        /// </summary>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
