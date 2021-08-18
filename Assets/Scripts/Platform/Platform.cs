using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //Linked List
    public Platform Next;
    public Animator Animator;
    public float Progress;

    /// <summary>
    /// did the player jump on it
    /// </summary>
    public bool IsUsed;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public virtual void Bounce() {
        IsUsed = false;
        if(Animator != null) Animator.Play("Bounce");
    }

    
}
