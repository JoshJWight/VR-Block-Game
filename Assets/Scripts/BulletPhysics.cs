using UnityEngine;
using System.Collections;

public class BulletPhysics : MonoBehaviour {

    public Explosion explosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        Explosion e = Instantiate(explosion);
        e.transform.position = gameObject.transform.position;
        Destroy(this.gameObject);
    }
}
