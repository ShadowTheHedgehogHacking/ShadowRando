using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace ShadowRando.Controls
{
	public partial class TrackerNode : UserControl
	{
		public TrackerNode() : this(0, true, true, true) { }

		public TrackerNode(int level, bool hasDark, bool hasNeutral, bool hasHero)
		{
			InitializeComponent();
			Level = level;
			LevelName.Text = LevelNames[level];
			using var asset = AssetLoader.Open(new Uri($"avares://ShadowRando/Assets/{LevelNames[level]}.png"));
			LevelImage.Source = new Bitmap(asset);
			ExitDark.IsVisible = hasDark;
			ExitNeutral.IsVisible = hasNeutral;
			ExitHero.IsVisible = hasHero;
		}

		private static readonly string[] LevelNames =
		[
			"Westopolis",
			"Digital Circuit",
			"Glyphic Canyon",
			"Lethal Highway",
			"Cryptic Castle",
			"Prison Island",
			"Circus Park",
			"Central City",
			"The Doom",
			"Sky Troops",
			"Mad Matrix",
			"Death Ruins",
			"The ARK",
			"Air Fleet",
			"Iron Jungle",
			"Space Gadget",
			"Lost Impact",
			"GUN Fortress",
			"Black Comet",
			"Lava Shelter",
			"Cosmic Fall",
			"Final Haunt",
			"The Last Way",
			"Black Bull (LH)",
			"Egg Breaker (CC)",
			"Heavy Dog",
			"Egg Breaker (MM)",
			"Black Bull (DR)",
			"Blue Falcon",
			"Egg Breaker (IJ)",
			"Black Doom (GF)",
			"Sonic & Diablon (GF)",
			"Egg Dealer (BC)",
			"Sonic & Diablon (BC)",
			"Egg Dealer (LS)",
			"Egg Dealer (CF)",
			"Black Doom (CF)",
			"Black Doom (FH)",
			"Sonic & Diablon (FH)",
			"Devil Doom"
		];

		public int Level { get; }

		public void HighlightDark(bool highlight) => ExitDark.Stroke = highlight ? Brushes.Yellow : Brushes.Black;
		public void HighlightNeutral(bool highlight) => ExitNeutral.Stroke = highlight ? Brushes.Yellow : Brushes.Black;
		public void HighlightHero(bool highlight) => ExitHero.Stroke = highlight ? Brushes.Yellow : Brushes.Black;
		public void HighlightStart(bool highlight) => StartPoint.Stroke = highlight ? Brushes.Yellow : Brushes.Black;

		public event EventHandler BeginDrag = delegate { };
		public event EventHandler EndDrag = delegate { };
		public event EventHandler ClickStart = delegate { };
		public event EventHandler<Avalonia.Input.PointerPressedEventArgs> ClickDark = delegate { };
		public event EventHandler<Avalonia.Input.PointerPressedEventArgs> ClickNeutral = delegate { };
		public event EventHandler<Avalonia.Input.PointerPressedEventArgs> ClickHero = delegate { };

		bool isdrag = false;

		private void UserControl_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
		{
			if (e.Source == StartPoint)
				ClickStart(this, EventArgs.Empty);
			else if (e.Source == ExitDark)
				ClickDark(this, e);
			else if (e.Source == ExitNeutral)
				ClickNeutral(this, e);
			else if (e.Source == ExitHero)
				ClickHero(this, e);
			else
			{
				BeginDrag(this, EventArgs.Empty);
				isdrag = true;
			}
			e.Handled = true;
		}

		private void UserControl_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
		{
			if (isdrag)
			{
				EndDrag(this, EventArgs.Empty);
				isdrag = false;
			}
		}
	}
}
