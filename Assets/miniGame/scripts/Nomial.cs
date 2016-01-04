using UnityEngine;
using System.Collections;

public abstract class Nomial : MonoBehaviour { //blueprint for the Symbol and Group classes
	public abstract float width { get; } //width of sprite
	public abstract float x { get; set; } //for easy access
	public abstract float midPoint { get; }
}
