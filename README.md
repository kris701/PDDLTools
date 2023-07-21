![Banner](https://github.com/kris701/PDDLTools/assets/22596587/bed28862-d60b-47fb-a802-9039d5ad64a3)

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

You can also right click most PDDL named elements and rename them. When you click to rename a valid element, a popup will display where you can write what you want to replace the name with.
If you select the option to rename across all matching problem files, the item will be renamed across all problems that uses the domain reference name given in the current PDDL document.

![image](https://github.com/kris701/PDDLTools/assets/22596587/658cb593-db2e-421d-afa8-567537fda67a)
![image](https://github.com/kris701/PDDLTools/assets/22596587/09a7b9c3-8621-4a12-9917-58e546e54ae1)

When you open a PDDL document, a small bar in the bottom of the editor shows some general information on the currently open domain or problem file.

![image](https://github.com/kris701/PDDLTools/assets/22596587/972815a7-9827-47b5-8f06-51e719d2725c)
![image](https://github.com/kris701/PDDLTools/assets/22596587/0cafda90-3354-4e2b-85ab-dc4c09da43b2)

When you write anything in the editor, some autocompletements will show up. These are either basic PDDL elements or names or predicates used in the current document.

![image](https://github.com/kris701/PDDLTools/assets/22596587/2890bcb4-2c3e-4d6d-b33b-a4aef988816d)
![image](https://github.com/kris701/PDDLTools/assets/22596587/f41ad6c9-1dbe-4632-9f05-a31136d18269)

Whenever you select a word in the current PDDL document, all other matching words is highlighted. Matching braces are also highlighted.

![image](https://github.com/kris701/PDDLTools/assets/22596587/3dc94247-e1a4-4d7c-8dad-3dc97756d32b)
![image](https://github.com/kris701/PDDLTools/assets/22596587/98d6429d-d7ab-4d28-b53e-9fdbc2deafbb)

All these editor features can be turned on or off in Options page.

### PDDL Execution

When creating PDDL projects, you get the option to execute PDDL files in it. To start, you can select domains/problems either from selecting it from the toolbar menu, the extension menu or context menus.

![image](https://github.com/kris701/PDDLTools/assets/22596587/064c69e0-77cc-40d1-bf87-8a7fc50f0642)
![image](https://github.com/kris701/PDDLTools/assets/22596587/2bcc3bdc-e29c-41ea-a51b-0ed15a36ce6f)
![image](https://github.com/kris701/PDDLTools/assets/22596587/c1afbba3-846e-4190-ad67-ad92ab2a6edc)

Note, need to have the current project set as the startup project for this to work.
(When a project is loaded, it may take a few seconds for the extension to index the PDDL files in it)

You can now select what engine you want to execute with. By default there are a handful of `--search` options and all the `--alias` options that are available in the current install of Fast Downward. You can add more engine options in the Options menu.

![image](https://github.com/kris701/PDDLTools/assets/22596587/17232362-6d78-4c2c-b46f-35930fc4ae6d)

You can now, assuming you have no syntax errors, execute the given domain and problem file by pressing F5 or clicking the `PDDL Executor` button in the toolbar.

If the execution of the PDDL files where successful, two windows will open. One showing general information from the Fast Downward log.

![image](https://github.com/kris701/PDDLTools/assets/22596587/e4b2dace-5b5a-4793-bee6-dd401ccbd86c)

As well as a visualiser showing details on the resulting plan file.

![image](https://github.com/kris701/PDDLTools/assets/22596587/b59f97bb-787f-4240-bbbd-2ca78ad5b88d)

Each node represents a step in the plan, if the circle is dark then its just a normal state, if its orange its a partial goal state and green means a full goal state.
You can move the nodes around by draging and dropping them, or you can reroll the layout by using the controls in the top of the window.
You can also move the slider in the bottom, to show specific parts of the plan.

### PDDL Projects

This extensions also brings custom project types. There are two primary types, `PDDL Projects` or `PDDL Test Projects`.
The `PDDL Projects` is used for developing PDDL domains and problems and its also the projects used to execute files with Fast Downward. The `PDDL Test Project` will be explained later.

![image](https://github.com/kris701/PDDLTools/assets/22596587/4874dac7-5987-4959-8880-4b2d2fe138b7)

When you create a new PDDL project, reload one, or startup a solution the files in each active PDDL projects will be indexed. 
You will most likely only notice this if you have a lot of PDDL files (several thousands). The indexing gives the rest of the extension an easier time when it comes to knowing what files are what.
You can right click a project file and reindex it manually if you need to.

![image](https://github.com/kris701/PDDLTools/assets/22596587/3fb83291-08ef-446d-92de-b94b830540b5)

### VAL

The plan validator [VAL](https://www.fast-downward.org/SettingUpVal) is also integrated into this extension. You can right click any PDDL file and plan file to send it to the VAL window.

![image](https://github.com/kris701/PDDLTools/assets/22596587/a4a2d338-2c40-4b8e-a168-cbc843448fb9)
![image](https://github.com/kris701/PDDLTools/assets/22596587/b4dc794c-56e4-4d61-b47e-76b639e8ab4e)

When you click on this, the VAL window will open, showing if a given combination of a domain, problem and plan file is valid.

![image](https://github.com/kris701/PDDLTools/assets/22596587/a8e4fd31-d47f-4ecf-bf23-5c3c3e87788a)

### PDDL Tests

This extension also includes an entire testing framework, to run tests of PDDL files.
This is the second PDDL project type thats included, that introduces a new file type being `.pddltest`.
This file describes a test for a given domain and a set of problems, as well as what tests to actually run. 
The file itself is structured as a JSON file.

![image](https://github.com/kris701/PDDLTools/assets/22596587/260fdd0d-9d9a-4d44-ac9b-d172b44d2e93)

The tasks describe what kind of test to run, the options are
* Parse
  * Simply parses the given PDDL files
* ParseAnalyse
  * Parses the PDDL files and checks for errors. If there are any errors, the test will be regarded as a failure.
* FDExecute
  * Executes the PDDL files with Fast Downward.

To configure the tests, the project template includes a `.runsettings` file that you should use. 
It contains the basic information that is needed to run Fast Downward, the settings are much the same as those in the Options page.
You can now open the Visual Studio Test Explorer and the tests should pop up.

![image](https://github.com/kris701/PDDLTools/assets/22596587/da98ea3c-6b4d-4474-bf6c-1e6e27b6901e)
![image](https://github.com/kris701/PDDLTools/assets/22596587/f5e10305-77d7-45fb-935b-cc41054fcbf6)

And thats all you need to execute tests with the adapter!

### Other

This extension makes it possible to visualise PDDL domain structures. This can again be done from the context menu by right clicking PDDL domains in the solution explorer.
You can then see a general overview of the structure in the PDDL files, as well as see if some predicates are unused/unnessesary.
You can also select to visualise types, as well as what objects use said type.
Generally, this visualiser is good for small PDDL domains.

![image](https://github.com/kris701/PDDLTools/assets/22596587/0c2cd918-5b79-487d-9855-df030bddfe60)
![image](https://github.com/kris701/PDDLTools/assets/22596587/7506c853-966c-497b-9e2a-29d5a2eac3b5)

More or less all the features have settings that are available under the `Options` page in Visual Studio.

![image](https://github.com/kris701/PDDLTools/assets/22596587/42cf7027-3c1d-4cb4-9fe1-129e9e08709c)




