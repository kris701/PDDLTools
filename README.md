# Visual Studio PDDL Tools Extension

![combined](https://github.com/kris701/PDDLTools/assets/22596587/5ead8019-9c45-4d5d-94d4-e72d4f361914)

This is an extension to make developing with PDDL files and planning in general easier.
This extension can be used with [Fast Downward](https://www.fast-downward.org/) to execute and analyse plans from running PDDL files. The extension is compatible with the PDDL 2.1 standard.
This extension can also be found on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=KristianSkovJohansen.pddltools).
Overall, this extensions adds:
* Editor features
  * Syntax checking
  * Syntax Highlighting with options for custom colors
  * Quick Info on all PDDL elements
  * Rename names and objects across multiple PDDL files
  * Small window in the bottom of the editor, showing general information on the document (such as predicate count or object count)
  * Auto completement of statements, both with basic PDDL elements and the predicates or objects you have written
  * Highlighting of same names across the entire document
  * Highlighting of braces
* PDDL Execution
  * Use Fast Downward to execute your current domain/problem with the F5 key
  * Get detailed results window from Fast Downward
  * Visualiser for the resulting Plans from Fast Downward
* PDDL Projects
  * Custom project type for PDDL development
  * Custom file templates to get you started with a PDDL file
* VAL
  * VAL integration to verify plans
* PDDL Tests
  * A small testing framework to execute and/or parse PDDL files
  * Fully integrated into the Visual Studio Test Explorer
* Other
  * Visualise predicate and type structure of a PDDL file
  * Quick Access toolbar
  * Settings for most of the features

## Getting Started
The only major requirement to running this is that you have a [Fast Downward](https://www.fast-downward.org/) installation build somewhere on your computer as well as having [Python](https://www.python.org/) installed.
When you have done that and made sure that Fast Downward works (try and call it from the command line), you are now ready to setup the extension.
After you have downloaded and installed this extension, a welcome window showing this will appear.
You can then go into `Options -> PDDL Tools` and set the `Fast Downward Folder` to be your install location. Its important that you set it to be the folder where the `fast-downward.py` is in.
Also set the `Python Prefix` to whatever your system environment is, its usually `python`, `python3` or `py`.
Now you are all set up and ready to use the extension. You can start by creating a new project in visual studio by `File -> New -> Project` and select the PDDL Project template.

If you want to use the Plan Validator, you must have [VAL](https://www.fast-downward.org/SettingUpVal) installed too. Its the same process as with Fast Downward, make sure you have it installed, and then give the location of `validate.exe` in the settings.

Note, you can also add the `PDDL Toolbar` if you want more direct access to the extension. You can enable it by right clicking the Visual Studio toolbar and add the `PDDL Toolbar`.

## Features
There are a lot of features in this extension, so each of them will be explained in their respective section below.

### Editor Features
One of the primary features of this extension is its syntax checker. Whenever you save a PDDL document, a background task will check through the PDDL document and give you errors, warnings and messages.

![image](https://github.com/kris701/PDDLTools/assets/22596587/33f0c0aa-c4d0-425f-beb6-19d6379b46fe)

This feature helps a lot with developing PDDL domains to make sure they work as intended. 
The next feature is the syntax highlighting.
This makes it a lot easier to see what is what in a PDDL document.

![image](https://github.com/kris701/PDDLTools/assets/22596587/9486e528-2299-4950-91c0-63a769aecaf5)

If you are not a fan of the default colors, you can change them all under `Options -> Environment -> Fonts and Colors` where all the relevant settings have the prefix `PDDL`.

![image](https://github.com/kris701/PDDLTools/assets/22596587/4b656014-204e-44eb-b231-e9202af2db49)

This extension also provides quick info on general PDDL elements. With the info is a short description of what it is, as well as some example syntax of how it should be set up.

![image](https://github.com/kris701/PDDLTools/assets/22596587/9c74bc06-6e3d-4e8b-851b-2392797ca8bb)




### PDDL Execution

### PDDL Projects

### VAL

### PDDL Tests

### Other











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
