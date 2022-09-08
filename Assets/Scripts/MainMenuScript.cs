using System.Collections;

using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public Transform title;
    Vector3 originalTitleScale = Vector3.zero;
    float timeOpen = 0f;
    public Transform newGameButton;
    public List<SpriteRenderer> renders;
    public bool menuEnabled = true;
    public BoxCollider2D newGameButtonCollider;
    public BoxCollider2D resumeGameButtonCollider;
    public TextMeshProUGUI highScoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetupMenu()
    {
        transform.localScale = Pooter.basicScale * 1.5f;
        originalTitleScale = transform.localScale;
        OpenMenu(true);
    }
    public void OpenMenu(bool showHighScore)
    {
        menuEnabled = true;
        //Debug.Log("opening menu");
        int renderCount = renders.Count; 
        if (showHighScore) 
        {
            renderCount--;
            highScoreText.fontSize = Screen.width / 20f;
            highScoreText.transform.position = new Vector3(Screen.width * 0.5f ,(Screen.height * 0.5f) - (Screen.width * 0.25f), 0f);
            highScoreText.text = "High Score : " + PlayerPrefs.GetInt("HighScore");
            renders[3].enabled = false;
            highScoreText.enabled = true;
        }
        else { highScoreText.enabled = false; }
        for(int i =0; i < renderCount; i++)
        {
            SpriteRenderer r = renders[i];
            r.enabled = true;
        }
        timeOpen = 0f;
    }
    public void CloseMenu()
    {
        //Debug.Log("closing menu");
        menuEnabled = false;
        highScoreText.enabled = false;
        foreach (SpriteRenderer r in renders) { r.enabled = false; }
        //enabled = false;
    }
    public void UpdateMenu(float timePassed)
    {
        if (menuEnabled)
        {
            
            timeOpen += timePassed * Mathf.PI * 1.5f;
            title.localScale = originalTitleScale + (0.062f * originalTitleScale * Mathf.Sin(timeOpen));
            foreach(Touch t in Input.touches)
            {
                if(t.phase == TouchPhase.Began)
                {
                    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(t.position);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                    //If something was hit, the RaycastHit2D.collider will not be null.

                    if (hit.collider == newGameButtonCollider)
                    {
                        MainScript m = Camera.main.GetComponent<MainScript>();
                        if (!MainScript.gameStarted) { m.StartGame(); }
                        else
                        {
                            m.ClearEverything();
                            m.StartGame();
                        }

                        MainScript.gamePaused = false;
                        CloseMenu();
                    }
                    else if (hit.collider == resumeGameButtonCollider)
                    {
                        MainScript.gamePaused = false;
                        CloseMenu();
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                //If something was hit, the RaycastHit2D.collider will not be null.
                
                if (hit.collider == newGameButtonCollider)
                {
                    MainScript m = Camera.main.GetComponent<MainScript>();
                    if (!MainScript.gameStarted) { m.StartGame(); } else
                    {
                        m.ClearEverything();
                        m.StartGame();
                    }
                    
                    MainScript.gamePaused = false;
                    CloseMenu();
                }else if (hit.collider == resumeGameButtonCollider)
                {
                    MainScript.gamePaused = false;
                    CloseMenu();
                }
            }
        }
        

    }
    // Update is called once per frame
    void Update()
    {
        UpdateMenu(Time.deltaTime);
    }
}
