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
	Vector3I StartPosition { get; set; }
	[Export]
	int StartLength { get; set; }
	[Export]
	string SegmentSceneFilename { get; set; }
	[Export]
	string CricketMapPath { get; set; }

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

    Vector2I H_Outside = new Vector2I(0, 0);
    Vector2I H_Barrier = new Vector2I(1, 0);
    Vector2I H_Empty = new Vector2I(2, 0);
    Vector2I H_Left = new Vector2I(3, 0);
	Vector2I H_Right = new Vector2I(4, 0);
    Vector2I H_Step = new Vector2I(5, 0);
    Vector2I H_GoStop = new Vector2I(6, 0);
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
		
	}

	public void Move(Vector3I Direction)
	{
		Vector3I NewPosition = worm.HeadPosition + Direction;
		Vector2I NewPositionType = CricketMap.GetCellAtlasCoords(0, new Vector2I(NewPosition.X, NewPosition.Y));
		if (IllegalHex(NewPositionType) || worm.Occupies(NewPosition) )
		{
			EmitSignal(SignalName.Die);
			return;
		}

		worm.Move(Direction);

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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey && @event.IsPressed())
		{
            if (Inputs.ContainsKey(inputEventKey.Keycode)) Move(Inputs[inputEventKey.Keycode]);
		}
    }

    public bool IllegalHex(Vector2I PositionType) => PositionType == H_Barrier || PositionType == H_Empty;

}
