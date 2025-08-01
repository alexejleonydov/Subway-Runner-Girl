using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimationState : MonoBehaviour
{
    public Animator charAnimator;
    public Animation charAnimation;
    private bool isRightAnim;
    private string currAnim;
    private string currAnimState = "idle";

    private void Awake()
    {
        currAnimState = "idle";
    }

    private void Update()
    {
        Debug.Log("Currently playing: " + currAnim);
        Debug.Log("Currently playing: " + currAnimState);

        foreach (AnimationState state in charAnimation)
        {
            if (charAnimation.IsPlaying(state.name))
            {
                currAnim = state.name;

                Debug.Log("Currently playing: " + state.name);
            }
        }

        isRightAnim = currAnim == currAnimState;



        if (!isRightAnim)
        {
            ChangeAnimatorState(currAnim);

        }
    }

    public void ChangeAnimatorState(string name)
    {
        currAnimState = name;
        charAnimator.Play(currAnimState);
    }


}
