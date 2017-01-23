using UnityEngine;
using System.Collections;

/// <summary>
/// シーン間の情報を保存する場所
/// MonoBehaviourを継承していないのでunityの機能が使えないことに注意
/// </summary>
public class SceneInfoManager
{
    public readonly static SceneInfoManager instance = new SceneInfoManager();

    // 今いたステージの名前(Schoolなど)
    public string select_stage_name = null;
    // 
    public string select_map_name = null;

    public Vector3 player_pos = new Vector3();

    public int enemy_num = 3;

    public bool is_stagechangeevent = false;
}
