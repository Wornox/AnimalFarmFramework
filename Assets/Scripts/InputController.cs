using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

enum Command { help, start, stop, restart, add, spawn, speed, clear, stats, statsAll, grass, range, grainMax, grainSpawnSpeed, grainRegenRate, drawHTSlider, set };

public class InputController : MonoBehaviour
{
    static Dictionary<Command, string> commandDescriptions = new Dictionary<Command, string> {
        {Command.help, "List all help commands"},
        {Command.start, "Stats the simualtion, entities (animals and vegetation) and time starts to move"},
        {Command.stop, "Stops the simualtion, entities (animals and vegetation) and time are frozen"},
        {Command.restart, "Restarts the simualtion, all entities (animals and vegetation) and statistics are deleted from the map"},
        {Command.add, "Add animals to the simulation, additional parameters required: \n usage: add <animaltype> <number> <location> \n example: add chicken 200 random \n note: <animalType> must be one of {Chicken, Dog, Fox, Cow, Pig, Lion}, <number> must be positive integer, <location> be one of {random} options"},
        {Command.spawn, "Add vegetations to the simulation, additional parameters required: \n usage: spawn <vegetationtype> <number> <location> \n example: spawn grain 180 random \n note: <vegetationType> must be 'grain', <number> must be positive integer, <location> must be 'random'"},
        {Command.speed, "Set the speed of the simulation, default value is 1: \n usage: speed <value> \n example: speed 2 \n note: <value> must be within the range of 0.1 and 3"},
        {Command.clear, "Clear the command window"},
        {Command.stats, "Writes the statistics on the command window"},
        {Command.statsAll, "Writes detailed statistics on the command window"},
        {Command.grass, "Graphical setting, turn on/off grass \n usage: grass <1/0>"},
        {Command.range, "Graphical setting, turn on/off animal viewrange for perfomance, default is turned off \n usage: range <1/0>"},
        {Command.grainMax, "Vegetation setting, set vegetation limit in the simualtion \n usage: grainMax <value>"},
        {Command.grainSpawnSpeed, "Vegetation setting, set vegetation spawn speed in the simualtion \n usage: grainSpawnSpeed <value>"},
        {Command.grainRegenRate, "Vegetation setting, set vegetation regen rate in the simualtion \n usage: grainRegenRate <value>"},
        {Command.drawHTSlider, "Graphical setting, turn on/off hunger and thirst bars in animal info (above head) for performance, default is turned on \n usage: drawHTSlider <1/0>"},
        {Command.set, "Animal setting, set animals parameter in the simualtion \n usage: set <animaltype> <attributename> <value>, \n example: set chicken pregnancyTime 60 \n note: <animalType> must be one of {Chicken, Dog, Fox, Cow, Pig, Lion} options, <attributename> must be one of {pregnancyTime} options, <value> must be positive integer"},
    };
    public Canvas Overlay;
    public GameObject MainCamera;
    public GameObject AnimalController;
    public GameObject VegetableSpawner;
    public TimeController TimeController;

    CameraRotation CameraRotation;
    CameraMovement CameraMovement;

    GameObject CommandPrompt;
    GameObject CommandInput;
    GameObject CommandsLogTextContent;

    private GameObject terrainSet;

    public bool ConsoleMode;
    public bool drawHTsliders = true;

    bool objectAwakened = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!objectAwakened)
        {
            objectAwakened = true;

            CameraRotation = MainCamera.GetComponent<CameraRotation>();
            CameraMovement = MainCamera.GetComponent<CameraMovement>();

            CommandPrompt = GameObject.Find("CommandPrompt");
            CommandInput = GameObject.Find("CommandInput");
            CommandsLogTextContent = GameObject.Find("CommandsLogTextContent");
            terrainSet = TerrainController.currentTerrainSetInstance;
            CommandPrompt.SetActive(false);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExcetuteCommandPrompt();
            CommandInput.GetComponent<InputField>().Select();
            CommandInput.GetComponent<InputField>().ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //Enables and disables Command propmt
            ConsoleMode = !ConsoleMode;
            SetCameraMovement(!ConsoleMode);
            SetCommandPromptVisibility(ConsoleMode);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            TimeController.GetComponent<TimeController>().Increase();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            TimeController.GetComponent<TimeController>().Decrease();
        }

    }

    void SetCameraMovement(bool b)
    {
        CameraRotation.enabled = b;
        CameraMovement.enabled = b;
    }

    void SetCommandPromptVisibility(bool b)
    {
        Overlay.transform.Find("CommandPrompt").gameObject.SetActive(b);
        CommandInput.GetComponent<InputField>().text = "";
        CommandInput.GetComponent<InputField>().Select();
        CommandInput.GetComponent<InputField>().ActivateInputField();
    }

    void ExcetuteCommandPrompt()
    {
        string commandLine = CommandInput.GetComponent<InputField>().text;
        CommandInput.GetComponent<InputField>().text = "";

        string[] commands = commandLine.Split(';');

        foreach(var command in commands)
        {
            InterpretCommand(command);
        }
    }

    public void WriteToOwnConsole(string s)
    {
        if (!objectAwakened) Awake();
        AddStringToLog(s);
    }

    public void InterpretCommand(string command)
    {
        if (!objectAwakened) Awake();
        string[] parts = command.Split(' ');

        Command mainPart;

        if (System.Enum.TryParse(parts[0], out mainPart))
        {
            switch (mainPart)
            {
                case Command.help:
                    AddStringToLog(CollectCommands());
                    break;
                case Command.start:
                    AnimalController.GetComponent<AnimalController>().StartSimulation();
                    AddStringToLog("Simulation started.");
                    break;
                case Command.stop:
                    AnimalController.GetComponent<AnimalController>().StopSimulation();
                    AddStringToLog("Simulation stopped.");
                    break;
                case Command.restart:
                    AnimalController.GetComponent<AnimalController>().RestartSimulation();
                    AddStringToLog("Simulation restarted.");
                    break;
                case Command.add:
                    (bool b, string s) = AnimalController.GetComponent<AnimalController>().AddAnimal(parts[1], parts[2], parts[3]);
                    if (b) AddStringToLog("Successfull adding:" + System.Environment.NewLine + s);
                    else AddStringToLog("Failed adding:" + System.Environment.NewLine + s);
                    break;
                case Command.spawn:
                    int num = int.Parse(parts[2]); 
                    for (int i = 0; i < num; i++)
                    {
                        VegetableSpawner.GetComponent<VegetableSpawner>().SpawnGrain();
                    }
                    AddStringToLog("Successfull spawning " + num + " " + parts[1] + " randomly.");
                    break;
                case Command.speed:
                    TimeController.GetComponent<TimeController>().SetScaleTime(float.Parse(parts[1]));
                    break;
                case Command.clear:
                    CommandsLogTextContent.GetComponent<Text>().text = "";
                    break;
                case Command.stats:
                    AddStringToLog(VegetableSpawner.GetComponent<VegetableSpawner>().GetStats());
                    AddStringToLog(AnimalController.GetComponent<AnimalController>().GetStatsShort());
                    break;
                case Command.statsAll:
                    AddStringToLog(VegetableSpawner.GetComponent<VegetableSpawner>().GetStats());
                    AddStringToLog(AnimalController.GetComponent<AnimalController>().GetStats());
                    break;
                case Command.grass:
                    bool draw;
                    bool.TryParse(parts[1], out draw);
                    TerrainController.DrawGrassOnAllTerrainOnSet(draw);
                    AddStringToLog("Grass set to " + draw);
                    break;
                case Command.range:
                    bool draw_range;
                    bool.TryParse(parts[1], out draw_range);
                    AnimalController.GetComponent<AnimalController>().SetDrawingOfRange(draw_range);
                    AddStringToLog("Scanner range drawing set to " + draw_range);
                    break;
                case Command.grainMax:
                    int grainMax;
                    int.TryParse(parts[1], out grainMax);
                    VegetableSpawner.GetComponent<VegetableSpawner>().maxGrainNumber = grainMax;
                    AddStringToLog("Maximum grain number limit was set to: " + grainMax);
                    break;
                case Command.grainSpawnSpeed:
                    float grainTimer;
                    float.TryParse(parts[1], out grainTimer);
                    VegetableSpawner.GetComponent<VegetableSpawner>().grainSpawnTimer = grainTimer;
                    AddStringToLog("Grain spawn speed was set to: " + grainTimer + " seconds");
                    break;
                case Command.grainRegenRate:
                    float grainRegen;
                    float.TryParse(parts[1], out grainRegen);
                    VegetableSpawner.GetComponent<VegetableSpawner>().regenateRate = grainRegen;
                    AddStringToLog("Grain regen rate was set to: " + grainRegen);
                    break;
                case Command.drawHTSlider:
                    bool draw_slider;
                    bool.TryParse(parts[1], out draw_slider);
                    AnimalController.GetComponent<AnimalController>().drawHTSlider = draw_slider;
                    VegetableSpawner.GetComponent<VegetableSpawner>().drawHTSlider = draw_slider;
                    break;
                case Command.set:
                    bool setValid = SetCommand(parts[1], parts[2], parts[3]);
                    if (setValid) AddStringToLog("Successfull set:" + System.Environment.NewLine + parts[1] + " " + parts[2] + " " + parts[3]);
                    else AddStringToLog("Failed adding:" + System.Environment.NewLine + parts[1] + " " + parts[2] + " " + parts[3]);
                    break;
                default:
                    AddStringToLog("Unknown command please try 'help' command.");
                    break;
            }
        }
        else
        {
            AddStringToLog("Unknown command - " + command + " - please try 'help' command.");
        }
    }

    void AddStringToLog(string s)
    {
        string oldText = CommandsLogTextContent.GetComponent<Text>().text;
        string systemTime = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;
        string simulationTime = Time.time.ToString();
        string newText = systemTime + " " + simulationTime + " " + s + System.Environment.NewLine + oldText;
        CommandsLogTextContent.GetComponent<Text>().text = newText;
    }

    string CollectCommands()
    {
        string s = "Avalible commands in the current version:";
        foreach(Command c in (Command[]) System.Enum.GetValues(typeof(Command)))
        {
            commandDescriptions.TryGetValue(c, out string description);
            s += System.Environment.NewLine + "- " + c.ToString() + description;
        }
        return s;
    }

    bool SetCommand(string animalTypeString, string parameter, string value)
    {
        bool validCommand = AnimalController.GetComponent<AnimalController>().SetParameter(animalTypeString, parameter, value);
        return validCommand;
    }
}
