# Visual Studio PDDL Tools Extension

![combined](https://github.com/kris701/PDDLTools/assets/22596587/5ead8019-9c45-4d5d-94d4-e72d4f361914)

This is an extension to make developing with PDDL files and planning in general easier.
This extension can be used with [Fast Downward](https://www.fast-downward.org/) to execute and analyse plans from running PDDL files.
This extension can also be found on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=KristianSkovJohansen.pddltools).
Overall, this extensions adds:
* PDDL Syntax Highlighting and custom coloring
* PDDL Quick Info for general elements
* Editor Options
* Execute PDDL problem/domains with Fast Downward
* Detailed Fast Downward results window
* Visual view of Fast Downward SAS plans
* Visual view of domain file strutures
* Quick access toolbar
* PDDL parser that outputs syntax errors in the PDDL files, if any
* PDDL file templates
* PDDL Project templates
* Plan Validation using VAL

## How to Use
The first thing you will see when you have installed this extension, is a welcome page showing this information.
Its recommended you read through the features, so you know how to use them correctly.
To use this extension, make sure you have [Fast Downward](https://www.fast-downward.org/) installed and build somewhere. You must also have [Python](https://www.python.org/) installed.
Then go to `Settings -> PDDL Tools` and set the `Fast Downward Folder` to be your install location. Its important that you set it to be the folder where the `fast-downward.py` is in.
Also set the `Python Prefix` to whatever your system environment is, its usually `python`, `python3` or `py`.
When you have done this, you are ready to use the extension, simply by making a new project under `File -> New -> Project`!

If you want to use the Plan Validator, you must have [VAL](https://www.fast-downward.org/SettingUpVal) installed too. 

Note, you can also add the `PDDL Toolbar` if you want more direct access to the extension. You can enable it by right clicking the Visual Studio toolbar and add the `PDDL Toolbar`.

## PDDL Syntax Highlighting and Quick Info
This extension contains a simple PDDL syntax highlighter for basic PDDL domain/problem constructs, such as `:objects` or `:action`.
Also included is a simple Quick info provider, that shows some basic information on the highlighted constructs.

![image](https://github.com/kris701/PDDLTools/assets/22596587/ec94d09a-548a-4129-8ee0-94770f63b8ed)

![image](https://github.com/kris701/PDDLTools/assets/22596587/0269bfe7-319e-4aad-85b7-2e8b9f7e73ea)

A basic parser is also in this extension, that gives you information on syntax errors in the given problem or domain file you are editing. This updated every time you save the document.

![image](https://github.com/kris701/PDDLTools/assets/22596587/fed581a2-6373-4a4a-84dc-86bb115bd2d2)

## Options
There are two options pages for this extension, being `Settings -> PDDL Tools` and `Settings -> Text Editor -> PDDL`.
The first one is fairly self explanatory, and the text editor one is simply default Visual Studio settings options.
You can also add more Fast Downward search options under the tools options, just make sure its a `;` seperated list.
There are also options under the `Fonts and Colors` category to change the syntax colors.

## Execute a PDDL Problem and Domain
To run a PDDL problem and domain, you must have an active PDDL project open. You can then select what problem or domain file you want, either by right clicking the fole in the Solution Explore, by selecting it in the Extensions menu or using the toolbar. You can then select what engine you want to use, that is either an "alias" or a "search".

![image](https://github.com/kris701/PDDLTools/assets/22596587/539f0c73-51e3-443e-a19b-c3b926c57da7)

You can now select what `Search` option you want to run Fast Downward with in the final textbox.
Finally, you can click the `PDDL Executer` button, or press F5, and start Fast Downward. When the run is finished the results will be shown in the Output Window and two windows will open, regarding further details on the results and plan.

## Result Windows
There are two result windows that is displayedm the `Fast Downward Result` window and the `SAS Solution` window.
The first window shows general results from Fast Downward regarding the Translator and Search parameters.
The SAS Solution window shows if a solution was found.
This window visually shows the plan, as well as giving you the option to scroll through the plan process, using the slider in the bottom, and its states:

![image](https://github.com/kris701/PDDLTools/assets/22596587/019a2883-6d5c-4f47-8362-ad301f1de13e)

You can hover over a given node in the plan, to show the state at that point, as well as what action it took:

![image](https://github.com/kris701/PDDLTools/assets/22596587/b917ea0d-0cdd-4ed3-af0d-e921de345df0)
