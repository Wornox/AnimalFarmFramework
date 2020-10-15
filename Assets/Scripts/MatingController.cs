using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MatingController : MonoBehaviour
{
    AnimalComponentReferences ACR;
    AnimalProperties AP;

    public MatingPropertie matingPropertie;

    private Scanner scanner;

    public Sex sex;

    public MatingStatus matingStatus;

    private AnimalBehaviorController abc;

    private AnimalController ac;

    public GameObject matingPartner;

    private float matingDistance;

    public float matingTime;

    public float pregnancyTime;

    public float spermatogenesisTime;

    public List<GameObject> parents;

    private bool MatingWaiterFlag = false;
    private bool PregnancyFlag = false;
    private bool CoolDownTimerFlag = false;

    public float startMatingHungerLowTreshold;
    public float startMatingThirstLowTreshold;

    private void Awake()
    {
        ACR = GetComponent<AnimalComponentReferences>();
        if (matingPropertie != null)
        {
            matingTime = matingPropertie.matingTime;
            pregnancyTime = matingPropertie.pregnancyTime;
            spermatogenesisTime = matingPropertie.spermatogenesisTime;
        }

        AP = GetComponent<AnimalProperties>();
        scanner = GetComponent<Scanner>();
        ac = GameObject.Find("AnimalController").GetComponent<AnimalController>();
        abc = GetComponent<AnimalBehaviorController>();
        if (sex == Sex.random) sex = (Sex)Random.Range(0, 2); // get a random sex on start
        matingPartner = null;
        parents = new List<GameObject>();

        matingDistance = 1.5f;
    }

    void Start()
    {

    }

    public void Update()
    {
        ScanForMatingPartner();
        Logic();
        UpdateStatusCanvas();
    }

    void ScanForMatingPartner()
    {
        if (AP.ageStage == AgeStage.Puppy) return; //puppies cant mate

        if (matingPartner != null) return; // if mating partner alredy selected then no need to scan

        if (matingStatus != MatingStatus.readyToMate) return; // if not ready to mate no scan needed

        if (!CheckSelfPhysicalFitness()) return; //not physically fit to mate

        //scanning the area for other animals
        string[] matingMask = new string[1];
        matingMask[0] = AP.animalType.ToString();
        Collider[] scannedObjs = scanner.ScanArea(LayerMask.GetMask(matingMask));

        foreach (var obj in scannedObjs)
        {
            if (obj == null) continue;
            if (obj.tag == "Terrain") continue;

            if (obj.gameObject.GetComponent<LivingCreature>() != null && !obj.gameObject.GetComponent<LivingCreature>().alive) continue; // ignore dead animals

            bool parentFound = false;
            foreach (var p in parents)
            {
                if (p == obj.gameObject) parentFound = true;
            }
            if (parentFound) continue; // skipping parents

            var other = obj.GetComponent<MatingController>();

            if (other == null) continue; // no mating-able animal found

            if (other.AP.animalType == AP.animalType) //self and other animal same spicies
            {
                if (other.sex != sex) // checking if other is from the opposite sex
                {
                    if (CheckOtherIsPreferred(other.gameObject)) //checking if other is preferred
                    {
                        if (other.matingStatus == MatingStatus.readyToMate) // other animal is ready for mating
                        {
                            if (sex == Sex.male) //checing if we are male (male is sending request)
                            {
                                if (other.MatingRequest(this.gameObject)) //sending mating request if accepted feamel set her mating to male
                                {
                                    // mating request accepted
                                    matingPartner = other.gameObject; //setting the male's mating partner to the female
                                    matingStatus = MatingStatus.foundMate;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool MatingRequest(GameObject requester)
    {
        bool answer = false;
        if (matingPartner == null) // only considering mating if not choosen already
        {
            bool like = CheckOtherIsPreferred(requester);
            bool selfPhysicalFitness = CheckSelfPhysicalFitness();
            if (like && selfPhysicalFitness)
            {
                answer = true;
                matingPartner = requester;
                matingStatus = MatingStatus.foundMate;
            }
        }
        return answer;
    }

    private bool CheckSelfPhysicalFitness()
    {
        bool accept = true;
        if (abc.hunger < startMatingHungerLowTreshold) accept = false;
        if (abc.thirst < startMatingThirstLowTreshold) accept = false;
        //TODO: more logic
        return accept;
    }


    /// <summary>
    /// Choosing mating partner
    /// </summary>
    /// <param name="requester">The animal who sends the mating request</param>
    /// <returns></returns>
    private bool CheckOtherIsPreferred(GameObject other)
    {
        bool like;

        //TODO: logic
        //For testing 50% random
        //if (Random.Range(0, 1) == 1) like = true;
        //else like = false;

        //For testing 100% yes
        like = true;

        return like;
    }

    public void ReachedMate()
    {
        //they are close start mating
        matingStatus = MatingStatus.mating;

        abc.StartMating(matingPartner); // both stops and AnimalBehaviorController phase cahgnes to mating
        StartCoroutine(MatingWaiter());
    }

    void Logic()
    {
        if (matingStatus == MatingStatus.foundMate) // if found mate go to it (in this state no mating has been done)
        {
            if (matingPartner != null)
            {
                abc.GoToMate(matingPartner); // telling the AnimalBehaviorController to go to the mate
                if (Vector3.Distance(transform.position, matingPartner.transform.position) < matingDistance) // check if they are near each other
                {
                    //found each other (first run of the detection will set both partners to mating state with ReachMate() method)

                    if (matingPartner.GetComponent<MatingController>() != null)
                    {
                        ReachedMate();
                        matingPartner.GetComponent<MatingController>().ReachedMate();
                    }
                    else matingStatus = MatingStatus.readyToMate;
                }
            }
            else
            {
                //mating partner has been lost, due like death -> cancelling mating -> go back to exploring
                matingStatus = MatingStatus.readyToMate;
                abc.MatingDone();
            }
        }

        if (matingStatus == MatingStatus.mating)
        {
            //nothing to do
        }

        if (sex == Sex.female)
        {
            if (matingStatus == MatingStatus.pregnant)
            {
                if (!PregnancyFlag)
                {
                    PregnancyFlag = true;
                    StartCoroutine(Pregnancy());
                }
            }
        }
        else if (sex == Sex.male)
        {
            if (matingStatus == MatingStatus.inactive)
            {
                if (!CoolDownTimerFlag)
                {
                    CoolDownTimerFlag = true;
                    StartCoroutine(CoolDownTimer());
                }
            }
        }
    }

    IEnumerator MatingWaiter()
    {
        //MatingWaiterFlag = true;
        // suspend execution for matingTime
        yield return new WaitForSeconds(matingTime);

        //change status after mating
        if (sex == Sex.female) matingStatus = MatingStatus.pregnant;
        else matingStatus = MatingStatus.inactive;

        //re-enable movement
        //abc.ToggleMovement(true);

        //if mating duration expires countinue exploring state
        abc.MatingDone();

    }

    IEnumerator Pregnancy()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(pregnancyTime);

        //TODO: child randomization
        SpawnChildren();

        //change status after pregnancy
        matingStatus = MatingStatus.readyToMate;

        //can choose new mating partner
        matingPartner = null;

        PregnancyFlag = false;
    }

    void SpawnChildren()
    {
        //Getting prefab reference form AnimalContoller
        GameObject prefab = ac.GetPrefabByAnimalType(AP.animalType);

        for (int i = 0; i < AP.childBirth; i++)
        {
            //Spawning ONE new animal per cycle
            GameObject child = Instantiate(prefab, transform.position, Quaternion.identity);
            AnimalComponentReferences childACR = child.GetComponent<AnimalComponentReferences>();

            childACR.matingController.matingStatus = MatingStatus.unMature;
            childACR.animalProperties.ageStage = AgeStage.NewBorn;
            childACR.animalBehaviorController.hunger = 25;
            childACR.animalBehaviorController.thirst = 25;
            childACR.matingController.parents.Add(this.gameObject); //add mother
            childACR.matingController.parents.Add(matingPartner); // add father
            childACR.animalStatusController.EnableHungerThirstSlider(AP.drawHungerThirstSlider);

            //----Pack----
            if (ACR.packController != null) //cheking if self animaltype capable of forming packs
            {
                PackController CPC = childACR.packController;
                Pack packToJoin = null;
                PackController MPPC = null;

                if (matingPartner != null) MPPC = matingPartner.GetComponent<PackController>();
                if (MPPC != null && MPPC.pack != null) packToJoin = MPPC.pack; // inherites faterher's pack
                else packToJoin = ACR.packController.pack; // if father is dead -> inherites mother's pack

                CPC.SendPackJoinRequest(packToJoin);
            }

            ac.animal.AddNewAnimal(AP.animalType, child);
            ac.animalReferences.Add((child, childACR));
            childACR.animalProperties.drawHungerThirstSlider = ac.drawHTSlider;
            childACR.animalStatusController.EnableHungerThirstSlider(childACR.animalProperties.drawHungerThirstSlider);

            //giving birth will exhaust the mother
            abc.hunger -= 5;
            abc.thirst -= 5;
            abc.CheckHungerThirst(); //checking hunger/thirst death during labor
        }
    }

    IEnumerator CoolDownTimer()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(spermatogenesisTime);

        //change status back after cooldown
        matingStatus = MatingStatus.readyToMate;

        //can choose new mating partner
        matingPartner = null;

        CoolDownTimerFlag = false;
    }

    void UpdateStatusCanvas()
    {
        //searching for canvas elements wihtout reference
        //foreach (Transform t in transform)
        //{
        //    if (t.name == "StatusCanvas")
        //    {
        //        foreach (Transform tt in t.transform)
        //        {
        //            if (tt.name == "MatingText") tt.GetComponent<TextMesh>().text = sex.ToString() + " : " + matingStatus.ToString();
        //        }
        //    }
        //}

        ACR.animalStatusController.MatingText.text = sex.ToString() + " : " + matingStatus.ToString();
    }
}
