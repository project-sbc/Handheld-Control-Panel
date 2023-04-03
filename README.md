# Handheld-Control-Panel

This software is aimed to be a one stop shop for windows gaming handhelds. It is designed to be controller friendly and convenient to use and not instrusive while working with other software seamlessly. 

# PLEASE READ THIS:
THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.

## Download
Head to the link [here](https://github.com/project-sbc/Handheld-Control-Panel/releases) to download the latest version.

## Current Features.
### Power Controls
TDP, max cpu clock, max cores, EPP, and gpu clock (for amd 4000 and amd 6000 and newer devices).

### Display Controls
Change resolution, refresh rate, and display scaling quickly.

### RTSS
FPS limiter integration with RTSS. Can limit FPS for your games. Also used for on screen displays.

### System Controls
Change brightness and volume and toggle mute, wifi and BT.

### Customizable Home Page
The controls on the home can be rearranged in any order or disabled.

### Profiles
Configure per app/game settings including FPS limits, power controls, and display controls that will apply if launched from my app OR will be detected automatically. Exception: display changes won't occur if game is detected automatically.

### Hot Keys
<p>Configure controller or keyboard hotkeys for numerous actions. As many hotkeys can be made as desired. Actions include:
Adjusting TDP, brightness, gpu clock (on supported devices), and volume</p>
<p>Launching a game/app</p>
<p>Launching steam big picture mode or playnite full screen</p>
<p>Toggling mouse mode</p>
<p>Toggling on screen keyboard</p>

### Mouse Mode
Configure your controller to act as a mouse/keyboard. 

### Game Launcher
Convenient way to launch games from popular game launchers including Steam, Epic Games, Battle.net, Origin, GOG, and Rockstar games. Profiles will auto apply including display changes when using this.

### Settings
Ability to change appearances such as light/dark mode, accent colors, menu on left or right and update settings. For Intel devices, you can change the TDP mode to best suit your device. Auto start and auto update are additional setting parameters. Ability to change max/min parameters can also be configured.

### Power Menu
Ability to hide or close handheld control panel, ability to close a running game (detected by RTSS), or restart/shutdown computer.

# Compatible Devices (Windows only)
## Aya
All aya devices are compatible with most features. Only newer devices such as the aya air and aya neo 2 will have fan control support.

## One Netbook
All one netbook devices are compatible with most features. Only new devices such as the mini pro AMD and one x player 2 will have fan support.

## GPD
All GPD devices are compatible with most features. Only new devices such as the Win Max 2 AMD and win 4 will have fan support.

## Steam
Steam Deck is compatible but some features may not work due to the controller implementation. No fan support.

## Anbernic
Win600. No fan support.

## Intel Laptops
4th generation or newer laptops or mini PCs. No fan support.

## AMD Laptops
Most ryzen laptops or mini PCs. No fan support.

# Donate
<p>If you feel this software has helped you, please consider donating. I enjoy doing this for the community and am a consumer of my own software, so I don't do it for the money, but I have easily put 100's of hours into this project to bring you the best possible experience. Thank you.</p>
https://ko-fi.com/project_sbc

https://www.paypal.com/donate?business=NQFQSSJBTTYY4&currency_code=USD

# Fix Error on Intel Devices Running Windows 11 22H2

Go to RegEdit to the following address:  Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CI\Config
change the DWORD value to 0 like the screenshot below.
Restart the computer

![Intel22H2](https://github.com/project-sbc/Power-Control-Panel-v2/blob/master/Intel%2022H2%20driver%20fix.jpg?raw=true "Intel 22H2 fix")

# Projects incorporated into this
<ul>
  <li>RyzenAdj</li>
  <li>GameLib.net</li>
  <li>TEC controller</li>
  <li>QRes</li>
  <li>RTSS support - for changing FPS limits and displaying OSDs</li>
</ul>





