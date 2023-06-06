using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
    public List<Sprite> tutorialImages;
    SpriteRenderer render;
    BoxCollider2D coll;
    bool tutorialScreenTapped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public bool GetTapped() { return tutorialScreenTapped; }
    public void SetTapped(bool b) { tutorialScreenTapped = b; }
    public void SetupTutorialScreen()
    {
        SetTapped(false);
        coll = GetComponent<BoxCollider2D>();
        float aspectRatio = (Screen.width * 0.0055f) / 5.12f  ;
        //float aspectRatio = 5.12f / (Screen.width * 0.55f);
        Vector3 pos = transform.localPosition;
        pos.y = Screen.height * 0.0025f;
        transform.localPosition = pos;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f);
        render = GetComponent<SpriteRenderer>();
        SetImage(0);
        render.enabled = false;
    }
    public void Activate(bool turnOnNotOff) 
    {
        render.enabled = turnOnNotOff;
        SetTapped(false);
    }
    public void SetImage(int i)
    {
        render.sprite = tutorialImages[i];
    }
    public bool DidThisTouchImpact(Touch t)
    {
        bool b = false;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(t.position);
        if (coll.OverlapPoint(worldPoint)) { b = true; }
        return b;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
