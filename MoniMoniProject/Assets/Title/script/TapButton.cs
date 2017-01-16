using UnityEngine;
using System.Collections;

public class TapButton : MonoBehaviour {
   private bool clicked;
    private float changeAlpha;

    // Use this for initialization
    void Start () {
        
        clicked = false;
        changeAlpha = 1.0f;

    }

    // Update is called once per frame
    void Update () {
         this.GetComponent<CanvasGroup>().alpha = changeAlpha;
          
        if (clicked == true)
        {
             if (changeAlpha > 0.00f)
            {
                changeAlpha -= 0.02f;
            }
            if (changeAlpha <= 0.00f)
            {
                changeAlpha = 0.00f;
                clicked = false;
                gameObject.SetActive(false);
            }
        }
    }
    public void OnClick()
    {
        clicked = true;
    }
   
}
