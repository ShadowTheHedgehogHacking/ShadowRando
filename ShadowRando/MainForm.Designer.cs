
namespace ShadowRando
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label levelOrder_seedLabel;
            System.Windows.Forms.Label levelOrder_mainPathLabel;
            System.Windows.Forms.Label levelOrder_maxBackwardsJumpLabel;
            System.Windows.Forms.Label levelOrder_maxForwardsJumpLabel;
            System.Windows.Forms.Label levelOrder_backwardsJumpProbabilityLabel;
            System.Windows.Forms.Label setLayout_HaveShieldBlockProbLabel;
            this.randomSeed = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mainPathSelector = new System.Windows.Forms.ComboBox();
            this.maxBackJump = new System.Windows.Forms.NumericUpDown();
            this.maxForwJump = new System.Windows.Forms.NumericUpDown();
            this.randomizeButton = new System.Windows.Forms.Button();
            this.backJumpProb = new System.Windows.Forms.NumericUpDown();
            this.allowSameLevel = new System.Windows.Forms.CheckBox();
            this.levelOrderModeSelector = new System.Windows.Forms.ComboBox();
            this.saveLogButton = new System.Windows.Forms.Button();
            this.makeChartButton = new System.Windows.Forms.Button();
            this.randomFNT = new System.Windows.Forms.CheckBox();
            this.includeLast = new System.Windows.Forms.CheckBox();
            this.includeBosses = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_SpecificCharacters = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_OnlyLinkedAudio = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_NoSystemMessages = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_NoDuplicatesPreRandomization = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_GiveAudioToNoLinkedAudio = new System.Windows.Forms.CheckBox();
            this.seedTextBox = new System.Windows.Forms.TextBox();
            this.randomMusic = new System.Windows.Forms.CheckBox();
            this.randomSET = new System.Windows.Forms.CheckBox();
            this.setLayout_Mode = new System.Windows.Forms.ComboBox();
            this.setLayout_randomWeaponsInBoxes = new System.Windows.Forms.CheckBox();
            this.setLayout_enemyRandomWeapons = new System.Windows.Forms.CheckBox();
            this.setLayout_keepAffiliation = new System.Windows.Forms.CheckBox();
            this.setLayout_keepType = new System.Windows.Forms.CheckBox();
            this.setLayout_haveShieldBlockProb = new System.Windows.Forms.NumericUpDown();
            this.setLayout_enemyOverrideShieldBlock = new System.Windows.Forms.CheckBox();
            this.setLayout_randomPartners = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLevelOrder = new System.Windows.Forms.TabPage();
            this.levelOrder_modeLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPageSET = new System.Windows.Forms.TabPage();
            this.tempWarning = new System.Windows.Forms.Label();
            this.setLayout_groupBoxEnemyConfiguration = new System.Windows.Forms.GroupBox();
            this.setLayout_ModeLabel = new System.Windows.Forms.Label();
            this.tabPageSubtitles = new System.Windows.Forms.TabPage();
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox = new System.Windows.Forms.GroupBox();
            this.FNTCheckBox_Chars_GUNSoldier = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_GUNCommander = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_BlackDoom = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Cheese = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Maria = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Cream = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Eggman = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Charmy = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Espio = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Vector = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Omega = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Rouge = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Amy = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Knuckles = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Tails = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Sonic = new System.Windows.Forms.CheckBox();
            this.FNTCheckBox_Chars_Shadow = new System.Windows.Forms.CheckBox();
            this.subtitleAndVoicelineConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.tabPageMusic = new System.Windows.Forms.TabPage();
            this.randomMusicSkipChaosPowers = new System.Windows.Forms.CheckBox();
            this.randomMusicSkipRankTheme = new System.Windows.Forms.CheckBox();
            this.tabPageSpoilers = new System.Windows.Forms.TabPage();
            this.spoilerLevelInfo = new System.Windows.Forms.TextBox();
            this.spoilerLevelList = new System.Windows.Forms.ListBox();
            this.tabPageProgramOptions = new System.Windows.Forms.TabPage();
            this.checkBoxProgramSound = new System.Windows.Forms.CheckBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.setLayout_adjustMissionCounts = new System.Windows.Forms.CheckBox();
            levelOrder_seedLabel = new System.Windows.Forms.Label();
            levelOrder_mainPathLabel = new System.Windows.Forms.Label();
            levelOrder_maxBackwardsJumpLabel = new System.Windows.Forms.Label();
            levelOrder_maxForwardsJumpLabel = new System.Windows.Forms.Label();
            levelOrder_backwardsJumpProbabilityLabel = new System.Windows.Forms.Label();
            setLayout_HaveShieldBlockProbLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backJumpProb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setLayout_haveShieldBlockProb)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageLevelOrder.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageSET.SuspendLayout();
            this.setLayout_groupBoxEnemyConfiguration.SuspendLayout();
            this.tabPageSubtitles.SuspendLayout();
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.SuspendLayout();
            this.subtitleAndVoicelineConfigurationGroupBox.SuspendLayout();
            this.tabPageMusic.SuspendLayout();
            this.tabPageSpoilers.SuspendLayout();
            this.tabPageProgramOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // levelOrder_seedLabel
            // 
            levelOrder_seedLabel.AutoSize = true;
            levelOrder_seedLabel.Location = new System.Drawing.Point(8, 10);
            levelOrder_seedLabel.Name = "levelOrder_seedLabel";
            levelOrder_seedLabel.Size = new System.Drawing.Size(35, 13);
            levelOrder_seedLabel.TabIndex = 0;
            levelOrder_seedLabel.Text = "Seed:";
            // 
            // levelOrder_mainPathLabel
            // 
            levelOrder_mainPathLabel.AutoSize = true;
            levelOrder_mainPathLabel.Location = new System.Drawing.Point(5, 6);
            levelOrder_mainPathLabel.Name = "levelOrder_mainPathLabel";
            levelOrder_mainPathLabel.Size = new System.Drawing.Size(58, 13);
            levelOrder_mainPathLabel.TabIndex = 0;
            levelOrder_mainPathLabel.Text = "Main Path:";
            // 
            // levelOrder_maxBackwardsJumpLabel
            // 
            levelOrder_maxBackwardsJumpLabel.AutoSize = true;
            levelOrder_maxBackwardsJumpLabel.Location = new System.Drawing.Point(62, 29);
            levelOrder_maxBackwardsJumpLabel.Name = "levelOrder_maxBackwardsJumpLabel";
            levelOrder_maxBackwardsJumpLabel.Size = new System.Drawing.Size(114, 13);
            levelOrder_maxBackwardsJumpLabel.TabIndex = 2;
            levelOrder_maxBackwardsJumpLabel.Text = "Max Backwards Jump:";
            // 
            // levelOrder_maxForwardsJumpLabel
            // 
            levelOrder_maxForwardsJumpLabel.AutoSize = true;
            levelOrder_maxForwardsJumpLabel.Location = new System.Drawing.Point(72, 55);
            levelOrder_maxForwardsJumpLabel.Name = "levelOrder_maxForwardsJumpLabel";
            levelOrder_maxForwardsJumpLabel.Size = new System.Drawing.Size(104, 13);
            levelOrder_maxForwardsJumpLabel.TabIndex = 4;
            levelOrder_maxForwardsJumpLabel.Text = "Max Forwards Jump:";
            // 
            // levelOrder_backwardsJumpProbabilityLabel
            // 
            levelOrder_backwardsJumpProbabilityLabel.AutoSize = true;
            levelOrder_backwardsJumpProbabilityLabel.Location = new System.Drawing.Point(34, 79);
            levelOrder_backwardsJumpProbabilityLabel.Name = "levelOrder_backwardsJumpProbabilityLabel";
            levelOrder_backwardsJumpProbabilityLabel.Size = new System.Drawing.Size(142, 13);
            levelOrder_backwardsJumpProbabilityLabel.TabIndex = 6;
            levelOrder_backwardsJumpProbabilityLabel.Text = "Backwards Jump Probability:";
            // 
            // setLayout_HaveShieldBlockProbLabel
            // 
            setLayout_HaveShieldBlockProbLabel.AutoSize = true;
            setLayout_HaveShieldBlockProbLabel.Enabled = false;
            setLayout_HaveShieldBlockProbLabel.Location = new System.Drawing.Point(6, 101);
            setLayout_HaveShieldBlockProbLabel.Name = "setLayout_HaveShieldBlockProbLabel";
            setLayout_HaveShieldBlockProbLabel.Size = new System.Drawing.Size(148, 13);
            setLayout_HaveShieldBlockProbLabel.TabIndex = 13;
            setLayout_HaveShieldBlockProbLabel.Text = "HaveShield/Block Probability:";
            // 
            // randomSeed
            // 
            this.randomSeed.AutoSize = true;
            this.randomSeed.Location = new System.Drawing.Point(232, 8);
            this.randomSeed.Name = "randomSeed";
            this.randomSeed.Size = new System.Drawing.Size(66, 17);
            this.randomSeed.TabIndex = 2;
            this.randomSeed.Text = "Random";
            this.toolTip1.SetToolTip(this.randomSeed, "Check this box to have the randomizer generate a seed for you based on the curren" +
        "t time.");
            this.randomSeed.UseVisualStyleBackColor = true;
            this.randomSeed.CheckedChanged += new System.EventHandler(this.randomSeed_CheckedChanged);
            this.randomSeed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // mainPathSelector
            // 
            this.mainPathSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mainPathSelector.Items.AddRange(new object[] {
            "Act Clear",
            "Any Exit"});
            this.mainPathSelector.Location = new System.Drawing.Point(67, 3);
            this.mainPathSelector.Name = "mainPathSelector";
            this.mainPathSelector.Size = new System.Drawing.Size(156, 21);
            this.mainPathSelector.TabIndex = 1;
            this.toolTip1.SetToolTip(this.mainPathSelector, "Which exits from a level are allowed to be part of the main path.");
            // 
            // maxBackJump
            // 
            this.maxBackJump.Location = new System.Drawing.Point(182, 27);
            this.maxBackJump.Maximum = new decimal(new int[] {
            69,
            0,
            0,
            0});
            this.maxBackJump.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxBackJump.Name = "maxBackJump";
            this.maxBackJump.Size = new System.Drawing.Size(41, 20);
            this.maxBackJump.TabIndex = 3;
            this.toolTip1.SetToolTip(this.maxBackJump, "The maximum number of stages along the main path that you can get sent backwards." +
        "");
            this.maxBackJump.Value = new decimal(new int[] {
            69,
            0,
            0,
            0});
            // 
            // maxForwJump
            // 
            this.maxForwJump.Location = new System.Drawing.Point(182, 53);
            this.maxForwJump.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.maxForwJump.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxForwJump.Name = "maxForwJump";
            this.maxForwJump.Size = new System.Drawing.Size(41, 20);
            this.maxForwJump.TabIndex = 5;
            this.toolTip1.SetToolTip(this.maxForwJump, "The maximum number of stages along the main path that you can get sent forwards.");
            this.maxForwJump.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // randomizeButton
            // 
            this.randomizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.randomizeButton.Location = new System.Drawing.Point(416, 224);
            this.randomizeButton.Name = "randomizeButton";
            this.randomizeButton.Size = new System.Drawing.Size(75, 23);
            this.randomizeButton.TabIndex = 7;
            this.randomizeButton.Text = "Randomize!";
            this.toolTip1.SetToolTip(this.randomizeButton, "Click this button to randomize the game with these settings.");
            this.randomizeButton.UseVisualStyleBackColor = true;
            this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
            this.randomizeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Shoot);
            this.randomizeButton.MouseEnter += new System.EventHandler(this.SharedMouseEnter);
            // 
            // backJumpProb
            // 
            this.backJumpProb.Location = new System.Drawing.Point(182, 77);
            this.backJumpProb.Name = "backJumpProb";
            this.backJumpProb.Size = new System.Drawing.Size(41, 20);
            this.backJumpProb.TabIndex = 7;
            this.toolTip1.SetToolTip(this.backJumpProb, "The probability that a backwards jump will be chosen instead of a forwards jump.");
            this.backJumpProb.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // allowSameLevel
            // 
            this.allowSameLevel.AutoSize = true;
            this.allowSameLevel.Location = new System.Drawing.Point(229, 79);
            this.allowSameLevel.Name = "allowSameLevel";
            this.allowSameLevel.Size = new System.Drawing.Size(155, 17);
            this.allowSameLevel.TabIndex = 8;
            this.allowSameLevel.Text = "Allow Jumps to Same Level";
            this.toolTip1.SetToolTip(this.allowSameLevel, "If checked, warps may take you to the start of the level you\'re in currently.");
            this.allowSameLevel.UseVisualStyleBackColor = true;
            this.allowSameLevel.CheckedChanged += new System.EventHandler(this.allowSameLevel_CheckedChanged);
            this.allowSameLevel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // levelOrderModeSelector
            // 
            this.levelOrderModeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelOrderModeSelector.FormattingEnabled = true;
            this.levelOrderModeSelector.Items.AddRange(new object[] {
            "Shuffle All Stages w/ Warps",
            "Vanilla Structure",
            "Branching Paths",
            "Reverse Branching",
            "Boss Rush",
            "Wild"});
            this.levelOrderModeSelector.Location = new System.Drawing.Point(49, 32);
            this.levelOrderModeSelector.Name = "levelOrderModeSelector";
            this.levelOrderModeSelector.Size = new System.Drawing.Size(177, 21);
            this.levelOrderModeSelector.TabIndex = 4;
            this.toolTip1.SetToolTip(this.levelOrderModeSelector, "If you don\'t know what this is, please read the mod\'s description.");
            // 
            // saveLogButton
            // 
            this.saveLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveLogButton.AutoSize = true;
            this.saveLogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveLogButton.Enabled = false;
            this.saveLogButton.Location = new System.Drawing.Point(417, 163);
            this.saveLogButton.Name = "saveLogButton";
            this.saveLogButton.Size = new System.Drawing.Size(72, 23);
            this.saveLogButton.TabIndex = 2;
            this.saveLogButton.Text = "Save Log...";
            this.toolTip1.SetToolTip(this.saveLogButton, "Click this button to generate a text log containing your settings and the level p" +
        "rogression.");
            this.saveLogButton.UseVisualStyleBackColor = true;
            this.saveLogButton.Click += new System.EventHandler(this.saveLogButton_Click);
            this.saveLogButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown);
            this.saveLogButton.MouseEnter += new System.EventHandler(this.SharedMouseEnter);
            // 
            // makeChartButton
            // 
            this.makeChartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.makeChartButton.AutoSize = true;
            this.makeChartButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.makeChartButton.Enabled = false;
            this.makeChartButton.Location = new System.Drawing.Point(330, 163);
            this.makeChartButton.Name = "makeChartButton";
            this.makeChartButton.Size = new System.Drawing.Size(81, 23);
            this.makeChartButton.TabIndex = 3;
            this.makeChartButton.Text = "Make Chart...";
            this.toolTip1.SetToolTip(this.makeChartButton, "Click this button to generate an image showing the progression between levels.");
            this.makeChartButton.UseVisualStyleBackColor = true;
            this.makeChartButton.Click += new System.EventHandler(this.makeChartButton_Click);
            this.makeChartButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown);
            this.makeChartButton.MouseEnter += new System.EventHandler(this.SharedMouseEnter);
            // 
            // randomFNT
            // 
            this.randomFNT.AutoSize = true;
            this.randomFNT.Location = new System.Drawing.Point(14, 6);
            this.randomFNT.Name = "randomFNT";
            this.randomFNT.Size = new System.Drawing.Size(181, 17);
            this.randomFNT.TabIndex = 7;
            this.randomFNT.Text = "Randomize Subtitles / Voicelines";
            this.toolTip1.SetToolTip(this.randomFNT, "Check this box to shuffle the subtitles played.");
            this.randomFNT.UseVisualStyleBackColor = true;
            this.randomFNT.CheckedChanged += new System.EventHandler(this.randomFNT_CheckedChanged);
            this.randomFNT.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // includeLast
            // 
            this.includeLast.AutoSize = true;
            this.includeLast.Location = new System.Drawing.Point(10, 164);
            this.includeLast.Name = "includeLast";
            this.includeLast.Size = new System.Drawing.Size(111, 17);
            this.includeLast.TabIndex = 6;
            this.includeLast.Text = "Include Last Story";
            this.toolTip1.SetToolTip(this.includeLast, "If checked, the levels from the Last Story will also be randomized.");
            this.includeLast.UseVisualStyleBackColor = true;
            this.includeLast.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // includeBosses
            // 
            this.includeBosses.AutoSize = true;
            this.includeBosses.Location = new System.Drawing.Point(117, 164);
            this.includeBosses.Name = "includeBosses";
            this.includeBosses.Size = new System.Drawing.Size(98, 17);
            this.includeBosses.TabIndex = 7;
            this.includeBosses.Text = "Include Bosses";
            this.toolTip1.SetToolTip(this.includeBosses, "If unchecked, boss fights will not be included in the game.");
            this.includeBosses.UseVisualStyleBackColor = true;
            this.includeBosses.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // FNTCheckBox_SpecificCharacters
            // 
            this.FNTCheckBox_SpecificCharacters.AutoSize = true;
            this.FNTCheckBox_SpecificCharacters.Enabled = false;
            this.FNTCheckBox_SpecificCharacters.Location = new System.Drawing.Point(6, 19);
            this.FNTCheckBox_SpecificCharacters.Name = "FNTCheckBox_SpecificCharacters";
            this.FNTCheckBox_SpecificCharacters.Size = new System.Drawing.Size(148, 17);
            this.FNTCheckBox_SpecificCharacters.TabIndex = 8;
            this.FNTCheckBox_SpecificCharacters.Text = "Only Specified Characters";
            this.toolTip1.SetToolTip(this.FNTCheckBox_SpecificCharacters, "Only characters specified are used");
            this.FNTCheckBox_SpecificCharacters.UseVisualStyleBackColor = true;
            this.FNTCheckBox_SpecificCharacters.CheckedChanged += new System.EventHandler(this.FNTCheckBox_SpecificCharacters_CheckedChanged);
            // 
            // FNTCheckBox_OnlyLinkedAudio
            // 
            this.FNTCheckBox_OnlyLinkedAudio.AutoSize = true;
            this.FNTCheckBox_OnlyLinkedAudio.Enabled = false;
            this.FNTCheckBox_OnlyLinkedAudio.Location = new System.Drawing.Point(6, 34);
            this.FNTCheckBox_OnlyLinkedAudio.Name = "FNTCheckBox_OnlyLinkedAudio";
            this.FNTCheckBox_OnlyLinkedAudio.Size = new System.Drawing.Size(134, 17);
            this.FNTCheckBox_OnlyLinkedAudio.TabIndex = 9;
            this.FNTCheckBox_OnlyLinkedAudio.Text = "Only with Linked Audio";
            this.toolTip1.SetToolTip(this.FNTCheckBox_OnlyLinkedAudio, "Only subtitles that have a matching voiceline are used in the random pool");
            this.FNTCheckBox_OnlyLinkedAudio.UseVisualStyleBackColor = true;
            this.FNTCheckBox_OnlyLinkedAudio.CheckedChanged += new System.EventHandler(this.FNTCheckBox_OnlyLinkedAudio_CheckedChanged);
            // 
            // FNTCheckBox_NoSystemMessages
            // 
            this.FNTCheckBox_NoSystemMessages.AutoSize = true;
            this.FNTCheckBox_NoSystemMessages.Enabled = false;
            this.FNTCheckBox_NoSystemMessages.Location = new System.Drawing.Point(6, 65);
            this.FNTCheckBox_NoSystemMessages.Name = "FNTCheckBox_NoSystemMessages";
            this.FNTCheckBox_NoSystemMessages.Size = new System.Drawing.Size(164, 17);
            this.FNTCheckBox_NoSystemMessages.TabIndex = 10;
            this.FNTCheckBox_NoSystemMessages.Text = "No System Messages In Pool";
            this.toolTip1.SetToolTip(this.FNTCheckBox_NoSystemMessages, "No system messages will be used in the random pool");
            this.FNTCheckBox_NoSystemMessages.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_NoDuplicatesPreRandomization
            // 
            this.FNTCheckBox_NoDuplicatesPreRandomization.AutoSize = true;
            this.FNTCheckBox_NoDuplicatesPreRandomization.Enabled = false;
            this.FNTCheckBox_NoDuplicatesPreRandomization.Location = new System.Drawing.Point(6, 80);
            this.FNTCheckBox_NoDuplicatesPreRandomization.Name = "FNTCheckBox_NoDuplicatesPreRandomization";
            this.FNTCheckBox_NoDuplicatesPreRandomization.Size = new System.Drawing.Size(129, 17);
            this.FNTCheckBox_NoDuplicatesPreRandomization.TabIndex = 11;
            this.FNTCheckBox_NoDuplicatesPreRandomization.Text = "No Duplicates In Pool";
            this.toolTip1.SetToolTip(this.FNTCheckBox_NoDuplicatesPreRandomization, "Every unique subtitle entry is only considered once in the pool. It is still poss" +
        "ible to get the same entry multiple times.");
            this.FNTCheckBox_NoDuplicatesPreRandomization.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_GiveAudioToNoLinkedAudio
            // 
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.AutoSize = true;
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.Enabled = false;
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.Location = new System.Drawing.Point(6, 49);
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.Name = "FNTCheckBox_GiveAudioToNoLinkedAudio";
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.Size = new System.Drawing.Size(172, 17);
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.TabIndex = 12;
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.Text = "Give Audio to No Linked Audio";
            this.toolTip1.SetToolTip(this.FNTCheckBox_GiveAudioToNoLinkedAudio, "Subtitles with no associated audio will be given random audio");
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.UseVisualStyleBackColor = true;
            this.FNTCheckBox_GiveAudioToNoLinkedAudio.CheckedChanged += new System.EventHandler(this.FNTCheckBox_GiveAudioToNoLinkedAudio_CheckedChanged);
            // 
            // seedTextBox
            // 
            this.seedTextBox.Location = new System.Drawing.Point(49, 6);
            this.seedTextBox.Name = "seedTextBox";
            this.seedTextBox.Size = new System.Drawing.Size(177, 20);
            this.seedTextBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.seedTextBox, "This value controls how things are randomized.");
            this.seedTextBox.WordWrap = false;
            // 
            // randomMusic
            // 
            this.randomMusic.AutoSize = true;
            this.randomMusic.Location = new System.Drawing.Point(14, 6);
            this.randomMusic.Name = "randomMusic";
            this.randomMusic.Size = new System.Drawing.Size(110, 17);
            this.randomMusic.TabIndex = 7;
            this.randomMusic.Text = "Randomize Music";
            this.toolTip1.SetToolTip(this.randomMusic, "Check this box to shuffle the music that\'s played in each area of the game.");
            this.randomMusic.UseVisualStyleBackColor = true;
            this.randomMusic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // randomSET
            // 
            this.randomSET.AutoSize = true;
            this.randomSET.Location = new System.Drawing.Point(14, 6);
            this.randomSET.Name = "randomSET";
            this.randomSET.Size = new System.Drawing.Size(300, 17);
            this.randomSET.TabIndex = 9;
            this.randomSET.Text = "Randomize SETs (Requires 64MB Patch / Dolphin ONLY)";
            this.toolTip1.SetToolTip(this.randomSET, "Check this box to enable randomization of enemies and weapons, per the options be" +
        "low.");
            this.randomSET.UseVisualStyleBackColor = true;
            this.randomSET.CheckedChanged += new System.EventHandler(this.randomSET_CheckedChanged);
            this.randomSET.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // setLayout_Mode
            // 
            this.setLayout_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.setLayout_Mode.FormattingEnabled = true;
            this.setLayout_Mode.Items.AddRange(new object[] {
            "No Random Enemies",
            "Wild (WIP)",
            "1:1 Per Stage (Not Ready)",
            "1:1 Global (Not Ready)",
            "All Enemies are GUN Soldiers",
            "All Objects are GUN Soldiers",
            "All Enemies are GUN Soldiers w/Translations"});
            this.setLayout_Mode.Location = new System.Drawing.Point(55, 25);
            this.setLayout_Mode.Name = "setLayout_Mode";
            this.setLayout_Mode.Size = new System.Drawing.Size(259, 21);
            this.setLayout_Mode.TabIndex = 11;
            this.toolTip1.SetToolTip(this.setLayout_Mode, "If you don\'t know what this is, please read the mod\'s description.");
            // 
            // setLayout_randomWeaponsInBoxes
            // 
            this.setLayout_randomWeaponsInBoxes.AutoSize = true;
            this.setLayout_randomWeaponsInBoxes.Enabled = false;
            this.setLayout_randomWeaponsInBoxes.Location = new System.Drawing.Point(247, 99);
            this.setLayout_randomWeaponsInBoxes.Name = "setLayout_randomWeaponsInBoxes";
            this.setLayout_randomWeaponsInBoxes.Size = new System.Drawing.Size(158, 17);
            this.setLayout_randomWeaponsInBoxes.TabIndex = 12;
            this.setLayout_randomWeaponsInBoxes.Text = "Random Weapons in Boxes";
            this.toolTip1.SetToolTip(this.setLayout_randomWeaponsInBoxes, "Makes wood, metal, and weapon boxes all contain a random weapon.");
            this.setLayout_randomWeaponsInBoxes.UseVisualStyleBackColor = true;
            this.setLayout_randomWeaponsInBoxes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // setLayout_enemyRandomWeapons
            // 
            this.setLayout_enemyRandomWeapons.AutoSize = true;
            this.setLayout_enemyRandomWeapons.Enabled = false;
            this.setLayout_enemyRandomWeapons.Location = new System.Drawing.Point(6, 49);
            this.setLayout_enemyRandomWeapons.Name = "setLayout_enemyRandomWeapons";
            this.setLayout_enemyRandomWeapons.Size = new System.Drawing.Size(115, 17);
            this.setLayout_enemyRandomWeapons.TabIndex = 12;
            this.setLayout_enemyRandomWeapons.Text = "Random Weapons";
            this.toolTip1.SetToolTip(this.setLayout_enemyRandomWeapons, "If off, enemies of the same type will all have the same weapon type");
            this.setLayout_enemyRandomWeapons.UseVisualStyleBackColor = true;
            // 
            // setLayout_keepAffiliation
            // 
            this.setLayout_keepAffiliation.AutoSize = true;
            this.setLayout_keepAffiliation.Enabled = false;
            this.setLayout_keepAffiliation.Location = new System.Drawing.Point(6, 34);
            this.setLayout_keepAffiliation.Name = "setLayout_keepAffiliation";
            this.setLayout_keepAffiliation.Size = new System.Drawing.Size(168, 17);
            this.setLayout_keepAffiliation.TabIndex = 9;
            this.setLayout_keepAffiliation.Text = "Keep Affiliation (GUN -> GUN)";
            this.toolTip1.SetToolTip(this.setLayout_keepAffiliation, "Enemies will randomize within their affiliation only. GUN -> GUN; BK -> BK; EGG -" +
        "> EGG");
            this.setLayout_keepAffiliation.UseVisualStyleBackColor = true;
            // 
            // setLayout_keepType
            // 
            this.setLayout_keepType.AutoSize = true;
            this.setLayout_keepType.Enabled = false;
            this.setLayout_keepType.Location = new System.Drawing.Point(6, 19);
            this.setLayout_keepType.Name = "setLayout_keepType";
            this.setLayout_keepType.Size = new System.Drawing.Size(156, 17);
            this.setLayout_keepType.TabIndex = 8;
            this.setLayout_keepType.Text = "Keep Type (Flying -> Flying)";
            this.toolTip1.SetToolTip(this.setLayout_keepType, "Example: A flying enemy (GUN Beetle) will be restricted to become another flying " +
        "enemy type.");
            this.setLayout_keepType.UseVisualStyleBackColor = true;
            // 
            // setLayout_haveShieldBlockProb
            // 
            this.setLayout_haveShieldBlockProb.Enabled = false;
            this.setLayout_haveShieldBlockProb.Location = new System.Drawing.Point(154, 98);
            this.setLayout_haveShieldBlockProb.Name = "setLayout_haveShieldBlockProb";
            this.setLayout_haveShieldBlockProb.Size = new System.Drawing.Size(41, 20);
            this.setLayout_haveShieldBlockProb.TabIndex = 14;
            this.toolTip1.SetToolTip(this.setLayout_haveShieldBlockProb, "The probability that a backwards jump will be chosen instead of a forwards jump.");
            this.setLayout_haveShieldBlockProb.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // setLayout_enemyOverrideShieldBlock
            // 
            this.setLayout_enemyOverrideShieldBlock.AutoSize = true;
            this.setLayout_enemyOverrideShieldBlock.Enabled = false;
            this.setLayout_enemyOverrideShieldBlock.Location = new System.Drawing.Point(6, 81);
            this.setLayout_enemyOverrideShieldBlock.Name = "setLayout_enemyOverrideShieldBlock";
            this.setLayout_enemyOverrideShieldBlock.Size = new System.Drawing.Size(169, 17);
            this.setLayout_enemyOverrideShieldBlock.TabIndex = 11;
            this.setLayout_enemyOverrideShieldBlock.Text = "Override Shield/Block % Ratio";
            this.toolTip1.SetToolTip(this.setLayout_enemyOverrideShieldBlock, "% an enemy will gain a shield or block ability, if eligible");
            this.setLayout_enemyOverrideShieldBlock.UseVisualStyleBackColor = true;
            // 
            // setLayout_randomPartners
            // 
            this.setLayout_randomPartners.AutoSize = true;
            this.setLayout_randomPartners.Enabled = false;
            this.setLayout_randomPartners.Location = new System.Drawing.Point(247, 122);
            this.setLayout_randomPartners.Name = "setLayout_randomPartners";
            this.setLayout_randomPartners.Size = new System.Drawing.Size(108, 17);
            this.setLayout_randomPartners.TabIndex = 15;
            this.setLayout_randomPartners.Text = "Random Partners";
            this.toolTip1.SetToolTip(this.setLayout_randomPartners, "All partners are random. You may not encounter Sonic / Black Doom in Westopolis f" +
        "or example.");
            this.setLayout_randomPartners.UseVisualStyleBackColor = true;
            this.setLayout_randomPartners.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLevelOrder);
            this.tabControl1.Controls.Add(this.tabPageSET);
            this.tabControl1.Controls.Add(this.tabPageSubtitles);
            this.tabControl1.Controls.Add(this.tabPageMusic);
            this.tabControl1.Controls.Add(this.tabPageSpoilers);
            this.tabControl1.Controls.Add(this.tabPageProgramOptions);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(503, 218);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageLevelOrder
            // 
            this.tabPageLevelOrder.Controls.Add(this.seedTextBox);
            this.tabPageLevelOrder.Controls.Add(this.includeBosses);
            this.tabPageLevelOrder.Controls.Add(this.includeLast);
            this.tabPageLevelOrder.Controls.Add(this.levelOrderModeSelector);
            this.tabPageLevelOrder.Controls.Add(this.levelOrder_modeLabel);
            this.tabPageLevelOrder.Controls.Add(this.panel1);
            this.tabPageLevelOrder.Controls.Add(levelOrder_seedLabel);
            this.tabPageLevelOrder.Controls.Add(this.randomSeed);
            this.tabPageLevelOrder.Location = new System.Drawing.Point(4, 22);
            this.tabPageLevelOrder.Name = "tabPageLevelOrder";
            this.tabPageLevelOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLevelOrder.Size = new System.Drawing.Size(495, 192);
            this.tabPageLevelOrder.TabIndex = 0;
            this.tabPageLevelOrder.Text = "Level Order";
            this.tabPageLevelOrder.UseVisualStyleBackColor = true;
            // 
            // levelOrder_modeLabel
            // 
            this.levelOrder_modeLabel.AutoSize = true;
            this.levelOrder_modeLabel.Location = new System.Drawing.Point(8, 37);
            this.levelOrder_modeLabel.Name = "levelOrder_modeLabel";
            this.levelOrder_modeLabel.Size = new System.Drawing.Size(37, 13);
            this.levelOrder_modeLabel.TabIndex = 3;
            this.levelOrder_modeLabel.Text = "Mode:";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(levelOrder_mainPathLabel);
            this.panel1.Controls.Add(this.allowSameLevel);
            this.panel1.Controls.Add(levelOrder_maxBackwardsJumpLabel);
            this.panel1.Controls.Add(this.backJumpProb);
            this.panel1.Controls.Add(this.mainPathSelector);
            this.panel1.Controls.Add(levelOrder_backwardsJumpProbabilityLabel);
            this.panel1.Controls.Add(this.maxBackJump);
            this.panel1.Controls.Add(levelOrder_maxForwardsJumpLabel);
            this.panel1.Controls.Add(this.maxForwJump);
            this.panel1.Location = new System.Drawing.Point(3, 56);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 100);
            this.panel1.TabIndex = 5;
            // 
            // tabPageSET
            // 
            this.tabPageSET.Controls.Add(this.setLayout_adjustMissionCounts);
            this.tabPageSET.Controls.Add(this.setLayout_randomPartners);
            this.tabPageSET.Controls.Add(this.tempWarning);
            this.tabPageSET.Controls.Add(this.setLayout_groupBoxEnemyConfiguration);
            this.tabPageSET.Controls.Add(this.setLayout_randomWeaponsInBoxes);
            this.tabPageSET.Controls.Add(this.setLayout_Mode);
            this.tabPageSET.Controls.Add(this.setLayout_ModeLabel);
            this.tabPageSET.Controls.Add(this.randomSET);
            this.tabPageSET.Location = new System.Drawing.Point(4, 22);
            this.tabPageSET.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageSET.Name = "tabPageSET";
            this.tabPageSET.Size = new System.Drawing.Size(495, 192);
            this.tabPageSET.TabIndex = 5;
            this.tabPageSET.Text = "SET Layout";
            this.tabPageSET.UseVisualStyleBackColor = true;
            // 
            // tempWarning
            // 
            this.tempWarning.AutoSize = true;
            this.tempWarning.BackColor = System.Drawing.Color.Wheat;
            this.tempWarning.Location = new System.Drawing.Point(244, 55);
            this.tempWarning.Name = "tempWarning";
            this.tempWarning.Size = new System.Drawing.Size(136, 26);
            this.tempWarning.TabIndex = 14;
            this.tempWarning.Text = "Applies to all .cmn and .nrm\r\n.hrd layouts are ignored";
            // 
            // setLayout_groupBoxEnemyConfiguration
            // 
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(this.setLayout_haveShieldBlockProb);
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(setLayout_HaveShieldBlockProbLabel);
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(this.setLayout_enemyRandomWeapons);
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(this.setLayout_enemyOverrideShieldBlock);
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(this.setLayout_keepAffiliation);
            this.setLayout_groupBoxEnemyConfiguration.Controls.Add(this.setLayout_keepType);
            this.setLayout_groupBoxEnemyConfiguration.Enabled = false;
            this.setLayout_groupBoxEnemyConfiguration.Location = new System.Drawing.Point(8, 50);
            this.setLayout_groupBoxEnemyConfiguration.Name = "setLayout_groupBoxEnemyConfiguration";
            this.setLayout_groupBoxEnemyConfiguration.Size = new System.Drawing.Size(215, 126);
            this.setLayout_groupBoxEnemyConfiguration.TabIndex = 13;
            this.setLayout_groupBoxEnemyConfiguration.TabStop = false;
            this.setLayout_groupBoxEnemyConfiguration.Text = "Enemy Configuration";
            // 
            // setLayout_ModeLabel
            // 
            this.setLayout_ModeLabel.AutoSize = true;
            this.setLayout_ModeLabel.Location = new System.Drawing.Point(13, 29);
            this.setLayout_ModeLabel.Name = "setLayout_ModeLabel";
            this.setLayout_ModeLabel.Size = new System.Drawing.Size(37, 13);
            this.setLayout_ModeLabel.TabIndex = 10;
            this.setLayout_ModeLabel.Text = "Mode:";
            // 
            // tabPageSubtitles
            // 
            this.tabPageSubtitles.Controls.Add(this.subtitleAndVoicelineSpecifiedCharactersGroupBox);
            this.tabPageSubtitles.Controls.Add(this.subtitleAndVoicelineConfigurationGroupBox);
            this.tabPageSubtitles.Controls.Add(this.randomFNT);
            this.tabPageSubtitles.Location = new System.Drawing.Point(4, 22);
            this.tabPageSubtitles.Name = "tabPageSubtitles";
            this.tabPageSubtitles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSubtitles.Size = new System.Drawing.Size(495, 192);
            this.tabPageSubtitles.TabIndex = 2;
            this.tabPageSubtitles.Text = "Subtitles";
            this.tabPageSubtitles.UseVisualStyleBackColor = true;
            // 
            // subtitleAndVoicelineSpecifiedCharactersGroupBox
            // 
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_GUNSoldier);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_GUNCommander);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_BlackDoom);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Cheese);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Maria);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Cream);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Eggman);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Charmy);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Espio);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Vector);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Omega);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Rouge);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Amy);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Knuckles);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Tails);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Sonic);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Controls.Add(this.FNTCheckBox_Chars_Shadow);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Enabled = false;
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Location = new System.Drawing.Point(217, 6);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Name = "subtitleAndVoicelineSpecifiedCharactersGroupBox";
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Size = new System.Drawing.Size(183, 151);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.TabIndex = 9;
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.TabStop = false;
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.Text = "Specified Characters";
            // 
            // FNTCheckBox_Chars_GUNSoldier
            // 
            this.FNTCheckBox_Chars_GUNSoldier.AutoSize = true;
            this.FNTCheckBox_Chars_GUNSoldier.Enabled = false;
            this.FNTCheckBox_Chars_GUNSoldier.Location = new System.Drawing.Point(77, 94);
            this.FNTCheckBox_Chars_GUNSoldier.Name = "FNTCheckBox_Chars_GUNSoldier";
            this.FNTCheckBox_Chars_GUNSoldier.Size = new System.Drawing.Size(85, 17);
            this.FNTCheckBox_Chars_GUNSoldier.TabIndex = 16;
            this.FNTCheckBox_Chars_GUNSoldier.Text = "GUN Soldier";
            this.FNTCheckBox_Chars_GUNSoldier.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_GUNCommander
            // 
            this.FNTCheckBox_Chars_GUNCommander.AutoSize = true;
            this.FNTCheckBox_Chars_GUNCommander.Enabled = false;
            this.FNTCheckBox_Chars_GUNCommander.Location = new System.Drawing.Point(77, 111);
            this.FNTCheckBox_Chars_GUNCommander.Name = "FNTCheckBox_Chars_GUNCommander";
            this.FNTCheckBox_Chars_GUNCommander.Size = new System.Drawing.Size(109, 17);
            this.FNTCheckBox_Chars_GUNCommander.TabIndex = 15;
            this.FNTCheckBox_Chars_GUNCommander.Text = "GUN Commander";
            this.FNTCheckBox_Chars_GUNCommander.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_BlackDoom
            // 
            this.FNTCheckBox_Chars_BlackDoom.AutoSize = true;
            this.FNTCheckBox_Chars_BlackDoom.Enabled = false;
            this.FNTCheckBox_Chars_BlackDoom.Location = new System.Drawing.Point(77, 61);
            this.FNTCheckBox_Chars_BlackDoom.Name = "FNTCheckBox_Chars_BlackDoom";
            this.FNTCheckBox_Chars_BlackDoom.Size = new System.Drawing.Size(84, 17);
            this.FNTCheckBox_Chars_BlackDoom.TabIndex = 14;
            this.FNTCheckBox_Chars_BlackDoom.Text = "Black Doom";
            this.FNTCheckBox_Chars_BlackDoom.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Cheese
            // 
            this.FNTCheckBox_Chars_Cheese.AutoSize = true;
            this.FNTCheckBox_Chars_Cheese.Enabled = false;
            this.FNTCheckBox_Chars_Cheese.Location = new System.Drawing.Point(77, 47);
            this.FNTCheckBox_Chars_Cheese.Name = "FNTCheckBox_Chars_Cheese";
            this.FNTCheckBox_Chars_Cheese.Size = new System.Drawing.Size(62, 17);
            this.FNTCheckBox_Chars_Cheese.TabIndex = 13;
            this.FNTCheckBox_Chars_Cheese.Text = "Cheese";
            this.FNTCheckBox_Chars_Cheese.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Maria
            // 
            this.FNTCheckBox_Chars_Maria.AutoSize = true;
            this.FNTCheckBox_Chars_Maria.Enabled = false;
            this.FNTCheckBox_Chars_Maria.Location = new System.Drawing.Point(77, 77);
            this.FNTCheckBox_Chars_Maria.Name = "FNTCheckBox_Chars_Maria";
            this.FNTCheckBox_Chars_Maria.Size = new System.Drawing.Size(52, 17);
            this.FNTCheckBox_Chars_Maria.TabIndex = 9;
            this.FNTCheckBox_Chars_Maria.Text = "Maria";
            this.FNTCheckBox_Chars_Maria.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Cream
            // 
            this.FNTCheckBox_Chars_Cream.AutoSize = true;
            this.FNTCheckBox_Chars_Cream.Enabled = false;
            this.FNTCheckBox_Chars_Cream.Location = new System.Drawing.Point(77, 33);
            this.FNTCheckBox_Chars_Cream.Name = "FNTCheckBox_Chars_Cream";
            this.FNTCheckBox_Chars_Cream.Size = new System.Drawing.Size(56, 17);
            this.FNTCheckBox_Chars_Cream.TabIndex = 12;
            this.FNTCheckBox_Chars_Cream.Text = "Cream";
            this.FNTCheckBox_Chars_Cream.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Eggman
            // 
            this.FNTCheckBox_Chars_Eggman.AutoSize = true;
            this.FNTCheckBox_Chars_Eggman.Enabled = false;
            this.FNTCheckBox_Chars_Eggman.Location = new System.Drawing.Point(77, 19);
            this.FNTCheckBox_Chars_Eggman.Name = "FNTCheckBox_Chars_Eggman";
            this.FNTCheckBox_Chars_Eggman.Size = new System.Drawing.Size(65, 17);
            this.FNTCheckBox_Chars_Eggman.TabIndex = 11;
            this.FNTCheckBox_Chars_Eggman.Text = "Eggman";
            this.FNTCheckBox_Chars_Eggman.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Charmy
            // 
            this.FNTCheckBox_Chars_Charmy.AutoSize = true;
            this.FNTCheckBox_Chars_Charmy.Enabled = false;
            this.FNTCheckBox_Chars_Charmy.Location = new System.Drawing.Point(77, 131);
            this.FNTCheckBox_Chars_Charmy.Name = "FNTCheckBox_Chars_Charmy";
            this.FNTCheckBox_Chars_Charmy.Size = new System.Drawing.Size(61, 17);
            this.FNTCheckBox_Chars_Charmy.TabIndex = 10;
            this.FNTCheckBox_Chars_Charmy.Text = "Charmy";
            this.FNTCheckBox_Chars_Charmy.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Espio
            // 
            this.FNTCheckBox_Chars_Espio.AutoSize = true;
            this.FNTCheckBox_Chars_Espio.Enabled = false;
            this.FNTCheckBox_Chars_Espio.Location = new System.Drawing.Point(6, 131);
            this.FNTCheckBox_Chars_Espio.Name = "FNTCheckBox_Chars_Espio";
            this.FNTCheckBox_Chars_Espio.Size = new System.Drawing.Size(52, 17);
            this.FNTCheckBox_Chars_Espio.TabIndex = 8;
            this.FNTCheckBox_Chars_Espio.Text = "Espio";
            this.FNTCheckBox_Chars_Espio.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Vector
            // 
            this.FNTCheckBox_Chars_Vector.AutoSize = true;
            this.FNTCheckBox_Chars_Vector.Enabled = false;
            this.FNTCheckBox_Chars_Vector.Location = new System.Drawing.Point(6, 117);
            this.FNTCheckBox_Chars_Vector.Name = "FNTCheckBox_Chars_Vector";
            this.FNTCheckBox_Chars_Vector.Size = new System.Drawing.Size(57, 17);
            this.FNTCheckBox_Chars_Vector.TabIndex = 7;
            this.FNTCheckBox_Chars_Vector.Text = "Vector";
            this.FNTCheckBox_Chars_Vector.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Omega
            // 
            this.FNTCheckBox_Chars_Omega.AutoSize = true;
            this.FNTCheckBox_Chars_Omega.Enabled = false;
            this.FNTCheckBox_Chars_Omega.Location = new System.Drawing.Point(6, 103);
            this.FNTCheckBox_Chars_Omega.Name = "FNTCheckBox_Chars_Omega";
            this.FNTCheckBox_Chars_Omega.Size = new System.Drawing.Size(60, 17);
            this.FNTCheckBox_Chars_Omega.TabIndex = 6;
            this.FNTCheckBox_Chars_Omega.Text = "Omega";
            this.FNTCheckBox_Chars_Omega.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Rouge
            // 
            this.FNTCheckBox_Chars_Rouge.AutoSize = true;
            this.FNTCheckBox_Chars_Rouge.Enabled = false;
            this.FNTCheckBox_Chars_Rouge.Location = new System.Drawing.Point(6, 89);
            this.FNTCheckBox_Chars_Rouge.Name = "FNTCheckBox_Chars_Rouge";
            this.FNTCheckBox_Chars_Rouge.Size = new System.Drawing.Size(58, 17);
            this.FNTCheckBox_Chars_Rouge.TabIndex = 5;
            this.FNTCheckBox_Chars_Rouge.Text = "Rouge";
            this.FNTCheckBox_Chars_Rouge.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Amy
            // 
            this.FNTCheckBox_Chars_Amy.AutoSize = true;
            this.FNTCheckBox_Chars_Amy.Enabled = false;
            this.FNTCheckBox_Chars_Amy.Location = new System.Drawing.Point(6, 75);
            this.FNTCheckBox_Chars_Amy.Name = "FNTCheckBox_Chars_Amy";
            this.FNTCheckBox_Chars_Amy.Size = new System.Drawing.Size(46, 17);
            this.FNTCheckBox_Chars_Amy.TabIndex = 4;
            this.FNTCheckBox_Chars_Amy.Text = "Amy";
            this.FNTCheckBox_Chars_Amy.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Knuckles
            // 
            this.FNTCheckBox_Chars_Knuckles.AutoSize = true;
            this.FNTCheckBox_Chars_Knuckles.Enabled = false;
            this.FNTCheckBox_Chars_Knuckles.Location = new System.Drawing.Point(6, 61);
            this.FNTCheckBox_Chars_Knuckles.Name = "FNTCheckBox_Chars_Knuckles";
            this.FNTCheckBox_Chars_Knuckles.Size = new System.Drawing.Size(70, 17);
            this.FNTCheckBox_Chars_Knuckles.TabIndex = 3;
            this.FNTCheckBox_Chars_Knuckles.Text = "Knuckles";
            this.FNTCheckBox_Chars_Knuckles.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Tails
            // 
            this.FNTCheckBox_Chars_Tails.AutoSize = true;
            this.FNTCheckBox_Chars_Tails.Enabled = false;
            this.FNTCheckBox_Chars_Tails.Location = new System.Drawing.Point(6, 47);
            this.FNTCheckBox_Chars_Tails.Name = "FNTCheckBox_Chars_Tails";
            this.FNTCheckBox_Chars_Tails.Size = new System.Drawing.Size(48, 17);
            this.FNTCheckBox_Chars_Tails.TabIndex = 2;
            this.FNTCheckBox_Chars_Tails.Text = "Tails";
            this.FNTCheckBox_Chars_Tails.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Sonic
            // 
            this.FNTCheckBox_Chars_Sonic.AutoSize = true;
            this.FNTCheckBox_Chars_Sonic.Enabled = false;
            this.FNTCheckBox_Chars_Sonic.Location = new System.Drawing.Point(6, 33);
            this.FNTCheckBox_Chars_Sonic.Name = "FNTCheckBox_Chars_Sonic";
            this.FNTCheckBox_Chars_Sonic.Size = new System.Drawing.Size(53, 17);
            this.FNTCheckBox_Chars_Sonic.TabIndex = 1;
            this.FNTCheckBox_Chars_Sonic.Text = "Sonic";
            this.FNTCheckBox_Chars_Sonic.UseVisualStyleBackColor = true;
            // 
            // FNTCheckBox_Chars_Shadow
            // 
            this.FNTCheckBox_Chars_Shadow.AutoSize = true;
            this.FNTCheckBox_Chars_Shadow.Enabled = false;
            this.FNTCheckBox_Chars_Shadow.Location = new System.Drawing.Point(6, 19);
            this.FNTCheckBox_Chars_Shadow.Name = "FNTCheckBox_Chars_Shadow";
            this.FNTCheckBox_Chars_Shadow.Size = new System.Drawing.Size(65, 17);
            this.FNTCheckBox_Chars_Shadow.TabIndex = 0;
            this.FNTCheckBox_Chars_Shadow.Text = "Shadow";
            this.FNTCheckBox_Chars_Shadow.UseVisualStyleBackColor = true;
            // 
            // subtitleAndVoicelineConfigurationGroupBox
            // 
            this.subtitleAndVoicelineConfigurationGroupBox.Controls.Add(this.FNTCheckBox_GiveAudioToNoLinkedAudio);
            this.subtitleAndVoicelineConfigurationGroupBox.Controls.Add(this.FNTCheckBox_NoDuplicatesPreRandomization);
            this.subtitleAndVoicelineConfigurationGroupBox.Controls.Add(this.FNTCheckBox_NoSystemMessages);
            this.subtitleAndVoicelineConfigurationGroupBox.Controls.Add(this.FNTCheckBox_OnlyLinkedAudio);
            this.subtitleAndVoicelineConfigurationGroupBox.Controls.Add(this.FNTCheckBox_SpecificCharacters);
            this.subtitleAndVoicelineConfigurationGroupBox.Enabled = false;
            this.subtitleAndVoicelineConfigurationGroupBox.Location = new System.Drawing.Point(8, 29);
            this.subtitleAndVoicelineConfigurationGroupBox.Name = "subtitleAndVoicelineConfigurationGroupBox";
            this.subtitleAndVoicelineConfigurationGroupBox.Size = new System.Drawing.Size(203, 102);
            this.subtitleAndVoicelineConfigurationGroupBox.TabIndex = 8;
            this.subtitleAndVoicelineConfigurationGroupBox.TabStop = false;
            this.subtitleAndVoicelineConfigurationGroupBox.Text = "Subtitles / Voicelines Configuration";
            // 
            // tabPageMusic
            // 
            this.tabPageMusic.Controls.Add(this.randomMusicSkipChaosPowers);
            this.tabPageMusic.Controls.Add(this.randomMusicSkipRankTheme);
            this.tabPageMusic.Controls.Add(this.randomMusic);
            this.tabPageMusic.Location = new System.Drawing.Point(4, 22);
            this.tabPageMusic.Name = "tabPageMusic";
            this.tabPageMusic.Size = new System.Drawing.Size(495, 192);
            this.tabPageMusic.TabIndex = 3;
            this.tabPageMusic.Text = "Music";
            this.tabPageMusic.UseVisualStyleBackColor = true;
            // 
            // randomMusicSkipChaosPowers
            // 
            this.randomMusicSkipChaosPowers.AutoSize = true;
            this.randomMusicSkipChaosPowers.Location = new System.Drawing.Point(14, 49);
            this.randomMusicSkipChaosPowers.Margin = new System.Windows.Forms.Padding(2);
            this.randomMusicSkipChaosPowers.Name = "randomMusicSkipChaosPowers";
            this.randomMusicSkipChaosPowers.Size = new System.Drawing.Size(170, 17);
            this.randomMusicSkipChaosPowers.TabIndex = 9;
            this.randomMusicSkipChaosPowers.Text = "Skip Chaos Power Use Jingles";
            this.randomMusicSkipChaosPowers.UseVisualStyleBackColor = true;
            this.randomMusicSkipChaosPowers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // randomMusicSkipRankTheme
            // 
            this.randomMusicSkipRankTheme.AutoSize = true;
            this.randomMusicSkipRankTheme.Location = new System.Drawing.Point(14, 28);
            this.randomMusicSkipRankTheme.Margin = new System.Windows.Forms.Padding(2);
            this.randomMusicSkipRankTheme.Name = "randomMusicSkipRankTheme";
            this.randomMusicSkipRankTheme.Size = new System.Drawing.Size(112, 17);
            this.randomMusicSkipRankTheme.TabIndex = 8;
            this.randomMusicSkipRankTheme.Text = "Skip Rank Theme";
            this.randomMusicSkipRankTheme.UseVisualStyleBackColor = true;
            this.randomMusicSkipRankTheme.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // tabPageSpoilers
            // 
            this.tabPageSpoilers.Controls.Add(this.makeChartButton);
            this.tabPageSpoilers.Controls.Add(this.saveLogButton);
            this.tabPageSpoilers.Controls.Add(this.spoilerLevelInfo);
            this.tabPageSpoilers.Controls.Add(this.spoilerLevelList);
            this.tabPageSpoilers.Location = new System.Drawing.Point(4, 22);
            this.tabPageSpoilers.Name = "tabPageSpoilers";
            this.tabPageSpoilers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSpoilers.Size = new System.Drawing.Size(495, 192);
            this.tabPageSpoilers.TabIndex = 1;
            this.tabPageSpoilers.Text = "Spoilers";
            this.tabPageSpoilers.UseVisualStyleBackColor = true;
            // 
            // spoilerLevelInfo
            // 
            this.spoilerLevelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spoilerLevelInfo.Location = new System.Drawing.Point(225, 6);
            this.spoilerLevelInfo.Multiline = true;
            this.spoilerLevelInfo.Name = "spoilerLevelInfo";
            this.spoilerLevelInfo.ReadOnly = true;
            this.spoilerLevelInfo.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.spoilerLevelInfo.Size = new System.Drawing.Size(264, 151);
            this.spoilerLevelInfo.TabIndex = 1;
            this.spoilerLevelInfo.WordWrap = false;
            // 
            // spoilerLevelList
            // 
            this.spoilerLevelList.Enabled = false;
            this.spoilerLevelList.FormattingEnabled = true;
            this.spoilerLevelList.Location = new System.Drawing.Point(6, 6);
            this.spoilerLevelList.Name = "spoilerLevelList";
            this.spoilerLevelList.Size = new System.Drawing.Size(213, 173);
            this.spoilerLevelList.TabIndex = 0;
            this.spoilerLevelList.SelectedIndexChanged += new System.EventHandler(this.spoilerLevelList_SelectedIndexChanged);
            // 
            // tabPageProgramOptions
            // 
            this.tabPageProgramOptions.Controls.Add(this.checkBoxProgramSound);
            this.tabPageProgramOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageProgramOptions.Name = "tabPageProgramOptions";
            this.tabPageProgramOptions.Size = new System.Drawing.Size(495, 192);
            this.tabPageProgramOptions.TabIndex = 4;
            this.tabPageProgramOptions.Text = "Program Options";
            this.tabPageProgramOptions.UseVisualStyleBackColor = true;
            // 
            // checkBoxProgramSound
            // 
            this.checkBoxProgramSound.AutoSize = true;
            this.checkBoxProgramSound.Checked = true;
            this.checkBoxProgramSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProgramSound.Location = new System.Drawing.Point(14, 6);
            this.checkBoxProgramSound.Name = "checkBoxProgramSound";
            this.checkBoxProgramSound.Size = new System.Drawing.Size(57, 17);
            this.checkBoxProgramSound.TabIndex = 10;
            this.checkBoxProgramSound.Text = "Sound";
            this.checkBoxProgramSound.UseVisualStyleBackColor = true;
            this.checkBoxProgramSound.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharedMouseDown_Chkchk);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.Filter = "Text Files|*.txt;*.log";
            this.saveFileDialog1.RestoreDirectory = true;
            // 
            // saveFileDialog2
            // 
            this.saveFileDialog2.DefaultExt = "png";
            this.saveFileDialog2.Filter = "PNG Files|*.png";
            // 
            // setLayout_adjustMissionCounts
            // 
            this.setLayout_adjustMissionCounts.AutoSize = true;
            this.setLayout_adjustMissionCounts.Enabled = false;
            this.setLayout_adjustMissionCounts.Location = new System.Drawing.Point(247, 145);
            this.setLayout_adjustMissionCounts.Name = "setLayout_adjustMissionCounts";
            this.setLayout_adjustMissionCounts.Size = new System.Drawing.Size(129, 17);
            this.setLayout_adjustMissionCounts.TabIndex = 16;
            this.setLayout_adjustMissionCounts.Text = "Adjust Mission Counts";
            this.toolTip1.SetToolTip(this.setLayout_adjustMissionCounts, "Missions counts are adjusted if enemy counts of affiliation are modified");
            this.setLayout_adjustMissionCounts.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.randomizeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 259);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.randomizeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Shadow the Hedgehog Randomizer ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backJumpProb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setLayout_haveShieldBlockProb)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageLevelOrder.ResumeLayout(false);
            this.tabPageLevelOrder.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageSET.ResumeLayout(false);
            this.tabPageSET.PerformLayout();
            this.setLayout_groupBoxEnemyConfiguration.ResumeLayout(false);
            this.setLayout_groupBoxEnemyConfiguration.PerformLayout();
            this.tabPageSubtitles.ResumeLayout(false);
            this.tabPageSubtitles.PerformLayout();
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.ResumeLayout(false);
            this.subtitleAndVoicelineSpecifiedCharactersGroupBox.PerformLayout();
            this.subtitleAndVoicelineConfigurationGroupBox.ResumeLayout(false);
            this.subtitleAndVoicelineConfigurationGroupBox.PerformLayout();
            this.tabPageMusic.ResumeLayout(false);
            this.tabPageMusic.PerformLayout();
            this.tabPageSpoilers.ResumeLayout(false);
            this.tabPageSpoilers.PerformLayout();
            this.tabPageProgramOptions.ResumeLayout(false);
            this.tabPageProgramOptions.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox randomSeed;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ComboBox mainPathSelector;
		private System.Windows.Forms.NumericUpDown maxBackJump;
		private System.Windows.Forms.NumericUpDown maxForwJump;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageLevelOrder;
		private System.Windows.Forms.TabPage tabPageSpoilers;
		private System.Windows.Forms.Button randomizeButton;
		private System.Windows.Forms.ListBox spoilerLevelList;
		private System.Windows.Forms.TextBox spoilerLevelInfo;
		private System.Windows.Forms.NumericUpDown backJumpProb;
		private System.Windows.Forms.CheckBox allowSameLevel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox levelOrderModeSelector;
		private System.Windows.Forms.Label levelOrder_modeLabel;
		private System.Windows.Forms.Button saveLogButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button makeChartButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private System.Windows.Forms.TabPage tabPageSubtitles;
		private System.Windows.Forms.CheckBox includeLast;
        private System.Windows.Forms.CheckBox randomFNT;
		private System.Windows.Forms.CheckBox includeBosses;
		private System.Windows.Forms.GroupBox subtitleAndVoicelineConfigurationGroupBox;
		private System.Windows.Forms.CheckBox FNTCheckBox_SpecificCharacters;
		private System.Windows.Forms.CheckBox FNTCheckBox_NoDuplicatesPreRandomization;
		private System.Windows.Forms.CheckBox FNTCheckBox_NoSystemMessages;
		private System.Windows.Forms.CheckBox FNTCheckBox_OnlyLinkedAudio;
		private System.Windows.Forms.CheckBox FNTCheckBox_GiveAudioToNoLinkedAudio;
		private System.Windows.Forms.TextBox seedTextBox;
		private System.Windows.Forms.GroupBox subtitleAndVoicelineSpecifiedCharactersGroupBox;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Shadow;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Espio;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Vector;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Omega;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Rouge;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Amy;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Knuckles;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Tails;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Sonic;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Maria;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Charmy;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Eggman;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_GUNCommander;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_BlackDoom;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Cheese;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_Cream;
		private System.Windows.Forms.CheckBox FNTCheckBox_Chars_GUNSoldier;
		private System.Windows.Forms.TabPage tabPageMusic;
		private System.Windows.Forms.CheckBox randomMusic;
		private System.Windows.Forms.TabPage tabPageProgramOptions;
		private System.Windows.Forms.CheckBox checkBoxProgramSound;
		private System.Windows.Forms.TabPage tabPageSET;
		private System.Windows.Forms.CheckBox randomSET;
		private System.Windows.Forms.ComboBox setLayout_Mode;
		private System.Windows.Forms.Label setLayout_ModeLabel;
		private System.Windows.Forms.CheckBox randomMusicSkipRankTheme;
		private System.Windows.Forms.CheckBox setLayout_randomWeaponsInBoxes;
		private System.Windows.Forms.GroupBox setLayout_groupBoxEnemyConfiguration;
		private System.Windows.Forms.CheckBox setLayout_enemyRandomWeapons;
		private System.Windows.Forms.CheckBox setLayout_keepAffiliation;
		private System.Windows.Forms.CheckBox setLayout_keepType;
		private System.Windows.Forms.NumericUpDown setLayout_haveShieldBlockProb;
		private System.Windows.Forms.CheckBox setLayout_enemyOverrideShieldBlock;
		private System.Windows.Forms.Label tempWarning;
		private System.Windows.Forms.CheckBox randomMusicSkipChaosPowers;
		private System.Windows.Forms.CheckBox setLayout_randomPartners;
		private System.Windows.Forms.CheckBox setLayout_adjustMissionCounts;
	}
}