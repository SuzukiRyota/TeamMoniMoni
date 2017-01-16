using UnityEngine;
using System.Collections;

public class black_mist : MonoBehaviour {
  
    private int can_not_look_time;
    private float move;
    private float alpha;
    private bool mist_on;
    private Vector3 pos;
    private Vector3 move_pos;

    
    // Use this for initialization

    void Start () {
        can_not_look_time = 0;
        move = 1.0f;
        alpha = 0.00f;
        mist_on = false;
        pos = transform.localPosition;
        move_pos = transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        can_not_look_time++;
        if (mist_on == true && alpha <= 0.75f)
        {
            alpha += 0.025f;
            this.GetComponent<CanvasGroup>().alpha = alpha;
            if (move > 0.00f)
            {
                move -= 0.04f;
                move_pos.y = pos.y * move;
                move_pos.x = pos.x * move;
                transform.localPosition = move_pos;
            }
            if (move < 0.00f)
            {
                move = 0.00f;
                move_pos.y = pos.y * move;
                move_pos.x = pos.x * move;
                transform.localPosition = move_pos;
            }
        }
    }
    public void OnClick() {
        if(can_not_look_time>=60) mist_on = true;

    }
    public void OnClick_2()
    {
        can_not_look_time = 0;
        
    }
}
