using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using ShadowRando.Core;
using System.IO;

namespace ShadowRando.Views;

public partial class FirstScreen : UserControl
{

	private readonly MainWindow _mainWindow;
	private bool buttonProcessing = false;
	private Settings _settings;
	
	public FirstScreen(MainWindow mainWindow)
	{
		_mainWindow = mainWindow;
		_settings = Settings.Load();
		InitializeComponent();
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

		if (_settings.GamePath != selectedFolderPath && Directory.Exists("backup"))
		{
			var msgbox = await Utils.ShowSimpleMessage("Shadow Randomizer", "New game directory selected!\n\nDo you wish to erase the previous backup data and use the new data as a base?", ButtonEnum.YesNoCancel, Icon.Question);
			switch (msgbox)
			{
				case ButtonResult.Yes:
					Directory.Delete("backup", true);
					break;
				case ButtonResult.No:
					break;
				case ButtonResult.Cancel:
					buttonProcessing = false;
					return;
				default:
					break;
			}
		}

		_mainWindow.LoadMainView(selectedFolderPath, _settings);
	}
}