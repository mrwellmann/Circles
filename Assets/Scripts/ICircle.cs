using UnityEngine;

public interface ICircle
{
    public CircleVisualisation CircleVisualisation { get; set; }
    public double Radius { get; set; }
    public bool GravityEnabled { get; set; }
    public Vector2 Position { get; set; }
}