# <img src="src/Resources/logo64.png" style="width: 32px; height: 32px" alt="Wrappr logo"> Wrappr

Wrappr is a desktop application that allows you to quickly and easily enable/disable Windows services and track their status.

## Features

1. Accessible from system tray at any moment.
2. Fast access to status of any service on your computer: you can track only services that you need.
3. Possibility to notify if service status was changes outside Wrappr app.

## Requirements

Will work only on Windows systems with .NET 9 installed (so, Windows 10 version 1607 or later).

> [!NOTE]
> Perhaps the .NET version (and Windows version) will be downgraded later.

## Installation

It's portable application, so just download .exe from [releases](https://github.com/prmncr/Wrappr/releases),
put this .exe in empty directory and run it! App will generate only one config file in the same directory.

## Building

### Prerequisites
- .NET SDK 9

Just run in project folder:
```shell
dotnet build
```

## How to autostart it after system login?

Currently, this feature is not implemented yet, but you can add a task in Windows Task Scheduler manually.
1. Win + R -> `taskschd.msc` -> Run
2. Action -> Create Task
3. Enter any name
4. Check "Run with higher privileges"
5. "Triggers" tab -> Add
6. Begin the task -> Select "At log on"
7. Select users, delay, etc. if needed -> OK
8. "Actions" tab -> Create
9. Make sure that "Action" is set to "Start a program"
10. "Program/script" -> set to Wrappr.exe **in your directory**
11. Add arguments -> write `--silent` (this allows app to launch in background without window opening) -> OK
12. Set conditions and settings in "Conditions" and "Settings" tabs at your discretion
13. Click OK and you're done!