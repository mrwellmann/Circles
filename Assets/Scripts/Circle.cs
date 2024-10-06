using EasyButtons;
using Lofelt.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Circles
{
    public class Circle : MonoBehaviour, ICircleProperties, IObjectInteractions
    {
        [SerializeField]
        private List<CircleVisualization> circleVisualizations;

        private int currentVisualization = 0;

        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body2D;
        private AudioSource audioSource;
        private Animator ballAnimator;

        private bool gravityEnabled;
        private int hitAnimationParameter;

        public CircleVisualization CircleVisualization
        {
            get
            {
                return circleVisualizations[CurrentVisualization];
            }
            private set
            {
                if (spriteRenderer == null)
                    spriteRenderer = GetComponent<SpriteRenderer>();

                spriteRenderer.sprite = value.CircleSprite;
                spriteRenderer.color = value.SpriteColor;
            }
        }

        public int CurrentVisualization
        {
            get => currentVisualization;
            set
            {
                currentVisualization = value;
                CircleVisualization = circleVisualizations[value];
            }
        }

        public double Radius { get; set; }

        public bool GravityEnabled
        {
            get => gravityEnabled;
            set
            {
                gravityEnabled = value;
                if (!value) body2D.constraints = RigidbodyConstraints2D.FreezeAll;
                else body2D.constraints = RigidbodyConstraints2D.None;
            }
        }

        public Vector2 Position { get; set; }

        private void Awake()
        {
            body2D = GetComponent<Rigidbody2D>();
            ballAnimator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            hitAnimationParameter = Animator.StringToHash("Hit");
        }

        private void Start()
        {
            audioSource.Play();
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log($"c.relativeVelocity: {collision.relativeVelocity}, c.magnitude: {collision.relativeVelocity.magnitude}, c.sqrMagnitude: {collision.relativeVelocity.sqrMagnitude} _body2D.velocity: {_body2D.velocity.magnitude}");
            if (GravityEnabled)
            {
                HitWall();
            }
        }

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void SwitchVisualization()
        {
            if (CurrentVisualization == circleVisualizations.Count - 1) CurrentVisualization = 0;
            else CurrentVisualization++;
        }

        public void SetRandomVisualization()
        {
            CurrentVisualization = Random.Range(0, circleVisualizations.Count);
        }

        [Button]
        public void SwitchVisualizationTo(int index)
        {
            if (index < circleVisualizations.Count) CurrentVisualization = index;
        }

        protected virtual void HitWall()
        {
            //_audioSource.volume = _body2D.velocity.magnitude;
            //_audioSource.Play();

            // float amplitude = body2D.linearVelocity.magnitude / 100f;
            // HapticPatterns.PlayEmphasis(amplitude, 0.7f);

            //ballAnimator.SetTrigger(hitAnimationParameter);
            //Debug.Log($"_body2D.velocity.magnitude: {_body2D.velocity.magnitude}, amplitude: {amplitude}");
        }

        public void OnTap()
        {
            SwitchVisualization();
            Debug.Log("OnTap");
        }

        public void OnDoubleTap()
        {
            SwitchVisualization();
            Debug.Log("OnDoubleTap");
        }

        public void OnWiping()
        {
            SwitchVisualization();
            Debug.Log("OnWiping");
        }
    }
}