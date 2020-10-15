using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Unity.Profiling;
using Unity.Jobs;

public class AnimalBehaviorController : MonoBehaviour
{
    AnimalComponentReferences ACR;
    AnimalProperties AP;
    AnimalStatusController ASC;

    public Phase phase;

    [Range(0, 100)]
    public float hunger;

    [Range(0, 100)]
    public float thirst;

    private bool godSetDestination;

    private Camera cam;

    private Scanner scanner;

    private NavMeshAgent agent;

    public Vector3 targetPos;

    private Animator anim;

    private AnimalProperties prop;

    private MatingController MC;

    List<Vector3> enemys;
    List<Vector3> foods;
    List<GameObject> waters;
    List<GameObject> allies;

    Vector3 fleeVec = Vector3.zero;

    Vector3 oldPos;

    public Vector3 directionVector;

    GameObject foodTarget;

    Vector3 waterTargetPos;

    //LineRenderer lRend;

    private float reacheckedForNewTarget;

    private LivingCreature lcSelf;

    void Awake()
    {
        ACR = GetComponent<AnimalComponentReferences>();
        AP = GetComponent<AnimalProperties>();
        anim = ACR.animalMeshController.animator;
        agent = GetComponent<NavMeshAgent>();
        MC = GetComponent<MatingController>();
        lcSelf = GetComponent<LivingCreature>();
        ASC = ACR.animalStatusController;

        agent.Warp(transform.position);
        cam = Camera.main;
        scanner = GetComponent<Scanner>();

    }

    void Start()
    {
        oldPos = transform.position;
    }

    static ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("MySystem.Prepare");

    public void Update()
    {
        agent.speed = AP.speed;
        Logic();
        PhaseExecute();

        hunger -= Time.deltaTime * 0.05f * AP.hungerModifier;
        thirst -= Time.deltaTime * 0.05f * AP.thirstModifier;

        CheckHungerThirst();

        if (GetComponent<LivingCreature>().alive) UpdateStatusCanvas();

    }

    void UpdateStatusCanvas()
    {
        ASC.StatusText.text = phase.ToString();
        ASC.SetHunger(hunger);
        ASC.SetThirst(thirst);
    }

    (GameObject, float, bool) GetClosestGO(Collider[] list, bool onlyAlive)
    {
        if (list.Length < 1) return (null, float.MaxValue, false);

        GameObject closest = null;
        float closestDistance = float.MaxValue;
        bool found = false;

        foreach (var o in list)
        {
            float dis = Vector3.Distance(transform.position, o.transform.position);

            bool condition = true;
            var LC = o.gameObject.GetComponent<LivingCreature>();
            if (onlyAlive && LC != null) condition = LC.alive;

            if (dis < closestDistance && condition)
            {
                closest = o.gameObject;
                closestDistance = dis;
                found = true;
            }
        }
        return (closest, closestDistance, found);
    }

    void Logic()
    {
        fleeVec = Vector3.zero;

        // *** if enemy spotted then instant fleeing in any state  *** 
        string[] enemyMask = new string[AP.enemyTypes.Count];
        for (int i = 0; i < AP.enemyTypes.Count; i++)
        {
            enemyMask[i] = AP.enemyTypes[i].ToString();
        }
        Collider[] scannedObjsEnemy = scanner.ScanArea(LayerMask.GetMask(enemyMask));
        (GameObject closestEnemy, float closestEnemyDistance, bool foundEnemy) = GetClosestGO(scannedObjsEnemy, true);

        //FUTURE REWORK: combine all enemy fleeVecs based on the distance from the target, to get better fleeing direction

        if (foundEnemy)
        {
            phase = Phase.Fleeing;
            foodTarget = null;
            waterTargetPos = Vector3.zero;
            MC.matingPartner = null;
            fleeVec = Vector3.Normalize(transform.position - closestEnemy.transform.position);
            directionVector = fleeVec; // after fleeing ended new diretion will be still steeing away from prev danger with random exploring
            SetNewRandomTargetPos();
            return; //-> PhaseExecute
        }
        else
        {
            if (phase == Phase.Fleeing) phase = Phase.Exploring; //setting back to deafult state, but no return, letting food, drink scan in same iteration
        }


        // *** if mating and no danger -> continue mating *** 
        if (phase == Phase.Mating)
        {
            if (MC.matingPartner == null)
            {
                phase = Phase.Exploring; //setting back to deafult state, but no return, letting food, drink scan in same iteration
            }
            else
            {
                return; //-> PhaseExecute
            }
        }

        // *** until this point no enemy detected, decide to drink/eat if already not doing that ***

        // *** if already chasing(eating) enemy ***
        if (phase == Phase.Eating)
        {
            if (foodTarget == null || !agent.hasPath)
            {
                //lost target or unreachable -> back to exploring
                foodTarget = null;
                phase = Phase.Exploring;
                SetNewRandomTargetPos();
                return;  //-> PhaseExecute
            }
            else
            {
                //1 second has passed since scanning, scan for new target
                string[] foodMask = new string[AP.foodTypes.Count];
                for (int i = 0; i < AP.foodTypes.Count; i++)
                {
                    foodMask[i] = AP.foodTypes[i].ToString();
                }
                Collider[] scannedObjsFood = scanner.ScanArea(LayerMask.GetMask(foodMask), AP.viewRadius / 3); //has a target, but scanning with lower range for new closer one
                (GameObject closestFood, float closestFoodDistance, bool found) = GetClosestGO(scannedObjsFood, false);

                if (found)
                {
                    //closer animal found -> setting new target
                    targetPos = closestFood.transform.position;
                    foodTarget = closestFood;
                    agent.SetDestination(targetPos);
                }
                else
                {
                    //no closer animal -> continuting hunting original one
                    targetPos = foodTarget.transform.position;
                    agent.SetDestination(targetPos);
                }
                return; //-> PhaseExecute
            }

        }

        // *** if not chaing and low hunger -> scan for food ***
        if (phase == Phase.Exploring && hunger < AP.hungerThresherhold)
        {
            string[] foodMask = new string[AP.foodTypes.Count];
            for (int i = 0; i < AP.foodTypes.Count; i++)
            {
                foodMask[i] = AP.foodTypes[i].ToString();
            }
            Collider[] scannedObjsFood = scanner.ScanArea(LayerMask.GetMask(foodMask));
            (GameObject closestFood, float closestFoodDistance, bool found) = GetClosestGO(scannedObjsFood, false);

            if (found)
            {
                //found target, setting destiantion
                phase = Phase.Eating;
                foodTarget = closestFood;
                targetPos = closestFood.transform.position;
                agent.SetDestination(targetPos);
                return; //-> PhaseExecute
            }
            else
            {
                //no target found, continue logic
            }
        }

        // *** if already selected a drinking source ***
        if (phase == Phase.Drinking)
        {
            //waters doesnt move -> no need to check if lost
            agent.updatePosition = true;
            agent.SetDestination(targetPos);
            if (targetPos == null)
            {
                phase = Phase.Exploring;
            }
        }

        // *** if not drinking already and lwo thirst -> scan for water ***
        if (phase == Phase.Exploring && thirst < AP.thirstThresherhold)
        {
            Collider[] scannedObjsWater = scanner.ScanArea(LayerMask.GetMask(AP.waterType.ToString()));
            (GameObject closestWaterTile, float closestWaterPivotDistance, bool foundWaterTile) = GetClosestGO(scannedObjsWater, false);

            if (foundWaterTile)
            {
                (Vector3 borderPoint, bool foundPoint) = closestWaterTile.GetComponent<WaterLevel>().GetClosestBorderPoint(transform.position, AP.viewRadius);
                if (foundPoint)
                {
                    //found water, setting destination
                    phase = Phase.Drinking;
                    waterTargetPos = borderPoint;
                    targetPos = borderPoint;
                    agent.SetDestination(targetPos);
                    return; //PhaseExecute
                }
                else
                {
                    //no water found, back to exploring
                }
            }
        }

        // *** no enemy, no mating, no eating, no drinking ----> setting to default explorations state ***
        if (phase == Phase.Exploring)
        {
            agent.updatePosition = true;

            if (Vector3.Distance(transform.position, targetPos) < 0.5 || (!agent.hasPath && !agent.pathPending)) // select new explore point if reached previous exploration point or destination cant be reached
            {
                SetNewRandomTargetPos();
            }

            //checking for pack
            if (GetComponent<PackController>() != null)
            {
                Collider[] scannedObjsAllies = scanner.ScanArea(LayerMask.GetMask(AP.animalType.ToString()));
                GetComponent<PackController>().LookForPack(scannedObjsAllies);
            }
        }
    }

    void SetNewRandomTargetPos()
    {
        agent.ResetPath();
        Vector3 validatedPosition = Vector3.zero;

        //temporaly vector for calculation
        Vector3 newDirectionVector;

        //generate random normalized pos
        Vector3 randomVec = Random.onUnitSphere;
        randomVec = new Vector3(randomVec.x, 0, randomVec.y);
        randomVec = Vector3.Normalize(randomVec);

        //origo vec
        Vector3 origoVector = Vector3.zero - transform.position; // DONT USE
        origoVector = Vector3.Normalize(origoVector); // DONT USE

        if (fleeVec != Vector3.zero)
        {
            //fleeing from danger with little randomness
            newDirectionVector = Vector3.LerpUnclamped(randomVec, fleeVec, 0.75f); //0.75f means 25%-75% weight of the 2 vecs
            newDirectionVector = Vector3.Normalize(newDirectionVector);
        }
        else if (GetComponent<PackController>() != null && GetComponent<PackController>().pack != null)
        {
            //going with the pack
            if (GetComponent<PackController>().pack.leader == this) //I'm the leader -> random movement
            {
                newDirectionVector = Vector3.LerpUnclamped(randomVec, directionVector, 0.75f); // keep previous direction vector form previous move and add 25% randomness
                newDirectionVector = Vector3.Normalize(newDirectionVector);
            }
            else
            {
                // asking pack diraction and considerin into new direction
                Vector3 packDirection = Vector3.zero;
                Vector3 leaderPos = Vector3.zero;
                var pack = GetComponent<PackController>().pack;
                if (pack != null)
                {
                    packDirection = pack.GetPackDirection();
                    leaderPos = pack.leader.transform.position;
                }
                Vector3 leaderVec = leaderPos - transform.position;
                if (leaderVec.sqrMagnitude > AP.viewRadius * AP.viewRadius) // if leader out of range go to leader
                {
                    newDirectionVector = Vector3.Normalize(leaderVec);
                }
                else // leader is close, copying 
                {
                    newDirectionVector = Vector3.LerpUnclamped(randomVec, packDirection, 0.75f);
                    newDirectionVector = Vector3.Normalize(newDirectionVector);
                }

            }

        }
        else
        {
            //no need to flee -> random direction
            newDirectionVector = Vector3.LerpUnclamped(randomVec, directionVector, 0.75f); // keep previous direction vector form previous move and add 25% randomness
            newDirectionVector = Vector3.Normalize(newDirectionVector);
        }

        Vector3 newPos = newDirectionVector * AP.viewRadius + transform.position; //picking the point of the edge of viewradius depanding on the direction vec

        //testing if valid point found on navmesh
        NavMeshHit hit;
        NavMesh.SamplePosition(newPos, out hit, AP.viewRadius / 2, 1);

        if (!float.IsPositiveInfinity(hit.position.x)) // checking if hit found a valid point valid point
        {
            //valid point found -> calculate path
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, hit.position, 1, path);
            if (path.status == NavMeshPathStatus.PathComplete) //path is complete = destiantion position can be reached
            {
                agent.path = path;
                validatedPosition = hit.position;
            }
            else
            {
                //no valid point could be found near target, stay still
                validatedPosition = transform.position;
            }
        }
        else
        {
            //couldnt find a valid point towards the middle = cant go to middle -> pick new random valid point around target

            Vector3 randomPoint;
            RandomPoint(transform.position, AP.viewRadius, out randomPoint);

            if (!float.IsPositiveInfinity(randomPoint.x)) // checking if hitFromMiddle found a valid point valid point
            {
                //valid point found -> calculate path
                NavMeshPath pathFromMiddle = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, randomPoint, 1, pathFromMiddle);
                if (pathFromMiddle.status == NavMeshPathStatus.PathComplete) //path is complete = destiantion position can be reached
                {
                    agent.path = pathFromMiddle;
                    validatedPosition = randomPoint;
                }
            }
            else
            {
                //no valid point could be found near target, stay still
                validatedPosition = transform.position;
            }
        }

        targetPos = validatedPosition;
        agent.SetDestination(targetPos);

        directionVector = (targetPos - transform.position).normalized;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    void PhaseExecute()
    {
        if (phase == Phase.Fleeing)
        {
            anim.SetInteger("Walk", 1);
            agent.updatePosition = true;

        }
        else if (phase == Phase.Exploring)
        {
            anim.SetInteger("Walk", 1);
            agent.updatePosition = true;
        }
        else if (phase == Phase.Eating)
        {
            if (foodTarget != null && (foodTarget.transform.position - transform.position).sqrMagnitude < 1) // stop if close to water
            {
                LivingCreature prey = foodTarget.GetComponent<LivingCreature>();
                if (prey != null) prey.CreatureDied(CauseOfDeath.predatorDeath);

                agent.updatePosition = false;
                anim.SetInteger("Walk", 0);
                agent.velocity = Vector3.zero;
                float gotFoodAmount = prey.GetFood(Time.deltaTime * 6f);
                hunger += gotFoodAmount;
            }

            if (hunger > 99)
            {
                agent.updatePosition = true;
                phase = Phase.Exploring;
                foodTarget = null;
                SetNewRandomTargetPos();
            }
        }
        else if (phase == Phase.Drinking)
        {
            if ((waterTargetPos - transform.position).sqrMagnitude < 4) // stop if close to water
            {
                //near water start drinking
                agent.updatePosition = false;
                anim.SetInteger("Walk", 0);
                agent.velocity = Vector3.zero;
                thirst += Time.deltaTime * 4;
            }
            if (thirst > 99) //drank enough -> back to exploring
            {
                agent.updatePosition = true;
                phase = Phase.Exploring;
                SetNewRandomTargetPos();
            }
        }
        else if (phase == Phase.Mating)
        {
            anim.SetInteger("Walk", 0);
        }
    }

    public void CheckHungerThirst()
    {
        if (hunger < 0.1f)
        {
            lcSelf.CreatureDied(CauseOfDeath.hungerDeath);
        }

        if (thirst < 0.1f)
        {
            lcSelf.CreatureDied(CauseOfDeath.thirstDeath);
        }
    }

    public void ToggleMovement(bool enableMovement)
    {
        agent.updatePosition = enableMovement;
    }

    public void GoToMate(GameObject mate)
    {
        ToggleMovement(true);
        phase = Phase.Mating;
        if (mate != null)
        {
            targetPos = mate.transform.position;
            agent.SetDestination(targetPos);
        }
        else SetNewRandomTargetPos();
    }

    public void StartMating(GameObject mate)
    {
        ToggleMovement(false);
        phase = Phase.Mating;
    }

    public void MatingDone()
    {
        ToggleMovement(true);
        phase = Phase.Exploring;
    }

    public void setAgentSpeed(float speed)
    {
        agent.speed = speed;
    }
}
