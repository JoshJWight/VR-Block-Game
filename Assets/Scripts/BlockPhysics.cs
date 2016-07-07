using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockPhysics : MonoBehaviour {

    public Explosion explosion;

    public bool isAnchor = false;

    float speedThreshold = 10;
    float anchorForce = 100;
    float glueForce = 50;
    float glueThresholdV = .6f;
    float glueThresholdH = .22f;
    bool hasGoneFast = false;

    HashSet<BlockPhysics> neighbors = new HashSet<BlockPhysics>();

    // Use this for initialization
    void Start()
    {
    }

    public void addNeighbor(BlockPhysics b)
    {
        if(b)
        {
            neighbors.Add(b);
        }
        
    }



    // Update is called once per frame
    void Update()
    {
        if(isAnchor && !hasGoneFast)
        {
            GetComponent<Rigidbody>().AddForce(anchorForce * Vector3.down);
        }
        foreach(BlockPhysics b in neighbors)
        {
            if(b)
            {
                Vector3 v = b.transform.position - transform.position;
                float vMag = v.y;
                float hMag = Mathf.Sqrt(v.x * v.x + (v.z * v.z));
                if (vMag < glueThresholdV && hMag < glueThresholdH)
                {
                    v.Normalize();
                    GetComponent<Rigidbody>().AddForce(glueForce * v);
                }
            }
            
            
        }

        if (GetComponent<Rigidbody>().velocity.magnitude > speedThreshold)
        {
            hasGoneFast = true;
        }
        if (this.transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<Explosion>())
        {
            Destroy(this.gameObject);
        }
    }
    
}
