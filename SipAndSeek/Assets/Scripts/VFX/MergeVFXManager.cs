using UnityEngine;
using SipAndSeek.Managers;
using SipAndSeek.Gameplay;

namespace SipAndSeek.VFX
{
    /// <summary>
    /// Visual effects for merge, reveal, and item interactions.
    /// Creates and manages particle systems at runtime.
    /// </summary>
    public class MergeVFXManager : MonoBehaviour
    {
        public static MergeVFXManager Instance { get; private set; }

        [Header("Merge VFX")]
        [SerializeField] private Color _mergeParticleColor = new Color(1f, 0.84f, 0.31f, 1f); // Gold
        [SerializeField] private int _mergeParticleCount = 12;

        [Header("Reveal VFX")]
        [SerializeField] private Color _revealParticleColor = new Color(0.65f, 0.84f, 0.65f, 1f); // Sage
        [SerializeField] private int _revealParticleCount = 8;

        [Header("Sell VFX")]
        [SerializeField] private Color _sellParticleColor = new Color(1f, 0.67f, 0.57f, 1f); // Rose
        [SerializeField] private int _sellParticleCount = 15;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            MergeManager.OnMergePerformed += HandleMerge;
            MergeManager.OnSellItem += HandleSell;
            RevealManager.OnTileRevealed += HandleReveal;
        }

        private void OnDisable()
        {
            MergeManager.OnMergePerformed -= HandleMerge;
            MergeManager.OnSellItem -= HandleSell;
            RevealManager.OnTileRevealed -= HandleReveal;
        }

        // ===============================
        // Event Handlers
        // ===============================

        private void HandleMerge(MergeItem oldItem, MergeItem newItem)
        {
            if (newItem != null)
            {
                SpawnParticleBurst(newItem.transform.position, _mergeParticleColor, _mergeParticleCount);
                SpawnScalePop(newItem.gameObject);
            }
        }

        private void HandleSell(int coins)
        {
            // Sell effect at screen center (item already destroyed)
            SpawnParticleBurst(Vector3.zero, _sellParticleColor, _sellParticleCount);
        }

        private void HandleReveal(int row, int col, float progress)
        {
            // Find the cell and play reveal effect
            if (GridManager.Instance != null)
            {
                GridCell cell = GridManager.Instance.GetCell(row, col);
                if (cell != null)
                {
                    SpawnParticleBurst(cell.WorldPosition, _revealParticleColor, _revealParticleCount);
                }
            }
        }

        // ===============================
        // VFX Creators
        // ===============================

        /// <summary>
        /// Spawn a radial particle burst at a position.
        /// </summary>
        public void SpawnParticleBurst(Vector3 position, Color color, int count)
        {
            GameObject vfxObj = new GameObject("VFX_Burst");
            vfxObj.transform.position = position;

            ParticleSystem ps = vfxObj.AddComponent<ParticleSystem>();
            
            // Stop it before configuring
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = 0.6f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = color;
            main.maxParticles = count;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0.3f;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
                AnimationCurve.EaseInOut(0, 1, 1, 0));

            // Use default particle material
            var renderer = vfxObj.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.material.color = color;

            ps.Play();
        }

        /// <summary>
        /// Quick scale pop animation on a GameObject.
        /// </summary>
        public void SpawnScalePop(GameObject target)
        {
            if (target == null) return;
            StartCoroutine(ScalePopCoroutine(target));
        }

        private System.Collections.IEnumerator ScalePopCoroutine(GameObject target)
        {
            if (target == null) yield break;

            Vector3 originalScale = target.transform.localScale;
            Vector3 popScale = originalScale * 1.3f;

            // Pop up
            float elapsed = 0;
            float duration = 0.15f;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.transform.localScale = Vector3.Lerp(originalScale, popScale, t);
                yield return null;
            }

            // Settle back
            elapsed = 0;
            duration = 0.2f;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Elastic ease out
                float bounce = Mathf.Sin(-13f * Mathf.PI / 4f * (t + 1)) * Mathf.Pow(2, -10 * t) + 1;
                target.transform.localScale = Vector3.Lerp(popScale, originalScale, bounce);
                yield return null;
            }

            if (target != null)
                target.transform.localScale = originalScale;
        }
    }
}
