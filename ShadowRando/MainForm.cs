using AFSLib;
using IniFile;
using ShadowFNT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShadowRando
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

        const string programVersion = "0.3.0-dev";
        Settings settings;

		private void MainForm_Load(object sender, EventArgs e)
		{
			settings = Settings.Load();
			Text += programVersion;
			seedSelector.Value = settings.Seed;
			randomSeed.Checked = settings.RandomSeed;
			modeSelector.SelectedIndex = (int)settings.Mode;
			mainPathSelector.SelectedIndex = (int)settings.MainPath;
			maxBackJump.Value = settings.MaxBackJump;
			maxForwJump.Value = settings.MaxForwJump;
			backJumpProb.Value = settings.BackJumpProb;
			allowSameLevel.Checked = settings.AllowSameLevel;
			includeLast.Checked = settings.IncludeLast;
			includeBosses.Checked = settings.IncludeBosses;
			randomMusic.Checked = settings.RandomMusic;
			randomFNT.Checked = settings.RandomFNT;
			using (var dlg = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog() { Description = "Select the root folder of an extracted Shadow the Hedgehog disc image." })
			{
				if (!string.IsNullOrEmpty(settings.GamePath))
					dlg.SelectedPath = settings.GamePath;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (settings.GamePath != dlg.SelectedPath && Directory.Exists("backup"))
						switch (MessageBox.Show(this, "New game directory selected!\n\nDo you wish to erase the previous backup data and use the new data as a base?", "Shadow Randomizer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
						{
							case DialogResult.Yes:
								Directory.Delete("backup", true);
								break;
							case DialogResult.No:
								break;
							default:
								Close();
								return;
						}
					settings.GamePath = dlg.SelectedPath;
					if (!Directory.Exists("backup"))
						Directory.CreateDirectory("backup");
					if (!File.Exists(Path.Combine("backup", "main.dol")))
						File.Copy(Path.Combine(settings.GamePath, "sys", "main.dol"), Path.Combine("backup", "main.dol"));
					if (!Directory.Exists(Path.Combine("backup", "fonts")))
						CopyDirectory(Path.Combine(settings.GamePath, "files", "fonts"), Path.Combine("backup", "fonts"));
					if (!Directory.Exists(Path.Combine("backup", "music")))
					{
						Directory.CreateDirectory(Path.Combine("backup", "music"));
						foreach (var fil in Directory.EnumerateFiles(Path.Combine(settings.GamePath, "files"), "*.adx"))
							File.Copy(fil, Path.Combine("backup", "music", Path.GetFileName(fil)));
					}
				}
				else
				{
					Close();
					return;
				}
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

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			settings.Seed = (int)seedSelector.Value;
			settings.RandomSeed = randomSeed.Checked;
			settings.Mode = (Modes)modeSelector.SelectedIndex;
			settings.MainPath = (MainPath)mainPathSelector.SelectedIndex;
			settings.MaxBackJump = (int)maxBackJump.Value;
			settings.MaxForwJump = (int)maxForwJump.Value;
			settings.BackJumpProb = (int)backJumpProb.Value;
			settings.AllowSameLevel = allowSameLevel.Checked;
			settings.IncludeLast = includeLast.Checked;
			settings.IncludeBosses = includeBosses.Checked;
			settings.RandomMusic = randomMusic.Checked;
			settings.RandomFNT = randomFNT.Checked;
			settings.Save();
		}

		private void randomSeed_CheckedChanged(object sender, EventArgs e)
		{
			seedSelector.Enabled = !randomSeed.Checked;
		}

		private void modeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			panel1.Enabled = modeSelector.SelectedIndex == 0;
		}

		private void allowSameLevel_CheckedChanged(object sender, EventArgs e)
		{
			maxBackJump.Minimum = maxForwJump.Minimum = allowSameLevel.Checked ? 0 : 1;
		}

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
		private void randomizeButton_Click(object sender, EventArgs e)
		{
			byte[] dolfile = File.ReadAllBytes(Path.Combine("backup", "main.dol"));
			int seed;
			if (randomSeed.Checked)
			{
				seed = (int)DateTime.Now.Ticks;
				seedSelector.Value = seed;
			}
			else
				seed = (int)seedSelector.Value;
			settings.Mode = (Modes)modeSelector.SelectedIndex;
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
				if (!includeLast.Checked)
					include = !stages[i].IsLast;
				if (settings.Mode == Modes.BossRush)
					include &= stages[i].IsBoss;
				else if (!includeBosses.Checked)
					include &= !stages[i].IsBoss;
				if (include)
					tmpids.Add(i);
			}
			stagecount = tmpids.Count;
			tmpids.Add(totalstagecount);
			stageids = tmpids.ToArray();
			switch (settings.Mode)
			{
				case Modes.AllStagesWarps:
					{
						Shuffle(r, stageids, stagecount);
						switch ((MainPath)mainPathSelector.SelectedIndex)
						{
							case MainPath.ActClear:
								for (int i = 0; i < stagecount; i++)
									stages[stageids[i]].SetExit(0, stageids[i + 1]);
								break;
							case MainPath.AnyExit:
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
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Neutral = stageids[r.Next(min, max)];
							}
							if (stg.HasHero && stg.Hero == -1)
							{
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Hero = stageids[r.Next(min, max)];
							}
							if (stg.HasDark && stg.Dark == -1)
							{
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Dark = stageids[r.Next(min, max)];
							}
						}
					}
					break;
				case Modes.VanillaStructure:
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
							if (includeBosses.Checked)
								for (int i = 0; i < set.bossCount; i++)
									neword.Add(bossq.Dequeue());
						}
						neword.AddRange(last);
						int ind = 0;
						foreach (var set in ShadowStageSet.StageList)
						{
							int bossind = ind + set.stages.Count;
							int next = set.stages.Count + (includeBosses.Checked ? set.bossCount : 0);
							if (set.stages[0].stageType == StageType.Neutral)
								++next;
							foreach (var item in set.stages)
							{
								Stage stg = stages[neword[ind]];
								int bosscnt = includeBosses.Checked ? item.bossCount : 0;
								if (bosscnt == 2)
								{
									stg.Dark = neword[bossind];
									stages[neword[bossind++]].Neutral = totalstagecount;
									stg.Hero = neword[bossind];
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
											bossstg.Dark = neword[ind + next];
											if (stg.HasNeutral)
												bossstg.Neutral = neword[ind + next + 1];
											else
												bossstg.Hero = neword[ind + next + 1];
											break;
										case StageType.Hero:
											bossstg.Hero = neword[ind + next];
											if (stg.HasNeutral)
												bossstg.Neutral = neword[ind + next - 1];
											else
												bossstg.Dark = neword[ind + next - 1];
											break;
										case StageType.End:
											bossstg.Hero = totalstagecount;
											bossstg.Dark = totalstagecount;
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
											stg.Dark = neword[ind + next];
											if (stg.HasNeutral)
												stg.Neutral = neword[ind + next + 1];
											else
												stg.Hero = neword[ind + next + 1];
											break;
										case StageType.Hero:
											stg.Hero = neword[ind + next];
											if (stg.HasNeutral)
												stg.Neutral = neword[ind + next - 1];
											else
												stg.Dark = neword[ind + next - 1];
											break;
										case StageType.End:
											stg.Hero = totalstagecount;
											stg.Dark = totalstagecount;
											break;
									}
								}
								++ind;
							}
							if (includeBosses.Checked)
								ind += set.bossCount;
						}
						neword.CopyTo(stageids);
					}
					break;
				case Modes.BranchingPaths:
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
						foreach (int item in curset)
							stages[item].Neutral = totalstagecount;
						ids2.CopyTo(stageids);
					}
					break;
				case Modes.ReverseBranching:
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
							if (!includeLast.Checked && stg.IsLast)
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
				case Modes.BossRush:
					Shuffle(r, stageids, stagecount);
					for (int i = 0; i < stagecount; i++)
						stages[stageids[i]].Neutral = stageids[i + 1];
					break;
				case Modes.Wild:
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
			File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "main.dol"), dolfile);
			if (randomMusic.Checked)
			{
				Dictionary<MusicCategory, List<string>> musicFiles = new Dictionary<MusicCategory, List<string>>()
				{
					{ MusicCategory.Stage, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_stg*.adx")) },
					{ MusicCategory.Jingle, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_jin*.adx")) },
					{ MusicCategory.Menu, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_sys*.adx")) },
					{ MusicCategory.Credits, new List<string>(Directory.EnumerateFiles(Path.Combine("backup", "music"), "sng_vox*.adx")) }
				};
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

			if (randomFNT.Checked)
				RandomizeFNTs(r);

			spoilerLevelList.BeginUpdate();
			spoilerLevelList.Items.Clear();
			for (int i = 0; i < stagecount; i++)
				spoilerLevelList.Items.Add(GetStageName(stageids[i]));
			spoilerLevelList.EndUpdate();
			spoilerLevelList.Enabled = true;
			spoilerLevelList.SelectedIndex = 0;
			saveLogButton.Enabled = true;
			makeChartButton.Enabled = true;
			MessageBox.Show("Randomization Complete", "Report");
		}

		private void RandomizeFNTs(Random r)
		{
			var fontAndAudioData = LoadFNTsAndAFS(true);
			for (int i = 0; i < fontAndAudioData.mutatedFnt.Count; i++)
			{
				for (int j = 0; j < fontAndAudioData.mutatedFnt[i].GetEntryTableCount(); j++)
				{
					// for now we simply swap everything without caring. We probably have to be careful about final entry etc.
					// Chained entries not accounted for, so may produce wacky results
					int donorFNTIndex = r.Next(0, fontAndAudioData.mutatedFnt.Count - 1);
					int donotFNTEntryIndex = r.Next(0, fontAndAudioData.initialFntState[donorFNTIndex].GetEntryTableCount() - 1);
					if (fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donotFNTEntryIndex) == -1)
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
				try
				{
					fnt.RecomputeAllSubtitleAddresses();
					string outfn = Path.Combine(settings.GamePath, "files", fnt.fileName.Substring(fnt.fileName.IndexOf("fonts")));
					File.WriteAllBytes(outfn, fnt.ToBytes());
					string prec = outfn.Remove(outfn.Length - 4);
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "res/EN.txd", prec + ".txd", true);
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "res/EN00.met", prec + "00.met", true);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed on " + fnt.ToString(), "An Exception Occurred");
					MessageBox.Show(ex.Message, "An Exception Occurred");
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

		private void spoilerLevelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (spoilerLevelList.SelectedIndex != -1)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				Stage stg = stages[stageids[spoilerLevelList.SelectedIndex]];
				switch (settings.Mode)
				{
					case Modes.AllStagesWarps:
						if (stg.Neutral != -1)
							sb.AppendLine($"Neutral -> {GetStageName(stg.Neutral)} ({Array.IndexOf(stageids, stg.Neutral) - spoilerLevelList.SelectedIndex:+##;-##;0})");
						if (stg.Hero != -1)
							sb.AppendLine($"Hero -> {GetStageName(stg.Hero)} ({Array.IndexOf(stageids, stg.Hero) - spoilerLevelList.SelectedIndex:+##;-##;0})");
						if (stg.Dark != -1)
							sb.AppendLine($"Dark -> {GetStageName(stg.Dark)} ({Array.IndexOf(stageids, stg.Dark) - spoilerLevelList.SelectedIndex:+##;-##;0})");
						sb.Append("Shortest Path: ");
						int[] shortestPath;
						if (maxForwJump.Value < 2)
						{
							shortestPath = new int[stagecount - 1 - spoilerLevelList.SelectedIndex];
							Array.Copy(stageids, spoilerLevelList.SelectedIndex, shortestPath, 0, shortestPath.Length);
						}
						else
							shortestPath = FindShortestPath(stageids[spoilerLevelList.SelectedIndex]);
						for (int i = 0; i < shortestPath.Length - 1; i++)
						{
							string exit;
							if (stages[shortestPath[i]].Neutral == shortestPath[i + 1])
								exit = "Neutral";
							else if (stages[shortestPath[i]].Hero == shortestPath[i + 1])
								exit = "Hero";
							else
								exit = "Dark";
							sb.AppendFormat("{0} ({1}) -> ", GetStageName(shortestPath[i]), exit);
						}
						sb.AppendFormat("Ending ({0} levels)", shortestPath.Length);
						break;
					case Modes.ReverseBranching:
						if (stg.Neutral != -1)
							sb.AppendLine($"Neutral -> {GetStageName(stg.Neutral)}");
						if (stg.Hero != -1)
							sb.AppendLine($"Hero -> {GetStageName(stg.Hero)}");
						if (stg.Dark != -1)
							sb.AppendLine($"Dark -> {GetStageName(stg.Dark)}");
						sb.Append("Shortest Path: ");
						shortestPath = FindShortestPath(stageids[spoilerLevelList.SelectedIndex]);
						for (int i = 0; i < shortestPath.Length - 1; i++)
						{
							string exit;
							if (stages[shortestPath[i]].Neutral == shortestPath[i + 1])
								exit = "Neutral";
							else if (stages[shortestPath[i]].Hero == shortestPath[i + 1])
								exit = "Hero";
							else
								exit = "Dark";
							sb.AppendFormat("{0} ({1}) -> ", GetStageName(shortestPath[i]), exit);
						}
						sb.AppendFormat("Ending ({0} levels)", shortestPath.Length);
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
				spoilerLevelInfo.Text = sb.ToString();
			}
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

		private void saveLogButton_Click(object sender, EventArgs e)
		{
			saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
			if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
				using (StreamWriter sw = File.CreateText(saveFileDialog1.FileName))
				{
					sw.WriteLine($"ShadowRando Version: {programVersion}");
					sw.WriteLine($"Seed: {seedSelector.Value}");
					sw.WriteLine($"Mode: {settings.Mode}");
					if (settings.Mode == Modes.AllStagesWarps)
					{
						sw.WriteLine($"Main Path: {mainPathSelector.SelectedItem}");
						sw.WriteLine($"Max Backwards Jump: {maxBackJump.Value}");
						sw.WriteLine($"Max Forwards Jump: {maxForwJump.Value}");
						sw.WriteLine($"Backwards Jump Probability: {backJumpProb.Value}");
						sw.WriteLine($"Allow Same Level: {allowSameLevel.Checked}");
					}
					sw.WriteLine($"Include Last Story: {includeLast.Checked}");
					sw.WriteLine($"Random Music: {randomMusic.Checked}");
					sw.WriteLine($"Random Subtitles / Voicelines: {randomFNT.Checked}");
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

		const int linespace = 8;
		private void makeChartButton_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog(this) != DialogResult.OK)
				return;
			ChartNode[] levels = new ChartNode[totalstagecount + 2];
			int gridmaxh = 0;
			int gridmaxv = 0;
			switch (settings.Mode)
			{
				case Modes.AllStagesWarps: // stages + warps
				case Modes.BossRush: // boss rush
				case Modes.Wild: // wild
					gridmaxh = 1;
					gridmaxv = stagecount + 2;
					for (int i = 0; i <= stagecount; i++)
						levels[stageids[i]] = new ChartNode(0, i + 1);
					levels[totalstagecount + 1] = new ChartNode(0, 0);
					break;
				case Modes.BranchingPaths: // branching paths
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
				case Modes.ReverseBranching: // reverse branching
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
					if (includeBosses.Checked)
					{
						gridmaxh = 1;
						gridmaxv = 11;
						int[] stgcnts = { 1, 3, 3, 5, 5, 5, 0 };
						int[][] bosses = { new int[] { }, new[] { 8 }, new[] { 4 }, new[] { 4, 8, 10 }, new[] { 2, 6 }, new int[] { }, new[] { 0, 1, 2, 3, 5, 7, 8, 9, 10 } };
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
						int[] stgcnts = { 1, 3, 3, 5, 5, 5 };
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
			Size textsz = Size.Empty;
			using (var g = CreateGraphics())
			{
				foreach (string item in LevelNames)
				{
					Size tmpsz = g.MeasureString(item, DefaultFont).ToSize();
					if (tmpsz.Width > textsz.Width)
						textsz.Width = tmpsz.Width;
					if (tmpsz.Height > textsz.Height)
						textsz.Height = tmpsz.Height;
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
			using (Bitmap bmp = new Bitmap(colwidth * gridmaxh, rowheight * gridmaxv))
			{
				using (Graphics gfx = Graphics.FromImage(bmp))
				{
					gfx.Clear(Color.White);
					List<int> stageorder = new List<int>(totalstagecount + 2);
					stageorder.Add(totalstagecount + 1);
					stageorder.AddRange(stageids);
					stageorder.Reverse();
					StringFormat fmt = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
					Pen pen = new Pen(Color.Black, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					foreach (var id in stageorder)
					{
						var node = levels[id];
						int x = colwidth * node.GridX + hmargin;
						int y = rowheight * node.GridY + vmargin;
						gfx.DrawRectangle(Pens.Black, x, y, textsz.Width, textsz.Height);
						gfx.DrawString(GetStageName(id), DefaultFont, Brushes.Black, new RectangleF(x, y, textsz.Width, textsz.Height), fmt);
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
								switch (con.Side)
								{
									case Direction.Left:
										dsty += hconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Top:
										dstx += vconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Right:
										dstx += textsz.Width + 1;
										dsty += hconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Bottom:
										dstx += vconoff + (dstlane * linespace) + (linespace / 2);
										dsty += textsz.Height + 1;
										break;
								}
								switch (con.Type)
								{
									case ConnectionType.Neutral:
										pen.Color = Color.Black;
										break;
									case ConnectionType.Hero:
										pen.Color = Color.Blue;
										break;
									case ConnectionType.Dark:
										pen.Color = Color.Red;
										break;
								}
								if (con.MaxX - con.MinX == 1 || con.MaxY - con.MinY == 1)
									pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
								else
									pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
								if (node.GetDistance(con.Node) == 1)
									gfx.DrawLine(pen, srcx, srcy, dstx, dsty);
								else
								{
									var path = new System.Drawing.Drawing2D.GraphicsPath();
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
									path.AddLine(srcx, srcy, midx, midy);
									switch (dir)
									{
										case Direction.Left:
										case Direction.Right:
											path.AddLine(midx, midy, midx, dsty);
											path.AddLine(midx, dsty, dstx, dsty);
											break;
										case Direction.Top:
										case Direction.Bottom:
											path.AddLine(midx, midy, dstx, midy);
											path.AddLine(dstx, midy, dstx, dsty);
											break;
									}
									gfx.DrawPath(pen, path);
								}
							}
					}
				}
				bmp.Save(saveFileDialog2.FileName);
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
	}

	static class Extensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
		{
			key = kvp.Key;
			value = kvp.Value;
		}
	}

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

	enum StageType { Neutral, Hero, Dark, End }

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

	class ChartNode
	{
		public int GridX { get; }
		public int GridY { get; }
		public Dictionary<Direction, List<ChartConnection>> OutgoingConnections { get; } = new Dictionary<Direction, List<ChartConnection>>();
		public Dictionary<Direction, List<ChartConnection>> IncomingConnections { get; } = new Dictionary<Direction, List<ChartConnection>>();
		public Dictionary<Direction, ChartConnection[]> ConnectionOrder { get; } = new Dictionary<Direction, ChartConnection[]>();
		public ChartNode(int x, int y)
		{
			GridX = x;
			GridY = y;
			foreach (var item in Enum.GetValues(typeof(Direction)).Cast<Direction>())
			{
				OutgoingConnections.Add(item, new List<ChartConnection>());
				IncomingConnections.Add(item, new List<ChartConnection>());
			}
		}

		public void Connect(ConnectionType color, ChartNode dest)
		{
			Direction outdir, indir;
			int xdiff = GridX - dest.GridX;
			int ydiff = GridY - dest.GridY;
			if (ydiff == -1)
			{
				outdir = Direction.Bottom;
				indir = Direction.Top;
			}
			else if (ydiff == 1)
			{
				outdir = Direction.Top;
				indir = Direction.Bottom;
			}
			else if (xdiff == 0)
			{
				if (ydiff < 1)
				{
					outdir = Direction.Right;
					indir = Direction.Right;
				}
				else
				{
					outdir = Direction.Left;
					indir = Direction.Left;
				}
			}
			else if (ydiff == 0 && (xdiff < -1 || xdiff > 1))
			{
				outdir = Direction.Top;
				indir = Direction.Top;
			}
			else if (xdiff < 0)
			{
				outdir = Direction.Right;
				indir = Direction.Left;
			}
			else
			{
				outdir = Direction.Left;
				indir = Direction.Right;
			}
			ChartConnection c = dest.IncomingConnections[indir].Find(a => a.Type == color);
			if (c == null)
			{
				c = new ChartConnection(indir, color, this, dest);
				dest.IncomingConnections[indir].Add(c);
			}
			else
				c.AddSource(this, dest);
			OutgoingConnections[outdir].Add(c);
		}

		public int GetDistance(ChartNode other) => Math.Abs(GridX - other.GridX) + Math.Abs(GridY - other.GridY);
	}

	class ChartConnection
	{
		public ChartNode Node { get; }
		public Direction Side { get; }
		public ConnectionType Type { get; }
		public List<ChartNode> Sources { get; }
		public int MinX { get; private set; }
		public int MinY { get; private set; }
		public int MaxX { get; private set; }
		public int MaxY { get; private set; }
		public int Distance { get; private set; }
		public int Lane { get; set; }

		public ChartConnection(Direction side, ConnectionType type, ChartNode src, ChartNode dst)
		{
			Node = dst;
			Side = side;
			Type = type;
			Sources = new List<ChartNode>() { src };
			MinX = Math.Min(src.GridX, dst.GridX);
			MinY = Math.Min(src.GridY, dst.GridY);
			MaxX = Math.Max(src.GridX, dst.GridX);
			MaxY = Math.Max(src.GridY, dst.GridY);
			Distance = src.GetDistance(dst);
		}

		public void AddSource(ChartNode src, ChartNode dst)
		{
			Sources.Add(src);
			MinX = Math.Min(src.GridX, MinX);
			MinY = Math.Min(src.GridY, MinY);
			MaxX = Math.Max(src.GridX, MaxX);
			MaxY = Math.Max(src.GridY, MaxY);
			Distance = Math.Max(src.GetDistance(dst), Distance);
		}
	}

	enum ConnectionType { Neutral, Hero, Dark }

	enum Direction { Left, Top, Right, Bottom }

	class Settings
	{
		public string GamePath { get; set; }
		[IniAlwaysInclude]
		public int Seed { get; set; }
		[IniAlwaysInclude]
		public bool RandomSeed { get; set; }
		[IniAlwaysInclude]
		public Modes Mode { get; set; }
		[IniAlwaysInclude]
		public MainPath MainPath { get; set; }
		[System.ComponentModel.DefaultValue(21)]
		[IniAlwaysInclude]
		public int MaxBackJump { get; set; } = 21;
		[System.ComponentModel.DefaultValue(22)]
		[IniAlwaysInclude]
		public int MaxForwJump { get; set; } = 22;
		[System.ComponentModel.DefaultValue(50)]
		[IniAlwaysInclude]
		public int BackJumpProb { get; set; } = 50;
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
		public bool RandomFNT { get; set; }

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

	enum Modes
	{
		AllStagesWarps,
		VanillaStructure,
		BranchingPaths,
		ReverseBranching,
		BossRush,
		Wild
	}

	enum MainPath
	{
		ActClear,
		AnyExit
	}

	enum MusicCategory
	{
		Stage,
		Jingle,
		Menu,
		Credits
	}
}
