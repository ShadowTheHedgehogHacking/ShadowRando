using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace ShadowRando.Views;

public partial class FirstScreen : UserControl
{

	private readonly MainWindow _mainWindow;
	private bool buttonProcessing = false;
	
	public FirstScreen(MainWindow mainWindow)
	{
		InitializeComponent();
		_mainWindow = mainWindow;
	}

	private async void Button_OnClick(object? sender, RoutedEventArgs e)
	{
		if (buttonProcessing)
			return;
		buttonProcessing = true;
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel == null) return;
		var folderPath = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = "Select the root folder of an extracted Shadow the Hedgehog disc image.",
			AllowMultiple = false
		});

		if (folderPath.Count <= 0)
		{
			buttonProcessing = false;
			return;
		}
		_mainWindow.LoadMainView(folderPath[0].Path.LocalPath);
	}
}