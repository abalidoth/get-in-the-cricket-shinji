using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class WormController : Node2D
{

	Worm worm;
	TileMap CricketMap;
	bool Stopped = true;
	Dictionary<Key, Vector3I> Inputs;

	
	[Export]
	int StartLength;
	[Export]
	string SegmentSceneFilename;
	[Export]
	Vector3I StartPosition;
	[Export]
	string[] StartingMoves;
	[Export]
	string CricketMapPath;

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

	[ExportGroup("TileTypeAtlasCoords")]
	[Export]
	Vector2I H_Outside = new Vector2I(0, 0);
	[Export]
	Vector2I H_Barrier = new Vector2I(1, 0);
    [Export]
    Vector2I H_Empty = new Vector2I(2, 0);
    [Export]
    Vector2I H_Left = new Vector2I(3, 0);
    [Export]
    Vector2I H_Right = new Vector2I(4, 0);
    [Export]
    Vector2I H_Step = new Vector2I(5, 0);
    [Export]
    Vector2I H_GoStop = new Vector2I(6, 0);
    [Export]
    Vector2I H_Jump = new Vector2I(7, 0);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CricketMap = GetNode<TileMap>(CricketMapPath);
		worm = new Worm(
			GD.Load<PackedScene>(SegmentSceneFilename),
			CricketMap,
			this,
			StartPosition,
			StartLength
			);

		Inputs = new Dictionary<Key, Vector3I>()
		{
			{ Key.W, new Vector3I(0,-1, 1)}, //NW
			{ Key.E, new Vector3I(1,-1, 0)}, //NE
			{ Key.D, new Vector3I(1, 0,-1)}, //E
			{ Key.X, new Vector3I(0, 1,-1)}, //SE
			{ Key.Z, new Vector3I(-1, 1, 0)}, //SW
			{ Key.A, new Vector3I(-1, 0, 1)}  //W
		};

		Dictionary<string, Vector3I> Directions = new Dictionary<string, Vector3I>()
		{
			{ "NW", new Vector3I(0,-1, 1)}, //NW
			{ "NE", new Vector3I(1,-1, 0)}, //NE
			{ "E", new Vector3I(1, 0,-1)}, //E
			{ "SE", new Vector3I(0, 1,-1)}, //SE
			{ "SW", new Vector3I(-1, 1, 0)}, //SW
			{ "W", new Vector3I(-1, 0, 1)}  //W
		};

		foreach (string MoveDir in StartingMoves) { Move(Directions[MoveDir]); }
	}

	public void Move(Vector3I Direction)
	{
		Vector3I NewPosition = worm.HeadPosition + Direction;
		Vector2I NewPositionType;

        try
		{
			 NewPositionType = CricketMap.GetCellAtlasCoords(0, new Vector2I(NewPosition.X, NewPosition.Y));
		} catch (Exception) {  NewPositionType = H_Outside;  }
		if (IllegalHex(NewPositionType) || worm.Occupies(NewPosition) )
		{
			EmitSignal(SignalName.Die);
			return;
		}

		worm.Move(Direction);
		if (NewPositionType != H_Empty) worm.TargetLength++;
		if (NewPositionType == H_Left) EmitSignal(SignalName.Left);
		if (NewPositionType == H_Right) EmitSignal(SignalName.Right);
		if (NewPositionType == H_Step) EmitSignal(SignalName.Step);
		if (NewPositionType == H_Jump) EmitSignal(SignalName.Jump);
		if (NewPositionType == H_GoStop)
		{
			if (Stopped) { EmitSignal(SignalName.Go); }
			else { EmitSignal(SignalName.Stop); }
			Stopped = !Stopped;
		}
	}

	public void Tick()
	{
		Move(worm.CurrentDir);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey inputEventKey && @event.IsPressed())
		{
			if (Inputs.ContainsKey(inputEventKey.Keycode)) Move(Inputs[inputEventKey.Keycode]);
		}
	}

	public bool IllegalHex(Vector2I PositionType) => PositionType == H_Barrier || PositionType == H_Empty;

}
