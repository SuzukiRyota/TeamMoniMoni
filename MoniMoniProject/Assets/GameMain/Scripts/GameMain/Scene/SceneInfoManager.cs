using UnityEngine;
using System.Collections;

public class SceneInfoManager
{
    public readonly static SceneInfoManager instance = new SceneInfoManager();

    public string select_stage_name = null;
    public string select_map_name = null;

    public Vector3 player_pos = new Vector3();

    public int enemy_num = 3;

    public bool is_stagechangeevent = false;
}
