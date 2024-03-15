using Godot;
using System;

public partial class WormController : Node
{
	Worm worm;
	TileMap CricketMap;
	[Export]
	Vector3I StartPosition { get; set; }
	[Export]
	int StartLength { get; set; }
	[Export]
	string SegmentSceneFilename { get; set; }
	[Export]
	string CricketMapPath { get; set; }
	[Export]
	string ContainerPath { get; set; }

	[Signal]
	public delegate void StepEventHandler();
	[Signal]
	public delegate void GoEventHandler();
	[Signal]
	public delegate void StopEventHandler();
	[Signal]
	public delegate void JumpEventHandler();
	[Signal]
	public delegate void RightEventHandler();
	[Signal]
	public delegate void LeftEventHandler();
	[Signal]
	public delegate void DieEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CricketMap = GetNode<TileMap>(CricketMapPath);
        worm = new Worm(
			GD.Load<PackedScene>(SegmentSceneFilename),
			CricketMap,
			GetNode<Node2D>(ContainerPath),
			StartPosition,
			StartLength
			);
		
	}


}
