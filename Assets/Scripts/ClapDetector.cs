using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace Circles
{
    public class ClapDetector : MonoBehaviour
    {
        /// <summary>
        /// Event that triggers when a clap is detected.
        /// </summary>
        public event Action OnClapDetected;

        /// <summary>
        /// The sound level threshold for detecting a clap.
        /// </summary>
        [FormerlySerializedAs("_clapThreshold")]
        [SerializeField]
        private float clapThreshold = 0.5f;

        /// <summary>
        /// The length of the sample data used to analyze the microphone input.
        /// A higher value provides a high resolution which is useful for detecting brief sounds like a hand clap.
        /// </summary>
        /// <remarks>
        /// If _sampleDataLength is 128 and _audioSampleRate is 44100, then the length in seconds would be 128 / 44100 = ~0.0029 seconds.
        /// </remarks>
        [FormerlySerializedAs("_sampleDataLength")]
        [SerializeField]
        private int sampleDataLength = 256;

        /// <summary>
        /// The maximum frequency of claps. In seconds, represents the minimum time interval between successive claps.
        /// </summary>
        [FormerlySerializedAs("_maxClapFrequency")]
        [SerializeField]
        private float maxClapFrequency = 0.25f;

        /// <summary>
        /// The attack/decay threshold to distinguish a clap from other noises.
        /// </summary>
        [FormerlySerializedAs("_attackDecayThreshold")]
        [SerializeField]
        private float attackDecayThreshold = 0.3f;

        private AudioClip microphoneInput;
        private bool isMicrophoneInitialized;
        private int audioSampleRate = 44100;
        private string microphoneDevice;
        private float timeSinceLastClap;
        private float averageSoundLevel;
        private bool isDetectionPaused;

        private void Start()
        {
            EnableClapDetection();
        }

        private void OnDestroy()
        {
            DisableClapDetection();
        }

        /// <summary>
        /// Starts the microphone and prepares the system for clap detection.
        /// </summary>
        public void EnableClapDetection()
        {
            InitializeMicrophone();
            timeSinceLastClap = maxClapFrequency;
        }

        /// <summary>
        /// Stops the microphone and disables clap detection.
        /// </summary>
        public void DisableClapDetection()
        {
            if (isMicrophoneInitialized)
            {
                Microphone.End(microphoneDevice);
                isMicrophoneInitialized = false;
            }
        }

        /// <summary>
        /// Pauses the clap detection system without stopping the microphone.
        /// </summary>
        public void PauseClapDetection()
        {
            isDetectionPaused = true;
        }

        /// <summary>
        /// Resumes the clap detection system after being paused.
        /// </summary>
        public void ResumeClapDetection()
        {
            isDetectionPaused = false;
        }

        private void Update()
        {
            if (isMicrophoneInitialized && !isDetectionPaused)
            {
                DetectClapping(Time.deltaTime);
            }
        }

        /// <summary>
        /// Analyzes the audio data and detects if a clap has occurred.
        /// </summary>
        private void DetectClapping(float deltaTime)
        {
            timeSinceLastClap += deltaTime;

            float soundLevel = GetMaxSoundLevel();
            averageSoundLevel = 0.99f * averageSoundLevel + 0.01f * soundLevel;

            if (soundLevel > clapThreshold && soundLevel > averageSoundLevel + attackDecayThreshold && timeSinceLastClap >= maxClapFrequency)
            {
                timeSinceLastClap = 0f;
                Debug.Log("Clap detected!");
                OnClapDetected?.Invoke();
            }
        }

        /// <summary>
        /// Initializes the microphone input for the system.
        /// </summary>
        private void InitializeMicrophone()
        {
            if (Microphone.devices.Length > 0)
            {
                microphoneDevice = Microphone.devices[0];
                microphoneInput = Microphone.Start(microphoneDevice, true, 1, audioSampleRate);
                isMicrophoneInitialized = true;
            }
            else
            {
                Debug.LogError("No microphone devices found!");
            }
        }

        /// <summary>
        /// Calculates the maximum sound level from the current audio sample.
        /// </summary>
        private float GetMaxSoundLevel()
        {
            float maxSoundLevel = 0f;
            float[] sampleData = new float[sampleDataLength];
            int microphonePosition = Microphone.GetPosition(microphoneDevice) - (sampleDataLength + 1);
            if (microphonePosition < 0) return 0;

            microphoneInput.GetData(sampleData, microphonePosition);

            for (int i = 0; i < sampleDataLength; i++)
            {
                float currentSoundLevel = Mathf.Abs(sampleData[i]);
                if (currentSoundLevel > maxSoundLevel)
                {
                    maxSoundLevel = currentSoundLevel;
                }
            }
            return maxSoundLevel;
        }
    }
}