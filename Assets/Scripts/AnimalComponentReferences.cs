using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class AnimalComponentReferences : MonoBehaviour
{
    public AnimalMeshController animalMeshController;
    public AnimalStatusController animalStatusController;
    public Scanner scanner;

    public AnimalProperties animalProperties;
    public AnimalBehaviorController animalBehaviorController;
    public LivingCreature livingCreature;
    public MatingController matingController;
    public PackController packController;
    public GeneController geneController;
    public AgeController ageController;

    [HideInInspector] public AnimalController animalController;

    void Start()
    {
        animalController = GameObject.Find("AnimalController").GetComponent<AnimalController>();
    }
}
