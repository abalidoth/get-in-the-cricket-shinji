using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class WormController : Node2D
{

	Worm worm;
	TileMap CricketMap;
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
	[Signal]
	public delegate void OnMoveEventHandler();

	[ExportGroup("TileTypeAtlasCoords")]
	[Export]
	Vector2I H_Outside;
	[Export]
	Vector2I H_Barrier;
	[Export]
	Vector2I H_Empty;
	[Export]
	Vector2I H_Left;
	[Export]
	Vector2I H_Right;
	[Export]
	Vector2I H_Step;
	[Export]
	Vector2I H_Stop;
	[Export]
	Vector2I H_Go;
	[Export]
	Vector2I H_Jump;


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
		
		GD.Print("LoadComplete");
		foreach (string MoveDir in StartingMoves) { 
			GD.Print(MoveDir);
			Move(Directions[MoveDir]);
			 }
	}

	public void Move(Vector3I Direction)
	{
		Vector3I NewPosition3 = worm.HeadPosition + Direction;
		Vector2I NewPosition = new Vector2I(NewPosition3.X, NewPosition3.Y);
		Vector2I NewPositionType;

		try
		{
			 NewPositionType = CricketMap.GetCellAtlasCoords(0, NewPosition);
		} catch (Exception) {  NewPositionType = H_Outside;  }
		GD.Print("Hex Type " + NewPositionType.ToString() + " " + IllegalHex(NewPositionType).ToString());
		if (IllegalHex(NewPositionType) || worm.Occupies(NewPosition3))
		{
			EmitSignal(SignalName.Die);
			return;
		}

		EmitSignal(SignalName.OnMove);
		worm.Move(Direction);
		GD.Print("Moved in " + Direction.ToString());
		if (NewPositionType != H_Empty) worm.TargetLength++;
		if (NewPositionType == H_Left) EmitSignal(SignalName.Left);
		if (NewPositionType == H_Right) EmitSignal(SignalName.Right);
		if (NewPositionType == H_Step) EmitSignal(SignalName.Step);
		if (NewPositionType == H_Jump) EmitSignal(SignalName.Jump);
		if (NewPositionType == H_Stop)
		{
			CricketMap.SetCell(0, NewPosition, 0, H_Go);
			EmitSignal(SignalName.Stop);
		}
		if (NewPositionType == H_Go)

		{
			CricketMap.SetCell(0, NewPosition, 0, H_Stop);
			EmitSignal(SignalName.Go);
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

	public bool IllegalHex(Vector2I PositionType) => PositionType == H_Barrier || PositionType == H_Outside;

}
