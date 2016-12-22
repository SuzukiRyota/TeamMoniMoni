using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class walk_eat : MonoBehaviour
{
    [SerializeField]
    Image white_image;

    private int walk_chenge;
    public bool start_judge;

    private Image image_eat;
    [SerializeField]
    GameObject slider;
    Slide slider_ani;
    //float end_size = 0.08f;
    private Vector3 animation_end_pos = new Vector3(0.1f, -1.4f, -5);
    //private Vector3 animation_eat_end_pos = new Vector3(2.5f, -0.16f, -6);
    private Vector3 animation_pos;
    private Vector3 animation_walk;
    //private Vector3 animation_end_walk;
    private float size;
    private float animation_time;
    public Sprite eat_1;
    public Sprite eat_2;
    public Sprite eat_3;
    public Sprite eat_4;
    
    // Use this for initialization
    void Start()
    {
        walk_chenge = 0;
        slider_ani = slider.GetComponent<Slide>();
        size = 0.0100001f;
        animation_pos = transform.localPosition;
        animation_walk = new Vector3(
            animation_pos.x - animation_end_pos.x,
            animation_pos.y - animation_end_pos.y,
            0);
        image_eat = GetComponent<Image>();
        
        //if (eater == false)
        //{
        //    size = 0.01f;
        //    animation_pos = transform.localPosition;
        //    animation_end_walk = new Vector3(
        //   animation_pos.x - animation_eat_end_pos.x,
        //   animation_pos.y - animation_eat_end_pos.y,
        //   0);
        //}

    }
    //x 9.0引くy 1.5引く
    //
    // Update is called once per frame
    void Update()
    {
        if (start_judge == true)
        {

            animation_time++;
            transform.localScale = new Vector3(size, size, 1);
            transform.localPosition = animation_pos;
            //
            //0.01    
            if (animation_time <= 100)
            {
                animation_pos -= animation_walk / 500;
                size += 0.00002f;
                //0.002
            }//0.012
            if (animation_time > 100 && animation_time <= 200)
            {
                animation_pos -= animation_walk * 3 / 1000;
                size += 0.00003f;
                //0.003
            }//0.015
            if (animation_time > 200 && animation_time <= 300)
            {
                animation_pos -= animation_walk / 400;
                size += 0.00005f;
                //0.005
            }//0.02
            if (animation_time > 300 && animation_time <= 400)
            {
                animation_pos -= animation_walk / 800;
                size += 0.0002f;
                //0.005
            }//0.03
            if (animation_time > 400 && animation_time <= 500)
            {
                animation_pos -= animation_walk / 800;
                size += 0.0004f;
                //0.005
            }//0.05
            if (animation_time > 500 && animation_time <= 600)
            {
                size += 0.0003f;
                var color = white_image.color;
                color.a = 0.0f + (animation_time - 500) / 100;
                color.r = 0.0f + (animation_time - 500) / 100;
                color.g = 0.0f + (animation_time - 500) / 100;
                //color.b = 0.0f + (animation_time - 500) / 100;
                white_image.color = color;
                //0.008
            }//0.04
             //x -4.9 y -1.8 size -0.01
            if (animation_time % 10 == 0)
            {
                switch (walk_chenge)
                {
                    case 0:
                        if (animation_time <= 185 || animation_time >= 210)
                        {
                            image_eat.sprite = eat_1;
                            walk_chenge = 1;
                        }
                        break;
                    case 1:
                        if (animation_time <= 185 || animation_time >= 210)
                        {

                            image_eat.sprite = eat_2;
                            walk_chenge = 0;
                        }
                        break;
                }
            }
            if (animation_time == 190)image_eat.sprite = eat_3;
            if (animation_time == 200)image_eat.sprite = eat_4;
            if (animation_time == 601)
            {
                slider_ani.SlideStart();
                Destroy(gameObject);
            }

        }
    }
    public void starter()
    {
        if (start_judge == false) start_judge = true;
    }
    //if (eater == false)
    //{
    //    if (animation_time <= 200)
    //    {
    //        animation_pos = new Vector3(
    //            -animation_end_walk.x * animation_time / 200,
    //            -animation_end_walk.y * animation_time / 200,
    //            animation_pos.z);
    //        size -= size / 200;
    //    }
    //    if (animation_time == 201) Destroy(gameObject);
    //}

}
