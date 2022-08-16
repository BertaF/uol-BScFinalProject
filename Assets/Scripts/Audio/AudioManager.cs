using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource _audioSource;

        public static AudioManager Instance;
        private const float FadeDuration = 1.0f;
        private const string ExposedParameter = "AmbientVolume";
        private float _currentVolume;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _audioMixer.GetFloat(ExposedParameter, out var currentVolume);
            _currentVolume = currentVolume;
        }

        public void FadeAudio(float targetVolume)
        {
            StartCoroutine(StartAudioFade(targetVolume));
        }

        // TODO: Call the audio fading function from the screen fader class instead of using the button events

        private IEnumerator StartAudioFade(float targetVolume)
        {
            float currentTime = 0;
            float currentVolume = Mathf.Pow(10, _currentVolume / 20);

            while (currentTime < FadeDuration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVolume, targetVolume, currentTime / FadeDuration);
                _audioMixer.SetFloat(ExposedParameter, Mathf.Log10(newVol) * 20);
                yield return null;
            }

            _audioSource.Stop();

            if (!_audioSource.isPlaying)
            {
                // Set the volume back to the default, to allow the next track to be audible.
                _audioMixer.SetFloat(ExposedParameter, Mathf.Log10(currentVolume) * 20);
            }
        }
    }
}