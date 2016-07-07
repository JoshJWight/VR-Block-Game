using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WandController : MonoBehaviour {

    public Canvas wandUI;
    private Canvas myUI;

    public SteamVR_PlayArea playArea;
    public GameObject teleportTarget;
    private GameObject myTarget;
    public GameObject bullet;
    public BlockPhysics skyscraperPiece;

    private float buildingDistanceFactor = 2;

    private float bulletVelocityFactor = 25f;

    private RobotArm arm;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();
    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    int currentGadget = 0;
    //0 = none
    //1 = teleport
    //2 = gun
    //3 = spawn building
    //TODO 4 = robot fist (i.e fist should not spawn buildings or shoot the gun)
    int nGadgets = 4;

    public void attachArm(RobotArm armObject)
    {
        arm = armObject;
    }

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        myTarget = Instantiate(teleportTarget);
        myTarget.GetComponent<MeshRenderer>().enabled = false;

        myUI = Instantiate(wandUI);


        Canvas getFucd = Instantiate(wandUI);
        getFucd.transform.position = new Vector3(10, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        myUI.transform.position = transform.position;
        myUI.transform.rotation = transform.rotation;

        if(controller!= null)
        {
            myTarget.transform.position = new Vector3(transform.position.x, 0, transform.position.z) + GetPointOffset();
        }
        

        //upkeep on current gadget
        switch (currentGadget)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;

        }

        if (controller.GetPressDown(triggerButton))
        {
            switch(currentGadget)
            {
                case 0:
                    GrabItem();
                    break;
                case 1:
                    Teleport();
                    break;
                case 2:
                    Shoot();
                    break;
                case 3:
                    SpawnSkyscraper();
                    break;
                default:
                    break;

            }
        }

        if (controller.GetPressUp(triggerButton))
        {
            switch (currentGadget)
            {
                case 0:
                    DropItem();
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;

            }
        }

        if (controller.GetPressDown(gripButton))
        {
            //clean up current gadget
            switch (currentGadget)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;

            }

            currentGadget = (currentGadget + 1) % nGadgets;

            //start up new gadget
            switch (currentGadget)
            {
                case 0:
                    SetUIText("Hand");
                    myTarget.GetComponent<MeshRenderer>().enabled = false;
                    break;
                case 1:
                    SetUIText("Teleport");
                    myTarget.GetComponent<MeshRenderer>().enabled = true;
                    break;
                case 2:
                    SetUIText("Gun");
                    myTarget.GetComponent<MeshRenderer>().enabled = false;
                    break;
                case 3:
                    SetUIText("Building");
                    myTarget.GetComponent<MeshRenderer>().enabled = true;
                    break;
                default:
                    break;

            }
        }
    }

    void SetUIText(string text)
    {
        Text t = myUI.GetComponentInChildren<Text>();
        t.text = text;
    }

    void GrabItem()
    {
        float minDistance = float.MaxValue;
        float distance;

        foreach (InteractableItem item in objectsHoveringOver)
        {
            if (item)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }

            
        }

        interactingItem = closestItem;
        if (interactingItem)
        {
            if (interactingItem.IsInteracting())
            {
                interactingItem.EndInteraction(this);
            }
            interactingItem.BeginInteraction(this);
        }
    }

    void DropItem()
    {
        if (interactingItem != null)
        {
            interactingItem.EndInteraction(this);
        }
    }

    void Teleport()
    {        
        playArea.transform.position = new Vector3(transform.position.x, 0, transform.position.z) + GetPointOffset();
    }

    Vector3 GetPointOffset()
    {

        Vector3 v = transform.rotation * Vector3.forward;
     
        if(v.x==0 && v.z==0)
        {
            return Vector3.zero;
        }
           
        float theta = Mathf.Atan(Mathf.Abs(v.y) / Mathf.Sqrt(v.x * v.x + v.z * v.z));//angle between the ground and v
        float d = playArea.transform.localScale.y / Mathf.Tan(theta); //horizontal distance to target
        return new Vector3(v.x, 0, v.z).normalized * d;
    }


    public void Shoot()
    {
        GameObject newBullet = Instantiate(bullet);
        Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
        //vector that points toward the front of the fist
        Vector3 v = this.transform.rotation * Vector3.forward;

        v.Normalize();

        newBullet.transform.position = this.transform.position + 1.5f * this.transform.localScale.y * v;
        newBullet.transform.rotation = this.transform.rotation;
        bulletRB.velocity = v * bulletVelocityFactor;
    }

    public void SpawnSkyscraper()
    {
        Vector3 position = transform.position + GetPointOffset() - new Vector3(skyscraperPiece.transform.localScale.x * 2.5f, 0, skyscraperPiece.transform.localScale.z * 2.5f);

        BlockPhysics[] pieces = new BlockPhysics[125];

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int z = 0; z < 5; z++)
                {
                    BlockPhysics piece = Instantiate(skyscraperPiece);
                    piece.transform.position = new Vector3(position.x + (x * skyscraperPiece.transform.localScale.x), ((y + 0.5f) * skyscraperPiece.transform.localScale.y), position.z + (z * skyscraperPiece.transform.localScale.z));

                    pieces[x * 25 + y * 5 + z] = piece;

                    if (x > 0)
                    {
                        BlockPhysics other = pieces[(x - 1) * 25 + y * 5 + z];
                        other.addNeighbor(piece);
                        piece.addNeighbor(other);
                    }
                    if (y > 0)
                    {
                        BlockPhysics other = pieces[x * 25 + (y - 1) * 5 + z];
                        other.addNeighbor(piece);
                        piece.addNeighbor(other);
                    }
                    else
                    {
                        piece.isAnchor = true;
                    }
                    if (z > 0)
                    {
                        BlockPhysics other = pieces[x * 25 + y * 5 + (z - 1)];
                        other.addNeighbor(piece);
                        piece.addNeighbor(other);
                    }
                }
            }
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
    }

}
