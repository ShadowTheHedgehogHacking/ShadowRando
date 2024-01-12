namespace ShadowRando
{
	public enum LevelOrderMode
	{
		None,
		AllStagesWarps,
		VanillaStructure,
		BranchingPaths,
		ReverseBranching,
		BossRush,
		Wild
	}

	public enum LevelOrderMainPath
	{
		ActClear,
		AnyExit
	}

	public enum LayoutEnemyMode
	{
		None,
		Wild,
		OneToOnePerStage,
		OneToOneGlobal
	}

	public enum LayoutPartnerMode
	{
		None,
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

	public enum StageType { Neutral, Hero, Dark, End }

	public enum ConnectionType { Neutral, Hero, Dark }

	public enum Direction { Left, Top, Right, Bottom }
}
