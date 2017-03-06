# Introduction
This is a flaskmanager plugin for POEHUD.

# Installing AutoFlask in PoeHUD
1. Hit Clone or Download -> Download as Zip
2. Extract the Folder from the zip file
3. Move the autoflaskmanger-master folder to the plugins folder in the poehud directory
4. launch poe
5. launch poehud
6. hit the poehud menu -> hover over plugins -> enable autoflaskmanager
7. Disable the "About" option to turn off the Splash.
8. Fiddle with the plugin settings to your satisfaction. 

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
