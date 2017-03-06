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
Q: There is an update what should I do?
A: Delete the old folder before downloading and installing the new one. OR things will fail.
```
```
Q: Does this plugin work with 32bit version of HUD?
A: NO, 32bit PoeHUD is being phased out, as the game itself shifts to 64bit operation exclusively.
```
```
Q: How come This plugin doesn't have or do feature XYZ?
A: Because its not flask related (ie DPS calculator, monsters remaining, etc). We have no plans of implementing it, feel free to implement it if u want.
```
```
Q: Does this Plugin support languages other than English?
A: NO, you are welcome to translate the source and fork it.
```
```
Q: My AutoFlask Manager is not working/I have binded the flask to other keys.
A: Change the keys info in flaskbind.json which is in config folder of this flask manager. You need to add ASCII of your keys, you can google ASCII table to lookup your keys to ASCII translation.
```
```
Q: Flask Manager is too fask, and drink all my flask in 1 go.
A: You need to increase the delays of the flask manager. It's in milisecond, so you need to change it to 3000 to 5000 or anything you think is good enough.
```
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
