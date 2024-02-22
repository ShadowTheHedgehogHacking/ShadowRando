namespace ShadowRando.Core
{
	public enum Levels
	{
		Westopolis,
		DigitalCircuit,
		GlyphicCanyon,
		LethalHighway,
		CrypticCastle,
		PrisonIsland,
		CircusPark,
		CentralCity,
		TheDoom,
		SkyTroops,
		MadMatrix,
		DeathRuins,
		TheARK,
		AirFleet,
		IronJungle,
		SpaceGadget,
		LostImpact,
		GUNFortress,
		BlackComet,
		LavaShelter,
		CosmicFall,
		FinalHaunt,
		TheLastWay,
		BlackBullLH,
		EggBreakerCC,
		HeavyDog,
		EggBreakerMM,
		BlackBullDR,
		BlueFalcon,
		EggBreakerIJ,
		BlackDoomGF,
		SonicDiablonGF,
		EggDealerBC,
		SonicDiablonBC,
		EggDealerLS,
		EggDealerCF,
		BlackDoomCF,
		BlackDoomFH,
		SonicDiablonFH,
		DevilDoom,
		Ending
	}

	public enum LevelOrderMode
	{
		Original,
		AllStagesWarps,
		VanillaStructure,
		VanillaStructureNoBosses,
		BranchingPaths,
		ReverseBranching,
		BossRush,
		SafeWild,
		Wild
	}

	public enum LevelOrderMainPath
	{
		ActClear,
		AnyExit
	}

	public enum LayoutEnemyMode
	{
		Original,
		Wild,
		OneToOnePerStage,
		OneToOneGlobal
	}

	public enum LayoutPartnerMode
	{
		Original,
		Wild,
		OneToOnePerStage,
		OneToOneGlobal
	}

	public enum MusicCategory
	{
		Stage,
		Jingle,
		Menu,
		Credits
	}

	public enum EnemyTypes
	{
		GUNSoldier,
		GUNBeetle,
		GUNBigfoot,
		GUNRobot,
		EggPierrot,
		EggPawn,
		ShadowAndroid,
		BAGiant,
		BASoldier,
		BAHawkVolt,
		BAWing,
		BAWorm,
		BALarva,
		ArtificialChaos,
		BAAssassin
	}

	public enum SubtitleCharacters
	{
		Shadow,
		Sonic,
		Tails,
		Knuckles,
		Amy,
		Rouge,
		Omega,
		Vector,
		Espio,
		Maria,
		Charmy,
		Eggman,
		BlackDoom,
		Cream,
		Cheese,
		GUNCommander,
		GUNSoldier
	}

public enum StageType { Neutral, Hero, Dark, End }

	public enum ConnectionType { Neutral, Hero, Dark }

	public enum Direction { Left, Top, Right, Bottom }
}
