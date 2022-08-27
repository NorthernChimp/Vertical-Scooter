using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeart : MonoBehaviour
{
    public bool isFull = true;
    // Start is called before the first frame update
    public Animator anim;
    public void SetZero(){anim.Play("loseHealthAnim"); isFull = false; }
    public void SetOne() { anim.Play("regenHealthAnim"); isFull = true; }
}
