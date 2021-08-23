using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVExport
{
    public string content = "";
    public bool needToFlush;
    
    public StreamWriter file;

    public CSVExport(string dirPath, string fileName)
    {
        if (File.Exists(dirPath))
        {
            File.Delete(dirPath);
        }
        file = File.CreateText(dirPath + "/" + fileName + ".csv");
        
        FileInfo fInfo = new FileInfo(dirPath);
        fInfo.IsReadOnly = true;
    }

    public void AddContent(object add)
    {
        content += add.ToString() + ",";
        needToFlush = true;
    }
}

public class CSVExporter : MonoBehaviour
{
    AnimalController AC;
    Dictionary<AnimalType, CSVExport> SWList = new Dictionary<AnimalType, CSVExport>();
    Dictionary<AnimalType, CSVExport> SWGenerationList = new Dictionary<AnimalType, CSVExport>();

    public string dirPath;

    public InputController InputController;

    void Awake()
    {
        AC = GetComponent<AnimalController>();
        Generate();

    }

    void Update()
    {
        //updateCSV();
        FlushAllNoneFlushContent();
    }

    public void Generate()
    {
        InputController.WriteToOwnConsole("CSVExporter.cs, Started generation of files");

        try
        {
            GenerateNewDirectory();
            GenerateNewPropertiesCSVFile();
        }
        catch (System.Exception)
        {
            InputController.WriteToOwnConsole("CSVExporter.cs, Generate error");
        }

        InputController.WriteToOwnConsole("File locations: " + dirPath);

        try
        {
            GenerateNewAnimalCSVFiles();
        }
        catch (System.Exception)
        {
            InputController.WriteToOwnConsole("CSVExporter.cs, Generate error2");
        }

        try
        {
            GenerateNewAnimalGenerationCSVFiles();
        }
        catch (System.Exception)
        {
            InputController.WriteToOwnConsole("CSVExporter.cs, Generate error3");
        }

        InputController.WriteToOwnConsole("CSVExporter.cs, Files are successfully generated");


    }

    public void GenerateNewDirectory()
    {
        string systemTime = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
        dirPath = Application.persistentDataPath + "/" + systemTime;

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public void GenerateNewPropertiesCSVFile()
    {
        CSVExport export = new CSVExport(dirPath,"Properties");
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
                var animalPropDic = AP.ACR.ageController.GetProperties();
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
            SWList.Add((AnimalType)type, new CSVExport(dirPath, "SimulationStatistics" + ((AnimalType)type).ToString()));
        }
        GenerateSCVColumns();
    }

    public void GenerateNewAnimalGenerationCSVFiles()
    {
        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            if ((AnimalType)type == AnimalType.None) continue;
            SWGenerationList.Add((AnimalType)type, new CSVExport(dirPath, "SimulationStatistics" + ((AnimalType)type).ToString() + "_Generation"));
        }
        GenerateGenerationSCVColumns();
    }

    void GenerateSCVColumns()
    {
        foreach (var CSVExporterVar in SWList)
        {
            if(AC == null) InputController.WriteToOwnConsole("AC null");
            if(AC.GetAnimalClass() == null) InputController.WriteToOwnConsole("AC animal null ");
            if(AC.GetAnimalClass().GetAgeStats(CSVExporterVar.Key).CSVColumns() == null) InputController.WriteToOwnConsole("CSVColumns is null ");

            var file = CSVExporterVar.Value.file;

            //making first row columns
            string columns = "System Time:," + "Simulation Time:," + "AnimalType:,";
            columns += AC.GetAnimalClass().GetAgeStats(CSVExporterVar.Key).CSVColumns() + ",";
            columns += AC.GetAnimalClass().GetSexStats(CSVExporterVar.Key).CSVColumns() + ",";
            columns += AC.GetAnimalClass().GetDeathStats(CSVExporterVar.Key).CSVColumns();

            file.WriteLine(columns);
            file.Flush();
        }
        
    }

    void GenerateGenerationSCVColumns()
    {
        foreach (var CSVExporterVar in SWGenerationList)
        {
            var file = CSVExporterVar.Value.file;

            //making first row columns
            string columns = "System Time:," + "Simulation Time:,";// + "AnimalType:,";

            columns += "Generation,";

            columns += "SPEED,";

            columns += "SpeedMutationType,";
            columns += "SpeedMutationProbability,";
            columns += "SpeedMutationMinMaxChange,";
            columns += "SpeedCominantRecessiveInheritancePriority,";

            columns += "SpeedDominantOriginal,";
            columns += "SpeedRecessiceOriginal,";
            columns += "SpeedMutationValue,";
            columns += "SpeedMutationResult,";
            columns += "SpeedDominant,";
            columns += "SpeedRecessice,";
            
            columns += "SpeedFather,";
            columns += "SpeedMother,";
            columns += "SpeedComparedToFather,";
            columns += "SpeedComparedToMother,";

            columns += "METABOLISM,";

            columns += "MetabolismMutationType,";
            columns += "MetabolismMutationProbability,";
            columns += "MetabolismMutationMinMaxChange,";
            columns += "MetabolismCominantRecessiveInheritancePriority,";

            columns += "MetabolismDominantOriginal,";
            columns += "MetabolismRecessiceOriginal,";
            columns += "MetabolismMutationValue,";
            columns += "MetabolismMutationResult,";
            columns += "MetabolismDominant,";
            columns += "MetabolismRecessice,";

            columns += "MetabolismFather,";
            columns += "MetabolismMother,";
            columns += "MetabolismComparedToFather,";
            columns += "MetabolismComparedToMother,";

            columns += "REPRODUCTIONRATE,";

            columns += "ReproductionRateMutationType,";
            columns += "ReproductionRateMutationProbability,";
            columns += "ReproductionRateMutationMinMaxChange,";
            columns += "ReproductionRateCominantRecessiveInheritancePriority,";

            columns += "ReproductionRateDominantOriginal,";
            columns += "ReproductionRateRecessiceOriginal,";
            columns += "ReproductionRateMutationValue,";
            columns += "ReproductionRateMutationResult,";
            columns += "ReproductionRateDominant,";
            columns += "ReproductionRateRecessice,";

            columns += "ReproductionRateFather,";
            columns += "ReproductionRateMother,";
            columns += "ReproductionRateComparedToFather,";
            columns += "ReproductionRateComparedToMother,";

            columns += "ATTRIBUTES AT NEWBORN,";

            columns += "Speed,";
            columns += "Hunger,";
            columns += "Thirst,";
            columns += "ViewRadius,";
            columns += "PregnancyTime";

            //columns += AC.animal.GetSexStats(CSVExporterVar.animalType).CSVColumns() + ",";
            //columns += AC.animal.GetDeathStats(CSVExporterVar.animalType).CSVColumns();
            file.WriteLine(columns);
            file.Flush();
        }
    }


    //Updating
    #region Update
    public void UpdateCSV(AnimalType animalType)
    {
        foreach (var CSVExporterVar in SWList)
        {
            if(CSVExporterVar.Key == animalType)
            {
                AddNewDataToContent(animalType,CSVExporterVar.Value);
            }
        }
    }

    public void UpdateGenerationCSV(AnimalType animalType, GeneController geneController, AnimalProperties animalProperties)
    {
        foreach (var CSVExporterVar in SWGenerationList)
        {
            if (CSVExporterVar.Key == animalType)
            {
                AddNewDataToGenerationContent(CSVExporterVar.Key,CSVExporterVar.Value, geneController, animalProperties);
            }
        }
    }

    void AddNewDataToContent(AnimalType animalType,CSVExport CSVExporterVar)
    {
        AnimalType type = animalType;

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

    void AddNewDataToGenerationContent(AnimalType animalType, CSVExport CSVExporterVar, GeneController geneController, AnimalProperties animalProperties)
    {
        AnimalType type = animalType;

        string systemTime = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;
        string simTime = Time.timeSinceLevelLoad.ToString();

        string generation = geneController.geneStructure.generation.ToString();


        //Speed
        string speedMutationType = geneController.geneAttributes.SpeedMutationType.ToString();
        string speedMutationProbability = geneController.geneAttributes.SpeedMutationProbability.ToString();
        string speedMutationMinMax = geneController.geneAttributes.SpeedMutationValueMin.ToString() + "_" + geneController.geneAttributes.SpeedMutationValueMax.ToString();
        string speedCominantRecessiveInheritancePriority = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominantRecessiveInheritancePriority.ToString();

        float speedDominantValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominantOriginal;
        string speedDominantOriginal = speedDominantValueOriginal.ToString();

        float speedRecessiveValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).recessiveOriginal;
        string speedRecessiceOriginal = speedRecessiveValueOriginal.ToString();

        float speedMutationValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).mutationValue;
        string speedMutation = speedMutationValue.ToString();
        string speedMutationResult = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).mutationResult;

        float speedDominantValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominant;
        string speedDominant = speedDominantValue.ToString();

        float speedRecessiveValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).recessive;
        string speedRecessice = speedRecessiveValue.ToString();

        float speedFatherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Father).GetGeneStructure(GeneAttributeType.Speed).dominant;
        string speedFather = speedFatherValue.ToString();

        float speedMotherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Mother).GetGeneStructure(GeneAttributeType.Speed).dominant;
        string speedMother = speedMotherValue.ToString();

        string speedComparedToFather = (speedDominantValue - speedFatherValue).ToString();

        string speedComparedToMother = (speedDominantValue - speedMotherValue).ToString();



        //Metabolism
        string metabolismMutationType = geneController.geneAttributes.MetabolismMutationType.ToString();
        string metabolismMutationProbability = geneController.geneAttributes.MetabolismMutationProbability.ToString();
        string metabolismMutationMinMaxValue = geneController.geneAttributes.MetabolismMutationValueMin.ToString() + "_" + geneController.geneAttributes.MetabolismMutationValueMax.ToString();
        string metabolismCominantRecessiveInheritancePriority = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominantRecessiveInheritancePriority.ToString();

        float metabolismDominantValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominantOriginal;
        string metabolismDominantOriginal = metabolismDominantValueOriginal.ToString();

        float metabolismRecessiveValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).recessiveOriginal;
        string metabolismRecessiceOriginal = metabolismRecessiveValueOriginal.ToString();

        float metabolismMutationValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).mutationValue;
        string metabolismMutation = metabolismMutationValue.ToString();
        string metabolismMutationResult = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).mutationResult;

        float metabolismDominantValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominant;
        string metabolismDominant = metabolismDominantValue.ToString();

        float metabolismRecessiveValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).recessive;
        string metabolismRecessice = metabolismRecessiveValue.ToString();

        float metabolismFatherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Father).GetGeneStructure(GeneAttributeType.Metabolism).dominant;
        string metabolismFather = metabolismFatherValue.ToString();

        float metabolismMotherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Mother).GetGeneStructure(GeneAttributeType.Metabolism).dominant;
        string metabolismMother = metabolismMotherValue.ToString();

        string metabolismComparedToFather = (metabolismDominantValue - metabolismFatherValue).ToString();

        string metabolismComparedToMother = (metabolismDominantValue - metabolismMotherValue).ToString();



        //ReproductionRate
        string reproductionRateMutationType = geneController.geneAttributes.ReproductionRateMutationType.ToString();
        string reproductionRateMutationProbability = geneController.geneAttributes.ReproductionRateMutationProbability.ToString();
        string reproductionRateMutationMinMaxValue = geneController.geneAttributes.ReproductionRateMutationValueMin.ToString() + "_" + geneController.geneAttributes.ReproductionRateMutationValueMax.ToString();
        string reproductionRateCominantRecessiveInheritancePriority = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).dominantRecessiveInheritancePriority.ToString();

        float reproductionRateDominantValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).dominantOriginal;
        string reproductionRateDominantOriginal = reproductionRateDominantValueOriginal.ToString();

        float reproductionRateRecessiveValueOriginal = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).recessiveOriginal;
        string reproductionRateRecessiceOriginal = reproductionRateRecessiveValueOriginal.ToString();

        float reproductionRateMutationValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).mutationValue;
        string reproductionRateMutation = reproductionRateMutationValue.ToString();
        string reproductionRateMutationResult = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).mutationResult;

        float reproductionRateDominantValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).dominant;
        string reproductionRateDominant = reproductionRateDominantValue.ToString();

        float reproductionRateRecessiveValue = geneController.geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).recessive;
        string reproductionRateRecessice = reproductionRateRecessiveValue.ToString();

        float reproductionRateFatherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Father).GetGeneStructure(GeneAttributeType.ReproductionRate).dominant;
        string reproductionRateFather = reproductionRateFatherValue.ToString();

        float reproductionRateMotherValue = geneController.geneStructure.GetParentGeneStructure(ParentType.Mother).GetGeneStructure(GeneAttributeType.ReproductionRate).dominant;
        string reproductionRateMother = reproductionRateMotherValue.ToString();

        string reproductionRateComparedToFather = (reproductionRateDominantValue - reproductionRateFatherValue).ToString();

        string reproductionRateComparedToMother = (reproductionRateDominantValue - reproductionRateMotherValue).ToString();


        //ViewRadius

        //Attributes
        string speed = animalProperties.speed.ToString();
        string hunger = animalProperties.hungerModifier.ToString();
        string thirst = animalProperties.thirstModifier.ToString();
        string viewRadius = animalProperties.viewRadius.ToString();
        string pregnancyTime = animalProperties.ACR.matingController.pregnancyTime.ToString();


        CSVExporterVar.content = ""; //only the newest datas are stored in one frame, and the end the last conetent content change in frame will be flushed

        CSVExporterVar.AddContent(systemTime);
        CSVExporterVar.AddContent(simTime);

        CSVExporterVar.AddContent(generation);

        CSVExporterVar.AddContent("SPEED");

        CSVExporterVar.AddContent(speedMutationType);
        CSVExporterVar.AddContent(speedMutationProbability);
        CSVExporterVar.AddContent(speedMutationMinMax);
        CSVExporterVar.AddContent(speedCominantRecessiveInheritancePriority);

        CSVExporterVar.AddContent(speedDominantOriginal);
        CSVExporterVar.AddContent(speedRecessiceOriginal);
        CSVExporterVar.AddContent(speedMutation);
        CSVExporterVar.AddContent(speedMutationResult);
        CSVExporterVar.AddContent(speedDominant);
        CSVExporterVar.AddContent(speedRecessice);

        CSVExporterVar.AddContent(speedFather);
        CSVExporterVar.AddContent(speedMother);
        CSVExporterVar.AddContent(speedComparedToFather);
        CSVExporterVar.AddContent(speedComparedToMother);

        CSVExporterVar.AddContent("METABOLISM");

        CSVExporterVar.AddContent(metabolismMutationType);
        CSVExporterVar.AddContent(metabolismMutationProbability);
        CSVExporterVar.AddContent(metabolismMutationMinMaxValue);
        CSVExporterVar.AddContent(metabolismCominantRecessiveInheritancePriority);

        CSVExporterVar.AddContent(metabolismDominantOriginal);
        CSVExporterVar.AddContent(metabolismRecessiceOriginal);
        CSVExporterVar.AddContent(metabolismMutation);
        CSVExporterVar.AddContent(metabolismMutationResult);
        CSVExporterVar.AddContent(metabolismDominant);
        CSVExporterVar.AddContent(metabolismRecessice);

        CSVExporterVar.AddContent(metabolismFather);
        CSVExporterVar.AddContent(metabolismMother);
        CSVExporterVar.AddContent(metabolismComparedToFather);
        CSVExporterVar.AddContent(metabolismComparedToMother);

        CSVExporterVar.AddContent("REPRODUCTIONRATE");

        CSVExporterVar.AddContent(reproductionRateMutationType);
        CSVExporterVar.AddContent(reproductionRateMutationProbability);
        CSVExporterVar.AddContent(reproductionRateMutationMinMaxValue);
        CSVExporterVar.AddContent(reproductionRateCominantRecessiveInheritancePriority);

        CSVExporterVar.AddContent(reproductionRateDominantOriginal);
        CSVExporterVar.AddContent(reproductionRateRecessiceOriginal);
        CSVExporterVar.AddContent(reproductionRateMutation);
        CSVExporterVar.AddContent(reproductionRateMutationResult);
        CSVExporterVar.AddContent(reproductionRateDominant);
        CSVExporterVar.AddContent(reproductionRateRecessice);

        CSVExporterVar.AddContent(reproductionRateFather);
        CSVExporterVar.AddContent(reproductionRateMother);
        CSVExporterVar.AddContent(reproductionRateComparedToFather);
        CSVExporterVar.AddContent(reproductionRateComparedToMother);

        CSVExporterVar.AddContent("ATTRIBUTES");

        CSVExporterVar.AddContent(speed);
        CSVExporterVar.AddContent(hunger);
        CSVExporterVar.AddContent(thirst);
        CSVExporterVar.AddContent(viewRadius);
        CSVExporterVar.AddContent(pregnancyTime);


        FlushAllNoneFlushContentGeneration();
    }

    void FlushAllNoneFlushContent() // only called once in 1 update
    {
        foreach (var CSVExporterVar in SWList)
        {
            if (CSVExporterVar.Value.content != "") //if no content, no need to flush
            {
                // only called once in 1 update -> at the end of frame exporter with data will be flushed
                IEnumerator coroutine = null;
                coroutine = FlushContent(CSVExporterVar.Value, coroutine);
                StartCoroutine(coroutine);
            }
        }
    }

    void FlushAllNoneFlushContentGeneration() // only called once in 1 update
    {
        foreach (var CSVExporterVar in SWGenerationList)
        {
            if (CSVExporterVar.Value.content != "") //if no content, no need to flush
            {
                // only called once in 1 update -> at the end of frame exporter with data will be flushed
                CSVExporterVar.Value.file.WriteLine(CSVExporterVar.Value.content);
                CSVExporterVar.Value.file.Flush();

                CSVExporterVar.Value.content = "";
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

    #endregion

    //Closing
    #region Close
    private void OnDestroy()
    {
        closeSCVFiles();
        closeGenerationSCVFiles();
    }

    public void closeSCVFiles()
    {
        foreach(var CSV in SWList)
        {
            CSV.Value.file.Close();
        }
        SWList.Clear();
    }

    public void closeGenerationSCVFiles()
    {
        foreach (var CSV in SWGenerationList)
        {
            CSV.Value.file.Close();
        }
        SWGenerationList.Clear();
    }
    #endregion

}
