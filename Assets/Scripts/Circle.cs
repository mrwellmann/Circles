using EasyButtons;
using Lofelt.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Circle : MonoBehaviour, ICircleProperties, IObjectInteractions
{
    [FormerlySerializedAs("_circleVisualisation")]
    [SerializeField]
    private List<CircleVisualisation> _circleVisualisations;

    private int _currentVisualisation = 0;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _body2D;
    private AudioSource _audioSource;
    private Animator _ballAnimator;

    private bool _gravityEnabled;
    private int _hitAnimationParameter;

    public CircleVisualisation CircleVisualisation
    {
        get
        {
            return _circleVisualisations[CurrentVisualisation];
        }
        private set
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _spriteRenderer.sprite = value.CircleSprite;
            _spriteRenderer.color = value.SpriteColor;
        }
    }

    public int CurrentVisualisation
    {
        get => _currentVisualisation;
        set
        {
            _currentVisualisation = value;
            CircleVisualisation = _circleVisualisations[value];
        }
    }

    public double Radius { get; set; }

    public bool GravityEnabled
    {
        get => _gravityEnabled;
        set
        {
            _gravityEnabled = value;
            if (!value) _body2D.constraints = RigidbodyConstraints2D.FreezeAll;
            else _body2D.constraints = RigidbodyConstraints2D.None;
        }
    }

    public Vector2 Position { get; set; }

    private void Awake()
    {
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

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    public void SwitchVisualisation()
    {
        if (CurrentVisualisation == _circleVisualisations.Count - 1) CurrentVisualisation = 0;
        else CurrentVisualisation++;
    }

    public void SetRandomVisualisation()
    {
        CurrentVisualisation = Random.Range(0, _circleVisualisations.Count);
    }

    [Button]
    public void SwitchVisualisationTo(int index)
    {
        if (index < _circleVisualisations.Count) CurrentVisualisation = index;
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

    public void OnTap()
    {
        SwitchVisualisation();
        Debug.Log("OnTap");
    }

    public void OnDoubleTap()
    {
        SwitchVisualisation();
        Debug.Log("OnDoubleTap");
    }

    public void OnWiping()
    {
        SwitchVisualisation();
        Debug.Log("OnWiping");
    }
}