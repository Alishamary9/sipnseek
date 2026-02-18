using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SipAndSeek.Testing;

namespace SipAndSeek.Editor
{
    /// <summary>
    /// Automates the creation of the Test Scene.
    /// Access via: Tools -> Sip & Seek -> Create Test Scene
    /// </summary>
    public static class TestSceneCreator
    {
        [MenuItem("Tools/Sip & Seek/Create Test Scene")]
        public static void CreateTestScene()
        {
            // Create a new empty scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Create the Setup GameObject
            GameObject setupObj = new GameObject("TestSetup");
            setupObj.AddComponent<TestSceneSetup>();
            
            // Save the scene
            string scenePath = "Assets/Scenes/TestScene.unity";
            
            // Ensure Scenes folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            
            // Auto-generate data tables to prevent "GameDatabase not found" errors
            DataTableGenerator.GenerateAllData();
            
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[TestSceneCreator] ✅ Test Scene created and saved at {scenePath}");
        }
    }
}
