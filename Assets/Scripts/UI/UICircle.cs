using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;
using static Lofelt.NiceVibrations.HapticPatterns;

namespace Circles.UI
{
    public enum UICircleType
    {
        Red,
        Blue,
        Yellow
    }

    public class UICircle : MonoBehaviour
    {
        public event CircleClickedDelegate CircleClicked;
        public delegate void CircleClickedDelegate(UICircle sender);

        [SerializeField]
        private GenericDictionary<UICircleType, Sprite> circleImages;

        private Button button;
        private Image image;
        private AudioSource audioSource;

        private Rigidbody2D rb;

        public UICircleType CircleType { get; private set; }

        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnClick()
        {
            CircleClicked?.Invoke(this);
        }

        public void Create(UICircleType circleType, bool preSpawn = false)
        {
            CircleType = circleType;

            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            image = GetComponent<Image>();
            image.sprite = circleImages[circleType];

            audioSource = GetComponent<AudioSource>();

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
            audioSource.Play();
        }

        private void PlayHaptics()
        {
            HapticPatterns.PlayPreset(PresetType.Selection);
        }

        public void ActivateRigidbody()
        {
            rb.simulated = true;
        }

        public void DeactivateRigidbody()
        {
            rb.simulated = false;
        }
    }
}