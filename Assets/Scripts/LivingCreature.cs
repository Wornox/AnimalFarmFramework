using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class LivingCreature : MonoBehaviour
{
    AnimalComponentReferences ACR;
    AnimalProperties AP;
    AnimalBehaviorController ABC;
    AnimalController AC;
    VegetableSpawner VS;

    public CreatureType creatureType;

    public bool alive;
    public float foodSource;
    private Animator animator;
    private bool canBeDeleted = false; // only after death registery can delete the whole animal



    public float vegetableRegenRate = 1.2f;

    private void Awake()
    {
        ACR = GetComponent<AnimalComponentReferences>();
        AP = GetComponent<AnimalProperties>();
        ABC = GetComponent<AnimalBehaviorController>();
        if (ACR.animalMeshController != null && ACR.animalMeshController.animator != null) animator = ACR.animalMeshController.animator;
        AC = GameObject.Find("AnimalController").GetComponent<AnimalController>();
        VS = GameObject.Find("GrainObject").GetComponent<VegetableSpawner>();
    }

    void Update()
    {
        if (foodSource < 0.1f)
        {
            if (creatureType == CreatureType.Vegetable)
            {
                Destroy(gameObject);
                VS.currentGrainNumber--;

            }
            else if (creatureType == CreatureType.Animal)
            {
                if (canBeDeleted) Destroy(gameObject);
            }
        }

        switch (alive)
        {
            case true:
                AliveCreature();
                break;
            case false:
                DeadCreature();
                break;
        }

    }

    void AliveCreature()
    {
        if (creatureType == CreatureType.Vegetable)
        {
            if (foodSource < 75f)
            {
                foodSource += Time.deltaTime * vegetableRegenRate;
                ACR.animalStatusController.SetHunger(foodSource);
            }
        }
        else if (creatureType == CreatureType.Animal)
        {
            //AP.age = Time.timeSinceLevelLoad - AP.bornTime;
            //CheckElderDeath();
        }

        UpdateStatusCanvasAliveCreature();

    }

    void DeadCreature()
    {
        //Corpse vanishes overtime
        foodSource -= Time.deltaTime * 0.9f;
        GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        UpdateStatusCanvasDeadCreature();

    }

    void UpdateStatusCanvasAliveCreature()
    {
        var agetext = ACR.animalStatusController.AgeText;
        if (agetext != null) agetext.text = ((int)AP.ACR.ageController.age).ToString() + " : " + AP.ACR.ageController.ageStage.ToString();

    }

    void UpdateStatusCanvasDeadCreature()
    {
        ACR.animalStatusController.SetStatus("Corpse");
        ACR.animalStatusController.SetHunger(foodSource);
        var thirst = ACR.animalStatusController.Thirst;
        var mating = ACR.animalStatusController.MatingText;
        var status = ACR.animalStatusController.StatusText;
        if (thirst != null) Destroy(thirst.gameObject, 0);
        if (mating != null) Destroy(mating.gameObject, 0);
        if (status != null) Destroy(status.gameObject, 0);
    }


    public void CreatureDied(CauseOfDeath causeOfDeath)
    {
        if (!alive) return;
        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.updatePosition = false;
        }

        if (creatureType == CreatureType.Animal) // only runs once
        {
            alive = false;
            if(animator != null) animator.SetBool("Dead", true);

            var MC = GetComponent<MatingController>();
            var sex = MC.sex;

            AC.animal.DeathOfAnimal(AP.animalType, this.gameObject, AP.ACR.ageController.ageStage, causeOfDeath, sex);

            //Plants dont have AnimalBehaviorController, plants foodsource stays, but animals food source will be corrected to their hunger level
            //Hunger animal means less nutrients means less foodSource for the predator
            if (ABC != null)
            {
                foodSource = ABC.hunger + 10;
                Destroy(ABC, 0);
            }
            if (MC != null)
            {
                Destroy(MC, 0);
            }

            var scanner = GetComponent<Scanner>();
            if (scanner != null)
            {
                Destroy(scanner, 0);
            }

            var LR = GetComponent<AnimalComponentReferences>().animalMeshController.lineRenderer;
            if (LR != null)
            {
                Destroy(LR, 0);
            }

            var PC = GetComponent<PackController>();
            if (PC != null && PC.pack != null)
            {
                PC.pack.RemoveMemember(this.gameObject);
                Destroy(PC, 0);
            }

            canBeDeleted = true;
            UpdateStatusCanvasDeadCreature();
        }
    }

    public float GetFood(float amount)
    {
        float modifier = 0.1f;
        if (creatureType == CreatureType.Vegetable) modifier = 1f;
        foodSource -= amount * modifier;
        UpdateStatusCanvasDeadCreature();
        return amount;
    }

    public float CheckFoodSource()
    {
        return foodSource;
    }


    

}
