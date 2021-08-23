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

public class AnimalProperties : MonoBehaviour
{
    public float viewRadius;
    public float speed;
    public AnimalType animalType;
    public List<AnimalType> enemyTypes;
    public List<Food> foodTypes;
    public Liquid waterType;

    public float hungerModifier;
    public float thirstModifier;

    public float hungerThresherhold;
    public float thirstThresherhold;

    public float pregnancyTime;

    public bool formPack;

    //Technical
    public AnimalComponentReferences ACR;

    public bool drawHungerThirstSlider = true;

    public void RefreshAttributes(float deltaTime)
    {
        //checking age
        float agePrecentageInStageLerped = ACR.ageController.RefreshAge(deltaTime);

        if(agePrecentageInStageLerped != -1)//-1 means animal reached the end of the Elder stage, no more modification required, the animal will die 
        {
            float ageSpeedLerped = ACR.ageController.GetLerpedValue(AgeAttributeType.speed, agePrecentageInStageLerped);
            float ageViewRadiusLerped = ACR.ageController.GetLerpedValue(AgeAttributeType.viewRadius, agePrecentageInStageLerped);
            float ageHungerLerped = ACR.ageController.GetLerpedValue(AgeAttributeType.agehungerModifier, agePrecentageInStageLerped);
            float ageThirstLerped = ACR.ageController.GetLerpedValue(AgeAttributeType.agethirstModifier, agePrecentageInStageLerped);
            float ageSizeLerped = ACR.ageController.GetLerpedValue(AgeAttributeType.ageSizeModifier, agePrecentageInStageLerped);
            float pregnancyTimeMC = ACR.matingController.pregnancyTime;


            //checking genes
            float speedGene = 1;
            float metabolismGene = 1;
            float reproductionRateGene = 1;
            if (ACR.geneController.isGeneHeritanceEnabled)
            {
                GeneStructure geneStructure = ACR.geneController.geneStructure;
                speedGene = geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominant;
                metabolismGene = geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominant;
                reproductionRateGene = geneStructure.GetGeneStructure(GeneAttributeType.ReproductionRate).dominant;
            }

            //refreshingAttributes
            speed = ageSpeedLerped + speedGene;
            viewRadius = ageViewRadiusLerped;
            hungerModifier = ageHungerLerped + metabolismGene;
            thirstModifier = ageThirstLerped + metabolismGene; 
            transform.localScale = new Vector3(ageSizeLerped, ageSizeLerped, ageSizeLerped);
            ACR.matingController.SetMatingStatus((int)ACR.ageController.GetCurrentAttribute(AgeAttributeType.matingStatus), ACR.ageController.ageStage);
            pregnancyTime = pregnancyTimeMC + reproductionRateGene;
        }

    }

    //If called attributes will be rewritten by geneStructure values
    public void ApplyGeneAttributes()
    {
        hungerModifier = ACR.geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominant;
        thirstModifier = ACR.geneController.geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominant;

        speed = ACR.geneController.geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominant;
    }

}
