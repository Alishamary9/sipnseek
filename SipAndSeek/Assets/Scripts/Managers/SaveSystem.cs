using UnityEngine;
using System.IO;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Generic JSON-based save/load system.
    /// Stores data as JSON files in Application.persistentDataPath.
    /// </summary>
    public static class SaveSystem
    {
        private static string GetFilePath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + ".json");
        }

        /// <summary>
        /// Save any serializable object as a JSON file.
        /// </summary>
        public static void Save<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data, true);
            string path = GetFilePath(key);

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, json);
                Debug.Log($"[SaveSystem] Data saved to: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save data for key '{key}': {e.Message}");
            }
        }

        /// <summary>
        /// Load a JSON file and deserialize it into the specified type.
        /// Returns default(T) if the file does not exist or fails to load.
        /// </summary>
        public static T Load<T>(string key) where T : class, new()
        {
            string path = GetFilePath(key);

            if (!File.Exists(path))
            {
                Debug.Log($"[SaveSystem] No save file found for key '{key}'. Returning new instance.");
                return new T();
            }

            try
            {
                string json = File.ReadAllText(path);
                T data = JsonUtility.FromJson<T>(json);
                Debug.Log($"[SaveSystem] Data loaded from: {path}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load data for key '{key}': {e.Message}");
                return new T();
            }
        }

        /// <summary>
        /// Check if a save file exists for the given key.
        /// </summary>
        public static bool HasSave(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        /// <summary>
        /// Delete a specific save file.
        /// </summary>
        public static void DeleteSave(string key)
        {
            string path = GetFilePath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SaveSystem] Save deleted: {path}");
            }
        }

        /// <summary>
        /// Delete all save files in the persistent data path.
        /// </summary>
        public static void DeleteAllSaves()
        {
            string directory = Application.persistentDataPath;
            string[] files = Directory.GetFiles(directory, "*.json");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Debug.Log($"[SaveSystem] All saves deleted. ({files.Length} files removed)");
        }
    }
}
