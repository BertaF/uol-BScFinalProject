using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    public class AudioTransitionController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _ambientTracks;

        public static AudioTransitionController Instance;
        private const float FadeDuration = 1.0f;
        private const string ExposedParameter = "AmbientVolume";
        private float _currentVolume;
        private int _currentTrackIndex;

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
            _currentTrackIndex = 0;

            if (!_audioSource.isPlaying)
            {
                // Always play the first track
                _audioSource.clip = _ambientTracks[_currentTrackIndex];
                _audioSource.loop = true;
                _audioSource.Play();
            }
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

        public void SwitchAmbientTrack()
        {
            if (_ambientTracks.IsUnityNull() || _ambientTracks.Length <= 1) return;

            if (!_audioSource.isPlaying) return;

            _audioSource.Stop();

            int numberOfIterations = 10;
            int selectedTrackIndex = _currentTrackIndex;
            while (selectedTrackIndex == _currentTrackIndex && numberOfIterations > 0)
            {
                selectedTrackIndex = Random.Range(0, _ambientTracks.Length);
                numberOfIterations--;
            }

            _currentTrackIndex = _currentTrackIndex = Mathf.Clamp(selectedTrackIndex, 0, _ambientTracks.Length - 1);
            _audioSource.clip = _ambientTracks[_currentTrackIndex];
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}