using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages daily challenge generation, progress tracking, and reward claiming.
    /// Picks 3 random challenges from GameDatabase each day.
    /// Progress is stored in PlayerPrefs for simplicity.
    /// </summary>
    public class DailyChallengeManager : MonoBehaviour
    {
        public static DailyChallengeManager Instance { get; private set; }

        private const int CHALLENGES_PER_DAY = 3;
        private const string PREFS_DATE_KEY = "daily_challenge_date";
        private const string PREFS_IDS_KEY = "daily_challenge_ids";
        private const string PREFS_PROGRESS_PREFIX = "dc_progress_";
        private const string PREFS_CLAIMED_PREFIX = "dc_claimed_";

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a challenge is completed (challengeId).</summary>
        public static event Action<string> OnChallengeCompleted;

        /// <summary>Fired when challenge progress is updated (challengeId, current, target).</summary>
        public static event Action<string, int, int> OnChallengeProgress;

        /// <summary>Fired when daily challenges are refreshed.</summary>
        public static event Action OnChallengesRefreshed;

        // Current day's challenges
        private List<DailyChallengeData> _todayChallenges = new List<DailyChallengeData>();
        public List<DailyChallengeData> TodayChallenges => _todayChallenges;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[DailyChallengeManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            RefreshIfNewDay();
        }

        // ===============================
        // Daily Refresh
        // ===============================

        /// <summary>
        /// Check if a new day has started and refresh challenges if needed.
        /// </summary>
        public void RefreshIfNewDay()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string savedDate = PlayerPrefs.GetString(PREFS_DATE_KEY, "");

            if (savedDate == today)
            {
                // Same day — load existing challenges
                LoadTodayChallenges();
                Debug.Log($"[DailyChallengeManager] 📋 Loaded {_todayChallenges.Count} challenges for today");
            }
            else
            {
                // New day — generate fresh challenges
                GenerateDailyChallenges();
                PlayerPrefs.SetString(PREFS_DATE_KEY, today);
                PlayerPrefs.Save();
                Debug.Log($"[DailyChallengeManager] 🔄 Generated {_todayChallenges.Count} new challenges");
                OnChallengesRefreshed?.Invoke();
            }
        }

        /// <summary>
        /// Generate new random daily challenges from the database.
        /// </summary>
        private void GenerateDailyChallenges()
        {
            _todayChallenges.Clear();

            if (GameDatabase.Instance == null || GameDatabase.Instance.dailyChallenges.Count == 0)
            {
                Debug.LogWarning("[DailyChallengeManager] No challenges in database.");
                return;
            }

            List<DailyChallengeData> pool = new List<DailyChallengeData>(GameDatabase.Instance.dailyChallenges);

            // Shuffle and pick up to CHALLENGES_PER_DAY
            for (int i = pool.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                var temp = pool[i];
                pool[i] = pool[j];
                pool[j] = temp;
            }

            int count = Mathf.Min(CHALLENGES_PER_DAY, pool.Count);
            string ids = "";

            for (int i = 0; i < count; i++)
            {
                _todayChallenges.Add(pool[i]);
                ids += pool[i].challengeId + (i < count - 1 ? "," : "");

                // Reset progress for this challenge
                PlayerPrefs.SetInt(PREFS_PROGRESS_PREFIX + pool[i].challengeId, 0);
                PlayerPrefs.SetInt(PREFS_CLAIMED_PREFIX + pool[i].challengeId, 0);
            }

            PlayerPrefs.SetString(PREFS_IDS_KEY, ids);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load today's challenges from saved IDs.
        /// </summary>
        private void LoadTodayChallenges()
        {
            _todayChallenges.Clear();

            if (GameDatabase.Instance == null) return;

            string ids = PlayerPrefs.GetString(PREFS_IDS_KEY, "");
            if (string.IsNullOrEmpty(ids)) return;

            string[] idArray = ids.Split(',');
            foreach (string id in idArray)
            {
                string trimmedId = id.Trim();
                if (string.IsNullOrEmpty(trimmedId)) continue;

                DailyChallengeData data = GameDatabase.Instance.GetDailyChallenge(trimmedId);
                if (data != null)
                {
                    _todayChallenges.Add(data);
                }
            }
        }

        // ===============================
        // Progress Tracking
        // ===============================

        /// <summary>
        /// Update progress for all active challenges of a specific type.
        /// Called by gameplay systems (e.g., MergeManager fires this on merge).
        /// </summary>
        public void UpdateProgress(ChallengeType type, int amount = 1)
        {
            foreach (var challenge in _todayChallenges)
            {
                if (challenge.type != type) continue;
                if (IsClaimed(challenge.challengeId)) continue;

                string key = PREFS_PROGRESS_PREFIX + challenge.challengeId;
                int current = PlayerPrefs.GetInt(key, 0) + amount;
                PlayerPrefs.SetInt(key, current);

                // Parse target from template (e.g., "Merge 20 items" → 20)
                int target = ParseTarget(challenge.targetTemplate);

                OnChallengeProgress?.Invoke(challenge.challengeId, current, target);

                if (current >= target && target > 0)
                {
                    Debug.Log($"[DailyChallengeManager] ✅ Challenge completed: {challenge.descriptionEN}");
                    OnChallengeCompleted?.Invoke(challenge.challengeId);
                }
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Parse the numeric target from a template string like "Merge {20} items".
        /// Falls back to extracting any number found in the string.
        /// </summary>
        private int ParseTarget(string template)
        {
            if (string.IsNullOrEmpty(template)) return 1;

            // Try to find a number in the template
            string number = "";
            bool inBrace = false;
            foreach (char c in template)
            {
                if (c == '{') { inBrace = true; continue; }
                if (c == '}') { inBrace = false; continue; }
                if (inBrace || char.IsDigit(c))
                    number += c;
            }

            if (int.TryParse(number, out int target))
                return target;

            return 1;
        }

        // ===============================
        // Claiming Rewards
        // ===============================

        /// <summary>
        /// Claim the reward for a completed challenge.
        /// </summary>
        public bool ClaimReward(string challengeId)
        {
            if (IsClaimed(challengeId)) return false;

            DailyChallengeData data = null;
            foreach (var c in _todayChallenges)
            {
                if (c.challengeId == challengeId) { data = c; break; }
            }

            if (data == null) return false;

            // Check completion
            int current = GetProgress(challengeId);
            int target = ParseTarget(data.targetTemplate);
            if (current < target) return false;

            // Grant rewards
            if (PlayerDataManager.Instance != null)
            {
                if (data.rewardCoins > 0)
                    PlayerDataManager.Instance.AddCoins(data.rewardCoins);
                if (data.rewardGems > 0)
                    PlayerDataManager.Instance.AddGems(data.rewardGems);
            }

            // Mark claimed
            PlayerPrefs.SetInt(PREFS_CLAIMED_PREFIX + challengeId, 1);
            PlayerPrefs.Save();

            Debug.Log($"[DailyChallengeManager] 🎁 Claimed reward: +{data.rewardCoins} coins, +{data.rewardGems} gems");
            return true;
        }

        // ===============================
        // Queries
        // ===============================

        public int GetProgress(string challengeId)
        {
            return PlayerPrefs.GetInt(PREFS_PROGRESS_PREFIX + challengeId, 0);
        }

        public bool IsClaimed(string challengeId)
        {
            return PlayerPrefs.GetInt(PREFS_CLAIMED_PREFIX + challengeId, 0) == 1;
        }

        public bool IsCompleted(string challengeId)
        {
            DailyChallengeData data = null;
            foreach (var c in _todayChallenges)
            {
                if (c.challengeId == challengeId) { data = c; break; }
            }
            if (data == null) return false;

            int current = GetProgress(challengeId);
            int target = ParseTarget(data.targetTemplate);
            return current >= target;
        }
    }
}
