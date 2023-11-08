### Disclaimer
This project is a work in progress

### Randomization Features
* Story Route Randomization with no deadlocking. You may warp back, but there is always a way forward! Keep trying! A detailed breakdown of Route Options is below.
* 'FNT' Randomization - This randomizes the in-game subtitles / audio hints. Currently if an entry has an audio association, it is kept. If it does not have one, a random audio line is picked.
* Music Randomization - This randomizes the in-game music, split up by category. Also supports including Custom Music in the randomization.

### How to use / play
ShadowRando **requires** an Extracted Shadow the Hedgehog USA game. Instructions on how to extract the game are below if you are not familiar with extracting games using Dolphin.

1. Download the newest release from [the "releases" page](https://github.com/ShadowTheHedgehogHacking/ShadowRando/releases)
2. When launching the program, you will be prompted to choose your Extracted Shadow the Hedgehog folder. Choose the folder that contains both the `sys` and `files` folder.

For example, if I extracted my game to:

`..\Desktop\ShadowExtracted\`

I would pick that folder, not the `..\Desktop\ShadowExtracted\sys` folder, unlike Dolphin.

3. Configure the Randomization Options as you please
4. Click Randomize
5. Launch the **Extracted Game** in Dolphin and enjoy!


### Route Randomization Options

* [**Shuffle All Stages w/ Warps**](/RouteExamples/ShuffleAllWarps.png): this mode works by first shuffling all stages in the game into a random order, called the "main path". You can set it so you will always progress along the main path by clearing stages normally or if other missions may be involved. The remaining exits that are not on the main path will cause you to jump backwards or forwards along the main path, constrained by the "Max Backwards Jump" and "Max Forwards Jump" settings. In general, increasing the backwards jump makes it harder, increasing the forwards jump makes it easier.
* [**Vanilla Structure**](/RouteExamples/VanillaStructure.png): this mode will leave the general structure of the original game intact, but which level/boss you will find in each place will be different.
* [**Branching Paths**](/RouteExamples/BranchingPaths.png): this mode makes each exit from a level send you to a different level, giving you multiple routes through the game on the same seed without getting stuck in a loop.
* [**Reverse Branching**](/RouteExamples/ReverseBranching.png): this mode works like Branching Paths, but it constructs the level order in reverse, which allows for more freedom in how many paths there can be, and allows for exits to warp you forwards and backwards in the tree.
* [**Wild**](/RouteExamples/Wild.png): no rules, any exit from a stage can lead to any other stage. The ending might not even be accessible!

#### Main Path Options
`Act Clear` -  clearing a stage through the neutral path (or the hero path if there is no neutral path) will always advance you by one level in the level order.

`Any Exit` - randomly selects one of the available exits from a level to take you to the next stage in the order.

### Custom Music
Warning: in-game stage music expects louder source files. The rank/stage clear jingle is uniquely abnormally loud.
1. Create a folder `RandoMusic` in the same directory as the ShadowRando program.
2. Create `Stage.txt`, `Jingle.txt`, `Menu.txt`, and `Credits.txt` - each containing the filename(s) of the songs you want to be included in the randomization category. Note if you only want to add custom Stage music, you don't need to add the other `.txt` files besides `Stage.txt`. If you have subfolders, you must also create these files in those folders to include files from subfolders.
3. Place any `*.adx` music files that you reference in this folder, or subfolders. You cannot use `*.mp3 / *.wav`! You must convert to `*.adx`.


### Extraction of Original Game / FST Format
1. Get the latest beta or dev Dolphin - [5.0-20201 or newer recommended](https://dolphin-emu.org/download/)
2. (Optional: only do this step if you want to keep config separate) Before launching dolphin, create an empty file
   `portable.txt` in the same folder as Dolphin.exe
3. Open Dolphin
4. Set game path to your Shadow the Hedgehog USA ISO
5. Right-click `Shadow The Hedgehog` in the game list
6. Select `Properties`
7. Select `Filesystem` Tab
8. Right-click `Disc`
9. Select `Extract Entire Disc...`
10. Select a new folder where you will store the game content and modify its files

### Playing the Extracted Game
1. Open Dolphin
2. Open `Config`
3. Select `Paths` Tab
5. Select `Add` for Game Folders
6. Navigate to the folder where you extracted the game
7. Open the `sys` folder, and select "Select Folder"
8. Close the `Config` window. Now your games list should have a new 0 filesize game of Shadow The Hedgehog. The 0 filesize entry is the Extracted game
9. You can now launch the game here if you wish to play in Extracted format

### Converting Extracted Game to ISO (OPTIONAL)
1. Right click the Extracted format game (0 filesize entry) and pick `Convert File...`
2. The Convert window will appear, click "Convert..." and name it `game.iso` for Nintendont, or `ShadowRando.iso` for Dolphin
3. Move/Save the ISO to the Path Dolphin detects your games. A new full-size entry should appear in your Dolphins game list. Use this when playing the game.