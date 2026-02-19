
#if UNITY_EDITOR || NODE_CANVAS_PRESENT
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SipAndSeek.Managers;

namespace SipAndSeek.NodeCanvasIntegration
{
    [Category("Sip & Seek")]
    [Description("Ends dialogue mode (resumes game, hides UI).")]
    public class EndDialogueMode : ActionTask
    {
        protected override void OnExecute()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.EndDialogue();
                EndAction(true);
            }
            else
            {
                EndAction(false);
            }
        }
    }
}
#endif
