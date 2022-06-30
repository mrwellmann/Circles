using UnityEngine;

public enum CircleType
{
    Red,
    Blue,
    Yellow
}

public interface ICircle
{
    public CircleType CircelType { get; set; }
    public double Radius { get; set; }
    public bool GravityEnabled { get; set; }
    public Vector2 Position { get; set; }
}