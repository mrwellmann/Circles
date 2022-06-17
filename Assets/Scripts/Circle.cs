using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        transform.DOShakeScale(.05f, .25f, 0, 45, false);

        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(.05f);
        mySequence.AppendCallback(() => PlayAudio());
    }

    public void PlayAudio()
    {
        _audioSource.Play();
    }
}