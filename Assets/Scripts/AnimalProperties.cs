using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType
{
    Chicken,
    Dog,
    Lion,
    Fox,
    Cow,
    Pig,
    None
};

public enum Phase
{
    Exploring,
    Fleeing,
    Drinking,
    Eating,
    Mating
}

public enum Food
{
    Chicken,
    Dog,
    Lion,
    Fox,
    Cow,
    Pig,
    None,
    Grain,
    Corn
}

public enum Liquid
{
    Water,
    SwampAndWater
}

public enum CreatureType
{
    Animal,
    Vegetable
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

public enum Sex
{
    male,
    female,
    random
}

public enum MatingStatus
{
    unMature,
    readyToMate,
    foundMate,
    mating,
    pregnant,
    inactive,
    tooOld
}

[System.Serializable]
public struct AgeAttributes
{
    public float viewRadius;
    public float speed;
    [Range(0f, 100f)]
    public float ageStageUpperPercentage;
    [Range(0.5f, 2f)]
    public float hungerModifier;
    [Range(0.5f, 2f)]
    public float thirstModifier;
}

public class AnimalProperties : MonoBehaviour
{
    public List<AgeAttributes> ageAttributes;
    public float viewRadius;
    public float speed;
    public AnimalType animalType;
    public List<AnimalType> enemyTypes;
    public List<Food> foodTypes;
    public Liquid waterType;

    public float bornTime { get; private set; }
    public float age;
    public AgeStage ageStage;
    public float avarageLifeTime;

    public int childBirth;

    public float hungerModifier { get; private set; }
    public float thirstModifier { get; private set; }

    public float hungerThresherhold;
    public float thirstThresherhold;

    public bool formPack;

    public float elderAgeStart;

    AnimalProperties AP;
    AnimalController AC;
    MatingController MC;

    public bool drawHungerThirstSlider = true;

    void Start()
    {
        bornTime = Time.timeSinceLevelLoad;
        AP = this;
        AC = GameObject.Find("AnimalController").GetComponent<AnimalController>();
        MC = GetComponent<MatingController>();
        elderAgeStart = AP.avarageLifeTime * ageAttributes[4].ageStageUpperPercentage / 100;
    }

    void Update()
    {
        checkAgeStage();
    }

    //TODO: rewrite with only neighbour stage checks || 2 new varaible which determins the preMilestone < current < nextMilestone 
    void checkAgeStage()
    {
        if (AP.age < AP.avarageLifeTime * ageAttributes[0].ageStageUpperPercentage / 100)
        {
            if (AP.ageStage == AgeStage.NewBorn)
            {
                AP.ageStage = AgeStage.Puppy;
                MC.matingStatus = MatingStatus.unMature;
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                AP.speed = ageAttributes[0].speed;
                AP.viewRadius = ageAttributes[0].viewRadius;
                hungerModifier = ageAttributes[0].hungerModifier;
                thirstModifier = ageAttributes[0].thirstModifier;
                AC.AgeStageModified(AP.animalType, AgeStage.Puppy, AgeStage.Puppy);
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

    public Dictionary<string, string> GetProperties()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>();

        foreach(var agestage in ageAttributes)
        {
            int index = ageAttributes.IndexOf(agestage);
            index++;
            string stage = ((AgeStage)index).ToString();
            AddToList(properties, "", stage);
            AddToList(properties, agestage.viewRadius, $"View distance ({stage})" );
            AddToList(properties, agestage.speed, $"Walking speed ({stage})");
            AddToList(properties, agestage.ageStageUpperPercentage * avarageLifeTime / 100, $"Age group limit ({stage})");
            AddToList(properties, agestage.hungerModifier, $"Hunger ({stage})");
            AddToList(properties, agestage.thirstModifier, $"Thirst ({stage})");
        }

        AddToList(properties, hungerThresherhold, $"HungerThresherhold");
        AddToList(properties, thirstThresherhold, $"ThirstThresherhold");

        AddToList(properties, avarageLifeTime, "Avarage lifetime");
        string enemyTypesString = "";
        foreach (var enemy in enemyTypes)
        {
            if (enemyTypesString != "") enemyTypesString += " & ";
            enemyTypesString += enemy.ToString();
        }
        AddToList(properties, enemyTypesString, "Enemys");
        string foodTypesString = "";
        foreach (var food in foodTypes)
        {
            if (foodTypesString != "") foodTypesString += " & ";
            foodTypesString += food.ToString();
        }
        AddToList(properties, foodTypesString, "Foods");
        AddToList(properties, childBirth, "Most childbirth in a pregnancy");

        var MC = GetComponent<MatingController>();
        AddToList(properties, MC.matingTime, "Mating duration");
        AddToList(properties, MC.pregnancyTime, "Pregnacy duration");
        AddToList(properties, MC.spermatogenesisTime, "Spermatogenesis duration");
        AddToList(properties, formPack, "Pack forming");

        return properties;
    }

    void AddToList(Dictionary<string, string> properties, System.Object o, string name)
    {
        properties.Add(name, o.ToString());
    }
}
