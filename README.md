![Roomsizer](/brandmark.png)

GameMaker Studio lets you resize your rooms, but it does not offer any options for anchoring the existing content of the room when doing so. Your assets, instances, and tiles, will always be locked to the top-left of the newly-resized room.

Now Roomsizer offers a traditional anchoring interface that should be familiar to users of any image editing software. No more resizing the room and then layer-by-layer dragging and dropping your content to the location you want it!

<img src="/screenshot.png" height="400" />

## DISCLAIMER

Roomsizer is an unofficial tool. It has been tested with GameMaker Studio 2.2.5.481.
It works on a reverse-engineering of the GameMaker room file format, which YoYo Games could change at any time.
If you are going to use Roomsizer **PLEASE** make sure you have a backup of your data first, or that your data is safely checked in to version control software like Git.

There will be an effort to keep Roomsizer working on newer versions of GameMaker as they are released, but don't be careless with your valuable room data!

## Get Roomsizer
Roomsizer is written for Windows 10 build 1809 and later.

You may have to enable "Sideloading" of apps. This just means allowing your computer to install Windows Apps that aren't from the Windows Store.\
To do so, go to **Settings > Update & Security > For Developers** and select "Sideload Apps".

Visit the [Releases page](https://github.com/daikon-games/roomsizer/releases) and download the latest release `zip` file. Extract the contents.\
Right-click on the file `Add-AppDevPackage.ps1` and select **Run with Powershell**. You will see a few prompts in the Powershell window, type `Y` and hit enter to accept these.

This script will install the self-signed certificate for Roomsizer, and then install the Roomsizer app on your computer. Once it successfully completes, you'll find Roomsizer in your Start menu and can begin using it!

## Using Roomsizer

Use the file browser to select a GameMaker Studio room `.yy` file. The current width and height of room will automatically load in.

Enter a new width and height for the room in pixels. \
**NOTE** currently this must be larger than the original width/height. Shrinking rooms may be added in a future update.

Select an anchor point for the resized room. For instance, choosing the top-left point preserves GameMaker Studio's original behavior.
Choosing the bottom-right point would have all the room's content in the bottom-right corner of the resized room, with all the additional space added on the top and left edges.
Choosing the center point will add space evenly around the top, bottom, and sides, to keep content centered in the resized room.

If you would like force your resize to a specific tile multiple without doing the math yourself, check the "Round up by tile size?" box, and enter the size in pixels of your tiles.
This will round *up* the values you entered for width and height to the nearest multiple of your tile size before applying the changes. For example, if you entered a width of 1000 and a tile size of 16, your actual new width would be 1008 pixels.

When you click "Resize", the changes will be applied to your room's `.yy` file. At this point to ensure things don't get out of sync, Roomsizer will disable the controls. If you want to edit the same room again you can simply click the Reload button to load the room file back in. Otherwise you can click Browse to pick a different room file to edit.

Once you bring the GameMaker Studio window back into focus, it will prompt you that a file has been changed and needs reloading. After the reload you will see your newly resized room!
