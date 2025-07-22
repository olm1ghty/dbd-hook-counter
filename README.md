![banner](images/banner.png)
)

## *Display hook stages when playing killer*
![ui example](images/ui_example.png)

This is a Dead by Daylight hook counter for killers (with additional features). It works by visually monitoring survivors' statuses on the screen and then displays them as an overlay.

## Features
    - Fully automatic
    - Has manual controls if you don't like automatic tracking
    - Hook counter
    - DS timer
    - Off-hook endurance timer
    - Supports 16:9 and 16:10 aspect ratios
    - Only visible in the match, hides itself in the menu
    - Doesn't trigger anti-cheat (we don't hack into the game's memory, we're just using visual data on the screen)
    - Doesn't breach EULA (it's as legal as putting a note on the screen)

## How to use
> Disclaimer: Supported platform: only Windows.
> Disclaimer: Works with DBD in `Windowed Fullscreen` mode. Doesn't work in Fullscreen mode - this mode doesn't allow for the overlay to be displayed on top of it. 

    1. Launch "DBD Hook Counter.exe"
    2. Shift + M to open settings.
    3. Set UI and HUD scale to match those you have in DBD's graphics settings.
    4. Save settings.
    5. Play DBD while this app is running, and it will do its thing automatically.

![settings](images/settings.png)

You will know that it's set up correctly by seeing empty hook counters on the UI (only when the match starts):

![ui on](images/ui_on.png)

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

## Acknowledgements
[@Nemonn](https://github.com/nemonn) - Thanks for helping out with the materials and for the good advice!
