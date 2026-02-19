
using UnityEngine;
#if UNITY_EDITOR || NODE_CANVAS_PRESENT
using NodeCanvas.Framework;
#endif

namespace SipAndSeek.Gameplay
{
    /// <summary>
    /// Triggers a NodeCanvas dialogue graph when conditions are met.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        public string triggerID;
        public bool triggerOnStart = false;

#if UNITY_EDITOR || NODE_CANVAS_PRESENT
        private GraphOwner graphOwner;

        private void Start()
        {
            graphOwner = GetComponent<GraphOwner>();
            if (triggerOnStart)
            {
                PlayDialogue();
            }
        }

        public void PlayDialogue()
        {
            if (graphOwner != null)
            {
                graphOwner.StartBehaviour();
            }
            else
            {
                Debug.LogWarning($"No GraphOwner found on DialogueTrigger {name}");
            }
        }
#else
        public void PlayDialogue()
        {
             Debug.Log("NodeCanvas not present. Dialogue would play here.");
        }
#endif
    }
}
