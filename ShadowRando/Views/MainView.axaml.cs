using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System;
using ShadowSET;
using ShadowFNT;
using AFSLib;
using System.Linq;
using HeroesONE_R.Structures.Common;
using HeroesONE_R.Structures;
using System.Diagnostics;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia;
using Avalonia.Controls;
using ShadowRando.Core;
using ShadowRando.Core.SETMutations;
using SkiaSharp;

namespace ShadowRando.Views;

public partial class MainView : UserControl
{
	const int stagefirst = 5;

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
	const int routeMenu6xxStagePreviewBlockerOffset = 0xB48B8;
	const int routeMenu6xxStagePreviewPatchValue = 0x48000110;
	const int storyModeStartAddress = 0x2CB9F0;
	const int firstStageOffset = 0x4C1BA8;
	const int modeOffset = 8;
	const int darkOffset = 0x1C;
	const int neutOffset = 0x28;
	const int heroOffset = 0x34;
	const int stageOffset = 0x50;
	const int shadowBoxPatchOffset = 0x3382E0;
	const int shadowBoxPatchValue = 0x480001B0;
	static readonly Dictionary<int, int> stageAssociationIDMap = new()
	{
			{ 5, 100 }, // first stage
			{ 6, 200 },
			{ 7, 201 },
			{ 8, 202 },
			{ 9, 300 },
			{ 10, 301 },
			{ 11, 302 },
			{ 12, 400 },
			{ 13, 401 },
			{ 14, 402 },
			{ 15, 403 },
			{ 16, 404 },
			{ 17, 500 },
			{ 18, 501 },
			{ 19, 502 },
			{ 20, 503 },
			{ 21, 504 },
			{ 22, 600 },
			{ 23, 601 },
			{ 24, 602 },
			{ 25, 603 },
			{ 26, 604 },
			{ 27, 700 }, // last stage
			{ 28, 210 }, // first sub boss
			{ 29, 310 },
			{ 30, 410 },
			{ 31, 411 },
			{ 32, 412 },
			{ 33, 510 },
			{ 34, 511 }, // last sub boss
			{ 35, 610 }, // first boss
			{ 36, 611 },
			{ 37, 612 },
			{ 38, 613 },
			{ 39, 614 },
			{ 40, 615 },
			{ 41, 616 },
			{ 42, 617 },
			{ 43, 618 },
			{ 44, 710 } // last boss
	};

	private const int totalstagecount = 40;
	private static int stagecount = 40;
	private int[] stageids;
	private readonly Stage[] stages = new Stage[totalstagecount];

	private static readonly Dictionary<int, Tuple<string, int>> Nukkoro2EnemyCountStages = new()
	{
			{ 100, Tuple.Create("City1",         2) }, // both
			{ 201, Tuple.Create("Canyon1",       1) }, // hero
			{ 301, Tuple.Create("PrisonIsland",  0) }, // dark
			{ 302, Tuple.Create("Circus",        0) }, // dark
			{ 401, Tuple.Create("ARKPast1",      0) }, // dark
			{ 404, Tuple.Create("Ruins",         1) }, // hero
			{ 501, Tuple.Create("Sky",           1) }, // hero
			{ 502, Tuple.Create("Jungle",        0) }, // dark
			{ 504, Tuple.Create("ARKPast2",      1) }, // hero
			{ 601, Tuple.Create("DoomsBase1",    0) }, // dark
	};

	private static readonly Type[] EnemyTypes = [
		typeof(Object0064_GUNSoldier),
		typeof(Object0065_GUNBeetle),
		typeof(Object0066_GUNBigfoot),
		typeof(Object0068_GUNRobot),
		typeof(Object0078_EggPierrot),
		typeof(Object0079_EggPawn),
		typeof(Object007A_EggShadowAndroid),
		typeof(Object008C_BkGiant),
		typeof(Object008D_BkSoldier),
		typeof(Object008E_BkWingLarge),
		typeof(Object008F_BkWingSmall),
		typeof(Object0090_BkWorm),
		typeof(Object0091_BkLarva),
		typeof(Object0092_BkChaos),
		typeof(Object0093_BkNinja),
	];

	private static readonly Type[] GroundEnemyTypes = [
		typeof(Object0064_GUNSoldier),
		typeof(Object0066_GUNBigfoot),
		typeof(Object0068_GUNRobot),
		typeof(Object0078_EggPierrot),
		typeof(Object0079_EggPawn),
		typeof(Object007A_EggShadowAndroid),
		typeof(Object008C_BkGiant),
		typeof(Object008D_BkSoldier),
		typeof(Object0090_BkWorm),
		typeof(Object0091_BkLarva),
		typeof(Object0093_BkNinja),
	];

	private static readonly Type[] FlyingEnemyTypes = [
		typeof(Object0065_GUNBeetle),
		typeof(Object008E_BkWingLarge),
		typeof(Object008F_BkWingSmall),
		typeof(Object0092_BkChaos),
		typeof(Object0066_GUNBigfoot), // only if AppearType is ZUTTO_HOVERING
		typeof(Object0093_BkNinja), // only if AppearType is ON_AIR_SAUCER_WARP
	];

	private static readonly EWeapon[] Weapons = [
		EWeapon.None,
		EWeapon.Pistol,
		EWeapon.SubmachineGun,
		EWeapon.MachineGun,
		EWeapon.HeavyMachineGun,
		EWeapon.GatlingGun,
		EWeapon.EggGun,
		EWeapon.LightShot,
		EWeapon.FlashShot,
		EWeapon.RingShot,
		EWeapon.HeavyShot,
		EWeapon.GrenadeLauncher,
		EWeapon.GUNBazooka,
		EWeapon.TankCannon,
		EWeapon.BlackBarrel,
		EWeapon.BigBarrel,
		EWeapon.EggBazooka,
		EWeapon.RPG,
		EWeapon.FourShot,
		EWeapon.EightShot,
		EWeapon.WormShooterBlack,
		EWeapon.WideWormShooterRed,
		EWeapon.BigWormShooterGold,
		EWeapon.VacuumPod,
		EWeapon.LaserRifle,
		EWeapon.Splitter,
		EWeapon.Refractor,
		EWeapon.Knife,
		EWeapon.BlackSword,
		EWeapon.DarkHammer,
		EWeapon.EggLance,
		EWeapon.SamuraiSwordLv1,
		EWeapon.SamuraiSwordLv2,
		EWeapon.SatelliteLaserLv1,
		EWeapon.SatelliteLaserLv2,
		EWeapon.EggVacLv1,
		EWeapon.EggVacLv2,
		EWeapon.OmochaoLv1,
		EWeapon.OmochaoLv2,
		EWeapon.HealCannonLv1,
		EWeapon.HealCannonLv2,
		EWeapon.ShadowRifle
	];

	const string programVersion = "0.5.0-dev";
	Settings settings;

	public MainView()
	{
		InitializeComponent();
	}

	private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (OperatingSystem.IsBrowser()) return; // TODO: Browser context only is allowed to read/write file dialogs if a user triggers the context, need to add buttons for browser to target
		if (topLevel is null) return;
		topLevel.IsVisible = false;
		settings = Settings.Load();

		// Program Configuration
		LevelOrder_Label_ProgramTitle.Content += " " + programVersion;

		// Level Order
		LevelOrder_TextBox_Seed.Text = settings.Seed;
		LevelOrder_CheckBox_Random_Seed.IsChecked = settings.RandomSeed;
		LevelOrder_ComboBox_Mode.SelectedIndex = (int)settings.LevelOrderMode;
		LevelOrder_ComboBox_MainPath.SelectedIndex = (int)settings.LevelOrderMainPath;
		LevelOrder_NumericUpDown_MaxForwardsJump.Value = settings.LevelOrderMaxForwardsJump;
		LevelOrder_NumericUpDown_MaxBackwardsJump.Value = settings.LevelOrderMaxBackwardsJump;
		LevelOrder_NumericUpDown_BackwardsJumpProbability.Value = settings.LevelOrderBackwardsJumpProbability;
		LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked = settings.LevelOrderAllowJumpsToSameLevel;
		LevelOrder_CheckBox_IncludeLastStory.IsChecked = settings.LevelOrderIncludeLastStory;
		LevelOrder_CheckBox_IncludeBosses.IsChecked = settings.LevelOrderIncludeBosses;
		LevelOrder_CheckBox_AllowBossToBoss.IsChecked = settings.LevelOrderAllowBossToBoss;

		// Layout
		Layout_CheckBox_RandomizeLayouts.IsChecked = settings.RandomizeLayouts;
		Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked = settings.LayoutMakeCCSplinesVehicleCompatible;
		// Enemy
		Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked = settings.LayoutAdjustMissionCounts;
		Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value = settings.LayoutAdjustMissionCountsReductionPercent;
		Layout_Enemy_ComboBox_Mode.SelectedIndex = (int)settings.LayoutEnemyMode;
		Layout_Enemy_CheckBox_KeepType.IsChecked = settings.LayoutEnemyKeepType;
		Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked = settings.LayoutEnemyOnlySelectedTypes;
		// Selected Enemies
		Layout_Enemy_CheckBox_SelectedEnemy_GUNSoldier.IsChecked = settings.LayoutEnemySelectedEnemyGUNSoldier;
		Layout_Enemy_CheckBox_SelectedEnemy_GUNBeetle.IsChecked = settings.LayoutEnemySelectedEnemyGUNBeetle;
		Layout_Enemy_CheckBox_SelectedEnemy_GUNBigfoot.IsChecked = settings.LayoutEnemySelectedEnemyGUNBigfoot;
		Layout_Enemy_CheckBox_SelectedEnemy_GUNRobot.IsChecked = settings.LayoutEnemySelectedEnemyGUNRobot;
		Layout_Enemy_CheckBox_SelectedEnemy_EggPierrot.IsChecked = settings.LayoutEnemySelectedEnemyEggPierrot;
		Layout_Enemy_CheckBox_SelectedEnemy_EggPawn.IsChecked = settings.LayoutEnemySelectedEnemyEggPawn;
		Layout_Enemy_CheckBox_SelectedEnemy_ShadowAndroid.IsChecked = settings.LayoutEnemySelectedEnemyShadowAndroid;
		Layout_Enemy_CheckBox_SelectedEnemy_BAGiant.IsChecked = settings.LayoutEnemySelectedEnemyBAGiant;
		Layout_Enemy_CheckBox_SelectedEnemy_BASoldier.IsChecked = settings.LayoutEnemySelectedEnemyBASoldier;
		Layout_Enemy_CheckBox_SelectedEnemy_BAHawkVolt.IsChecked = settings.LayoutEnemySelectedEnemyBAHawkVolt;
		Layout_Enemy_CheckBox_SelectedEnemy_BAWing.IsChecked = settings.LayoutEnemySelectedEnemyBAWing;
		Layout_Enemy_CheckBox_SelectedEnemy_BAWorm.IsChecked = settings.LayoutEnemySelectedEnemyBAWorm;
		Layout_Enemy_CheckBox_SelectedEnemy_BALarva.IsChecked = settings.LayoutEnemySelectedEnemyBALarva;
		Layout_Enemy_CheckBox_SelectedEnemy_ArtificialChaos.IsChecked = settings.LayoutEnemySelectedEnemyArtificialChaos;
		Layout_Enemy_CheckBox_SelectedEnemy_BAAssassin.IsChecked = settings.LayoutEnemySelectedEnemyBAAssassin;
		// Weapon
		Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked = settings.LayoutWeaponRandomWeaponsInAllBoxes;
		Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked = settings.LayoutWeaponRandomWeaponsInWeaponBoxes;
		Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked = settings.LayoutWeaponRandomExposedWeapons;
		Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked = settings.LayoutWeaponRandomWeaponsFromEnvironment;
		Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked = settings.LayoutWeaponOnlySelectedTypes;
		// Selected Weapons
		Layout_Weapon_CheckBox_SelectedWeapon_None.IsChecked = settings.LayoutWeaponSelectedWeaponNone;
		Layout_Weapon_CheckBox_SelectedWeapon_Pistol.IsChecked = settings.LayoutWeaponSelectedWeaponPistol;
		Layout_Weapon_CheckBox_SelectedWeapon_SubmachineGun.IsChecked = settings.LayoutWeaponSelectedWeaponSubmachineGun;
		Layout_Weapon_CheckBox_SelectedWeapon_AssaultRifle.IsChecked = settings.LayoutWeaponSelectedWeaponAssaultRifle;
		Layout_Weapon_CheckBox_SelectedWeapon_HeavyMachineGun.IsChecked = settings.LayoutWeaponSelectedWeaponHeavyMachineGun;
		Layout_Weapon_CheckBox_SelectedWeapon_GatlingGun.IsChecked = settings.LayoutWeaponSelectedWeaponGatlingGun;
		Layout_Weapon_CheckBox_SelectedWeapon_EggPistol.IsChecked = settings.LayoutWeaponSelectedWeaponEggPistol;
		Layout_Weapon_CheckBox_SelectedWeapon_LightShot.IsChecked = settings.LayoutWeaponSelectedWeaponLightShot;
		Layout_Weapon_CheckBox_SelectedWeapon_FlashShot.IsChecked = settings.LayoutWeaponSelectedWeaponFlashShot;
		Layout_Weapon_CheckBox_SelectedWeapon_RingShot.IsChecked = settings.LayoutWeaponSelectedWeaponRingShot;
		Layout_Weapon_CheckBox_SelectedWeapon_HeavyShot.IsChecked = settings.LayoutWeaponSelectedWeaponHeavyShot;
		Layout_Weapon_CheckBox_SelectedWeapon_GrenadeLauncher.IsChecked = settings.LayoutWeaponSelectedWeaponGrenadeLauncher;
		Layout_Weapon_CheckBox_SelectedWeapon_GUNBazooka.IsChecked = settings.LayoutWeaponSelectedWeaponGUNBazooka;
		Layout_Weapon_CheckBox_SelectedWeapon_TankCannon.IsChecked = settings.LayoutWeaponSelectedWeaponTankCannon;
		Layout_Weapon_CheckBox_SelectedWeapon_BlackBarrel.IsChecked = settings.LayoutWeaponSelectedWeaponBlackBarrel;
		Layout_Weapon_CheckBox_SelectedWeapon_BigBarrel.IsChecked = settings.LayoutWeaponSelectedWeaponBigBarrel;
		Layout_Weapon_CheckBox_SelectedWeapon_EggBazooka.IsChecked = settings.LayoutWeaponSelectedWeaponEggBazooka;
		Layout_Weapon_CheckBox_SelectedWeapon_RPG.IsChecked = settings.LayoutWeaponSelectedWeaponRPG;
		Layout_Weapon_CheckBox_SelectedWeapon_FourShot.IsChecked = settings.LayoutWeaponSelectedWeaponFourShot;
		Layout_Weapon_CheckBox_SelectedWeapon_EightShot.IsChecked = settings.LayoutWeaponSelectedWeaponEightShot;
		Layout_Weapon_CheckBox_SelectedWeapon_WormShooterBlack.IsChecked = settings.LayoutWeaponSelectedWeaponWormShooterBlack;
		Layout_Weapon_CheckBox_SelectedWeapon_WormShooterRed.IsChecked = settings.LayoutWeaponSelectedWeaponWormShooterRed;
		Layout_Weapon_CheckBox_SelectedWeapon_WormShooterGold.IsChecked = settings.LayoutWeaponSelectedWeaponWormShooterGold;
		Layout_Weapon_CheckBox_SelectedWeapon_VacuumPod.IsChecked = settings.LayoutWeaponSelectedWeaponVacuumPod;
		Layout_Weapon_CheckBox_SelectedWeapon_LaserRifle.IsChecked = settings.LayoutWeaponSelectedWeaponLaserRifle;
		Layout_Weapon_CheckBox_SelectedWeapon_Splitter.IsChecked = settings.LayoutWeaponSelectedWeaponSplitter;
		Layout_Weapon_CheckBox_SelectedWeapon_Refractor.IsChecked = settings.LayoutWeaponSelectedWeaponRefractor;
		Layout_Weapon_CheckBox_SelectedWeapon_Knife.IsChecked = settings.LayoutWeaponSelectedWeaponKnife;
		Layout_Weapon_CheckBox_SelectedWeapon_BlackSword.IsChecked = settings.LayoutWeaponSelectedWeaponBlackSword;
		Layout_Weapon_CheckBox_SelectedWeapon_DarkHammer.IsChecked = settings.LayoutWeaponSelectedWeaponDarkHammer;
		Layout_Weapon_CheckBox_SelectedWeapon_EggLance.IsChecked = settings.LayoutWeaponSelectedWeaponEggLance;
		Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv1.IsChecked = settings.LayoutWeaponSelectedWeaponSamuraiSwordLv1;
		Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv2.IsChecked = settings.LayoutWeaponSelectedWeaponSamuraiSwordLv2;
		Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv1.IsChecked = settings.LayoutWeaponSelectedWeaponSatelliteLaserLv1;
		Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv2.IsChecked = settings.LayoutWeaponSelectedWeaponSatelliteLaserLv2;
		Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv1.IsChecked = settings.LayoutWeaponSelectedWeaponEggVacuumLv1;
		Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv2.IsChecked = settings.LayoutWeaponSelectedWeaponEggVacuumLv2;
		Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv1.IsChecked = settings.LayoutWeaponSelectedWeaponOmochaoGunLv1;
		Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv2.IsChecked = settings.LayoutWeaponSelectedWeaponOmochaoGunLv2;
		Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv1.IsChecked = settings.LayoutWeaponSelectedWeaponHealCannonLv1;
		Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv2.IsChecked = settings.LayoutWeaponSelectedWeaponHealCannonLv2;
		Layout_Weapon_CheckBox_SelectedWeapon_ShadowRifle.IsChecked = settings.LayoutWeaponSelectedWeaponShadowRifle;
		// Partner
		Layout_Partner_ComboBox_Mode.SelectedIndex = (int)settings.LayoutPartnerMode;
		Layout_Partner_CheckBox_KeepAffiliationOfOriginalObject.IsChecked = settings.LayoutPartnerKeepOriginalObjectAffiliation;

		// Subtitles
		Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked = settings.RandomizeSubtitlesVoicelines;
		Subtitles_CheckBox_NoDuplicates.IsChecked = settings.SubtitlesNoDuplicates;
		Subtitles_CheckBox_NoSystemMessages.IsChecked = settings.SubtitlesNoSystemMessages;
		Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked = settings.SubtitlesOnlyLinkedAudio;
		Subtitles_CheckBox_OnlySelectedCharacters.IsChecked = settings.SubtitlesOnlySelectedCharacters;
		Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked = settings.SubtitlesGiveAudioToNoLinkedAudio;

		// Selected Characters
		Subtitles_CheckBox_SelectedCharacter_Shadow.IsChecked = settings.SubtitlesSelectedCharacterShadow;
		Subtitles_CheckBox_SelectedCharacter_Sonic.IsChecked = settings.SubtitlesSelectedCharacterSonic;
		Subtitles_CheckBox_SelectedCharacter_Tails.IsChecked = settings.SubtitlesSelectedCharacterTails;
		Subtitles_CheckBox_SelectedCharacter_Knuckles.IsChecked = settings.SubtitlesSelectedCharacterKnuckles;
		Subtitles_CheckBox_SelectedCharacter_Amy.IsChecked = settings.SubtitlesSelectedCharacterAmy;
		Subtitles_CheckBox_SelectedCharacter_Rouge.IsChecked = settings.SubtitlesSelectedCharacterRouge;
		Subtitles_CheckBox_SelectedCharacter_Omega.IsChecked = settings.SubtitlesSelectedCharacterOmega;
		Subtitles_CheckBox_SelectedCharacter_Vector.IsChecked = settings.SubtitlesSelectedCharacterVector;
		Subtitles_CheckBox_SelectedCharacter_Espio.IsChecked = settings.SubtitlesSelectedCharacterEspio;
		Subtitles_CheckBox_SelectedCharacter_Maria.IsChecked = settings.SubtitlesSelectedCharacterMaria;
		Subtitles_CheckBox_SelectedCharacter_Charmy.IsChecked = settings.SubtitlesSelectedCharacterCharmy;
		Subtitles_CheckBox_SelectedCharacter_Eggman.IsChecked = settings.SubtitlesSelectedCharacterEggman;
		Subtitles_CheckBox_SelectedCharacter_BlackDoom.IsChecked = settings.SubtitlesSelectedCharacterBlackDoom;
		Subtitles_CheckBox_SelectedCharacter_Cream.IsChecked = settings.SubtitlesSelectedCharacterCream;
		Subtitles_CheckBox_SelectedCharacter_Cheese.IsChecked = settings.SubtitlesSelectedCharacterCheese;
		Subtitles_CheckBox_SelectedCharacter_GUNCommander.IsChecked = settings.SubtitlesSelectedCharacterGUNCommander;
		Subtitles_CheckBox_SelectedCharacter_GUNSoldier.IsChecked = settings.SubtitlesSelectedCharacterGUNSoldier;

		// Music
		Music_CheckBox_RandomizeMusic.IsChecked = settings.RandomizeMusic;
		Music_CheckBox_SkipChaosPowerUseJingles.IsChecked = settings.MusicSkipChaosPowers;
		Music_CheckBox_SkipRankTheme.IsChecked = settings.MusicSkipRankTheme;

		// Models
		Models_CheckBox_RandomizeModel.IsChecked = settings.RandomizeModel;
		Models_CheckBox_ModelP2.IsChecked = settings.RandomizeP2Model;

		LoadGameData();
	}

	private void UserControl_Unloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		// Level Order
		settings.Seed = LevelOrder_TextBox_Seed.Text ?? string.Empty;
		settings.RandomSeed = LevelOrder_CheckBox_Random_Seed.IsChecked.Value;
		settings.LevelOrderMode = (LevelOrderMode)LevelOrder_ComboBox_Mode.SelectedIndex;
		settings.LevelOrderMainPath = (LevelOrderMainPath)LevelOrder_ComboBox_MainPath.SelectedIndex;
		settings.LevelOrderMaxForwardsJump = (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value;
		settings.LevelOrderMaxBackwardsJump = (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value;
		settings.LevelOrderBackwardsJumpProbability = (int)LevelOrder_NumericUpDown_BackwardsJumpProbability.Value;
		settings.LevelOrderAllowJumpsToSameLevel = LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value;
		settings.LevelOrderIncludeLastStory = LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value;
		settings.LevelOrderIncludeBosses = LevelOrder_CheckBox_IncludeBosses.IsChecked.Value;
		settings.LevelOrderAllowBossToBoss = LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value;

		// Layout
		settings.RandomizeLayouts = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		settings.LayoutMakeCCSplinesVehicleCompatible = Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked.Value;
		// Enemy
		settings.LayoutAdjustMissionCounts = Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value;
		settings.LayoutAdjustMissionCountsReductionPercent = (int)Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value;
		settings.LayoutEnemyMode = (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex;
		settings.LayoutEnemyKeepType = Layout_Enemy_CheckBox_KeepType.IsChecked.Value;
		settings.LayoutEnemyOnlySelectedTypes = Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value;
		// Selected Enemies
		settings.LayoutEnemySelectedEnemyGUNSoldier = Layout_Enemy_CheckBox_SelectedEnemy_GUNSoldier.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyGUNBeetle = Layout_Enemy_CheckBox_SelectedEnemy_GUNBeetle.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyGUNBigfoot = Layout_Enemy_CheckBox_SelectedEnemy_GUNBigfoot.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyGUNRobot = Layout_Enemy_CheckBox_SelectedEnemy_GUNRobot.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyEggPierrot = Layout_Enemy_CheckBox_SelectedEnemy_EggPierrot.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyEggPawn = Layout_Enemy_CheckBox_SelectedEnemy_EggPawn.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyShadowAndroid = Layout_Enemy_CheckBox_SelectedEnemy_ShadowAndroid.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBAGiant = Layout_Enemy_CheckBox_SelectedEnemy_BAGiant.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBASoldier = Layout_Enemy_CheckBox_SelectedEnemy_BASoldier.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBAHawkVolt = Layout_Enemy_CheckBox_SelectedEnemy_BAHawkVolt.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBAWing = Layout_Enemy_CheckBox_SelectedEnemy_BAWing.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBAWorm = Layout_Enemy_CheckBox_SelectedEnemy_BAWorm.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBALarva = Layout_Enemy_CheckBox_SelectedEnemy_BALarva.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyArtificialChaos = Layout_Enemy_CheckBox_SelectedEnemy_ArtificialChaos.IsChecked.Value;
		settings.LayoutEnemySelectedEnemyBAAssassin = Layout_Enemy_CheckBox_SelectedEnemy_BAAssassin.IsChecked.Value;
		// Weapon
		settings.LayoutWeaponRandomWeaponsInAllBoxes = Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value;
		settings.LayoutWeaponRandomWeaponsInWeaponBoxes = Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value;
		settings.LayoutWeaponRandomExposedWeapons = Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked.Value;
		settings.LayoutWeaponRandomWeaponsFromEnvironment = Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked.Value;
		settings.LayoutWeaponOnlySelectedTypes = Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value;
		// Selected Weapons
		settings.LayoutWeaponSelectedWeaponNone = Layout_Weapon_CheckBox_SelectedWeapon_None.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponPistol = Layout_Weapon_CheckBox_SelectedWeapon_Pistol.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSubmachineGun = Layout_Weapon_CheckBox_SelectedWeapon_SubmachineGun.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponAssaultRifle = Layout_Weapon_CheckBox_SelectedWeapon_AssaultRifle.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponHeavyMachineGun = Layout_Weapon_CheckBox_SelectedWeapon_HeavyMachineGun.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponGatlingGun = Layout_Weapon_CheckBox_SelectedWeapon_GatlingGun.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEggPistol = Layout_Weapon_CheckBox_SelectedWeapon_EggPistol.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponLightShot = Layout_Weapon_CheckBox_SelectedWeapon_LightShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponFlashShot = Layout_Weapon_CheckBox_SelectedWeapon_FlashShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponRingShot = Layout_Weapon_CheckBox_SelectedWeapon_RingShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponHeavyShot = Layout_Weapon_CheckBox_SelectedWeapon_HeavyShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponGrenadeLauncher = Layout_Weapon_CheckBox_SelectedWeapon_GrenadeLauncher.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponGUNBazooka = Layout_Weapon_CheckBox_SelectedWeapon_GUNBazooka.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponTankCannon = Layout_Weapon_CheckBox_SelectedWeapon_TankCannon.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponBlackBarrel = Layout_Weapon_CheckBox_SelectedWeapon_BlackBarrel.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponBigBarrel = Layout_Weapon_CheckBox_SelectedWeapon_BigBarrel.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEggBazooka = Layout_Weapon_CheckBox_SelectedWeapon_EggBazooka.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponRPG = Layout_Weapon_CheckBox_SelectedWeapon_RPG.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponFourShot = Layout_Weapon_CheckBox_SelectedWeapon_FourShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEightShot = Layout_Weapon_CheckBox_SelectedWeapon_EightShot.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponWormShooterBlack = Layout_Weapon_CheckBox_SelectedWeapon_WormShooterBlack.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponWormShooterRed = Layout_Weapon_CheckBox_SelectedWeapon_WormShooterRed.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponWormShooterGold = Layout_Weapon_CheckBox_SelectedWeapon_WormShooterGold.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponVacuumPod = Layout_Weapon_CheckBox_SelectedWeapon_VacuumPod.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponLaserRifle = Layout_Weapon_CheckBox_SelectedWeapon_LaserRifle.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSplitter = Layout_Weapon_CheckBox_SelectedWeapon_Splitter.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponRefractor = Layout_Weapon_CheckBox_SelectedWeapon_Refractor.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponKnife = Layout_Weapon_CheckBox_SelectedWeapon_Knife.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponBlackSword = Layout_Weapon_CheckBox_SelectedWeapon_BlackSword.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponDarkHammer = Layout_Weapon_CheckBox_SelectedWeapon_DarkHammer.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEggLance = Layout_Weapon_CheckBox_SelectedWeapon_EggLance.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSamuraiSwordLv1 = Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv1.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSamuraiSwordLv2 = Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv2.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSatelliteLaserLv1 = Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv1.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponSatelliteLaserLv2 = Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv2.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEggVacuumLv1 = Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv1.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponEggVacuumLv2 = Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv2.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponOmochaoGunLv1 = Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv1.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponOmochaoGunLv2 = Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv2.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponHealCannonLv1 = Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv1.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponHealCannonLv2 = Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv2.IsChecked.Value;
		settings.LayoutWeaponSelectedWeaponShadowRifle = Layout_Weapon_CheckBox_SelectedWeapon_ShadowRifle.IsChecked.Value;
		// Partner
		settings.LayoutPartnerMode = (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex;
		settings.LayoutPartnerKeepOriginalObjectAffiliation = Layout_Partner_CheckBox_KeepAffiliationOfOriginalObject.IsChecked.Value;

		// Subtitles
		settings.RandomizeSubtitlesVoicelines = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		settings.SubtitlesNoDuplicates = Subtitles_CheckBox_NoDuplicates.IsChecked.Value;
		settings.SubtitlesNoSystemMessages = Subtitles_CheckBox_NoSystemMessages.IsChecked.Value;
		settings.SubtitlesOnlyLinkedAudio = Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value;
		settings.SubtitlesOnlySelectedCharacters = Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value;
		settings.SubtitlesGiveAudioToNoLinkedAudio = Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value;
		// Selected Characters
		settings.SubtitlesSelectedCharacterShadow = Subtitles_CheckBox_SelectedCharacter_Shadow.IsChecked.Value;
		settings.SubtitlesSelectedCharacterSonic = Subtitles_CheckBox_SelectedCharacter_Sonic.IsChecked.Value;
		settings.SubtitlesSelectedCharacterTails = Subtitles_CheckBox_SelectedCharacter_Tails.IsChecked.Value;
		settings.SubtitlesSelectedCharacterKnuckles = Subtitles_CheckBox_SelectedCharacter_Knuckles.IsChecked.Value;
		settings.SubtitlesSelectedCharacterAmy = Subtitles_CheckBox_SelectedCharacter_Amy.IsChecked.Value;
		settings.SubtitlesSelectedCharacterRouge = Subtitles_CheckBox_SelectedCharacter_Rouge.IsChecked.Value;
		settings.SubtitlesSelectedCharacterOmega = Subtitles_CheckBox_SelectedCharacter_Omega.IsChecked.Value;
		settings.SubtitlesSelectedCharacterVector = Subtitles_CheckBox_SelectedCharacter_Vector.IsChecked.Value;
		settings.SubtitlesSelectedCharacterEspio = Subtitles_CheckBox_SelectedCharacter_Espio.IsChecked.Value;
		settings.SubtitlesSelectedCharacterMaria = Subtitles_CheckBox_SelectedCharacter_Maria.IsChecked.Value;
		settings.SubtitlesSelectedCharacterCharmy = Subtitles_CheckBox_SelectedCharacter_Charmy.IsChecked.Value;
		settings.SubtitlesSelectedCharacterEggman = Subtitles_CheckBox_SelectedCharacter_Eggman.IsChecked.Value;
		settings.SubtitlesSelectedCharacterBlackDoom = Subtitles_CheckBox_SelectedCharacter_BlackDoom.IsChecked.Value;
		settings.SubtitlesSelectedCharacterCream = Subtitles_CheckBox_SelectedCharacter_Cream.IsChecked.Value;
		settings.SubtitlesSelectedCharacterCheese = Subtitles_CheckBox_SelectedCharacter_Cheese.IsChecked.Value;
		settings.SubtitlesSelectedCharacterGUNCommander = Subtitles_CheckBox_SelectedCharacter_GUNCommander.IsChecked.Value;
		settings.SubtitlesSelectedCharacterGUNSoldier = Subtitles_CheckBox_SelectedCharacter_GUNSoldier.IsChecked.Value;

		// Music
		settings.RandomizeMusic = Music_CheckBox_RandomizeMusic.IsChecked.Value;
		settings.MusicSkipChaosPowers = Music_CheckBox_SkipChaosPowerUseJingles.IsChecked.Value;
		settings.MusicSkipRankTheme = Music_CheckBox_SkipRankTheme.IsChecked.Value;

		// Models
		settings.RandomizeModel = Models_CheckBox_RandomizeModel.IsChecked.Value;
		settings.RandomizeP2Model = Models_CheckBox_ModelP2.IsChecked.Value;

		settings.Save();
	}

	private async void LoadGameData()
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel == null) return;
		var folderPath = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = "Select the root folder of an extracted Shadow the Hedgehog disc image.",
		});

		if (folderPath.Count <= 0)
		{
			if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktopApp)
			{
				desktopApp.Shutdown();
			}
			return;
		}
		
		if (settings.GamePath != folderPath[0].Path.LocalPath && Directory.Exists("backup"))
		{
			var msgbox = MessageBoxManager.GetMessageBoxStandard("Shadow Randomizer", "New game directory selected!\n\nDo you wish to erase the previous backup data and use the new data as a base?", ButtonEnum.YesNo, Icon.Question);
			var result = await msgbox.ShowAsync();
			switch (result)
			{
				case ButtonResult.Yes:
					Directory.Delete("backup", true);
					break;
				case ButtonResult.No:
					break;
				default:
					break;
			}
		}
		settings.GamePath = folderPath[0].Path.LocalPath;
		if (!Directory.Exists("backup"))
			Directory.CreateDirectory("backup");
		if (!File.Exists(Path.Combine("backup", "main.dol")))
			File.Copy(Path.Combine(settings.GamePath, "sys", "main.dol"), Path.Combine("backup", "main.dol"));
		if (!File.Exists(Path.Combine("backup", "bi2.bin")))
			File.Copy(Path.Combine(settings.GamePath, "sys", "bi2.bin"), Path.Combine("backup", "bi2.bin"));
		if (!File.Exists(Path.Combine("backup", "setid.bin")))
			File.Copy(Path.Combine(settings.GamePath, "files", "setid.bin"), Path.Combine("backup", "setid.bin"));
		if (!File.Exists(Path.Combine("backup", "nukkoro2.inf")))
			File.Copy(Path.Combine(settings.GamePath, "files", "nukkoro2.inf"), Path.Combine("backup", "nukkoro2.inf"));
		if (!Directory.Exists(Path.Combine("backup", "fonts")))
			CopyDirectory(Path.Combine(settings.GamePath, "files", "fonts"), Path.Combine("backup", "fonts"));
		if (!Directory.Exists(Path.Combine("backup", "music")))
		{
			Directory.CreateDirectory(Path.Combine("backup", "music"));
			foreach (var fil in Directory.EnumerateFiles(Path.Combine(settings.GamePath, "files"), "*.adx"))
				File.Copy(fil, Path.Combine("backup", "music", Path.GetFileName(fil)));
		}
		if (!Directory.Exists(Path.Combine("backup", "character")))
			CopyDirectory(Path.Combine(settings.GamePath, "files", "character"), Path.Combine("backup", "character"));
		if (!Directory.Exists(Path.Combine("backup", "sets")))
		{
			Directory.CreateDirectory(Path.Combine("backup", "sets"));
			for (var stageIdToModify = 5; stageIdToModify < 45; stageIdToModify++)
			{
				stageAssociationIDMap.TryGetValue(stageIdToModify, out var stageId);
				var stageDataIdentifier = "stg0" + stageId;
				var datOne = stageDataIdentifier + "_dat.one";
				var cmnLayout = stageDataIdentifier + "_cmn.dat";
				var nrmLayout = stageDataIdentifier + "_nrm.dat";
				var hrdLayout = stageDataIdentifier + "_hrd.dat";
				var ds1Layout = stageDataIdentifier + "_ds1.dat";
				var datOnePath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, datOne);
				var cmnLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, cmnLayout);
				var nrmLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, nrmLayout);
				var hrdLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, hrdLayout);
				var ds1LayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, ds1Layout);

				if (!Directory.Exists(Path.Combine("backup", "sets", stageDataIdentifier)))
					Directory.CreateDirectory(Path.Combine("backup", "sets", stageDataIdentifier));
				File.Copy(datOnePath, Path.Combine("backup", "sets", stageDataIdentifier, datOne));
				File.Copy(cmnLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, cmnLayout));
				try { File.Copy(nrmLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, nrmLayout)); } catch (FileNotFoundException) { } // some stages don't have nrm
				try { File.Copy(hrdLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, hrdLayout)); } catch (FileNotFoundException) { } // some stages don't have hrd
				try { File.Copy(ds1LayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, ds1Layout)); } catch (FileNotFoundException) { } // some stages don't have ds1
			}
		}
		topLevel.IsVisible = true;
	}

	private static int CalculateSeed(string seedString)
	{
		return BitConverter.ToInt32(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(seedString)), 0);
	}

	private void Button_Randomize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		ProgressBar_RandomizationProgress.Value = 0;
		RandomizationProcess();
		// Task.Run(() => RandomizationProcess()); // We can't do this (yet) until we properly MVVM-ify since the UI Thread is actively used to evaluate CheckBox state
	}

	private async void RandomizationProcess()
	{
		byte[] dolfile = File.ReadAllBytes(Path.Combine("backup", "main.dol"));
		if (LevelOrder_CheckBox_Random_Seed.IsChecked.Value)
		{
			var randomBytes = new byte[10];
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(randomBytes);
			}
			LevelOrder_TextBox_Seed.Text = Convert.ToBase64String(randomBytes);
		}
		if (string.IsNullOrEmpty(LevelOrder_TextBox_Seed.Text))
		{
			ShowSimpleMessage("Error", "Invalid Seed", ButtonEnum.Ok, Icon.Error);
			ProgressBar_RandomizationProgress.Value = 0;
			return;
		}

		var seed = CalculateSeed(LevelOrder_TextBox_Seed.Text);
		settings.LevelOrderMode = (LevelOrderMode)LevelOrder_ComboBox_Mode.SelectedIndex;
		Random r = new Random(seed);
		byte[] buf;
		List<int> tmpids = new List<int>(totalstagecount + 1);
		for (int i = 0; i < totalstagecount; i++)
		{
			stages[i] = new Stage(i);
			buf = new byte[4];
			Array.Copy(dolfile, firstStageOffset + (i * stageOffset) + modeOffset, buf, 0, 4);
			Array.Reverse(buf);
			int mode = BitConverter.ToInt32(buf, 0);
			if ((mode & 0x10) == 0x10)
				stages[i].IsLast = true;
			if ((mode & 1) == 0)
			{
				if (!stages[i].IsLast)
				{
					stages[i].HasDark = BitConverter.ToInt32(dolfile, firstStageOffset + (i * stageOffset) + darkOffset + 4) != -1;
					stages[i].HasNeutral = BitConverter.ToInt32(dolfile, firstStageOffset + (i * stageOffset) + neutOffset + 4) != -1;
					stages[i].HasHero = BitConverter.ToInt32(dolfile, firstStageOffset + (i * stageOffset) + heroOffset + 4) != -1;
				}
				else
					stages[i].HasNeutral = true;
			}
			else
				stages[i].IsBoss = true;
			bool include = true;
			if (!LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value)
				include = !stages[i].IsLast;
			if (settings.LevelOrderMode == LevelOrderMode.BossRush)
				include &= stages[i].IsBoss;
			else if (!LevelOrder_CheckBox_IncludeBosses.IsChecked.Value)
				include &= !stages[i].IsBoss;
			if (include)
				tmpids.Add(i);
		}
		stagecount = tmpids.Count;
		tmpids.Add(totalstagecount);
		stageids = tmpids.ToArray();
		switch (settings.LevelOrderMode)
		{
			case LevelOrderMode.Original:
				break;
			case LevelOrderMode.AllStagesWarps:
				{
					if (!LevelOrder_CheckBox_IncludeBosses.IsChecked.Value || LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value)
						Shuffle(r, stageids, stagecount);
					else
					{
						int[] tmparr = stageids.Take(stagecount).Where(a => !stages[a].IsBoss).ToArray();
						Shuffle(r, tmparr);
						tmpids = tmparr.ToList();
						tmparr = stageids.Take(stagecount).Where(a => stages[a].IsBoss).ToArray();
						Shuffle(r, tmparr);
						int[] inds = Enumerable.Range(0, tmpids.Count).ToArray();
						Shuffle(r, inds);
						inds = inds.Take(tmparr.Length).ToArray();
						Array.Sort(inds);
						for (int i = 0; i < tmparr.Length; i++)
							tmpids.Insert(inds[inds.Length - i - 1], tmparr[i]);
						tmpids.CopyTo(stageids);
					}
					switch ((LevelOrderMainPath)LevelOrder_ComboBox_MainPath.SelectedIndex)
					{
						case LevelOrderMainPath.ActClear:
							for (int i = 0; i < stagecount; i++)
								stages[stageids[i]].SetExit(0, stageids[i + 1]);
							break;
						case LevelOrderMainPath.AnyExit:
							for (int i = 0; i < stagecount; i++)
								stages[stageids[i]].SetExit(r.Next(stages[stageids[i]].CountExits()), stageids[i + 1]);
							break;
					}
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[stageids[i]];
						int min, max;
						if (stg.HasNeutral && stg.Neutral == -1)
						{
							if (r.Next(100) < LevelOrder_NumericUpDown_BackwardsJumpProbability.Value && (i > 0 || LevelOrder_NumericUpDown_BackwardsJumpProbability.Value == 100))
							{
								min = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value, 0);
								max = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Minimum + 1, 0);
							}
							else
							{
								min = i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Minimum;
								max = Math.Min(i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value + 1, stagecount + 1);
							}
							stg.Neutral = stageids[r.Next(min, max)];
						}
						if (stg.HasHero && stg.Hero == -1)
						{
							if (r.Next(100) < LevelOrder_NumericUpDown_BackwardsJumpProbability.Value && (i > 0 || LevelOrder_NumericUpDown_BackwardsJumpProbability.Value == 100))
							{
								min = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value, 0);
								max = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Minimum + 1, 0);
							}
							else
							{
								min = i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Minimum;
								max = Math.Min(i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value + 1, stagecount + 1);
							}
							stg.Hero = stageids[r.Next(min, max)];
						}
						if (stg.HasDark && stg.Dark == -1)
						{
							if (r.Next(100) < LevelOrder_NumericUpDown_BackwardsJumpProbability.Value && (i > 0 || LevelOrder_NumericUpDown_BackwardsJumpProbability.Value == 100))
							{
								min = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value, 0);
								max = Math.Max(i - (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Minimum + 1, 0);
							}
							else
							{
								min = i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Minimum;
								max = Math.Min(i + (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value + 1, stagecount + 1);
							}
							stg.Dark = stageids[r.Next(min, max)];
						}
					}
				}
				break;
			case LevelOrderMode.VanillaStructure:
				{
					List<int> twoexitlst = new List<int>();
					List<int> threeexitlst = new List<int>();
					List<int> bosslst = new List<int>();
					List<int> last = new List<int>();
					for (int i = 0; i < stagecount; i++)
					{
						var stg = stages[stageids[i]];
						if (stg.IsLast)
							last.Add(stageids[i]);
						else if (stg.IsBoss)
							bosslst.Add(stageids[i]);
						else if (stg.CountExits() == 3)
							threeexitlst.Add(stageids[i]);
						else
							twoexitlst.Add(stageids[i]);
					}
					int[] twoexit = twoexitlst.ToArray();
					int[] threeexit = threeexitlst.ToArray();
					int[] boss = bosslst.ToArray();
					Shuffle(r, twoexit);
					Shuffle(r, threeexit);
					Shuffle(r, boss);
					Queue<int> twoq = new Queue<int>(twoexit);
					Queue<int> threeq = new Queue<int>(threeexit);
					Queue<int> bossq = new Queue<int>(boss);
					List<int> neword = new List<int>(stagecount);
					foreach (var set in ShadowStageSet.StageList)
					{
						foreach (var stg in set.stages)
							switch (stg.stageType)
							{
								case StageType.Neutral:
									neword.Add(threeq.Dequeue());
									break;
								case StageType.Hero:
								case StageType.Dark:
								case StageType.End:
									neword.Add(twoq.Dequeue());
									break;
							}
						if (LevelOrder_CheckBox_IncludeBosses.IsChecked.Value)
							for (int i = 0; i < set.bossCount; i++)
								neword.Add(bossq.Dequeue());
					}
					neword.AddRange(last);
					int ind = 0;
					foreach (var set in ShadowStageSet.StageList)
					{
						int bossind = ind + set.stages.Count;
						int next = set.stages.Count + (LevelOrder_CheckBox_IncludeBosses.IsChecked.Value ? set.bossCount : 0);
						if (set.stages[0].stageType == StageType.Neutral)
							++next;
						foreach (var item in set.stages)
						{
							Stage stg = stages[neword[ind]];
							int bosscnt = LevelOrder_CheckBox_IncludeBosses.IsChecked.Value ? item.bossCount : 0;
							if (bosscnt == 2)
							{
								stg.SetExit(0, neword[bossind]);
								stages[neword[bossind++]].Neutral = totalstagecount;
								stg.SetExit(1, neword[bossind]);
								stages[neword[bossind++]].Neutral = totalstagecount;
							}
							else if (bosscnt == 1)
							{
								Stage bossstg = stages[neword[bossind]];
								switch (item.stageType)
								{
									case StageType.Neutral:
										bossstg.Dark = neword[ind + next - 1];
										bossstg.Neutral = neword[ind + next];
										bossstg.Hero = neword[ind + next + 1];
										break;
									case StageType.Dark:
										if (stg.HasDark)
										{
											bossstg.Dark = neword[ind + next];
											if (stg.HasNeutral)
												bossstg.Neutral = neword[ind + next + 1];
											else
												bossstg.Hero = neword[ind + next + 1];
										}
										else
										{
											bossstg.Neutral = neword[ind + next];
											bossstg.Hero = neword[ind + next + 1];
										}
										break;
									case StageType.Hero:
										if (stg.HasHero)
										{
											bossstg.Hero = neword[ind + next];
											if (stg.HasNeutral)
												bossstg.Neutral = neword[ind + next - 1];
											else
												bossstg.Dark = neword[ind + next - 1];
										}
										else
										{
											bossstg.Neutral = neword[ind + next];
											bossstg.Dark = neword[ind + next - 1];
										}
										break;
									case StageType.End:
										bossstg.Neutral = totalstagecount;
										break;
								}
								if (stg.HasNeutral)
									stg.Neutral = neword[bossind];
								if (stg.HasHero)
									stg.Hero = neword[bossind];
								if (stg.HasDark)
									stg.Dark = neword[bossind];
								bossind++;
							}
							else
							{
								switch (item.stageType)
								{
									case StageType.Neutral:
										stg.Dark = neword[ind + next - 1];
										stg.Neutral = neword[ind + next];
										stg.Hero = neword[ind + next + 1];
										break;
									case StageType.Dark:
										if (stg.HasDark)
										{
											stg.Dark = neword[ind + next];
											if (stg.HasNeutral)
												stg.Neutral = neword[ind + next + 1];
											else
												stg.Hero = neword[ind + next + 1];
										}
										else
										{
											stg.Neutral = neword[ind + next];
											stg.Hero = neword[ind + next + 1];
										}
										break;
									case StageType.Hero:
										if (stg.HasHero)
										{
											stg.Hero = neword[ind + next];
											if (stg.HasNeutral)
												stg.Neutral = neword[ind + next - 1];
											else
												stg.Dark = neword[ind + next - 1];
										}
										else
										{
											stg.Neutral = neword[ind + next];
											stg.Dark = neword[ind + next - 1];
										}
										break;
									case StageType.End:
										stg.Hero = totalstagecount;
										stg.Dark = totalstagecount;
										break;
								}
							}
							++ind;
						}
						if (LevelOrder_CheckBox_IncludeBosses.IsChecked.Value)
							ind += set.bossCount;
					}
					neword.CopyTo(stageids);
				}
				break;
			case LevelOrderMode.BranchingPaths:
				{
					List<int> stagepool = new List<int>(stageids.Take(stagecount));
					List<int> curset = new List<int>() { r.Next(stagecount) };
					stagepool.Remove(curset[0]);
					List<int> ids2 = new List<int>() { curset[0] };
					while (stagepool.Count > 0)
					{
						List<int> newset = new List<int>();
						for (int i = 0; i < curset.Count; i++)
						{
							Stage stg = stages[curset[i]];
							int next;
							int l = 0;
							do
							{
								next = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
							}
							while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
							stg.SetExit(0, next);
							if (!newset.Contains(next))
								newset.Add(next);
							if (stg.HasHero && stg.Hero == -1)
							{
								l = 0;
								do
								{
									next = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
								}
								while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
								stg.Hero = next;
								if (!newset.Contains(stg.Hero))
									newset.Add(stg.Hero);
							}
							if (stg.HasDark && stg.Dark == -1)
							{
								l = 0;
								do
								{
									next = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
								}
								while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
								stg.Dark = next;
								if (!newset.Contains(stg.Dark))
									newset.Add(stg.Dark);
							}
						}
						stagepool.RemoveAll(newset.Contains);
						curset = newset;
						ids2.AddRange(newset);
					}
					foreach (Stage stg in curset.Select(a => stages[a]))
					{
						stg.SetExit(0, totalstagecount);
						if (stg.HasHero)
							stg.Hero = totalstagecount;
						if (stg.HasDark)
							stg.Dark = totalstagecount;
					}
					ids2.CopyTo(stageids);
				}
				break;
			case LevelOrderMode.ReverseBranching:
				{
					int exitcnt = stages.Sum(a => a.CountExits()) - stages.Count(a => a.CountExits() == 1);
					Shuffle(r, stageids, stagecount);
					Stack<int> stagepool = new Stack<int>(stageids.Take(stagecount));
					List<int> usedstg = new List<int>(stagecount + 1) { totalstagecount };
					List<int> orphans = new List<int>();
					int[] stagedepths = new int[totalstagecount + 1];
					List<List<int>> depthstages = new List<List<int>>() { new List<int>() { totalstagecount } };
					while (orphans.Count < exitcnt - stages[stagepool.Peek()].CountExits())
					{
						int stgid = stagepool.Pop();
						Stage stg = stages[stgid];
						exitcnt -= stg.CountExits();
						int next;
						int l = 0;
						do
						{
							next = GetStageFromLists(r, orphans, usedstg, 2);
						}
						while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
						int depth = stagedepths[next] + 1;
						stagedepths[stgid] = depth;
						while (depthstages.Count <= depth)
							depthstages.Add(new List<int>());
						depthstages[depth].Add(stgid);
						stg.SetExit(r.Next(stg.CountExits()), next);
						orphans.Remove(next);
						usedstg.Add(stgid);
						orphans.Add(stgid);
					}
					while (stagepool.Count > 0)
					{
						int stgid = stagepool.Pop();
						Stage stg = stages[stgid];
						int next;
						int depth = 0;
						if (stg.IsBoss || stg.HasNeutral)
						{
							int l = 0;
							do
							{
								next = orphans[r.Next(orphans.Count)];
							}
							while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
							stg.Neutral = next;
							orphans.Remove(next);
							depth = stagedepths[next] + 1;
						}
						if (orphans.Count > 0 && stg.HasHero)
						{
							next = orphans[r.Next(orphans.Count)];
							stg.Hero = next;
							orphans.Remove(next);
							depth = Math.Max(depth, stagedepths[next] + 1);
						}
						if (orphans.Count > 0 && stg.HasDark)
						{
							next = orphans[r.Next(orphans.Count)];
							stg.Dark = next;
							orphans.Remove(next);
							depth = Math.Max(depth, stagedepths[next] + 1);
						}
						stagedepths[stgid] = depth;
						while (depthstages.Count <= depth)
							depthstages.Add(new List<int>());
						depthstages[depth].Add(stgid);
						orphans.Add(stgid);
					}
					foreach (Stage stg in stages)
					{
						if (!LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value && stg.IsLast)
							continue;
						if ((stg.IsBoss || stg.HasNeutral) && stg.Neutral == -1)
						{
							var pool = depthstages[Math.Min(stagedepths[stg.ID] + r.Next(-1, 2), depthstages.Count - 1)];
							int next;
							int l = 0;
							do
							{
								next = pool[r.Next(pool.Count)];
							}
							while (l++ < 10 && !LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value && stg.IsBoss && stages[next].IsBoss);
							stg.Neutral = next;
						}
						if (stg.HasHero && stg.Hero == -1)
						{
							var pool = depthstages[Math.Min(stagedepths[stg.ID] + r.Next(-1, 2), depthstages.Count - 1)];
							stg.Hero = pool[r.Next(pool.Count)];
						}
						if (stg.HasDark && stg.Dark == -1)
						{
							var pool = depthstages[Math.Min(stagedepths[stg.ID] + r.Next(-1, 2), depthstages.Count - 1)];
							stg.Dark = pool[r.Next(pool.Count)];
						}
					}
				}
				break;
			case LevelOrderMode.BossRush:
				Shuffle(r, stageids, stagecount);
				for (int i = 0; i < stagecount; i++)
					stages[stageids[i]].Neutral = stageids[i + 1];
				break;
			case LevelOrderMode.Wild:
				{
					Queue<int> stgq = new Queue<int>();
					stgq.Enqueue(stageids[r.Next(stagecount)]);
					List<int> neword = new List<int>(stagecount);
					while (neword.Count < stagecount)
					{
						if (stgq.Count == 0)
						{
							foreach (var id in stageids.Except(neword))
								if (id != totalstagecount)
									stgq.Enqueue(id);
						}
						int i = stgq.Dequeue();
						neword.Add(i);
						Stage stg = stages[i];
						if (stg.IsBoss || stg.HasNeutral)
						{
							stg.Neutral = stageids[r.Next(stagecount + 1)];
							if (stg.Neutral != totalstagecount && !neword.Contains(stg.Neutral) && !stgq.Contains(stg.Neutral))
								stgq.Enqueue(stg.Neutral);
						}
						if (stg.HasHero)
						{
							stg.Hero = stageids[r.Next(stagecount + 1)];
							if (stg.Hero != totalstagecount && !neword.Contains(stg.Hero) && !stgq.Contains(stg.Hero))
								stgq.Enqueue(stg.Hero);
						}
						if (stg.HasDark)
						{
							stg.Dark = stageids[r.Next(stagecount + 1)];
							if (stg.Dark != totalstagecount && !neword.Contains(stg.Dark) && !stgq.Contains(stg.Dark))
								stgq.Enqueue(stg.Dark);
						}
					}
					neword.CopyTo(stageids);
				}
				break;
		}
		if (settings.LevelOrderMode != LevelOrderMode.Original)
		{
			for (int i = 0; i < totalstagecount; i++)
			{
				Stage stg = stages[i];
				if (stg.IsBoss && stg.Hero == -1 && stg.Dark == -1)
					stg.Dark = stg.Hero = stg.Neutral;
				if (stg.Dark != -1)
				{
					buf = BitConverter.GetBytes(stg.Dark == totalstagecount ? -2 : stg.Dark + stagefirst);
					Array.Reverse(buf);
					buf.CopyTo(dolfile, firstStageOffset + (i * stageOffset) + darkOffset);
				}
				if (stg.Neutral != -1)
				{
					buf = BitConverter.GetBytes(stg.Neutral == totalstagecount ? -2 : stg.Neutral + stagefirst);
					Array.Reverse(buf);
					buf.CopyTo(dolfile, firstStageOffset + (i * stageOffset) + neutOffset);
				}
				if (stg.Hero != -1)
				{
					buf = BitConverter.GetBytes(stg.Hero == totalstagecount ? -2 : stg.Hero + stagefirst);
					Array.Reverse(buf);
					buf.CopyTo(dolfile, firstStageOffset + (i * stageOffset) + heroOffset);
				}
			}

			buf = BitConverter.GetBytes(0x38000000 | stageAssociationIDMap[stageids[0] + stagefirst]);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, storyModeStartAddress);
		}

		ProgressBar_RandomizationProgress.Value = 15;

		// patch the route menu to allow stg06xx+ to display next stages
		buf = BitConverter.GetBytes(routeMenu6xxStagePreviewPatchValue);
		Array.Reverse(buf);
		buf.CopyTo(dolfile, routeMenu6xxStagePreviewBlockerOffset);
		// end patch

		if (Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value || Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value))
		{
			// special weapons box patch
			buf = BitConverter.GetBytes(shadowBoxPatchValue);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, shadowBoxPatchOffset);
			// end special weapons box patch
		}

		File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "main.dol"), dolfile);
		if (Music_CheckBox_RandomizeMusic.IsChecked.Value)
		{
			Dictionary<MusicCategory, List<string>> musicFiles = new Dictionary<MusicCategory, List<string>>()
				{
					{ MusicCategory.Stage, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_stg*.adx")) },
					{ MusicCategory.Jingle, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_jin*.adx")) },
					{ MusicCategory.Menu, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_sys*.adx")) },
					{ MusicCategory.Credits, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_vox*.adx")) }
				};
			if (Music_CheckBox_SkipRankTheme.IsChecked.Value)
				musicFiles[MusicCategory.Jingle].RemoveAll(a => a.EndsWith("sng_jin_roundclear.adx"));
			if (Music_CheckBox_SkipChaosPowerUseJingles.IsChecked.Value)
				musicFiles[MusicCategory.Jingle].RemoveAll(a => a.EndsWith("_e.adx"));
			var outfiles = musicFiles.ToDictionary(a => a.Key, b => b.Value.Select(c => Path.GetFileName(c)).ToArray());
			if (Directory.Exists("RandoMusic"))
				foreach (var file in Directory.EnumerateFiles("RandoMusic", "*.txt", SearchOption.AllDirectories))
					if (Enum.TryParse<MusicCategory>(Path.GetFileNameWithoutExtension(file), out var cat))
					{
						string dir = Path.GetDirectoryName(file);
						musicFiles[cat].AddRange(File.ReadAllLines(file).Select(a => Path.Combine(dir, a)));
					}
			foreach (var cat in outfiles.Keys)
			{
				var pool = musicFiles[cat].ToArray();
				Shuffle(r, pool);
				var files = outfiles[cat];
				for (int i = 0; i < files.Length; i++)
					File.Copy(pool[i % pool.Length], Path.Combine(settings.GamePath, "files", files[i]), true);
			}
		}

		ProgressBar_RandomizationProgress.Value = 25;

		if (Layout_CheckBox_RandomizeLayouts.IsChecked.Value) {
			var layoutResult = RandomizeLayouts(r);
			if (layoutResult == 1)
			{
				ProgressBar_RandomizationProgress.Value = 0;
				return;
			}
		}

		ProgressBar_RandomizationProgress.Value = 50;

		if (Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value)
			RandomizeSubtitles(r);

		ProgressBar_RandomizationProgress.Value = 75;

		if (Models_CheckBox_RandomizeModel.IsChecked.Value)
			RandomizeModels(r);

		Spoilers_ListBox_LevelList.Items.Clear();
		for (int i = 0; i < stagecount; i++)
			Spoilers_ListBox_LevelList.Items.Add(GetStageName(stageids[i]));
		Spoilers_ListBox_LevelList.IsEnabled = true;
		Spoilers_ListBox_LevelList.SelectedIndex = 0;
		Spoilers_Button_SaveLog.IsEnabled = true;
		Spoilers_Button_MakeChart.IsEnabled = true;
		ProgressBar_RandomizationProgress.Value = 100;
		settings.Save();
		var msgbox = MessageBoxManager.GetMessageBoxStandard("ShadowRando", "Randomization Complete", ButtonEnum.Ok, Icon.Info);
		var result = await msgbox.ShowAsync();
		if (result == ButtonResult.Ok)
		{
			ProgressBar_RandomizationProgress.Value = 0;
		}
	}

	private static void CopyDirectory(DirectoryInfo srcDir, string dstDir, bool overwrite = false)
	{
		Directory.CreateDirectory(dstDir);
		foreach (var dir in srcDir.EnumerateDirectories())
			CopyDirectory(dir, Path.Combine(dstDir, dir.Name), overwrite);
		foreach (var fil in srcDir.EnumerateFiles())
			fil.CopyTo(Path.Combine(dstDir, fil.Name), overwrite);
	}

	private static void CopyDirectory(string srcDir, string dstDir, bool overwrite = false) => CopyDirectory(new DirectoryInfo(srcDir), dstDir, overwrite);

	private void RandomizeSubtitles(Random r)
	{
		var fontAndAudioData = LoadFNTsAndAFS(true);
		var fntRandomPool = new List<ShadowFNT.Structures.TableEntry>();
		var uniqueAudioIDs = new Dictionary<int, bool>();
		var uniqueSubtitles = new Dictionary<string, bool>();
		if (Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value || Subtitles_CheckBox_NoDuplicates.IsChecked.Value || Subtitles_CheckBox_NoSystemMessages.IsChecked.Value || Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value)
		{
			for (int i = 0; i < fontAndAudioData.initialFntState.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.initialFntState[i].GetEntryTableCount(); j++)
				{
					var entry = fontAndAudioData.initialFntState[i].GetEntryTable()[j];
					if (Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value && entry.audioId == -1)
						continue;
					if (Subtitles_CheckBox_NoSystemMessages.IsChecked.Value && (entry.entryType == ShadowFNT.Structures.EntryType.MENU || entry.entryType == ShadowFNT.Structures.EntryType.FINAL_ENTRY || entry.messageIdBranchSequence == 9998100))
						continue;
					if (Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value && entry.audioId != -1 && !SubtitleCharacterPicked(fontAndAudioData.afs.Files[entry.audioId].Name))
						continue;

					if (Subtitles_CheckBox_NoDuplicates.IsChecked.Value)
					{
						try
						{
							if (entry.audioId != -1 && uniqueAudioIDs[entry.audioId])
								continue;
						}
						catch (KeyNotFoundException)
						{
							// not previously seen, we continue the flow
						}

						try
						{
							// this covers chained entries and any repeating messages with -1; Such as system dialogs if the user is not using that filter
							if (entry.audioId == -1 && uniqueSubtitles[entry.subtitle])
								continue;
						}
						catch (KeyNotFoundException)
						{
							// not previously seen, we reach the end and add to the list
						}
					}
					uniqueAudioIDs[entry.audioId] = true;
					uniqueSubtitles[entry.subtitle] = true;
					fntRandomPool.Add(entry);
				}
			}
			// customized fnt pool built; begin applying
			for (int i = 0; i < fontAndAudioData.mutatedFnt.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.mutatedFnt[i].GetEntryTableCount(); j++)
				{
					// Chained entries not accounted for, so may produce wacky results
					int donorFNTEntryIndex = r.Next(0, fntRandomPool.Count - 1);
					if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value && fntRandomPool[donorFNTEntryIndex].audioId == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fntRandomPool[donorFNTEntryIndex].audioId);
					}
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fntRandomPool[donorFNTEntryIndex].subtitle);
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fntRandomPool[donorFNTEntryIndex].subtitleActiveTime);
				}
			}
		}
		else
		{
			for (int i = 0; i < fontAndAudioData.mutatedFnt.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.mutatedFnt[i].GetEntryTableCount(); j++)
				{
					// Chained entries not accounted for, so may produce wacky results
					int donorFNTIndex = r.Next(0, fontAndAudioData.mutatedFnt.Count - 1);
					int donorFNTEntryIndex = r.Next(0, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryTableCount() - 1);
					if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value && fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donorFNTEntryIndex) == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donorFNTEntryIndex));
					}

					fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitle(donorFNTEntryIndex));
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitleActiveTime(donorFNTEntryIndex));
				}
			}
		}
		ExportChangedFNTs(fontAndAudioData.mutatedFnt, fontAndAudioData.initialFntState);
	}

	private (List<FNT> mutatedFnt, List<FNT> initialFntState, AfsArchive afs) LoadFNTsAndAFS(bool loadAFS, string localeOverride = "EN")
	{
		// Load all target FNTs
		var initialFntsOpenedState = new List<FNT>();
		var openedFnts = new List<FNT>();
		AfsArchive currentAfs = null;

		var fontDirectory = Path.Combine("backup", "fonts");
		string[] foundFnts = Directory.GetFiles(fontDirectory, "*_" + localeOverride + ".fnt", SearchOption.AllDirectories);
		for (int i = 0; i < foundFnts.Length; i++)
		{
			byte[] readFile = File.ReadAllBytes(foundFnts[i]);
			FNT newFnt = FNT.ParseFNTFile(foundFnts[i], ref readFile, fontDirectory);
			FNT mutatedFnt = FNT.ParseFNTFile(foundFnts[i], ref readFile, fontDirectory);

			openedFnts.Add(newFnt);
			initialFntsOpenedState.Add(mutatedFnt);
		}

		if (!loadAFS)
			return (initialFntsOpenedState, openedFnts, null);

		// Should probably find a different way to peak at total size, because each time mem footprint increases w/AFS size
		var data = File.ReadAllBytes(Path.Combine(settings.GamePath, "files", "PRS_VOICE_E.afs"));
		if (AfsArchive.TryFromFile(data, out var afsArchive))
		{
			currentAfs = afsArchive;
			data = null; // for GC purpose
		}
		return (mutatedFnt: initialFntsOpenedState, initialFntState: openedFnts, afs: currentAfs);
	}

	private void ExportChangedFNTs(List<FNT> mutatedFnt, List<FNT> initialFntState)
	{
		List<FNT> filesToWrite = new List<FNT>();
		for (int i = 0; i < initialFntState.Count; i++)
		{
			if (initialFntState[i].Equals(mutatedFnt[i]) == false)
			{
				filesToWrite.Add(mutatedFnt[i]);
			}
		}
		foreach (FNT fnt in filesToWrite)
		{
			fnt.RecomputeAllSubtitleAddresses();
			string outfn = Path.Combine(settings.GamePath, "files", fnt.fileName.Substring(fnt.fileName.IndexOf("fonts")));
			File.WriteAllBytes(outfn, fnt.ToBytes());
			string prec = outfn.Remove(outfn.Length - 4);
			File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Assets/EN.txd", prec + ".txd", true);
			File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Assets/EN00.met", prec + "00.met", true);
		}
	}

	private bool SubtitleCharacterPicked(string audioName)
	{
		if (Subtitles_CheckBox_SelectedCharacter_Shadow.IsChecked.Value && audioName.EndsWith("_sd.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Sonic.IsChecked.Value && audioName.EndsWith("_sn.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Tails.IsChecked.Value && audioName.EndsWith("_tl.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Knuckles.IsChecked.Value && audioName.EndsWith("_kn.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Amy.IsChecked.Value && audioName.EndsWith("_am.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Rouge.IsChecked.Value && audioName.EndsWith("_rg.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Omega.IsChecked.Value && audioName.EndsWith("_om.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Vector.IsChecked.Value && audioName.EndsWith("_vc.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Espio.IsChecked.Value && audioName.EndsWith("_es.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Maria.IsChecked.Value && (audioName.EndsWith("_mr.adx") || audioName.EndsWith("_mr2.adx")))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Charmy.IsChecked.Value && audioName.EndsWith("_ch.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Eggman.IsChecked.Value && audioName.EndsWith("_eg.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_BlackDoom.IsChecked.Value && audioName.EndsWith("_bd.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Cream.IsChecked.Value && audioName.EndsWith("_cr.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_Cheese.IsChecked.Value && audioName.EndsWith("_co.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_GUNCommander.IsChecked.Value && audioName.EndsWith("_cm.adx"))
			return true;
		if (Subtitles_CheckBox_SelectedCharacter_GUNSoldier.IsChecked.Value && audioName.EndsWith("_sl.adx"))
			return true;
		return false;
	}

	private int RandomizeLayouts(Random r)
	{
		var enemyMode = (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex;
		var nukkoro2 = Nukkoro2.ReadFile(Path.Combine("backup", "nukkoro2.inf"));

		ShadowSET.LayoutEditorSystem.SetupLayoutEditorSystem(); // Critical to load relevent data
		for (int stageIdToModify = 5; stageIdToModify < 45; stageIdToModify++)
		{
			stageAssociationIDMap.TryGetValue(stageIdToModify, out var stageId);
			var stageDataIdentifier = "stg0" + stageId;
			var cmnLayout = stageDataIdentifier + "_cmn.dat";
			var cmnLayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, cmnLayout), out var resultcmn);
			var nrmLayout = stageDataIdentifier + "_nrm.dat";
			List<SetObjectShadow> nrmLayoutData = null;
			var hrdLayout = stageDataIdentifier + "_hrd.dat";
			List<SetObjectShadow> hrdLayoutData = null;
			var ds1Layout = stageDataIdentifier + "_ds1.dat";
			List<SetObjectShadow> ds1LayoutData = null;

			try
			{
				nrmLayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, nrmLayout), out var resultnrm);
			}
			catch (FileNotFoundException)
			{
				// some stages don't have nrm
			}

			try
			{
				hrdLayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, hrdLayout), out var resulthrd);
			}
			catch (FileNotFoundException)
			{
				// some stages don't have hrd
			}

			try
			{
				ds1LayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, ds1Layout), out var resultds1);
			}
			catch (FileNotFoundException)
			{
				// some stages don't have ds1
			}

			List<EWeapon> weaponsPool = [];

			if (Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value)
			{
				if (Layout_Weapon_CheckBox_SelectedWeapon_None.IsChecked.Value)
					weaponsPool.Add(EWeapon.None);
				if (Layout_Weapon_CheckBox_SelectedWeapon_Pistol.IsChecked.Value)
					weaponsPool.Add(EWeapon.Pistol);
				if (Layout_Weapon_CheckBox_SelectedWeapon_SubmachineGun.IsChecked.Value)
					weaponsPool.Add(EWeapon.SubmachineGun);
				if (Layout_Weapon_CheckBox_SelectedWeapon_AssaultRifle.IsChecked.Value)
					weaponsPool.Add(EWeapon.MachineGun);
				if (Layout_Weapon_CheckBox_SelectedWeapon_HeavyMachineGun.IsChecked.Value)
					weaponsPool.Add(EWeapon.HeavyMachineGun);
				if (Layout_Weapon_CheckBox_SelectedWeapon_GatlingGun.IsChecked.Value)
					weaponsPool.Add(EWeapon.GatlingGun);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EggPistol.IsChecked.Value)
					weaponsPool.Add(EWeapon.EggGun);
				if (Layout_Weapon_CheckBox_SelectedWeapon_LightShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.LightShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_FlashShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.FlashShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_RingShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.RingShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_HeavyShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.HeavyShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_GrenadeLauncher.IsChecked.Value)
					weaponsPool.Add(EWeapon.GrenadeLauncher);
				if (Layout_Weapon_CheckBox_SelectedWeapon_GUNBazooka.IsChecked.Value)
					weaponsPool.Add(EWeapon.GUNBazooka);
				if (Layout_Weapon_CheckBox_SelectedWeapon_TankCannon.IsChecked.Value)
					weaponsPool.Add(EWeapon.TankCannon);
				if (Layout_Weapon_CheckBox_SelectedWeapon_BlackBarrel.IsChecked.Value)
					weaponsPool.Add(EWeapon.BlackBarrel);
				if (Layout_Weapon_CheckBox_SelectedWeapon_BigBarrel.IsChecked.Value)
					weaponsPool.Add(EWeapon.BigBarrel);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EggBazooka.IsChecked.Value)
					weaponsPool.Add(EWeapon.EggBazooka);
				if (Layout_Weapon_CheckBox_SelectedWeapon_RPG.IsChecked.Value)
					weaponsPool.Add(EWeapon.RPG);
				if (Layout_Weapon_CheckBox_SelectedWeapon_FourShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.FourShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EightShot.IsChecked.Value)
					weaponsPool.Add(EWeapon.EightShot);
				if (Layout_Weapon_CheckBox_SelectedWeapon_WormShooterBlack.IsChecked.Value)
					weaponsPool.Add(EWeapon.WormShooterBlack);
				if (Layout_Weapon_CheckBox_SelectedWeapon_WormShooterRed.IsChecked.Value)
					weaponsPool.Add(EWeapon.WideWormShooterRed);
				if (Layout_Weapon_CheckBox_SelectedWeapon_WormShooterGold.IsChecked.Value)
					weaponsPool.Add(EWeapon.BigWormShooterGold);
				if (Layout_Weapon_CheckBox_SelectedWeapon_VacuumPod.IsChecked.Value)
					weaponsPool.Add(EWeapon.VacuumPod);
				if (Layout_Weapon_CheckBox_SelectedWeapon_LaserRifle.IsChecked.Value)
					weaponsPool.Add(EWeapon.LaserRifle);
				if (Layout_Weapon_CheckBox_SelectedWeapon_Splitter.IsChecked.Value)
					weaponsPool.Add(EWeapon.Splitter);
				if (Layout_Weapon_CheckBox_SelectedWeapon_Refractor.IsChecked.Value)
					weaponsPool.Add(EWeapon.Refractor);
				if (Layout_Weapon_CheckBox_SelectedWeapon_Knife.IsChecked.Value)
					weaponsPool.Add(EWeapon.Knife);
				if (Layout_Weapon_CheckBox_SelectedWeapon_BlackSword.IsChecked.Value)
					weaponsPool.Add(EWeapon.BlackSword);
				if (Layout_Weapon_CheckBox_SelectedWeapon_DarkHammer.IsChecked.Value)
					weaponsPool.Add(EWeapon.DarkHammer);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EggLance.IsChecked.Value)
					weaponsPool.Add(EWeapon.EggLance);
				if (Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv1.IsChecked.Value)
					weaponsPool.Add(EWeapon.SamuraiSwordLv1);
				if (Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv2.IsChecked.Value)
					weaponsPool.Add(EWeapon.SamuraiSwordLv2);
				if (Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv1.IsChecked.Value)
					weaponsPool.Add(EWeapon.SatelliteLaserLv1);
				if (Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv2.IsChecked.Value)
					weaponsPool.Add(EWeapon.SatelliteLaserLv2);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv1.IsChecked.Value)
					weaponsPool.Add(EWeapon.EggVacLv1);
				if (Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv2.IsChecked.Value)
					weaponsPool.Add(EWeapon.EggVacLv2);
				if (Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv1.IsChecked.Value)
					weaponsPool.Add(EWeapon.OmochaoLv1);
				if (Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv2.IsChecked.Value)
					weaponsPool.Add(EWeapon.OmochaoLv2);
				if (Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv1.IsChecked.Value)
					weaponsPool.Add(EWeapon.HealCannonLv1);
				if (Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv2.IsChecked.Value)
					weaponsPool.Add(EWeapon.HealCannonLv2);
				if (Layout_Weapon_CheckBox_SelectedWeapon_ShadowRifle.IsChecked.Value)
					weaponsPool.Add(EWeapon.ShadowRifle);
				if (weaponsPool.Count == 0)
				{
					ShowSimpleMessage("Error", "Must select at least one weapon.", ButtonEnum.Ok, Icon.Error);
					return 1;
				}
			}
			else
			{
				weaponsPool.AddRange(Weapons);
			}

			if (Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value)
			{
				MakeAllBoxesHaveRandomWeapons(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					MakeAllBoxesHaveRandomWeapons(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					MakeAllBoxesHaveRandomWeapons(ref hrdLayoutData, weaponsPool, r);
			} else if (Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value)
			{
				MakeAllWeaponBoxesHaveRandomWeapons(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					MakeAllWeaponBoxesHaveRandomWeapons(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					MakeAllWeaponBoxesHaveRandomWeapons(ref hrdLayoutData, weaponsPool, r);
			}

			if (Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked.Value)
			{
				RandomizeWeaponsOnGround(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					RandomizeWeaponsOnGround(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					RandomizeWeaponsOnGround(ref hrdLayoutData, weaponsPool, r);
			}

			if (Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked.Value)
			{
				RandomizeEnvironmentWeaponDrops(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					RandomizeEnvironmentWeaponDrops(ref nrmLayoutData, weaponsPool, r);
				if (ds1LayoutData != null)
					RandomizeEnvironmentWeaponDrops(ref ds1LayoutData, weaponsPool, r);
			}

			if ((LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild)
			{
				MakeAllPartnersRandom(ref cmnLayoutData, Layout_Partner_CheckBox_KeepAffiliationOfOriginalObject.IsChecked.Value, r);
				if (nrmLayoutData != null)
					MakeAllPartnersRandom(ref nrmLayoutData, Layout_Partner_CheckBox_KeepAffiliationOfOriginalObject.IsChecked.Value, r);
			}

			List<Type> allEnemies = new List<Type>();
			List<Type> groundEnemies = new List<Type>();
			List<Type> flyingEnemies = new List<Type>();
			List<Type> pathTypeFlyingEnemies = new List<Type>();

			if (Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value && enemyMode != LayoutEnemyMode.Original)
			{
				if (Layout_Enemy_CheckBox_SelectedEnemy_GUNSoldier.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0064_GUNSoldier));
					allEnemies.Add(typeof(Object0064_GUNSoldier));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_GUNBeetle.IsChecked.Value)
				{
					flyingEnemies.Add(typeof(Object0065_GUNBeetle));
					pathTypeFlyingEnemies.Add(typeof(Object0065_GUNBeetle));
					allEnemies.Add(typeof(Object0065_GUNBeetle));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_GUNBigfoot.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0066_GUNBigfoot));
					flyingEnemies.Add(typeof(Object0066_GUNBigfoot));
					allEnemies.Add(typeof(Object0066_GUNBigfoot));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_GUNRobot.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0068_GUNRobot));
					allEnemies.Add(typeof(Object0068_GUNRobot));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_EggPierrot.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0078_EggPierrot));
					allEnemies.Add(typeof(Object0078_EggPierrot));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_EggPawn.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0079_EggPawn));
					allEnemies.Add(typeof(Object0079_EggPawn));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_ShadowAndroid.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object007A_EggShadowAndroid));
					allEnemies.Add(typeof(Object007A_EggShadowAndroid));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BAGiant.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object008C_BkGiant));
					allEnemies.Add(typeof(Object008C_BkGiant));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BASoldier.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object008D_BkSoldier));
					allEnemies.Add(typeof(Object008D_BkSoldier));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BAHawkVolt.IsChecked.Value)
				{
					flyingEnemies.Add(typeof(Object008E_BkWingLarge));
					pathTypeFlyingEnemies.Add(typeof(Object008E_BkWingLarge));
					allEnemies.Add(typeof(Object008E_BkWingLarge));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BAWing.IsChecked.Value)
				{
					flyingEnemies.Add(typeof(Object008F_BkWingSmall));
					pathTypeFlyingEnemies.Add(typeof(Object008F_BkWingSmall));
					allEnemies.Add(typeof(Object008F_BkWingSmall));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BAWorm.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0090_BkWorm));
					allEnemies.Add(typeof(Object0090_BkWorm));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BALarva.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0091_BkLarva));
					allEnemies.Add(typeof(Object0091_BkLarva));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_ArtificialChaos.IsChecked.Value)
				{
					flyingEnemies.Add(typeof(Object0092_BkChaos));
					allEnemies.Add(typeof(Object0092_BkChaos));
				}
				if (Layout_Enemy_CheckBox_SelectedEnemy_BAAssassin.IsChecked.Value)
				{
					groundEnemies.Add(typeof(Object0093_BkNinja));
					flyingEnemies.Add(typeof(Object0093_BkNinja));
					allEnemies.Add(typeof(Object0093_BkNinja));
				}
				// error checking
				if (Layout_Enemy_CheckBox_KeepType.IsChecked.Value)
				{
					if (groundEnemies.Count == 0 || flyingEnemies.Count == 0)
					{
						ShowSimpleMessage("Error", "Must have at least one ground and one flying enemy.", ButtonEnum.Ok, Icon.Error);
						return 1; // TODO do we want to throw errors?
					}
					if (groundEnemies.Count == 1)
					{
						// make sure there is at least one other enemy if GUN Soldiers are only picked
						if (groundEnemies[0] == typeof(Object0064_GUNSoldier))
						{
							ShowSimpleMessage("Error", "GUN Soldiers have an issue with some Link IDs, add an extra ground enemy type.", ButtonEnum.Ok, Icon.Error);
							return 1;
						}
					}
				} else
				{
					if (allEnemies.Count == 0)
					{
						ShowSimpleMessage("Error", "Must pick at least one enemy type.", ButtonEnum.Ok, Icon.Error);
						return 1;
					}
					else if (allEnemies.Count == 1)
					{
						// make sure there is at least one other enemy if GUN Soldiers are only picked
						if (allEnemies[0] == typeof(Object0064_GUNSoldier))
						{
							ShowSimpleMessage("Error", "GUN Soldiers have an issue with some Link IDs, add an extra enemy type.", ButtonEnum.Ok, Icon.Error);
							return 1;
						}
					}
				}
			}
			else
			{
				groundEnemies.AddRange(GroundEnemyTypes);
				flyingEnemies.AddRange(FlyingEnemyTypes);
				pathTypeFlyingEnemies.AddRange([typeof(Object0065_GUNBeetle), typeof(Object008E_BkWingLarge), typeof(Object008F_BkWingSmall)]);
				allEnemies.AddRange(EnemyTypes);
			}

			switch (enemyMode)
			{
				case LayoutEnemyMode.Original:
					break;
				case LayoutEnemyMode.Wild:
					WildRandomizeAllEnemiesWithTranslations(ref cmnLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
					DelinkVehicleObjects(ref cmnLayoutData);
					if (nrmLayoutData != null)
					{
						WildRandomizeAllEnemiesWithTranslations(ref nrmLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
						DelinkVehicleObjects(ref nrmLayoutData);
					}
					if (hrdLayoutData != null)
					{
						WildRandomizeAllEnemiesWithTranslations(ref hrdLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
						DelinkVehicleObjects(ref hrdLayoutData);
					}
					break;
				default:
					break;
			}

			LayoutEditorFunctions.SaveShadowLayout(cmnLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, cmnLayout), false);
			if (nrmLayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(nrmLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, nrmLayout), false);
			if (hrdLayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(hrdLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, hrdLayout), false);
			if (ds1LayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(ds1LayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, ds1Layout), false);

			if (Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value && Nukkoro2EnemyCountStages.TryGetValue(stageId, out var nukkoro2StageString))
			{
				nukkoro2.TryGetValue(nukkoro2StageString.Item1, out var nukkoro2Stage);
				switch (nukkoro2StageString.Item2)
				{
					case 0:
						var total = GetTotalGUNEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountDark.Success = total - (int)(total * (Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value / 100));
						break;
					case 1:
						total = GetTotalBlackArmsEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountHero.Success = total - (int)(total * (Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value / 100));
						break;
					case 2:
						total = GetTotalGUNEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountDark.Success = total - (int)(total * (Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value / 100));
						total = GetTotalBlackArmsEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountHero.Success = total - (int)(total * (Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value / 100));
						break;
					default:
						break;
				}
			}

			if (Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked.Value)
			{
				if (stageDataIdentifier == "stg0400" || stageDataIdentifier == "stg0700" || stageIdToModify >= 28)
					continue;
				var datOneFile = stageDataIdentifier + "_dat.one";
				var datOneData = File.ReadAllBytes(Path.Combine("backup", "sets", stageDataIdentifier, datOneFile));
				ONEArchiveType archiveType = ONEArchiveTester.GetArchiveType(ref datOneData);
				var datOneDataContent = Archive.FromONEFile(ref datOneData);
				if (datOneDataContent.Files[0].Name == "PATH.PTP")
				{
					var splines = SplineReader.ReadShadowSplineFile(datOneDataContent.Files[0]);
					foreach (var spline in splines)
					{
						if (spline.SplineType == 32 && spline.Name.Contains("_cc_") && !spline.Name.Contains("stg0300_cc_dr_jn_208") && !spline.Name.Contains("stg0300_cc_pr_jn_210"))
							spline.Setting2 = 1;
					}
					var updatedPATHPTP = SplineReader.ShadowSplinesToByteArray(stageDataIdentifier, splines);
					datOneDataContent.Files[0].CompressedData = FraGag.Compression.Prs.Compress(updatedPATHPTP);
					var updatedDatOneData = datOneDataContent.BuildShadowONEArchive(archiveType == ONEArchiveType.Shadow060);
					File.WriteAllBytes(Path.Combine(settings.GamePath, "files", stageDataIdentifier, datOneFile), updatedDatOneData.ToArray());
				}
			}
		} // end - layout operations

		// setIdBin operations
		var setIdBINPath = Path.Combine("backup", "setid.bin");
		var setIdTable = ShadowSET.SETIDBIN.SetIdTableFunctions.LoadTable(setIdBINPath, true, LayoutEditorSystem.shadowObjectEntries);

		// 00 - 0x0C
		// 00, 0x64 = gun soldier | 0x93 = BkNinja (last enemy type)
		foreach (ShadowSET.SETIDBIN.TableEntry entry in setIdTable)
		{
			if (entry.objectEntry.List == 0x00 &&
				(
					entry.objectEntry.Type <= 0x0C || // is a box/spring
					entry.objectEntry.Type == 0x3A || // special weapons box
					entry.objectEntry.Type is >= 0x64 and <= 0x93 ||  // is an enemy
					entry.objectEntry.Type is >= 0xC8 and <= 0xFD || // is a weapon
					entry.objectEntry.Type is >= 0x46 and <= 0x4E // is a vehicle
				)
			)
			{
				foreach (ShadowSET.SETIDBIN.StageEntry stage in LayoutEditorSystem.shadowStageEntries)
				{
					entry.values0 |= stage.flag0;
					entry.values1 |= stage.flag1;
					entry.values2 |= stage.flag2;
				}
			}
		}

		ShadowSET.SETIDBIN.SetIdTableFunctions.SaveTable(Path.Combine(settings.GamePath, "files", "setid.bin"), true, setIdTable);

		// patch bi2.bin since we require 64MB Dolphin
		var buf = BitConverter.GetBytes(0);
		var bi2 = File.ReadAllBytes(Path.Combine("backup", "bi2.bin"));
		buf.CopyTo(bi2, 0x4);
		File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "bi2.bin"), bi2);
		// end patch

		if (Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value)
		{
			Nukkoro2.WriteFile(Path.Combine(settings.GamePath, "files", "nukkoro2.inf"), nukkoro2);
		}

		return 0;
	}

	private static async void ShowSimpleMessage(string title, string message, ButtonEnum messageType, Icon messageIcon) {
		var error = MessageBoxManager.GetMessageBoxStandard(title, message, messageType, messageIcon);
		await error.ShowAsync();
	}

	private static int GetTotalGUNEnemies(IReadOnlyList<SetObjectShadow> cmn, IReadOnlyList<SetObjectShadow>? nrm = null)
	{
		int total = cmn.Count(setObject => setObject is { List: 0x00, Type: >= 0x64 and <= 0x68 });
		if (nrm == null) return total;
		
		total += nrm.Count(setObject => setObject is { List: 0x00, Type: >= 0x64 and <= 0x68 });
		return total;
	}

	private static int GetTotalBlackArmsEnemies(IReadOnlyList<SetObjectShadow> cmn, IReadOnlyList<SetObjectShadow>? nrm = null)
	{
		int total = 0;

		foreach (var setObject in cmn)
		{
			if (setObject is { List: 0x00, Type: >= 0x8C and <= 0x93 })
			{
				if (setObject.Type == 0x91) // BkLarva
				{
					total += ((Object0091_BkLarva)setObject).NumberOfLarva;
				}
				else
				{
					total++;
				}
			}
		}

		if (nrm == null) return total;
		foreach (var setObject in nrm)
		{
			if (setObject is { List: 0x00, Type: >= 0x8C and <= 0x93 })
			{
				if (setObject.Type == 0x91) // BkLarva
				{
					total += ((Object0091_BkLarva)setObject).NumberOfLarva;
				}
				else
				{
					total++;
				}
			}
		}
		return total;
	}

	private static void MakeAllPartnersRandom(ref List<SetObjectShadow> setData, bool keepOriginalObjectAffiliation, Random r)
	{
		List<(Object0190_Partner item, int index)> partnerItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object0190_Partner)
			.Select(pair => (Item: (Object0190_Partner)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var partner in partnerItems)
		{
			if (keepOriginalObjectAffiliation)
			{
				if (partner.item.Partner == Object0190_Partner.EPartner.Eggman || partner.item.Partner == Object0190_Partner.EPartner.DoomsEye) 
					partner.item.Partner = (Object0190_Partner.EPartner)r.Next(0x0B, 0x0D);
				else
					partner.item.Partner = (Object0190_Partner.EPartner)r.Next(0x01, 0x0B);
			}
			else
			{
				partner.item.Partner = (Object0190_Partner.EPartner)r.Next(0x01, 0x0D);
			}
			setData[partner.index] = partner.item;
		}
	}

	private static void MakeAllWeaponBoxesHaveRandomWeapons(ref List<SetObjectShadow> setData, IReadOnlyList<EWeapon> weaponsPool, Random r)
	{
		List<(Object000C_WeaponBox item, int index)> weaponBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object000C_WeaponBox)
			.Select(pair => (Item: (Object000C_WeaponBox)pair.Item, Index: pair.Index))
			.ToList();

		List<(Object003A_SpecialWeaponBox item, int index)> specialWeaponsBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object003A_SpecialWeaponBox)
			.Select(pair => (Item: (Object003A_SpecialWeaponBox)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var weaponBox in weaponBoxItems)
		{
			weaponBox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = WeaponContainers.GetWeaponAffiliationBoxType(weaponBox.item.Weapon);
			if (boxType.HasValue)
			{
				weaponBox.item.BoxType = boxType.Value;
				setData[weaponBox.index] = weaponBox.item;
			}
			else
			{
				switch (weaponBox.item.Weapon)
				{
					case EWeapon.SamuraiSwordLv1:
					case EWeapon.SamuraiSwordLv2:
					case EWeapon.SatelliteLaserLv1:
					case EWeapon.SatelliteLaserLv2:
					case EWeapon.EggVacLv1:
					case EWeapon.EggVacLv2:
					case EWeapon.OmochaoLv1:
					case EWeapon.OmochaoLv2:
					case EWeapon.HealCannonLv1:
					case EWeapon.HealCannonLv2:
					case EWeapon.ShadowRifle:
						WeaponContainers.ToSpecialWeaponBox(weaponBox.index, ref setData);
						break;
					default:
						setData[weaponBox.index] = weaponBox.item;
						break;
				}
			}
		}

		foreach (var specialWeaponsBox in specialWeaponsBoxItems)
		{
			specialWeaponsBox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			setData[specialWeaponsBox.index] = specialWeaponsBox.item;

			switch (specialWeaponsBox.item.Weapon)
			{
				case EWeapon.SamuraiSwordLv1:
				case EWeapon.SamuraiSwordLv2:
				case EWeapon.SatelliteLaserLv1:
				case EWeapon.SatelliteLaserLv2:
				case EWeapon.EggVacLv1:
				case EWeapon.EggVacLv2:
				case EWeapon.OmochaoLv1:
				case EWeapon.OmochaoLv2:
				case EWeapon.HealCannonLv1:
				case EWeapon.HealCannonLv2:
				case EWeapon.ShadowRifle:
					setData[specialWeaponsBox.index] = specialWeaponsBox.item;
					break;
				default:
					WeaponContainers.ToWeaponBox(specialWeaponsBox.index, ref setData);
					break;
			}
		}
	}

	private static void MakeAllBoxesHaveRandomWeapons(ref List<SetObjectShadow> setData, IReadOnlyList<EWeapon> weaponsPool, Random r)
	{
		List<(Object0009_WoodBox item, int index)> woodBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object0009_WoodBox)
			.Select(pair => (Item: (Object0009_WoodBox)pair.Item, Index: pair.Index))
			.ToList();

		List<(Object000C_WeaponBox item, int index)> weaponBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object000C_WeaponBox)
			.Select(pair => (Item: (Object000C_WeaponBox)pair.Item, Index: pair.Index))
			.ToList();

		List<(Object000A_MetalBox item, int index)> metalBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object000A_MetalBox)
			.Select(pair => (Item: (Object000A_MetalBox)pair.Item, Index: pair.Index))
			.ToList();

		List<(Object003A_SpecialWeaponBox item, int index)> specialWeaponsBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object003A_SpecialWeaponBox)
			.Select(pair => (Item: (Object003A_SpecialWeaponBox)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var woodBox in woodBoxItems)
		{
			woodBox.item.BoxItem = EBoxItem.Weapon;
			woodBox.item.ModifierWeapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = WeaponContainers.GetWeaponAffiliationBoxType(woodBox.item.ModifierWeapon);
			if (boxType.HasValue)
			{
				woodBox.item.BoxType = boxType.Value;
				setData[woodBox.index] = woodBox.item;
			}
			else
			{
				switch (woodBox.item.ModifierWeapon)
				{
					case EWeapon.SamuraiSwordLv1:
					case EWeapon.SamuraiSwordLv2:
					case EWeapon.SatelliteLaserLv1:
					case EWeapon.SatelliteLaserLv2:
					case EWeapon.EggVacLv1:
					case EWeapon.EggVacLv2:
					case EWeapon.OmochaoLv1:
					case EWeapon.OmochaoLv2:
					case EWeapon.HealCannonLv1:
					case EWeapon.HealCannonLv2:
					case EWeapon.ShadowRifle:
						WeaponContainers.ToSpecialWeaponBox(woodBox.index, ref setData);
						break;
					default:
						setData[woodBox.index] = woodBox.item;
						break;
				}
			}
		}

		foreach (var weaponBox in weaponBoxItems)
		{
			weaponBox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = WeaponContainers.GetWeaponAffiliationBoxType(weaponBox.item.Weapon);
			if (boxType.HasValue)
			{
				weaponBox.item.BoxType = boxType.Value;
				setData[weaponBox.index] = weaponBox.item;
			}
			else
			{
				switch (weaponBox.item.Weapon)
				{
					case EWeapon.SamuraiSwordLv1:
					case EWeapon.SamuraiSwordLv2:
					case EWeapon.SatelliteLaserLv1:
					case EWeapon.SatelliteLaserLv2:
					case EWeapon.EggVacLv1:
					case EWeapon.EggVacLv2:
					case EWeapon.OmochaoLv1:
					case EWeapon.OmochaoLv2:
					case EWeapon.HealCannonLv1:
					case EWeapon.HealCannonLv2:
					case EWeapon.ShadowRifle:
						WeaponContainers.ToSpecialWeaponBox(weaponBox.index, ref setData);
						break;
					default:
						setData[weaponBox.index] = weaponBox.item;
						break;
				}
			}
		}

		foreach (var metalBox in metalBoxItems)
		{
			metalBox.item.BoxItem = EBoxItem.Weapon;
			metalBox.item.ModifierWeapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = WeaponContainers.GetWeaponAffiliationBoxType(metalBox.item.ModifierWeapon);
			if (boxType.HasValue)
			{
				metalBox.item.BoxType = boxType.Value;
				setData[metalBox.index] = metalBox.item;
			}
			else
			{
				switch (metalBox.item.ModifierWeapon)
				{
					case EWeapon.SamuraiSwordLv1:
					case EWeapon.SamuraiSwordLv2:
					case EWeapon.SatelliteLaserLv1:
					case EWeapon.SatelliteLaserLv2:
					case EWeapon.EggVacLv1:
					case EWeapon.EggVacLv2:
					case EWeapon.OmochaoLv1:
					case EWeapon.OmochaoLv2:
					case EWeapon.HealCannonLv1:
					case EWeapon.HealCannonLv2:
					case EWeapon.ShadowRifle:
						WeaponContainers.ToSpecialWeaponBox(metalBox.index, ref setData);
						break;
					default:
						setData[metalBox.index] = metalBox.item;
						break;
				}
			}
		}

		foreach (var specialWeaponsBox in specialWeaponsBoxItems)
		{
			specialWeaponsBox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			setData[specialWeaponsBox.index] = specialWeaponsBox.item;

			switch (specialWeaponsBox.item.Weapon)
			{
				case EWeapon.SamuraiSwordLv1:
				case EWeapon.SamuraiSwordLv2:
				case EWeapon.SatelliteLaserLv1:
				case EWeapon.SatelliteLaserLv2:
				case EWeapon.EggVacLv1:
				case EWeapon.EggVacLv2:
				case EWeapon.OmochaoLv1:
				case EWeapon.OmochaoLv2:
				case EWeapon.HealCannonLv1:
				case EWeapon.HealCannonLv2:
				case EWeapon.ShadowRifle:
					setData[specialWeaponsBox.index] = specialWeaponsBox.item;
					break;
				default:
					WeaponContainers.ToWeaponBox(specialWeaponsBox.index, ref setData);
					break;
			}
		}
	}

	private static void RandomizeWeaponsOnGround(ref List<SetObjectShadow> setData, IReadOnlyList<EWeapon> weaponsPool, Random r)
	{
		List<(Object0020_Weapon item, int index)> weaponsOnGroundItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object0020_Weapon)
			.Select(pair => (Item: (Object0020_Weapon)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var weaponOnGround in weaponsOnGroundItems)
		{
			weaponOnGround.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			setData[weaponOnGround.index] = weaponOnGround.item;
		}
	}

	private static void RandomizeEnvironmentWeaponDrops(ref List<SetObjectShadow> setData, List<EWeapon> weaponsPool, Random r)
	{
		List<(Object012C_EnvironmentalWeapon item, int index)> environmentWeaponItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object012C_EnvironmentalWeapon)
			.Select(pair => (Item: (Object012C_EnvironmentalWeapon)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var environmentWeapon in environmentWeaponItems)
		{
			if (environmentWeapon.item.DropType == Object012C_EnvironmentalWeapon.EDropType.LanternTorch_CrypticCastle)
				continue; // skip Cryptic Castle Torch to prevent breaking mission
			environmentWeapon.item.DropType = (Object012C_EnvironmentalWeapon.EDropType)weaponsPool[r.Next(weaponsPool.Count)];
			setData[environmentWeapon.index] = environmentWeapon.item;
		}
	}

	private static void DelinkVehicleObjects(ref List<SetObjectShadow> setData)
	{
		List<(Object004F_Vehicle item, int index)> vehicleItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object004F_Vehicle)
			.Select(pair => (Item: (Object004F_Vehicle)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var vehicle in vehicleItems)
		{
			vehicle.item.Link = 0;
			setData[vehicle.index] = vehicle.item;
		}
	}

	private void WildRandomizeAllEnemiesWithTranslations(ref List<SetObjectShadow> setData, IReadOnlyList<Type> allEnemies, IReadOnlyList<Type> groundEnemies, IReadOnlyList<Type> flyingEnemies, IReadOnlyList<Type> pathTypeFlyingEnemies, Random r)
	{
		// Wild Randomize of all Enemies
		for (int i = 0; i < setData.Count; i++)
		{
			if (setData[i].List != 0x00 || setData[i].Type < 0x64 || setData[i].Type > 0x93) continue;
			Type randomEnemyType;
			if (Layout_Enemy_CheckBox_KeepType.IsChecked.Value)
			{
				if (EnemyHelpers.IsFlyingEnemy(setData[i]))
				{
					if (setData[i].List == 0x00 && setData[i].Type == 0x90)
					{
						// if BkWorm, mutate original posY +50
						setData[i].PosY = setData[i].PosY + 50;
					}
					// if path type enemy
					if (EnemyHelpers.IsPathTypeFlyingEnemy(setData[i]))
					{
						if (pathTypeFlyingEnemies.Count == 0)
						{
							// delete the object? for now just skip it
							continue;
						}
						randomEnemyType = pathTypeFlyingEnemies[r.Next(pathTypeFlyingEnemies.Count)];
					}
					else
					{
						randomEnemyType = flyingEnemies[r.Next(flyingEnemies.Count)];
						if (randomEnemyType == typeof(Object0066_GUNBigfoot)) // special case for BkNinja and Bigfoot, since we need to force a specific 
						{
							var donor = new Object0066_GUNBigfoot
							{
								List = 0x00,
								Type = 0x66,
								MoveRange = 200, // EnemyBase
								SearchRange = 200,
								SearchAngle = 0,
								SearchWidth = 600,
								SearchHeight = 400,
								SearchHeightOffset = 0,
								MoveSpeedRatio = 1, // end EnemyBase
								AppearType = Object0066_GUNBigfoot.EAppear.ZUTTO_HOVERING,
								WeaponType = (Object0066_GUNBigfoot.EWeapon)r.Next(2),
								OffsetPos_Y = 50
							};
							SETMutations.MutateObjectAtIndex(i, donor, ref setData, true, r);
							continue; // skip the MutateObject below since we handled it ourselves
						}

						if (randomEnemyType == typeof(Object0093_BkNinja))
						{
							var donor = new Object0093_BkNinja
							{
								List = 0x00,
								Type = 0x93,
								MoveRange = 300,
								SearchRange = 0,
								SearchAngle = 0,
								SearchWidth = 500,
								SearchHeight = 300,
								SearchHeightOffset = 0,
								MoveSpeedRatio = 1,
								AppearType = Object0093_BkNinja.EAppear.ON_AIR_SAUCER_WARP,
								ShootCount = r.Next(1, 5),
								AttackInterval = 1,
								WaitInterval = 1,
								Pos0_X = 0,
								Pos0_Y = 0,
								Pos0_Z = 0,
								UNUSED_Pos0_IntWaitType = 0,
								UNUSED_Pos0_DisappearTime = 0,
								UNUSED_Pos1_X = 0,
								UNUSED_Pos1_Y = 0,
								UNUSED_Pos1_Z = 0,
								UNUSED_Pos1_WaitType = 0,
								UNUSED_Pos1_DisappearTime = 0,
								UNUSED_Float21 = 0,
								UNUSED_Float22 = 0
							};
							SETMutations.MutateObjectAtIndex(i, donor, ref setData, true, r);
							continue; // skip the MutateObject below since we handled it ourselves
						}
					}
				}
				else
				{ // ground enemies
					int randomEnemy;
					if (setData[i].Link == 0 || setData[i].Link == 50)
					{
						randomEnemy = r.Next(groundEnemies.Count); // All enemies if LinkID = 0 or 50
					}
					else
					{
						if (groundEnemies[0] == typeof(Object0064_GUNSoldier))
							randomEnemy = r.Next(1, groundEnemies.Count); // skip GUN Soldiers otherwise
						else
							randomEnemy = r.Next(groundEnemies.Count);
					}
					randomEnemyType = groundEnemies[randomEnemy];
				}
			}
			else
			{
				int randomEnemy;
				if (setData[i].Link == 0 || setData[i].Link == 50)
				{
					randomEnemy = r.Next(allEnemies.Count); // All enemies if LinkID = 0 or 50
				}
				else
				{
					if (groundEnemies[0] == typeof(Object0064_GUNSoldier))
						randomEnemy = r.Next(1, allEnemies.Count); // skip GUN Soldiers otherwise
					else
						randomEnemy = r.Next(allEnemies.Count);
				}
				randomEnemyType = allEnemies[randomEnemy];
			}
			SETMutations.MutateObjectAtIndex(i, randomEnemyType, ref setData, true, r);
		}
	}

	private void RandomizeModels(Random r)
	{
		CopyDirectory(Path.Combine("backup", "character"), Path.Combine(settings.GamePath, "files", "character"), true); // restore all default models
		var mdls = Directory.GetFiles("RandoModels", "shadow.one", SearchOption.AllDirectories).Prepend(Path.Combine("backup", "character", "shadow.one")).ToArray();
		var p1mdl = mdls[r.Next(mdls.Length)]; // pick a random p1 model
		if (p1mdl.Contains("ModelPack")) // if the model belongs to a pack, copy all files from the pack and do nothing else
			CopyDirectory(Path.GetDirectoryName(p1mdl), Path.Combine(settings.GamePath, "files", "character"), true);
		else
		{
			File.Copy(p1mdl, Path.Combine(settings.GamePath, "files", "character", "shadow.one"), true);
			if (Models_CheckBox_ModelP2.IsChecked.Value) // do we care about p2?
			{
				mdls = Directory.EnumerateFiles("RandoModels", "shadow2py.one", SearchOption.AllDirectories).Where(a => !a.Contains("ModelPack")).Prepend(Path.Combine("backup", "character", "shadow2py.one")).ToArray();
				var p2mdl = mdls[r.Next(mdls.Length)]; // pick a random p2 model
				File.Copy(p2mdl, Path.Combine(settings.GamePath, "files", "character", "shadow2py.one"), true);
				if (p2mdl.Contains("CustomBON")) // does the model use custom bones?
				{
					if (!Path.GetDirectoryName(p2mdl).Equals(Path.GetDirectoryName(p1mdl))) // is it from a different folder than the p1 model?
					{
						var datOneData = File.ReadAllBytes(p2mdl);
						ONEArchiveType archiveType = ONEArchiveTester.GetArchiveType(ref datOneData);
						var datOneDataContent = Archive.FromONEFile(ref datOneData);
						if (datOneDataContent.Files.Find(a => a.Name.Equals("HS.BON")) == null) // does the model already come with bones?
						{
							var p1data = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(p2mdl), "shadow.one")); // get the corresponding p1 model
							var p1content = Archive.FromONEFile(ref p1data);
							var bonfile = p1content.Files.Find(a => a.Name.Equals("SH.BON")); // grab p1's bon file
							bonfile.Name = "HS.BON"; // rename it
							datOneDataContent.Files.Add(bonfile); // put it into p2's file
							datOneDataContent.Files.Add(p1content.Files.Find(a => a.Name.EndsWith(".MTP"))); // and the mtp file too
							var updatedDatOneData = datOneDataContent.BuildShadowONEArchive(archiveType == ONEArchiveType.Shadow060);
							File.WriteAllBytes(Path.Combine(settings.GamePath, "files", "character", "shadow2py.one"), updatedDatOneData.ToArray());
						}
					}
				}
				else if (p1mdl.Contains("CustomBON")) // does p1 use custom bones?
				{
					var datOneData = File.ReadAllBytes(p2mdl);
					ONEArchiveType archiveType = ONEArchiveTester.GetArchiveType(ref datOneData);
					var datOneDataContent = Archive.FromONEFile(ref datOneData);
					var p1data = File.ReadAllBytes(Path.Combine("backup", "character", "shadow.one")); // grab the default shadow model
					var p1content = Archive.FromONEFile(ref p1data);
					var bonfile = p1content.Files.Find(a => a.Name.Equals("SH.BON")); // grab p1's bon file
					bonfile.Name = "HS.BON"; // rename it
					datOneDataContent.Files.Add(bonfile); // put it into p2's file
					datOneDataContent.Files.Add(p1content.Files.Find(a => a.Name.EndsWith(".MTP"))); // and the mtp file too
					var updatedDatOneData = datOneDataContent.BuildShadowONEArchive(archiveType == ONEArchiveType.Shadow060);
					File.WriteAllBytes(Path.Combine(settings.GamePath, "files", "character", "shadow2py.one"), updatedDatOneData.ToArray());
				}
			}
		}
	}

	private static void Shuffle<T>(Random r, T[] array, int count)
	{
		int[] order = new int[count];
		for (int i = 0; i < count; i++)
			order[i] = r.Next();
		Array.Sort(order, array);
	}

	private static void Shuffle<T>(Random r, T[] array) => Shuffle(r, array, array.Length);

	private static int GetStageFromLists(Random r, List<int> curset, List<int> stagepool, int weight)
	{
		--weight;
		if (weight <= 0)
			return stagepool[r.Next(stagepool.Count)];
		int tmp = r.Next((curset.Count * weight) + stagepool.Count);
		if (tmp < curset.Count * weight)
			return curset[tmp / weight];
		else
			return stagepool[tmp - (curset.Count * weight)];
	}

	private static string GetStageName(int id)
	{
		if (id == totalstagecount + 1)
			return "Start";
		else if (id == totalstagecount)
			return "Ending";
		return LevelNames[id];
	}

	private int[] FindShortestPath(int start)
	{
		Stack<int> stack = new Stack<int>(stagecount);
		stack.Push(start);
		return FindShortestPath(stages[start], stack, null);
	}

	private int[] FindShortestPath(Stage stage, Stack<int> path, int[] shortestPath)
	{
		if (shortestPath != null && path.Count >= shortestPath.Length)
			return shortestPath;
		if (stage.Neutral != -1 && !path.Contains(stage.Neutral))
		{
			path.Push(stage.Neutral);
			if (stage.Neutral == totalstagecount)
			{
				if (shortestPath == null || path.Count < shortestPath.Length)
				{
					shortestPath = path.ToArray();
					Array.Reverse(shortestPath);
					path.Pop();
					return shortestPath;
				}
			}
			else
				shortestPath = FindShortestPath(stages[stage.Neutral], path, shortestPath);
			path.Pop();
		}
		if (stage.Hero != -1 && !path.Contains(stage.Hero))
		{
			path.Push(stage.Hero);
			if (stage.Hero == totalstagecount)
			{
				if (shortestPath == null || path.Count < shortestPath.Length)
				{
					shortestPath = path.ToArray();
					Array.Reverse(shortestPath);
					path.Pop();
					return shortestPath;
				}
			}
			else
				shortestPath = FindShortestPath(stages[stage.Hero], path, shortestPath);
			path.Pop();
		}
		if (stage.Dark != -1 && !path.Contains(stage.Dark))
		{
			path.Push(stage.Dark);
			if (stage.Dark == totalstagecount)
			{
				if (shortestPath == null || path.Count < shortestPath.Length)
				{
					shortestPath = path.ToArray();
					Array.Reverse(shortestPath);
					path.Pop();
					return shortestPath;
				}
			}
			else
				shortestPath = FindShortestPath(stages[stage.Dark], path, shortestPath);
			path.Pop();
		}
		return shortestPath;
	}

	private void LevelOrder_Button_ProjectPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://github.com/ShadowTheHedgehogHacking/ShadowRando") { UseShellExecute = true });
	}

	private void LevelOrder_CheckBox_Random_Seed_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		LevelOrder_TextBox_Seed.IsEnabled = !LevelOrder_CheckBox_Random_Seed.IsChecked.Value;
	}

	private void LevelOrder_CheckBox_AllowJumpsToSameLevel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		LevelOrder_NumericUpDown_MaxBackwardsJump.Minimum = LevelOrder_NumericUpDown_MaxForwardsJump.Minimum = LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value ? 0 : 1;
	}

	private void Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		if (Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value)
			Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked = false;
	}

	private void Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		if (Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value)
			Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked = false;
	}

	private void Subtitles_CheckBox_OnlyWithLinkedAudio_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
		if (Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value)
			Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked = false;
	}

    private void Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
		if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value)
			Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked = false;
	}

    const int linespace = 8;
    private async void Spoilers_Button_MakeChart_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
	var topLevel = TopLevel.GetTopLevel(this);
		var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
		{
			Title = "Save chart",
			DefaultExtension = ".png",
			FileTypeChoices = [FilePickerFileTypes.ImagePng]
		});

		if (file is null)
			return;
		ChartNode[] levels = new ChartNode[totalstagecount + 2];
		int gridmaxh = 0;
		int gridmaxv = 0;
		switch (settings.LevelOrderMode)
		{
			case LevelOrderMode.AllStagesWarps: // stages + warps
			case LevelOrderMode.BossRush: // boss rush
			case LevelOrderMode.Wild: // wild
				gridmaxh = 1;
				gridmaxv = stagecount + 2;
				for (int i = 0; i <= stagecount; i++)
					levels[stageids[i]] = new ChartNode(0, i + 1);
				levels[totalstagecount + 1] = new ChartNode(0, 0);
				break;
			case LevelOrderMode.BranchingPaths: // branching paths
				{
					int row = 0;
					int col = 0;
					int nextrow = stageids[0];
					for (int i = 0; i < stagecount; i++)
					{
						if (stageids[i] == nextrow)
						{
							++row;
							col = 0;
							nextrow = stages[stageids[i]].Neutral;
						}
						levels[stageids[i]] = new ChartNode(col++, row);
						gridmaxh = Math.Max(col, gridmaxh);
					}
					levels[totalstagecount] = new ChartNode(0, ++row);
					gridmaxv = row + 1;
					levels[totalstagecount + 1] = new ChartNode(0, 0);
				}
				break;
			case LevelOrderMode.ReverseBranching: // reverse branching
				{
					List<List<int>> depthstages = new List<List<int>>() { new List<int>() { totalstagecount } };
					List<Stage> stages2 = new List<Stage>(stageids.Take(stagecount).Select(a => stages[a]));
					while (stages2.Count > 0)
					{
						var next = stages2.Where(a => depthstages[depthstages.Count - 1].Contains(a.Neutral) || depthstages[depthstages.Count - 1].Contains(a.Hero) || depthstages[depthstages.Count - 1].Contains(a.Dark)).Select(a => a.ID).ToList();
						depthstages.Add(next);
						stages2.RemoveAll(a => next.Contains(a.ID));
					}
					depthstages.Add(new List<int>() { totalstagecount + 1 });
					depthstages.Reverse();
					gridmaxh = depthstages.Max(a => a.Count);
					gridmaxv = depthstages.Count;
					int row = 0;
					int col = 0;
					foreach (var ds in depthstages)
					{
						foreach (var id in ds)
							levels[id] = new ChartNode(col++, row);
						gridmaxh = Math.Max(col, gridmaxh);
						++row;
						col = 0;
					}
				}
				break;
			default: // normal game structure
				if (LevelOrder_CheckBox_IncludeBosses.IsChecked ?? false)
				{
					gridmaxh = 1;
					gridmaxv = 11;
					int[] stgcnts = [1, 3, 3, 5, 5, 5, 0];
					int[][] bosses = [[], [8], [4], [4, 8, 10], [2, 6], [], [0, 1, 2, 3, 5, 7, 8, 9, 10]];
					int ind = 0;
					for (int i = 0; i < stgcnts.Length; i++)
					{
						int y = gridmaxv / 4 - stgcnts[i] / 2;
						for (int j = 0; j < stgcnts[i]; j++)
							levels[stageids[ind++]] = new ChartNode(gridmaxh, y++ * 2 + 1);
						if (bosses[i].Length > 0)
							for (int j = 0; j < bosses[i].Length; j++)
								levels[stageids[ind++]] = new ChartNode(gridmaxh, bosses[i][j]);
						gridmaxh++;
					}
					levels[totalstagecount] = new ChartNode(gridmaxh++, 5);
					levels[totalstagecount + 1] = new ChartNode(0, 5);
				}
				else
				{
					gridmaxh = 1;
					gridmaxv = 5;
					int[] stgcnts = [1, 3, 3, 5, 5, 5];
					int ind = 0;
					for (int i = 0; i < stgcnts.Length; i++)
					{
						int y = gridmaxv / 2 - stgcnts[i] / 2;
						for (int j = 0; j < stgcnts[i]; j++)
							levels[stageids[ind++]] = new ChartNode(gridmaxh, y++);
						gridmaxh++;
					}
					levels[totalstagecount] = new ChartNode(gridmaxh++, 2);
					levels[totalstagecount + 1] = new ChartNode(0, 2);
				}
				break;
		}
		levels[totalstagecount + 1].Connect(ConnectionType.Neutral, levels[stageids[0]]);
		for (int i = 0; i < totalstagecount; i++)
		{
			ChartNode node = levels[i];
			if (node == null)
				continue;
			Stage stage = stages[i];
			if (stage.Neutral != -1)
				node.Connect(ConnectionType.Neutral, levels[stage.Neutral]);
			if (stage.Hero != -1)
				node.Connect(ConnectionType.Hero, levels[stage.Hero]);
			if (stage.Dark != -1)
				node.Connect(ConnectionType.Dark, levels[stage.Dark]);
		}
		SKSizeI textsz = SKSizeI.Empty;
		using (var g = new SKPaint())
		{
			foreach (string item in LevelNames)
			{
				SKRect bounds = default;
				g.MeasureText(item, ref bounds);
				if (bounds.Width > textsz.Width)
					textsz.Width = (int)bounds.Width;
				if (bounds.Height > textsz.Height)
					textsz.Height = (int)bounds.Height;
			}
			textsz.Width += 6;
			textsz.Height += 6;
		}
		List<(ChartNode src, ChartConnection con)> shortcons = new List<(ChartNode src, ChartConnection con)>();
		List<ChartConnection>[] vcons = new List<ChartConnection>[gridmaxh * 2];
		for (int i = 0; i < gridmaxh * 2; i++)
			vcons[i] = new List<ChartConnection>();
		List<ChartConnection>[] hcons = new List<ChartConnection>[gridmaxv * 2];
		for (int i = 0; i < gridmaxv * 2; i++)
			hcons[i] = new List<ChartConnection>();
		foreach (var item in levels)
		{
			if (item == null)
				continue;
			textsz.Height = Math.Max((item.OutgoingConnections[Direction.Left].Count + item.IncomingConnections[Direction.Left].Count) * linespace, textsz.Height);
			textsz.Width = Math.Max((item.OutgoingConnections[Direction.Top].Count + item.IncomingConnections[Direction.Top].Count) * linespace, textsz.Width);
			textsz.Height = Math.Max((item.OutgoingConnections[Direction.Right].Count + item.IncomingConnections[Direction.Right].Count) * linespace, textsz.Height);
			textsz.Width = Math.Max((item.OutgoingConnections[Direction.Bottom].Count + item.IncomingConnections[Direction.Bottom].Count) * linespace, textsz.Width);
			shortcons.AddRange(item.OutgoingConnections.SelectMany(a => a.Value).Where(a => item.GetDistance(a.Node) == 1).Select(a => (item, a)));
			vcons[item.GridX * 2].AddRange(item.IncomingConnections[Direction.Left].Where(a => a.Distance != 1));
			vcons[item.GridX * 2 + 1].AddRange(item.IncomingConnections[Direction.Right].Where(a => a.Distance != 1));
			if (item.GridY > 0)
				hcons[item.GridY * 2 - 1].AddRange(item.IncomingConnections[Direction.Top].Where(a => a.Distance != 1 && a.MinY == item.GridY - 1));
			hcons[item.GridY * 2].AddRange(item.IncomingConnections[Direction.Top].Where(a => a.Distance != 1 && a.MinY != item.GridY - 1));
			hcons[item.GridY * 2 + 1].AddRange(item.IncomingConnections[Direction.Bottom].Where(a => a.Distance != 1));
		}
		int conslotsh = textsz.Height / linespace;
		int conslotsv = textsz.Width / linespace;
		int hconoff = (textsz.Height - (conslotsh * linespace)) / 2;
		int vconoff = (textsz.Width - (conslotsv * linespace)) / 2;
		foreach (var item in levels)
		{
			if (item == null)
				continue;
			item.ConnectionOrder[Direction.Left] = new ChartConnection[conslotsh];
			item.ConnectionOrder[Direction.Top] = new ChartConnection[conslotsv];
			item.ConnectionOrder[Direction.Right] = new ChartConnection[conslotsh];
			item.ConnectionOrder[Direction.Bottom] = new ChartConnection[conslotsv];
		}
		foreach (var (src, con) in shortcons)
		{
			ChartConnection[] srcord = src.ConnectionOrder[src.OutgoingConnections.First(a => a.Value.Contains(con)).Key];
			ChartConnection[] dstord = con.Node.ConnectionOrder[con.Side];
			int mid = srcord.Length / 2;
			int slot = mid;
			while (slot < srcord.Length && (srcord[slot] != null || dstord[slot] != null))
				++slot;
			if (slot == srcord.Length)
			{
				slot = mid - 1;
				while (srcord[slot] != null || dstord[slot] != null)
					--slot;
			}
			srcord[slot] = con;
			dstord[slot] = con;
		}
		foreach (var item in levels)
		{
			if (item == null)
				continue;
			int preslots = Array.FindIndex(item.ConnectionOrder[Direction.Left], a => a != null);
			int postslots;
			List<ChartConnection> prelist = new List<ChartConnection>();
			List<ChartConnection> postlist = new List<ChartConnection>();
			if (preslots == -1)
			{
				prelist.AddRange(item.IncomingConnections[Direction.Left]);
				prelist.AddRange(item.OutgoingConnections[Direction.Left]);
				prelist.Sort(CompareConnV);
				prelist.CopyTo(item.ConnectionOrder[Direction.Left], (item.ConnectionOrder[Direction.Left].Length - prelist.Count) / 2);
			}
			else
			{
				postslots = item.ConnectionOrder[Direction.Left].Length - Array.FindIndex(item.ConnectionOrder[Direction.Left], preslots, a => a == null);
				foreach (var con in item.IncomingConnections[Direction.Left].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Left], a) == -1))
				{
					if (con.MaxY == item.GridY)
						prelist.Add(con);
					else if (con.MinY == item.GridY)
						postlist.Add(con);
					else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				foreach (var con in item.OutgoingConnections[Direction.Left].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Left], a) == -1))
				{
					if (con.MinY == item.GridY)
						postlist.Add(con);
					else if (con.MaxY == item.GridY)
						prelist.Add(con);
					else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				if (prelist.Count > 0 || postlist.Count > 0)
				{
					prelist.Sort(CompareConnV);
					postlist.Sort(CompareConnV);
					if (prelist.Count > preslots)
					{
						postlist.InsertRange(0, prelist.Skip(preslots));
						prelist.RemoveRange(preslots, prelist.Count - preslots);
					}
					else if (postlist.Count > postslots)
					{
						prelist.AddRange(postlist.Take(postlist.Count - postslots));
						postlist.RemoveRange(0, postlist.Count - postslots);
					}
					prelist.CopyTo(item.ConnectionOrder[Direction.Left], preslots - prelist.Count);
					postlist.CopyTo(item.ConnectionOrder[Direction.Left], Math.Max(item.ConnectionOrder[Direction.Left].Length - postslots, 0));
				}
			}
			preslots = Array.FindIndex(item.ConnectionOrder[Direction.Top], a => a != null);
			prelist.Clear();
			postlist.Clear();
			if (preslots == -1)
			{
				prelist.AddRange(item.IncomingConnections[Direction.Top]);
				prelist.AddRange(item.OutgoingConnections[Direction.Top]);
				prelist.Sort(CompareConnV);
				prelist.CopyTo(item.ConnectionOrder[Direction.Top], (item.ConnectionOrder[Direction.Top].Length - prelist.Count) / 2);
			}
			else
			{
				postslots = item.ConnectionOrder[Direction.Top].Length - Array.FindIndex(item.ConnectionOrder[Direction.Top], preslots, a => a == null);
				foreach (var con in item.IncomingConnections[Direction.Top].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Top], a) == -1))
				{
					if (con.MaxX == item.GridX)
						prelist.Add(con);
					else if (con.MinX == item.GridX)
						postlist.Add(con);
					else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				foreach (var con in item.OutgoingConnections[Direction.Top].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Top], a) == -1))
				{
					if (con.MinX == item.GridX)
						postlist.Add(con);
					else if (con.MaxX == item.GridX)
						prelist.Add(con);
					else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				if (prelist.Count > 0 || postlist.Count > 0)
				{
					prelist.Sort(CompareConnH);
					postlist.Sort(CompareConnH);
					if (prelist.Count > preslots)
					{
						postlist.InsertRange(0, prelist.Skip(preslots));
						prelist.RemoveRange(preslots, prelist.Count - preslots);
					}
					else if (postlist.Count > postslots)
					{
						prelist.AddRange(postlist.Take(postlist.Count - postslots));
						postlist.RemoveRange(0, postlist.Count - postslots);
					}
					prelist.CopyTo(item.ConnectionOrder[Direction.Top], preslots - prelist.Count);
					postlist.CopyTo(item.ConnectionOrder[Direction.Top], item.ConnectionOrder[Direction.Top].Length - postslots);
				}
			}
			preslots = Array.FindIndex(item.ConnectionOrder[Direction.Right], a => a != null);
			prelist.Clear();
			postlist.Clear();
			if (preslots == -1)
			{
				prelist.AddRange(item.IncomingConnections[Direction.Right]);
				prelist.AddRange(item.OutgoingConnections[Direction.Right]);
				prelist.Sort(CompareConnV);
				prelist.CopyTo(item.ConnectionOrder[Direction.Right], (item.ConnectionOrder[Direction.Right].Length - prelist.Count) / 2);
			}
			else
			{
				postslots = item.ConnectionOrder[Direction.Right].Length - Array.FindIndex(item.ConnectionOrder[Direction.Right], preslots, a => a == null);
				foreach (var con in item.IncomingConnections[Direction.Right].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Right], a) == -1))
				{
					if (con.MaxY == item.GridY)
						prelist.Add(con);
					else if (con.MinY == item.GridY)
						postlist.Add(con);
					else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				foreach (var con in item.OutgoingConnections[Direction.Right].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Right], a) == -1))
				{
					if (con.MinY == item.GridY)
						postlist.Add(con);
					else if (con.MaxY == item.GridY)
						prelist.Add(con);
					else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				if (prelist.Count > 0 || postlist.Count > 0)
				{
					prelist.Sort(CompareConnV);
					postlist.Sort(CompareConnV);
					if (prelist.Count > preslots)
					{
						postlist.InsertRange(0, prelist.Skip(preslots));
						prelist.RemoveRange(preslots, prelist.Count - preslots);
					}
					else if (postlist.Count > postslots)
					{
						prelist.AddRange(postlist.Take(postlist.Count - postslots));
						postlist.RemoveRange(0, postlist.Count - postslots);
					}
					prelist.CopyTo(item.ConnectionOrder[Direction.Right], preslots - prelist.Count);
					postlist.CopyTo(item.ConnectionOrder[Direction.Right], item.ConnectionOrder[Direction.Right].Length - postslots);
				}
			}
			preslots = Array.FindIndex(item.ConnectionOrder[Direction.Bottom], a => a != null);
			prelist.Clear();
			postlist.Clear();
			if (preslots == -1)
			{
				prelist.AddRange(item.IncomingConnections[Direction.Bottom]);
				prelist.AddRange(item.OutgoingConnections[Direction.Bottom]);
				prelist.Sort(CompareConnV);
				prelist.CopyTo(item.ConnectionOrder[Direction.Bottom], (item.ConnectionOrder[Direction.Bottom].Length - prelist.Count) / 2);
			}
			else
			{
				postslots = item.ConnectionOrder[Direction.Bottom].Length - Array.FindIndex(item.ConnectionOrder[Direction.Bottom], preslots, a => a == null);
				foreach (var con in item.IncomingConnections[Direction.Bottom].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Bottom], a) == -1))
				{
					if (con.MaxX == item.GridX)
						prelist.Add(con);
					else if (con.MinX == item.GridX)
						postlist.Add(con);
					else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				foreach (var con in item.OutgoingConnections[Direction.Bottom].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Bottom], a) == -1))
				{
					if (con.MinX == item.GridX)
						postlist.Add(con);
					else if (con.MaxX == item.GridX)
						prelist.Add(con);
					else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
						prelist.Add(con);
					else
						postlist.Add(con);
				}
				if (prelist.Count > 0 || postlist.Count > 0)
				{
					prelist.Sort(CompareConnH);
					postlist.Sort(CompareConnH);
					if (prelist.Count > preslots)
					{
						postlist.InsertRange(0, prelist.Skip(preslots));
						prelist.RemoveRange(preslots, prelist.Count - preslots);
					}
					else if (postlist.Count > postslots)
					{
						prelist.AddRange(postlist.Take(postlist.Count - postslots));
						postlist.RemoveRange(0, postlist.Count - postslots);
					}
					prelist.CopyTo(item.ConnectionOrder[Direction.Bottom], preslots - prelist.Count);
					postlist.CopyTo(item.ConnectionOrder[Direction.Bottom], item.ConnectionOrder[Direction.Bottom].Length - postslots);
				}
			}
		}
		int vlanemax = 0;
		foreach (var list in vcons)
		{
			list.Sort((a, b) =>
			{
				int r = a.Distance.CompareTo(b.Distance);
				if (r == 0)
				{
					r = a.MinY.CompareTo(b.MinY);
					if (r == 0)
						r = a.Type.CompareTo(b.Type);
				}
				return r;
			});
			for (int i = 0; i < list.Count; i++)
			{
				var line = list[i];
				for (int j = 0; j < i; j++)
					if (list[j].Lane == line.Lane && line.MaxY >= list[j].MinY && list[j].MaxY >= line.MinY)
					{
						line.Lane++;
						j = -1;
					}
				vlanemax = Math.Max(line.Lane + 1, vlanemax);
			}
		}
		int hlanemax = 0;
		foreach (var list in hcons)
		{
			list.Sort((a, b) =>
			{
				int r = a.Distance.CompareTo(b.Distance);
				if (r == 0)
				{
					r = a.MinX.CompareTo(b.MinX);
					if (r == 0)
						r = a.Type.CompareTo(b.Type);
				}
				return r;
			});
			for (int i = 0; i < list.Count; i++)
			{
				var line = list[i];
				for (int j = 0; j < i; j++)
					if (list[j].Lane == line.Lane && line.MaxX >= list[j].MinX && list[j].MaxX >= line.MinX)
					{
						line.Lane++;
						j = -1;
					}
				hlanemax = Math.Max(line.Lane + 1, hlanemax);
			}
		}
		int margin = Math.Min(textsz.Width / 2, textsz.Height / 2);
		int hmargin = Math.Max(vlanemax * linespace + 5, margin);
		int vmargin = Math.Max(hlanemax * linespace + 5, margin);
		int colwidth = textsz.Width + hmargin * 2;
		int rowheight = textsz.Height + vmargin * 2;
		var info = new SKImageInfo(colwidth * gridmaxh, rowheight * gridmaxv);

		//Color Adjustments per side
		int leftSideCount = 0;
		int rightSideCount = 0;

		using (SKSurface surface = SKSurface.Create(info))
		{
			SKCanvas gfx = surface.Canvas;
			using (SKPaint rectPaint = new SKPaint() { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 1 })
			using (SKPaint textPaint = new SKPaint() { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 1, TextAlign = SKTextAlign.Center, IsAntialias = true })
			using (SKPaint linePaint = new SKPaint() { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 3 })
			using (SKPaint triPaint = new SKPaint() { Color = SKColors.Black, Style = SKPaintStyle.Fill, IsAntialias = true })
			using (var dash = SKPathEffect.CreateDash([9, 3], 0))
			{
				gfx.Clear(SKColors.White);
				List<int> stageorder = new List<int>(totalstagecount + 2)
				{
					totalstagecount + 1
				};
				stageorder.AddRange(stageids);
				stageorder.Reverse();
				foreach (var id in stageorder)
				{
					var node = levels[id];
					int x = colwidth * node.GridX + hmargin;
					int y = rowheight * node.GridY + vmargin;
					gfx.DrawRect(x, y, textsz.Width, textsz.Height, rectPaint);
					gfx.DrawText(GetStageName(id), x + textsz.Width / 2, y + textsz.Height / 2 + textPaint.FontMetrics.XHeight / 2, textPaint);
					foreach (var (dir, list) in node.OutgoingConnections)
						foreach (var con in list)
						{
							int srclane = Array.LastIndexOf(node.ConnectionOrder[dir], con);
							int srcx = 0;
							int srcy = 0;
							switch (dir)
							{
								case Direction.Left:
									srcx = x;
									srcy = y + hconoff + (srclane * linespace) + (linespace / 2);
									break;
								case Direction.Top:
									srcx = x + vconoff + (srclane * linespace) + (linespace / 2);
									srcy = y;
									break;
								case Direction.Right:
									srcx = x + textsz.Width + 1;
									srcy = y + hconoff + (srclane * linespace) + (linespace / 2);
									break;
								case Direction.Bottom:
									srcx = x + vconoff + (srclane * linespace) + (linespace / 2);
									srcy = y + textsz.Height + 1;
									break;
							}
							int dstlane = Array.IndexOf(con.Node.ConnectionOrder[con.Side], con);
							int dstx = colwidth * con.Node.GridX + hmargin;
							int dsty = rowheight * con.Node.GridY + vmargin;

							SKColor lineColor = new SKColor();
							switch (con.Type)
							{
								case ConnectionType.Neutral:
									lineColor = SKColors.Black;
									break;
								case ConnectionType.Hero:
									lineColor = SKColors.Blue;
									break;
								case ConnectionType.Dark:
									lineColor = SKColors.Red;
									break;
							}

							var shiftAmount = 0;
							switch (con.Side)
							{
								case Direction.Left:
									dsty += hconoff + (dstlane * linespace) + (linespace / 2);
									shiftAmount = leftSideCount++;
									break;
								case Direction.Top:
									dstx += vconoff + (dstlane * linespace) + (linespace / 2);
									break;
								case Direction.Right:
									dstx += textsz.Width + 1;
									dsty += hconoff + (dstlane * linespace) + (linespace / 2);
									shiftAmount = rightSideCount++;
									break;
								case Direction.Bottom:
									dstx += vconoff + (dstlane * linespace) + (linespace / 2);
									dsty += textsz.Height + 1;
									break;
							}

							var lineHSV = new float[3];
							lineColor.ToHsv(out lineHSV[0], out lineHSV[1], out lineHSV[2]);
							if (shiftAmount % 2 == 0)
							{
								//Darkness Shfit
								triPaint.Color = linePaint.Color = SKColor.FromHsv(lineHSV[0], lineHSV[1], lineHSV[2] - ((float)Spoilers_NumericUpDown_DarknessShift.Value * (shiftAmount/2)));
							}
							else
							{
								//Hue Shift
								triPaint.Color = linePaint.Color = SKColor.FromHsv(lineHSV[0] + ((float)Spoilers_NumericUpDown_HueShift.Value * (shiftAmount/2)), lineHSV[1], lineHSV[2]);
							}

							if (con.MaxX - con.MinX != 1 && con.MaxY - con.MinY != 1)
								linePaint.PathEffect = dash;
							if (node.GetDistance(con.Node) == 1)
								gfx.DrawLine(srcx, srcy, dstx, dsty, linePaint);
							else
							{
								using (var path = new SKPath())
								{
									path.MoveTo(srcx, srcy);
									int midx = srcx;
									int midy = srcy;
									switch (dir)
									{
										case Direction.Left:
											midx -= con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Top:
											midy -= con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Right:
											midx += con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Bottom:
											midy += con.Lane * linespace + (linespace / 2) + 5;
											break;
									}
									path.LineTo(midx, midy);
									switch (dir)
									{
										case Direction.Left:
										case Direction.Right:
											path.LineTo(midx, dsty);
											path.LineTo(dstx, dsty);
											break;
										case Direction.Top:
										case Direction.Bottom:
											path.LineTo(dstx, midy);
											path.LineTo(dstx, dsty);
											break;
									}
									gfx.DrawPath(path, linePaint);
								}
							}
							using (var tri = new SKPath())
							{
								tri.MoveTo(dstx, dsty);
								switch (con.Side)
								{
									case Direction.Left:
										tri.RLineTo(-8.5f, -5);
										tri.RLineTo(0, 10);
										break;
									case Direction.Top:
										tri.RLineTo(-5, -8.5f);
										tri.RLineTo(10, 0);
										break;
									case Direction.Right:
										tri.RLineTo(8.5f, 5);
										tri.RLineTo(0, -10);
										break;
									case Direction.Bottom:
										tri.RLineTo(5, 8.5f);
										tri.RLineTo(-10, 0);
										break;
								}
								tri.Close();
								gfx.DrawPath(tri, triPaint);
							}
						}
				}
			}
			using (var img = surface.Snapshot())
			using (var data = img.Encode())
			using (var stream = await file.OpenWriteAsync())
				data.SaveTo(stream);
		}
	}

	private static int CompareConnV(ChartConnection a, ChartConnection b)
	{
		int r = a.MinY.CompareTo(b.MinY);
		if (r == 0)
			r = a.MaxY.CompareTo(b.MaxY);
		return r;
	}

	private static int CompareConnH(ChartConnection a, ChartConnection b)
	{
		int r = a.MinX.CompareTo(b.MinX);
		if (r == 0)
			r = a.MaxX.CompareTo(b.MaxX);
		return r;
	}

    private async void Spoilers_Button_SaveLog_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel is null) return;
		var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
		{
			Title = "Save log",
			DefaultExtension = ".txt",
			FileTypeChoices = [FilePickerFileTypes.TextPlain]
		});

		if (file is null) return;
		await using var stream = await file.OpenWriteAsync();
		await using var sw = new StreamWriter(stream);
		await sw.WriteLineAsync($"ShadowRando Version: {programVersion}");
		await sw.WriteLineAsync($"Seed: {LevelOrder_TextBox_Seed.Text}");
		await sw.WriteLineAsync("---- Level Order ----");
		await sw.WriteLineAsync($"Level Order Mode: {settings.LevelOrderMode}");
		if (settings.LevelOrderMode == LevelOrderMode.AllStagesWarps)
		{
			await sw.WriteLineAsync($"Main Path: {settings.LevelOrderMainPath}");
			await sw.WriteLineAsync($"Max Forwards Jump: {LevelOrder_NumericUpDown_MaxForwardsJump.Value}");
			await sw.WriteLineAsync($"Max Backwards Jump: {LevelOrder_NumericUpDown_MaxBackwardsJump.Value}");
			await sw.WriteLineAsync($"Backwards Jump Probability: {LevelOrder_NumericUpDown_BackwardsJumpProbability.Value}");
			await sw.WriteLineAsync($"Allow Jumps To Same Level: {LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value}");
		}
		await sw.WriteLineAsync($"Include Last Story: {LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value}");
		await sw.WriteLineAsync($"Include Bosses: {LevelOrder_CheckBox_IncludeBosses.IsChecked.Value}");
		await sw.WriteLineAsync($"Allow Boss -> Boss: {LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value}");

		await sw.WriteLineAsync("---- Layout ----");
		await sw.WriteLineAsync($"Randomize Layouts: {Layout_CheckBox_RandomizeLayouts.IsChecked.Value}");
		if (settings.RandomizeLayouts == true)
		{
			await sw.WriteLineAsync($"Make CC Splines Vehicle Compatible: {Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked.Value}");
			await sw.WriteLineAsync("--- Enemy ---");
			await sw.WriteLineAsync($"Enemy Mode: {settings.LayoutEnemyMode}");
			await sw.WriteLineAsync($"Adjust Mission Counts: {Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value}");
			await sw.WriteLineAsync($"Adjust Mission Counts Reduction %: {Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value}");
			await sw.WriteLineAsync($"Keep Type: {Layout_Enemy_CheckBox_KeepType.IsChecked.Value}");
			await sw.WriteLineAsync($"Only Selected Enemy Types: {Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value}");
			if (Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value)
			{
				await sw.WriteLineAsync($"GUN Soldier: {Layout_Enemy_CheckBox_SelectedEnemy_GUNSoldier.IsChecked.Value}");
				await sw.WriteLineAsync($"GUN Beetle: {Layout_Enemy_CheckBox_SelectedEnemy_GUNBeetle.IsChecked.Value}");
				await sw.WriteLineAsync($"GUN Bigfoot: {Layout_Enemy_CheckBox_SelectedEnemy_GUNBigfoot.IsChecked.Value}");
				await sw.WriteLineAsync($"GUN Robot: {Layout_Enemy_CheckBox_SelectedEnemy_GUNRobot.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Pierrot: {Layout_Enemy_CheckBox_SelectedEnemy_EggPierrot.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Pawn: {Layout_Enemy_CheckBox_SelectedEnemy_EggPawn.IsChecked.Value}");
				await sw.WriteLineAsync($"Shadow Android: {Layout_Enemy_CheckBox_SelectedEnemy_ShadowAndroid.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Giant: {Layout_Enemy_CheckBox_SelectedEnemy_BAGiant.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Soldier: {Layout_Enemy_CheckBox_SelectedEnemy_BASoldier.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Hawk/Volt: {Layout_Enemy_CheckBox_SelectedEnemy_BAHawkVolt.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Wing: {Layout_Enemy_CheckBox_SelectedEnemy_BAWing.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Worm: {Layout_Enemy_CheckBox_SelectedEnemy_BAWorm.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Larva: {Layout_Enemy_CheckBox_SelectedEnemy_BALarva.IsChecked.Value}");
				await sw.WriteLineAsync($"Artificial Chaos: {Layout_Enemy_CheckBox_SelectedEnemy_ArtificialChaos.IsChecked.Value}");
				await sw.WriteLineAsync($"BA Assassin: {Layout_Enemy_CheckBox_SelectedEnemy_BAAssassin.IsChecked.Value}");
			}
			await sw.WriteLineAsync("--- Weapon ---");
			await sw.WriteLineAsync($"Random Weapons In Weapon Boxes: {Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value}");
			await sw.WriteLineAsync($"Random Weapons In All Boxes: {Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value}");
			await sw.WriteLineAsync($"Random Exposed Weapons: {Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked.Value}");
			await sw.WriteLineAsync($"Environment Drops Random Weapons: {Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked.Value}");
			await sw.WriteLineAsync($"Only Selected Weapons: {Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value}");
			if (Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value)
			{
				await sw.WriteLineAsync($"None: {Layout_Weapon_CheckBox_SelectedWeapon_None.IsChecked.Value}");
				await sw.WriteLineAsync($"Pistol: {Layout_Weapon_CheckBox_SelectedWeapon_Pistol.IsChecked.Value}");
				await sw.WriteLineAsync($"Submachine Gun: {Layout_Weapon_CheckBox_SelectedWeapon_SubmachineGun.IsChecked.Value}");
				await sw.WriteLineAsync($"Assault Rifle: {Layout_Weapon_CheckBox_SelectedWeapon_AssaultRifle.IsChecked.Value}");
				await sw.WriteLineAsync($"Heavy Machine Gun: {Layout_Weapon_CheckBox_SelectedWeapon_HeavyMachineGun.IsChecked.Value}");
				await sw.WriteLineAsync($"Gatling Gun: {Layout_Weapon_CheckBox_SelectedWeapon_GatlingGun.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Pistol: {Layout_Weapon_CheckBox_SelectedWeapon_EggPistol.IsChecked.Value}");
				await sw.WriteLineAsync($"Light Shot: {Layout_Weapon_CheckBox_SelectedWeapon_LightShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Flash Shot: {Layout_Weapon_CheckBox_SelectedWeapon_FlashShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Ring Shot: {Layout_Weapon_CheckBox_SelectedWeapon_RingShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Heavy Shot: {Layout_Weapon_CheckBox_SelectedWeapon_HeavyShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Grenade Launcher: {Layout_Weapon_CheckBox_SelectedWeapon_GrenadeLauncher.IsChecked.Value}");
				await sw.WriteLineAsync($"GUN Bazooka: {Layout_Weapon_CheckBox_SelectedWeapon_GUNBazooka.IsChecked.Value}");
				await sw.WriteLineAsync($"Tank Cannon: {Layout_Weapon_CheckBox_SelectedWeapon_TankCannon.IsChecked.Value}");
				await sw.WriteLineAsync($"Black Barrel: {Layout_Weapon_CheckBox_SelectedWeapon_BlackBarrel.IsChecked.Value}");
				await sw.WriteLineAsync($"Big Barrel: {Layout_Weapon_CheckBox_SelectedWeapon_BigBarrel.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Bazooka: {Layout_Weapon_CheckBox_SelectedWeapon_EggBazooka.IsChecked.Value}");
				await sw.WriteLineAsync($"RPG: {Layout_Weapon_CheckBox_SelectedWeapon_RPG.IsChecked.Value}");
				await sw.WriteLineAsync($"Four Shot: {Layout_Weapon_CheckBox_SelectedWeapon_FourShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Eight Shot: {Layout_Weapon_CheckBox_SelectedWeapon_EightShot.IsChecked.Value}");
				await sw.WriteLineAsync($"Worm Shooter Black: {Layout_Weapon_CheckBox_SelectedWeapon_WormShooterBlack.IsChecked.Value}");
				await sw.WriteLineAsync($"Worm Shooter Red: {Layout_Weapon_CheckBox_SelectedWeapon_WormShooterRed.IsChecked.Value}");
				await sw.WriteLineAsync($"Worm Shooter Gold: {Layout_Weapon_CheckBox_SelectedWeapon_WormShooterGold.IsChecked.Value}");
				await sw.WriteLineAsync($"Vacuum Pod: {Layout_Weapon_CheckBox_SelectedWeapon_VacuumPod.IsChecked.Value}");
				await sw.WriteLineAsync($"Laser Rifle: {Layout_Weapon_CheckBox_SelectedWeapon_LaserRifle.IsChecked.Value}");
				await sw.WriteLineAsync($"Splitter: {Layout_Weapon_CheckBox_SelectedWeapon_Splitter.IsChecked.Value}");
				await sw.WriteLineAsync($"Refractor: {Layout_Weapon_CheckBox_SelectedWeapon_Refractor.IsChecked.Value}");
				await sw.WriteLineAsync($"Knife: {Layout_Weapon_CheckBox_SelectedWeapon_Knife.IsChecked.Value}");
				await sw.WriteLineAsync($"Black Sword: {Layout_Weapon_CheckBox_SelectedWeapon_BlackSword.IsChecked.Value}");
				await sw.WriteLineAsync($"Dark Hammer: {Layout_Weapon_CheckBox_SelectedWeapon_DarkHammer.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Lance: {Layout_Weapon_CheckBox_SelectedWeapon_EggLance.IsChecked.Value}");
				await sw.WriteLineAsync($"Samurai Sword Lv1: {Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv1.IsChecked.Value}");
				await sw.WriteLineAsync($"Samurai Sword Lv2: {Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv2.IsChecked.Value}");
				await sw.WriteLineAsync($"Satellite Laser Lv1: {Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv1.IsChecked.Value}");
				await sw.WriteLineAsync($"Satellite Laser Lv2: {Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv2.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Vacuum Lv1: {Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv1.IsChecked.Value}");
				await sw.WriteLineAsync($"Egg Vacuum Lv2: {Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv2.IsChecked.Value}");
				await sw.WriteLineAsync($"Omochao Gun Lv1: {Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv1.IsChecked.Value}");
				await sw.WriteLineAsync($"Omochao Gun Lv2: {Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv2.IsChecked.Value}");
				await sw.WriteLineAsync($"Heal Cannon Lv1: {Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv1.IsChecked.Value}");
				await sw.WriteLineAsync($"Heal Cannon Lv2: {Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv2.IsChecked.Value}");
				await sw.WriteLineAsync($"Shadow Rifle: {Layout_Weapon_CheckBox_SelectedWeapon_ShadowRifle.IsChecked.Value}");
			}
			await sw.WriteLineAsync("--- Partner ---");
			await sw.WriteLineAsync($"Partner Mode: {settings.LayoutPartnerMode}");
			await sw.WriteLineAsync($"Keep Affiliation of Original Object: {Layout_Partner_CheckBox_KeepAffiliationOfOriginalObject.IsChecked.Value}");
		}
			
		await sw.WriteLineAsync("---- Subtitles ----");
		await sw.WriteLineAsync($"Randomize Subtitles / Voicelines: {Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value}");
		await sw.WriteLineAsync($"Only With Linked Audio: {Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value}");
		await sw.WriteLineAsync($"Give Audio to No Linked Audio Subtitles: {Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value}");
		await sw.WriteLineAsync($"No System Messages: {Subtitles_CheckBox_NoSystemMessages.IsChecked.Value}");
		await sw.WriteLineAsync($"No Duplicates: {Subtitles_CheckBox_NoDuplicates.IsChecked.Value}");
		await sw.WriteLineAsync($"Only Selected Characters: {Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value}");
		if (Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value)
		{
			await sw.WriteLineAsync($"Shadow: {Subtitles_CheckBox_SelectedCharacter_Shadow.IsChecked.Value}");
			await sw.WriteLineAsync($"Sonic: {Subtitles_CheckBox_SelectedCharacter_Sonic.IsChecked.Value}");
			await sw.WriteLineAsync($"Tails: {Subtitles_CheckBox_SelectedCharacter_Tails.IsChecked.Value}");
			await sw.WriteLineAsync($"Knuckles: {Subtitles_CheckBox_SelectedCharacter_Knuckles.IsChecked.Value}");
			await sw.WriteLineAsync($"Amy: {Subtitles_CheckBox_SelectedCharacter_Amy.IsChecked.Value}");
			await sw.WriteLineAsync($"Rouge: {Subtitles_CheckBox_SelectedCharacter_Rouge.IsChecked.Value}");
			await sw.WriteLineAsync($"Omega: {Subtitles_CheckBox_SelectedCharacter_Omega.IsChecked.Value}");
			await sw.WriteLineAsync($"Vector: {Subtitles_CheckBox_SelectedCharacter_Vector.IsChecked.Value}");
			await sw.WriteLineAsync($"Espio: {Subtitles_CheckBox_SelectedCharacter_Espio.IsChecked.Value}");
			await sw.WriteLineAsync($"Maria: {Subtitles_CheckBox_SelectedCharacter_Maria.IsChecked.Value}");
			await sw.WriteLineAsync($"Charmy: {Subtitles_CheckBox_SelectedCharacter_Charmy.IsChecked.Value}");
			await sw.WriteLineAsync($"Eggman: {Subtitles_CheckBox_SelectedCharacter_Eggman.IsChecked.Value}");
			await sw.WriteLineAsync($"Black Doom: {Subtitles_CheckBox_SelectedCharacter_BlackDoom.IsChecked.Value}");
			await sw.WriteLineAsync($"Cream: {Subtitles_CheckBox_SelectedCharacter_Cream.IsChecked.Value}");
			await sw.WriteLineAsync($"Cheese: {Subtitles_CheckBox_SelectedCharacter_Cheese.IsChecked.Value}");
			await sw.WriteLineAsync($"GUN Soldier: {Subtitles_CheckBox_SelectedCharacter_GUNSoldier.IsChecked.Value}");
			await sw.WriteLineAsync($"GUN Commander: {Subtitles_CheckBox_SelectedCharacter_GUNCommander.IsChecked.Value}");
		}

		await sw.WriteLineAsync("---- Music ----");
		await sw.WriteLineAsync($"Randomize Music: {Music_CheckBox_RandomizeMusic.IsChecked.Value}");
		await sw.WriteLineAsync($"Skip Chaos Power Use Jingles: {Music_CheckBox_SkipChaosPowerUseJingles.IsChecked.Value}");
		await sw.WriteLineAsync($"Skip Rank Theme: {Music_CheckBox_SkipRankTheme.IsChecked.Value}");
		await sw.WriteLineAsync();
		await sw.WriteLineAsync("---- Models ----");
		await sw.WriteLineAsync($"Randomize Player Model: {Models_CheckBox_RandomizeModel.IsChecked.Value}");
		await sw.WriteLineAsync($"Randomize Player 2's Model: {Models_CheckBox_ModelP2.IsChecked.Value}");
		await sw.WriteLineAsync();
		await sw.WriteLineAsync("---- Stage Reorder Result ----");
		for (var i = 0; i < stagecount; i++)
		{
			Stage stg = stages[stageids[i]];
			await sw.WriteLineAsync($"{GetStageName(stageids[i])}:");
			if (stg.Neutral != -1)
				await sw.WriteLineAsync($"Neutral -> {GetStageName(stg.Neutral)} ({Array.IndexOf(stageids, stg.Neutral) - i:+##;-##;0})");
			if (stg.Hero != -1)
				await sw.WriteLineAsync($"Hero -> {GetStageName(stg.Hero)} ({Array.IndexOf(stageids, stg.Hero) - i:+##;-##;0})");
			if (stg.Dark != -1)
				await sw.WriteLineAsync($"Dark -> {GetStageName(stg.Dark)} ({Array.IndexOf(stageids, stg.Dark) - i:+##;-##;0})");
			await sw.WriteLineAsync();
		}
		sw.Close();
    }

	private void Spoilers_ListBox_LevelList_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
	{
		if (Spoilers_ListBox_LevelList.SelectedIndex == -1) return;
		var sb = new System.Text.StringBuilder();
		Stage stg = stages[stageids[Spoilers_ListBox_LevelList.SelectedIndex]];
		switch (settings.LevelOrderMode)
		{
			case LevelOrderMode.AllStagesWarps:
				if (stg.Neutral != -1)
					sb.AppendLine($"Neutral -> {GetStageName(stg.Neutral)} ({Array.IndexOf(stageids, stg.Neutral) - Spoilers_ListBox_LevelList.SelectedIndex:+##;-##;0})");
				if (stg.Hero != -1)
					sb.AppendLine($"Hero -> {GetStageName(stg.Hero)} ({Array.IndexOf(stageids, stg.Hero) - Spoilers_ListBox_LevelList.SelectedIndex:+##;-##;0})");
				if (stg.Dark != -1)
					sb.AppendLine($"Dark -> {GetStageName(stg.Dark)} ({Array.IndexOf(stageids, stg.Dark) - Spoilers_ListBox_LevelList.SelectedIndex:+##;-##;0})");
				sb.Append($"{Environment.NewLine}Shortest Path: ");
				int[] shortestPath;
				if (LevelOrder_NumericUpDown_MaxForwardsJump.Value < 2)
				{
					shortestPath = new int[stagecount - 1 - Spoilers_ListBox_LevelList.SelectedIndex];
					Array.Copy(stageids, Spoilers_ListBox_LevelList.SelectedIndex, shortestPath, 0, shortestPath.Length);
				}
				else
					shortestPath = FindShortestPath(stageids[Spoilers_ListBox_LevelList.SelectedIndex]);
				for (int i = 0; i < shortestPath.Length - 1; i++)
				{
					string exit;
					if (stages[shortestPath[i]].Neutral == shortestPath[i + 1])
						exit = "Neutral";
					else if (stages[shortestPath[i]].Hero == shortestPath[i + 1])
						exit = "Hero";
					else
						exit = "Dark";
					sb.AppendFormat("{2}{0} ({1}) -> ", GetStageName(shortestPath[i]), exit, Environment.NewLine);
				}
				sb.AppendFormat("{1}Ending ({0} levels)", shortestPath.Length, Environment.NewLine);
				break;
			case LevelOrderMode.ReverseBranching:
				if (stg.Neutral != -1)
					sb.AppendLine($"Neutral -> {GetStageName(stg.Neutral)}");
				if (stg.Hero != -1)
					sb.AppendLine($"Hero -> {GetStageName(stg.Hero)}");
				if (stg.Dark != -1)
					sb.AppendLine($"Dark -> {GetStageName(stg.Dark)}");
				sb.AppendFormat("{0}Shortest Path: ", Environment.NewLine);
				shortestPath = FindShortestPath(stageids[Spoilers_ListBox_LevelList.SelectedIndex]);
				for (int i = 0; i < shortestPath.Length - 1; i++)
				{
					string exit;
					if (stages[shortestPath[i]].Neutral == shortestPath[i + 1])
						exit = "Neutral";
					else if (stages[shortestPath[i]].Hero == shortestPath[i + 1])
						exit = "Hero";
					else
						exit = "Dark";
					sb.AppendFormat("{2}{0} ({1}) -> ", GetStageName(shortestPath[i]), exit, Environment.NewLine);
				}
				sb.AppendFormat("{1}Ending ({0} levels)", shortestPath.Length, Environment.NewLine);
				break;
			default:
				if (stg.Neutral != -1)
					sb.AppendLine($"Neutral -> {GetStageName(stg.Neutral)}");
				if (stg.Hero != -1)
					sb.AppendLine($"Hero -> {GetStageName(stg.Hero)}");
				if (stg.Dark != -1)
					sb.AppendLine($"Dark -> {GetStageName(stg.Dark)}");
				break;
		}
		Spoilers_TextBox_LevelInfo.Text = sb.ToString();
	}

	private void LevelOrder_Button_Credits_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		ShowSimpleMessage("Credits", 
			"Additional Contributors:\n" +
			 "BlazinZzetti\n" +
	         "\nSoftware Used:\n" +
	         "AFSLib\n" +
	         "Avalonia UI\n" +
	         "HeroesONE-Reloaded\n" +
	         "MessageBox.Avalonia\n" +
	         "prs.net - temporary; to be replaced with prs-rs\n" +
	         "ShadowFNT\n" +
	         "ShadowSET\n" +
	         "SkiaSharp",
			ButtonEnum.Ok, Icon.None);
	}
}
