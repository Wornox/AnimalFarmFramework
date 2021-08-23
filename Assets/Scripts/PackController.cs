using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PackNames
{
    static int index = 1;

    static Dictionary<AnimalType, int> dict = new Dictionary<AnimalType, int>();
    public static (string, string) Getname(AnimalType animalType)
    {
        string ID = "Pack" + index.ToString();
        index++;

        string name = animalType + "Pack";
        int value = 1;

        if (dict.TryGetValue(animalType, out value))
        {
            name += value + 1;
            dict[animalType] = value + 1;
        }
        else
        {
            dict.Add(animalType, 1);
            name += 1;
        }

        return (ID, name);
    }

    public static void Restart()
    {
        index = 1;
        dict = new Dictionary<AnimalType, int>();
    }
}

public class Pack
{
    public string id;
    public string packName;
    public GameObject leader;
    public int maxPackSize;
    List<GameObject> memebers;

    public GameObject sharedHuntingTarget; //TODO: implement -> if leader marks and enemy to hunt, all memebers will lock on target

    public Vector3 packDirection { get; private set; }

    public Pack(GameObject leader, int maxPackSize)
    {
        (this.id,this.packName) = PackNames.Getname(leader.GetComponent<AnimalProperties>().animalType);
        leader.GetComponent<PackController>().imLeader = true;
        this.leader = leader;
        this.maxPackSize = maxPackSize;
        memebers = new List<GameObject>();
        memebers.Add(leader);
    }
    
    public bool JoinRequest(GameObject candidate, bool forceAccept)
    {
        bool accepted;
        if (forceAccept) accepted = true;
        else accepted = JoinLogic(candidate);
        if (accepted)
        {
            memebers.Add(candidate);
            UpdateMemebersText();
        }
        return accepted;
    }

    private void UpdateMemebersText()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var m in memebers)
        {
            if (m == null)
            {
                toRemove.Add(m);
                continue;
            }
            var PC = m.GetComponent<PackController>();
            if(PC != null) PC.SetPackStatus(this);
        }
        foreach (var rem in toRemove)
        {
            RemoveMemember(rem);
        }
    }

    private bool JoinLogic(GameObject candidate)
    {
        bool accept = false;
        if (memebers.Count < maxPackSize) accept = true;
        else accept = false;
        return accept;
    }

    //public void ChildJoin(GameObject child)
    //{
    //    memebers.Add(child);
    //}

    public int PackSize()
    {
        return memebers.Count;
    }

    public Vector3 GetPackDirection()
    {
        Vector3 dir = Vector3.zero;
        if(leader == null)
        {
            //electing new leader, beacuse previous died
            if (memebers.Count > 0) leader = ElectNewLeader();
        } else
        {
            if (leader.GetComponent<AnimalBehaviorController>() != null) dir = leader.GetComponent<AnimalBehaviorController>().directionVector;
            else dir = Vector3.zero;
        }
        packDirection = dir;
        return dir;
    }

    private GameObject ElectNewLeader()
    {
        if (memebers.Count < 1) return null;
        GameObject oldest = null;
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var m in memebers)
        {
            if(m == null)
            {
                toRemove.Add(m);
                continue;
            } else
            {
                var LC = m.GetComponent<LivingCreature>();
                var PC = m.GetComponent<PackController>();

                if (m.GetComponent<AnimalProperties>() == null ||
                    LC == null ||
                    !LC.alive ||
                    PC == null
                    )  // LivingCreature die method sould remove deda drom list, but sometimes stays
                {
                    toRemove.Add(m);
                    continue;
                }
            }
            
        }

        foreach (var m in toRemove) //removing null members
        {
            RemoveMemember(m);
        }

        if (memebers.Count < 1) return null;


        foreach (var m in memebers)
        {
            var ACR = m.GetComponent<AnimalComponentReferences>();
            ACR.packController.SetPackStatus(this);
            if (oldest == null)
            {
                oldest = m;
                continue;
            }
            if (ACR.ageController.age > oldest.GetComponent<AgeController>().age) oldest = m;
        }

        
        if(oldest == null)
        {
            return null;
        }

        if (oldest.GetComponent<PackController>() == null)
        {
            return null;
        }
        oldest.GetComponent<PackController>().SetPackStatus(this);
        oldest.GetComponent<PackController>().imLeader = true;
        return oldest;
    }

    public void RemoveMemember(GameObject go)
    {
        bool leaderLeaving = (leader == go); //if leader leaves set to true
        memebers.Remove(go);
        if (leaderLeaving) ElectNewLeader(); //after leader has been removed, Elect new leader
    }

}

public class PackController : MonoBehaviour
{
    public Pack pack;
    public bool imLeader = false;
    public int maxPackSize;

    AnimalComponentReferences ACR;
    //[SerializeField] private Vector3 packdirection;
    //public int packChangePossibility;

    void Awake()
    {
        ACR = GetComponent<AnimalComponentReferences>();
    }

    void Start()
    {
        if (!GetComponent<AnimalProperties>().formPack) Destroy(this);
        if (ACR == null)
        {
            int a = 5;
        }
    }

    void Update()
    {
        if(pack == null 
            //&& GetComponent<AnimalProperties>().ageStage == AgeStage.Juvenile
            ) // if adult and not belonging to a group, create one
        {
            CreateNewPack();
        } else
        {
            int b = 6;
        }
            
        //packdirection = pack.packDirection;

    }

    public void CreateNewPack()
    {
        pack = new Pack(this.gameObject, maxPackSize);
        SetPackStatus(pack);
    }

    public void SetPackStatus(Pack pack)
    {
        string packText = pack.packName;
        packText += ", " + pack.PackSize();
        if (imLeader) packText += ", Leader";
        if(ACR == null)
        {
            int a = 5;
        }else
        {
            ACR.animalStatusController.PackText.text = packText;
        }
    }

    public void LookForPack(Collider[] allies)
    {
        foreach (var ally in allies)
        {
            var PC = ally.gameObject.GetComponent<PackController>();
            if (PC == null) continue; //cant from a pack with someone who doesnt have a PackController

            Pack otherPack = PC.pack;

            if (otherPack != null)  //if ally doesnt have pack, I cant join
            {
                Pack selfPack = GetComponent<PackController>().pack;
                if (otherPack == selfPack) continue; // same pack -> skip
                if (selfPack == null)
                {
                    //no pack, trying to join other
                    SendPackJoinRequest(otherPack);
                } else
                {
                    //has already pack, but consider changing
                    if (ally.gameObject.GetComponent<PackController>().pack.PackSize() > GetComponent<PackController>().pack.PackSize())
                    {
                        //bigger pack, tryig to join
                        SendPackJoinRequest(otherPack);
                    }
                    if(GetComponent<PackController>().pack.PackSize() == 1)
                    {
                        //join other pack, to not be alone
                        SendPackJoinRequest(otherPack);
                    }

                    //if (Random.RandomRange(0, 100) < packChangePossibility)
                    //{
                    //    //changing wihtout reason
                    //}
                }
            }
        }
    }

    public bool SendPackJoinRequest(Pack otherPack, bool forceAccept = false)
    {
        var accepted = otherPack.JoinRequest(this.gameObject, forceAccept);
        if (accepted)
        {
            if(pack != null) pack.RemoveMemember(this.gameObject); // remove from old if have one
            pack = otherPack; // setting self pack refrence to new pack
            if (pack.leader == gameObject) imLeader = true;
            else imLeader = false;
            SetPackStatus(pack); // updating status canvas
        } else
        {
            CreateNewPack();
        }
        return accepted;
    }

    //public void CreatingPackRequest(GameObject candidate)
    //{
    //    if(pack == null && PackRequestLogic())
    //    {
    //        Pack p = new Pack()
    //    }
    //}

    //private bool PackRequestLogic()
    //{
    //    return true; //TODO: advance packRequestLogic
    //}
}
