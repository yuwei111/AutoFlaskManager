# Introduction

This is a flaskmanager plugin for POEHUD.

# Installing AutoFlask in PoeHUD

- Hit Clone or Download -> Download as Zip
- Extract the Folder from the zip file
- Move the autoflaskmanger-master folder to the plugins folder in the poehud directory
- launch poe
- launch poehud
- hit the poehud menu -> hover over plugins -> enable autoflaskmanager
- Disable the "About" option to turn off the Splash.
- Fiddle with the plugin settings to your satisfaction. 

# F.A.Qs

```
There is an POEHUD/AutoFlaskManager update what should I do?
```
Delete the old folder before downloading and installing the new one. OR things will fail.

```
There is a POE game update, what should I do? who will change the offsets.
```
Wait for POEHUD to be updated. If POEHUD works, this will work too. No need to change offsets as it automatically ask POEHUD for all the offsets.

```
Does this plugin work with 32bit version of HUD?
```
NO, 32bit PoeHUD is being phased out, as the game itself shifts to 64bit operation exclusively.

```
How come This plugin doesn't have or do feature XYZ that orriginal AutoAHK had?
```
Because either its not flask related (ie DPS calculator, monsters remaining, etc) or it's not useful anymore.
We have no plans of implementing it, feel free to implement it if u want.

```
Does this Plugin support languages other than English?
```
NO, you are welcome to translate the source and fork it.

```
My AutoFlask Manager is not working/I have binded the flask to other keys.
```
Change the keys info in flaskbind.json which is in config folder of this flask manager.
You can look into following file
- C:\Users\[yourname]\Documents\My Games\Path of Exile\production_Config.ini

Look for following lines in the file
- use_flask_in_slot1=49
- use_flask_in_slot2=50
- use_flask_in_slot3=51
- use_flask_in_slot4=52
- use_flask_in_slot5=53

and add those numbers in the flaskbind.json respectively.

```
Flask Manager is too fask, and drink all my flask in 1 go.
```
You need to increase the delays of the flask manager. It's in milisecond, so you need to change it to 3000 to 5000 or anything you think is good enough.

```
What is debug mode/debug log. How can I enable/use it?
```

- Debug mode is only to provide debugging output to us.
- You need to enable the debug mode from the autoflaskmanager settings menu of poehud to enable this feature.
- This mode create a file in poehud.exe folder named as autoflaskmanagerdebug.log containing the debug logs.
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
