using System.Diagnostics;
using System.Linq;

namespace ShadowRando.Core
{
	class Stage
	{
		public int ID { get; }
		public bool IsBoss { get; set; }
		public bool IsLast { get; set; }
		public bool HasNeutral { get; set; }
		public bool HasHero { get; set; }
		public bool HasDark { get; set; }
		public int Neutral { get; set; } = -1;
		public int Hero { get; set; } = -1;
		public int Dark { get; set; } = -1;

		public Stage(int id)
		{
			ID = id;
		}

		[DebuggerNonUserCode]
		public int GetExit(int exit)
		{
			if (IsBoss)
				return Neutral;
			else
			{
				if (HasNeutral && exit-- == 0)
					return Neutral;
				else if (HasHero && exit-- == 0)
					return Hero;
				else
					return Dark;
			}
		}

		[DebuggerNonUserCode]
		public void SetExit(int exit, int stage)
		{
			if (IsBoss)
				Neutral = stage;
			else
			{
				if (HasNeutral && exit-- == 0)
					Neutral = stage;
				else if (HasHero && exit-- == 0)
					Hero = stage;
				else
					Dark = stage;
			}
		}

		[DebuggerNonUserCode]
		public int CountExits()
		{
			if (IsBoss)
				return 1;
			int ex = 0;
			if (HasNeutral)
				ex++;
			if (HasHero)
				ex++;
			if (HasDark)
				ex++;
			return ex;
		}
	}

	readonly struct ShadowStageSet
	{
		public readonly System.Collections.ObjectModel.ReadOnlyCollection<ShadowStage> stages;
		public readonly int bossCount;

		public ShadowStageSet(params ShadowStage[] stages)
		{
			this.stages = new System.Collections.ObjectModel.ReadOnlyCollection<ShadowStage>(stages);
			bossCount = stages.Sum(a => a.bossCount);
		}

		public static readonly ShadowStageSet[] StageList = new[]
		{
			new ShadowStageSet(new ShadowStage(StageType.Neutral)),
			new ShadowStageSet(new ShadowStage(StageType.Dark), new ShadowStage(StageType.Neutral), new ShadowStage(StageType.Hero, 1)),
			new ShadowStageSet(new ShadowStage(StageType.Neutral, 1), new ShadowStage(StageType.Neutral), new ShadowStage(StageType.Neutral)),
			new ShadowStageSet(new ShadowStage(StageType.Dark), new ShadowStage(StageType.Neutral, 1), new ShadowStage(StageType.Neutral), new ShadowStage(StageType.Neutral, 1), new ShadowStage(StageType.Hero, 1)),
			new ShadowStageSet(new ShadowStage(StageType.Dark, 1), new ShadowStage(StageType.Neutral), new ShadowStage(StageType.Neutral, 1), new ShadowStage(StageType.Neutral), new ShadowStage(StageType.Hero)),
			new ShadowStageSet(new ShadowStage(StageType.End, 2), new ShadowStage(StageType.End, 2), new ShadowStage(StageType.End, 1), new ShadowStage(StageType.End, 2), new ShadowStage(StageType.End, 2)),
		};
	}

	readonly struct ShadowStage
	{
		public readonly StageType stageType;
		public readonly int bossCount;

		public ShadowStage(StageType stageType, int bossCount = 0)
		{
			this.stageType = stageType;
			this.bossCount = bossCount;
		}
	}
}
