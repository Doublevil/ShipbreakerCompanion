# Shipbreaker Companion

## Overview

The Shipbreaker Companion is a tool for the game Hardspace: Shipbreaker by Blackbird Interactive, designed to provide features specifically aimed at the speedrunning community. The goal is to enable speedrunners (newcomers or not) to easily challenge categories that would otherwise be complex to set up.

It is only compatible with Windows and will not be released for other systems.

The speedrunning community pushed for new categories that would be more interesting than the existing ones. The main issue was that due to the way the different game modes are setup, the rules of such categories would be hard to enforce, and require manual setup beforehand. This tool is here to solve this problem.

## Features

### Load any competitive ship onto an existing profile save

This feature is designed to address the individual ship categories. For these categories, the runner is required to load a specific ship file into their saved profile.

When starting up, the Companion will automatically find the save files of the game, read some data in them, and display each profile to the user. The user will then be able to select one of the profiles so that they can load ships on that specific profile.

Meanwhile, the Companion will also download a curated list of ships (matching each speedrun subcategory), present the user with the list of available ships, and allow them to attach any of the ships to the currently selected profile, by clicking a single button.

When the user wants to compete for a particular individual ship category, they just have to find the right ship in the list and click the load button. Their in-game ship will then instantly be the right one.

### Coming soon? Display a salvaging indicator overlay

This feature is still not confirmed.

It would theoretically allow runners to visualize a simple percentage (or more complex indicators) representing the salvaging state of the ship being worked on.

The speedrunning community would ideally have a rule for individual ship categories that you'd have to salvage 95% of the ship before the run is considered complete. However, depending on the game mode, this percentage is either only available when pausing (not ideal) or not available at all (definitely not ideal).

The goal of this feature is to display this percentage as an overlay on top of the game constantly, to make it more convenient for everyone involved. It could even be expanded even further with graphics and fancy other features, but the primary goal is to display the current percentage.

## Technical overview (under the hood)

This section is aimed at people who might be interested in the technical aspect of this project.

The Shipbreaker Companion is written in C# .net 6, and uses WPF.

### Reading profile save files

Save files for this game are always found in the same AppData/LocalLow subdirectory. There can be at most 4 different profile saves (.lpw), matching the 4 difficulties of the game. The naming scheme of the save files, however, is a mystery to me. It contains a sequence of numbers that seems to have no consistency between different players/machines. An example file name would be "vglpp3_250774108.lpw".

The challenge was to still present the user with some information that would allow them to differentiate each of their profile saves, since the file name would clearly not do that.

Using Cheat Engine and hexadecimal editors, I tried finding different values that would be interesting to show or manipulate. However, I was only able to find two: the profile name (as entered in-game by the player), and the rank number.

By seeking at certain adresses in the file and doing sequential byte reads, I was able to successfuly read these values. For now, the Companion only shows the profile name, which should be enough in most cases to recognize which save is which.

### File manipulation

One of the main features is to be able to attach a ship file to a profile save. This is done simply by copying the ship file in the same directory as the profile save, with the same name, but the extension ".ship" instead of ".lpw". This is pretty straightforward. When a ship isn't available locally, the Companion will simply download it and write it to a particular folder where it will be found next time.

The only interesting piece of challenge (if you want to call it that) is that I wanted the Companion to assert the local ship file integrity as compared with the remote file. This should ensure that everyone competes with the same ships, even in the event of a change in one of the ships, or a local file manipulation by a runner. In order to do that, I included an MD5 file checksum in the ship list that's downloaded by the Companion on startup. When loading a ship, the Companion will produce a checksum of the local file and check it against the expected checksum retrieved from the list. If they don't match, the file will be (re-)downloaded.

### UI responsiveness

I enjoy working with WPF and wanted to include a little bit of a challenge for myself. Despite the very limited scope of the tool, it performs a lot of IO operations. One of my self-imposed challenges was to avoid at all cost blocking the UI thread during these operations, and also have visual loading indicators on the UI for every asynchronous action. Moreover, I did not want to use disruptive UI elements (dialogs and the like).

It's not immediately obvious when you use the tool, because most operations are almost instantly resolved if you have decent hardware, but I did implement (and test) a variety of loading indicators for every possible situation.

### In-game memory reading

The overlay feature would only be possible through 2 ways. One is modding the game (modifying game files), the other is reading the memory of the original, untouched game while it's running. Because I feel like allowing mods can facilitate cheating, I'm leaning more towards the latter.

Observing a process' memory is relatively common in speedrunning, mostly to build timing scripts that remove loading times or that automatically split. It is, however, a very complex endeavor. It requires spending time analyzing ASM code and memory pointers with tools like Cheat Engine, in order to determine a sequence of pointers that would allow you to always find what you're looking for, no matter the state of the game (what the player does) or how the game juggles with its internal memory.

Hardspace: Shipbreaker is built with the game engine Unity. Because this engine is built with .net, this means that we can inspect unobfuscated classes and their members with more powerful tools. However, because of the complexity of the engine, it also means that finding a valid pointer sequence is more complex in most cases (layers upon layers of references).

I have yet to dig into it more thoroughly, so I will update this category once I'm done.
