### Disclaimer
This project is a work in progress

### Randomization Features
* Level Order / Story Route Randomization with no deadlocking. You may warp back, but there is always a way forward! Keep trying! A detailed breakdown of Route Options is below.
* Level Exclusion Option - Exclude levels and bosses you don't want to play.
* Expert Mode Randomization - Builds a random order of Expert Mode based on all levels not excluded, including bosses.
* Layout Randomization - 64 MB / Dolphin ONLY - This has configurations to randomize enemies, weapons, and partners. Only compatible with Dolphin due to the extra RAM usage from object usage in stage where they are not normally present.
* Subtitles Randomization - This randomizes the in-game subtitles / audio hints. Has configurations, including a generator option.
* Music Randomization - This randomizes the in-game music, split up by category. Also supports including Custom Music in the randomization.
* Model Randomization - See further below in the README (Custom Models) for details. Requires pre-created packs.

### How to use / play
ShadowRando **requires** an Extracted Shadow the Hedgehog NTSC-U game. Instructions on how to extract the game are below if you are not familiar with extracting games using Dolphin.

1. Download the newest release from [the "releases" page](https://github.com/ShadowTheHedgehogHacking/ShadowRando/releases)
2. When launching the program, you will be prompted to choose your Extracted Shadow the Hedgehog folder. Choose the folder that contains both the `sys` and `files` folder.

For example, if I extracted my game to:

`..\Desktop\ShadowExtracted\`

I would pick that folder, not the `..\Desktop\ShadowExtracted\sys` folder, unlike Dolphin.

3. Configure the Randomization Options as you please
4. Click Randomize
5. Set Dolphin's MEM1 to 64 MB if using Layout Randomization (See below `Setting Dolphin MEM1 to 64MB` if you do not know how)
6. Launch the **Extracted Game** in Dolphin and enjoy!

### How to Interpret the Route Menu (Story Mode -> Pause In-Stage -> Y Button):
* Seeing a Dark Mission going to Digital Circuit can mean Digital Circuit, a Boss Stage, Westopolis, Last Way, or the end of route.
* Seeing a Normal Mission going to Glyphic Canyon can mean Glyphic Canyon, a Boss Stage, Westopolis, Last Way, or the end of route.
* Seeing a Hero Mission going to Lethal Highway can mean Lethal Highway, a Boss Stage, Westopolis, Last Way, or the end of route.
* All other cases reflect the actual mission you will progress to, though the possibility exists of a single boss in between the progression.

### Route Randomization Options

* [**Shuffle All Stages w/ Warps**](/RouteExamples/ShuffleAllWarps.png): this mode works by first shuffling all stages in the game into a random order, called the "main path". You can set it so you will always progress along the main path by clearing stages normally or if other missions may be involved. The remaining exits that are not on the main path will cause you to jump backwards or forwards along the main path, constrained by the "Max Backwards Jump" and "Max Forwards Jump" settings. In general, increasing the backwards jump makes it harder, increasing the forwards jump makes it easier.
* [**Vanilla Structure**](/RouteExamples/VanillaStructure.png): this mode will leave the general structure of the original game intact, but which level/boss you will find in each place will be different.
* [**Branching Paths**](/RouteExamples/BranchingPaths.png): this mode makes each exit from a level send you to a different level, giving you multiple routes through the game on the same seed without getting stuck in a loop.
* [**Reverse Branching**](/RouteExamples/ReverseBranching.png): this mode works like Branching Paths, but it constructs the level order in reverse, which allows for more freedom in how many paths there can be, and allows for exits to warp you forwards and backwards in the tree.
* [**Boss Rush**](/RouteExamples/BossRush.png): play through all the game's bosses in a random order.
* [**Safe Wild**](/RouteExamples/SafeWild.png): only two rules are enforced: all stages must be accessible from the starting stage, and all stages must be able to reach the ending eventually.
* [**Wild**](/RouteExamples/Wild.png): no rules, any exit from a stage can lead to any other stage. The ending might not even be accessible!

#### Main Path Options
`Act Clear` -  clearing a stage through the neutral path (or the hero path if there is no neutral path) will always advance you by one level in the level order.

`Any Exit` - randomly selects one of the available exits from a level to take you to the next stage in the order.

### Layout Randomization Options

----
#### This feature requires setting Dolphin MEM1 to 64MB mode! Be aware of this if you choose to use Layout Randomization, otherwise your game will crash on level load.

#### A Warning about Reloaded <= 1.1 & 2P-Reloaded <= 1.0b
If you use the "Random Partners" option, you may be unable to complete certain missions. To fix this issue, download the '[missing_events_reloaded_based_roms](https://github.com/ShadowTheHedgehogHacking/ShadowRando/releases/download/0.4.0/missing_events_reloaded_based_roms.7z)' and merge these into your extracted game's `events` folder. It can be done before or after your randomization.

----

`Make CC Splines Vehicle Compatible` - turns most splines in the game into Black Hawk / Black Volt compatible splines. Allows you to use them in areas you normally cannot. Note, very experimental - expect weirdness.

`Adjust Mission Counts` - adjusts the mission counts based on the newly randomized enemies. Reduction % allows you to reduce the amount of enemies for a mission complete.
``

`Enemy - Keep Type` - keeps ground enemies mapped to ground enemies, and flying enemies mapped to flying enemies. Recommended.

`Enemy - Only Selected Enemy Types` - allows you to choose which enemies to use in randomization. Note you must have at least one ground and one flying enemy. Additionally, GUN Soldiers require at least one other enemy type due to Link ID constraints relating to them.

`Weapon - Random Weapons in Weapon Boxes` - randomizes weapon boxes only

`Weapon - Random Weapons in All Boxes` - randomizes all boxes to contain weapons, even if they had nothing or something else in them

`Weapon - Random Exposed Weapons` - randomizes weapons found outside of crates (such as Air Fleet Armory)

`Weapon - Environment Drops Random Weapons` - randomizes what drops when you destroy EnvWeapon objects (trees, stop signs, etc...)

`Weapon - Only Selected Weapons` - allows you to choose which weapons to use in randomization

`Partner - Keep Affiliation of Original Object` - Ensures originally dark partner objects only pick a dark partner, and hero only picks a hero partner. Recommended for non-reloaded variants of the game.

Both Enemy and Partner only feature "Original (untouched)" and "Wild" randomization options as of v0.5. In the future 1:1 mappings will be possible.

----

### Custom Music
Warning: in-game stage music expects louder source files. The rank/stage clear jingle is uniquely abnormally loud.
1. Create a folder `RandoMusic` in the same directory as the ShadowRando program.
2. Create `Stage.txt`, `Jingle.txt`, `Menu.txt`, and `Credits.txt` - each containing the filename(s) of the songs you want to be included in the randomization category. Note if you only want to add custom Stage music, you don't need to add the other `.txt` files besides `Stage.txt`. If you have subfolders, you must also create these files in those folders to include files from subfolders.
3. Place any `*.adx` music files that you reference in this folder, or subfolders. You cannot use `*.mp3 / *.wav`! You must convert to `*.adx`.

### Custom Models
The randomizer can select randomized models for Shadow and optionally the 2P Yellow Shadow. To use this feature, create a `RandoModels` folder in the same directory as the ShadowRando program using the following directory structure:
* `RandoModels` - You can place models here if they use the default bones, but for organization it's probably better to use another folder.
 * `DefaultBON` - Suggested name for a folder containing models using the default bones.
 * `CustomBON` - Models in this folder use custom bones. If a P2 model uses the same bones as a P1 model, they must be together in the same subfolder.
 * `ModelPack` - Models in this folder are treated as a pack; any other files in the same folder will be copied to the ROM. Use this when a model requires edits to partners to avoid crashes. P2 randomization is ignored if one of these is picked for P1.
Each model must be placed in its own folder, keeping the original file names (P1 models are `shadow.one`, P2 models are `shadow2py.one`). P1 and P2 models made as a set should be placed in the same folder, but can be selected independently of each other.

### Extraction of Game
1. Get the latest release or dev Dolphin - [5.0-21088 or newer recommended](https://dolphin-emu.org/download/)
2. (Optional: only do this step if you want to keep config separate) Before launching dolphin, create an empty file
   `portable.txt` in the same folder as Dolphin.exe
3. Open Dolphin
4. Set game path to your Shadow the Hedgehog NTSC-U ISO
5. Right-click `Shadow The Hedgehog` in the game list
6. Select `Properties`
7. Select `Filesystem` Tab
8. Right-click `Disc`
9. Select `Extract Entire Disc...`
10. Select a new folder where you will store the game content and modify its files

### Setting Dolphin MEM1 to 64MB
1. Open Dolphin
2. Click `Config`
3. Click `Advanced` Tab
4. In `Memory Override` section, check `Enable Emulated Memory Size Override`
5. Slide `MEM1` to 64 MB (maximum)

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