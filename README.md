VRMOD.Template
====

Unity3Dで作成されたゲームをVR化するMOD.

MOD for making games created in Unity 3D compatible with VR.

## Requirement tool.
[IPA(Illusion Plugin Architecture)](https://github.com/Eusth/IPA)

[UABE(Unity Asset Bundle Extractor)](https://github.com/DerPopo/UABE)

## Using Source Code.

1. Checkout the project 
~~~
git clone --recursive https://github.com/obtdai/VRMOD.Template.git
cd VRMOD.Template
~~~
2. Open the Solution (*.sln) File.
3. Start the build in the Release configuration
This will generate all you need inside bin\Release. (This should be the same output as on the releases page).
If you've set the game directory, your game will also have been updated.

### Running
1. Patch your game by dragging its executable on IPA.exe (only has to be done once).
2. By default, the game will now boot into VR mode when SteamVR is running (make sure Desktop Game Theatre is disabled).

## Usage the release.

1. Download release from the release page
2. Extract contents into a Unity folder
3. Drag the game executable onto IPA.exe
4. Patching Your Game.

### Runnning
Start the game while SteamVR is running or use the --vr flag.

#### パッチ当て方法 How to Patching
- ゲームインストールフォルダにある「ゲーム名\_Data/globalgamemanagers」をUABEで開く。(Open "Game name\_Data/globalgamemanagers" in the game installation folder with UABE.)

- Path ID列が11のTypeがBuild Settingsとなっている行を選択し、Export Dumpを行う。(Select the row whose Path ID column is 11 and whose Type is Build Settings, and perform Export Dump.)
- 作成されたダンプファイルを開き、22行目の0 vector enabledVRDevicesから0 int size = 0までを下記のように修正。 (Open the created dump file and fix from 0 vector enabledVRDevices on line 22 to 0 int size = 0 as shown below.)

_修正前 preview fix_

0 vector enabledVRDevices  
 0 Array Array (0 items)  
  0 int size = 0  

_修正後 after fix_

0 vector enabledVRDevices  
  0 Array Array (2 items)  
   0 int size = 2  
   [0]  
    1 string data = "OpenVR"  
   [1]  
    1 string data = "None"  

\*1)0 vector buildTags の前行まで置き換える/0 Replace the previous line of vector buildTags.  
\*2)"Oculus"にすればOculusでも動くかもしれませんが、未確認です。/If it is "Oculus" it may work with Oculus, but it is unconfirmed.  

- 再度UABEでglobalgamemanagerを開き、修正したダンプファイルをインポートして更新してください。  
- Open globalgamemanager again with UABE and import and update the modified dump file.

## Author

[obtdai](https://github.com/obtdai)

## Licence

[MIT](https://github.com/obtdai/VRMOD.Template/LICENCE)

## Copyright

### [uDesktopDuplication](https://github.com/hecomi/uDesktopDuplication)

The MIT License (MIT)

Copyright (c) 2016 hecomi

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### [uTouchInjection](https://github.com/hecomi/uTouchInjection)

The MIT License (MIT)

Copyright (c) 2016 hecomi

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


### [VRGIN](https://github.com/Eusth/VRGIN)

The MIT License (MIT)

Copyright (c) 2016 Eusth

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

### [IPA](https://github.com/Eusth/IPA)
MIT License

Copyright (c) 2016 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.