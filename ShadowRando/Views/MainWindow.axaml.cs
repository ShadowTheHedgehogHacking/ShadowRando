using Avalonia.Controls;

namespace ShadowRando.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void Control_OnSizeChanged(object? sender, SizeChangedEventArgs e)
	{
		switch (this.Height)
	    {
		    case <= 750:
			    MainView.Layout_Weapon_Border.Height = 475;
			    break;
		    case > 750:
			    MainView.Layout_Weapon_Border.Height = 475 + (this.Height - 750);
				    break;
	    }
    }
}
