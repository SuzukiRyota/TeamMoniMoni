using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// イベントたちを作るクラス
/// イベントの作り方
/// 各関数ごとにintの返り値がある
/// return 0    イベント続行中
///        1    イベントを終了させるかつ、呼び出されるたびに同じイベントを繰り返す
///        2    イベントを終了させるかつ、イベントの段階を進める
/// </summary>
public class EventsCreate : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    PlayerController playercontroller;

    [SerializeField]
    GameObject mapcontroller;

    [SerializeField]
    MapChipController chipcontroller;

    [SerializeField]
    EventTalkManager talkmanager;
    [SerializeField]
    EventLoader eventloader;
    [SerializeField]
    EventRepository eventrepository;

    [SerializeField]
    StagingController staging;

    [SerializeField]
    GameObject stagemoveeventcanvas;

    [SerializeField]
    EnemyManager enemymanager;
    [SerializeField]
    TrapController trapcontroller;

    public void stateSave()
    {
        SceneInfoManager.instance.player_pos = player.transform.position;
        SceneInfoManager.instance.select_map_name = chipcontroller.select_map_name;
        SceneInfoManager.instance.select_stage_name = chipcontroller.select_stage_name;
        SceneInfoManager.instance.enemy_num = enemymanager.enemy_num;
        if (SceneInfoManager.instance.enemy_num > 0)
            SceneInfoManager.instance.enemy_num--;
    }

    public void returnHouse()
    {
        SceneInfoManager.instance.player_pos = new Vector3();
        SceneInfoManager.instance.select_map_name = "House1F";
        SceneInfoManager.instance.select_stage_name = "Videl";
        SceneInfoManager.instance.enemy_num = 0;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  セットアップ                                                                                                                      //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        schoolBlackOutEventSetup();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  アップデート                                                                                                                      //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (is_roommove)
            if (staging.fadeInBlack())
            {
                is_roommove = false;
            }
        if (Input.GetKeyDown(KeyCode.C))
        {
            eventrepository.houseEndEventSetup();
            eventloader.eventRegister();
        }
        schoolBlackOutEvent();
    }

    int upcount = 0;
    // イベントが正常に動くかどうかのお試し関数２つ
    public int playerUpEvent()
    {
        player.transform.Translate(0, chipcontroller.chip_size, 0);
        upcount++;
        if (upcount < 1)
            return 1;
        return 2;
    }
    public int playerDirectionEvent()
    {
        if (playercontroller.player_direction == PlayerController.PlayerDirection.UP)
        {
            player.transform.Translate(0, 1.5f, 0);
        }
        if (playercontroller.player_direction == PlayerController.PlayerDirection.DOWN)
        {
            player.transform.Translate(0, -1.5f, 0);
        }
        upcount++;
        if (upcount < 1)
            return 1;
        return 1;
    }

    // 実装できてないイベントに入れる
    public int emptyEvent()
    {
        return 2;
    }
    public int overrapEmptyEvent()
    {
        playercontroller.state = PlayerController.State.NORMAL;
        return 1;
    }

    // 手が出てくるイベントを作ったけど、使ってないs
    float player_z = -0.5f;
    int zombiecount = 0;
    Vector2 player_start_pos;
    public int playerZombieHand2()
    {
        if (is_setup)
        {
            talkmanager.startTalk("testevent");
            is_setup = false;
        }

        if (talkmanager.is_talknow)
            return 0;
        zombiecount = 0;
        return 1;
    }


    bool is_setup = false;

    /// <summary>
    /// 会話だけの時に使用する関数
    /// </summary>
    /// <returns></returns>
    int talkEvent(string talkname_)
    {
        if (is_setup == false)
        {
            talkmanager.startTalk(talkname_);
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }

    /// <summary>
    /// 会話だけの時かつ、次のイベントに進む関数
    /// </summary>
    /// <param name="talkname_"></param>
    /// <returns></returns>
    int talkNextEvent(string talkname_)
    {
        if (is_setup == false)
        {
            talkmanager.startTalk(talkname_);
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 2;
    }


    /// イベントの作り方
    /// 各関数ごとにintの返り値がある
    /// return 0    イベント続行中
    ///        1    イベントを終了させるかつ、呼び出されるたびに同じイベントを繰り返す
    ///        2    イベントを終了させるかつ、イベントの段階を進める
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  学校イベント                                                                                                                      //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // 停電イベントが起きたかどうか
    bool is_blackoutevent = false;
    bool is_blackout;
    int blackoutstartcount;
    int blackoutcount;

    [SerializeField]
    Image blackout_background;

    void schoolBlackOutEventSetup()
    {
        blackoutstartcount = 4 * 60 * 60;
        blackoutcount = 0;
        is_blackout = false;
    }
    void schoolBlackOutEvent()
    {
        if (is_blackoutevent) return;

        if (chipcontroller.select_stage_name == "School" &&
            playercontroller.state == PlayerController.State.NORMAL)
        {
            blackoutcount++;
            if (blackoutcount > blackoutstartcount &&
                is_setup == false)
            {
                talkmanager.startTalk("school_10");
                blackout_background.gameObject.SetActive(true);
                is_blackout = true;
                is_setup = true;
            }
        }
        if (is_blackout)
        {
            if (talkmanager.talkmode == EventTalkManager.TalkMode.EVENT &&
                talkmanager.event_call_count == 1)
            {
                SoundManager.Instance.volume.SE = 0.5f;
                SoundManager.Instance.PlaySE(0);
                talkmanager.talkmode = EventTalkManager.TalkMode.NORMAL;
                return;
            }
            if (talkmanager.talkmode == EventTalkManager.TalkMode.EVENT &&
                talkmanager.event_call_count == 2)
            {
                SoundManager.Instance.PlaySE(0);

                blackout_background.gameObject.SetActive(false);
                talkmanager.talkmode = EventTalkManager.TalkMode.NORMAL;
            }
        }
        else
            return;
        playercontroller.state = PlayerController.State.TALK;
        if (talkmanager.is_talknow)
            return;
        playercontroller.state = PlayerController.State.NORMAL;
        is_setup = false;
        is_blackoutevent = true;
    }


    bool is_flush = false;
    public int schoolEvent01()
    {
        if (is_flush == false)
        {
            if (is_setup == false)
            {
                stagemoveeventcanvas.SetActive(true);
                talkmanager.startTalk("school_scenario");
                is_setup = true;
            }
            playercontroller.state = PlayerController.State.TALK;
        }
        if (talkmanager.is_talknow)
            return 0;
        if (is_flush == false)
        {
            if (staging.flushStart())
            {
                is_flush = true;
                is_setup = false;
                stagemoveeventcanvas.SetActive(false);
            }
            return 0;
        }
        else
        {
            if (staging.flushEnd())
            {
                return 2;
            }
            return 0;
        }
    }

    public int schoolEvent01_2()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_01");
            is_setup = true;
        }
        playercontroller.state = PlayerController.State.TALK;
        if (talkmanager.is_talknow)
            return 0;
        playercontroller.state = PlayerController.State.NORMAL;
        is_setup = false;
        return 2;
    }

    public int schoolEvent02()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_02");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        if (talkmanager.selectbuttonnum == 1)
            playercontroller.setItem("Boarderaser");
        is_setup = false;
        return 1;
    }
    public int schoolEvent03()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_03");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        if (talkmanager.selectbuttonnum == 1)
        {
            if (playercontroller.have_item_name == "Boarderaser")
            {
                trapcontroller.eventPutTrap("Boarderaser", 80, chipcontroller.eventselect_cell_x, chipcontroller.eventselect_cell_y);
            }
        }
        is_setup = false;
        return 1;
    }
    public int schoolEvent04()
    {
        if (is_setup == false)
        {
            player_start_pos = player.transform.position;

            is_setup = true;
        }
        zombiecount++;
        if (zombiecount / 60.0f <= 2)
        {
            player.transform.position = player_start_pos;
            Vector3 randompos = Random.insideUnitCircle * 0.05f;
            player.transform.Translate(randompos + new Vector3(0, 0, player_z));
            return 0;
        }
        else if (zombiecount == 60 * 2 + 1)
        {
            player.transform.Translate(0, 1, 0);
            talkmanager.startTalk("school_04");
            playercontroller.state = PlayerController.State.TALK;
        }

        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent05()
    {
        return talkNextEvent("school_05");
    }
    public int schoolEvent05_3()
    {
        return talkEvent("school_05-3");
    }
    public int schoolEvent06()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_06");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        if (talkmanager.selectbuttonnum == 1)
            playercontroller.setItem("Easel");
        is_setup = false;
        return 1;
    }
    public int schoolEvent06_2()//---------------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_06-1");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent07()
    {
        return talkNextEvent("school_07");
    }
    public int schoolEvent07_2()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_07-2");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent08()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_08");
            is_setup = true;
        }
        if (talkmanager.talkmode == EventTalkManager.TalkMode.EVENT &&
                talkmanager.event_call_count == 1)
        {
            SoundManager.Instance.volume.SE = 0.5f;
            SoundManager.Instance.PlaySE(2);
            talkmanager.talkmode = EventTalkManager.TalkMode.NORMAL;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 2;
    }
    public int schoolEvent08_2()
    {
        return talkEvent("school_08-2");
    }
    public int schoolEvent09()//------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_09");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 2;
    }
    public int schoolEvent09_2()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_09-2");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent11()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_11");
            is_setup = true;
        }
        if (talkmanager.talkmode == EventTalkManager.TalkMode.EVENT &&
                talkmanager.event_call_count == 1)
        {
            SoundManager.Instance.volume.SE = 0.5f;
            SoundManager.Instance.PlaySE(1);
            talkmanager.talkmode = EventTalkManager.TalkMode.NORMAL;
        }
        if (talkmanager.talkmode == EventTalkManager.TalkMode.EVENT &&
                talkmanager.event_call_count == 2)
        {
            SoundManager.Instance.volume.SE = 0.5f;
            SoundManager.Instance.PlaySE(1);
            talkmanager.talkmode = EventTalkManager.TalkMode.NORMAL;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 2;
    }
    public int schoolEvent11_2()
    {
        return talkNextEvent("school_11-2");
    }
    public int schoolEvent11_3()
    {
        return talkEvent("school_11-3");
    }
    public int schoolEvent12()
    {
        return talkEvent("school_12");
    }
    public int schoolEvent13()
    {
        return talkEvent("school_13");
    }
    public int schoolEvent14()
    {
        return talkNextEvent("school_14");
    }
    public int schoolEvent14_2()
    {
        return talkNextEvent("school_14-2");
    }
    public int schoolEvent14_3()
    {
        return talkEvent("school_14-3");
    }
    public int schoolEvent15()//-------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_15");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent16()//----------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_16");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent17()
    {
        return talkEvent("school_17");
    }
    public int schoolEvent18()
    {
        return talkEvent("school_18");
    }//-------------------------------------------------------------------------------------------
    public int schoolEvent19()
    {
        return talkEvent("school_19");
    }
    public int schoolEvent20()
    {
        return talkNextEvent("school_20");
    }
    public int schoolEvent20_2()//------------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_20_2");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    public int schoolEvent21()
    {
        return talkEvent("school_21");
    }
    public int schoolEvent22()//----------------------------------------------------------------------------------------------
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("school_22");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        is_setup = false;
        return 1;
    }
    bool is_enemypop = false;
    public int schoolBossEvent()
    {
        if (is_enemypop == false)
        {
            if (staging.flushStart())
            {
                is_enemypop = true;
            }
            return 0;
        }
        else
        {
            if (staging.flushEnd())
            {
                if (is_setup == false)
                {
                    talkmanager.startTalk("school_23");
                    is_setup = true;
                }
                if (talkmanager.is_talknow)
                    return 0;
                enemymanager.nightmareRePop(talkmanager.selectbuttonnum);
                is_setup = false;
                staging.flushcount = 0;
                return 1;
            }
            return 0;
        }
    }


    /// イベントの作り方
    /// 各関数ごとにintの返り値がある
    /// return 0    イベント続行中
    ///        1    イベントを終了させるかつ、呼び出されるたびに同じイベントを繰り返す
    ///        2    イベントを終了させるかつ、イベントの段階を進める
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  ヴィーデルの館イベント                                                                                                             //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //int floor_layer = (int)LayerController.Layer.FLOOR;
    //int wall_layer = (int)LayerController.Layer.WALL;
    //int door_layer = (int)LayerController.Layer.DOOR;
    //int object_layer = (int)LayerController.Layer.OBJECT;
    int event_layer = (int)LayerController.Layer.EVENT;



    /// <summary>
    /// プレイヤーを引数で渡したイベントのマスに移動させる関数
    /// 移動させるときに、周囲４マスを調べて移動イベントの上や、壁ドア物がないところに移動させる
    /// </summary>
    bool roomMoveToPlayer(int gotoevent_)
    {
        for (int y = 0; y < chipcontroller.chip_num_y; y++)
        {
            for (int x = 0; x < chipcontroller.chip_num_x; x++)
            {
                if (chipcontroller.blockcomponents[event_layer][y][x].number == gotoevent_)
                {
                    if (x + 1 < chipcontroller.chip_num_x - 1 &&
                        chipcontroller.blockcomponents[event_layer][y][x + 1].number != gotoevent_ &&
                        chipcontroller.isFloor(x + 1, y))
                    {
                        chipcontroller.playerSelectCellPop(x + 1, y);
                        return true;
                    }
                    if (x - 1 > 0 &&
                        chipcontroller.blockcomponents[event_layer][y][x - 1].number != gotoevent_ &&
                        chipcontroller.isFloor(x - 1, y))
                    {
                        chipcontroller.playerSelectCellPop(x - 1, y);
                        return true;
                    }
                    if (y + 1 < chipcontroller.chip_num_y - 1 &&
                        chipcontroller.blockcomponents[event_layer][y + 1][x].number != gotoevent_ &&
                        chipcontroller.isFloor(x, y + 1))
                    {
                        chipcontroller.playerSelectCellPop(x, y + 1);
                        return true;
                    }
                    if (y - 1 > 0 &&
                        chipcontroller.blockcomponents[event_layer][y - 1][x].number != gotoevent_ &&
                        chipcontroller.isFloor(x, y - 1))
                    {
                        chipcontroller.playerSelectCellPop(x, y - 1);
                        return true;
                    }
                }
            }

        }
        return false;
    }

    public int houseEvent02()//-------------------------------------------------------------------------------
    {
        return talkEvent("home_2");
    }

    bool is_roommove = false;

    // 入る場合は自分の数字
    // 出る場合は出る先の数字より１少ない数字
    int moveToRoom(string room_name_, int eventnum_, string loadTexture_ = "Videl")
    {
        if (is_roommove == false)
            if (staging.fadeOutBlack())
            {
                chipcontroller.mapChange(room_name_, loadTexture_);
                eventloader.eventRegister();
                roomMoveToPlayer(eventnum_);
                is_roommove = true;
                return 1;
            }
        return 0;
    }

    public int houseEvent03()
    {
        return moveToRoom("House1FLeft", 3);
    }
    public int houseEvent04()
    {
        return moveToRoom("House1F", 2);
    }
    public int houseEvent05()
    {
        return moveToRoom("House1FRight", 5);
    }
    public int houseEvent06()
    {
        return moveToRoom("House1F", 4);
    }
    public int houseEvent07()
    {
        return moveToRoom("House1FCenter", 7);
    }
    public int houseEvent08()
    {
        return moveToRoom("House1F", 6);
    }
    public int houseEvent09()
    {
        return moveToRoom("House2F", 10);
    }
    public int houseEvent10()
    {
        return moveToRoom("House2F", 11);
    }
    public int houseEvent11()
    {
        return moveToRoom("House1F", 8);
    }
    public int houseEvent12()
    {
        return moveToRoom("House1F", 9);
    }

    public int houseEvent13()
    {
        return talkEvent("home_13");
    }//-------------------------------------------------------------------------------------
    public int houseEvent14()
    {
        return talkEvent("home_14");
    }//-------------------------------------------------------------------------------------
    public int houseEvent15()
    {
        return talkEvent("home_15");
    }//-------------------------------------------------------------------------------------

    public int houseEvent16()
    {
        return moveToRoom("House2FLeftBook", 16);
    }
    public int houseEvent17()
    {
        return moveToRoom("House2F", 15);
    }
    public int houseEvent18()
    {
        return moveToRoom("House2FRightBook", 18);
    }
    public int houseEvent19()
    {
        return moveToRoom("House2F", 17);
    }

    //-------------------------------------------------------------------------------------
    public int houseEvent20()//-------------------------------------------------------------------------------------
    {
        return talkEvent("home_20");
    }

    public int houseEvent21()
    {
        if (is_setup == false)
        {
            talkmanager.startTalk("home_21");
            is_setup = true;
        }
        if (talkmanager.is_talknow)
            return 0;
        if (talkmanager.selectbuttonnum == 1)
        {
            if (is_roommove == false)
                if (staging.fadeOutBlack())
                {
                    eventrepository.schoolEventSetup();
                    chipcontroller.mapChange("school1", "School");
                    eventloader.eventRegister();
                    chipcontroller.playerPop();
                    enemymanager.enemySetup();
                    is_setup = false;
                    is_roommove = true;
                    return 1;
                }
            return 0;
        }
        is_setup = false;
        return 1;
    }
    public int houseEvent22()
    {
        return talkEvent("home_22");
    }
    public int houseEvent22_2()
    {
        return talkEvent("home_22-2");
    }
    public int houseEvent23()
    {
        return talkEvent("home_23");
    }
    public int houseEvent24()
    {
        return talkEvent("home_24");
    }
    public int houseEvent25()
    {
        return talkEvent("home_25");
    }
    public int houseEvent25_2()
    {
        return talkEvent("home_25-2");
    }
    public int houseEvent26()
    {
        return talkEvent("home_26");
    }
    public int houseEvent26_2()
    {
        return talkEvent("home_26-2");
    }
    public int houseEvent27()
    {
        return talkEvent("home_27");
    }
    public int houseEvent27_2()
    {
        return talkEvent("home_27-2");
    }
    public int houseEvent28()
    {
        return talkEvent("home_28");
    }
    public int houseEvent28_2()
    {
        return talkEvent("home_28-2");
    }
    public int houseEvent29()
    {
        return talkEvent("home_29");
    }
    public int houseEvent29_2()
    {
        return talkEvent("home_29-2");
    }
    public int houseEvent30()
    {
        return talkEvent("home_30");
    }
    public int houseEvent31()
    {
        return talkEvent("home_31");
    }
    public int houseEvent31_2()
    {
        return talkEvent("home_31-2");
    }
    public int houseEvent32()
    {
        return talkEvent("home_32");
    }
    public int houseEvent32_2()
    {
        return talkEvent("home_32-2");
    }
    public int houseEvent33()
    {
        return talkEvent("home_33");
    }
    public int houseEvent33_2()
    {
        return talkEvent("home_33-2");
    }
    public int houseEvent34()
    {
        return talkEvent("home_34");
    }
    public int houseEvent34_2()
    {
        return talkEvent("home_34-2");
    }
    public int houseEvent35()
    {
        return talkEvent("home_35");
    }
    public int houseEvent35_2()
    {
        return talkEvent("home_35-2");
    }
    public int houseEvent36()
    {
        return talkEvent("home_36");
    }
    public int houseEvent36_2()
    {
        return talkEvent("home_36-2");
    }
    public int houseEvent37()
    {
        return talkEvent("home_37");
    }
    public int houseEvent37_2()
    {
        return talkEvent("home_37-2");
    }
    public int houseEvent38()
    {
        return talkEvent("home_38");
    }
    public int houseEvent38_2()
    {
        return talkEvent("home_38-2");
    }
    public int houseEvent39()
    {
        return talkEvent("home_39");
    }
    public int houseEvent39_2()
    {
        return talkEvent("home_39-2");
    }
    public int houseEvent40()
    {
        return talkEvent("home_40");
    }
    public int houseEvent41()
    {
        return talkEvent("home_41");
    }
    public int houseEvent41_2()
    {
        return talkEvent("home_41-2");
    }
    public int houseEvent42()
    {
        return talkEvent("home_42");
    }
    public int houseEvent42_2()
    {
        return talkEvent("home_42-2");
    }
    public int houseEvent43()
    {
        return talkEvent("home_43");
    }
    public int houseEvent43_2()
    {
        return talkEvent("home_43-2");
    }
    public int houseEvent44()
    {
        return talkEvent("home_44");
    }
    public int houseEvent44_2()
    {
        return talkEvent("home_44-2");
    }
    public int houseEvent45()
    {
        return talkEvent("home_45");
    }
    public int houseEvent45_2()
    {
        return talkEvent("home_45-2");
    }
    public int houseEvent46()
    {
        return talkEvent("home_46");
    }
    public int houseEvent46_2()
    {
        return talkEvent("home_46-2");
    }
    public int houseEvent47()
    {
        return talkEvent("home_47");
    }
    public int houseEvent47_2()
    {
        return talkEvent("home_47-2");
    }
    public int houseEvent48()
    {
        return talkEvent("home_48");
    }
    public int houseEvent48_2()
    {
        return talkEvent("home_48-2");
    }


    public int houseEvent49()
    {
        int rand = Random.Range(1, 6);
        return talkEvent("home_49-" + rand.ToString());
    }
    public int houseEvent49_2()
    {
        int rand = Random.Range(6, 11);
        return talkEvent("home_49-" + rand.ToString());
    }
}
