using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneStructureTester : MonoBehaviour
{
    void Start()
    {
        foreach (AnimalType animalType in System.Enum.GetValues(typeof(AnimalType)))
        {

            GeneStructure dummyFather = GenerateDummyGeneStructure(2f, 1f, 0.5f, 0.3f, 1f,0.6f);
            GeneStructure dummyMother = GenerateDummyGeneStructure(1.4f, 0.8f, 0.6f, 0.4f, 1.2f, 0.9f);

            Debug.Log(animalType.ToString() + " dummyFather:\n" + dummyFather.ToString());
            Debug.Log(animalType.ToString() + " dummyMother:\n" + dummyMother.ToString());

            GeneStructure child = new GeneStructure();
            child.Inherit(dummyFather, dummyMother);
            
            Debug.Log(animalType.ToString() + " bornChild:\n" + child.ToString());

            child.defaultMutationValues.Add(GeneAttributeType.Speed, (-0.3f, 0.3f));
            child.defaultMutationValues.Add(GeneAttributeType.Metabolism, (-0.25f, 0.25f));
            child.Mutate(GeneAttributeType.Speed, 1, MutationType.Dominant);

            Debug.Log(animalType.ToString() + " bornChildAfterMutation:\n" + child.ToString());

        }

    }

    GeneStructure GenerateDummyGeneStructure(float dom, float rec, float domM, float recM, float domR, float recR)
    {
        Gene dummySpeed = GenerateDummyGene(dom, rec, DominantRecessiveInheritancePriority.Bigger, GeneAttributeType.Speed);
        Gene dummyMetabolism = GenerateDummyGene(domM, recM, DominantRecessiveInheritancePriority.Lower, GeneAttributeType.Metabolism);
        Gene dummyReproductionRate = GenerateDummyGene(domR, recR, DominantRecessiveInheritancePriority.Lower, GeneAttributeType.ReproductionRate);

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

        return dummyGeneStructure;
    }

    Gene GenerateDummyGene(float dom, float rec, DominantRecessiveInheritancePriority dominantRecessiveInheritancePriority, GeneAttributeType attributeType)
    {
        Gene dummy = new Gene();
        dummy.Specify(dom, rec, dominantRecessiveInheritancePriority, attributeType);
        return dummy;
    }

}
