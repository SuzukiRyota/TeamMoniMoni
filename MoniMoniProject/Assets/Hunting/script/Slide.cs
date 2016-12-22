using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Slide : MonoBehaviour
{
    [SerializeField]
    float size_y_use;
    
    float end_time;
    [SerializeField]
    Image white_image;

    [SerializeField]
    private GameObject slide_text;
    Text text_slide;
    [SerializeField]
    private GameObject eat_end_text;
    Text text_eat_end;

    AudioSource mogu_sound;

    [SerializeField]
    private GameObject BGMer;
    AudioSource touch_sound;

    Vector3 text_eat_end_pos;
    
    public GameObject mogu;
    public int call_mogu;
    public int call_mogu_pos;

    public bool text_eat_end_look;
    float text_eat_end_move;

    private Vector3 pos;
    bool starter;
    //[SerializeField]
    //private Slider eat_slider;
    //private float slide_level;
    //private int eat_count;
    //// Use this for initialization
    private Image image_eat;
    [SerializeField]
    private int chew_number;
    public static Vector3 mousePosition;
    private bool eating_1;
    private bool eating_2;
    private bool eating_yet;
    private bool turning;
    public int eat_count;
    public int chew_count;

    public Sprite eat_1;
    public Sprite eat_2;
    //public Sprite eat_3;
    //public Sprite eat_4;
    public bool chew_halo;
    public int effect_time;

    private bool eat_end;

    bool start_anime;
    float start_time;
    Color color;
    void Start()
    {
        color = white_image.color;
        this.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        end_time = 0.0f;
        mogu_sound = GetComponent<AudioSource>();
        touch_sound = BGMer.GetComponent<AudioSource>();
        call_mogu_pos = 0;
        call_mogu = 5;
        text_slide = slide_text.GetComponent<Text>();
        text_eat_end = eat_end_text.GetComponent<Text>();
       
        text_eat_end_pos = new Vector3(0, 1, 0);
        text_eat_end_look = false;
        text_eat_end_move = 0.0f;
        
        starter = false;
        eat_end = false;
        mousePosition = Vector3.zero;
        eating_1 = false;
        eating_2 = false;
        eating_yet = false;
        turning = false;
        eat_count = 0;
        chew_count = 0; ;
        image_eat = GetComponent<Image>();
        //slide_level = eat_slider.value;
        //eat_count = 0;
        
        start_anime = false;
        start_time = 0;
    }

    // Update is called once per frame


    void Update()
    {
        if (starter == false)
        {
            if (start_anime == true)
            {
                start_time++;
                if (start_time < 100)
                {
                    color.a = 1.0f - start_time / 100;
                    color.r = 1.0f - start_time / 100;
                    color.g = 1.0f - start_time / 100;
                    //color.b = 1.0f - start_time / 100;
                    white_image.color = color;
                    
                }
                if (start_time == 100) starter = true;

            }
        }
        if (starter == true)
        {

            
            
            if (Input.GetMouseButtonUp(0))
            {
                eating_1 = false;
                eating_2 = false;
                eating_yet = false;
                turning = false;
                eat_count = 0;
                call_mogu = 5;
            }
           
            if (eat_end == false)
            {
                mousePosition = Input.mousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    if (mousePosition.y > size_y_use / 3 * 2 && mousePosition.y < size_y_use + 5.0f) eating_1 = true;
                    else if (mousePosition.y > -5.0f && mousePosition.y < size_y_use / 3) eating_2 = true;
                    else if (mousePosition.y >= size_y_use / 3 && mousePosition.y <= size_y_use / 3 * 2) eating_yet = true;
                    else print(mousePosition);                    
                }
                call_mogu++;
                if (eating_1 == true)
                {
                    if (mousePosition.y > size_y_use/3*2 && mousePosition.y < size_y_use+5.0f)
                    {
                        eat_count = 0;
                        if (turning == true)
                        {
                            call_mogu = 0;
                            chew_count++;
                            turning = false;
                        }
                    }
                    if (mousePosition.y > size_y_use/3 && mousePosition.y < size_y_use/3*2)
                    {
                        eat_count = 1;
                        if (turning == true)
                        {
                            eat_count = 3;
                        }
                    }
                    if (mousePosition.y > -5.0f && mousePosition.y < size_y_use/3)
                    {
                        eat_count = 2;
                        if (turning == false)
                        {
                            call_mogu = 0;
                            turning = true;
                        }
                    }

                }
                if (eating_2 == true){
                    if (mousePosition.y > -5.0f && mousePosition.y < size_y_use/3){
                        eat_count = 0;
                        if (turning == true){
                            call_mogu = 0;
                            chew_count++;
                            turning = false;
                        }
                    }
                    if (mousePosition.y > size_y_use/3 && mousePosition.y < size_y_use/3*2){
                        eat_count = 1;
                        if (turning == true)eat_count = 3;
                    }
                    if (mousePosition.y > size_y_use/3*2 && mousePosition.y < size_y_use+5.0f){
                        eat_count = 2;
                        if (turning == false){
                            call_mogu = 0;
                            turning = true;
                        }
                    }
                }

                if (eating_yet == true)
                {
                    if (mousePosition.y > size_y_use/3*2 && mousePosition.y < size_y_use+5.0f)
                    {
                        eating_1 = true;
                        eating_yet = false;
                    }
                    else if (mousePosition.y > -5.0f && mousePosition.y < size_y_use/3)
                    {
                        eating_2 = true;
                        eating_yet = false;
                    }
                    else { };
                }

                //switch (eat_count)
                //{
                //    case 0:
                //        if (eating_yet == true)
                //        image_eat.sprite = eat_2;
                //    else
                //        image_eat.sprite = eat_1;
                //    break;
                //    case 1:
                //        image_eat.sprite = eat_2;
                //    break;
                //    case 2:
                //        image_eat.sprite = eat_1;
                //    break;
                //    case 3:
                //        image_eat.sprite = eat_2;
                //    break;
                //}
                switch (eat_count)
                {
                    case 0:
                        if (turning == true) call_mogu = 0;
                        if (eating_yet == true)image_eat.sprite = eat_1;
                        else
                        image_eat.sprite = eat_1;
                        break;
                    case 1:
                        image_eat.sprite = eat_1;
                        break;
                    case 2:
                        image_eat.sprite = eat_2;
                        break;
                    case 3:
                        image_eat.sprite = eat_2;
                        break;
                }
            }
            if (call_mogu == 2) {
                mogu_sound.Play();
                switch (call_mogu_pos)
                {
                    case 0:
                        Instantiate(mogu, new Vector3(0 + Random.Range(-1.5f, 1.5f), 1.7f + Random.Range(-0.2f, 0.5f), 0), Quaternion.Euler(Vector3.zero));
                        call_mogu_pos = 1;
                        break;
                    case 1:
                        Instantiate(mogu, new Vector3(2.2f + Random.Range(-0.2f, 0.5f), 0 + Random.Range(-1.5f, 1.5f), 0), Quaternion.Euler(Vector3.zero));
                        call_mogu_pos = 2;
                        break;
                    case 2:
                        Instantiate(mogu, new Vector3(0 + Random.Range(-1.5f, 1.5f), -1.5f + Random.Range(-0.5f, 0.5f), 0), Quaternion.Euler(Vector3.zero));
                        call_mogu_pos = 3;
                        break;
                    case 3:
                        Instantiate(mogu, new Vector3(-2.4f + Random.Range(-0.5f, 0.2f), 0 + Random.Range(-1.5f, 1.5f), 0), Quaternion.Euler(Vector3.zero));
                        call_mogu_pos = 0;
                        break;

                }
                
            }
            if (eat_end == true)
            {
                image_eat.sprite = eat_1;
                text_eat_end.transform.localPosition = text_eat_end_pos;
                if (text_eat_end_look == false)
                {
                    if (text_eat_end_pos.y > -4) text_eat_end_pos.y -= 0.05f;
                    if (text_eat_end_pos.y > -3) text_eat_end_pos.y -= 0.02f;
                    if (text_eat_end_pos.y > -2) text_eat_end_pos.y -= 0.015f;
                    if (text_eat_end_pos.y > -1) text_eat_end_pos.y -= 0.015f;
                    if (text_eat_end_pos.y > 0) text_eat_end_pos.y -= 0.01f;
                    if (text_eat_end_pos.y <= -4) text_eat_end_look = true;
                }
                else
                {
                    text_eat_end_move += 0.10f;
                    text_eat_end_pos.y = -4 + Mathf.Sin(-text_eat_end_move) / 4;
                    end_time++;
                    
                    color.b = 0.0f;
                    color.r = 0.0f;
                    color.g = 0.0f;
                    if (end_time >= 100) {
                        
                        color.a = 0.0f + (end_time - 100.0f) / 100;
                        white_image.color = color;
                    }
                    if(text_eat_end_move>=20.0f)SceneManager.LoadScene("Ending");
                }
            }
            if (chew_count == chew_number)
            {
                chew_count = 0;
                eat_end = true;
                print("咀嚼完了");
                text_slide.fontSize = 40;
                text_slide.text = " ";
                text_eat_end.text = "ごちそうさまでした！";
                touch_sound.Stop();

            }


            //    if (eat_slider.value != slide_level)
            //    {
            //        slide_level = eat_slider.value;
            //        if (eat_slider.value == 0)
            //        {



            //        }
            //        if (eat_slider.value == 1)
            //        {



            //        }
            //        if (eat_slider.value == 3)
            //        {



            //        }
            //        if (eat_slider.value == 5)
            //        {



            //        }
            //        if (eat_slider.value == 7)
            //        {



            //        }
            //        if (eat_slider.value == 8)
            //        {



            //        }
            //    }
        }
    }
    public void SlideStart()
    {
        start_anime = true;
        text_slide.text = "し縦てに食スべラてイねド";
        this.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
