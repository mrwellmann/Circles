using System;
using UnityEngine;

public class ClapDetector : MonoBehaviour
{
    /// <summary>
    /// Event that triggers when a clap is detected.
    /// </summary>
    public event Action OnClapDetected;

    /// <summary>
    /// The sound level threshold for detecting a clap.
    /// </summary>
    [SerializeField]
    private float _clapThreshold = 0.5f;

    /// <summary>
    /// The length of the sample data used to analyze the microphone input.
    /// A higher value provides a high resolution which is useful for detecting brief sounds like a hand clap.
    /// </summary>
    /// <remarks>
    /// If _sampleDataLength is 128 and _audioSampleRate is 44100, then the length in seconds would be 128 / 44100 = ~0.0029 seconds.
    /// </remarks>
    [SerializeField]
    private int _sampleDataLength = 256;

    /// <summary>
    /// The maximum frequency of claps. In seconds, represents the minimum time interval between successive claps.
    /// </summary>
    [SerializeField]
    private float _maxClapFrequency = 0.25f;

    /// <summary>
    /// The attack/decay threshold to distinguish a clap from other noises.
    /// </summary>
    [SerializeField]
    private float _attackDecayThreshold = 0.3f;

    private AudioClip _microphoneInput;
    private bool _isMicrophoneInitialized;
    private int _audioSampleRate = 44100;
    private string _microphoneDevice;
    private float _timeSinceLastClap;
    private float _averageSoundLevel;
    private bool _isDetectionPaused;

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
        _timeSinceLastClap = _maxClapFrequency;
    }

    /// <summary>
    /// Stops the microphone and disables clap detection.
    /// </summary>
    public void DisableClapDetection()
    {
        if (_isMicrophoneInitialized)
        {
            Microphone.End(_microphoneDevice);
            _isMicrophoneInitialized = false;
        }
    }

    /// <summary>
    /// Pauses the clap detection system without stopping the microphone.
    /// </summary>
    public void PauseClapDetection()
    {
        _isDetectionPaused = true;
    }

    /// <summary>
    /// Resumes the clap detection system after being paused.
    /// </summary>
    public void ResumeClapDetection()
    {
        _isDetectionPaused = false;
    }

    private void Update()
    {
        if (_isMicrophoneInitialized && !_isDetectionPaused)
        {
            DetectClapping(Time.deltaTime);
        }
    }

    /// <summary>
    /// Analyzes the audio data and detects if a clap has occurred.
    /// </summary>
    private void DetectClapping(float deltaTime)
    {
        _timeSinceLastClap += deltaTime;

        float soundLevel = GetMaxSoundLevel();
        _averageSoundLevel = 0.99f * _averageSoundLevel + 0.01f * soundLevel;

        if (soundLevel > _clapThreshold && soundLevel > _averageSoundLevel + _attackDecayThreshold && _timeSinceLastClap >= _maxClapFrequency)
        {
            _timeSinceLastClap = 0f;
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
            _microphoneDevice = Microphone.devices[0];
            _microphoneInput = Microphone.Start(_microphoneDevice, true, 1, _audioSampleRate);
            _isMicrophoneInitialized = true;
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
        float[] sampleData = new float[_sampleDataLength];
        int microphonePosition = Microphone.GetPosition(_microphoneDevice) - (_sampleDataLength + 1);
        if (microphonePosition < 0) return 0;

        _microphoneInput.GetData(sampleData, microphonePosition);

        for (int i = 0; i < _sampleDataLength; i++)
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