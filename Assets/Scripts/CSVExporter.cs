using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVExport
{
    public CSVExport(string fileName, AnimalType animalType, string animalName = "")
    {
        this.fileName = fileName;
        this.animalType = animalType;
        string systemTime = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
        string dirPath = Application.persistentDataPath + "/" + systemTime;
        if (animalType != AnimalType.None) animalName = "_" + animalType.ToString();
        filePath = dirPath + "/" + fileName + animalName +  ".csv";

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        file = File.CreateText(filePath);
        
        FileInfo fInfo = new FileInfo(filePath);
        fInfo.IsReadOnly = true;
    }

    public void AddContent(object add)
    {
        content += add.ToString() + ",";
        needToFlush = true;
    }

    public string content = "";
    public bool needToFlush;
    string fileName;
    string filePath;
    public AnimalType animalType;
    public StreamWriter file;
}

public class CSVExporter : MonoBehaviour
{
    AnimalController AC;
    List<CSVExport> SWList = new List<CSVExport>();

    void Awake()
    {
        AC = GetComponent<AnimalController>();
    }

    void Start()
    {
        GenerateNewPropertiesCSVFile();
        GenerateNewAnimalCSVFiles();
    }

    void Update()
    {
        //updateCSV();
        FlushAllNoneFlushContent();
    }

    public void GenerateNewPropertiesCSVFile()
    {
        CSVExport export = new CSVExport("Properties", AnimalType.None);
        Dictionary<string, List<string>> properties = new Dictionary<string, List<string>>();
        properties.Add("Animals", new List<string>());
        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            if ((AnimalType)type == AnimalType.None) continue;
            properties["Animals"].Add(((AnimalType)type).ToString());
            GameObject prefab = AC.GetPrefabByAnimalType((AnimalType)type);
            if (prefab != null)
            {
                AnimalProperties AP = prefab.GetComponent<AnimalProperties>();
                var animalPropDic = AP.GetProperties();
                foreach(var rowsInAnimal in animalPropDic)
                {
                    if (properties.ContainsKey(rowsInAnimal.Key)) // if key exists add value
                    {
                        properties[rowsInAnimal.Key].Add(rowsInAnimal.Value);
                    } else // add new row
                    {
                        int prevanimals = properties["Animals"].Count;
                        List<string> values = new List<string>();
                        for(int i = 0; i < prevanimals - 1; i++)
                        {
                            values.Add("No value");
                        }
                        values.Add(rowsInAnimal.Value);
                        properties.Add(rowsInAnimal.Key,values);
                    }
                }
            }
        }

        foreach(var row in properties)
        {
            export.AddContent(row.Key);
            foreach(var listitem in row.Value)
            {
                export.AddContent(listitem.ToString());
            }
            export.file.WriteLine(export.content);
            export.file.Flush();
            export.content = "";
        }
        export.file.Close();
    }

    public void GenerateNewAnimalCSVFiles()
    {
        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            if ((AnimalType)type == AnimalType.None) continue;
            SWList.Add(new CSVExport("SimulationStatistics", (AnimalType)type));
        }
        GenerateSCVColumns();
    }

    void GenerateSCVColumns()
    {
        foreach(var CSVExporterVar in SWList)
        {
            var file = CSVExporterVar.file;

            //making first row columns
            string columns = "System Time:," + "Simulation Time:," + "AnimalType:,";
            columns += AC.animal.GetAgeStats(CSVExporterVar.animalType).CSVColumns() + ",";
            columns += AC.animal.GetSexStats(CSVExporterVar.animalType).CSVColumns() + ",";
            columns += AC.animal.GetDeathStats(CSVExporterVar.animalType).CSVColumns();
            file.WriteLine(columns);
            file.Flush();
        }
    }

    public void UpdateCSV(AnimalType animalType)
    {
        foreach (var CSVExporterVar in SWList)
        {
            if(CSVExporterVar.animalType == animalType)
            {
                AddNewDataToContent(CSVExporterVar);
            }
        }

    }

    void AddNewDataToContent(CSVExport CSVExporterVar)
    {
        AnimalType type = CSVExporterVar.animalType;

        string systemTime = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;
        string simTime = Time.timeSinceLevelLoad.ToString();
        string sex = AC.animal.GetSexStats(type).CSVString();
        string age = AC.animal.GetAgeStats(type).CSVString();
        string death = AC.animal.GetDeathStats(type).CSVString();

        CSVExporterVar.content = ""; //only the newest datas are stored in one frame, and the end the last conetent content change in frame will be flushed

        CSVExporterVar.AddContent(systemTime);
        CSVExporterVar.AddContent(simTime);
        CSVExporterVar.AddContent(type);
        CSVExporterVar.AddContent(age);
        CSVExporterVar.AddContent(sex);
        CSVExporterVar.AddContent(death);
        
    }

    void FlushAllNoneFlushContent() // only called once in 1 update
    {
        foreach (var CSVExporterVar in SWList)
        {
            if (CSVExporterVar.content != "") //if no content, no need to flush
            {
                // only called once in 1 update -> at the end of frame exporter with data will be flushed
                IEnumerator coroutine = null;
                coroutine = FlushContent(CSVExporterVar, coroutine);
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator FlushContent(CSVExport CSVExporterVar, IEnumerator coroutine)
    {
        yield return new WaitForEndOfFrame(); // wait until the end of frame to flush exporter with data
        CSVExporterVar.file.WriteLine(CSVExporterVar.content);
        CSVExporterVar.file.Flush();

        CSVExporterVar.content = "";
    }
    
    public void closeSCVFiles()
    {
        foreach(var CSV in SWList)
        {
            CSV.file.Close();
        }
        SWList.Clear();
    }

    private void OnDestroy()
    {
        closeSCVFiles();
    }
}
