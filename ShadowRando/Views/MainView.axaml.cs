using Avalonia.Controls;
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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Platform;

namespace ShadowRando.Views;

public partial class MainView : UserControl
{
	const int stagefirst = 5;
	static readonly string[] LevelNames =
	{
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
	};
	const int routeMenu6xxStagePreviewBlockerOffset = 0xB48B8;
	const int routeMenu6xxStagePreviewPatchValue = 0x48000110;
	const int storyModeStartAddress = 0x2CB9F0;
	const int firstStageOffset = 0x4C1BA8;
	const int modeOffset = 8;
	const int darkOffset = 0x1C;
	const int neutOffset = 0x28;
	const int heroOffset = 0x34;
	const int stageOffset = 0x50;
	static readonly Dictionary<int, int> stageAssociationIDMap = new Dictionary<int, int>
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

	const int totalstagecount = 40;
	static int stagecount = 40;
	int[] stageids;
	readonly Stage[] stages = new Stage[totalstagecount];

	static readonly Dictionary<int, Tuple<String, int>> nukkoro2EnemyCountStagesMap = new Dictionary<int, Tuple<String, int>>
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

	static readonly Type[] enemyTypeMap = [
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

	static readonly Type[] groundEnemyTypeMap = [
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

	static readonly Type[] flyingEnemyTypeMap = [
		typeof(Object0065_GUNBeetle),
		typeof(Object008E_BkWingLarge),
		typeof(Object008F_BkWingSmall),
		typeof(Object0092_BkChaos),
		typeof(Object0066_GUNBigfoot), // only if AppearType is ZUTTO_HOVERING
		typeof(Object0093_BkNinja), // only if AppearType is ON_AIR_SAUCER_WARP
	];

	public static readonly EWeapon[] weapons = [
		EWeapon.None,
		EWeapon.Pistol,
		EWeapon.SubmachineGun,
		EWeapon.MachineGun,
		EWeapon.HeavyMachineGun,
		EWeapon.GatlingGun,
		// EWeapon.None06,
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
		// EWeapon.UnusedGUNWeaponSlot,
		// EWeapon.UnusedBlackArmsWeaponSlot,
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
	private bool programInitialized = false;

	public MainView()
	{
		InitializeComponent();
	}

	private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);
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

		LoadGameData();
	}

	private void UserControl_Unloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		// Level Order
		settings.Seed = LevelOrder_TextBox_Seed.Text;
		settings.RandomSeed = LevelOrder_CheckBox_Random_Seed.IsChecked.Value;
		settings.LevelOrderMode = (LevelOrderMode)LevelOrder_ComboBox_Mode.SelectedIndex;
		settings.LevelOrderMainPath = (LevelOrderMainPath)LevelOrder_ComboBox_MainPath.SelectedIndex;
		settings.LevelOrderMaxForwardsJump = (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value;
		settings.LevelOrderMaxBackwardsJump = (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value;
		settings.LevelOrderBackwardsJumpProbability = (int)LevelOrder_NumericUpDown_BackwardsJumpProbability.Value;
		settings.LevelOrderAllowJumpsToSameLevel = LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value;
		settings.LevelOrderIncludeLastStory = LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value;
		settings.LevelOrderIncludeBosses = LevelOrder_CheckBox_IncludeBosses.IsChecked.Value;

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

		settings.Save();
	}

	private async void LoadGameData()
	{
		var topLevel = TopLevel.GetTopLevel(this);
		var folderPath = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = "Select the root folder of an extracted Shadow the Hedgehog disc image.",
		});

		if (folderPath is not null && folderPath.Count > 0)
		{
			if (settings.GamePath != folderPath.First().Path.LocalPath && Directory.Exists("backup"))
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
			settings.GamePath = folderPath.First().Path.LocalPath;
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
			if (!Directory.Exists(Path.Combine("backup", "sets")))
			{
				Directory.CreateDirectory(Path.Combine("backup", "sets"));
				for (int stageIdToModify = 5; stageIdToModify < 45; stageIdToModify++)
				{
					stageAssociationIDMap.TryGetValue(stageIdToModify, out var stageId);
					var stageDataIdentifier = "stg0" + stageId.ToString();
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
			programInitialized = true;
		}
		else
		{
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
			{
				desktopApp.Shutdown();
			}
		}
	}

	private static int CalculateSeed(string seedString)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			return BitConverter.ToInt32(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(seedString)), 0);
		}
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
		int seed;
		if (LevelOrder_CheckBox_Random_Seed.IsChecked.Value)
		{
			var randomBytes = new byte[10];
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(randomBytes);
			}
			LevelOrder_TextBox_Seed.Text = Convert.ToBase64String(randomBytes);
		}
		if (LevelOrder_TextBox_Seed.Text == null || LevelOrder_TextBox_Seed.Text == "")
		{
			ShowErrorMessage("Error", "Invalid Seed", ButtonEnum.Ok, Icon.Error);
			ProgressBar_RandomizationProgress.Value = 0;
			return;
		}

		seed = CalculateSeed(LevelOrder_TextBox_Seed.Text);
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
					Shuffle(r, stageids, stagecount);
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
							int next = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
							stg.SetExit(0, next);
							if (!newset.Contains(next))
								newset.Add(next);
							if (stg.HasHero && stg.Hero == -1)
							{
								stg.Hero = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
								if (!newset.Contains(stg.Hero))
									newset.Add(stg.Hero);
							}
							if (stg.HasDark && stg.Dark == -1)
							{
								stg.Dark = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
								if (!newset.Contains(stg.Dark))
									newset.Add(stg.Dark);
							}
						}
						stagepool.RemoveAll(a => newset.Contains(a));
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
						int next = GetStageFromLists(r, orphans, usedstg, 2);
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
							next = orphans[r.Next(orphans.Count)];
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
							stg.Neutral = pool[r.Next(pool.Count)];
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

		Spoilers_ListBox_LevelList.Items.Clear();
		for (int i = 0; i < stagecount; i++)
			Spoilers_ListBox_LevelList.Items.Add(GetStageName(stageids[i]));
		Spoilers_ListBox_LevelList.IsEnabled = true;
		Spoilers_ListBox_LevelList.SelectedIndex = 0;
		Spoilers_Button_SaveLog.IsEnabled = true;
		Spoilers_Button_MakeChart.IsEnabled = true;
		ProgressBar_RandomizationProgress.Value = 100;
		var msgbox = MessageBoxManager.GetMessageBoxStandard("ShadowRando", "Randomization Complete", ButtonEnum.Ok, Icon.Info);
		var result = await msgbox.ShowAsync();
		if (result == ButtonResult.Ok)
		{
			ProgressBar_RandomizationProgress.Value = 0;
		}
	}

	private void CopyDirectory(DirectoryInfo srcDir, string dstDir)
	{
		Directory.CreateDirectory(dstDir);
		foreach (var dir in srcDir.EnumerateDirectories())
			CopyDirectory(dir, Path.Combine(dstDir, dir.Name));
		foreach (var fil in srcDir.EnumerateFiles())
			fil.CopyTo(Path.Combine(dstDir, fil.Name));
	}

	private void CopyDirectory(string srcDir, string dstDir) => CopyDirectory(new DirectoryInfo(srcDir), dstDir);

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
					int donotFNTEntryIndex = r.Next(0, fntRandomPool.Count - 1);
					if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value && fntRandomPool[donotFNTEntryIndex].audioId == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fntRandomPool[donotFNTEntryIndex].audioId);
					}
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fntRandomPool[donotFNTEntryIndex].subtitle);
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fntRandomPool[donotFNTEntryIndex].subtitleActiveTime);
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
					int donotFNTEntryIndex = r.Next(0, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryTableCount() - 1);
					if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value && fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donotFNTEntryIndex) == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donotFNTEntryIndex));
					}

					fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitle(donotFNTEntryIndex));
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitleActiveTime(donotFNTEntryIndex));
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
		};
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
			var stageDataIdentifier = "stg0" + stageId.ToString();
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

			List<EWeapon> weaponsPool = new List<EWeapon>();

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
					ShowErrorMessage("Error", "Must select at least one weapon.", ButtonEnum.Ok, Icon.Error);
					return 1;
				}
			}
			else
			{
				weaponsPool.AddRange(weapons);
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
				if (ds1LayoutData != null)
					RandomizeEnvironmentWeaponDrops(ref ds1LayoutData, weaponsPool, r);
			}


			if ((LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild)
			{
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
						ShowErrorMessage("Error", "Must have at least one ground and one flying enemy.", ButtonEnum.Ok, Icon.Error);
						return 1; // TODO do we want to throw errors?
					}
					if (groundEnemies.Count == 1)
					{
						// make sure there is at least one other enemy if GUN Soldiers are only picked
						if (groundEnemies[0] == typeof(Object0064_GUNSoldier))
						{
							ShowErrorMessage("Error", "GUN Soldiers have an issue with some Link IDs, add an extra ground enemy type.", ButtonEnum.Ok, Icon.Error);
							return 1;
						}
					}
				} else
				{
					if (allEnemies.Count == 1)
					{
						// make sure there is at least one other enemy if GUN Soldiers are only picked
						if (allEnemies[0] == typeof(Object0064_GUNSoldier))
						{
							ShowErrorMessage("Error", "GUN Soldiers have an issue with some Link IDs, add an extra enemy type.", ButtonEnum.Ok, Icon.Error);
							return 1;
						}
					}
				}
			}
			else
			{
				groundEnemies.AddRange(groundEnemyTypeMap);
				flyingEnemies.AddRange(flyingEnemyTypeMap);
				pathTypeFlyingEnemies.AddRange([typeof(Object0065_GUNBeetle), typeof(Object008E_BkWingLarge), typeof(Object008F_BkWingSmall)]);
				allEnemies.AddRange(enemyTypeMap);
			}

			switch (enemyMode)
			{
				case LayoutEnemyMode.Original:
					break;
				case LayoutEnemyMode.Wild:
					WildRandomizeAllEnemiesWithTranslations(ref cmnLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
					if (nrmLayoutData != null)
						WildRandomizeAllEnemiesWithTranslations(ref nrmLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
					if (hrdLayoutData != null)
						WildRandomizeAllEnemiesWithTranslations(ref hrdLayoutData, allEnemies, groundEnemies, flyingEnemies, pathTypeFlyingEnemies, r);
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

			if (Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value && nukkoro2EnemyCountStagesMap.TryGetValue(stageId, out var nukkoro2StageString))
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
					datOneDataContent.Files[0].CompressedData = Prs.Compress(ref updatedPATHPTP);
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
					(entry.objectEntry.Type >= 0x00 && entry.objectEntry.Type <= 0x0C) || // is a box/spring
					(entry.objectEntry.Type >= 0x64 && entry.objectEntry.Type <= 0x93) ||  // is an enemy
					(entry.objectEntry.Type >= 0xC8 && entry.objectEntry.Type <= 0xFD) || // is a weapon
					(entry.objectEntry.Type >= 0x46 && entry.objectEntry.Type <= 0x4E) // is a vehicle
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

	private async void ShowErrorMessage(string title, string message, ButtonEnum messageType, Icon messageIcon) {
		var error = MessageBoxManager.GetMessageBoxStandard(title, message, messageType, messageIcon);
		await error.ShowAsync();
	}

	private int GetTotalGUNEnemies(List<SetObjectShadow> cmn, List<SetObjectShadow> nrm = null)
	{
		int total = 0;

		for (int i = 0; i < cmn.Count(); i++)
		{
			if (cmn[i].List == 0x00 && (cmn[i].Type >= 0x64 && cmn[i].Type <= 0x68))
			{
				total++;
			}
		}

		if (nrm != null)
		{
			for (int i = 0; i < nrm.Count(); i++)
			{
				if (nrm[i].List == 0x00 && (nrm[i].Type >= 0x64 && nrm[i].Type <= 0x68))
				{
					total++;
				}
			}
		}
		return total;
	}

	private int GetTotalBlackArmsEnemies(List<SetObjectShadow> cmn, List<SetObjectShadow> nrm = null)
	{
		int total = 0;

		for (int i = 0; i < cmn.Count(); i++)
		{
			if (cmn[i].List == 0x00 && (cmn[i].Type >= 0x8C && cmn[i].Type <= 0x93))
			{
				if (cmn[i].Type == 0x91) // BkLarva
				{
					total += ((Object0091_BkLarva)cmn[i]).NumberOfLarva;
				}
				else
				{
					total++;
				}
			}
		}

		if (nrm != null)
		{
			for (int i = 0; i < nrm.Count(); i++)
			{
				if (nrm[i].List == 0x00 && (nrm[i].Type >= 0x8C && nrm[i].Type <= 0x93))
				{
					if (nrm[i].Type == 0x91) // BkLarva
					{
						total += ((Object0091_BkLarva)nrm[i]).NumberOfLarva;
					}
					else
					{
						total++;
					}
				}
			}
		}
		return total;
	}

	private void MakeAllPartnersRandom(ref List<SetObjectShadow> setData, bool keepOriginalObjectAffiliation, Random r)
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

	private EBoxType? GetWeaponAffiliationBoxType(EWeapon weapon)
	{
		switch (weapon)
		{
			case EWeapon.Pistol:
			case EWeapon.SubmachineGun:
			case EWeapon.MachineGun:
			case EWeapon.HeavyMachineGun:
			case EWeapon.GatlingGun:
			case EWeapon.GrenadeLauncher:
			case EWeapon.GUNBazooka:
			case EWeapon.TankCannon:
			case EWeapon.RPG:
			case EWeapon.FourShot:
			case EWeapon.EightShot:
			case EWeapon.LaserRifle:
			case EWeapon.Knife:
				return EBoxType.GUN;
			case EWeapon.LightShot:
			case EWeapon.FlashShot:
			case EWeapon.RingShot:
			case EWeapon.HeavyShot:
			case EWeapon.BlackBarrel:
			case EWeapon.BigBarrel:
			case EWeapon.WormShooterBlack:
			case EWeapon.WideWormShooterRed:
			case EWeapon.BigWormShooterGold:
			case EWeapon.VacuumPod:
			case EWeapon.Splitter:
			case EWeapon.Refractor:
			case EWeapon.BlackSword:
			case EWeapon.DarkHammer:
				return EBoxType.BlackArms;
			case EWeapon.EggGun:
			case EWeapon.EggBazooka:
			case EWeapon.EggLance:
				return EBoxType.Eggman;
			default:
				return null;
		}
	}

	private void MakeAllWeaponBoxesHaveRandomWeapons(ref List<SetObjectShadow> setData, List<EWeapon> weaponsPool, Random r)
	{
		List<(Object000C_WeaponBox item, int index)> weaponBoxItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object000C_WeaponBox)
			.Select(pair => (Item: (Object000C_WeaponBox)pair.Item, Index: pair.Index))
			.ToList();

		foreach (var weaponbox in weaponBoxItems)
		{
			weaponbox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = GetWeaponAffiliationBoxType(weaponbox.item.Weapon);
			if (boxType.HasValue)
				weaponbox.item.BoxType = boxType.Value;
			setData[weaponbox.index] = weaponbox.item;
		}
	}

	private void MakeAllBoxesHaveRandomWeapons(ref List<SetObjectShadow> setData, List<EWeapon> weaponsPool, Random r)
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

		/*			List<(Object003A_SpecialWeaponBox item, int index)> specialWeaponsBoxItems = setData // Only do this when we can override spawning spw to use single default setdata weapon
						.Select((item, index) => new { Item = item, Index = index })
						.Where(pair => pair.Item is Object003A_SpecialWeaponBox)
						.Select(pair => (Item: (Object003A_SpecialWeaponBox)pair.Item, Index: pair.Index))
						.ToList(); */

		foreach (var woodbox in woodBoxItems)
		{
			woodbox.item.BoxItem = EBoxItem.Weapon;
			woodbox.item.ModifierWeapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = GetWeaponAffiliationBoxType(woodbox.item.ModifierWeapon);
			if (boxType.HasValue)
				woodbox.item.BoxType = boxType.Value;
			setData[woodbox.index] = woodbox.item;
		}

		foreach (var weaponbox in weaponBoxItems)
		{
			weaponbox.item.Weapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = GetWeaponAffiliationBoxType(weaponbox.item.Weapon);
			if (boxType.HasValue)
				weaponbox.item.BoxType = boxType.Value;
			setData[weaponbox.index] = weaponbox.item;
		}

		foreach (var metalbox in metalBoxItems)
		{
			metalbox.item.BoxItem = EBoxItem.Weapon;
			metalbox.item.ModifierWeapon = weaponsPool[r.Next(weaponsPool.Count)];
			var boxType = GetWeaponAffiliationBoxType(metalbox.item.ModifierWeapon);
			if (boxType.HasValue)
				metalbox.item.BoxType = boxType.Value;
			setData[metalbox.index] = metalbox.item;
		}

		/*			foreach (var specialWeaponsBox in specialWeaponsBoxItems) // Only do this when we can override spawning spw to use single default setdata weapon
					{
						specialWeaponsBox.item.Weapon = weapons[r.Next(weapons.Length)];
						setData[specialWeaponsBox.index] = specialWeaponsBox.item;
					}*/
	}

	private void RandomizeWeaponsOnGround(ref List<SetObjectShadow> setData, List<EWeapon> weaponsPool, Random r)
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

	private void RandomizeEnvironmentWeaponDrops(ref List<SetObjectShadow> setData, List<EWeapon> weaponsPool, Random r)
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

	private void WildRandomizeAllEnemiesWithTranslations(ref List<SetObjectShadow> setData, List<Type> allEnemies, List<Type> groundEnemies, List<Type> flyingEnemies, List<Type> pathTypeFlyingEnemies, Random r)
	{
		// Wild Randomize of all Enemies
		for (int i = 0; i < setData.Count(); i++)
		{
			if (setData[i].List == 0x00 && (setData[i].Type >= 0x64 && setData[i].Type <= 0x93))
			{
				Type randomEnemyType = typeof(Nullable);
				if (Layout_Enemy_CheckBox_KeepType.IsChecked.Value)
				{
					if (IsFlyingEnemy(setData[i]))
					{
						if (setData[i].List == 0x00 && setData[i].Type == 0x90)
						{
							// if BkWorm, mutate original posY +50
							setData[i].PosY = setData[i].PosY + 50;
						}
						// if path type enemy
						if (IsPathTypeFlyingEnemy(setData[i]))
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
								EnemySETMutations.MutateObjectAtIndex(i, donor, ref setData, true, r);
								continue; // skip the MutateObject below since we handled it ourselves
							}
							else if (randomEnemyType == typeof(Object0093_BkNinja))
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
								EnemySETMutations.MutateObjectAtIndex(i, donor, ref setData, true, r);
								continue; // skip the MutateObject below since we handled it ourselves
							}
						}
					}
					else
					{ // ground enemies
						int randomEnemy = -1;
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
					int randomEnemy = -1;
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
				EnemySETMutations.MutateObjectAtIndex(i, randomEnemyType, ref setData, true, r);
			}
		}
	}

	private bool IsFlyingEnemy(SetObjectShadow enemy)
	{
		switch (enemy.Type)
		{
			case 0x65: // GUNBeetle
			case 0x8E: // BkWingLarge
			case 0x8F: // BkWingSmall
			case 0x92: // BkChaos
				return true;
			case 0x66: // GUNBigfoot
				if (((Object0066_GUNBigfoot)enemy).AppearType == Object0066_GUNBigfoot.EAppear.ZUTTO_HOVERING)
				{
					return true;
				}
				break;
			case 0x90: // BkWorm
				if (enemy.UnkBytes[2] == 0x40 && enemy.UnkBytes[6] == 0x40) // BkWorms that spawn on killplanes
				{
					return true;
				}
				break;
			case 0x93: // BkNinja
				if (((Object0093_BkNinja)enemy).AppearType == Object0093_BkNinja.EAppear.ON_AIR_SAUCER_WARP)
				{
					return true;
				}
				break;
			default:
				return false;
		}
		return false;
	}

	private bool IsPathTypeFlyingEnemy(SetObjectShadow enemy)
	{
		switch (enemy.Type)
		{
			case 0x65: // GUNBeetle
			case 0x8E: // BkWingLarge
			case 0x8F: // BkWingSmall
				return true;
			default:
				return false;
		}
	}

	private void MakeAllEnemiesGUNSoldiers(ref List<SetObjectShadow> setData, Random r)
	{
		var soldier = new Object0064_GUNSoldier();
		soldier.List = 0x00;
		soldier.Type = 0x64;

		// make all enemies a gun soldier
		for (int i = 0; i < setData.Count(); i++)
		{
			if (setData[i].List == 0x00 &&
					(
						(setData[i].Type >= 0x64 && setData[i].Type <= 0x93)
					)
				)
			{
				if (setData[i].Link == 0 || setData[i].Link == 50) // Skip enemies with LinkID to prevent softlock
					CloneObjectOverIndex(i, soldier, ref setData, true, r);
			}
		}
	}

	private void MakeAllEnemiesGUNSoldiersWithTranslations(ref List<SetObjectShadow> setData, Random r)
	{
		// make all enemies a gun soldier
		for (int i = 0; i < setData.Count(); i++)
		{
			if (setData[i].List == 0x00 &&
					(
						(setData[i].Type >= 0x64 && setData[i].Type <= 0x93)
					)
				)
			{
				if (setData[i].Link == 0 || setData[i].Link == 50) // Skip enemies with LinkID to prevent softlock
					EnemySETMutations.MutateObjectAtIndex(i, typeof(Object0064_GUNSoldier), ref setData, true, r);
			}
		}
	}

	private void MakeAllObjectsGUNSoldiers(ref List<SetObjectShadow> setData, Random r)
	{
		var soldier = new Object0064_GUNSoldier();
		soldier.List = 0x00;
		soldier.Type = 0x64;

		// make all objects a gun soldier
		for (int i = 0; i < setData.Count(); i++)
		{
			// skip core objs
			if (setData[i].List == 0x00 &&
					(
						(setData[i].Type >= 0x00 && setData[i].Type <= 0x07) ||
						(setData[i].Type == 0x14) || // goal ring
						(setData[i].Type == 0x3A) || // shadow box
						(setData[i].Type == 0x4F) || // vehicles
						(setData[i].Type == 0x61) || // dark spin entrance
						(setData[i].Type >= 0xB4 && setData[i].Type <= 0xBE) // bosses
					)
				|| setData[i].List == 0x14) // gravity related
			{
				continue;
			}
			CloneObjectOverIndex(i, soldier, ref setData, true, r);
		}
	}

	// TODO move this to ShadowSET library?
	private void CloneObjectOverIndex(int index, Object0064_GUNSoldier cloneObject, ref List<SetObjectShadow> setData, bool isShadow, Random r)
	{
		// isShadow ?
		var oldEntry = setData[index];
		// may need to make clone entry unkbytes instead , but for now leaving
		var newEntry = LayoutEditorFunctions.CreateShadowObject(cloneObject.List, cloneObject.Type, oldEntry.PosX, oldEntry.PosY,
			oldEntry.PosZ, oldEntry.RotX, oldEntry.RotY, oldEntry.RotZ, oldEntry.Link, oldEntry.Rend, oldEntry.UnkBytes); // :
		var modifier = (Object0064_GUNSoldier)newEntry;                                                                                                                                    //LayoutEditorFunctions.CreateHeroesObject(newEntry.List, newEntry.Type, pos, rot, link, rend, unkb);

		modifier.WeaponType = (Object0064_GUNSoldier.EWeapon)r.Next(0x7);
		// 10% chance of shield
		var hasShield = r.Next(10) == 1;
		modifier.HaveShield = hasShield ? (ENoYes)1 : (ENoYes)0;
		modifier.SearchRange = 500;
		modifier.SearchWidth = 500;
		modifier.SearchHeight = 200;
		modifier.MoveRange = 2000;

		setData[index] = modifier;
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

	int[] FindShortestPath(int start)
	{
		Stack<int> stack = new Stack<int>(stagecount);
		stack.Push(start);
		return FindShortestPath(stages[start], stack, null);
	}

	int[] FindShortestPath(Stage stage, Stack<int> path, int[] shortestPath)
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

    private void Spoilers_Button_MakeChart_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
		// TODO: Implement cross platform MakeChart OR make it Windows exclusive
	}

    private async void Spoilers_Button_SaveLog_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
		var topLevel = TopLevel.GetTopLevel(this);
		var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
		{
			Title = "Save log",
			DefaultExtension = ".txt",
			FileTypeChoices = [FilePickerFileTypes.TextPlain]
		});

		if (file is not null)
		{
			// Open writing stream from the file.
			await using var stream = await file.OpenWriteAsync();
			using var sw = new StreamWriter(stream);
			// Write some content to the file.
			sw.WriteLine($"ShadowRando Version: {programVersion}");
			sw.WriteLine($"Seed: {LevelOrder_TextBox_Seed.Text}");
			sw.WriteLine($"Level Order Mode: {settings.LevelOrderMode}");
			if (settings.LevelOrderMode == LevelOrderMode.AllStagesWarps)
			{
				sw.WriteLine($"Main Path: {settings.LevelOrderMainPath}");
				sw.WriteLine($"Max Forwards Jump: {LevelOrder_NumericUpDown_MaxForwardsJump.Value}");
				sw.WriteLine($"Max Backwards Jump: {LevelOrder_NumericUpDown_MaxBackwardsJump.Value}");
				sw.WriteLine($"Backwards Jump Probability: {LevelOrder_NumericUpDown_BackwardsJumpProbability.Value}");
				sw.WriteLine($"Allow Jumps To Same Level: {LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value}");
			}
			sw.WriteLine($"Include Last Story: {LevelOrder_CheckBox_IncludeLastStory.IsChecked.Value}");
			sw.WriteLine($"Include Bosses: {LevelOrder_CheckBox_IncludeBosses.IsChecked.Value}");

			sw.WriteLine($"Randomize Layouts: {Layout_CheckBox_RandomizeLayouts.IsChecked.Value}");
			if (settings.RandomizeLayouts == true)
			{
				sw.WriteLine($"Enemy Mode: {settings.LayoutEnemyMode}");
				sw.WriteLine($"Random Weapons In All Boxes: {Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value}");
				sw.WriteLine($"Random Weapons In Weapon Boxes: {Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value}");
				sw.WriteLine($"Random Exposed Weapons: {Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked.Value}");
				sw.WriteLine($"Environment Drops Random Weapons: {Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked.Value}");
				sw.WriteLine($"Partner Mode: {settings.LayoutPartnerMode}");
			}
			sw.WriteLine($"Randomize Subtitles / Voicelines: {Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value}");
			sw.WriteLine($"Randomize Music: {Music_CheckBox_RandomizeMusic.IsChecked.Value}");
			sw.WriteLine($"Skip Chaos Power Use Jingles: {Music_CheckBox_SkipChaosPowerUseJingles.IsChecked.Value}");
			sw.WriteLine($"Skip Rank Theme: {Music_CheckBox_SkipRankTheme.IsChecked.Value}");
			sw.WriteLine();
			for (int i = 0; i < stagecount; i++)
			{
				Stage stg = stages[stageids[i]];
				sw.WriteLine($"{GetStageName(stageids[i])}:");
				if (stg.Neutral != -1)
					sw.WriteLine($"Neutral -> {GetStageName(stg.Neutral)} ({Array.IndexOf(stageids, stg.Neutral) - i:+##;-##;0})");
				if (stg.Hero != -1)
					sw.WriteLine($"Hero -> {GetStageName(stg.Hero)} ({Array.IndexOf(stageids, stg.Hero) - i:+##;-##;0})");
				if (stg.Dark != -1)
					sw.WriteLine($"Dark -> {GetStageName(stg.Dark)} ({Array.IndexOf(stageids, stg.Dark) - i:+##;-##;0})");
				sw.WriteLine();
			}
		}

	}

	private void Spoilers_ListBox_LevelList_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
		if (Spoilers_ListBox_LevelList.SelectedIndex != -1)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
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
					sb.AppendFormat("{0}Shortest Path: ", Environment.NewLine);
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
	}
}
