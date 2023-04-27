using System;
using UnityEngine;

public class ClapDetector : MonoBehaviour
{
    public event Action OnClapDetected;

    [SerializeField]
    private float _clapThreshold = 0.7f;

    [SerializeField]
    private int _sampleDataLength = 128;

    [SerializeField]
    private float _maxClapFrequency = 0.5f;

    private AudioClip _microphoneInput;
    private bool _isMicrophoneInitialized;
    private int _audioSampleRate = 44100;
    private string _microphoneDevice;
    private float _timeSinceLastClap;

    private void Start()
    {
        InitializeMicrophone();
        _timeSinceLastClap = _maxClapFrequency;
    }

    private void Update()
    {
        _timeSinceLastClap += Time.deltaTime;

        if (_isMicrophoneInitialized)
        {
            float soundLevel = GetMaxSoundLevel();
            if (soundLevel > _clapThreshold && _timeSinceLastClap >= _maxClapFrequency)
            {
                Debug.Log("Clap detected!");
                _timeSinceLastClap = 0f;
                OnClapDetected?.Invoke();
            }
        }
    }

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

    private void OnDestroy()
    {
        if (_isMicrophoneInitialized)
        {
            Microphone.End(_microphoneDevice);
        }
    }
}