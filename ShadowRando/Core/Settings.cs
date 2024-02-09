using System.Collections.Generic;
using System.IO;

namespace ShadowRando.Core
{
	public class Settings
	{
		[IniAlwaysInclude]
		public int ProgramTheme = 2;
		[IniAlwaysInclude]
		public string GamePath;
		[IniAlwaysInclude]
		public string Seed;
		[IniAlwaysInclude]
		public bool RandomSeed;
		[IniAlwaysInclude]
		public LevelOrderMode LevelOrderMode = LevelOrderMode.AllStagesWarps;
		[IniAlwaysInclude]
		public LevelOrderMainPath LevelOrderMainPath;
		[System.ComponentModel.DefaultValue(22)]
		[IniAlwaysInclude]
		public int LevelOrderMaxForwardsJump = 22;
		[System.ComponentModel.DefaultValue(4)]
		[IniAlwaysInclude]
		public int LevelOrderMaxBackwardsJump = 4;
		[System.ComponentModel.DefaultValue(10)]
		[IniAlwaysInclude]
		public int LevelOrderBackwardsJumpProbability = 10;
		[IniAlwaysInclude]
		public bool LevelOrderAllowJumpsToSameLevel;
		[System.ComponentModel.DefaultValue(true)]
		[IniAlwaysInclude]
		public bool LevelOrderAllowBossToBoss = true;
		[IniAlwaysInclude]
		public bool LevelOrderExpertMode;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<Levels> ExcludeLevels = new List<Levels>();

		// Layout
		[IniAlwaysInclude]
		public bool RandomizeLayouts;
		[IniAlwaysInclude]
		public bool LayoutMakeCCSplinesVehicleCompatible;

		// Enemy
		[IniAlwaysInclude]
		public bool LayoutAdjustMissionCounts;
		[System.ComponentModel.DefaultValue(20)]
		[IniAlwaysInclude]
		public int LayoutAdjustMissionCountsReductionPercent = 20;
		[IniAlwaysInclude]
		public LayoutEnemyMode LayoutEnemyMode;
		[IniAlwaysInclude]
		public bool LayoutEnemyKeepType;
		[IniAlwaysInclude]
		public bool LayoutEnemyOnlySelectedTypes;

		// Enemy Selected Types
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyGUNSoldier;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyGUNBeetle;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyGUNBigfoot;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyGUNRobot;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyEggPierrot;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyEggPawn;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyShadowAndroid;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBAGiant;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBASoldier;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBAHawkVolt;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBAWing;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBAWorm;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBALarva;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyArtificialChaos;
		[IniAlwaysInclude]
		public bool LayoutEnemySelectedEnemyBAAssassin;

		// Weapon
		[IniAlwaysInclude]
		public bool LayoutWeaponRandomWeaponsInAllBoxes;
		[IniAlwaysInclude]
		public bool LayoutWeaponRandomWeaponsInWeaponBoxes;
		[IniAlwaysInclude]
		public bool LayoutWeaponRandomExposedWeapons;
		[IniAlwaysInclude]
		public bool LayoutWeaponRandomWeaponsFromEnvironment;
		[IniAlwaysInclude]
		public bool LayoutWeaponOnlySelectedTypes;

		// Weapon Selected Types
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponNone;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponPistol;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSubmachineGun;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponAssaultRifle;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponHeavyMachineGun;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponGatlingGun;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEggPistol;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponLightShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponFlashShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponRingShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponHeavyShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponGrenadeLauncher;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponGUNBazooka;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponTankCannon;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponBlackBarrel;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponBigBarrel;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEggBazooka;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponRPG;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponFourShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEightShot;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponWormShooterBlack;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponWormShooterRed;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponWormShooterGold;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponVacuumPod;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponLaserRifle;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSplitter;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponRefractor;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponKnife;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponBlackSword;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponDarkHammer;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEggLance;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSamuraiSwordLv1;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSamuraiSwordLv2;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSatelliteLaserLv1;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponSatelliteLaserLv2;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEggVacuumLv1;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponEggVacuumLv2;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponOmochaoGunLv1;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponOmochaoGunLv2;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponHealCannonLv1;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponHealCannonLv2;
		[IniAlwaysInclude]
		public bool LayoutWeaponSelectedWeaponShadowRifle;

		// Partner
		[IniAlwaysInclude]
		public LayoutPartnerMode LayoutPartnerMode;
		[IniAlwaysInclude]
		public bool LayoutPartnerKeepOriginalObjectAffiliation;

		// Subtitles
		[IniAlwaysInclude]
		public bool RandomizeSubtitlesVoicelines;
		[IniAlwaysInclude]
		public bool SubtitlesNoDuplicates;
		[IniAlwaysInclude]
		public bool SubtitlesNoSystemMessages;
		[IniAlwaysInclude]
		public bool SubtitlesOnlyLinkedAudio;
		[IniAlwaysInclude]
		public bool SubtitlesOnlySelectedCharacters;
		[IniAlwaysInclude]
		public bool SubtitlesGiveAudioToNoLinkedAudio;
		// Subtitle Selected Characters
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterShadow;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterSonic;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterTails;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterKnuckles;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterAmy;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterRouge;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterOmega;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterVector;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterEspio;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterMaria;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterCharmy;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterEggman;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterCream;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterCheese;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterBlackDoom;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterGUNCommander;
		[IniAlwaysInclude]
		public bool SubtitlesSelectedCharacterGUNSoldier;

		// Music
		[IniAlwaysInclude]
		public bool RandomizeMusic;
		[IniAlwaysInclude]
		public bool MusicSkipRankTheme;
		[IniAlwaysInclude]
		public bool MusicSkipChaosPowers;

		// Models
		[IniAlwaysInclude]
		public bool RandomizeModel;
		[IniAlwaysInclude]
		public bool RandomizeP2Model;

		public static Settings Load()
		{
			if (File.Exists("RandoSettings.ini"))
				return IniSerializer.Deserialize<Settings>("RandoSettings.ini");
			return new Settings();
		}

		public void Save()
		{
			if (ExcludeLevels.Count == 0)
				ExcludeLevels = null;
			IniSerializer.Serialize(this, "RandoSettings.ini");
			if (ExcludeLevels == null)
				ExcludeLevels = new List<Levels>();
		}
	}
}
