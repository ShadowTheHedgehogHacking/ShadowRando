using ShadowSET;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ShadowRando.Core
{
	public class Settings
	{
		[IniAlwaysInclude]
		public string GamePath;
		[IniAlwaysInclude]
		public string Seed;
		[IniAlwaysInclude]
		public bool RandomSeed;
		[IniAlwaysInclude]
		public SettingsLevelOrder LevelOrder = new();
		[IniAlwaysInclude]
		public SettingsLayout Layout = new();
		[IniAlwaysInclude]
		public SettingsSubtitles Subtitles = new();
		[IniAlwaysInclude]
		public SettingsMusic Music = new();
		[IniAlwaysInclude]
		public SettingsModels Models = new();
		[IniAlwaysInclude]
		public SettingsSpoilers Spoilers = new();

		public static Settings Load()
		{
			if (File.Exists("RandoSettings.ini"))
			{
				var result = IniSerializer.Deserialize<Settings>("RandoSettings.ini");
				if (result.LevelOrder == null)
					result.LevelOrder = new();
				else
					result.LevelOrder.ExcludeLevels ??= [];
				if (result.Layout == null)
					result.Layout = new();
				else
				{
					if (result.Layout.Enemy == null)
						result.Layout.Enemy = new();
					else
						result.Layout.Enemy.SelectedEnemies ??= [];
					if (result.Layout.Weapon == null)
						result.Layout.Weapon = new();
					else
						result.Layout.Weapon.SelectedWeapons ??= [];
					if (result.Layout.Partner == null)
						result.Layout.Partner = new();
					else
						result.Layout.Partner.SelectedPartners ??= [];
				}
				if (result.Subtitles == null)
					result.Subtitles = new();
				else
					result.Subtitles.SelectedCharacters ??= [];
				result.Music ??= new();
				result.Models ??= new();
				result.Spoilers ??= new SettingsSpoilers();
				return result;
			}
			return new Settings();
		}

		public void Save()
		{
			if (LevelOrder.ExcludeLevels.Count == 0)
				LevelOrder.ExcludeLevels = null;
			if (Layout.Enemy.SelectedEnemies.Count == 0)
				Layout.Enemy.SelectedEnemies = null;
			if (Layout.Weapon.SelectedWeapons.Count == 0)
				Layout.Weapon.SelectedWeapons = null;
			if (Layout.Partner.SelectedPartners.Count == 0)
				Layout.Partner.SelectedPartners = null;
			if (Subtitles.SelectedCharacters.Count == 0)
				Subtitles.SelectedCharacters = null;
			IniSerializer.Serialize(this, "RandoSettings.ini");
			LevelOrder.ExcludeLevels ??= [];
			Layout.Enemy.SelectedEnemies ??= [];
			Layout.Weapon.SelectedWeapons ??= [];
			Layout.Partner.SelectedPartners ??= [];
			Subtitles.SelectedCharacters ??= [];
		}
	}

	public class SettingsLevelOrder
	{
		[IniAlwaysInclude]
		public LevelOrderMode Mode = LevelOrderMode.AllStagesWarps;
		[IniAlwaysInclude]
		public LevelOrderMainPath MainPath;
		[DefaultValue(22)]
		[IniAlwaysInclude]
		public int MaxForwardsJump = 22;
		[DefaultValue(4)]
		[IniAlwaysInclude]
		public int MaxBackwardsJump = 4;
		[DefaultValue(10)]
		[IniAlwaysInclude]
		public int BackwardsJumpProbability = 10;
		[IniAlwaysInclude]
		public bool AllowJumpsToSameLevel;
		[DefaultValue(false)]
		[IniAlwaysInclude]
		public bool AllowBossToBoss = false;
		[IniAlwaysInclude]
		public bool ExpertMode;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<Levels> ExcludeLevels = [];
	}

	public class SettingsLayout
	{
		[IniAlwaysInclude]
		public bool Randomize;
		[IniAlwaysInclude]
		public bool MakeCCSplinesVehicleCompatible;
		[IniAlwaysInclude]
		public SettingsLayoutEnemy Enemy = new SettingsLayoutEnemy();
		[IniAlwaysInclude]
		public SettingsLayoutWeapon Weapon = new SettingsLayoutWeapon();
		[IniAlwaysInclude]
		public SettingsLayoutPartner Partner = new SettingsLayoutPartner();
		[IniAlwaysInclude]
		public SettingsLayoutMisc Misc = new SettingsLayoutMisc();
	}

	public class SettingsLayoutEnemy
	{
		[IniAlwaysInclude]
		public bool AdjustMissionCounts = true;
		[DefaultValue(20)]
		[IniAlwaysInclude]
		public int AdjustMissionCountsReductionPercent = 20;
		[IniAlwaysInclude]
		public LayoutEnemyMode Mode;
		[IniAlwaysInclude]
		public bool KeepType = true;
		[IniAlwaysInclude]
		public bool OnlySelectedTypes;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<EnemyTypes> SelectedEnemies = [];
	}

	public class SettingsLayoutWeapon
	{
		[IniAlwaysInclude]
		public bool RandomWeaponsInAllBoxes;
		[IniAlwaysInclude]
		public bool RandomWeaponsInWeaponBoxes = true;
		[IniAlwaysInclude]
		public bool RandomExposedWeapons = true;
		[IniAlwaysInclude]
		public bool RandomWeaponsFromEnvironment;
		[IniAlwaysInclude]
		public bool OnlySelectedTypes;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<EWeapon> SelectedWeapons = [];
	}

	public class SettingsLayoutPartner
	{
		[IniAlwaysInclude]
		public LayoutPartnerMode Mode;
		[IniAlwaysInclude]
		public bool RandomizeAffiliations = true;
		[IniAlwaysInclude]
		public bool KeepAffiliationsAtOriginalLocation = true;
		[IniAlwaysInclude]
		public bool OnlySelectedPartners;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<Object0190_Partner.EPartner> SelectedPartners = [];
	}

	public class SettingsLayoutMisc
	{
		[IniAlwaysInclude]
		public bool RandomItemCapsules;
		[IniAlwaysInclude]
		public bool RandomItemBalloons;
		[IniAlwaysInclude]
		public bool PseudoRandomCreamCheeseLocations;
		[IniAlwaysInclude]
		public bool PseudoRandomCreamCheeseLocationsTreasureHunt;
		[IniAlwaysInclude]
		public bool DestructiblesToEnemies;
		[IniAlwaysInclude]
		public bool MadMatrixRandomizedBombLocations;
	}

	public class SettingsSubtitles
	{
		[IniAlwaysInclude]
		public bool Randomize;
		[IniAlwaysInclude]
		public bool NoDuplicates = true;
		[IniAlwaysInclude]
		public bool NoSystemMessages = true;
		[IniAlwaysInclude]
		public bool ShutUpCharmy;
		[IniAlwaysInclude]
		public bool OnlyLinkedAudio = true;
		[IniAlwaysInclude]
		public bool OnlySelectedCharacters;
		[IniAlwaysInclude]
		public bool GiveAudioToNoLinkedAudio;
		[IniAlwaysInclude]
		public bool GenerateMessages;
		[IniAlwaysInclude]
		[DefaultValue(2)]
		public int MarkovLevel = 2;
		[IniCollection(IniCollectionMode.SingleLine, Format = ", ")]
		public List<SubtitleCharacters> SelectedCharacters = [];
	}

	public class SettingsMusic
	{
		[IniAlwaysInclude]
		public bool Randomize;
		[IniAlwaysInclude]
		public bool SkipRankTheme = true;
		[IniAlwaysInclude]
		public bool SkipChaosPowers;
	}

	public class SettingsModels
	{
		[IniAlwaysInclude]
		public bool Randomize;
		[IniAlwaysInclude]
		public bool RandomizeP2;
	}

	public class SettingsSpoilers
	{
		[IniAlwaysInclude]
		public bool GraphUseIcons;
		[IniAlwaysInclude]
		public bool AutosaveLog = true;
	}
}
