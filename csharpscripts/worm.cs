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

	public int CurrentLength { get => Segments.Count + 1; }
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
		HeadSegment.ToHead(new Vector3I(1,-1,0)); // doesn't matter but prevents an error due to empty SourceDir
		Segments = new Queue<Segment>();
	}

	public void Move(Vector3I Dir)
	{
		HeadSegment.ToMiddle(Dir);
		if (CurrentLength == 1) { 
			TailSegment = HeadSegment;
			TailSegment.ToTail();
		}
		Vector3I NewPosition = HeadSegment.Position + Dir;
		HeadSegment = new Segment(NewPosition, CricketMap, SegmentScene, Container);
		HeadSegment.ToHead(Dir);
		Segments.Enqueue(HeadSegment);
		GD.Print(CurrentLength);
		if (CurrentLength > TargetLength)
		{
			TailSegment.Clear();
			TailSegment = Segments.Dequeue();
			GD.Print("TailSegment Dequeued Successfully");
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

		public void ToHead(Vector3I sourceDir)
		{
			SourceDir = sourceDir;
			//if (SourceDir.LengthSquared() != 2) throw new ArgumentOutOfRangeException("Source");
			Sprite.Call("set_head_segment", -SourceDir);
		}
		public void ToMiddle(Vector3I targetDir) 
		{
			TargetDir = targetDir;
			//if (TargetDir.LengthSquared() != 2) throw new ArgumentOutOfRangeException("Target");
			Sprite.Call("set_middle_segment", -TargetDir, -SourceDir);
		}
		public void ToTail()
		{
			Sprite.Call("set_tail_segment", -TargetDir);
		}
		public void Clear()
		{
			Sprite.QueueFree();
			GD.Print("QueueFree Complete");
		}
		public Segment(Vector3I position, TileMap tileMap, PackedScene segment, Node2D container)
		{
			Position = position;
			Sprite = segment.Instantiate<Sprite2D>();
			container.AddChild(Sprite);
			Sprite.GlobalPosition = tileMap.ToGlobal(tileMap.MapToLocal(new Vector2I(position.X, position.Y)));
			GD.Print("Created new segment at " + position.ToString());
		}
	}
	
	
}
