using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Title_logo : MonoBehaviour
{
    private GameObject black_1=null;
    black_mist black_mist_1;
    private GameObject black_2=null;
    black_mist black_mist_2;
    private GameObject black_3=null;
    black_mist black_mist_3;
    private GameObject black_4=null;
    black_mist black_mist_4;
    bool Title_end;
    Vector3 pos;
    Vector3 size;
    float pos_times;
    private float changeAlpha;
	// Use this for initialization
	void Start ()
    {
        size = transform.localScale;
        black_1 = GameObject.Find("black_mist_1");
        black_2 = GameObject.Find("black_mist_2"); ;
        black_3 = GameObject.Find("black_mist_3"); ;
        black_4 = GameObject.Find("black_mist_4"); ;
        black_mist_1 = black_1.GetComponent<black_mist>();
        black_mist_2 = black_2.GetComponent<black_mist>();
        black_mist_3 = black_3.GetComponent<black_mist>();
        black_mist_4 = black_4.GetComponent<black_mist>();
        Title_end = false;
        pos = transform.localPosition;
        pos_times = 1.00f;
        changeAlpha = 1.00f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Title_end == true)
        {
            pos_times -= 0.025f;
            transform.localPosition =
                new Vector3(
                pos.x * pos_times,
                pos.y * pos_times,
                pos.z
                );
            if (pos_times <= 0.00f)
            {
                size.x += 0.001f;
                size.y += 0.001f;
                transform.localScale = size;
                if (changeAlpha>0.00f)changeAlpha -= 0.02f;
                else SceneManager.LoadScene("Scenario");
                this.GetComponent<CanvasGroup>().alpha = changeAlpha;
            }
        }
	}
    public void end_logo()
    {
        if (Title_end == false)
        {
            pos = transform.localPosition;
            Title_end = true;
            black_mist_1.OnClick();
            black_mist_2.OnClick();
            black_mist_3.OnClick();
            black_mist_4.OnClick();
        }
    }
}
