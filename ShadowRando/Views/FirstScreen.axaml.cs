using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using ShadowRando.Core;
using System.IO;
using System.Linq;

namespace ShadowRando.Views;

public partial class FirstScreen : UserControl
{

	private readonly MainWindow _mainWindow;
	private bool buttonProcessing = false;
	private Settings _settings;

	private static readonly string[] reloaded_missing_event_names =
	[
		"event0811_sceneA.one",
		"event0903_sceneA.one",
		"event0906_sceneA.one",
		"event0913_sceneA.one",
		"event0915_sceneA.one",
		"event0917_sceneA.one",
		"event0919_sceneA.one",
		"event0921_sceneA.one",
		"event0922_sceneA.one",
		"event0924_sceneA.one",
		"event0925_sceneA.one",
		"event0960_sceneA.one",
		"event0961_sceneA.one"
	];

	public FirstScreen()
	{
		InitializeComponent();
	}
	
	public FirstScreen(MainWindow mainWindow)
	{
		_mainWindow = mainWindow;
		_settings = Settings.Load();
		InitializeComponent();
	}

	private async void LoadNewGameFolder_OnClick(object? sender, RoutedEventArgs e)
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

		ProcessGameFolder(selectedFolderPath);
	}

	private async void LoadPriorGameFolder_OnClick(object? sender, RoutedEventArgs e)
	{
		if (buttonProcessing)
			return;
		buttonProcessing = true;
		ProcessGameFolder(_settings.GamePath);
	}

	private async void ProcessGameFolder(string gamePath)
	{
		if (!Directory.Exists(gamePath))
		{
			_ = Utils.ShowSimpleMessage("Error", "Folder not found.", ButtonEnum.Ok, Icon.Error);
			buttonProcessing = false;
			return;
		}

		var multiPlatformSafeGamePathCompare = gamePath.Replace("\\", "").Replace("/", "");

		if (multiPlatformSafeGamePathCompare.EndsWith("files") || multiPlatformSafeGamePathCompare.EndsWith("sys")) {
			var parent = Directory.GetParent(gamePath);
			if (parent != null && File.Exists(Path.Combine(parent.FullName, "sys", "main.dol")) && File.Exists(Path.Combine(parent.FullName, "sys", "bi2.bin")))
			{
				gamePath = parent.FullName;
			} else
			{
				gamePath = parent?.Parent?.FullName ?? gamePath;
			}
		}

		if (!File.Exists(Path.Combine(gamePath, "sys", "main.dol")) || !File.Exists(Path.Combine(gamePath, "sys", "bi2.bin")))
		{
			_ = Utils.ShowSimpleMessage("Error", "Not a valid Shadow the Hedgehog Extracted game.", ButtonEnum.Ok, Icon.Error);
			buttonProcessing = false;
			return;
		}

		bool missingEventsDetected = reloaded_missing_event_names.Any(eventName => !File.Exists(Path.Combine(gamePath, "files", "event", eventName)));
		if (missingEventsDetected)
		{
			await Utils.ShowSimpleMessage("Missing Events Detected", $"We have detected missing event files in your game.{Environment.NewLine}{Environment.NewLine}If you use the \"Random Partners\" option, you may be unable to complete certain missions.{Environment.NewLine}{Environment.NewLine}To fix this issue, download the 'missing_events_reloaded_based_roms'{Environment.NewLine}and merge these into your extracted game's events folder.{Environment.NewLine}It can be done before or after your randomization.{Environment.NewLine}{Environment.NewLine}See the README on the Project Page for details.", ButtonEnum.Ok, Icon.Warning);
		}

		if (_settings.GamePath != gamePath && Directory.Exists("backup"))
		{
			var msgbox = await Utils.ShowSimpleMessage("Shadow Randomizer", $"New game directory selected!{Environment.NewLine}{Environment.NewLine}Do you wish to erase the previous backup data and use the new data as a base?", ButtonEnum.YesNoCancel, Icon.Question);
			switch (msgbox)
			{
				case ButtonResult.Yes:
					Directory.Delete("backup", true);
					break;
				case ButtonResult.No:
					break;
				case ButtonResult.Cancel:
				default:
					buttonProcessing = false;
					return;
			}
		}

		_mainWindow.LoadMainView(gamePath, _settings);
	}
}