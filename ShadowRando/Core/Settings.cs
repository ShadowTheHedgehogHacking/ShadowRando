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
		[System.ComponentModel.DefaultValue(4)]
		[IniAlwaysInclude]
		public int MaxBackJump { get; set; } = 4;
		[System.ComponentModel.DefaultValue(22)]
		[IniAlwaysInclude]
		public int MaxForwJump { get; set; } = 22;
		[System.ComponentModel.DefaultValue(10)]
		[IniAlwaysInclude]
		public int BackJumpProb { get; set; } = 10;
		[IniAlwaysInclude]
		public bool AllowSameLevel { get; set; }
		[IniAlwaysInclude]
		public bool IncludeLast { get; set; }
		[System.ComponentModel.DefaultValue(true)]
		[IniAlwaysInclude]
		public bool IncludeBosses { get; set; } = true;
		[IniAlwaysInclude]
		public bool RandomMusic { get; set; }
		[IniAlwaysInclude]
		public bool RandomMusicSkipRankTheme { get; set; }
		[IniAlwaysInclude]
		public bool RandomMusicSkipChaosPowers { get; set; }
		public bool RandomFNT { get; set; }
		[IniAlwaysInclude]
		public bool RandomSET { get; set; }
		// SET
		[IniAlwaysInclude]
		public LayoutEnemyMode LayoutEnemyMode { get; set; }
		//public SETMode
		[IniAlwaysInclude]
		public bool SETEnemyKeepType { get; set; }
		[IniAlwaysInclude]
		public bool SETRandomPartners { get; set; }
		[IniAlwaysInclude]
		public bool SETRandomWeaponsInBoxes { get; set; }
		[IniAlwaysInclude]
		public bool SETRandomMakeCCSplinesAWRidable { get; set; }
		[IniAlwaysInclude]
		public bool SETRandomAdjustMissionCounts { get; set; }
		// FNT
		[IniAlwaysInclude]
		public bool FNTNoDuplicatesPreRandomization;
		[IniAlwaysInclude]
		public bool FNTNoSystemMessages;
		[IniAlwaysInclude]
		public bool FNTOnlyLinkedAudio;
		[IniAlwaysInclude]
		public bool FNTSpecificCharacters;
		[IniAlwaysInclude]
		public bool FNTGiveAudioToNoLinkedAudio;
		// FNT Specific Chars Section
		[IniAlwaysInclude]
		public bool FNTShadowSelected;
		[IniAlwaysInclude]
		public bool FNTSonicSelected;
		[IniAlwaysInclude]
		public bool FNTTailsSelected;
		[IniAlwaysInclude]
		public bool FNTKnucklesSelected;
		[IniAlwaysInclude]
		public bool FNTAmySelected;
		[IniAlwaysInclude]
		public bool FNTRougeSelected;
		[IniAlwaysInclude]
		public bool FNTOmegaSelected;
		[IniAlwaysInclude]
		public bool FNTVectorSelected;
		[IniAlwaysInclude]
		public bool FNTEspioSelected;
		[IniAlwaysInclude]
		public bool FNTMariaSelected;
		[IniAlwaysInclude]
		public bool FNTCharmySelected;
		[IniAlwaysInclude]
		public bool FNTEggmanSelected;
		[IniAlwaysInclude]
		public bool FNTCreamSelected;
		[IniAlwaysInclude]
		public bool FNTCheeseSelected;
		[IniAlwaysInclude]
		public bool FNTBlackDoomSelected;
		[IniAlwaysInclude]
		public bool FNTGUNCommanderSelected;
		[IniAlwaysInclude]
		public bool FNTGUNSoldierSelected;


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
