using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lofelt.NiceVibrations;
using static Lofelt.NiceVibrations.HapticPatterns;

public enum CircleType
{
    Red,
    Blue,
    Yellow
}

public class Circle : MonoBehaviour
{
    public event CircleClickedDelegate CircleClicked;
    public delegate void CircleClickedDelegate(Circle sender);

    [SerializeField]
    private GenericDictionary<CircleType, Sprite> circleImages;

    private Button _button;
    private Image _image;
    private AudioSource _audioSource;

    public CircleType CircleType { get; private set; }

    public void Awake()
    {
    }

    private void OnClick()
    {
        CircleClicked?.Invoke(this);
    }

    public void Create(CircleType circleType, bool preSpawn = false)
    {
        CircleType = circleType;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

        _image = GetComponent<Image>();
        _image.sprite = circleImages[circleType];

        _audioSource = GetComponent<AudioSource>();

        gameObject.SetActive(true);
        if (!preSpawn)
        {
            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendCallback(() => PlayAudio());
        mySequence.AppendInterval(.05f);
        mySequence.Append(transform.DOShakeScale(.05f, new Vector3(.25f, .25f, 0), 0, 45, false));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence.AppendInterval(.1f);
        mySequence.AppendCallback(() => PlayHaptics());

        //Sequence mySequence3 = DOTween.Sequence();
        //mySequence.Append(mySequence);
        //mySequence.Join(mySequence2);
    }

    private void PlayAudio()
    {
        _audioSource.Play();
    }

    private void PlayHaptics()
    {
        HapticPatterns.PlayPreset(PresetType.Selection);
    }
}