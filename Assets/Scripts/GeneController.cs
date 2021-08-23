using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GeneAttributes
{
    public MutationType SpeedMutationType;
    [Range(0f,1f)]
    public float SpeedMutationProbability;
    public float SpeedMutationValueMin;
    public float SpeedMutationValueMax;
    public float DominantSpeed;
    public float RecessiveSpeed;

    public MutationType MetabolismMutationType;
    [Range(0f, 1f)]
    public float MetabolismMutationProbability;
    public float MetabolismMutationValueMin; 
    public float MetabolismMutationValueMax; 
    public float DominantMetabloism;
    public float RecessiveMetabloism;

    public MutationType ReproductionRateMutationType;
    [Range(0f, 1f)]
    public float ReproductionRateMutationProbability;
    public float ReproductionRateMutationValueMin;
    public float ReproductionRateMutationValueMax;
    public float DominantReproductionRate;
    public float RecessiveReproductionRate;
}

public class GeneController : MonoBehaviour
{
    public GeneStructure geneStructure = new GeneStructure();
    public GeneAttributes geneAttributes;
    public bool isGeneHeritanceEnabled;

    /// <summary>
    /// Gets the default values from the inspector and generates the first animal's "dummy" gene structre
    /// </summary>
    public void GenerateDummyGene()
    {
        geneStructure = GenerateDummyGeneStructure(
            geneAttributes.SpeedMutationValueMin,
            geneAttributes.SpeedMutationValueMax,
            geneAttributes.DominantSpeed,
            geneAttributes.RecessiveSpeed,
            geneAttributes.MetabolismMutationValueMin,
            geneAttributes.MetabolismMutationValueMax,
            geneAttributes.DominantMetabloism,
            geneAttributes.RecessiveMetabloism,
            geneAttributes.ReproductionRateMutationValueMin,
            geneAttributes.ReproductionRateMutationValueMax,
            geneAttributes.DominantReproductionRate,
            geneAttributes.RecessiveReproductionRate,
            0
            );

        Mutate();
    }

    public void RefreshInspectorValues()
    {
        //only to show in inspector the new values
        geneAttributes.DominantSpeed = geneStructure.GetGeneStructure(GeneAttributeType.Speed).dominant;
        geneAttributes.RecessiveSpeed = geneStructure.GetGeneStructure(GeneAttributeType.Speed).recessive;
        geneAttributes.DominantMetabloism = geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).dominant;
        geneAttributes.RecessiveMetabloism = geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).recessive;
        geneAttributes.DominantReproductionRate = geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).recessive;
        geneAttributes.RecessiveReproductionRate = geneStructure.GetGeneStructure(GeneAttributeType.Metabolism).recessive;
    }

    /// <summary>
    /// Inherits the combination of the father's and mother's genstructure's combination
    /// </summary>
    /// <param name="fatherGeneStructure"></param>
    /// <param name="motherGeneStructure"></param>
    public void Inherit(GeneStructure fatherGeneStructure, GeneStructure motherGeneStructure)
    {
        geneStructure.Inherit(fatherGeneStructure, motherGeneStructure);

        RefreshInspectorValues();
    }

    /// <summary>
    /// Mutate the current gene structure with predefined mutation methods and 
    /// </summary>
    public void Mutate()
    {
        geneStructure.Mutate(GeneAttributeType.Speed, geneAttributes.SpeedMutationProbability, geneAttributes.SpeedMutationType);
        geneStructure.Mutate(GeneAttributeType.Metabolism, geneAttributes.MetabolismMutationProbability, geneAttributes.MetabolismMutationType);
        geneStructure.Mutate(GeneAttributeType.ReproductionRate, geneAttributes.ReproductionRateMutationProbability, geneAttributes.ReproductionRateMutationType);

        RefreshInspectorValues();
    }

    GeneStructure GenerateDummyGeneStructure(
        float SpeedMutationValueMin, float SpeedMutationValueMax, float DominantSpeed, float RecessiveSpeed,
        float MetabloismMutationValueMin, float MetabloismMutationValueMax, float DominantMetabloism, float RecessiveMetabloism,
        float ReproductionRateMutationValueMin, float ReproductionRateMutationValueMax, float DominantReproductionRate, float RecessiveReproductionRate,
        float Generation
        )
    {
        Gene dummySpeed = GenerateDummyGene(DominantSpeed, RecessiveSpeed, DominantRecessiveInheritancePriority.Bigger, GeneAttributeType.Speed);
        Gene dummyMetabolism = GenerateDummyGene(DominantMetabloism, RecessiveMetabloism, DominantRecessiveInheritancePriority.Lower, GeneAttributeType.Metabolism);
        Gene dummyReproductionRate = GenerateDummyGene(DominantReproductionRate, RecessiveReproductionRate, DominantRecessiveInheritancePriority.Lower, GeneAttributeType.Metabolism);

        GeneStructure dummyGeneStructure = new GeneStructure();
        dummyGeneStructure.AddAttribute(
            GeneAttributeType.Speed,
            dummySpeed);
        dummyGeneStructure.AddAttribute(
            GeneAttributeType.Metabolism,
            dummyMetabolism);
        dummyGeneStructure.AddAttribute(
            GeneAttributeType.ReproductionRate,
            dummyReproductionRate);

        dummyGeneStructure.defaultMutationValues = new Dictionary<GeneAttributeType, (float, float)>();
        dummyGeneStructure.defaultMutationValues.Add(GeneAttributeType.Speed, (SpeedMutationValueMin, SpeedMutationValueMax));
        dummyGeneStructure.defaultMutationValues.Add(GeneAttributeType.Metabolism, (MetabloismMutationValueMin, MetabloismMutationValueMax));
        dummyGeneStructure.defaultMutationValues.Add(GeneAttributeType.ReproductionRate, (ReproductionRateMutationValueMin, ReproductionRateMutationValueMax));

        dummyGeneStructure.generation = Generation;

        return dummyGeneStructure;
    }

    Gene GenerateDummyGene(float dom, float rec, DominantRecessiveInheritancePriority dominantRecessiveInheritancePriority, GeneAttributeType attributeType)
    {
        Gene dummy = new Gene();
        dummy.Specify(dom, rec, dominantRecessiveInheritancePriority, attributeType);
        return dummy;
    }
}


