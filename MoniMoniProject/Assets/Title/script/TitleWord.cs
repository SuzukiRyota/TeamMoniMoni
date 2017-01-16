using UnityEngine;
using System.Collections;

public class TitleWord : MonoBehaviour {
    public Sprite school_1;




    [SerializeField]
    private int wait_time;
    Vector2 pos;
    private AudioSource Title_BGM_TAP;
    private int BGM_start;
    // Use this for initialization
    void Start () {
    pos = transform.localPosition;
        BGM_start = 0;
        Title_BGM_TAP = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (wait_time > 0)
        {
            wait_time--;
            transform.localPosition = new Vector3(pos.x, pos.y + 150, 0);
        }
        
        if (wait_time < 0) { wait_time = 0; }
        if (wait_time == 0) {
            BGM_start++;
            transform.localPosition = new Vector3(pos.x, pos.y, 0);
        }
        if (BGM_start == 1) Title_BGM_TAP.PlayOneShot(Title_BGM_TAP.clip);
    }
}
