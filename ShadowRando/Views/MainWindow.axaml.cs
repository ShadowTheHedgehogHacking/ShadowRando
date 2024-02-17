using Avalonia.Controls;

namespace ShadowRando.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		_firstScreen = new FirstScreen(this);
		Content = _firstScreen;
		InitializeComponent();
	}

	private MainView? _mainView;
	private readonly FirstScreen _firstScreen;

	public void LoadMainView(string folderPath)
	{
		_mainView = new MainView(folderPath);
		_firstScreen.IsVisible = false;
		Content = _mainView;
	}

	private void Control_OnSizeChanged(object? sender, SizeChangedEventArgs e)
	{
		if (_mainView == null)
			return;
		switch (this.Height)
	    {
		    case <= 750:
			    _mainView.Layout_Weapon_Border.Height = 475;
			    break;
		    case > 750:
			    _mainView.Layout_Weapon_Border.Height = 475 + (this.Height - 750);
				    break;
	    }
    }
}
