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

    public CreatureType creatureType;

    public bool alive;
    public float foodSource;
    private Animator animator;
    private bool canBeDeleted = false; // only after death registery can delete the whole animal

    private float checkElderDeathAge = 0f;
    private float checkElderDeathTimer = 10f;

    public float vegetableRegenRate = 1.2f;

    private void Awake()
    {
        ACR = GetComponent<AnimalComponentReferences>();
        AP = GetComponent<AnimalProperties>();
        ABC = GetComponent<AnimalBehaviorController>();
        if (ACR.animalMeshController != null && ACR.animalMeshController.animator != null) animator = ACR.animalMeshController.animator;
        AC = GameObject.Find("AnimalController").GetComponent<AnimalController>();
    }

    void Update()
    {
        if (foodSource < 0.1f)
        {
            if (creatureType == CreatureType.Vegetable)
            {
                Destroy(gameObject);
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
            AP.age = Time.timeSinceLevelLoad - AP.bornTime;
            CheckElderDeath();
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
        if (agetext != null) agetext.text = ((int)AP.age).ToString() + " : " + AP.ageStage.ToString();

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

            AC.animal.DeathOfAnimal(AP.animalType, this.gameObject, AP.ageStage, causeOfDeath, sex);

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


    void CheckElderDeath()
    {
        if (AP.ageStage == AgeStage.Elder && checkElderDeathAge + checkElderDeathTimer < AP.age)
        {
            checkElderDeathAge = AP.age; // resetting last elder death check age time
            // (elderAgeStart = elderThreshold * avarageLifeTime)
            // ((460-450) / (600-450))/2 = 10/150 / 2      = 0.03% 
            // ((600-450) / (600-450))/2 = 150/150 / 2     = 50% (50% to die if age is equal to avarage lifetime)
            // ((620-450) / (600-450))/2 = 170/150 / 2     = 56.6% (after avarage lifetime deth % is bigger)
            float deathPercentage = ((AP.age - AP.elderAgeStart) / (AP.avarageLifeTime - AP.elderAgeStart)) / 2f;
            float random = Random.Range(0.0f, 1.0f);
            if (random <= deathPercentage)
            {
                //animalController.GetComponent<AnimalController>().RemoveAnimalFromList(gameObject);
                CreatureDied(CauseOfDeath.ageDeath);
            }
        }
    }

}
