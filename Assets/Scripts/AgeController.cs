using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgeAttributeType
{
    matingStatus,
    viewRadius,
    speed,
    ageStageUpperValue,
    agehungerModifier,
    agethirstModifier,
    ageSizeModifier
}

[System.Serializable]
public struct AgeAttributeInspector
{
    public AgeStage ageStage;

    public bool matingCapability;

    public float viewRadius;

    public float speed;

    [Range(0f, 100f)]
    public float ageStageUpperPercentage;

    [Range(0.5f, 2f)]
    public float agehungerModifier;

    [Range(0.5f, 2f)]
    public float agethirstModifier;

    [Range(0.1f, 100f)]
    public float ageSizeModifier;

}

public enum AgeStage
{
    NewBorn,
    Puppy,
    Juvenile,
    YoungAdult,
    Adult,
    AgedAdult,
    Elder
}

public class AgeController : MonoBehaviour
{
    public List<AgeAttributeInspector> ageAttributesList;

    private Dictionary<AgeStage, Dictionary<AgeAttributeType, float>> attributeDic;

    public float age;

    private AgeStage prevAgeStage;
    public AgeStage ageStage;

    private AgeStage[] ageArray;

    public float bornTime { get; private set; }
    public float avarageLifeTime;

    public float elderAgeStart;

    public AnimalComponentReferences ACR;

    //Elder dath check
    private float checkElderDeathAge = 0f;
    private float checkElderDeathTimer = 10f;

    void Awake()
    {
        age = 0;
        prevAgeStage = AgeStage.NewBorn;
        ageStage = AgeStage.NewBorn;
        bornTime = Time.timeSinceLevelLoad;
        elderAgeStart = avarageLifeTime * ageAttributesList[5].ageStageUpperPercentage / 100;

        ageArray = (AgeStage[])Enum.GetValues(typeof(AgeStage));

        GenerateDictionary();
    }

    void GenerateDictionary()
    {
        attributeDic = new Dictionary<AgeStage, Dictionary<AgeAttributeType, float>>();

        foreach (var item in ageAttributesList)
        {
            Dictionary<AgeAttributeType, float> att = new Dictionary<AgeAttributeType, float>();

            att.Add(AgeAttributeType.matingStatus, item.matingCapability ? 1 : 0);
            att.Add(AgeAttributeType.viewRadius, item.viewRadius);
            att.Add(AgeAttributeType.speed, item.speed);
            att.Add(AgeAttributeType.ageStageUpperValue, item.ageStageUpperPercentage / 100 * avarageLifeTime);
            att.Add(AgeAttributeType.agehungerModifier, item.agehungerModifier);
            att.Add(AgeAttributeType.agethirstModifier, item.agethirstModifier);
            att.Add(AgeAttributeType.ageSizeModifier, item.ageSizeModifier);

            attributeDic.Add(item.ageStage,att);
        }
    }

    public float GetCurrentAttribute(AgeAttributeType ageAttributeType)
    {
        return attributeDic[ageStage][ageAttributeType];
    }

    public float RefreshAge(float deltaTime)
    {
        age += deltaTime;
        float agePrecentageInStage = CheckAgeStage();
        return agePrecentageInStage;
    }

    public float CheckAgeStage()
    {
        float prevageUpperLimit = attributeDic[prevAgeStage][AgeAttributeType.ageStageUpperValue];
        float ageUpperLimit = attributeDic[ageStage][AgeAttributeType.ageStageUpperValue];

        //if reached new stage
        if (age >= ageUpperLimit)
        {
            int index = (int)ageStage;
            if(index + 1 >= ageArray.Length)
            {
                //Animal Reached the end of "Elder" stage -> meaning absolute age death

                ACR.livingCreature.CreatureDied(CauseOfDeath.ageDeath);

            } else
            {
                prevAgeStage = ageArray[index];
                ageStage = ageArray[index + 1];

                ACR.animalController.AgeStageModified(ACR.animalProperties.animalType, prevAgeStage, ageStage);

                prevageUpperLimit = attributeDic[prevAgeStage][AgeAttributeType.ageStageUpperValue]; ;
                ageUpperLimit = attributeDic[ageStage][AgeAttributeType.ageStageUpperValue];

            }
        }

        CheckElderDeath(); //if elder, checking death probalility every time

        float agePrecentageInStageLerped = (age - prevageUpperLimit) / (ageUpperLimit - prevageUpperLimit) * 100; // calculates the age in percentage between prev and current age

        return agePrecentageInStageLerped;
    }

    void CheckElderDeath()
    {
        if (ageStage == AgeStage.Elder && checkElderDeathAge + checkElderDeathTimer < age)
        {
            checkElderDeathAge = age; // resetting last elder death check age time
            // (elderAgeStart = elderThreshold * avarageLifeTime)
            // ((460-450) / (600-450))/2 = 10/150 / 2      = 0.03% 
            // ((600-450) / (600-450))/2 = 150/150 / 2     = 50% (50% to die if age is equal to avarage lifetime)
            // ((620-450) / (600-450))/2 = 170/150 / 2     = 56.6% (after avarage lifetime deth % is bigger)
            float deathPercentage = ((age - elderAgeStart) / (avarageLifeTime - elderAgeStart)) / 2f;
            float random = UnityEngine.Random.Range(0.0f, 1.0f);
            if (random <= deathPercentage)
            {
                ACR.livingCreature.CreatureDied(CauseOfDeath.ageDeath);
            }
        }
    }

    public float GetLerpedValue(AgeAttributeType ageAttributeType, float percentage)
    {
        float previous = attributeDic[prevAgeStage][ageAttributeType];
        float current = attributeDic[ageStage][ageAttributeType];

        float zeroOne = percentage / 100;
        float lerp = Mathf.Lerp(previous, current, zeroOne);
        return lerp;
    }

    /*
    public void CheckAgeStageOLD()
    {
        if (AP.age < avarageLifeTime * ageAttributes[0].ageStageUpperPercentage / 100)
        {
            if (ageStage == AgeStage.NewBorn)
            {
                ageStage = AgeStage.Puppy;
                MC.matingStatus = MatingStatus.unMature;
                transform.localScale = ageAttributes[0].size;
                AP.speed = ageAttributes[0].speed;
                AP.viewRadius = ageAttributes[0].viewRadius;
                AP.hungerModifier = ageAttributes[0].hungerModifier;
                AP.thirstModifier = ageAttributes[0].thirstModifier;
                AP.ACR.animalController.AgeStageModified(AP.animalType, AgeStage.Puppy, AgeStage.Puppy);
            }
        }
        else if (AP.avarageLifeTime * ageAttributes[0].ageStageUpperPercentage / 100 <= AP.age && AP.age < AP.avarageLifeTime * ageAttributes[1].ageStageUpperPercentage / 100)
        {
            if (AP.ageStage == AgeStage.Puppy) // only runs once when changed from puppy to juvenile
            {
                AC.AgeStageModified(AP.animalType, AgeStage.Puppy, AgeStage.Juvenile);
                AP.ageStage = AgeStage.Juvenile;
                MC.matingStatus = MatingStatus.readyToMate;
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                AP.speed = ageAttributes[1].speed;
                AP.viewRadius = ageAttributes[1].viewRadius;
                hungerModifier = ageAttributes[1].hungerModifier;
                thirstModifier = ageAttributes[1].thirstModifier;
            }
        }
        else if (AP.avarageLifeTime * ageAttributes[1].ageStageUpperPercentage / 100 <= AP.age && AP.age < AP.avarageLifeTime * ageAttributes[2].ageStageUpperPercentage / 100)
        {
            if (AP.ageStage == AgeStage.Juvenile)
            {
                AC.AgeStageModified(AP.animalType, AgeStage.Juvenile, AgeStage.YoungAdult);
                AP.ageStage = AgeStage.YoungAdult;
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                AP.speed = ageAttributes[2].speed;
                AP.viewRadius = ageAttributes[2].viewRadius;
                hungerModifier = ageAttributes[2].hungerModifier;
                thirstModifier = ageAttributes[2].thirstModifier;
            }
        }
        else if (AP.avarageLifeTime * ageAttributes[2].ageStageUpperPercentage / 100 <= AP.age && AP.age < AP.avarageLifeTime * ageAttributes[3].ageStageUpperPercentage / 100)
        {
            if (AP.ageStage == AgeStage.YoungAdult)
            {
                AC.AgeStageModified(AP.animalType, AgeStage.YoungAdult, AgeStage.Adult);
                AP.ageStage = AgeStage.Adult;
                transform.localScale = new Vector3(1f, 1f, 1f);
                AP.speed = ageAttributes[3].speed;
                AP.viewRadius = ageAttributes[3].viewRadius;
                hungerModifier = ageAttributes[3].hungerModifier;
                thirstModifier = ageAttributes[3].thirstModifier;
            }
        }
        else if (AP.avarageLifeTime * ageAttributes[3].ageStageUpperPercentage / 100 <= AP.age && AP.age < AP.avarageLifeTime * ageAttributes[4].ageStageUpperPercentage / 100)
        {
            if (AP.ageStage == AgeStage.Adult)
            {
                AC.AgeStageModified(AP.animalType, AgeStage.Adult, AgeStage.AgedAdult);
                AP.ageStage = AgeStage.AgedAdult;
                transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                AP.speed = ageAttributes[4].speed;
                AP.viewRadius = ageAttributes[4].viewRadius;
                hungerModifier = ageAttributes[4].hungerModifier;
                thirstModifier = ageAttributes[4].thirstModifier;
            }
        }
        else if (AP.avarageLifeTime * ageAttributes[4].ageStageUpperPercentage / 100 <= AP.age)
        {
            if (AP.ageStage == AgeStage.AgedAdult)
            {
                AC.AgeStageModified(AP.animalType, AgeStage.AgedAdult, AgeStage.Elder);
                AP.ageStage = AgeStage.Elder;
                MC.matingStatus = MatingStatus.tooOld;
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                AP.speed = ageAttributes[5].speed;
                AP.viewRadius = ageAttributes[5].viewRadius;
                hungerModifier = ageAttributes[5].hungerModifier;
                thirstModifier = ageAttributes[5].thirstModifier;
            }
        }
    }
    */

    public Dictionary<string, string> GetProperties()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>();

        foreach (var agestage in ageAttributesList)
        {
            int index = ageAttributesList.IndexOf(agestage);
            index++;
            string stage = ((AgeStage)index).ToString();
            AddToList(properties, "", stage);
            AddToList(properties, agestage.viewRadius, $"View distance ({stage})");
            AddToList(properties, agestage.speed, $"Walking speed ({stage})");
            AddToList(properties, agestage.ageStageUpperPercentage * avarageLifeTime / 100, $"Age group limit ({stage})");
            AddToList(properties, agestage.agehungerModifier, $"Hunger ({stage})");
            AddToList(properties, agestage.agethirstModifier, $"Thirst ({stage})");
        }

        AddToList(properties, ACR.animalProperties.hungerThresherhold, $"HungerThresherhold");
        AddToList(properties, ACR.animalProperties.thirstThresherhold, $"ThirstThresherhold");

        AddToList(properties, avarageLifeTime, "Avarage lifetime");
        string enemyTypesString = "";
        foreach (var enemy in ACR.animalProperties.enemyTypes)
        {
            if (enemyTypesString != "") enemyTypesString += " & ";
            enemyTypesString += enemy.ToString();
        }
        AddToList(properties, enemyTypesString, "Enemys");
        string foodTypesString = "";
        foreach (var food in ACR.animalProperties.foodTypes)
        {
            if (foodTypesString != "") foodTypesString += " & ";
            foodTypesString += food.ToString();
        }
        AddToList(properties, foodTypesString, "Foods");
        AddToList(properties, ACR.matingController.childBirth, "Most childbirth in a pregnancy");

        var MC = GetComponent<MatingController>();
        AddToList(properties, MC.matingTime, "Mating duration");
        AddToList(properties, ACR.animalProperties.pregnancyTime, "Pregnacy duration");
        AddToList(properties, MC.spermatogenesisTime, "Spermatogenesis duration");
        AddToList(properties, ACR.animalProperties.formPack, "Pack forming");

        return properties;
    }

    void AddToList(Dictionary<string, string> properties, System.Object o, string name)
    {
        properties.Add(name, o.ToString());
    }

}
