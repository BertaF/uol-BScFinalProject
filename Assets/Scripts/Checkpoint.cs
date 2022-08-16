using System.Collections;
using Assets.Scripts.ScreenFadeFeature;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : CheckpointManager
    {
        public ScreenFade _screenFade = null;

        private void FixedUpdate()
        {
            // Check if the player fell from the higher up platforms and teleport to the closest checkpoint if so.

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            SetCheckpoint(transform.position);
            StartCoroutine(FadeSequence(other.gameObject));
        }

        private IEnumerator FadeSequence(GameObject player)
        {
            Debug.Log("Inside FadeSequence");

            // Fade to black
            float duration = _screenFade.FadeIn();

            Debug.Log("Inside FadeSequence, duration: " + duration);

            // Wait, then do the teleport stuff
            yield return new WaitForSeconds(duration);

            Debug.Log("Inside FadeSequence, calling teleport.");
            TeleportToCheckpoint(player.transform);

            // Fade to clear
            Debug.Log("Inside FadeSequence, calling fade out.");
            _screenFade.FadeOut();
        }

        private void OnValidate()
        {
            if (!_screenFade)
            {
                Debug.Log("Inside FadeSequence, OOPS screen fade not valid!");
                _screenFade = FindObjectOfType<ScreenFade>();
            }
        }
    }
}
