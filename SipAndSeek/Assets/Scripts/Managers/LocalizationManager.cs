using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SipAndSeek.Managers
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public enum Language { English, Arabic }
        public Language CurrentLanguage { get; private set; } = Language.English;

        private Dictionary<string, string> _localizedTextEN = new Dictionary<string, string>();
        private Dictionary<string, string> _localizedTextAR = new Dictionary<string, string>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            LoadLocalization();
        }

        public void SetLanguage(Language lang)
        {
            CurrentLanguage = lang;
            // Trigger UI update event here
        }

        private void LoadLocalization()
        {
            TextAsset csvFile = Resources.Load<TextAsset>("Localization/localization");
            if (csvFile == null)
            {
                Debug.LogError("Localization CSV not found in Resources/Localization/");
                return;
            }

            string[] lines = csvFile.text.Split('\n');
            // Assuming header is line 0: Key_ID,AR_Text,EN_Text

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Simple CSV parsing - handling quotes if necessary, but for now simple split
                // Better CSV parser needed for production if text contains commas
                // For this task, we will implementation a robust regex split
                
                string[] parts = ParseCsvLine(line);

                if (parts.Length >= 3)
                {
                    string key = parts[0];
                    string ar = parts[1];
                    string en = parts[2];

                    if (!_localizedTextAR.ContainsKey(key)) _localizedTextAR.Add(key, ar);
                    if (!_localizedTextEN.ContainsKey(key)) _localizedTextEN.Add(key, en);
                }
            }
            Debug.Log($"Loaded {_localizedTextEN.Count} localization entries.");
        }

        // Handles "quoted text, with commas" correctly
        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string current = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            result.Add(current);

            // Clean up quotes
            for(int i=0; i<result.Count; i++)
            {
                result[i] = result[i].Trim();
                if (result[i].StartsWith("\"") && result[i].EndsWith("\""))
                {
                    result[i] = result[i].Substring(1, result[i].Length - 2);
                }
                // Handle escaped quotes if needed ("" -> ")
                result[i] = result[i].Replace("\"\"", "\"");
            }

            return result.ToArray();
        }

        public string GetText(string key)
        {
            Dictionary<string, string> dict = (CurrentLanguage == Language.Arabic) ? _localizedTextAR : _localizedTextEN;
            
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return $"[{key}]";
        }

        // Helper for parameterized text
        public string GetText(string key, object param)
        {
            string text = GetText(key);
            return text.Replace("{target}", param.ToString());
        }
    }
}
