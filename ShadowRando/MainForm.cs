using AFSLib;
using IniFile;
using NAudio.Wave;
using ShadowFNT;
using ShadowFNT.Structures;
using ShadowSET;
using ShadowSET.SETIDBIN;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ShadowRando
{
	public partial class MainForm : Form
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

		static readonly Dictionary<int, Type> enemyTypeMap = new Dictionary<int, Type>
		{
			{ 0, typeof(Object0064_GUNSoldier) },
			{ 1, typeof(Object0065_GUNBeetle) },
			{ 2, typeof(Object0066_GUNBigfoot) },
			{ 3, typeof(Object0068_GUNRobot) },
			{ 4, typeof(Object0078_EggPierrot) },
			{ 5, typeof(Object0079_EggPawn) },
			{ 6, typeof(Object007A_EggShadowAndroid) },
			{ 7, typeof(Object008C_BkGiant) },
			{ 8, typeof(Object008D_BkSoldier) },
			{ 9, typeof(Object008E_BkWingLarge) },
			{ 10, typeof(Object008F_BkWingSmall) },
			{ 11, typeof(Object0090_BkWorm) },
			{ 12, typeof(Object0091_BkLarva) },
			{ 13, typeof(Object0092_BkChaos) },
			{ 14, typeof(Object0093_BkNinja) },
		};

		static readonly Dictionary<int, Type> groundEnemyTypeMap = new Dictionary<int, Type>
		{
			{ 0, typeof(Object0064_GUNSoldier) },
			{ 1, typeof(Object0066_GUNBigfoot) },
			{ 2, typeof(Object0068_GUNRobot) },
			{ 3, typeof(Object0078_EggPierrot) },
			{ 4, typeof(Object0079_EggPawn) },
			{ 5, typeof(Object007A_EggShadowAndroid) },
			{ 6, typeof(Object008C_BkGiant) },
			{ 7, typeof(Object008D_BkSoldier) },
			{ 8, typeof(Object0090_BkWorm) },
			{ 9, typeof(Object0091_BkLarva) },
			{ 10, typeof(Object0093_BkNinja) },
		};

		static readonly Dictionary<int, Type> flyingEnemyTypeMap = new Dictionary<int, Type>
		{
			{ 0, typeof(Object0065_GUNBeetle) },
			{ 1, typeof(Object008E_BkWingLarge) },
			{ 2, typeof(Object008F_BkWingSmall) },
			{ 3, typeof(Object0092_BkChaos) },
			{ 4, typeof(Object0066_GUNBigfoot) }, // only if AppearType is ZUTTO_HOVERING
			{ 5, typeof(Object0093_BkNinja) }, // only if AppearType is ON_AIR_SAUCER_WARP
		};

		public MainForm()
		{
			InitializeComponent();
		}

		const string programVersion = "0.4.0-preview-2023-12-28";
		private static string hoverSoundPath = AppDomain.CurrentDomain.BaseDirectory + "res/hover.wav";
		private static string selectSoundPath = AppDomain.CurrentDomain.BaseDirectory + "res/select.wav";
		Settings settings;

		private void MainForm_Load(object sender, EventArgs e)
		{
			settings = Settings.Load();
			Text += programVersion;
			checkBoxProgramSound.Checked = settings.ProgramSound;
			seedTextBox.Text = settings.Seed;
			randomSeed.Checked = settings.RandomSeed;
			levelOrderModeSelector.SelectedIndex = (int)settings.Mode;
			mainPathSelector.SelectedIndex = (int)settings.MainPath;
			maxBackJump.Value = settings.MaxBackJump;
			maxForwJump.Value = settings.MaxForwJump;
			backJumpProb.Value = settings.BackJumpProb;
			allowSameLevel.Checked = settings.AllowSameLevel;
			includeLast.Checked = settings.IncludeLast;
			includeBosses.Checked = settings.IncludeBosses;
			randomMusic.Checked = settings.RandomMusic;
			randomFNT.Checked = settings.RandomFNT;
			randomSET.Checked = settings.RandomSET;
			// SET Configuration
			setLayout_Mode.SelectedIndex = (int)settings.SETMode;
			setLayout_keepType.Checked = settings.SETEnemyKeepType;
			setLayout_randomPartners.Checked = settings.SETRandomPartners;
			setLayout_randomWeaponsInBoxes.Checked = settings.SETRandomWeaponsInBoxes;
			// FNT Configuration
			FNTCheckBox_NoDuplicatesPreRandomization.Checked = settings.FNTNoDuplicatesPreRandomization;
			FNTCheckBox_NoSystemMessages.Checked = settings.FNTNoSystemMessages;
			FNTCheckBox_OnlyLinkedAudio.Checked = settings.FNTOnlyLinkedAudio;
			FNTCheckBox_SpecificCharacters.Checked = settings.FNTSpecificCharacters;
			FNTCheckBox_GiveAudioToNoLinkedAudio.Checked = settings.FNTGiveAudioToNoLinkedAudio;

			// FNT Configuration Specific Characters
			FNTCheckBox_Chars_Shadow.Checked = settings.FNTShadowSelected;
			FNTCheckBox_Chars_Sonic.Checked = settings.FNTSonicSelected;
			FNTCheckBox_Chars_Tails.Checked = settings.FNTTailsSelected;
			FNTCheckBox_Chars_Knuckles.Checked = settings.FNTKnucklesSelected;
			FNTCheckBox_Chars_Amy.Checked = settings.FNTAmySelected;
			FNTCheckBox_Chars_Rouge.Checked = settings.FNTRougeSelected;
			FNTCheckBox_Chars_Omega.Checked = settings.FNTOmegaSelected;
			FNTCheckBox_Chars_Vector.Checked = settings.FNTVectorSelected;
			FNTCheckBox_Chars_Espio.Checked = settings.FNTEspioSelected;
			FNTCheckBox_Chars_Maria.Checked = settings.FNTMariaSelected;
			FNTCheckBox_Chars_Charmy.Checked = settings.FNTCharmySelected;
			FNTCheckBox_Chars_Eggman.Checked = settings.FNTEggmanSelected;
			FNTCheckBox_Chars_BlackDoom.Checked = settings.FNTBlackDoomSelected;
			FNTCheckBox_Chars_Cream.Checked = settings.FNTCreamSelected;
			FNTCheckBox_Chars_Cheese.Checked = settings.FNTCheeseSelected;
			FNTCheckBox_Chars_GUNCommander.Checked = settings.FNTGUNCommanderSelected;
			FNTCheckBox_Chars_GUNSoldier.Checked = settings.FNTGUNSoldierSelected;

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
					if (!File.Exists(Path.Combine("backup", "bi2.bin")))
						File.Copy(Path.Combine(settings.GamePath, "sys", "bi2.bin"), Path.Combine("backup", "bi2.bin"));
					if (!File.Exists(Path.Combine("backup", "setid.bin")))
						File.Copy(Path.Combine(settings.GamePath, "files", "setid.bin"), Path.Combine("backup", "setid.bin"));
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
							var cmnLayout = stageDataIdentifier + "_cmn.dat";
							var nrmLayout = stageDataIdentifier + "_nrm.dat";
							var hrdLayout = stageDataIdentifier + "_hrd.dat";
							var cmnLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, cmnLayout);
							var nrmLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, nrmLayout);
							var hrdLayoutPath = Path.Combine(settings.GamePath, "files", stageDataIdentifier, hrdLayout);
							if (!Directory.Exists(Path.Combine("backup", "sets", stageDataIdentifier)))
								Directory.CreateDirectory(Path.Combine("backup", "sets", stageDataIdentifier));
							File.Copy(cmnLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, cmnLayout));
							try { File.Copy(nrmLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, nrmLayout)); } catch (FileNotFoundException) { } // some stages don't have nrm
							try { File.Copy(hrdLayoutPath, Path.Combine("backup", "sets", stageDataIdentifier, hrdLayout)); } catch (FileNotFoundException) { } // some stages don't have hrd
						}
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
			settings.ProgramSound = checkBoxProgramSound.Checked;
			settings.Seed = seedTextBox.Text;
			settings.RandomSeed = randomSeed.Checked;
			settings.Mode = (Modes)levelOrderModeSelector.SelectedIndex;
			settings.MainPath = (MainPath)mainPathSelector.SelectedIndex;
			settings.MaxBackJump = (int)maxBackJump.Value;
			settings.MaxForwJump = (int)maxForwJump.Value;
			settings.BackJumpProb = (int)backJumpProb.Value;
			settings.AllowSameLevel = allowSameLevel.Checked;
			settings.IncludeLast = includeLast.Checked;
			settings.IncludeBosses = includeBosses.Checked;
			settings.RandomMusic = randomMusic.Checked;
			settings.RandomFNT = randomFNT.Checked;
			settings.RandomSET = randomSET.Checked;
			// SET Configuration
			settings.SETMode = (SETRandomizationModes)setLayout_Mode.SelectedIndex;
			settings.SETEnemyKeepType = setLayout_keepType.Checked;
			settings.SETRandomPartners = setLayout_randomPartners.Checked;
			settings.SETRandomWeaponsInBoxes = setLayout_randomWeaponsInBoxes.Checked;
			// FNT Configuration
			settings.FNTNoDuplicatesPreRandomization = FNTCheckBox_NoDuplicatesPreRandomization.Checked;
			settings.FNTNoSystemMessages = FNTCheckBox_NoSystemMessages.Checked;
			settings.FNTOnlyLinkedAudio = FNTCheckBox_OnlyLinkedAudio.Checked;
			settings.FNTSpecificCharacters = FNTCheckBox_SpecificCharacters.Checked;
			settings.FNTGiveAudioToNoLinkedAudio = FNTCheckBox_GiveAudioToNoLinkedAudio.Checked;
			// FNT Configuration Specific Characters
			settings.FNTShadowSelected = FNTCheckBox_Chars_Shadow.Checked;
			settings.FNTSonicSelected = FNTCheckBox_Chars_Sonic.Checked;
			settings.FNTTailsSelected = FNTCheckBox_Chars_Tails.Checked;
			settings.FNTKnucklesSelected = FNTCheckBox_Chars_Knuckles.Checked;
			settings.FNTAmySelected = FNTCheckBox_Chars_Amy.Checked;
			settings.FNTRougeSelected = FNTCheckBox_Chars_Rouge.Checked;
			settings.FNTOmegaSelected = FNTCheckBox_Chars_Omega.Checked;
			settings.FNTVectorSelected = FNTCheckBox_Chars_Vector.Checked;
			settings.FNTEspioSelected = FNTCheckBox_Chars_Espio.Checked;
			settings.FNTMariaSelected = FNTCheckBox_Chars_Maria.Checked;
			settings.FNTCharmySelected = FNTCheckBox_Chars_Charmy.Checked;
			settings.FNTEggmanSelected = FNTCheckBox_Chars_Eggman.Checked;
			settings.FNTBlackDoomSelected = FNTCheckBox_Chars_BlackDoom.Checked;
			settings.FNTCreamSelected = FNTCheckBox_Chars_Cream.Checked;
			settings.FNTCheeseSelected = FNTCheckBox_Chars_Cheese.Checked;
			settings.FNTGUNCommanderSelected = FNTCheckBox_Chars_GUNCommander.Checked;
			settings.FNTGUNSoldierSelected = FNTCheckBox_Chars_GUNSoldier.Checked;
			settings.Save();
		}

		private void randomSeed_CheckedChanged(object sender, EventArgs e)
		{
			seedTextBox.Enabled = !randomSeed.Checked;
		}

        private void allowSameLevel_CheckedChanged(object sender, EventArgs e)
		{
			maxBackJump.Minimum = maxForwJump.Minimum = allowSameLevel.Checked ? 0 : 1;
		}

		private void randomizeButton_Click(object sender, EventArgs e)
		{
			byte[] dolfile = File.ReadAllBytes(Path.Combine("backup", "main.dol"));
			int seed;
			if (randomSeed.Checked)
			{
				var randomBytes = new byte[10];
				using (var rng = new RNGCryptoServiceProvider())
				{
					rng.GetBytes(randomBytes);
				}
				seedTextBox.Text = Convert.ToBase64String(randomBytes);
			}
			seed = CalculateSeed(seedTextBox.Text);
			settings.Mode = (Modes)levelOrderModeSelector.SelectedIndex;
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

			// patch the route menu to allow stg06xx+ to display next stages
			buf = BitConverter.GetBytes(routeMenu6xxStagePreviewPatchValue);
			Array.Reverse(buf);
			buf.CopyTo(dolfile, routeMenu6xxStagePreviewBlockerOffset);
			// end patch

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
				if (randomMusicSkipRankTheme.Checked)
					musicFiles[MusicCategory.Jingle].RemoveAll(a => a.EndsWith("sng_jin_roundclear.adx"));
				if (randomMusicSkipChaosPowers.Checked)
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

			if (randomFNT.Checked)
				RandomizeFNTs(r);

			if (randomSET.Checked)
				RandomizeSETs(r);

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
			var fntRandomPool = new List<ShadowFNT.Structures.TableEntry>();
			var uniqueAudioIDs = new Dictionary<int, bool>();
			var uniqueSubtitles = new Dictionary<string, bool>();
			if (FNTCheckBox_OnlyLinkedAudio.Checked || FNTCheckBox_NoDuplicatesPreRandomization.Checked || FNTCheckBox_NoSystemMessages.Checked || FNTCheckBox_SpecificCharacters.Checked)
			{
				for (int i = 0; i < fontAndAudioData.initialFntState.Count; i++)
				{
					for (int j = 0; j < fontAndAudioData.initialFntState[i].GetEntryTableCount(); j++)
					{
						var entry = fontAndAudioData.initialFntState[i].GetEntryTable()[j];
						if (FNTCheckBox_OnlyLinkedAudio.Checked && entry.audioId == -1)
							continue;
						if (FNTCheckBox_NoSystemMessages.Checked && (entry.entryType == EntryType.MENU || entry.entryType == EntryType.FINAL_ENTRY || entry.messageIdBranchSequence == 9998100))
							continue;
						if (FNTCheckBox_SpecificCharacters.Checked && entry.audioId != -1 && !CharacterPicked(fontAndAudioData.afs.Files[entry.audioId].Name))
							continue;

						if (FNTCheckBox_NoDuplicatesPreRandomization.Checked) {
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
						if (FNTCheckBox_GiveAudioToNoLinkedAudio.Checked && fntRandomPool[donotFNTEntryIndex].audioId == -1)
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
						if (FNTCheckBox_GiveAudioToNoLinkedAudio.Checked && fontAndAudioData.initialFntState[donorFNTIndex].GetEntryAudioId(donotFNTEntryIndex) == -1)
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

		private bool CharacterPicked(string audioName)
		{
			if (FNTCheckBox_Chars_Shadow.Checked && audioName.EndsWith("_sd.adx"))
				return true;
			if (FNTCheckBox_Chars_Sonic.Checked && audioName.EndsWith("_sn.adx"))
				return true;
			if (FNTCheckBox_Chars_Tails.Checked && audioName.EndsWith("_tl.adx"))
				return true;
			if (FNTCheckBox_Chars_Knuckles.Checked && audioName.EndsWith("_kn.adx"))
				return true;
			if (FNTCheckBox_Chars_Amy.Checked && audioName.EndsWith("_am.adx"))
				return true;
			if (FNTCheckBox_Chars_Rouge.Checked && audioName.EndsWith("_rg.adx"))
				return true;
			if (FNTCheckBox_Chars_Omega.Checked && audioName.EndsWith("_om.adx"))
				return true;
			if (FNTCheckBox_Chars_Vector.Checked && audioName.EndsWith("_vc.adx"))
				return true;
			if (FNTCheckBox_Chars_Espio.Checked && audioName.EndsWith("_es.adx"))
				return true;
			if (FNTCheckBox_Chars_Maria.Checked && (audioName.EndsWith("_mr.adx") || audioName.EndsWith("_mr2.adx")))
				return true;
			if (FNTCheckBox_Chars_Charmy.Checked && audioName.EndsWith("_ch.adx"))
				return true;
			if (FNTCheckBox_Chars_Eggman.Checked && audioName.EndsWith("_eg.adx"))
				return true;
			if (FNTCheckBox_Chars_BlackDoom.Checked && audioName.EndsWith("_bd.adx"))
				return true;
			if (FNTCheckBox_Chars_Cream.Checked && audioName.EndsWith("_cr.adx"))
				return true;
			if (FNTCheckBox_Chars_Cheese.Checked && audioName.EndsWith("_co.adx"))
				return true;
			if (FNTCheckBox_Chars_GUNCommander.Checked && audioName.EndsWith("_cm.adx"))
				return true;
			if (FNTCheckBox_Chars_GUNSoldier.Checked && audioName.EndsWith("_sl.adx"))
				return true;
			return false;
		}

		private void RandomizeSETs(Random r)
		{
			var mode = (SETRandomizationModes)setLayout_Mode.SelectedIndex;

			// begin randomization
			ShadowSET.LayoutEditorSystem.SetupLayoutEditorSystem(); // Critical to load relevent data
			for (int stageIdToModify = 5; stageIdToModify < 45; stageIdToModify++) {
				stageAssociationIDMap.TryGetValue(stageIdToModify, out var stageId);
				var stageDataIdentifier = "stg0" + stageId.ToString();
				var cmnLayout = stageDataIdentifier + "_cmn.dat";
				var cmnLayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, cmnLayout), out var resultcmn);
				var nrmLayout = stageDataIdentifier + "_nrm.dat";
				List<SetObjectShadow> nrmLayoutData = null;
				try
				{
					nrmLayoutData = LayoutEditorFunctions.GetShadowLayout(Path.Combine("backup", "sets", stageDataIdentifier, nrmLayout), out var resultnrm);
				} catch (FileNotFoundException)
				{
					// some stages don't have nrm
				}

				// iterate whatever rules we want, look into making this more efficient as well...

				/***
				 *          Planned modes
				    wild enemies (no rules at all, ignoring any restrictions below)
					random per stage (all hawks in one stage become a specific enemy)
					random per seed (all hawks in ALL stages become a specific enemy)

					enemy prop customization:
					same type (ex flying -> flying) (note: * include Bigfoot with prop floating as flying type)
					same affiliation (ex bk -> bk)
					all same weapon type or random
					shield type enemies % (percent that enemies will have shields or the canBlockShots property)
					prevent invalid linkid scenario for certain enemies (gun soldier being linked to a respawner, leading to never triggering it)
				*/

				// get all gunsoldiers in a layout...
				/*				List<(Object0064_GUNSoldier item, int index)> gunsoldiers = cmnLayoutData
									.Select((item, index) => new { Item = item, Index = index })
									.Where(pair => pair.Item is Object0064_GUNSoldier)
									.Select(pair => (Item: (Object0064_GUNSoldier)pair.Item, Index: pair.Index))
									.ToList();*/


				if (setLayout_randomWeaponsInBoxes.Checked)
				{
					MakeAllBoxesHaveRandomWeapons(ref cmnLayoutData, r);
					if (nrmLayoutData != null)
						MakeAllBoxesHaveRandomWeapons(ref nrmLayoutData, r);
				}

				if (setLayout_randomPartners.Checked)
				{
					MakeAllPartnersRandom(ref cmnLayoutData, r);
					if (nrmLayoutData != null)
						MakeAllPartnersRandom(ref nrmLayoutData, r);
				}

				switch (mode)
				{
					case SETRandomizationModes.None:
						break;
					case SETRandomizationModes.Wild:
						WildRandomizeAllEnemiesWithTranslations(ref cmnLayoutData, r);
						if (nrmLayoutData != null)
							WildRandomizeAllEnemiesWithTranslations(ref nrmLayoutData, r);
						break;
					case SETRandomizationModes.AllObjectsAreGUNSoldiers:
						MakeAllObjectsGUNSoldiers(ref cmnLayoutData, r);
						if (nrmLayoutData != null)
							MakeAllObjectsGUNSoldiers(ref nrmLayoutData, r);
						break;
					case SETRandomizationModes.AllEnemiesAreGUNSoldiers:
						MakeAllEnemiesGUNSoldiers(ref cmnLayoutData, r);
						if (nrmLayoutData != null)
							MakeAllEnemiesGUNSoldiers(ref nrmLayoutData, r);
						break;
					case SETRandomizationModes.AllEnemiesAreGUNSoldiersWithTranslations:
						MakeAllEnemiesGUNSoldiersWithTranslations(ref cmnLayoutData, r);
						if (nrmLayoutData != null)
							MakeAllEnemiesGUNSoldiersWithTranslations(ref nrmLayoutData, r);
						break;
				}

				LayoutEditorFunctions.SaveShadowLayout(cmnLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, cmnLayout), false);
				if (nrmLayoutData != null)
					LayoutEditorFunctions.SaveShadowLayout(nrmLayoutData, Path.Combine(settings.GamePath, "files", stageDataIdentifier, nrmLayout), false);
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
					foreach (StageEntry stage in LayoutEditorSystem.shadowStageEntries)
					{
						entry.values0 |= stage.flag0;
						entry.values1 |= stage.flag1;
						entry.values2 |= stage.flag2;
					}
				}
			}

			SetIdTableFunctions.SaveTable(Path.Combine(settings.GamePath, "files", "setid.bin"), true, setIdTable);

			// lastly, patch bi2.bin since we require 64MB Dolphin
			var buf = BitConverter.GetBytes(0);
			var bi2 = File.ReadAllBytes(Path.Combine("backup", "bi2.bin"));
			buf.CopyTo(bi2, 0x4);
			File.WriteAllBytes(Path.Combine(settings.GamePath, "sys", "bi2.bin"), bi2);
			// end patch

			MessageBox.Show("WARNING: You must set Dolphin -> Config -> Advanced -> MEM1 value to 64MB!");
		}

		private void MakeAllPartnersRandom(ref List<SetObjectShadow> setData, Random r)
		{
			List<(Object0190_Partner item, int index)> partnerItems = setData
				.Select((item, index) => new { Item = item, Index = index })
				.Where(pair => pair.Item is Object0190_Partner)
				.Select(pair => (Item: (Object0190_Partner)pair.Item, Index: pair.Index))
				.ToList();

			foreach (var partner in partnerItems)
			{
				partner.item.Partner = (Object0190_Partner.EPartner)r.Next(0x01, 0x0D);
				setData[partner.index] = partner.item;
			}
		}

		private void MakeAllBoxesHaveRandomWeapons(ref List<SetObjectShadow> setData, Random r)
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


			// valid weapons are 0x0 - 0x21

			foreach (var woodbox in woodBoxItems)
			{
				woodbox.item.BoxItem = EBoxItem.Weapon;
				woodbox.item.ModifierWeapon = (EWeapon)r.Next(0x22);
				setData[woodbox.index] = woodbox.item;
			}

			foreach (var weaponbox in weaponBoxItems)
			{
				weaponbox.item.Weapon = (EWeapon)r.Next(0x22);
				setData[weaponbox.index] = weaponbox.item;
			}

			foreach (var metalbox in metalBoxItems)
			{
				metalbox.item.BoxItem = EBoxItem.Weapon;
				metalbox.item.ModifierWeapon = (EWeapon)r.Next(0x22);
				setData[metalbox.index] = metalbox.item;
			}
		}

		private void WildRandomizeAllEnemiesWithTranslations(ref List<SetObjectShadow> setData, Random r)
		{
			// Wild Randomize of all Enemies
			for (int i = 0; i < setData.Count(); i++)
			{
				if (setData[i].List == 0x00 && (setData[i].Type >= 0x64 && setData[i].Type <= 0x93))
				{
					int randomEnemy;
					Type randomEnemyType = typeof(Nullable);
					if (setLayout_keepType.Checked)
					{
						if (IsFlyingEnemy(setData[i]))
						{
							randomEnemy = r.Next(6);
							flyingEnemyTypeMap.TryGetValue(randomEnemy, out Type enemyType);
							randomEnemyType = enemyType;
							if (randomEnemy == 4) // special case for BkNinja and Bigfoot, since we need to force a specific 
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
							} else if (randomEnemy == 5)
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
						} else
						{ // ground enemies
							if (setData[i].Type == 0x64 && (setData[i].Link == 0 || setData[i].Link == 50))
							{
								randomEnemy = r.Next(11); // All enemies if LinkID = 0 or 50
							}
							else
							{
								randomEnemy = r.Next(1, 11); // skip GUN Soldiers otherwise
							}
							groundEnemyTypeMap.TryGetValue(randomEnemy, out Type enemyType);
							randomEnemyType = enemyType;
						}
					}
					else
					{
						if (setData[i].Type == 0x64 && (setData[i].Link == 0 || setData[i].Link == 50))
						{
							randomEnemy = r.Next(15); // All enemies if LinkID = 0 or 50
						}
						else
						{
							randomEnemy = r.Next(1, 15); // skip GUN Soldiers otherwise
						}
						enemyTypeMap.TryGetValue(randomEnemy, out Type enemyType);
						randomEnemyType = enemyType;
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
					if (((Object0066_GUNBigfoot)enemy).AppearType == Object0066_GUNBigfoot.EAppear.ZUTTO_HOVERING) {
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
					sw.WriteLine($"Seed: {seedTextBox.Text}");
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

		private void randomFNT_CheckedChanged(object sender, EventArgs e)
		{
			subtitleAndVoicelineConfigurationGroupBox.Enabled = randomFNT.Checked;
			FNTCheckBox_NoDuplicatesPreRandomization.Enabled = randomFNT.Checked;
			FNTCheckBox_NoSystemMessages.Enabled = randomFNT.Checked;
			FNTCheckBox_OnlyLinkedAudio.Enabled = randomFNT.Checked;
			FNTCheckBox_SpecificCharacters.Enabled = randomFNT.Checked;
			FNTCheckBox_GiveAudioToNoLinkedAudio.Enabled = randomFNT.Checked;
			SetEnabledStateSpecifiedCharGroup(randomFNT.Checked && FNTCheckBox_SpecificCharacters.Checked);
		}

		private void FNTCheckBox_OnlyLinkedAudio_CheckedChanged(object sender, EventArgs e)
		{
			if (FNTCheckBox_OnlyLinkedAudio.Checked)
				FNTCheckBox_GiveAudioToNoLinkedAudio.Checked = false;
		}

		private void FNTCheckBox_GiveAudioToNoLinkedAudio_CheckedChanged(object sender, EventArgs e)
		{
			if (FNTCheckBox_GiveAudioToNoLinkedAudio.Checked)
				FNTCheckBox_OnlyLinkedAudio.Checked = false;
		}

		static int CalculateSeed(string seedString)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				return BitConverter.ToInt32(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(seedString)), 0);
			}
		}

		private void FNTCheckBox_SpecificCharacters_CheckedChanged(object sender, EventArgs e)
		{
			SetEnabledStateSpecifiedCharGroup(randomFNT.Checked && FNTCheckBox_SpecificCharacters.Checked);
		}

		private void SetEnabledStateSpecifiedCharGroup(bool enable)
		{
			subtitleAndVoicelineSpecifiedCharactersGroupBox.Enabled = enable;
			FNTCheckBox_Chars_Shadow.Enabled = enable;
			FNTCheckBox_Chars_Sonic.Enabled = enable;
			FNTCheckBox_Chars_Tails.Enabled = enable;
			FNTCheckBox_Chars_Knuckles.Enabled = enable;
			FNTCheckBox_Chars_Amy.Enabled = enable;
			FNTCheckBox_Chars_Rouge.Enabled = enable;
			FNTCheckBox_Chars_Omega.Enabled = enable;
			FNTCheckBox_Chars_Vector.Enabled = enable;
			FNTCheckBox_Chars_Espio.Enabled = enable;
			FNTCheckBox_Chars_Maria.Enabled = enable;
			FNTCheckBox_Chars_Charmy.Enabled = enable;
			FNTCheckBox_Chars_Eggman.Enabled = enable;
			FNTCheckBox_Chars_BlackDoom.Enabled = enable;
			FNTCheckBox_Chars_Cream.Enabled = enable;
			FNTCheckBox_Chars_Cheese.Enabled = enable;
			FNTCheckBox_Chars_GUNCommander.Enabled = enable;
			FNTCheckBox_Chars_GUNSoldier.Enabled = enable;
		}

		private void SharedMouseEnter(object sender, EventArgs e)
		{
			PlayAudio(hoverSoundPath);
		}

		private void SharedMouseDown_Shoot(object sender, MouseEventArgs e)
		{
			PlayAudio(selectSoundPath);
		}

		private void SharedMouseDown_Chkchk(object sender, MouseEventArgs e)
		{
			PlayAudio(hoverSoundPath);
		}

		private void SharedMouseDown(object sender, EventArgs e)
		{
			PlayAudio(selectSoundPath);
		}

		private void PlayAudio(string soundPath)
		{
			if (!checkBoxProgramSound.Checked)
				return;

			var outputDevice = new WaveOutEvent();

			// Create a new instance of WaveFileReader for each call to playsound
			WaveFileReader reader = new WaveFileReader(soundPath);

			outputDevice.Init(reader);
			outputDevice.Play();

			// Hook the PlaybackStopped event to dispose of resources when playback is finished
			outputDevice.PlaybackStopped += (sender, args) =>
			{
				outputDevice.Dispose();
				reader.Dispose();
			};
		}

		private void randomSET_CheckedChanged(object sender, EventArgs e)
		{
			setLayout_groupBoxEnemyConfiguration.Enabled = randomSET.Checked;
			setLayout_keepType.Enabled = randomSET.Checked;
			setLayout_randomWeaponsInBoxes.Enabled = randomSET.Checked;
			setLayout_randomPartners.Enabled = randomSET.Checked;
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
		[IniAlwaysInclude]
		public bool ProgramSound { get; set; } = true;
		[IniAlwaysInclude]
		public string GamePath { get; set; }
		[IniAlwaysInclude]
		public string Seed { get; set; }
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
		public bool RandomFNT { get; set; }
		[IniAlwaysInclude]
		public bool RandomSET { get; set; }
		// SET
		[IniAlwaysInclude]
		public SETRandomizationModes SETMode { get; set; }
		//public SETMode
		[IniAlwaysInclude]
		public bool SETEnemyKeepType { get; set; }
		[IniAlwaysInclude]
		public bool SETRandomPartners {  get; set; }
		[IniAlwaysInclude]
		public bool SETRandomWeaponsInBoxes {  get; set; }
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

	enum Modes
	{
		AllStagesWarps,
		VanillaStructure,
		BranchingPaths,
		ReverseBranching,
		BossRush,
		Wild
	}

	enum SETRandomizationModes
	{
		None,
		Wild,
		OneToOnePerStage,
		OneToOneGlobal,
		AllEnemiesAreGUNSoldiers,
		AllObjectsAreGUNSoldiers,
		AllEnemiesAreGUNSoldiersWithTranslations
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
