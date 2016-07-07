using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    float growthRate = 1.05f;
    float duration = 0.5f;
    float power = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale *= growthRate;
        duration -= Time.deltaTime;
        if(duration <= 0)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision col)
    {
        //This code makes explosions push things, but that seems bad now
        /*if(col.rigidbody)
        {
            Vector3 v = col.gameObject.transform.position - gameObject.transform.position;
            v.Normalize();
            col.rigidbody.AddForce(v * power);
        }*/
    }
}
