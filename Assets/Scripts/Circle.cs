using Lofelt.NiceVibrations;
using UnityEngine;

public class Circle : MonoBehaviour, ICircle
{
    [SerializeField]
    private GenericDictionary<CircleType, Sprite> _circleImages;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _body2D;
    private AudioSource _audioSource;
    private Animator _ballAnimator;

    private CircleType _circleType;
    private bool _gravityEnabled;
    private int _hitAnimationParameter;

    public CircleType CircelType
    {
        get => _circleType;
        set
        {
            _circleType = value;
            _spriteRenderer.sprite = _circleImages[_circleType];
        }
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