using UnityEngine;
using System.Collections;



public class alpha : MonoBehaviour {
    [SerializeField]
    private float changeAlpha;
    float changeRed = 1.0f;
    float changeGreen = 1.0f;
    float changeBlue = 1.0f;


    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<SpriteRenderer>().color = new Color(changeRed, changeGreen, changeBlue, changeAlpha);

    }
}
