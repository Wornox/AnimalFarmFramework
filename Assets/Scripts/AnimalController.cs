using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SpawnPosition { random, fix };

public enum CauseOfDeath { none, thirstDeath, hungerDeath, ageDeath, predatorDeath }

public struct SexDiversion
{
    public int female;
    public int overall_female;
    public int male;
    public int overall_male;

    public override string ToString() => $"female: {female}  | male: {male} | overall_female: {overall_female} | overall_male: {overall_male}";
    public string CSVColumns() => $"female,male,overall_female,overall_male";
    public string CSVString() => $"{female},{male},{overall_female},{overall_male}";
}

public struct AgeDiversion
{
    public int overall;
    public int alive;
    public int puppy;
    public int overall_puppy;
    public int juvenile;
    public int overall_juvenile;
    public int young_adult;
    public int overall_young_adult;
    public int adult;
    public int overall_adult;
    public int aged_adult;
    public int overall_aged_adult;
    public int elder;
    public int overall_elder;

    public override string ToString() => $"overall: {overall} | alive: {alive} {System.Environment.NewLine}puppy: {puppy} | juven: {juvenile} | y_adult: {young_adult} | adult: {adult} | a_adult: {aged_adult} | elder: {elder} {System.Environment.NewLine}overall_puppy: {overall_puppy} | overall_juven: {overall_juvenile} | overall_y_adult: {overall_young_adult} | overall_adult: {overall_adult} | overall_a_adult: {overall_aged_adult} | overall_elder: {overall_elder}";
    public string CSVColumns() => $"overall,alive,puppy,juvenile,young_adult,adult,aged_adult,elder,overall_puppy,overall_juvenile,overall_young_adult,overall_adult,overall_aged_adult,overall_elder";
    public string CSVString() => $"{overall},{alive},{puppy},{juvenile},{young_adult},{adult},{aged_adult},{elder},{overall_puppy},{overall_juvenile},{overall_young_adult},{overall_adult},{overall_aged_adult},{overall_elder}";
    public string ShortStats() => $"overall: {overall} | alive: {alive}";
}

public struct DeathDiversion
{
    public int thirstDeath;
    public int hungerDeath;
    public int ageDeath;
    public int predatorDeath;

    public override string ToString() => $"thirstDeath: {thirstDeath} | hungerDeath: {hungerDeath} | ageDeath: {ageDeath} | predatorDeath: {predatorDeath}";
    public string CSVColumns() => $"thirstDeath,hungerDeath,ageDeath,predatorDeath";
    public string CSVString() => $"{thirstDeath},{hungerDeath},{ageDeath},{predatorDeath}";
}

public class Animal
{
    AnimalController AC;

    private CSVExporter exporter;

    private List<StatsController> statsControllerList = new List<StatsController>();
    private List<CreatureController> creatureControllerList = new List<CreatureController>();

    public Animal()
    {
        //statsControllerList = new List<StatsController>();
        //creatureControllerList = new List<CreatureController>();
    }

    public void CreateExporter(AnimalController AC)
    {
        this.AC = AC;
        exporter = AC.GetComponent<CSVExporter>();
    }

    class StatsController
    {
        class AnimalSexDiversion
        {
            //public (int, int, int, int) GetStats()
            //{
            //    return (thirstDeath, hungerDeath, ageDeath, predatorDeath);
            //}
            //
            //public void GetStats(out int thirstDeath, out int hungerDeath, out int ageDeath, out int predatorDeath)
            //{
            //    thirstDeath = this.thirstDeath;
            //    hungerDeath = this.hungerDeath;
            //    ageDeath = this.ageDeath;
            //    predatorDeath = this.predatorDeath;
            //}

            public SexDiversion GetStat()
            {
                return SD;
            }

            public void NewAnimal(Sex sex)
            {
                switch (sex)
                {
                    case Sex.female:
                        SD.female++;
                        SD.overall_female++;
                        break;
                    case Sex.male:
                        SD.male++;
                        SD.overall_male++;
                        break;
                }
            }

            public void Death(Sex sex)
            {
                switch (sex)
                {
                    case Sex.female:
                        SD.female--;
                        break;
                    case Sex.male:
                        SD.male--;
                        break;
                }
            }

            public void Reset()
            {
                SD = new SexDiversion();
            }

            SexDiversion SD = new SexDiversion();
        }
        class AnimalAgeDiversion
        {
            public void NewAnimal(AgeStage to)
            {
                AD.overall++;
                AD.alive++;
                Modify(to, 1);
            }

            public void StageModified(AgeStage from, AgeStage to)
            {
                if (from != to) Modify(from, -1);
                Modify(to, 1);
            }

            void Modify(AgeStage stage, int i)
            {
                switch (stage)
                {
                    case AgeStage.Puppy:
                        AD.puppy += i;
                        if (i == 1) AD.overall_puppy++;
                        break;
                    case AgeStage.Juvenile:
                        AD.juvenile += i;
                        if (i == 1) AD.overall_juvenile++;
                        break;
                    case AgeStage.YoungAdult:
                        AD.young_adult += i;
                        if (i == 1) AD.overall_young_adult++;
                        break;
                    case AgeStage.Adult:
                        AD.adult += i;
                        if (i == 1) AD.overall_adult++;
                        break;
                    case AgeStage.AgedAdult:
                        AD.aged_adult += i;
                        if (i == 1) AD.overall_aged_adult++;
                        break;
                    case AgeStage.Elder:
                        AD.elder += i;
                        if (i == 1) AD.overall_elder++;
                        break;
                }
            }

            public AgeDiversion GetStat()
            {
                return AD;
            }

            public string GetStatShort()
            {
                return AD.ShortStats();
            }

            public void Death(AgeStage stage)
            {
                AD.alive--;
                Modify(stage, -1);
            }
            public void Reset()
            {
                AD = new AgeDiversion();
            }

            AgeDiversion AD = new AgeDiversion();
        }
        class AnimalDeathDiversion
        {
            public DeathDiversion GetStat()
            {
                return DD;
            }

            public void Death(CauseOfDeath causeOfDeath)
            {
                switch (causeOfDeath)
                {
                    case CauseOfDeath.thirstDeath:
                        DD.thirstDeath++;
                        break;
                    case CauseOfDeath.hungerDeath:
                        DD.hungerDeath++;
                        break;
                    case CauseOfDeath.ageDeath:
                        DD.ageDeath++;
                        break;
                    case CauseOfDeath.predatorDeath:
                        DD.predatorDeath++;
                        break;
                }
            }

            public void Reset()
            {
                DD = new DeathDiversion();
            }

            DeathDiversion DD = new DeathDiversion();
        }

        public StatsController(AnimalType animalType)
        {
            this.animalType = animalType;
        }

        public void AddAnimal(GameObject reference)
        {
            AAD.NewAnimal(reference.GetComponent<AnimalProperties>().ageStage);
            ASD.NewAnimal(reference.GetComponent<MatingController>().sex);
        }

        public void AgeStageModified(AgeStage from, AgeStage to)
        {
            AAD.StageModified(from, to);
        }

        public void Death(AgeStage ageStage, CauseOfDeath causeOfDeath, Sex sex)
        {
            AAD.Death(ageStage);
            ADD.Death(causeOfDeath);
            ASD.Death(sex);
        }

        public void Reset()
        {
            ASD.Reset();
            AAD.Reset();
            ADD.Reset();
        }

        public SexDiversion GetSexStats()
        {
            return ASD.GetStat();
        }

        public string GetAgeStatsShort()
        {
            return AAD.GetStatShort();
        }

        public AgeDiversion GetAgeStats()
        {
            return AAD.GetStat();
        }

        public DeathDiversion GetDeathStats()
        {
            return ADD.GetStat();
        }

        public AnimalType animalType;
        AnimalSexDiversion ASD = new AnimalSexDiversion();
        AnimalAgeDiversion AAD = new AnimalAgeDiversion();
        AnimalDeathDiversion ADD = new AnimalDeathDiversion();
    }

    class CreatureController
    {
        public CreatureController(AnimalType animalType)
        {
            this.animalType = animalType;
        }

        class Creature
        {
            public Creature(GameObject reference)
            {
                this.reference = reference;
                //alive = true;
                causeOfDeath = CauseOfDeath.none;
            }

            public GameObject reference;
            public CauseOfDeath causeOfDeath;
        }

        public void AddAnimal(GameObject reference)
        {
            creatureList.Add(new Creature(reference));
        }

        public void Death(GameObject reference, CauseOfDeath causeOfDeath)
        {
            Creature creature = creatureList.Find(x => x.reference == reference);
            if(creature != null) creature.causeOfDeath = causeOfDeath;
        }

        public void Reset()
        {
            foreach (var cc in creatureList)
            {
                Object.Destroy(cc.reference);
            }
            creatureList = new List<Creature>();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public void DrawRange(bool draw)
        {
            foreach (var c in creatureList)
            {
                if (c.reference != null && c.reference.GetComponent<Scanner>() != null) c.reference.GetComponent<Scanner>().draw_scanner = draw;
            }
        }

        public AnimalType animalType;
        private List<Creature> creatureList = new List<Creature>();
    }

    public void AddNewAnimalType(AnimalType animalType)
    {
        statsControllerList.Add(new StatsController(animalType));
        creatureControllerList.Add(new CreatureController(animalType));
    }

    public void AddNewAnimal(AnimalType animalType, GameObject reference)
    {
        FindStatsController(animalType).AddAnimal(reference);
        FindCreatureController(animalType).AddAnimal(reference);
        StatsHasBeenUpdated(animalType);
    }

    public void AgeStageModified(AnimalType animalType, AgeStage from, AgeStage to)
    {
        FindStatsController(animalType).AgeStageModified(from, to);
        StatsHasBeenUpdated(animalType);
    }

    public void DeathOfAnimal(AnimalType animalType, GameObject reference, AgeStage ageStage, CauseOfDeath causeOfDeath, Sex sex)
    {
        FindStatsController(animalType).Death(ageStage, causeOfDeath, sex);
        FindCreatureController(animalType).Death(reference, causeOfDeath);
        StatsHasBeenUpdated(animalType);
    }

    public void Reset()
    {
        foreach (var sc in statsControllerList)
        {
            sc.Reset(); // Destorying all stats
        }
        foreach (var cc in creatureControllerList)
        {
            cc.Reset(); // Destorying all animals
        }
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void SetDrawRange(bool draw)
    {
        foreach (var ct in creatureControllerList)
        {
            ct.DrawRange(draw);
        }
    }

    public SexDiversion GetSexStats(AnimalType animalType)
    {
        return FindStatsController(animalType).GetSexStats();
    }

    public string GetAgeStatsShort(AnimalType animalType)
    {
        return FindStatsController(animalType).GetAgeStatsShort();
    }

    public AgeDiversion GetAgeStats(AnimalType animalType)
    {
        return FindStatsController(animalType).GetAgeStats();
    }

    public DeathDiversion GetDeathStats(AnimalType animalType)
    {
        return FindStatsController(animalType).GetDeathStats();
    }

    StatsController FindStatsController(AnimalType animalType)
    {
        StatsController returnSC = null;
        foreach (var sc in statsControllerList)
        {
            if (sc.animalType == animalType)
            {
                returnSC = sc;
                break;
            }

        }
        if (returnSC == null)
        {
            Debug.Log("AnimalController.cs FindStatsController can't find existing SC, created a new one");
        }
        return returnSC;
    }


    CreatureController FindCreatureController(AnimalType animalType)
    {
        CreatureController returnCC = null;
        foreach (var cc in creatureControllerList)
        {
            if (cc.animalType == animalType)
            {
                returnCC = cc;
                break;
            }
        }
        if (returnCC == null)
        {
            Debug.Log("AnimalController.cs FindCreatureController can't find existing CC, created a new one");
            //returnCC = new CreatureController(animalType);

        }
        return returnCC;
    }

    void StatsHasBeenUpdated(AnimalType animalType)
    {
        exporter.UpdateCSV(animalType);
    }

}

public class AnimalController : MonoBehaviour
{
    public List<GameObject> animalPrefabList;
    public Animal animal;
    public List<(GameObject,AnimalComponentReferences)> animalReferences;

    public bool drawHTSlider = true;
    public bool drawHTValue = false;

    void Awake()
    {
        createAnimalClass();
        animalReferences = new List<(GameObject, AnimalComponentReferences)>();
    }

    void createAnimalClass()
    {
        animal = new Animal();
        animal.CreateExporter(this);

        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            animal.AddNewAnimalType((AnimalType)type);
        }
    }
    void Update()
    {
        if (animal == null)
        {
            createAnimalClass();
        }
    }

    public void StartSimulation()
    {
        Time.timeScale = 1;
    }

    public void StopSimulation()
    {
        Time.timeScale = 0;
    }

    public void RestartSimulation()
    {
        animal.Reset();
        animalReferences = new List<(GameObject, AnimalComponentReferences)>();

        PackNames.Restart();

        GetComponent<CSVExporter>().closeSCVFiles();
        GetComponent<CSVExporter>().GenerateNewAnimalCSVFiles();
    }

    public (bool, string) AddAnimal(string animalTypeString, string animalCountString, string spawnPos)
    {
        string returnS = "";
        bool returnB = true;
        int animalCount = int.Parse(animalCountString);
        SpawnPosition sp;
        System.Enum.TryParse(spawnPos, out sp);
        AnimalType animalType;
        System.Enum.TryParse(animalTypeString, true, out animalType);

        GameObject prefab = GetPrefabByAnimalType(animalType);

        if (prefab != null)
        {
            SpawnAnimal(prefab, animalType, animalCount, sp, out returnS);
        }
        else
        {
            returnS = "Animal type '" + animalTypeString + "' doesn't exists.";
            returnB = false;
        }

        return (returnB, returnS);
    }

    void SpawnAnimal(GameObject animalPrefab, AnimalType animalType, int count, SpawnPosition sp, out string returnS)
    {
        Vector3 spVec = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            switch (sp)
            {
                case SpawnPosition.random:
                    spVec = TerrainController.GetRandomPosOnTerrainSet();
                    break;
                case SpawnPosition.fix:
                    spVec = Vector3.zero;
                    break;
            }

            if (Vector3.Equals(spVec, Vector3.negativeInfinity))
            {
                returnS = "No valid terrain";
                return;
            }

            var navMeshPos = NavMeshController.GetRandomPointOnNavmesh(animalPrefab.GetComponent<NavMeshAgent>().areaMask, spVec);

            GameObject animalRef = Instantiate(animalPrefab, navMeshPos, Quaternion.identity);
            animalRef.GetComponent<NavMeshAgent>().Warp(navMeshPos);
            animal.AddNewAnimal(animalType, animalRef);
            var animalACR = animalRef.GetComponent<AnimalComponentReferences>();
            animalReferences.Add((animalRef, animalACR));
            animalACR.animalProperties.drawHungerThirstSlider = drawHTSlider;
            animalACR.animalStatusController.EnableHungerThirstSlider(animalACR.animalProperties.drawHungerThirstSlider);
        }
        returnS = count + " " + animalPrefab.name.ToString() + "(s) were added to the simulation.";
    }

    public string GetStatsShort()
    {
        string s = "Populations :" + System.Environment.NewLine + System.Environment.NewLine;
        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            if ((AnimalType)type == AnimalType.None) continue;
            s += type.ToString() + " -> ";
            s += animal.GetAgeStatsShort((AnimalType)type).ToString() + System.Environment.NewLine;
            s += System.Environment.NewLine;
        }
        return s;
    }

    public string GetStats()
    {
        string s = "Populations :" + System.Environment.NewLine + System.Environment.NewLine;
        foreach (var type in System.Enum.GetValues(typeof(AnimalType)))
        {
            if ((AnimalType)type == AnimalType.None) continue;
            s += type.ToString() + " -> ";
            s += animal.GetAgeStats((AnimalType)type).ToString() + System.Environment.NewLine;
            s += animal.GetSexStats((AnimalType)type).ToString() + System.Environment.NewLine;
            s += animal.GetDeathStats((AnimalType)type).ToString() + System.Environment.NewLine;
            s += System.Environment.NewLine;
        }
        return s;
    }

    public void SetCauseOfDeath(AnimalType animalType, GameObject reference, AgeStage ageStage, CauseOfDeath causeOfDeath, Sex sex)
    {
        animal.DeathOfAnimal(animalType, reference, ageStage, causeOfDeath, sex);
    }

    public void AgeStageModified(AnimalType animalType, AgeStage from, AgeStage to)
    {
        animal.AgeStageModified(animalType, from, to);
    }

    public GameObject GetPrefabByAnimalType(AnimalType animalType)
    {
        GameObject returnGO = null;
        foreach (var a in animalPrefabList)
        {
            if (a.GetComponent<AnimalProperties>().animalType == animalType) returnGO = a;
        }
        return returnGO;
    }

    public void SetDrawingOfRange(bool draw)
    {
        animal.SetDrawRange(draw);
    }
}
