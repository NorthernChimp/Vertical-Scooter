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
    public Vector2 moveDirect = Vector2.zero;
    public void Setup(float timeAlive)
    {
        endCounter = new Counter(timeAlive);
    }
    public void Destroy() { Destroy(gameObject); }
}
