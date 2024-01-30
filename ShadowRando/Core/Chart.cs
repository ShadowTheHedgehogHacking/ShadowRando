using System;
using System.Collections.Generic;
using System.Linq;

namespace ShadowRando.Core;

public class ChartNode
{
	public int GridX { get; }
	public int GridY { get; }

	public Dictionary<Direction, List<ChartConnection>> OutgoingConnections { get; } =
		new Dictionary<Direction, List<ChartConnection>>();

	public Dictionary<Direction, List<ChartConnection>> IncomingConnections { get; } =
		new Dictionary<Direction, List<ChartConnection>>();

	public Dictionary<Direction, ChartConnection[]> ConnectionOrder { get; } =
		new Dictionary<Direction, ChartConnection[]>();

	public ChartNode(int x, int y)
	{
		GridX = x;
		GridY = y;
		foreach (var item in Enum.GetValues(typeof(Direction)).Cast<Direction>())
		{
			OutgoingConnections.Add(item, new List<ChartConnection>());
			IncomingConnections.Add(item, new List<ChartConnection>());
		}
	}

	public void Connect(ConnectionType color, ChartNode dest)
	{
		Direction outdir, indir;
		int xdiff = GridX - dest.GridX;
		int ydiff = GridY - dest.GridY;
		if (ydiff == -1)
		{
			outdir = Direction.Bottom;
			indir = Direction.Top;
		}
		else if (ydiff == 1)
		{
			outdir = Direction.Top;
			indir = Direction.Bottom;
		}
		else if (xdiff == 0)
		{
			if (ydiff < 1)
			{
				outdir = Direction.Right;
				indir = Direction.Right;
			}
			else
			{
				outdir = Direction.Left;
				indir = Direction.Left;
			}
		}
		else if (ydiff == 0 && (xdiff < -1 || xdiff > 1))
		{
			outdir = Direction.Top;
			indir = Direction.Top;
		}
		else if (xdiff < 0)
		{
			outdir = Direction.Right;
			indir = Direction.Left;
		}
		else
		{
			outdir = Direction.Left;
			indir = Direction.Right;
		}

		ChartConnection c = dest.IncomingConnections[indir].Find(a => a.Type == color);
		if (c == null)
		{
			c = new ChartConnection(indir, color, this, dest);
			dest.IncomingConnections[indir].Add(c);
		}
		else
			c.AddSource(this, dest);

		OutgoingConnections[outdir].Add(c);
	}

	public int GetDistance(ChartNode other) => Math.Abs(GridX - other.GridX) + Math.Abs(GridY - other.GridY);
}

public class ChartConnection
{
	public ChartNode Node { get; }
	public Direction Side { get; }
	public ConnectionType Type { get; }
	public List<ChartNode> Sources { get; }
	public int MinX { get; private set; }
	public int MinY { get; private set; }
	public int MaxX { get; private set; }
	public int MaxY { get; private set; }
	public int Distance { get; private set; }
	public int Lane { get; set; }

	public ChartConnection(Direction side, ConnectionType type, ChartNode src, ChartNode dst)
	{
		Node = dst;
		Side = side;
		Type = type;
		Sources = new List<ChartNode>() { src };
		MinX = Math.Min(src.GridX, dst.GridX);
		MinY = Math.Min(src.GridY, dst.GridY);
		MaxX = Math.Max(src.GridX, dst.GridX);
		MaxY = Math.Max(src.GridY, dst.GridY);
		Distance = src.GetDistance(dst);
	}

	public void AddSource(ChartNode src, ChartNode dst)
	{
		Sources.Add(src);
		MinX = Math.Min(src.GridX, MinX);
		MinY = Math.Min(src.GridY, MinY);
		MaxX = Math.Max(src.GridX, MaxX);
		MaxY = Math.Max(src.GridY, MaxY);
		Distance = Math.Max(src.GetDistance(dst), Distance);
	}

	public static int CompareConnV(ChartConnection a, ChartConnection b)
	{
		int r = a.MinY.CompareTo(b.MinY);
		if (r == 0)
			r = a.MaxY.CompareTo(b.MaxY);
		return r;
	}

	public static int CompareConnH(ChartConnection a, ChartConnection b)
	{
		int r = a.MinX.CompareTo(b.MinX);
		if (r == 0)
			r = a.MaxX.CompareTo(b.MaxX);
		return r;
	}
}