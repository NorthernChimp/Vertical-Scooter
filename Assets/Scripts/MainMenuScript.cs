using System.Collections;

using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

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
    public BoxCollider2D tutorialButtonCollider;
    public TextMeshProUGUI highScoreText;
    public Counter menuButtonCounter;
    float width = 0f;
    float height = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetupMenu()
    {
        menuButtonCounter = new Counter(0.2f);menuButtonCounter.UpdateCounter(0.2f);
        width = Screen.width * 0.0085f;//occupies 85% of the width
        //width = Screen.width * 0.01f;//occupies 1% of the width
        height = width * 2f;
        Vector3 scale = new Vector3(width/10.24f , width/ 10.24f, 1f);
        //Vector3 scale = new Vector3(10.24f/width , 10.24f/width, 1f);
        //transform.localScale = Pooter.basicScale * 1.5f;
        transform.localScale = scale * 2f;
        //originalTitleScale = transform.localScale;
        //originalTitleScale = scale;
        originalTitleScale = new Vector3(2f, 2f, 2f);
        OpenMenu(true);
    }
    public void OpenMenu(bool showHighScore)
    {
        menuEnabled = true;
        //Debug.Log("opening menu");
        int renderCount = renders.Count;
        title.localPosition = new Vector3( 0f,2.56f, -1f);
        if (showHighScore) 
        {
            renderCount--;
            highScoreText.fontSize = Screen.width / 20f;
            highScoreText.transform.position = new Vector3(Screen.width * 0.5f ,(Screen.height * 0.5f) - (Screen.width * 0.45f), 0f);
            highScoreText.text = "High Score : " + PlayerPrefs.GetInt("HighScore", 0).ToString();
            renders[1].transform.localPosition = new Vector3(0f, 0f, -1f);
            renders[4].transform.localPosition = new Vector3(0f, -1.28f, -1f);
            renders[0].enabled = true;
            renders[1].enabled = true;
            renders[2].enabled = true;
            renders[4].enabled = true;
            resumeGameButtonCollider.transform.localPosition = new Vector3(Screen.width * 1f, 0f, -1f);
            renders[3].enabled = false;
            highScoreText.enabled = true;
        }
        else 
        {
            renders[0].enabled = true;
            renders[2].enabled = true;
            renders[1].transform.localPosition = new Vector3(0f, 0f, -1f);
            renders[3].transform.localPosition = new Vector3(0f, -1.28f, -1f);
            renders[4].transform.localPosition = new Vector3(Screen.width * 0.5f, -1.28f, -1f);
            renders[1].enabled = true;
            renders[4].enabled = false;
            renders[3].enabled = true;
            highScoreText.enabled = false; 
        }
        for(int i =0; i < renderCount; i++)
        {
            SpriteRenderer r = renders[i];
            //r.enabled = true;
        }
        timeOpen = 0f;
    }
    public void CloseMenu()
    {
        //Debug.Log("closing menu");
        menuButtonCounter.ResetCounter();
        menuEnabled = false;
        highScoreText.enabled = false;
        
        foreach (SpriteRenderer r in renders) { r.enabled = false; }
        //enabled = false;
    }
    public void UpdateMenu(float timePassed)
    {
        if (!menuButtonCounter.hasfinished) { menuButtonCounter.UpdateCounter(timePassed); }
        if (menuEnabled && !Advertisement.isShowing)
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
                        if (!MainScript.gameStarted) { m.StartGame(false); }
                        else
                        {
                            m.ClearEverything();
                            m.StartGame(false);
                        }

                        MainScript.gamePaused = false;
                        CloseMenu();
                    }
                    else if (hit.collider == resumeGameButtonCollider && renders[3].enabled)
                    {
                        MainScript.gamePaused = false;
                        MainScript m = Camera.main.GetComponent<MainScript>();
                        m.EnableHudElements(true);
                        //Debug.Log("tapped the resume button");
                        CloseMenu();
                    }else if(hit.collider == tutorialButtonCollider && renders[4].enabled)
                    {
                        MainScript m = Camera.main.GetComponent<MainScript>();
                        m.ClearEverything();
                        m.StartGame(true);
                        m.EnableHudElements(false);
                        MainScript.gamePaused = false;
                        CloseMenu();
                        if (!MainScript.gameStarted)
                        {
                            
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                /*Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                }*/
            }
        }
        

    }
    // Update is called once per frame
    void Update()
    {
        UpdateMenu(Time.deltaTime);
    }
}
