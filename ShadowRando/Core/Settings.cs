using IniFile;
using System.IO;

namespace ShadowRando
{
	public class Settings
	{
		[IniAlwaysInclude]
		public bool ProgramSound { get; set; } = true;
		[IniAlwaysInclude]
		public int ProgramTheme { get; set; } = 0;
		[IniAlwaysInclude]
		public string GamePath { get; set; }
		[IniAlwaysInclude]
		public string Seed { get; set; }
		[IniAlwaysInclude]
		public bool RandomSeed { get; set; }
		[IniAlwaysInclude]
		public LevelOrderMode LevelOrderMode { get; set; }
		[IniAlwaysInclude]
		public LevelOrderMainPath LevelOrderMainPath { get; set; }
		[System.ComponentModel.DefaultValue(22)]
		[IniAlwaysInclude]
		public int LevelOrderMaxForwardsJump { get; set; } = 22;
		[System.ComponentModel.DefaultValue(4)]
		[IniAlwaysInclude]
		public int LevelOrderMaxBackwardsJump { get; set; } = 4;
		[System.ComponentModel.DefaultValue(10)]
		[IniAlwaysInclude]
		public int LevelOrderBackwardsJumpProbability { get; set; } = 10;
		[IniAlwaysInclude]
		public bool LevelOrderAllowJumpsToSameLevel { get; set; }
		[IniAlwaysInclude]
		public bool LevelOrderIncludeLastStory { get; set; }
		[System.ComponentModel.DefaultValue(true)]
		[IniAlwaysInclude]
		public bool LevelOrderIncludeBosses { get; set; } = true;
		[IniAlwaysInclude]
		public bool RandomizeMusic { get; set; }
		[IniAlwaysInclude]
		public bool MusicSkipRankTheme { get; set; }
		[IniAlwaysInclude]
		public bool MusicSkipChaosPowers { get; set; }
		public bool RandomizeSubtitlesVoicelines { get; set; }
		[IniAlwaysInclude]
		public bool RandomizeLayouts { get; set; }
		// SET
		[IniAlwaysInclude]
		public LayoutEnemyMode LayoutEnemyMode { get; set; }
		//public SETMode
		[IniAlwaysInclude]
		public bool LayoutEnemyKeepType { get; set; }
		[IniAlwaysInclude]
		public LayoutPartnerMode LayoutPartnerMode { get; set; }
		[IniAlwaysInclude]
		public bool LayoutRandomWeaponsInAllBoxes { get; set; }
		[IniAlwaysInclude]
		public bool LayoutMakeCCSplinesVehicleCompatible { get; set; }
		[IniAlwaysInclude]
		public bool LayoutAdjustMissionCounts { get; set; }
		// FNT
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
		// FNT Specific Chars Section
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


		public static Settings Load()
		{
			if (File.Exists("RandoSettings.ini"))
				return IniSerializer.Deserialize<Settings>("RandoSettings.ini");
			return new Settings();
		}

		public void Save()
		{
			IniSerializer.Serialize(this, "RandoSettings.ini");
		}
	}
}
