using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ShadowRando.Core;
using System.IO;

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

		string selectedFolderPath = folderPath[0].Path.LocalPath;

		if (selectedFolderPath.EndsWith("\\files") || selectedFolderPath.EndsWith("\\sys")) {
			var parent = Directory.GetParent(selectedFolderPath);
			if (parent != null)
				selectedFolderPath = parent.FullName;
		}

		if (!File.Exists(Path.Combine(selectedFolderPath, "sys", "main.dol")) || !File.Exists(Path.Combine(selectedFolderPath, "sys", "bi2.bin")))
		{
			Utils.ShowSimpleMessage("Error", "Not a valid Shadow the Hedgehog Extracted game.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
			buttonProcessing = false;
			return;
		}
		_mainWindow.LoadMainView(selectedFolderPath);
	}
}