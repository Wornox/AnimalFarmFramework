using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <param name="Speed">Determines the animal's movement speed</param>
/// <param name="Metabolism">Determines the animal's hunger and thirst parameters</param>
/// </summary>
public enum GeneAttributeType {Speed, Metabolism, ReproductionRate };

/// <summary>
/// <param name="Bigger">Between the two inherited values, the bigger will be the dominant gene</param>
/// <param name="Lower">Between the two inherited values, the lower will be the dominant gene</param>
/// <param name="Random">Between the two inherited values, a randomly choosen will be the dominant gene</param>
/// </summary>
public enum DominantRecessiveInheritancePriority {Bigger, Lower, Random }

/// <summary>
/// <param name="Dominant">Fixly mutate dominant gene with the specified value</param>
/// <param name="Recessive">Fixly mutate recessive gene with the specified value</param>
/// <param name="Both">Mutate both (dominant and recessive) gene with the same specified value</param>
/// <param name="Singleton">Mutate only one (randomly choosen) of the genes (dominant or recessive) with the specified value</param>
/// <param name="Random">Mutate none, one (randomly choosen) or both of the genes (dominant or recessive) with the specified value</param>
/// </summary>
public enum MutationType {Dominant, Recessive, Both, Singleton, Random };

public enum ParentType { Father, Mother};

public class GeneStructure
{
    private Dictionary<GeneAttributeType, Gene> geneStructure = new Dictionary<GeneAttributeType, Gene>();

    public Dictionary<GeneAttributeType, (float, float)> defaultMutationValues = new Dictionary<GeneAttributeType, (float, float)>();

    private GeneStructure faterGeneStructure;
    private GeneStructure motherGeneStructure;

    public float generation;

    public void Inherit(GeneStructure father, GeneStructure mother)
    {
        faterGeneStructure = father;
        motherGeneStructure = mother;
        foreach (GeneAttributeType attributeType in System.Enum.GetValues(typeof(GeneAttributeType)))
        {
            Gene gene = new Gene();
            System.Random rand = new System.Random();
            gene.Inherit(attributeType, faterGeneStructure.GetGeneStructure(attributeType).dominantRecessiveInheritancePriority, faterGeneStructure.GetGeneStructure(attributeType), motherGeneStructure.GetGeneStructure(attributeType), rand);
            AddAttribute(attributeType, gene, true);

            defaultMutationValues = father.defaultMutationValues; //same species, so the mother's the same
        }
        generation = (father.generation + mother.generation) / 2 + 1;
    }

    public bool AddAttribute(GeneAttributeType attributeType, Gene gene, bool force = false)
    {
        bool success;
        if (!geneStructure.ContainsKey(attributeType) || force)
        {
            if (geneStructure.ContainsKey(attributeType))
                geneStructure[attributeType] = gene;
            else
                geneStructure.Add(attributeType, gene);
            success = true;
        }
        else
        {
            success = false;
        }
        return success;
    }

    public void Mutate(GeneAttributeType attributeType, float probablity, MutationType mutationType)
    {
        Gene gene = geneStructure[attributeType];
        gene.Mutate(
            defaultMutationValues[attributeType].Item1,
            defaultMutationValues[attributeType].Item2,
            probablity,
            mutationType);
    }

    public Gene GetGeneStructure(GeneAttributeType attributeType)
    {
        Gene gene;
        if (geneStructure.ContainsKey(attributeType))
            gene = geneStructure[attributeType];
        else
            gene = null;
        return gene;
    }

    public GeneStructure GetParentGeneStructure(ParentType parentType)
    {
        GeneStructure geneStructure;
        if (parentType == ParentType.Father)
            geneStructure = faterGeneStructure;
        else
            geneStructure = motherGeneStructure;
        return geneStructure;
    }

    public void SetGeneration(float generation)
    {
        this.generation = generation;
    }

    public override string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var gene in geneStructure)
        {
            sb.Append(gene.ToString()+ "\n");
        }
        return sb.ToString();
    }
}

public class Gene
{
    public DominantRecessiveInheritancePriority dominantRecessiveInheritancePriority { get; private set; }

    public GeneAttributeType attributeType { get; private set; }

    public float dominant { get; private set; }
    public float recessive { get; private set; }

    public float dominantOriginal { get; private set; }
    public float recessiveOriginal { get; private set; }
    public float mutationValue { get; private set; }
    public string mutationResult { get; private set; }
    


    public void Specify(float dominant, float recessive, DominantRecessiveInheritancePriority dominantRecessiveInheritancePriority, GeneAttributeType attributeType)
    {
        this.dominantRecessiveInheritancePriority = dominantRecessiveInheritancePriority;
        this.attributeType = attributeType;
        this.dominant = dominant;
        this.recessive = recessive;
    }

    private void Set(float dominant, float recessive)
    {
        this.dominant = dominant;
        this.recessive = recessive;
    }

    public void Inherit(GeneAttributeType attributeType, DominantRecessiveInheritancePriority dominantRecessiveInheritancePriority, Gene faterGene, Gene motherGene, System.Random rand)
    {
        this.attributeType = attributeType;
        this.dominantRecessiveInheritancePriority = dominantRecessiveInheritancePriority;

        double randomFater = rand.NextDouble();
        double randomMother = rand.NextDouble();

        float randomValueFromFater = Getvalue(randomFater, faterGene);
        float randomValueFromMother = Getvalue(randomMother, motherGene);

        switch (dominantRecessiveInheritancePriority)
        {
            case DominantRecessiveInheritancePriority.Bigger:
                if (randomValueFromFater >= randomValueFromMother)
                {
                    Set(randomValueFromFater, randomValueFromMother);
                }
                else
                {
                    Set(randomValueFromMother, randomValueFromFater);
                }
                break;
            case DominantRecessiveInheritancePriority.Lower:
                if (randomValueFromFater <= randomValueFromMother)
                {
                    Set(randomValueFromFater, randomValueFromMother);
                }
                else
                {
                    Set(randomValueFromMother, randomValueFromFater);
                }
                break;
            case DominantRecessiveInheritancePriority.Random:
                double randomDomRec = rand.NextDouble();
                if (randomDomRec < 0.5)
                {
                    Set(randomValueFromFater, randomValueFromMother);
                } else
                {
                    Set(randomValueFromMother, randomValueFromFater);
                }
                break;
        }
    }

    public void Mutate(float min, float max, float probability, MutationType mutationType)
    {
        float randProb = Random.Range(0f, 1f);
        float randMutationValue = Random.Range(min, max);

        if(randProb < probability)
        {
            dominantOriginal = dominant;
            recessiveOriginal = recessive;
            mutationValue = randMutationValue;
            mutationResult = "None";

            switch (mutationType)
            {
                case MutationType.Dominant:
                    dominant += randMutationValue;
                    mutationResult = "Dominant";
                    break;
                case MutationType.Recessive:
                    recessive += randMutationValue;
                    mutationResult = "Recessive";
                    break;
                case MutationType.Both:
                    dominant += randMutationValue;
                    recessive += randMutationValue;
                    mutationResult = "Dominant and Recessive";
                    break;
                case MutationType.Singleton:
                    float randomSingle = Random.Range(0f, 1f);
                    if (randomSingle < 0.5)
                    {
                        dominant += randMutationValue;
                        mutationResult = "Dominant";
                    }
                    else
                    {
                        recessive += randMutationValue;
                        mutationResult = "Recessive";
                    }
                    break;
                case MutationType.Random:
                    float randomUniform = Random.Range(0f, 1f);
                    float randomUniform2 = Random.Range(0f, 1f);
                    if (randomUniform < 0.5)
                    {
                        dominant += randMutationValue;
                        mutationResult = "Dominant";
                    }
                    if (randomUniform2 < 0.5)
                    {
                        recessive += randMutationValue;
                        mutationResult = "Recessive";
                    }
                    if (randomUniform < 0.5 && randomUniform2 < 0.5) mutationResult = "Dominant and Recessive";
                    break;
            }

            //Mathf.Clamp(dominant, 0.5f, 1.5f);
            //Mathf.Clamp(recessive, 0.5f, 1.5f);
        }
    }

    private float Getvalue(double probability, Gene gene)
    {
        float value;
        if(probability < 0.5)
        {
            value = gene.dominant;
        } else
        {
            value = gene.recessive;
        }
        return value;
    }

    public override string ToString()
    {
        return "Dominance: " + dominantRecessiveInheritancePriority.ToString() + ", dominant: " + dominant + ", recessive: " + recessive;
    }
}
