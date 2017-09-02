# Introduction

This is a highly customizable and intuitive Flask Manager plugin for POEHUD  
###### *Note: Plugins are only supported on the x64 branch of PoEHUD*

# Installing AutoFlask in PoeHUD  
- Hit Clone or Download -> Download as Zip
- Extract the Folder from the zip file
- Move the autoflaskmanger-master folder to the plugins folder in the poehud directory
- launch poe
- launch poehud
- hit the poehud menu -> hover over plugins -> enable autoflaskmanager
- Disable the "About" option to turn off the Splash.
- Fiddle with the plugin settings to your satisfaction.
- Note: As per normal HUD operation a red underline means disabled.

# AutoQuit  
- AutoQuit is moved to another plugin (https://github.com/zaafar/AutoQuit)

# F.A.Qs  
###### *Please skim through the FAQ to see if there is a solution to your issue. If not, please feel free to join the official [AutoFlaskManager Support](https://discord.gg/sK9JdJH) channel on discord and someone will be glad to assist you as soon as possible.*
###### *If the issue is still not resolved, please feel free to post a ticket on the Issues page with as much information as possible and try to include a detailed description as well as screenshots of the issue to ensure it is resolved as soon as possible.*  

```
how to set offensive flask usage to the right/left mouse button
```
- First of all thx to xxsevernajaxx for figuring this out!
- Go into your autoflask directory and open the config.ini file with a text editor (notepad++ works well) as shown below
![alt text](http://i.imgur.com/grKGDPP.png "Help Image 1")
- Find the code where offensive flasks are configured:

![alt text](http://i.imgur.com/m4XLfom.png "Help Image 2")

- Note: you might have something else standing there, replace the code between the blue marked brackets, not the brackets themselfs, only whats in the right box with the following:
```
"OffensiveWhenAttacking": false,
"UseWhileKeyPressed": true,
"KeyPressed": {
"Value": 2
```
- Now when you use a skill that is bound to your right mouse button autoflask manager will use your offensive flasks (only works if a skill is bound to the respective key from what I can tell)

- If you want to use some other key that is not recognized by default you can check this table to get the button's code:
http://cherrytree.at/misc/vk.htm
```
Auto Quit is too slow/Auto Quit isn't working.
```
- Make sure you have copied cports.exe to _*c:\Windows\System32*_
- Make sure you are using Predictive Network mode (_*PoE Options->UI->Network Mode*_ must be set from the log-in page).
- Make sure you've extracted the AutoQuit plugin, as well as AutoFlaskManager to _*\Plugins\*_ and that they're enabled in the menu
- If all else fails, the following steps are typically the best way to prepare yourself for requesting support which can then be used to diagnose the issue.
  1. Open the PoEHUD menu by clicking the three red lines at the top left while in-game
  2. Ensure AutoQuit and Flask Manager are enabled in the plugins section, and inspect the settings to make sure they're properly set
  3. Turn on debug mode ( _*Flask Manager->UI Settings*_ )
  4. Press F4 to force autoquit to attempt to quit
  5. Exit the game and open ErrorLog.txt in _*\Path of Exile\ErrorLog.txt*_
  6. If the issue isn't obvious, or remedied; take any applicable screenshots (settings menu opened to AutoQuit, and the error message if possible) and request assistance with the information you've obtained by going to the official [AutoQuit](https://github.com/zaafar/AutoQuit) Issues page, or the discord [support](https://discord.gg/sK9JdJH) channel
```
There is an POEHUD/AutoFlaskManager update what should I do?
```
Delete the old folder before downloading and installing the new one. OR things will fail.  
###### *You can take a screenshot of your settings, or write them down to make the update easier*

```
There is a POE game update, what should I do? who will change the offsets.
```
Wait for POEHUD to be updated. If POEHUD works, this will work too. No need to change offsets as they are automatically obtained from PoEHUD.

```
Does this plugin work with 32bit version of HUD?
```
NO, 32bit PoeHUD is being phased out as the game itself shifts to 64bit operation exclusively.

```
How come this plugin doesn't have or do 'feature XYZ' that original AutoAHK had?
```
Because either its not flask related (ie DPS calculator, monsters remaining, etc) or it's not useful anymore.
We have no plans of implementing it in the near future. Feel free to make a fork of the project and implement it if you want.

```
Does this Plugin support languages other than English?
```
Not at this time, however, anyone is welcome to translate the source and fork it.

```
My AutoFlask Manager is not working (I have binded the flask to other keys).
```
Change the keys info in flaskbind.json in the _*\Flask Manager\Config\*_ folder.

You can look in _*Documents\My Games\Path of Exile\production_Config.ini*_  
Search for following lines in the file:
- use_flask_in_slot1=49
- use_flask_in_slot2=50
- use_flask_in_slot3=51
- use_flask_in_slot4=52
- use_flask_in_slot5=53

and add those numbers in the flaskbind.json respectively.

```
Flask Manager is too fast, and all of my flasks are being consumed at once.
```
You need to increase the delay of the flask manager. It's in milliseconds, so 1000 = 1 second.  
Try increasing it in intervals of 250-500ms until the flask consumption suits your character's needs.

```
What is debug mode/debug log. How can I enable/use it?
```
- Debug mode is only to provide debugging output to help us provide support.
- You need to enable the debug mode from the Flask Manager settings in the PoEHUD menu to enable this feature.
- This mode creates a file in poehud.exe folder named as autoflaskmanagerdebug.log containing the debug logs.
- This file is updated/written after you close the game or close the poehud.exe.
- With this file also provide us with screenshots/use case and descriptions.

```
There is a lot of spam of logs.
```
Disable the debug log, it's for debugging only. If there is still a lot of spam after distabling the debug log, report to us, we will try to reduce it.

```
I am getting a warning "Warning: Speed Run mod is ignored on mana/life/hybrid flasks."
```
Use Alt orb on your mana/life/hybrid flask containing speed mod as mana/life/hybrid flask shouldn't contain speed mod.
This will greatly reduce the duration of a speed mod. Use utility flasks for speed mods.

```
There seems to be a logic error causing the plugin to not function correctly, will you look into it?
```
Yes, please provide details about usage case, screenshots and a debug log.

---------

# Setting Up POEHUD in plugin Development environment
- Download x64 POEHUD version as only x64 support plugins.
 - git clone https://github.com/TehCheat/PoEHUD.git -b x64
- Open ExileHUD.sln and select "Debug" from top menu.
- Run Build-all (Build->Build-all). Note: This will create the require folders in PoEHUD\src\bin\x64\Debug
- Now copy paste the following folders in Debug folder.
 - config
 - plugins
 - sounds
 - textures
- Now go into plugins folder that you have copied into the Debug folder and do git clone of this repo
 - git clone https://github.com/Xcesius/AutoFlaskManager.git
- Open "ExileHUD.sln" again and right click "Solution ExileHUD" -> Add -> Existing Project
	and select "FlaskManager.csproj" from the menu.
- Now add POEHUD reference.
 - Right click FlaskManager/References and select Add References
 - check mark Projects -> PoeHud
 - Click OK
- You are all done. Code and run with debugging.
