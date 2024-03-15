using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Worm
{
	Queue<Segment> Segments;
	Segment HeadSegment;
	Segment TailSegment;
	TileMap CricketMap;
	PackedScene SegmentScene;
	Node2D Container;
	
	public int TargetLength { get; set; }

	public int CurrentLength { get => Segments.Count; }
	public Vector3I CurrentDir { get => HeadSegment.SourceDir; }
	public Vector3I HeadPosition { get => HeadSegment.Position; }
	public Vector3I TailPosition { get => TailSegment.Position; }

	public Worm(PackedScene segmentScene, TileMap cricketMap, Node2D container, Vector3I startPosition, int targetLength)
	{
		SegmentScene = segmentScene;
		CricketMap = cricketMap;
		Container = container;
		TargetLength = targetLength;
		HeadSegment = new Segment(startPosition, CricketMap, SegmentScene, Container);
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
		HeadSegment = new Segment(NewPosition, CricketMap, SegmentScene, Container);
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
		public Segment(Vector3I position, TileMap tileMap, PackedScene segment, Node2D container)
		{
			Position = position;
			Sprite = segment.Instantiate<Sprite2D>();
			container.AddChild(Sprite);
			Sprite.GlobalPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(position.X, position.Y)));
		}
	}
	
	
}
