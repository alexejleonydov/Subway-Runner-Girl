using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnChangeAnimation : MonoBehaviour
{
    private Animator charAnimator;
    public Animation charAnimation;
    private bool isRightAnim;
    private string currAnim;
    private string currAnimState = "idle";
    private string prevAnim;
    private bool isNormalAnim = true;
    private bool isAbility;
    private void Awake()
    {
        charAnimator = GetComponent<Animator>();
        currAnimState = "idle";
    }

    private void Update()
    {
        //   Debug.Log("Currently playing: " + currAnim);


        foreach (AnimationState state in charAnimation)
        {
            if (charAnimation.IsPlaying(state.name))
            {
                currAnim = state.name;

                // Debug.Log("Currently playing: " + state.name);
            }
        }

        isRightAnim = currAnim == currAnimState;



        if (!isRightAnim)
        {

            ChangeAnimatorState(currAnim);
        }
    }

    public void SetAbilty(bool ability)
    {
        isAbility = ability;
    }

    public void ChangeAnimatorState(string name)
    {
        if (isAbility)
        {

            if (name == "jump" || name == "jump_salto" || name == "jump_2" || name == "jump_3")
            {

                //if(name == "hold_magnet")
                currAnimState = "hold_magnet_jump";
            }

            else if (name == "roll")
            {
                currAnimState = "roll";
            }

            else if (name == "run_side_R")
            {
                currAnimState = "run_side_R";
            }

            else if (name == "run_side_L")
            {
                currAnimState = "run_side_L";
            }

            else
            {
                currAnimState = "hold_magnet_run";
            }

            // Debug.Log("Currently playing: " + currAnimState);
        }


        /*else if (currAnimState == "jump" || currAnimState == "jump_salto" || currAnimState == "jump_2" || currAnimState == "jump_3")
        {
            if (name == "hangtime")
            {
                currAnimState = "run";
            }

            else if (name == "hangtime_3")
            {
                currAnimState = "run";
            }

        }*/

        else
        {
            currAnimState = name;
            isNormalAnim = true;
        }
        //if(isNormalAnim)
        // Debug.Log("Currently finally playing: " + currAnimState);
        charAnimator.Play(currAnimState);

    }


}
