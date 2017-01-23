using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class enemy_move : MonoBehaviour
{
    [SerializeField]
    private bool before;



    public int backgound_number;
    public Sprite school_1;
    public Sprite school_2;
    public Sprite school_boss;
    public Sprite hospital_1;
    public Sprite hospital_2;
    public Sprite hospital_boss;


    public int Anime_one_number; //１回目に行う動きの番号
    public int Anime_two_number;//２回目に行う動きの番号　１回目と同じ番号が出たら振り直し
    public int Anime_three_number;//３回目に行う動きの番号　１，２回目と同じ番号が出たら振り直し
    public int Anime_number; //現在の動き
    public int Anime_motion_change;//軌道を変更する
    public int Anime_root_change;//１２で突然方向転換する
    public int time_can_touch;//クリックできる時間の制限


    [SerializeField]
    Image background;

    public float start_time;

    [SerializeField]
    Image white_image;
    [SerializeField]
    private GameObject walker;
    walk_eat walker_move;
    [SerializeField]
    private GameObject enemy_HP_text;
    Text HP_text;
    [SerializeField]
    private GameObject enemy_HP_text_slot;
    Text HP_text_slot;


    [SerializeField]
    private GameObject BGMer;
    AudioSource touch_sound;

    public int time;
    public int warp_time;
    private int touch_count;
    public bool enemy_eat;
    private Vector3 enemy_pos;
    float anime_pos_x;
    float anime_pos_y;

    private Vector3 enemy_end_pos;
    private Vector3 animation_eat_end_pos = new Vector3(3.0f, 0.5f, -6);
    private float size;
    float speed;
    float speed_regulation;

    [SerializeField]
    private Collider2D _collider2D = null;
    // Use this for initialization
    void Start()
    {
        time_can_touch = 0;
        Anime_motion_change = 0;
        if (before == true)
        {
            Anime_one_number = Random.Range(1, 13);
            Anime_two_number = Random.Range(1, 13);
            Anime_three_number = Random.Range(1, 13);
        }
        if (before == false)
        {
            Anime_one_number = Random.Range(1, 9);
            Anime_two_number = Random.Range(1, 9);
            Anime_three_number = Random.Range(1, 9);
        }
        switch (backgound_number)
        {
            case 1:
                background.sprite = school_1;
                break;
            case 2:
                background.sprite = school_2;
                break;
            case 3:
                background.sprite = school_boss;
                break;
            case 4:
                background.sprite = hospital_1;
                break;
            case 5:
                background.sprite = hospital_2;
                break;
            case 6:
                background.sprite = hospital_boss;
                break;
        }
        start_time = 0.0f;
        Anime_root_change = 0;
        touch_sound = BGMer.GetComponent<AudioSource>();
        walker_move = walker.GetComponent<walk_eat>();
        HP_text = enemy_HP_text.GetComponent<Text>();
        HP_text_slot = enemy_HP_text_slot.GetComponent<Text>();
        size = 1.0f;
        transform.localScale = new Vector3(1, 1, 1);
        time = 0;
        warp_time = 0;
        touch_count = 0;
        enemy_eat = false;
        enemy_pos = Vector3.zero;
        enemy_end_pos = Vector3.zero;
        speed = Random.Range(0.0f, 10.0f);
        speed_regulation = 3;
        touch_sound.Play();
        touch_sound.loop = true;
        anime_pos_x = Random.Range(-5.0f, 5.0f);
        anime_pos_y = Random.Range(-3.2f, 3.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (before == true)
        {
            if (Anime_two_number == Anime_one_number) Anime_two_number = Random.Range(1, 13);
            if (Anime_three_number == Anime_one_number || Anime_three_number == Anime_two_number) Anime_three_number = Random.Range(1, 13);
        }
        if (before == false)
        {
            if (Anime_two_number == Anime_one_number) Anime_two_number = Random.Range(1, 9);
            if (Anime_three_number == Anime_one_number || Anime_three_number == Anime_two_number) Anime_three_number = Random.Range(1, 9);
        }
        start_time++;
        if (start_time < 100)
        {
            transform.localPosition = new Vector3(30, 0, 0);
            var color = white_image.color;
            color.a = 1.0f - start_time / 100;
            white_image.color = color;
        }
        if (start_time == 100)
        {
            HP_text.text = "3";
            HP_text_slot.text = "あと    回敵をタップしてね";
            Anime_number = Anime_one_number;
            if (Anime_one_number >= 7) {
                anime_pos_x = -4.0f;
                anime_pos_y = 0;
                enemy_pos = new Vector3(-4.0f, 0, 0);
            }
        }
        if (start_time >= 100)
        {
            if (before == true)
            {
                switch (Anime_number)
                {//x=-5~5 y=-3.2~3.2 モーション切り替えすべし
                    case 1://上下cos移動右へ進み右端で左端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_x += 0.025f * speed_regulation;
                        if (anime_pos_x >= 5.0f)
                        {
                            speed = 0;
                            anime_pos_x = -5.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, Mathf.Cos(speed) * 1.8f, 0);
                        break;
                    case 2://上下cos移動左へ進み左端で右端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_x -= 0.025f * speed_regulation;
                        if (anime_pos_x <= -5.0f)
                        {
                            speed = 0;
                            anime_pos_x = 5.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, Mathf.Cos(-speed) * 1.8f, 0);
                        break;
                    case 3://上下cos移動、左右行ったり来たり
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Sin(-speed / 6) * 4.0f, Mathf.Cos(-speed) * 1.8f, 0);
                        break;
                    case 4://左右cos移動下へ進み下端で上端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_y += 0.015f * speed_regulation;
                        if (anime_pos_y >= 3.2f)
                        {
                            speed = 0;
                            anime_pos_y = -3.2f;
                        }
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 2, anime_pos_y, 0);
                        break;
                    case 5://左右cos移動上へ進み上端で下端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_y -= 0.015f * speed_regulation;
                        if (anime_pos_y <= -3.2f)
                        {
                            speed = 0;
                            anime_pos_y = 3.2f;
                        }
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 2, anime_pos_y, 0);
                        break;
                    case 6://左右cos移動、上下行ったり来たり
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 3, Mathf.Sin(-speed / 4) * 1.8f, 0);
                        break;
                    case 7://∞ 廃止　ナナメ移動
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Sin(speed) * 2.5f, Mathf.Sin(speed * 2) * 1.5f, 0);
                        break;
                    case 8://∞→大∞→∞→大∞→・・・
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        switch (Anime_motion_change)
                        {
                            case 0:
                                if (speed >= 6.2f)
                                {
                                    speed = 0.0f;
                                    Anime_motion_change = 1;
                                }
                                enemy_pos = new Vector3(Mathf.Sin(speed) * 2, Mathf.Sin(speed * 2) * 1.2f, 0);
                                break;
                            case 1:
                                if (speed >= 6.2f)
                                {
                                    speed = 0.0f;
                                    Anime_motion_change = 0;
                                }
                                enemy_pos = new Vector3(Mathf.Sin(speed) * 3.6f, Mathf.Sin(speed * 2) * 1.8f, 0);
                                break;
                        }
                        break;
                    case 9://8
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.025f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 2.5f, Mathf.Sin(speed) * 1.5f, 0);
                        break;
                    case 10://8→大8→8→大8→・・・
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.025f * speed_regulation;
                        switch (Anime_motion_change)
                        {
                            case 0:
                                if (speed >= 6.2f)
                                {
                                    speed = 0.0f;
                                    Anime_motion_change = 1;
                                }
                                enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 2, Mathf.Sin(speed) * 1.2f, 0);
                                break;
                            case 1:
                                if (speed >= 6.2f)
                                {
                                    speed = 0.0f;
                                    Anime_motion_change = 0;
                                }
                                enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 3.6f, Mathf.Sin(speed) * 1.8f, 0);
                                break;
                        }
                        break;
                    case 11://菱型
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 8.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 3.0f) speed_regulation -= 0.04f;
                        switch (Anime_motion_change)
                        {
                            case 0:
                                anime_pos_x += 0.032f * speed_regulation;
                                anime_pos_y += 0.02f * speed_regulation;
                                break;
                            case 1:
                                anime_pos_x += 0.032f * speed_regulation;
                                anime_pos_y -= 0.02f * speed_regulation;
                                break;
                            case 2:
                                anime_pos_x -= 0.032f * speed_regulation;
                                anime_pos_y -= 0.02f * speed_regulation;
                                break;
                            case 3:
                                anime_pos_x -= 0.032f * speed_regulation;
                                anime_pos_y += 0.02f * speed_regulation;
                                break;
                        }
                        if (anime_pos_y >= 2.5f)
                        {
                            Anime_motion_change = 1;
                            anime_pos_y = 2.5f;
                        }
                        if (anime_pos_x >= 4.0f)
                        {
                            Anime_motion_change = 2;
                            anime_pos_x = 4.0f;
                        }
                        if (anime_pos_y <= -2.5f)
                        {
                            Anime_motion_change = 3;
                            anime_pos_y = -2.5f;
                        }
                        if (anime_pos_x <= -4.0f)
                        {
                            Anime_motion_change = 0;
                            anime_pos_x = -4.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, anime_pos_y, 0);
                        break;
                    case 12://ランダム軌道菱型
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 8.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 3.0f) speed_regulation -= 0.04f;
                        switch (Anime_root_change)
                        {
                            case 0:
                                switch (Anime_motion_change)
                                {
                                    case 0:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                    case 1:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 2:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 3:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                }
                                break;
                            case 1:
                                switch (Anime_motion_change)
                                {
                                    case 0:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 1:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 2:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                    case 3:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                }
                                break;
                        }
                        if (anime_pos_y >= 2.5f)
                        {
                            Anime_motion_change = 1;
                            anime_pos_y = 2.5f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_x >= 4.0f)
                        {
                            Anime_motion_change = 2;
                            anime_pos_x = 4.0f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_y <= -2.5f)
                        {
                            Anime_motion_change = 3;
                            anime_pos_y = -2.5f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_x <= -4.0f)
                        {
                            Anime_motion_change = 0;
                            anime_pos_x = -4.0f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        enemy_pos = new Vector3(anime_pos_x, anime_pos_y, 0);
                        break;
                }
            }
            if (before == false)
            {
                switch (Anime_number)
                {//x=-5~5 y=-3.2~3.2 モーション切り替えすべし
                    case 1://上下cos移動右へ進み右端で左端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_x += 0.025f * speed_regulation;
                        if (anime_pos_x >= 5.0f)
                        {
                            speed = 0;
                            anime_pos_x = -5.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, Mathf.Cos(speed) * 1.8f, 0);
                        break;
                    case 2://上下cos移動左へ進み左端で右端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_x -= 0.025f * speed_regulation;
                        if (anime_pos_x <= -5.0f)
                        {
                            speed = 0;
                            anime_pos_x = 5.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, Mathf.Cos(-speed) * 1.8f, 0);
                        break;
                    case 3://上下cos移動、左右行ったり来たり
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Sin(-speed / 6) * 4.0f, Mathf.Cos(-speed) * 1.8f, 0);
                        break;
                    case 4://左右cos移動下へ進み下端で上端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_y += 0.015f * speed_regulation;
                        if (anime_pos_y >= 3.2f)
                        {
                            speed = 0;
                            anime_pos_y = -3.2f;
                        }
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 2, anime_pos_y, 0);
                        break;
                    case 5://左右cos移動上へ進み上端で下端へワープ
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        anime_pos_y -= 0.015f * speed_regulation;
                        if (anime_pos_y <= -3.2f)
                        {
                            speed = 0;
                            anime_pos_y = 3.2f;
                        }
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 2, anime_pos_y, 0);
                        break;
                    case 6://左右cos移動、上下行ったり来たり
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                        speed += 0.033f * speed_regulation;
                        enemy_pos = new Vector3(Mathf.Cos(-speed) * 3, Mathf.Sin(-speed / 4) * 1.8f, 0);
                        break;
                    //case 7://∞ 廃止　ナナメ移動
                    //    transform.localPosition = enemy_pos;
                    //    if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                    //    if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                    //    speed += 0.033f * speed_regulation;
                    //    enemy_pos = new Vector3(Mathf.Sin(speed) * 2.5f, Mathf.Sin(speed * 2) * 1.5f, 0);
                    //    break;
                    //case 8://∞→大∞→∞→大∞→・・・
                    //    transform.localPosition = enemy_pos;
                    //    if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                    //    if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                    //    speed += 0.033f * speed_regulation;
                    //    switch (Anime_motion_change)
                    //    {
                    //        case 0:
                    //            if (speed >= 6.2f)
                    //            {
                    //                speed = 0.0f;
                    //                Anime_motion_change = 1;
                    //            }
                    //            enemy_pos = new Vector3(Mathf.Sin(speed) * 2, Mathf.Sin(speed * 2) * 1.2f, 0);
                    //            break;
                    //        case 1:
                    //            if (speed >= 6.2f)
                    //            {
                    //                speed = 0.0f;
                    //                Anime_motion_change = 0;
                    //            }
                    //            enemy_pos = new Vector3(Mathf.Sin(speed) * 3.6f, Mathf.Sin(speed * 2) * 1.8f, 0);
                    //            break;
                    //    }
                    //    break;
                    //case 9://8
                    //    transform.localPosition = enemy_pos;
                    //    if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                    //    if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                    //    speed += 0.025f * speed_regulation;
                    //    enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 2.5f, Mathf.Sin(speed) * 1.5f, 0);
                    //    break;
                    //case 10://8→大8→8→大8→・・・
                    //    transform.localPosition = enemy_pos;
                    //    if (time < 150 && speed_regulation < 4.0f) speed_regulation += 0.04f;
                    //    if (time > 150 && speed_regulation > 1.0f) speed_regulation -= 0.04f;
                    //    speed += 0.025f * speed_regulation;
                    //    switch (Anime_motion_change)
                    //    {
                    //        case 0:
                    //            if (speed >= 6.2f)
                    //            {
                    //                speed = 0.0f;
                    //                Anime_motion_change = 1;
                    //            }
                    //            enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 2, Mathf.Sin(speed) * 1.2f, 0);
                    //            break;
                    //        case 1:
                    //            if (speed >= 6.2f)
                    //            {
                    //                speed = 0.0f;
                    //                Anime_motion_change = 0;
                    //            }
                    //            enemy_pos = new Vector3(Mathf.Sin(speed * 2) * 3.6f, Mathf.Sin(speed) * 1.8f, 0);
                    //            break;
                    //    }
                    //    break;
                    case 7://菱型
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 8.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 3.0f) speed_regulation -= 0.04f;
                        switch (Anime_motion_change)
                        {
                            case 0:
                                anime_pos_x += 0.032f * speed_regulation;
                                anime_pos_y += 0.02f * speed_regulation;
                                break;
                            case 1:
                                anime_pos_x += 0.032f * speed_regulation;
                                anime_pos_y -= 0.02f * speed_regulation;
                                break;
                            case 2:
                                anime_pos_x -= 0.032f * speed_regulation;
                                anime_pos_y -= 0.02f * speed_regulation;
                                break;
                            case 3:
                                anime_pos_x -= 0.032f * speed_regulation;
                                anime_pos_y += 0.02f * speed_regulation;
                                break;
                        }
                        if (anime_pos_y >= 2.5f)
                        {
                            Anime_motion_change = 1;
                            anime_pos_y = 2.5f;
                        }
                        if (anime_pos_x >= 4.0f)
                        {
                            Anime_motion_change = 2;
                            anime_pos_x = 4.0f;
                        }
                        if (anime_pos_y <= -2.5f)
                        {
                            Anime_motion_change = 3;
                            anime_pos_y = -2.5f;
                        }
                        if (anime_pos_x <= -4.0f)
                        {
                            Anime_motion_change = 0;
                            anime_pos_x = -4.0f;
                        }
                        enemy_pos = new Vector3(anime_pos_x, anime_pos_y, 0);
                        break;
                    case 8://ランダム軌道菱型
                        transform.localPosition = enemy_pos;
                        if (time < 150 && speed_regulation < 8.0f) speed_regulation += 0.04f;
                        if (time > 150 && speed_regulation > 3.0f) speed_regulation -= 0.04f;
                        switch (Anime_root_change)
                        {
                            case 0:
                                switch (Anime_motion_change)
                                {
                                    case 0:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                    case 1:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 2:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 3:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                }
                                break;
                            case 1:
                                switch (Anime_motion_change)
                                {
                                    case 0:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 1:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y -= 0.02f * speed_regulation;
                                        break;
                                    case 2:
                                        anime_pos_x -= 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                    case 3:
                                        anime_pos_x += 0.032f * speed_regulation;
                                        anime_pos_y += 0.02f * speed_regulation;
                                        break;
                                }
                                break;
                        }
                        if (anime_pos_y >= 2.5f)
                        {
                            Anime_motion_change = 1;
                            anime_pos_y = 2.5f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_x >= 4.0f)
                        {
                            Anime_motion_change = 2;
                            anime_pos_x = 4.0f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_y <= -2.5f)
                        {
                            Anime_motion_change = 3;
                            anime_pos_y = -2.5f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        if (anime_pos_x <= -4.0f)
                        {
                            Anime_motion_change = 0;
                            anime_pos_x = -4.0f;
                            Anime_root_change = Random.Range(0, 2);
                        }
                        enemy_pos = new Vector3(anime_pos_x, anime_pos_y, 0);
                        break;
                }
            }
            if (time >= 350) time = 0;
            time_can_touch++;
            time++;
            switch (touch_count)
            {
                //case 0:
                //transform.localPosition = enemy_pos;
                //if (time < 150&&speed_regulation < 3.0f) speed_regulation += 0.04f;
                //if (time >= 150&&speed_regulation >= 1.0f)speed_regulation -= 0.04f;
                //speed += 0.05f * speed_regulation;
                //enemy_pos = new Vector3(Mathf.Sin(-speed) * 2.0f, Mathf.Cos(-speed) * 1.5f, 0);
                //break;
                //case 1:
                //    transform.localPosition = enemy_pos;
                //    if (time < 150 && speed_regulation < 5.0f) speed_regulation += 0.04f;
                //    if (time >= 150 && speed_regulation >= 1.0f) speed_regulation -= 0.04f;
                //    speed += 0.03f * speed_regulation;
                //    enemy_pos = new Vector3(Mathf.Sin(-speed / 4) * 2, Mathf.Cos(-speed) * 1.8f, 0);
                //    break;
                //case 2:
                //    transform.localPosition = enemy_pos;
                //    if (time < 200) warp_time += 6;
                //    if (time >= 200 && time < 500) warp_time++;
                //    if (time >= 500 && time % 2 == 0) warp_time++;
                //    speed += 0.03f * speed_regulation;
                //    if (warp_time >= 40)
                //    {
                //        enemy_pos = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(-2.2f, 2.2f), 0);
                //        warp_time = 0;
                //    }
                //    break;
                case 3:
                    if (time <= 100)
                    {
                        transform.localPosition = enemy_end_pos - enemy_end_pos / 100 * time;
                    }
                    if (time == 100) { touch_count = 4; time = 0; walker_move.starter(); }
                    break;
                case 4:
                    if (time <= 202)
                    {
                        transform.localScale = new Vector3(size, size, 1);
                        transform.localPosition = new Vector3(
                            0 + animation_eat_end_pos.x / 202 * time,
                            0 + animation_eat_end_pos.y / 202 * time,
                            -6);
                        size -= 0.8f / 202;
                    }
                    if (time == 203) Destroy(gameObject);
                    break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (touch_count < 3)
                {
                    time = 0;
                    touch_count = 3;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider = Physics2D.OverlapPoint(tapPoint);

                if (collider == _collider2D)
                {
                    if (time_can_touch > 200)
                    {
                         switch (touch_count)
                        {
                            case 0:
                                Anime_motion_change = 0;
                                touch_count++;
                                time_can_touch = 0;
                                time = 0;
                                speed_regulation = 5;
                                Anime_number = Anime_two_number;
                                HP_text.text = "2";
                                if (Anime_two_number >= 7)
                                {
                                    anime_pos_x = -4.0f;
                                    anime_pos_y = 0;
                                    enemy_pos = new Vector3(-4.0f, 0, 0);
                                }
                                break;
                            case 1:
                                Anime_motion_change = 0;
                                touch_count++;
                                time_can_touch = 0;
                                time = 0;
                                speed_regulation = 5;
                                Anime_number = Anime_three_number;
                                HP_text.text = "1";
                                if (Anime_three_number >= 7)
                                {
                                    anime_pos_x = -4.0f;
                                    anime_pos_y = 0;
                                    enemy_pos = new Vector3(-4.0f, 0, 0);
                                }
                                break;
                            case 2:
                                enemy_end_pos = transform.localPosition;
                                enemy_eat = true;
                                touch_count++;

                                time = 0;
                                HP_text.text = " ";
                                HP_text_slot.text = " ";
                                break;
                        }

                    }

                    //Debug.Log(cout);
                }
            }
        }
    }

}
