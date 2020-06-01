# FullscreenDetector
A program that stops a process when the focused window is in fullsceen. If the focus switches to a non fullscreen window, the stopped process is started again.

## How to build

Open the .sln file in Visual Studio 2019 and build the FullsreenDetector Project. The GUI isn't implemented yet.

## How to use

The program is a windows desktop application and therefore does not spawn a console. It takes a single argument, the name of the process you want to stop/restart.

An example on how to use it with Rainmeter:

`FullscreenDetector.exe Rainmeter`

## Autostart with Windows

There are multiple ways to autostart the program with Windows. Two of the most common ways are to add a link to the exe to your shell:startup folder, or to create a scheduled task.

### Adding it to shell:startup

* Press Win-Key+R and enter `shell:startup`, press enter
* A new explorer window should now be open in your startup folder
* Hold an Alt-Key and drag the FullscreenDetector.exe into the startup folder, an link to the exe should have been created
* Rightclick and open the properties of the link, in the target field before the last " add a whitespace and enter your process name
* click ok and you are done
