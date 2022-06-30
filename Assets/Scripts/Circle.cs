using UnityEngine;

public class Circle : MonoBehaviour, ICircle
{
    [SerializeField]
    private GenericDictionary<CircleType, Sprite> _circleImages;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _body2D;

    private CircleType _circleType;
    private bool _gravityEnabled;

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
    }
}