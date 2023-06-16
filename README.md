# Visual Studio PDDL Tools Extension
[![Build](https://github.com/kris701/PDDLTools/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kris701/PDDLTools/actions/workflows/dotnet.yml)

![combined](https://github.com/kris701/PDDLTools/assets/22596587/b9ec7c1c-6064-46b1-a4f9-48aee036f160)

This is an extension to make developing with PDDL files and planning in general easier.
This extension can be used with [Fast Downward](https://www.fast-downward.org/) to execute and analyse plans from running PDDL files.
This extension can also be found on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=KristianSkovJohansen.pddltools).
Overall, this extensions adds:
* PDDL Syntax Highlighting
* PDDL Quick Info for general elements
* Editor Options
* Execute PDDL problem/domains with Fast Downward
* Detailed Fast Downward results window
* Visual view of Fast Downward SAS plans
* Quick access toolbar
* PDDL parser that outputs syntax errors in the PDDL files, if any

## How to Use
The first thing you will see when you have installed this extension, is a welcome page showing this information.
Its recommended you read through the features, so you know how to use them correctly.
To use this extension, make sure you have [Fast Downward](https://www.fast-downward.org/) installed and build somewhere. You must also have [Python](https://www.python.org/) installed.
Then go to `Settings -> PDDL Tools` and set the `Fast Downward Folder` to be your install location. Its important that you set it to be the folder where the `fast-downward.py` is in.
Also set the `Python Prefix` to whatever your system environment is, its usually `python`, `python3` or `py`.
When you have done this, you are ready to use the extension!

Note, you can also add the `PDDL Toolbar` if you want more direct access to the extension. You can enable it by right clicking the Visual Studio toolbar and add the `PDDL Toolbar`.

## PDDL Syntax Highlighting and Quick Info
This extension contains a simple PDDL syntax highlighter for basic PDDL domain/problem constructs, such as `:objects` or `:action`.
Also included is a simple Quick info provider, that shows some basic information on the highlighted constructs.
![image](https://github.com/kris701/PDDLTools/assets/22596587/3d00388c-af64-4441-9066-c0c7ccf84dbf)
![image](https://github.com/kris701/PDDLTools/assets/22596587/7b014c17-18e8-468d-b823-9532719d0d00)
A basic parser is also in this extension, that gives you information on syntax errors in the given problem or domain file you are editing. This updated every time you save the document.
![image](https://github.com/kris701/PDDLTools/assets/22596587/b13132b8-bed4-4d7a-86a9-f6fee863e5d7)

## Options
There are two options pages for this extension, being `Settings -> PDDL Tools` and `Settings -> Text Editor -> PDDL`.
The first one is fairly self explanatory, and the text editor one is simply default Visual Studio settings options.
You can also add more Fast Downward search options under the tools options, just make sure its a `;` seperated list.

## Execute a PDDL Problem and Domain
To run a PDDL problem and domain, you must have both files open in Visual Studio. The extension will then automatically recognise what is problem and what is domain files.
You can then select what problem and domain you want to run with under the comboboxes in the extension:
![image](https://github.com/kris701/PDDLTools/assets/22596587/e1346a45-04ad-4f25-8eda-6a7059f777b7)
You can now select what `Search` option you want to run Fast Downward with in the final checkbox.
Finally, you can click the `Run` button and start fast downward. When the run is finished the results will be shown in the Output Window and two windows will open, regarding further details on the results and plan.

## Result Windows
There are two result windows that is displayedm the `Fast Downward Result` window and the `SAS Solution` window.
The first window shows general results from Fast Downward regarding the Translator and Search parameters.
The SAS Solution window shows if a solution was found.
This window visually shows the plan, as well as giving you the option to scroll through the plan process, using the slider in the bottom, and its states:
![image](https://github.com/kris701/PDDLTools/assets/22596587/a2e73cba-c61f-4929-9fb9-3063c140ad30)
You can hover over a given node in the plan, to show the state at that point, as well as what action it took:
![image](https://github.com/kris701/PDDLTools/assets/22596587/3de28ae5-dd21-4c64-8192-488bc8b02a74)


