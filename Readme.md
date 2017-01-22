#Introduction
This is a flaskmanager plugin for POEHUD.

#Setting Up POEHUD in plugin Development environment

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
