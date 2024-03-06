using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShadowRando.Controls;
using System.Collections.Generic;

namespace ShadowRando.Core;

class TrackerLevelInfo(Stage stage)
{
	public TrackerNode Node { get; } = new TrackerNode(stage.ID, stage.HasDark, stage.HasNeutral || stage.IsBoss, stage.HasHero);
	public TrackerLine? DarkLine { get; set; }
	public TrackerLine? NeutralLine { get; set; }
	public TrackerLine? HeroLine { get; set; }
	public List<TrackerLine> IncomingLines { get; } = [];
}

class TrackerLine
{
	public TrackerLevelInfo Start { get; }
	public TrackerLevelInfo End { get; }
	public ConnectionType Type { get; }
	public Line Line { get; } = new Line() { StrokeThickness = 3, IsHitTestVisible = false };

	public TrackerLine(TrackerLevelInfo start, TrackerLevelInfo end, ConnectionType type)
	{
		Start = start;
		End = end;
		Type = type;
		switch (type)
		{
			case ConnectionType.Neutral:
				Line.Stroke = Brushes.White;
				break;
			case ConnectionType.Hero:
				Line.Stroke = Brushes.Blue;
				break;
			case ConnectionType.Dark:
				Line.Stroke = Brushes.Red;
				break;
		}
	}
}
