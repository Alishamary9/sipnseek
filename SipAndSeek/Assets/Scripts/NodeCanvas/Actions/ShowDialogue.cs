
#if UNITY_EDITOR || NODE_CANVAS_PRESENT
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SipAndSeek.Managers;

namespace SipAndSeek.NodeCanvasIntegration
{
    [Category("Sip & Seek")]
    [Description("Displays a dialogue line and waits for completion.")]
    public class ShowDialogue : ActionTask
    {
        [RequiredField]
        public BBParameter<string> characterName;
        public BBParameter<string> emotion;
        
        [RequiredField]
        [TextArea]
        public BBParameter<string> textKey;

        protected override string OnInit()
        {
            return null;
        }

        protected override void OnExecute()
        {
            if (DialogueManager.Instance == null)
            {
                EndAction(false);
                return;
            }

            DialogueManager.Instance.ShowLine(characterName.value, emotion.value, textKey.value, OnDialogueComplete);
        }

        private void OnDialogueComplete()
        {
            EndAction(true);
        }

        protected override void OnUpdate()
        {
            // Nothing needed here, DialogueManager handles the wait.
        }
    }
}
#endif
