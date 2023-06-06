using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedTimeVisualEffect :MonoBehaviour
{
    public Counter endCounter;
    public bool rotates = false;
    public float rotateSpeed = 0f;
    public bool moves = false;
    public float moveSpeed = 0f;
    bool fades = false;
    public Vector2 moveDirect = Vector2.zero;
    Color setColor;
    SpriteRenderer render;
    public void Setup(float timeAlive,bool fade,Color c)
    {
        setColor = c;
        endCounter = new Counter(timeAlive);
        fades = fade;
        render = GetComponent<SpriteRenderer>(); render.color = setColor;
    }
    public void UpdateEffect(float timePassed)
    {
        endCounter.UpdateCounter(timePassed);
        if (fades)
        {
            float percent = endCounter.GetPercentageDone();
            float alpha = 1f - percent;
            render.color = new Color(setColor.r, setColor.g, setColor.b, setColor.a * alpha);
        }
    }
    public void Destroy() { Destroy(gameObject); }
}
