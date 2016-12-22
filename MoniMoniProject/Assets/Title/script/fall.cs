using UnityEngine;
using System.Collections;

public class fall : MonoBehaviour {
    [SerializeField]
    private float fall_time;
    float fall_max;
    float fall_y;
    Vector3 pos;
    private AudioSource Title_BGM_Name;
    private int BGM_start;
    // Use this for initialization
    void Start()
    {
        Title_BGM_Name = GetComponent<AudioSource>();
        fall_y = 4.8f;
        pos = transform.localPosition;
        fall_max = fall_time;
    }

    // Update is called once per frame
    void Update()
    {
        if (fall_time > 0)
        {
            fall_time--;
         }
        fall_y = 4.8f * (fall_time/180);
        transform.localPosition = new Vector3(pos.x,pos.y + fall_y, pos.z);
        
        if (fall_time == 0) BGM_start++;
        if (BGM_start == 1)Title_BGM_Name.PlayOneShot(Title_BGM_Name.clip);
       
    }
}
