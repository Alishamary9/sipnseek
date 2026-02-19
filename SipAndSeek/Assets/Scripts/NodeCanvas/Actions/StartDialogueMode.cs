
#if UNITY_EDITOR || NODE_CANVAS_PRESENT
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SipAndSeek.Managers;

namespace SipAndSeek.NodeCanvasIntegration
{
    [Category("Sip & Seek")]
    [Description("Starts dialogue mode (pauses game, shows UI).")]
    public class StartDialogueMode : ActionTask
    {
        protected override void OnExecute()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue();
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
