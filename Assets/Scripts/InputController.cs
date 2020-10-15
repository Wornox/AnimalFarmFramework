using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

enum Command { help, start, stop, restart, add, spawn, speed, clear, stats, statsAll, grass, range, grainMax, grainSpawnSpeed, grainRegenRate, drawHTSlider };

public class InputController : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Awake()
    {

        CameraRotation = MainCamera.GetComponent<CameraRotation>();
        CameraMovement = MainCamera.GetComponent<CameraMovement>();

        CommandPrompt = GameObject.Find("CommandPrompt");
        CommandInput = GameObject.Find("CommandInput");
        CommandsLogTextContent = GameObject.Find("CommandsLogTextContent");
        terrainSet = TerrainController.currentTerrainSetInstance;
        CommandPrompt.SetActive(false);

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

    public void InterpretCommand(string command)
    {
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
        string s = "Avalible commands:";
        foreach(Command c in (Command[]) System.Enum.GetValues(typeof(Command)))
        {
            s += System.Environment.NewLine + "- " + c.ToString();
        }
        return s;
    }
}
