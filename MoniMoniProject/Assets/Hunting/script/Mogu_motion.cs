using UnityEngine;
using System.Collections;

public class Mogu_motion : MonoBehaviour {

    float mogu_motion_time;
   
    // Use this for initialization
    void Start () {
        transform.localScale = Vector3.zero;
       
        transform.localPosition = new Vector3(Random.Range(-0.5f,0.5f), Random.Range(-0.3f, 0.5f),0);
    }
	
	// Update is called once per frame
	void Update () {
        mogu_motion_time++;
        transform.localScale =
        //new Vector3(1.0f + Mathf.Sin(mogu_motion_time / 45) / 3,
        //            1.0f + Mathf.Sin(mogu_motion_time / 15) / 3,
        //            0);
        new Vector3(0.003f + Mathf.Sin(mogu_motion_time / 45) / 1000,
                    0.003f + Mathf.Sin(mogu_motion_time / 15) / 1000,
                    0);
        if (mogu_motion_time > 90) Destroy(gameObject.transform.root.gameObject);

    }
}
