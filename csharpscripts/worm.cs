using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Worm : Node2D
{
	Queue<Segment> Segments;
	Segment HeadSegment;
	Segment TailSegment;
	TileMap CricketMap;
	PackedScene SegmentScene;

    [Export]
	public int TargetLength { get; set; }
	public int CurrentLength { get => Segments.Count; }
    public Vector3I CurrentDir { get => HeadSegment.SourceDir; }
    public Vector3I HeadPosition { get => HeadSegment.Position; }
	public Vector3I TailPosition { get => TailSegment.Position; }

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		HeadSegment = new Segment(new Vector3I(0, 0, 0), CricketMap, SegmentScene);
		Segments = new Queue<Segment>();
		Segments.Enqueue(HeadSegment);
	}

	public void Move(Vector3I Dir)
	{

		if (CurrentLength == 1) { 
			TailSegment = HeadSegment;
            TailSegment.ToTail();
        } else { HeadSegment.ToMiddle(Dir); }
		Vector3I NewPosition = HeadSegment.Position + Dir;
		HeadSegment = new Segment(NewPosition, CricketMap, SegmentScene);
		HeadSegment.ToHead(Dir);
		Segments.Enqueue(HeadSegment);
		if (CurrentLength > TargetLength)
		{
			TailSegment.Clear();
			TailSegment = Segments.Dequeue();
			TailSegment.ToTail();
		}
	}

	public bool Occupies(Vector3I Position) => Segments.Any(s => s.Position == Position);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public class Segment
	{
		public Vector3I Position { get; private set; }
		public Vector3I SourceDir { get; private set; }
        public Vector3I TargetDir { get; private set; }
		public Sprite2D Sprite { get; private set; }

		public void ToHead(Vector3I Source)
		{
			SourceDir = Source - Position;
			if (SourceDir.LengthSquared() != 2) throw new ArgumentOutOfRangeException("Source");
			Sprite.Call("set_head_segment", -SourceDir);
		}
		public void ToMiddle(Vector3I Target) 
		{
            TargetDir = Target - Position;
            if (TargetDir.LengthSquared() != 2) throw new ArgumentOutOfRangeException("Source");
            Sprite.Call("set_middle_segment", -TargetDir, -SourceDir);
        }
		public void ToTail()
		{
			Sprite.Call("set_tail_segment", -TargetDir);
		}
		public void Clear()
		{
			Sprite.QueueFree();
		}
		public Segment(Vector3I position, TileMap tileMap, PackedScene segment)
        {
            Position = position;
			Sprite = segment.Instantiate<Sprite2D>();
			Sprite.GlobalPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(position.X, position.Y)));
        }
    }
	
	
}
