using UnityEngine;

public interface ICircleProperties
{
    public CircleVisualisation CircleVisualisation { get; }
    public double Radius { get; set; }
    public bool GravityEnabled { get; set; }
    public Vector2 Position { get; set; }
}