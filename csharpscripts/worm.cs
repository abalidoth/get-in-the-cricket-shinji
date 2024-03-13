using Godot;
using System;
using System.Collections;

public partial class Worm : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public class Segment
	{
		public Vector3I Position { get; private set; }
		public Vector3I SourceDir { get; private set; }
        public Vector3I TargetDir { get; private set; }


    }
	
	
}
