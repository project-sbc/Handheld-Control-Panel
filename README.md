# Handheld-Control-Panel

This software is aimed to be a one stop shop for windows gaming handhelds. It is designed to be conveint to use and not instrusive while working with other software seamlessly. 

# PLEASE READ THIS:
THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.

## Current Features.
### Power Controls
TDP, cpu clock, max cores, EPP, and gpu clock (for amd 4000 and amd 6000 and newer devices).

### Display Controls
Change resolution, refresh rate, and scaling quickly.

## Download
Head to the link [here](https://github.com/project-sbc/Handheld-Control-Panel/releases) to download the latest version.

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
If you feel this software has helped you, please consider donating. I enjoy doing this for the community and am a consumer of my own software, so I don't do it for the money, but I have easily put 100's of hours into this project to bring you the best possible experience. Thank you.
https://ko-fi.com/project_sbc

https://www.paypal.com/donate?business=NQFQSSJBTTYY4&currency_code=USD

# Fix Error on Intel Devices Running Windows 11 22H2

Go to RegEdit to the following address:  Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CI\Config
change the DWORD value to 0 like the screenshot below.
Restart the computer

![Intel22H2](https://github.com/project-sbc/Power-Control-Panel-v2/blob/master/Intel%2022H2%20driver%20fix.jpg?raw=true "Intel 22H2 fix")

# Projects incorporated into this
RyzenAdj
GameLib.net
EC controller
QRes
