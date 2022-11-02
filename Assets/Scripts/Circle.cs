using EasyButtons;
using Lofelt.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Circle : MonoBehaviour, ICircle
{
    [FormerlySerializedAs("_circleImages")]
    [SerializeField]
    private List<CircleVisualisation> _circleVisualisation;

    private int _currentVisualisation = 0;

    private Color _color;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _body2D;
    private AudioSource _audioSource;
    private Animator _ballAnimator;

    private bool _gravityEnabled;
    private int _hitAnimationParameter;

    public CircleVisualisation CircleVisualisation
    {
        get;
        set;
    }

    //public CircleSprite CircelType
    //{
    //    get => _circleType;
    //    set
    //    {
    //        _circleType = value;
    //        _spriteRenderer.sprite = _circleVisualisation[_circleType];
    //    }
    //}
    [Button]
    public void SwitchVisualisation()
    {
        _currentVisualisation++;
        if (_currentVisualisation >= _circleVisualisation.Count) _currentVisualisation = 0;

        _spriteRenderer.sprite = _circleVisualisation[_currentVisualisation].CircleSprite;
        _spriteRenderer.color = _circleVisualisation[_currentVisualisation].SpriteColor;
    }

    public double Radius { get; set; }

    public bool GravityEnabled
    {
        get => _gravityEnabled;
        set
        {
            _gravityEnabled = value;
            _body2D.simulated = value;
        }
    }

    public Vector2 Position { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _body2D = GetComponent<Rigidbody2D>();
        _ballAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _hitAnimationParameter = Animator.StringToHash("Hit");
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"c.relativeVelocity: {collision.relativeVelocity}, c.magnitude: {collision.relativeVelocity.magnitude}, c.sqrMagnitude: {collision.relativeVelocity.sqrMagnitude} _body2D.velocity: {_body2D.velocity.magnitude}");
        HitWall();
    }

    protected virtual void HitWall()
    {
        _audioSource.volume = _body2D.velocity.magnitude;
        _audioSource.Play();

        float amplitude = _body2D.velocity.magnitude / 100f;
        HapticPatterns.PlayEmphasis(amplitude, 0.7f);

        //_ballAnimator.SetTrigger(_hitAnimationParameter);
        //Debug.Log($"_body2D.velocity.magnitude: {_body2D.velocity.magnitude}, amplitude: {amplitude}");
    }
}