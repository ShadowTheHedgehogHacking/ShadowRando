using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AFSLib;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HeroesONE_R.Structures;
using HeroesONE_R.Structures.Common;
using HeroesONE_R.Utilities;
using MsBox.Avalonia.Enums;
using ShadowFNT;
using ShadowFNT.Structures;
using ShadowRando.Core;
using ShadowRando.Core.SETMutations;
using ShadowSET;
using ShadowSET.SETIDBIN;
using SkiaSharp;
using TableEntry = ShadowFNT.Structures.TableEntry;

namespace ShadowRando.Views;

public partial class MainView : UserControl
{
	const int stagefirst = 5;

	private byte[] dolfile;

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

	private CheckBox[] LevelCheckBoxes, EnemyCheckBoxes, WeaponCheckBoxes, PartnerCheckBoxes, SubtitleCheckBoxes;

	private const int cutsceneEventDisablerOffset = 0x256D20;
	private const int cutsceneEventDisablerPatchValue = 0x60000000;
	private const int routeMenu6xxStagePreviewBlockerOffset = 0xB48B8;
	private const int routeMenu6xxStagePreviewPatchValue = 0x48000110;
	private const int storyModeStartAddress = 0x2CB9F0;
	private const int firstStageOffset = 0x4C1BA8;
	private const int modeOffset = 8;
	private const int darkOffset = 0x1C;
	private const int neutOffset = 0x28;
	private const int heroOffset = 0x34;
	private const int stageOffset = 0x50;
	private const int shadowBoxPatchOffset = 0x3382E0;
	private const int shadowBoxPatchValue = 0x480001B0;
	private const int partnerAffiliationDarkPatchValue = 0x38E00000;
	private const int partnerAffiliationHeroPatchValue = 0x38E00002;
	private const int partnerAffiliationEggmanStaticAssociationPatchOffset = 0x20A048;
	private const int partnerAffiliationEggmanStaticAssociationPatchValue = 0x48000010;
	private const int partnerAffiliationSonicPatchOffset = 0x209820;
	private const int partnerAffiliationTailsPatchOffset = 0x20C368;
	private const int partnerAffiliationKnucklesPatchOffset = 0x209040;
	private const int partnerAffiliationAmyPatchOffset = 0x2136E0;
	private const int partnerAffiliationRougePatchOffset = 0x211228;
	private const int partnerAffiliationOmegaPatchOffset = 0x20D800;
	private const int partnerAffiliationVectorPatchOffset = 0x20FE90;
	private const int partnerAffiliationEspioPatchOffset = 0x20EAF8;
	private const int partnerAffiliationMariaPatchOffset = 0x20B000;
	private const int partnerAffiliationCharmyPatchOffset = 0x212520;
	private const int partnerAffiliationEggmanPatchOffset = 0x20A008;
	private const int partnerAffiliationDoomsEyePatchOffset = 0x20A81C;

	private const int expertModeExtendedLevelSlotsPatchOffset1 = 0x34E968;
	private const int expertModeExtendedLevelSlotsPatchOffset2 = 0x34E528;
	private const int expertModeExtendedLevelSlotsPatchValue = 0x38631934;
	private const int expertModeExtendedLevelSlotsEndCheckPatchOffset1 = 0x34E91C;
	private const int expertModeExtendedLevelSlotsEndCheckPatchOffset2 = 0x34E4E8;
	private const int expertModeExtendedLevelSlotsEndCheckPatchValue = 0x28000000; // add stage total before patching
	private const int expertModeLevelsOffset = 0x55E934;
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

	private string selectedFolderPath;
	private bool avaloniaPreviewUI;
	const string programVersion = "0.5.0-RC4";
	private bool programInitialized = false;
	private bool randomizeProcessing = false;

	Settings settings;
	
	public MainView()
	{
		avaloniaPreviewUI = true;
		InitializeComponent();
	}

	public MainView(string folderPath, Settings settings)
	{
		selectedFolderPath = folderPath;
		this.settings = settings;
		InitializeComponent();
		MainTabParent.IsVisible = false;
		BottomBarGrid.IsVisible = false;
		LoadingDataIndicator.IsVisible = true;
	}

	private void UserControl_Loaded(object? sender, RoutedEventArgs e)
	{
		if (avaloniaPreviewUI) return;
		var topLevel = TopLevel.GetTopLevel(this);
		if (OperatingSystem.IsBrowser()) return; // TODO: Browser context only is allowed to read/write file dialogs if a user triggers the context, need to add buttons for browser to target
		if (topLevel is null) return;

		LevelCheckBoxes = [
			Levels_CheckBox_Westopolis,
			Levels_CheckBox_DigitalCircuit,
			Levels_CheckBox_GlyphicCanyon,
			Levels_CheckBox_LethalHighway,
			Levels_CheckBox_CrypticCastle,
			Levels_CheckBox_PrisonIsland,
			Levels_CheckBox_CircusPark,
			Levels_CheckBox_CentralCity,
			Levels_CheckBox_TheDoom,
			Levels_CheckBox_SkyTroops,
			Levels_CheckBox_MadMatrix,
			Levels_CheckBox_DeathRuins,
			Levels_CheckBox_TheARK,
			Levels_CheckBox_AirFleet,
			Levels_CheckBox_IronJungle,
			Levels_CheckBox_SpaceGadget,
			Levels_CheckBox_LostImpact,
			Levels_CheckBox_GUNFortress,
			Levels_CheckBox_BlackComet,
			Levels_CheckBox_LavaShelter,
			Levels_CheckBox_CosmicFall,
			Levels_CheckBox_FinalHaunt,
			Levels_CheckBox_TheLastWay,
			Levels_CheckBox_BlackBullLH,
			Levels_CheckBox_EggBreakerCC,
			Levels_CheckBox_HeavyDog,
			Levels_CheckBox_EggBreakerMM,
			Levels_CheckBox_BlackBullDR,
			Levels_CheckBox_BlueFalcon,
			Levels_CheckBox_EggBreakerIJ,
			Levels_CheckBox_BlackDoomGF,
			Levels_CheckBox_SonicDiablonGF,
			Levels_CheckBox_EggDealerBC,
			Levels_CheckBox_SonicDiablonBC,
			Levels_CheckBox_EggDealerLS,
			Levels_CheckBox_EggDealerCF,
			Levels_CheckBox_BlackDoomCF,
			Levels_CheckBox_BlackDoomFH,
			Levels_CheckBox_SonicDiablonFH,
			Levels_CheckBox_DevilDoom
		];
		
		EnemyCheckBoxes = [
			Layout_Enemy_CheckBox_SelectedEnemy_GUNSoldier,
			Layout_Enemy_CheckBox_SelectedEnemy_GUNBeetle,
			Layout_Enemy_CheckBox_SelectedEnemy_GUNBigfoot,
			Layout_Enemy_CheckBox_SelectedEnemy_GUNRobot,
			Layout_Enemy_CheckBox_SelectedEnemy_EggPierrot,
			Layout_Enemy_CheckBox_SelectedEnemy_EggPawn,
			Layout_Enemy_CheckBox_SelectedEnemy_ShadowAndroid,
			Layout_Enemy_CheckBox_SelectedEnemy_BAGiant,
			Layout_Enemy_CheckBox_SelectedEnemy_BASoldier,
			Layout_Enemy_CheckBox_SelectedEnemy_BAHawkVolt,
			Layout_Enemy_CheckBox_SelectedEnemy_BAWing,
			Layout_Enemy_CheckBox_SelectedEnemy_BAWorm,
			Layout_Enemy_CheckBox_SelectedEnemy_BALarva,
			Layout_Enemy_CheckBox_SelectedEnemy_ArtificialChaos,
			Layout_Enemy_CheckBox_SelectedEnemy_BAAssassin
		];

		WeaponCheckBoxes = [
			Layout_Weapon_CheckBox_SelectedWeapon_None,
			Layout_Weapon_CheckBox_SelectedWeapon_Pistol,
			Layout_Weapon_CheckBox_SelectedWeapon_SubmachineGun,
			Layout_Weapon_CheckBox_SelectedWeapon_AssaultRifle,
			Layout_Weapon_CheckBox_SelectedWeapon_HeavyMachineGun,
			Layout_Weapon_CheckBox_SelectedWeapon_GatlingGun,
			null,
			Layout_Weapon_CheckBox_SelectedWeapon_EggPistol,
			Layout_Weapon_CheckBox_SelectedWeapon_LightShot,
			Layout_Weapon_CheckBox_SelectedWeapon_FlashShot,
			Layout_Weapon_CheckBox_SelectedWeapon_RingShot,
			Layout_Weapon_CheckBox_SelectedWeapon_HeavyShot,
			Layout_Weapon_CheckBox_SelectedWeapon_GrenadeLauncher,
			Layout_Weapon_CheckBox_SelectedWeapon_GUNBazooka,
			Layout_Weapon_CheckBox_SelectedWeapon_TankCannon,
			Layout_Weapon_CheckBox_SelectedWeapon_BlackBarrel,
			Layout_Weapon_CheckBox_SelectedWeapon_BigBarrel,
			Layout_Weapon_CheckBox_SelectedWeapon_EggBazooka,
			Layout_Weapon_CheckBox_SelectedWeapon_RPG,
			Layout_Weapon_CheckBox_SelectedWeapon_FourShot,
			Layout_Weapon_CheckBox_SelectedWeapon_EightShot,
			Layout_Weapon_CheckBox_SelectedWeapon_WormShooterBlack,
			Layout_Weapon_CheckBox_SelectedWeapon_WormShooterRed,
			Layout_Weapon_CheckBox_SelectedWeapon_WormShooterGold,
			Layout_Weapon_CheckBox_SelectedWeapon_VacuumPod,
			Layout_Weapon_CheckBox_SelectedWeapon_LaserRifle,
			Layout_Weapon_CheckBox_SelectedWeapon_Splitter,
			Layout_Weapon_CheckBox_SelectedWeapon_Refractor,
			null,
			null,
			Layout_Weapon_CheckBox_SelectedWeapon_Knife,
			Layout_Weapon_CheckBox_SelectedWeapon_BlackSword,
			Layout_Weapon_CheckBox_SelectedWeapon_DarkHammer,
			Layout_Weapon_CheckBox_SelectedWeapon_EggLance,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv1,
			Layout_Weapon_CheckBox_SelectedWeapon_SamuraiSwordLv2,
			Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv1,
			Layout_Weapon_CheckBox_SelectedWeapon_SatelliteLaserLv2,
			Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv1,
			Layout_Weapon_CheckBox_SelectedWeapon_EggVacuumLv2,
			Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv1,
			Layout_Weapon_CheckBox_SelectedWeapon_OmochaoGunLv2,
			Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv1,
			Layout_Weapon_CheckBox_SelectedWeapon_HealCannonLv2,
			Layout_Weapon_CheckBox_SelectedWeapon_ShadowRifle
		];

		PartnerCheckBoxes = [
			Layout_Partner_CheckBox_SelectedPartner_Sonic,
			Layout_Partner_CheckBox_SelectedPartner_Tails,
			Layout_Partner_CheckBox_SelectedPartner_Knuckles,
			Layout_Partner_CheckBox_SelectedPartner_Amy,
			Layout_Partner_CheckBox_SelectedPartner_Rouge,
			Layout_Partner_CheckBox_SelectedPartner_Omega,
			Layout_Partner_CheckBox_SelectedPartner_Vector,
			Layout_Partner_CheckBox_SelectedPartner_Espio,
			Layout_Partner_CheckBox_SelectedPartner_Maria,
			Layout_Partner_CheckBox_SelectedPartner_Charmy,
			Layout_Partner_CheckBox_SelectedPartner_EggMonitor,
			Layout_Partner_CheckBox_SelectedPartner_DoomsEye
		];

		SubtitleCheckBoxes = [
			Subtitles_CheckBox_SelectedCharacter_Shadow,
			Subtitles_CheckBox_SelectedCharacter_Sonic,
			Subtitles_CheckBox_SelectedCharacter_Tails,
			Subtitles_CheckBox_SelectedCharacter_Knuckles,
			Subtitles_CheckBox_SelectedCharacter_Amy,
			Subtitles_CheckBox_SelectedCharacter_Rouge,
			Subtitles_CheckBox_SelectedCharacter_Omega,
			Subtitles_CheckBox_SelectedCharacter_Vector,
			Subtitles_CheckBox_SelectedCharacter_Espio,
			Subtitles_CheckBox_SelectedCharacter_Maria,
			Subtitles_CheckBox_SelectedCharacter_Charmy,
			Subtitles_CheckBox_SelectedCharacter_Eggman,
			Subtitles_CheckBox_SelectedCharacter_BlackDoom,
			Subtitles_CheckBox_SelectedCharacter_Cream,
			Subtitles_CheckBox_SelectedCharacter_Cheese,
			Subtitles_CheckBox_SelectedCharacter_GUNCommander,
			Subtitles_CheckBox_SelectedCharacter_GUNSoldier
		];

		// Program Configuration
		LevelOrder_Label_ProgramTitle.Content += " " + programVersion;

		// Level Order
		LevelOrder_TextBox_Seed.Text = settings.Seed;
		LevelOrder_CheckBox_Random_Seed.IsChecked = settings.RandomSeed;
		LevelOrder_ComboBox_Mode.SelectedIndex = (int)settings.LevelOrder.Mode;
		LevelOrder_ComboBox_MainPath.SelectedIndex = (int)settings.LevelOrder.MainPath;
		LevelOrder_NumericUpDown_MaxForwardsJump.Value = settings.LevelOrder.MaxForwardsJump;
		LevelOrder_NumericUpDown_MaxBackwardsJump.Value = settings.LevelOrder.MaxBackwardsJump;
		LevelOrder_NumericUpDown_BackwardsJumpProbability.Value = settings.LevelOrder.BackwardsJumpProbability;
		LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked = settings.LevelOrder.AllowJumpsToSameLevel;
		LevelOrder_CheckBox_AllowBossToBoss.IsChecked = settings.LevelOrder.AllowBossToBoss;
		LevelOrder_CheckBox_ExpertRando.IsChecked = settings.LevelOrder.ExpertMode;

		foreach (var lev in settings.LevelOrder.ExcludeLevels)
			LevelCheckBoxes[(int)lev].IsChecked = true;

		// Layout
		Layout_CheckBox_RandomizeLayouts.IsChecked = settings.Layout.Randomize;
		Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked = settings.Layout.MakeCCSplinesVehicleCompatible;
		// Enemy
		Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked = settings.Layout.Enemy.AdjustMissionCounts;
		Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value = settings.Layout.Enemy.AdjustMissionCountsReductionPercent;
		Layout_Enemy_ComboBox_Mode.SelectedIndex = (int)settings.Layout.Enemy.Mode;
		Layout_Enemy_CheckBox_KeepType.IsChecked = settings.Layout.Enemy.KeepType;
		Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked = settings.Layout.Enemy.OnlySelectedTypes;
		
		foreach (var nme in settings.Layout.Enemy.SelectedEnemies)
			EnemyCheckBoxes[(int)nme].IsChecked = true;

		// Weapon
		Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked = settings.Layout.Weapon.RandomWeaponsInAllBoxes;
		Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked = settings.Layout.Weapon.RandomWeaponsInWeaponBoxes;
		Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked = settings.Layout.Weapon.RandomExposedWeapons;
		Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked = settings.Layout.Weapon.RandomWeaponsFromEnvironment;
		Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked = settings.Layout.Weapon.OnlySelectedTypes;

		foreach (var wep in settings.Layout.Weapon.SelectedWeapons)
			WeaponCheckBoxes[(int)wep].IsChecked = true;
		
		// Partner
		Layout_Partner_ComboBox_Mode.SelectedIndex = (int)settings.Layout.Partner.Mode;
		Layout_Partner_CheckBox_RandomizeAffiliations.IsChecked = settings.Layout.Partner.RandomizeAffiliations;
		Layout_Partner_CheckBox_KeepAffiliationsAtSameLocation.IsChecked = settings.Layout.Partner.KeepAffiliationsAtSameLocation;
		Layout_Partner_CheckBox_OnlySelectedPartners.IsChecked = settings.Layout.Partner.OnlySelectedPartners;
		
		foreach (var par in settings.Layout.Partner.SelectedPartners)
			PartnerCheckBoxes[(int)par - 1].IsChecked = true;

		// Subtitles
		Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked = settings.Subtitles.Randomize;
		Subtitles_CheckBox_NoDuplicates.IsChecked = settings.Subtitles.NoDuplicates;
		Subtitles_CheckBox_NoSystemMessages.IsChecked = settings.Subtitles.NoSystemMessages;
		Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked = settings.Subtitles.OnlyLinkedAudio;
		Subtitles_CheckBox_OnlySelectedCharacters.IsChecked = settings.Subtitles.OnlySelectedCharacters;
		Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked = settings.Subtitles.GiveAudioToNoLinkedAudio;
		Subtitles_CheckBox_GenerateMessages.IsChecked = settings.Subtitles.GenerateMessages;
		Subtitles_NumericUpDown_MarkovLevel.Value = settings.Subtitles.MarkovLevel;

		foreach (var chr in settings.Subtitles.SelectedCharacters)
			SubtitleCheckBoxes[(int)chr].IsChecked = true;

		// Music
		Music_CheckBox_RandomizeMusic.IsChecked = settings.Music.Randomize;
		Music_CheckBox_SkipChaosPowerUseJingles.IsChecked = settings.Music.SkipChaosPowers;
		Music_CheckBox_SkipRankTheme.IsChecked = settings.Music.SkipRankTheme;

		// Models
		Models_CheckBox_RandomizeModel.IsChecked = settings.Models.Randomize;
		Models_CheckBox_ModelP2.IsChecked = settings.Models.RandomizeP2;

		// Spoilers
		Spoilers_CheckBox_UseIcons.IsChecked = settings.Spoilers.GraphUseIcons;
		Spoilers_CheckBox_AutosaveLog.IsChecked = settings.Spoilers.AutosaveLog;

		LoadGameData();
		programInitialized = true;
		UpdateUIEnabledState();
		MainTabParent.IsVisible = true;
		BottomBarGrid.IsVisible = true;
		LoadingDataIndicator.IsVisible = false;
	}

	private void UserControl_Unloaded(object? sender, RoutedEventArgs e)
	{
		UpdateSettings();
	}

	private void UpdateSettings()
	{
		// Level Order
		settings.Seed = LevelOrder_TextBox_Seed.Text ?? string.Empty;
		settings.RandomSeed = LevelOrder_CheckBox_Random_Seed.IsChecked.Value;
		settings.LevelOrder.Mode = (LevelOrderMode)LevelOrder_ComboBox_Mode.SelectedIndex;
		settings.LevelOrder.MainPath = (LevelOrderMainPath)LevelOrder_ComboBox_MainPath.SelectedIndex;
		settings.LevelOrder.MaxForwardsJump = (int)LevelOrder_NumericUpDown_MaxForwardsJump.Value;
		settings.LevelOrder.MaxBackwardsJump = (int)LevelOrder_NumericUpDown_MaxBackwardsJump.Value;
		settings.LevelOrder.BackwardsJumpProbability = (int)LevelOrder_NumericUpDown_BackwardsJumpProbability.Value;
		settings.LevelOrder.AllowJumpsToSameLevel = LevelOrder_CheckBox_AllowJumpsToSameLevel.IsChecked.Value;
		settings.LevelOrder.AllowBossToBoss = LevelOrder_CheckBox_AllowBossToBoss.IsChecked.Value;
		settings.LevelOrder.ExpertMode = LevelOrder_CheckBox_ExpertRando.IsChecked.Value;

		settings.LevelOrder.ExcludeLevels = [];
		for (int i = 0; i < LevelCheckBoxes.Length; i++)
			if (LevelCheckBoxes[i].IsChecked == true)
				settings.LevelOrder.ExcludeLevels.Add((Levels)i);

		// Layout
		settings.Layout.Randomize = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		settings.Layout.MakeCCSplinesVehicleCompatible = Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsChecked.Value;
		// Enemy
		settings.Layout.Enemy.AdjustMissionCounts = Layout_Enemy_CheckBox_AdjustMissionCounts.IsChecked.Value;
		settings.Layout.Enemy.AdjustMissionCountsReductionPercent = (int)Layout_Enemy_NumericUpDown_AdjustMissionsReductionPercent.Value;
		settings.Layout.Enemy.Mode = (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex;
		settings.Layout.Enemy.KeepType = Layout_Enemy_CheckBox_KeepType.IsChecked.Value;
		settings.Layout.Enemy.OnlySelectedTypes = Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value;

		settings.Layout.Enemy.SelectedEnemies = [];
		for (int i = 0; i < EnemyCheckBoxes.Length; i++)
			if (EnemyCheckBoxes[i].IsChecked == true)
				settings.Layout.Enemy.SelectedEnemies.Add((EnemyTypes)i);

		// Weapon
		settings.Layout.Weapon.RandomWeaponsInAllBoxes = Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value;
		settings.Layout.Weapon.RandomWeaponsInWeaponBoxes = Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value;
		settings.Layout.Weapon.RandomExposedWeapons = Layout_Weapon_CheckBox_RandomExposedWeapons.IsChecked.Value;
		settings.Layout.Weapon.RandomWeaponsFromEnvironment = Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsChecked.Value;
		settings.Layout.Weapon.OnlySelectedTypes = Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value;

		settings.Layout.Weapon.SelectedWeapons = [];
		for (int i = 0; i < WeaponCheckBoxes.Length; i++)
			if (WeaponCheckBoxes[i]?.IsChecked == true)
				settings.Layout.Weapon.SelectedWeapons.Add((EWeapon)i);

		// Partner
		settings.Layout.Partner.Mode = (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex;
		settings.Layout.Partner.RandomizeAffiliations = Layout_Partner_CheckBox_RandomizeAffiliations.IsChecked.Value;
		settings.Layout.Partner.KeepAffiliationsAtSameLocation = Layout_Partner_CheckBox_KeepAffiliationsAtSameLocation.IsChecked.Value;
		settings.Layout.Partner.OnlySelectedPartners = Layout_Partner_CheckBox_OnlySelectedPartners.IsChecked.Value;

		settings.Layout.Partner.SelectedPartners = [];
		for (int i = 0; i < PartnerCheckBoxes.Length; i++)
			if (PartnerCheckBoxes[i].IsChecked == true)
				settings.Layout.Partner.SelectedPartners.Add((Object0190_Partner.EPartner)(i + 1));

		// Subtitles
		settings.Subtitles.Randomize = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		settings.Subtitles.NoDuplicates = Subtitles_CheckBox_NoDuplicates.IsChecked.Value;
		settings.Subtitles.NoSystemMessages = Subtitles_CheckBox_NoSystemMessages.IsChecked.Value;
		settings.Subtitles.OnlyLinkedAudio = Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value;
		settings.Subtitles.OnlySelectedCharacters = Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value;
		settings.Subtitles.GiveAudioToNoLinkedAudio = Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value;
		settings.Subtitles.GenerateMessages = Subtitles_CheckBox_GenerateMessages.IsChecked.Value;
		settings.Subtitles.MarkovLevel = (int)Subtitles_NumericUpDown_MarkovLevel.Value;

		settings.Subtitles.SelectedCharacters = [];
		for (int i = 0; i < SubtitleCheckBoxes.Length; i++)
			if (SubtitleCheckBoxes[i].IsChecked == true)
				settings.Subtitles.SelectedCharacters.Add((SubtitleCharacters)i);

		// Music
		settings.Music.Randomize = Music_CheckBox_RandomizeMusic.IsChecked.Value;
		settings.Music.SkipChaosPowers = Music_CheckBox_SkipChaosPowerUseJingles.IsChecked.Value;
		settings.Music.SkipRankTheme = Music_CheckBox_SkipRankTheme.IsChecked.Value;

		// Models
		settings.Models.Randomize = Models_CheckBox_RandomizeModel.IsChecked.Value;
		settings.Models.RandomizeP2 = Models_CheckBox_ModelP2.IsChecked.Value;

		// Spoilers
		settings.Spoilers.GraphUseIcons = Spoilers_CheckBox_UseIcons.IsChecked.Value;
		settings.Spoilers.AutosaveLog = Spoilers_CheckBox_AutosaveLog.IsChecked.Value;

		settings.Save();
	}

	private async void LoadGameData()
	{
		settings.GamePath = selectedFolderPath;
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
				if (File.Exists(nrmLayoutPath)) // some stages don't have nrm
					File.Copy(nrmLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, nrmLayout));
				if (File.Exists(hrdLayoutPath)) // some stages don't have hrd
					File.Copy(hrdLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, hrdLayout));
				if (File.Exists(ds1LayoutPath)) // some stages don't have ds1
					File.Copy(ds1LayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, ds1Layout));
			}
		}
	}

	private static int CalculateSeed(string seedString)
	{
		return BitConverter.ToInt32(SHA256.HashData(Encoding.UTF8.GetBytes(seedString)), 0);
	}

	private async void Button_Randomize_Click(object? sender, RoutedEventArgs e)
	{
		if (randomizeProcessing)
			return;
		randomizeProcessing = true;
		ProgressBar_RandomizationProgress.Value = 0;
		Spoilers_Button_MakeChart.IsEnabled = false;
		Spoilers_Button_SaveLog.IsEnabled = false;
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
			await Utils.ShowSimpleMessage("Error", "Invalid Seed", ButtonEnum.Ok, Icon.Error);
			randomizeProcessing = false;
			return;
		}
		ProgressBar_RandomizationProgress.Value = 5;

		UpdateSettings();
		int result = await Task.Run(RandomizationProcess);
		if (result != 0)
		{
			ProgressBar_RandomizationProgress.Value = 0;
			randomizeProcessing = false;
			return;
		}
		Spoilers_ListBox_LevelList.Items.Clear();
		for (int i = 0; i < stagecount; i++)
			Spoilers_ListBox_LevelList.Items.Add(GetStageName(stageids[i]));
		Spoilers_ListBox_LevelList.IsEnabled = true;
		Spoilers_ListBox_LevelList.SelectedIndex = 0;
		Spoilers_Button_SaveLog.IsEnabled = true;
		Spoilers_Button_MakeChart.IsEnabled = true;

		randomizeProcessing = false;
		var msgbox = await Utils.ShowSimpleMessage("ShadowRando", "Randomization Complete", ButtonEnum.Ok, Icon.Info);
	}

	private async Task<int> RandomizationProcess()
	{
		// Restore all backup files to undo any previous randomizations.
		// No need to restore main.dol as we overwrite that regardless of settings.
		File.Copy(Path.Combine("backup", "bi2.bin"), Path.Combine(settings.GamePath, "sys", "bi2.bin"), true);
		File.Copy(Path.Combine("backup", "setid.bin"), Path.Combine(settings.GamePath, "files", "setid.bin"), true);
		File.Copy(Path.Combine("backup", "nukkoro2.inf"), Path.Combine(settings.GamePath, "files", "nukkoro2.inf"), true);
		CopyDirectory(Path.Combine("backup", "fonts"), Path.Combine(settings.GamePath, "files", "fonts"), true);
		CopyDirectory(Path.Combine("backup", "music"), Path.Combine(settings.GamePath, "files"), true);
		CopyDirectory(Path.Combine("backup", "character"), Path.Combine(settings.GamePath, "files", "character"), true);
		CopyDirectory(Path.Combine("backup", "sets"), Path.Combine(settings.GamePath, "files"), true);

		Dispatcher.UIThread.Post(() => UpdateProgressBar(10));
		
		dolfile = File.ReadAllBytes(Path.Combine("backup", "main.dol"));
		var seed = CalculateSeed(settings.Seed);
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
			switch (settings.LevelOrder.Mode)
			{
				case LevelOrderMode.Original:
				case LevelOrderMode.VanillaStructure:
					include = !stages[i].IsLast;
					break;
				case LevelOrderMode.VanillaStructureNoBosses:
					include = !stages[i].IsLast && !stages[i].IsBoss;
					break;
				case LevelOrderMode.BossRush:
					include = stages[i].IsBoss && !settings.LevelOrder.ExcludeLevels.Contains((Levels)i);
					break;
				default:
					include = !settings.LevelOrder.ExcludeLevels.Contains((Levels)i);
					break;
			}
			if (include)
				tmpids.Add(i);
		}
		if (tmpids.Count == 0)
		{
			Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("ShadowRando", "All valid stages for the selected randomization mode have been excluded! You must enable at least one stage.", ButtonEnum.Ok, Icon.Error));
			return 1;
		}
		stagecount = tmpids.Count;
		tmpids.Add(totalstagecount);
		stageids = tmpids.ToArray();
		switch (settings.LevelOrder.Mode)
		{
			case LevelOrderMode.Original:
				break;
			case LevelOrderMode.AllStagesWarps:
				{
					if (settings.LevelOrder.AllowBossToBoss || stageids.Take(stagecount).Count(a => stages[a].IsBoss) < 2)
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
					switch (settings.LevelOrder.MainPath)
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
							if (r.Next(100) < settings.LevelOrder.BackwardsJumpProbability && (i > 0 || settings.LevelOrder.BackwardsJumpProbability == 100))
							{
								min = Math.Max(i - settings.LevelOrder.MaxBackwardsJump, 0);
								max = Math.Max(i - 1 + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1), 0);
							}
							else
							{
								min = i + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1);
								max = Math.Min(i + settings.LevelOrder.MaxForwardsJump + 1, stagecount + 1);
							}
							stg.Neutral = stageids[r.Next(min, max)];
						}
						if (stg.HasHero && stg.Hero == -1)
						{
							if (r.Next(100) < settings.LevelOrder.BackwardsJumpProbability && (i > 0 || settings.LevelOrder.BackwardsJumpProbability == 100))
							{
								min = Math.Max(i - settings.LevelOrder.MaxBackwardsJump, 0);
								max = Math.Max(i - 1 + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1), 0);
							}
							else
							{
								min = i + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1);
								max = Math.Min(i + settings.LevelOrder.MaxForwardsJump + 1, stagecount + 1);
							}
							stg.Hero = stageids[r.Next(min, max)];
						}
						if (stg.HasDark && stg.Dark == -1)
						{
							if (r.Next(100) < settings.LevelOrder.BackwardsJumpProbability && (i > 0 || settings.LevelOrder.BackwardsJumpProbability == 100))
							{
								min = Math.Max(i - settings.LevelOrder.MaxBackwardsJump, 0);
								max = Math.Max(i - 1 + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1), 0);
							}
							else
							{
								min = i + (settings.LevelOrder.AllowJumpsToSameLevel ? 0 : 1);
								max = Math.Min(i + settings.LevelOrder.MaxForwardsJump + 1, stagecount + 1);
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
						for (int i = 0; i < set.bossCount; i++)
							neword.Add(bossq.Dequeue());
					}
					neword.AddRange(last);
					int ind = 0;
					foreach (var set in ShadowStageSet.StageList)
					{
						int bossind = ind + set.stages.Count;
						int next = set.stages.Count + set.bossCount;
						if (set.stages[0].stageType == StageType.Neutral)
							++next;
						foreach (var item in set.stages)
						{
							Stage stg = stages[neword[ind]];
							if (item.bossCount == 2)
							{
								stg.SetExit(0, neword[bossind]);
								stages[neword[bossind++]].Neutral = totalstagecount;
								stg.SetExit(1, neword[bossind]);
								stages[neword[bossind++]].Neutral = totalstagecount;
							}
							else if (item.bossCount == 1)
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
						ind += set.bossCount;
					}
					neword.CopyTo(stageids);
				}
				break;
			case LevelOrderMode.VanillaStructureNoBosses:
				{
					List<int> twoexitlst = new List<int>();
					List<int> threeexitlst = new List<int>();
					List<int> last = new List<int>();
					for (int i = 0; i < stagecount; i++)
					{
						var stg = stages[stageids[i]];
						if (stg.IsLast)
							last.Add(stageids[i]);
						else if (stg.CountExits() == 3)
							threeexitlst.Add(stageids[i]);
						else
							twoexitlst.Add(stageids[i]);
					}
					int[] twoexit = twoexitlst.ToArray();
					int[] threeexit = threeexitlst.ToArray();
					Shuffle(r, twoexit);
					Shuffle(r, threeexit);
					Queue<int> twoq = new Queue<int>(twoexit);
					Queue<int> threeq = new Queue<int>(threeexit);
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
					}
					neword.AddRange(last);
					int ind = 0;
					foreach (var set in ShadowStageSet.StageList)
					{
						int bossind = ind + set.stages.Count;
						int next = set.stages.Count;
						if (set.stages[0].stageType == StageType.Neutral)
							++next;
						foreach (var item in set.stages)
						{
							Stage stg = stages[neword[ind]];
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
							++ind;
						}
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
							while (l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
								while (l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
								while (l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
						while (next != totalstagecount && l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
							while (next != totalstagecount && l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
						if (settings.LevelOrder.ExcludeLevels.Contains((Levels)stg.ID))
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
							while (next != totalstagecount && l++ < 10 && !settings.LevelOrder.AllowBossToBoss && stg.IsBoss && stages[next].IsBoss);
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
		if (settings.LevelOrder.Mode != LevelOrderMode.Original)
		{
			// patch the events code to allow skip all events
			buf = BitConverter.GetBytes(cutsceneEventDisablerPatchValue);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, cutsceneEventDisablerOffset);
			// end patch
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

		Dispatcher.UIThread.Post(() => UpdateProgressBar(15));

		// patch the route menu to allow stg06xx+ to display next stages
		buf = BitConverter.GetBytes(routeMenu6xxStagePreviewPatchValue);
		Array.Reverse(buf);
		buf.CopyTo(dolfile, routeMenu6xxStagePreviewBlockerOffset);
		// end patch

		if (settings.LevelOrder.ExpertMode)
		{
			var exids = Enumerable.Range(0, totalstagecount).Except(settings.LevelOrder.ExcludeLevels.Select(a => (int)a)).ToArray();
			Shuffle(r, exids);
			for (int i = 0; i < exids.Length; i++)
			{
				buf = BitConverter.GetBytes(exids[i] + stagefirst);
				Array.Reverse(buf);
				buf.CopyTo(dolfile, expertModeLevelsOffset + (i * sizeof(int)));
			}

			// patch expert mode list memory region, to allow more than 27 stages
			buf = BitConverter.GetBytes(expertModeExtendedLevelSlotsPatchValue);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, expertModeExtendedLevelSlotsPatchOffset1);
			buf.CopyTo(dolfile, expertModeExtendedLevelSlotsPatchOffset2);
			
			buf = BitConverter.GetBytes(expertModeExtendedLevelSlotsEndCheckPatchValue + exids.Length);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, expertModeExtendedLevelSlotsEndCheckPatchOffset1);
			buf.CopyTo(dolfile, expertModeExtendedLevelSlotsEndCheckPatchOffset2);
			
			// end patch
		}

		if (settings.Layout.Randomize && (settings.Layout.Weapon.RandomWeaponsInAllBoxes || settings.Layout.Weapon.RandomWeaponsInWeaponBoxes))
		{
			// special weapons box patch
			buf = BitConverter.GetBytes(shadowBoxPatchValue);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, shadowBoxPatchOffset);
			// end special weapons box patch
		}

		if (settings.Music.Randomize)
		{
			Dictionary<MusicCategory, List<string>> musicFiles = new Dictionary<MusicCategory, List<string>>()
				{
					{ MusicCategory.Stage, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_stg*.adx").Where(a => !a.EndsWith("sng_stg0710b.adx"))) },
					{ MusicCategory.Jingle, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_jin*.adx")) },
					{ MusicCategory.Menu, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_sys*.adx")) },
					{ MusicCategory.Credits, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_vox*.adx")) }
				};
			if (settings.Music.SkipRankTheme)
				musicFiles[MusicCategory.Jingle].RemoveAll(a => a.EndsWith("sng_jin_roundclear.adx"));
			if (settings.Music.SkipChaosPowers)
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

		Dispatcher.UIThread.Post(() => UpdateProgressBar(25));

		if (settings.Layout.Randomize)
		{
			var layoutResult = RandomizeLayouts(r);
			if (layoutResult == 1)
				return 1;
		}

		Dispatcher.UIThread.Post(() => UpdateProgressBar(50));

		if (settings.Subtitles.Randomize)
		{
			var randomizeSubtitles = RandomizeSubtitles(r);
			if (randomizeSubtitles == 1)
				return 1;
		}

		Dispatcher.UIThread.Post(() => UpdateProgressBar(75));

		if (settings.Models.Randomize)
		{
			var	randomizeModelsResult = RandomizeModels(r);
			if (randomizeModelsResult == 1)
				return 1;
		}

		settings.Save();
		File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "main.dol"), dolfile);
		Dispatcher.UIThread.Post(() => UpdateProgressBar(100));
		if (settings.Spoilers.AutosaveLog)
		{
			Directory.CreateDirectory("logs");
			WriteLog(Path.Combine("logs", DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".txt"));
		}
		return 0;
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

	private int RandomizeSubtitles(Random r)
	{
		var fontAndAudioData = LoadFNTsAndAFS(true);
		var fntRandomPool = new List<TableEntry>();
		var uniqueAudioIDs = new Dictionary<int, bool>();
		var uniqueSubtitles = new Dictionary<string, bool>();
		MarkovTextModel markov = new MarkovTextModel(settings.Subtitles.MarkovLevel);

		if (settings.Subtitles.OnlySelectedCharacters && settings.Subtitles.SelectedCharacters.Count == 0)
		{
			Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Subtitle/Voice Randomization Error", "Must select at least one character for subtitles", ButtonEnum.Ok, Icon.Error));
			return 1;
		}

		if (settings.Subtitles.OnlyLinkedAudio || settings.Subtitles.NoDuplicates || settings.Subtitles.NoSystemMessages || settings.Subtitles.OnlySelectedCharacters)
		{
			for (int i = 0; i < fontAndAudioData.initialFntState.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.initialFntState[i].GetEntryTableCount(); j++)
				{
					var entry = fontAndAudioData.initialFntState[i].GetEntryTable()[j];
					if (settings.Subtitles.OnlyLinkedAudio && entry.audioId == -1)
						continue;
					if (settings.Subtitles.NoSystemMessages && (entry.entryType == EntryType.MENU || entry.entryType == EntryType.FINAL_ENTRY || entry.messageIdBranchSequence == 9998100))
						continue;
					if (settings.Subtitles.OnlySelectedCharacters && entry.audioId != -1 && !SubtitleCharacterPicked(fontAndAudioData.afs.Files[entry.audioId].Name))
						continue;

					if (settings.Subtitles.NoDuplicates)
					{
						if (entry.audioId != -1 && uniqueAudioIDs.ContainsKey(entry.audioId))
							continue;
						// this covers chained entries and any repeating messages with -1; Such as system dialogs if the user is not using that filter
						if (entry.audioId == -1 && uniqueSubtitles.ContainsKey(entry.subtitle))
							continue;
					}
					uniqueAudioIDs[entry.audioId] = true;
					uniqueSubtitles[entry.subtitle] = true;
					fntRandomPool.Add(entry);
				}
			}
			// customized fnt pool built; begin applying
			if (settings.Subtitles.GenerateMessages)
				foreach (var a in fntRandomPool)
					markov.AddString(a.subtitle);
			for (int i = 0; i < fontAndAudioData.mutatedFnt.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.mutatedFnt[i].GetEntryTableCount(); j++)
				{
					// Chained entries not accounted for, so may produce wacky results
					int donorFNTEntryIndex = r.Next(0, fntRandomPool.Count - 1);
					if (settings.Subtitles.GiveAudioToNoLinkedAudio && fntRandomPool[donorFNTEntryIndex].audioId == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fntRandomPool[donorFNTEntryIndex].audioId);
					}
					if (settings.Subtitles.GenerateMessages)
						fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, markov.Generate(r));
					else
						fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fntRandomPool[donorFNTEntryIndex].subtitle);
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fntRandomPool[donorFNTEntryIndex].subtitleActiveTime);
				}
			}
		}
		else
		{
			if (settings.Subtitles.GenerateMessages)
				foreach (var a in fontAndAudioData.initialFntState)
					foreach (var b in a.GetEntryTable())
						markov.AddString(b.subtitle);
			for (int i = 0; i < fontAndAudioData.mutatedFnt.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.mutatedFnt[i].GetEntryTableCount(); j++)
				{
					// Chained entries not accounted for, so may produce wacky results
					int donorFNTIndex = r.Next(0, fontAndAudioData.mutatedFnt.Count - 1);
					int donorFNTEntryIndex = r.Next(0, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryTableCount() - 1);
					if (settings.Subtitles.GiveAudioToNoLinkedAudio && fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donorFNTEntryIndex) == -1)
					{
						int audio = r.Next(0, fontAndAudioData.afs.Files.Count - 1);
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, audio);
					}
					else
					{
						fontAndAudioData.mutatedFnt[i].SetEntryAudioId(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donorFNTEntryIndex));
					}

					if (settings.Subtitles.GenerateMessages)
						fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, markov.Generate(r));
					else
						fontAndAudioData.mutatedFnt[i].SetEntrySubtitle(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitle(donorFNTEntryIndex));
					fontAndAudioData.mutatedFnt[i].SetEntrySubtitleActiveTime(j, fontAndAudioData.initialFntState[donorFNTIndex].GetEntrySubtitleActiveTime(donorFNTEntryIndex));
				}
			}
		}
		ExportChangedFNTs(fontAndAudioData.mutatedFnt, fontAndAudioData.initialFntState);
		return 0;
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
		using var enTXD = AssetLoader.Open(new Uri($"avares://ShadowRando/Assets/EN.txd"));
		using var enMET = AssetLoader.Open(new Uri($"avares://ShadowRando/Assets/EN00.met"));
		using MemoryStream txdStream = new MemoryStream();
		enTXD.CopyTo(txdStream);
		byte[] txdBytes = txdStream.ToArray();
		using MemoryStream metStream = new MemoryStream();
		enMET.CopyTo(metStream);
		byte[] metBytes = metStream.ToArray();

		foreach (FNT fnt in filesToWrite)
		{
			fnt.RecomputeAllSubtitleAddresses();
			string outfn = Path.Combine(settings.GamePath, "files", fnt.fileName.Substring(fnt.fileName.IndexOf("fonts")));
			File.WriteAllBytes(outfn, fnt.ToBytes());
			string prec = outfn.Remove(outfn.Length - 4);
			File.WriteAllBytes(prec + ".txd", txdBytes);
			File.WriteAllBytes(prec + "00.met", metBytes);
		}
	}

	private bool SubtitleCharacterPicked(string audioName)
	{
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Shadow) && audioName.EndsWith("_sd.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Sonic) && audioName.EndsWith("_sn.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Tails) && audioName.EndsWith("_tl.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Knuckles) && audioName.EndsWith("_kn.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Amy) && audioName.EndsWith("_am.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Rouge) && audioName.EndsWith("_rg.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Omega) && audioName.EndsWith("_om.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Vector) && audioName.EndsWith("_vc.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Espio) && audioName.EndsWith("_es.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Maria) && (audioName.EndsWith("_mr.adx") || audioName.EndsWith("_mr2.adx")))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Charmy) && audioName.EndsWith("_ch.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Eggman) && audioName.EndsWith("_eg.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.BlackDoom) && audioName.EndsWith("_bd.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Cream) && audioName.EndsWith("_cr.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.Cheese) && audioName.EndsWith("_co.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.GUNCommander) && audioName.EndsWith("_cm.adx"))
			return true;
		if (settings.Subtitles.SelectedCharacters.Contains(SubtitleCharacters.GUNSoldier) && audioName.EndsWith("_sl.adx"))
			return true;
		return false;
	}

	private int RandomizeLayouts(Random r)
	{
		var enemyMode = (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex;
		var nukkoro2 = Nukkoro2.ReadFile(Path.Combine("backup", "nukkoro2.inf"));

		LayoutEditorSystem.SetupLayoutEditorSystem(); // Critical to load relevent data

		// Perform our error checking and filtering before we enter the per-stage loop

		// Enemy Filtering & Error Cases
		List<Type> allEnemies = new List<Type>();
		List<Type> groundEnemies = new List<Type>();
		List<Type> flyingEnemies = new List<Type>();
		List<Type> pathTypeFlyingEnemies = new List<Type>();

		if (settings.Layout.Enemy.OnlySelectedTypes && enemyMode != LayoutEnemyMode.Original)
		{
			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.GUNSoldier))
			{
				groundEnemies.Add(typeof(Object0064_GUNSoldier));
				allEnemies.Add(typeof(Object0064_GUNSoldier));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.GUNBeetle))
			{
				flyingEnemies.Add(typeof(Object0065_GUNBeetle));
				pathTypeFlyingEnemies.Add(typeof(Object0065_GUNBeetle));
				allEnemies.Add(typeof(Object0065_GUNBeetle));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.GUNBigfoot))
			{
				groundEnemies.Add(typeof(Object0066_GUNBigfoot));
				flyingEnemies.Add(typeof(Object0066_GUNBigfoot));
				allEnemies.Add(typeof(Object0066_GUNBigfoot));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.GUNRobot))
			{
				groundEnemies.Add(typeof(Object0068_GUNRobot));
				allEnemies.Add(typeof(Object0068_GUNRobot));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.EggPierrot))
			{
				groundEnemies.Add(typeof(Object0078_EggPierrot));
				allEnemies.Add(typeof(Object0078_EggPierrot));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.EggPawn))
			{
				groundEnemies.Add(typeof(Object0079_EggPawn));
				allEnemies.Add(typeof(Object0079_EggPawn));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.ShadowAndroid))
			{
				groundEnemies.Add(typeof(Object007A_EggShadowAndroid));
				allEnemies.Add(typeof(Object007A_EggShadowAndroid));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BAGiant))
			{
				groundEnemies.Add(typeof(Object008C_BkGiant));
				allEnemies.Add(typeof(Object008C_BkGiant));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BASoldier))
			{
				groundEnemies.Add(typeof(Object008D_BkSoldier));
				allEnemies.Add(typeof(Object008D_BkSoldier));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BAHawkVolt))
			{
				flyingEnemies.Add(typeof(Object008E_BkWingLarge));
				pathTypeFlyingEnemies.Add(typeof(Object008E_BkWingLarge));
				allEnemies.Add(typeof(Object008E_BkWingLarge));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BAWing))
			{
				flyingEnemies.Add(typeof(Object008F_BkWingSmall));
				pathTypeFlyingEnemies.Add(typeof(Object008F_BkWingSmall));
				allEnemies.Add(typeof(Object008F_BkWingSmall));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BAWorm))
			{
				groundEnemies.Add(typeof(Object0090_BkWorm));
				allEnemies.Add(typeof(Object0090_BkWorm));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BALarva))
			{
				groundEnemies.Add(typeof(Object0091_BkLarva));
				allEnemies.Add(typeof(Object0091_BkLarva));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.ArtificialChaos))
			{
				flyingEnemies.Add(typeof(Object0092_BkChaos));
				allEnemies.Add(typeof(Object0092_BkChaos));
			}

			if (settings.Layout.Enemy.SelectedEnemies.Contains(Core.EnemyTypes.BAAssassin))
			{
				groundEnemies.Add(typeof(Object0093_BkNinja));
				flyingEnemies.Add(typeof(Object0093_BkNinja));
				allEnemies.Add(typeof(Object0093_BkNinja));
			}

			// error checking
			if (settings.Layout.Enemy.KeepType)
			{
				if (groundEnemies.Count == 0 || flyingEnemies.Count == 0)
				{
					Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error", "Must have at least one ground and one flying enemy.", ButtonEnum.Ok, Icon.Error));
					return 1; // TODO do we want to throw errors?
				}

				if (groundEnemies.Count == 1)
				{
					// make sure there is at least one other enemy if GUN Soldiers are only picked
					if (groundEnemies[0] == typeof(Object0064_GUNSoldier))
					{
						Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error",
							"GUN Soldiers have an issue with some Link IDs, add an extra ground enemy type.",
							ButtonEnum.Ok, Icon.Error));
						return 1;
					}
				}
			}
			else
			{
				if (allEnemies.Count == 0)
				{
					Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error", "Must pick at least one enemy type.", ButtonEnum.Ok, Icon.Error));
					return 1;
				}

				if (allEnemies.Count == 1)
				{
					// make sure there is at least one other enemy if GUN Soldiers are only picked
					if (allEnemies[0] == typeof(Object0064_GUNSoldier))
					{
						Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error",
							"GUN Soldiers have an issue with some Link IDs, add an extra enemy type.", ButtonEnum.Ok,
							Icon.Error));
						return 1;
					}
				}
			}
		}
		else
		{
			groundEnemies.AddRange(GroundEnemyTypes);
			flyingEnemies.AddRange(FlyingEnemyTypes);
			pathTypeFlyingEnemies.AddRange([
				typeof(Object0065_GUNBeetle), typeof(Object008E_BkWingLarge), typeof(Object008F_BkWingSmall)
			]);
			allEnemies.AddRange(EnemyTypes);
		}


		// Weapon Filtering
		List<EWeapon> weaponsPool = [];

		if (settings.Layout.Weapon.OnlySelectedTypes)
		{
			weaponsPool.AddRange(settings.Layout.Weapon.SelectedWeapons);
			if (weaponsPool.Count == 0)
			{
				Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error", "Must select at least one weapon.", ButtonEnum.Ok, Icon.Error));
				return 1;
			}
		}
		else
		{
			weaponsPool.AddRange(Weapons);
		}
		
		// Partner Filtering & Error Cases
		List<Object0190_Partner.EPartner> darkPartners =
		[
			Object0190_Partner.EPartner.Eggman,
			Object0190_Partner.EPartner.DoomsEye
		];
		List<Object0190_Partner.EPartner> heroPartners =
		[
			Object0190_Partner.EPartner.Sonic,
			Object0190_Partner.EPartner.Tails,
			Object0190_Partner.EPartner.Knuckles,
			Object0190_Partner.EPartner.Amy,
			Object0190_Partner.EPartner.Rouge,
			Object0190_Partner.EPartner.Omega,
			Object0190_Partner.EPartner.Vector,
			Object0190_Partner.EPartner.Espio,
			Object0190_Partner.EPartner.Maria,
			Object0190_Partner.EPartner.Charmy
		];

		Dispatcher.UIThread.Post(() => UpdateProgressBar(30));

		if (settings.Layout.Partner.Mode == LayoutPartnerMode.Wild)
		{
			if (settings.Layout.Partner.OnlySelectedPartners)
			{
				heroPartners = heroPartners.Intersect(settings.Layout.Partner.SelectedPartners).ToList();
				darkPartners = darkPartners.Intersect(settings.Layout.Partner.SelectedPartners).ToList();
			}

			if (settings.Layout.Partner.RandomizeAffiliations)
			{
				var partners = darkPartners.Concat(heroPartners).ToArray();
				Shuffle(r, partners);
				darkPartners = partners.Take(partners.Length / 2).ToList();
				heroPartners = partners.Skip(partners.Length / 2).ToList();

				// patch partner affiliations
				var darkAffiliation = BitConverter.GetBytes(partnerAffiliationDarkPatchValue);
				Array.Reverse(darkAffiliation);
				PatchPartnerAffiliations(darkAffiliation, darkPartners);
				var heroAffiliation = BitConverter.GetBytes(partnerAffiliationHeroPatchValue);
				Array.Reverse(heroAffiliation);
				PatchPartnerAffiliations(heroAffiliation, heroPartners);
				// end patching partner affiliations
			}
			
			if (heroPartners.Count == 0 || darkPartners.Count == 0)
			{
				Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Error", "Must have at least one dark and one hero partner.", ButtonEnum.Ok, Icon.Error));
				return 1;
			}
		}
		
		// Begin the per-stage layout modifications
		for (int stageIdToModify = 5; stageIdToModify < 45; stageIdToModify++)
		{
			Dispatcher.UIThread.Post(() => UpdateProgressBar(30 + stageIdToModify / 9));
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

			if (settings.Layout.Weapon.RandomWeaponsInAllBoxes)
			{
				MakeAllBoxesHaveRandomWeapons(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					MakeAllBoxesHaveRandomWeapons(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					MakeAllBoxesHaveRandomWeapons(ref hrdLayoutData, weaponsPool, r);
			} else if (settings.Layout.Weapon.RandomWeaponsInWeaponBoxes)
			{
				MakeAllWeaponBoxesHaveRandomWeapons(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					MakeAllWeaponBoxesHaveRandomWeapons(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					MakeAllWeaponBoxesHaveRandomWeapons(ref hrdLayoutData, weaponsPool, r);
			}

			if (settings.Layout.Weapon.RandomExposedWeapons)
			{
				RandomizeWeaponsOnGround(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					RandomizeWeaponsOnGround(ref nrmLayoutData, weaponsPool, r);
				if (hrdLayoutData != null)
					RandomizeWeaponsOnGround(ref hrdLayoutData, weaponsPool, r);
			}

			if (settings.Layout.Weapon.RandomWeaponsFromEnvironment)
			{
				RandomizeEnvironmentWeaponDrops(ref cmnLayoutData, weaponsPool, r);
				if (nrmLayoutData != null)
					RandomizeEnvironmentWeaponDrops(ref nrmLayoutData, weaponsPool, r);
				if (ds1LayoutData != null)
					RandomizeEnvironmentWeaponDrops(ref ds1LayoutData, weaponsPool, r);
			}

			if (settings.Layout.Partner.Mode == LayoutPartnerMode.Wild)
			{
				MakeAllPartnersRandom(ref cmnLayoutData, settings.Layout.Partner.KeepAffiliationsAtSameLocation, darkPartners, heroPartners, r);
				if (nrmLayoutData != null)
					MakeAllPartnersRandom(ref nrmLayoutData, settings.Layout.Partner.KeepAffiliationsAtSameLocation, darkPartners, heroPartners, r);
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
			
			if (stageId == 400)
			{
				// add guaranteed vacuums for central city
				var centralCityVacuum1 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, -110f, -9f, 96f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				centralCityVacuum1.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(centralCityVacuum1);
				var centralCityVacuum2 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, -81f, -9f, 120f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				centralCityVacuum2.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(centralCityVacuum2);
			} else if (stageId == 604)
			{
				// add guaranteed vacuums for final haunt
				var finalHauntVacuum1 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, 283f, -1080f, -11974f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				finalHauntVacuum1.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(finalHauntVacuum1);
				var finalHauntVacuum2 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, 1365f, -1583f, -15446f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				finalHauntVacuum2.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(finalHauntVacuum2);
				var finalHauntVacuum3 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, 517f, -4220f, -27053f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				finalHauntVacuum3.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(finalHauntVacuum3);
				var finalHauntVacuum4 = (Object0020_Weapon)LayoutEditorFunctions.CreateShadowObject(0x00,
					0x20, -110f, -4300f, -31851f, 0f, 0f, 0f, 0, 6, new byte[] {0x01, 0x10, 0x00, 0x80, 0x01, 0x10, 0x00, 0x00});
				finalHauntVacuum4.Weapon = EWeapon.VacuumPod;
				cmnLayoutData.Add(finalHauntVacuum4);
			}

			LayoutEditorFunctions.SaveShadowLayout(cmnLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, cmnLayout), false);
			if (nrmLayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(nrmLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, nrmLayout), false);
			if (hrdLayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(hrdLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, hrdLayout), false);
			if (ds1LayoutData != null)
				LayoutEditorFunctions.SaveShadowLayout(ds1LayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, ds1Layout), false);

			if (settings.Layout.Enemy.AdjustMissionCounts && settings.Layout.Enemy.Mode == LayoutEnemyMode.Wild && Nukkoro2EnemyCountStages.TryGetValue(stageId, out var nukkoro2StageString))
			{
				nukkoro2.TryGetValue(nukkoro2StageString.Item1, out var nukkoro2Stage);
				switch (nukkoro2StageString.Item2)
				{
					case 0:
						var total = GetTotalGUNEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountDark.Success = total - (int)(total * (settings.Layout.Enemy.AdjustMissionCountsReductionPercent / 100));
						break;
					case 1:
						total = GetTotalBlackArmsEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountHero.Success = total - (int)(total * (settings.Layout.Enemy.AdjustMissionCountsReductionPercent / 100));
						break;
					case 2:
						total = GetTotalGUNEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountDark.Success = total - (int)(total * (settings.Layout.Enemy.AdjustMissionCountsReductionPercent / 100));
						total = GetTotalBlackArmsEnemies(cmnLayoutData, nrmLayoutData);
						nukkoro2Stage.MissionCountHero.Success = total - (int)(total * (settings.Layout.Enemy.AdjustMissionCountsReductionPercent / 100));
						break;
					default:
						break;
				}
			}

			if (settings.Layout.MakeCCSplinesVehicleCompatible)
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
						if (spline.SplineType == 32 && spline.Name.Contains("_cc_")
							&& !spline.Name.Contains("stg0300_cc_dr_jn_207")
							&& !spline.Name.Contains("stg0300_cc_dr_jn_208")
							&& !spline.Name.Contains("stg0300_cc_pr_jn_210")
							&& !((spline.Name.Contains("stg0100_jn_cc_010") || spline.Name.Contains("stg0100_jn_cc_011")) && stageDataIdentifier == "stg0202") // Ignore the leftover spline error in stg0202 from Sonic Team
							)
							spline.Setting2 = 1;
					}
					var updatedPATHPTP = SplineReader.ShadowSplinesToByteArray(stageDataIdentifier, splines);
					datOneDataContent.Files[0].CompressedData = Prs.CompressData(updatedPATHPTP).ToArray();
					var updatedDatOneData = datOneDataContent.BuildShadowONEArchive(archiveType == ONEArchiveType.Shadow060);
					File.WriteAllBytes(Path.Combine(settings.GamePath, "files", stageDataIdentifier, datOneFile), updatedDatOneData.ToArray());
				}
			}
		} // end - layout operations

		// setIdBin operations
		var setIdBINPath = Path.Combine("backup", "setid.bin");
		var setIdTable = SetIdTableFunctions.LoadTable(setIdBINPath, true, LayoutEditorSystem.shadowObjectEntries);

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
				foreach (StageEntry stage in LayoutEditorSystem.shadowStageEntries)
				{
					entry.values0 |= stage.flag0;
					entry.values1 |= stage.flag1;
					entry.values2 |= stage.flag2;
				}
			}
		}

		Dispatcher.UIThread.Post(() => UpdateProgressBar(44));

		SetIdTableFunctions.SaveTable(Path.Combine(settings.GamePath, "files", "setid.bin"), true, setIdTable);

		// patch bi2.bin since we require 64MB Dolphin
		var buf = BitConverter.GetBytes(0);
		var bi2 = File.ReadAllBytes(Path.Combine("backup", "bi2.bin"));
		buf.CopyTo(bi2, 0x4);
		File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "bi2.bin"), bi2);
		// end patch

		if (settings.Layout.Enemy.AdjustMissionCounts && settings.Layout.Enemy.Mode == LayoutEnemyMode.Wild)
		{
			Nukkoro2.WriteFile(Path.Combine(settings.GamePath, "files", "nukkoro2.inf"), nukkoro2);
		}

		return 0;
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

	private void PatchPartnerAffiliations(byte[] affiliationBytes, List<Object0190_Partner.EPartner> partners)
	{
		foreach (var partner in partners)
		{
			switch (partner)
			{
				case Object0190_Partner.EPartner.Sonic:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationSonicPatchOffset);
					break;
				case Object0190_Partner.EPartner.Tails:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationTailsPatchOffset);
					break;
				case Object0190_Partner.EPartner.Knuckles:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationKnucklesPatchOffset);
					break;
				case Object0190_Partner.EPartner.Amy:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationAmyPatchOffset);
					break;
				case Object0190_Partner.EPartner.Rouge:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationRougePatchOffset);
					break;
				case Object0190_Partner.EPartner.Omega:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationOmegaPatchOffset);
					break;
				case Object0190_Partner.EPartner.Vector:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationVectorPatchOffset);
					break;
				case Object0190_Partner.EPartner.Espio:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationEspioPatchOffset);
					break;
				case Object0190_Partner.EPartner.Maria:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationMariaPatchOffset);
					break;
				case Object0190_Partner.EPartner.Charmy:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationCharmyPatchOffset);
					break;
				case Object0190_Partner.EPartner.Eggman:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationEggmanPatchOffset);
					break;
				case Object0190_Partner.EPartner.DoomsEye:
					affiliationBytes.CopyTo(dolfile, partnerAffiliationDoomsEyePatchOffset);
					break;
				default:
					break;
			}
		}
		var eggmanStaticAssociation = BitConverter.GetBytes(partnerAffiliationEggmanStaticAssociationPatchValue);
		Array.Reverse(eggmanStaticAssociation);
		eggmanStaticAssociation.CopyTo(dolfile, partnerAffiliationEggmanStaticAssociationPatchOffset);
	}

	private static void MakeAllPartnersRandom(ref List<SetObjectShadow> setData, bool keepOriginalObjectAffiliation, List<Object0190_Partner.EPartner> darkPartners, List<Object0190_Partner.EPartner> heroPartners, Random r)
	{
		List<(Object0190_Partner item, int index)> partnerItems = setData
			.Select((item, index) => new { Item = item, Index = index })
			.Where(pair => pair.Item is Object0190_Partner)
			.Select(pair => (Item: (Object0190_Partner)pair.Item, Index: pair.Index))
			.ToList();
		
		List<Object0190_Partner.EPartner> partners = [];
		if (!keepOriginalObjectAffiliation) {
			partners.AddRange(darkPartners);
			partners.AddRange(heroPartners);
		}

		foreach (var partner in partnerItems)
		{
			if (keepOriginalObjectAffiliation)
			{
				if (partner.item.Partner is Object0190_Partner.EPartner.Eggman or Object0190_Partner.EPartner.DoomsEye) 
					partner.item.Partner = darkPartners[r.Next(darkPartners.Count)];
				else
					partner.item.Partner = heroPartners[r.Next(heroPartners.Count)];
			}
			else
			{
				partner.item.Partner = partners[r.Next(partners.Count)];
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
			if (settings.Layout.Enemy.KeepType)
			{
				if (EnemyHelpers.IsFlyingEnemy(setData[i]))
				{
					if (setData[i].List == 0x00 && setData[i].Type == 0x90)
					{
						// if BkWorm, mutate original posY +50
						setData[i].PosY = setData[i].PosY + 50;
					}
					// if path type enemy
					if (EnemyHelpers.IsRequiredPathTypeFlyingEnemy(setData[i]))
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
					if (groundEnemies.Count > 0 && groundEnemies[0] == typeof(Object0064_GUNSoldier))
						randomEnemy = r.Next(1, allEnemies.Count); // skip GUN Soldiers otherwise
					else
						randomEnemy = r.Next(allEnemies.Count);
				}
				randomEnemyType = allEnemies[randomEnemy];
			}
			SETMutations.MutateObjectAtIndex(i, randomEnemyType, ref setData, true, r);
		}
	}

	private int RandomizeModels(Random r)
	{
		if (!Directory.Exists("RandoModels"))
		{
			Dispatcher.UIThread.Post(() => Utils.ShowSimpleMessage("Model Randomization Error", "Model Randomization error! Check README on Project Page to learn how to setup this feature.", ButtonEnum.Ok, Icon.Error));
			return 1;
		}
		var mdls = Directory.GetFiles("RandoModels", "shadow.one", SearchOption.AllDirectories).Prepend(Path.Combine("backup", "character", "shadow.one")).ToArray();
		var p1mdl = mdls[r.Next(mdls.Length)]; // pick a random p1 model
		if (p1mdl.Contains("ModelPack")) // if the model belongs to a pack, copy all files from the pack and do nothing else
			CopyDirectory(Path.GetDirectoryName(p1mdl), Path.Combine(settings.GamePath, "files", "character"), true);
		else
		{
			File.Copy(p1mdl, Path.Combine(settings.GamePath, "files", "character", "shadow.one"), true);
			if (settings.Models.RandomizeP2) // do we care about p2?
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
		return 0;
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

	private void LevelOrder_Button_ProjectPage_Click(object? sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://github.com/ShadowTheHedgehogHacking/ShadowRando") { UseShellExecute = true });
	}

	private void LevelOrder_CheckBox_Random_Seed_Click(object? sender, RoutedEventArgs e)
	{
		UpdateUIEnabledState();
	}

	private void Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes_Click(object? sender, RoutedEventArgs e)
	{
		if (Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked.Value)
			Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked = false;
	}

	private void Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes_Click(object? sender, RoutedEventArgs e)
	{
		if (Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsChecked.Value)
			Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsChecked = false;
	}

	private void Subtitles_CheckBox_OnlyWithLinkedAudio_Click(object? sender, RoutedEventArgs e)
	{
		if (Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked.Value)
			Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked = false;
	}

	private void Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles_Click(object? sender, RoutedEventArgs e)
	{
		if (Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsChecked.Value)
			Subtitles_CheckBox_OnlyWithLinkedAudio.IsChecked = false;
	}

	const int linespace = 8;
	private async void Spoilers_Button_MakeChart_Click(object? sender, RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel == null)
			return;
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
		switch (settings.LevelOrder.Mode)
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
			case LevelOrderMode.Original:
			case LevelOrderMode.VanillaStructure:
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
				break;
			case LevelOrderMode.VanillaStructureNoBosses:
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
		if (Spoilers_CheckBox_UseIcons.IsChecked.Value)
			textsz = new SKSizeI(88, 69);
		else
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
					if (Spoilers_CheckBox_UseIcons.IsChecked.Value && id < totalstagecount)
						using (var asset = AssetLoader.Open(new Uri($"avares://ShadowRando/Assets/{GetStageName(id)}.png")))
						using (var bmp = SKBitmap.Decode(asset))
							gfx.DrawBitmap(bmp, new SKPoint(x, y));
					else
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
								//Darkness Shift
								const float darknessShift = 4.00f;
								triPaint.Color = linePaint.Color = SKColor.FromHsv(lineHSV[0], lineHSV[1], lineHSV[2] - (darknessShift * (shiftAmount/2)));
							}
							else
							{
								//Hue Shift
								const float hueShift = 3.00f;
								triPaint.Color = linePaint.Color = SKColor.FromHsv(lineHSV[0] + (hueShift * (shiftAmount/2)), lineHSV[1], lineHSV[2]);
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

	private async void Spoilers_Button_SaveLog_Click(object? sender, RoutedEventArgs e)
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
		WriteLog(file.TryGetLocalPath());
	}

	private async void WriteLog(string? file)
	{
		if (file is null) return;
		await using var stream = File.OpenWrite(file);
		await using var sw = new StreamWriter(stream);
		await sw.WriteLineAsync($"ShadowRando Version: {programVersion}");
		await sw.WriteLineAsync($"Seed: {settings.Seed}");
		await sw.WriteLineAsync($"Generated at: {DateTime.Now}");
		await sw.WriteLineAsync("---- Level Order ----");
		await sw.WriteLineAsync($"Level Order Mode: {settings.LevelOrder.Mode}");
		if (settings.LevelOrder.Mode == LevelOrderMode.AllStagesWarps)
		{
			await sw.WriteLineAsync($"Main Path: {settings.LevelOrder.MainPath}");
			await sw.WriteLineAsync($"Max Forwards Jump: {settings.LevelOrder.MaxForwardsJump}");
			await sw.WriteLineAsync($"Max Backwards Jump: {settings.LevelOrder.MaxBackwardsJump}");
			await sw.WriteLineAsync($"Backwards Jump Probability: {settings.LevelOrder.BackwardsJumpProbability}");
			await sw.WriteLineAsync($"Allow Jumps To Same Level: {settings.LevelOrder.AllowJumpsToSameLevel}");
		}
		await sw.WriteLineAsync($"Allow Boss -> Boss: {settings.LevelOrder.AllowBossToBoss}");
		await sw.WriteLineAsync($"Expert Mode: {settings.LevelOrder.ExpertMode}");
		await sw.WriteLineAsync($"Excluded Levels: {string.Join(", ", settings.LevelOrder.ExcludeLevels.Select(a => LevelNames[(int)a]))}");

		await sw.WriteLineAsync("---- Layout ----");
		await sw.WriteLineAsync($"Randomize Layouts: {settings.Layout.Randomize}");
		if (settings.Layout.Randomize)
		{
			await sw.WriteLineAsync($"Make CC Splines Vehicle Compatible: {settings.Layout.MakeCCSplinesVehicleCompatible}");

			await sw.WriteLineAsync("--- Enemy ---");
			await sw.WriteLineAsync($"Enemy Mode: {settings.Layout.Enemy.Mode}");
			await sw.WriteLineAsync($"Adjust Mission Counts: {settings.Layout.Enemy.AdjustMissionCounts}");
			await sw.WriteLineAsync($"Adjust Mission Counts Reduction %: {settings.Layout.Enemy.AdjustMissionCountsReductionPercent}");
			await sw.WriteLineAsync($"Keep Type: {settings.Layout.Enemy.KeepType}");
			await sw.WriteLineAsync($"Only Selected Enemy Types: {settings.Layout.Enemy.OnlySelectedTypes}");
			if (settings.Layout.Enemy.OnlySelectedTypes)
			{
				await sw.WriteLineAsync("-Selected Enemies-");
				foreach (var enemy in settings.Layout.Enemy.SelectedEnemies)
					await sw.WriteLineAsync($"{enemy}");
			}

			await sw.WriteLineAsync("--- Weapon ---");
			await sw.WriteLineAsync($"Random Weapons In Weapon Boxes: {settings.Layout.Weapon.RandomWeaponsInWeaponBoxes}");
			await sw.WriteLineAsync($"Random Weapons In All Boxes: {settings.Layout.Weapon.RandomWeaponsInAllBoxes}");
			await sw.WriteLineAsync($"Random Exposed Weapons: {settings.Layout.Weapon.RandomExposedWeapons}");
			await sw.WriteLineAsync($"Environment Drops Random Weapons: {settings.Layout.Weapon.RandomWeaponsFromEnvironment}");
			await sw.WriteLineAsync($"Only Selected Weapons: {settings.Layout.Weapon.OnlySelectedTypes}");
			if (settings.Layout.Weapon.OnlySelectedTypes)
			{
				await sw.WriteLineAsync("-Selected Weapons-");
				foreach (var wep in settings.Layout.Weapon.SelectedWeapons)
					await sw.WriteLineAsync($"{wep}");
			}

			await sw.WriteLineAsync("--- Partner ---");
			await sw.WriteLineAsync($"Partner Mode: {settings.Layout.Partner.Mode}");
			await sw.WriteLineAsync($"Randomize Dark/Hero Affiliations: {settings.Layout.Partner.RandomizeAffiliations}");
			await sw.WriteLineAsync($"Keep Affiliations At Same Locations: {settings.Layout.Partner.KeepAffiliationsAtSameLocation}");
			await sw.WriteLineAsync($"Only Selected Partners: {settings.Layout.Partner.OnlySelectedPartners}");
			if (settings.Layout.Partner.OnlySelectedPartners)
			{
				await sw.WriteLineAsync("-Selected Partners-");
				foreach (var partner in settings.Layout.Partner.SelectedPartners)
					await sw.WriteLineAsync($"{partner}");
			}
		}

		await sw.WriteLineAsync("---- Subtitles ----");
		await sw.WriteLineAsync($"Randomize Subtitles / Voicelines: {settings.Subtitles.Randomize}");
		await sw.WriteLineAsync($"Only With Linked Audio: {settings.Subtitles.OnlyLinkedAudio}");
		await sw.WriteLineAsync($"Give Audio to No Linked Audio Subtitles: {settings.Subtitles.GiveAudioToNoLinkedAudio}");
		await sw.WriteLineAsync($"No System Messages: {settings.Subtitles.NoSystemMessages}");
		await sw.WriteLineAsync($"No Duplicates: {settings.Subtitles.NoDuplicates}");
		await sw.WriteLineAsync($"Only Selected Characters: {settings.Subtitles.OnlySelectedCharacters}");
		if (settings.Subtitles.OnlySelectedCharacters)
		{
			await sw.WriteLineAsync("-Selected Characters-");
			foreach (var character in settings.Subtitles.SelectedCharacters)
				await sw.WriteLineAsync($"{character}");
		}

		await sw.WriteLineAsync("---- Music ----");
		await sw.WriteLineAsync($"Randomize Music: {settings.Music.Randomize}");
		await sw.WriteLineAsync($"Skip Chaos Power Use Jingles: {settings.Music.SkipChaosPowers}");
		await sw.WriteLineAsync($"Skip Rank Theme: {settings.Music.SkipRankTheme}");
		await sw.WriteLineAsync();
		await sw.WriteLineAsync("---- Models ----");
		await sw.WriteLineAsync($"Randomize Player Model: {settings.Models.Randomize}");
		await sw.WriteLineAsync($"Randomize Player 2's Model: {settings.Models.RandomizeP2}");
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

	private void Spoilers_ListBox_LevelList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (Spoilers_ListBox_LevelList.SelectedIndex == -1) return;
		var sb = new StringBuilder();
		Stage stg = stages[stageids[Spoilers_ListBox_LevelList.SelectedIndex]];
		switch (settings.LevelOrder.Mode)
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

	private void Levels_Button_ToggleAll_Click(object? sender, RoutedEventArgs e)
	{
		bool value = !LevelCheckBoxes.Any(a => a.IsChecked.Value);
		foreach (var stg in LevelCheckBoxes)
			stg.IsChecked = value;
	}

	Levels[] bossLevels = [
		Levels.BlackBullDR,
		Levels.BlackBullLH,
		Levels.BlackDoomCF,
		Levels.BlackDoomFH,
		Levels.BlackDoomGF,
		Levels.BlueFalcon,
		Levels.DevilDoom,
		Levels.EggBreakerCC,
		Levels.EggBreakerIJ,
		Levels.EggBreakerMM,
		Levels.EggDealerBC,
		Levels.EggDealerCF,
		Levels.EggDealerLS,
		Levels.HeavyDog,
		Levels.SonicDiablonBC,
		Levels.SonicDiablonFH,
		Levels.SonicDiablonGF
	];
	private void Levels_Button_ToggleBosses_Click(object? sender, RoutedEventArgs e)
	{
		bool value = !bossLevels.Any(b => LevelCheckBoxes[(int)b].IsChecked.Value);
		foreach (var stg in bossLevels)
			LevelCheckBoxes[(int)stg].IsChecked = value;
	}

	Levels[] lastLevels = [
		Levels.TheLastWay,
		Levels.DevilDoom
	];
	private void Levels_Button_ToggleLastStory_Click(object? sender, RoutedEventArgs e)
	{
		bool value = !lastLevels.Any(b => LevelCheckBoxes[(int)b].IsChecked.Value);
		foreach (var stg in lastLevels)
			LevelCheckBoxes[(int)stg].IsChecked = value;
	}

	private void UpdateProgressBar(int value)
	{
		ProgressBar_RandomizationProgress.Value = value;
	}

	private void UpdateUIEnabledState()
	{
		if (!programInitialized)
			return;
		// Level Order
		LevelOrder_TextBox_Seed.IsEnabled = !LevelOrder_CheckBox_Random_Seed.IsChecked.Value;
		// --Layout--
		// Enemy
		Layout_CheckBox_MakeCCSplinesVehicleCompatible.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Enemy_ComboBox_Mode.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Enemy_CheckBox_AdjustMissionCounts.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex == LayoutEnemyMode.Wild;
		Layout_Enemy_CheckBox_KeepType.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex == LayoutEnemyMode.Wild;
		Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex == LayoutEnemyMode.Wild;
		for (int i = 0; i < EnemyCheckBoxes.Length; i++)
			EnemyCheckBoxes[i].IsEnabled = Layout_Enemy_CheckBox_OnlySelectedEnemyTypes.IsChecked.Value && Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutEnemyMode)Layout_Enemy_ComboBox_Mode.SelectedIndex == LayoutEnemyMode.Wild;
		// Weapon
		Layout_Weapon_CheckBox_RandomWeaponsInWeaponBoxes.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Weapon_CheckBox_RandomWeaponsInAllBoxes.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Weapon_CheckBox_RandomExposedWeapons.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Weapon_CheckBox_RandomWeaponsFromEnvironment.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Weapon_CheckBox_OnlySelectedWeapons.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		for (int i = 0; i < WeaponCheckBoxes.Length; i++)
			if (WeaponCheckBoxes[i] != null)
				WeaponCheckBoxes[i].IsEnabled = Layout_Weapon_CheckBox_OnlySelectedWeapons.IsChecked.Value && Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		// Partner
		Layout_Partner_ComboBox_Mode.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value;
		Layout_Partner_CheckBox_RandomizeAffiliations.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild;
		Layout_Partner_CheckBox_KeepAffiliationsAtSameLocation.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild;
		Layout_Partner_CheckBox_OnlySelectedPartners.IsEnabled = Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild;
		for (int i = 0; i < PartnerCheckBoxes.Length; i++)
			PartnerCheckBoxes[i].IsEnabled = Layout_Partner_CheckBox_OnlySelectedPartners.IsChecked.Value && Layout_CheckBox_RandomizeLayouts.IsChecked.Value && (LayoutPartnerMode)Layout_Partner_ComboBox_Mode.SelectedIndex == LayoutPartnerMode.Wild;
		// --End Layout--
		// Subtitles
		Subtitles_CheckBox_OnlyWithLinkedAudio.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;	
		Subtitles_CheckBox_GiveAudioToNoLinkedAudioSubtitles.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		Subtitles_CheckBox_NoSystemMessages.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		Subtitles_CheckBox_NoDuplicates.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		Subtitles_CheckBox_GenerateMessages.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		Subtitles_CheckBox_OnlySelectedCharacters.IsEnabled = Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		for (int i = 0; i < SubtitleCheckBoxes.Length; i++)
			SubtitleCheckBoxes[i].IsEnabled = Subtitles_CheckBox_OnlySelectedCharacters.IsChecked.Value && Subtitles_CheckBox_RandomizeSubtitlesVoicelines.IsChecked.Value;
		// Music
		Music_CheckBox_SkipRankTheme.IsEnabled = Music_CheckBox_RandomizeMusic.IsChecked.Value;
		Music_CheckBox_SkipChaosPowerUseJingles.IsEnabled = Music_CheckBox_RandomizeMusic.IsChecked.Value;
	}

	private void Shared_CheckBox_UIUpdate_OnClick(object? sender, RoutedEventArgs e)
	{
		if (programInitialized)
			UpdateUIEnabledState();
	}

	private void Shared_ComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (programInitialized)
			UpdateUIEnabledState();
	}
}
