![banner](readme_images/banner.png)

<div align="center">

### [Download](https://github.com/olm1ghty/dbd-hook-counter/releases/download/release/DBD.Hook.Counter.rar)
**SHA256**: `0B555ED4FDF7CE89CC0A9FD864F4A05A6ED32BC742378E1CD5901F5AA38864E8`


## *Display hook stages when playing killer*
![ui example](readme_images/ui_example.png)

</div>

This is a Dead by Daylight hook counter for killers (with additional features). It works by visually monitoring survivors' statuses on the screen and then displays them as an overlay.

## Features
- ⚙️ Fully automatic (HUD-based tracking)
- 🧠 Manual mode if you prefer full control
- 🎯 Hook stage counter for all survivors
- ⏱️ DS & Endurance timers (optional)
- 🖥️ Works on any resolution (16:9, 16:10)
- 🔒 Doesn’t store or send any data — it's all local and discarded immediately
- 🧼 Hides itself in menus, only shows in match
- 🧩 Doesn’t trigger anti-cheat (no memory access)
- 🧾 EULA-safe (like putting a sticky note on your screen)
- 🛠️ Fully open-source & customizable

<details>
<summary>🛡️ Why it's safe</summary>

- No game files are modified
- No memory is read
- Doesn’t interact with DBD’s process
- Just an overlay that analyzes HUD pixels

Think of it like a sticky note on your screen — legal, simple, and safe.
</details>

## How to use
> Supported platforms: only Windows.
---
> Works with DBD in `Windowed Fullscreen` mode. Doesn't work in Fullscreen mode - this mode doesn't allow for the overlay to be displayed on top of it.
---
> Multiple monitors? Launch the app on the same monitor you launch DBD.
---

1. Launch the app.
2. Shift + M to open settings.
3. Set UI and HUD scale to match those you have in DBD's graphics settings.
4. Save settings.
5. Play DBD while this app is running, and it will do its thing automatically.

<div align="center">

![settings](readme_images/settings.png)

</div>

You will know that it's set up correctly by seeing empty hook counters on the UI (only when the match starts):

![ui on](readme_images/ui_on.png)

</div>

If it doesn't work, try Shift + R to restart the app or make sure that you configured your UI/HUD scales correctly in the settings menu.

## Controls
- Shift + H - Hot keys.
- Shift + M - Settings.
- Shift + K - Exit the app.
- Shift + R - Restart the app.
- Shift + P - Pause/Unpause the app (if playing survivor, for example)
- 1/2/3/4 - Manually add a hook stage. If the hook counter is at 2, set it to 0.
- 5 - Clear all hook stages.
- Shift + 1/2/3/4 - Manually trigger unhook timers.
- Shift + 5 - Clear all timers.

## Antivirus
The app is unsigned, so your antivirus might view it as a threat.

**SHA256**: `0B555ED4FDF7CE89CC0A9FD864F4A05A6ED32BC742378E1CD5901F5AA38864E8`

This hash lets users verify the file is untampered. If you’re unsure, scan the file on [VirusTotal](https://virustotal.com/) or build from source.

## Acknowledgements
[@Nemonn](https://github.com/nemonn) - Thanks for helping out with the materials and for the good advice!