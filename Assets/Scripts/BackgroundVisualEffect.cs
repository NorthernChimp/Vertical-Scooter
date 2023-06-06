using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundVisualEffect : MonoBehaviour
{
    // Start is called before the first frame update
    Counter replayCounter;
    SpriteRenderer render;
    float baseSpeed = 1f;
    float baseTime = 1f;
    float currentSpeed;
    Animator anim;
    void Start()
    {
        
    }
    public void SetupBackgroundVisualEffect(float scaleMultiplier,Color c,float timeBetweenPlays,float velocity)
    {
        baseSpeed = velocity;
        RandomizeCurrentSpeed();
        transform.localScale = Pooter.basicScale * scaleMultiplier;
        render = GetComponent<SpriteRenderer>();
        render.color = c;
        baseTime = timeBetweenPlays;
        RandomizeCurrentCounter();
        anim = GetComponent<Animator>();
        PlayAnimation();
    }
    void RandomizeCurrentSpeed()
    {
        currentSpeed = baseSpeed * (0.75f + (Random.value * 0.5f));
    }
    void RandomizeCurrentCounter()
    {
        replayCounter = new Counter(baseTime * (0.75f + (Random.value * 0.75f)));
    }
    public void UpdateEffect(float timePassed)
    {
        replayCounter.UpdateCounter(timePassed);
        if (replayCounter.hasfinished)
        {
            RandomizeCurrentCounter();
            PlayAnimation();
        }
        transform.Translate(Vector3.down * timePassed * currentSpeed);
        RepositionIfOffCamera();
    }
    void RepositionIfOffCamera()
    {
        Vector3 pos = transform.position;
        float y = pos.y;
        float camY = Camera.main.transform.position.y;
        if(camY - y > Screen.height * 0.0055f)
        {
            float randomXpos = Random.value * Screen.width * 0.01f;randomXpos -= Screen.width * 0.005f;
            pos.x = randomXpos;
            pos.y += Screen.height * 0.011f;
            transform.position = pos;
            RandomizeCurrentSpeed();
        }
    }
    void PlayAnimation() { anim.Play("EffectAnimation"); }
    public void PauseEffect()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
