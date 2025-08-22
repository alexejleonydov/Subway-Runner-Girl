using UnityEngine;

public class ChangeAnimationState : MonoBehaviour
{
    private Animator charAnimator;
    public Animation charAnimation;

    private bool isRightAnim;
    private string currAnim;
    private string currAnimState = "idle";
    private bool isAbility;

    private void Awake()
    {
        currAnimState = "idle";
        UpdateActiveAnimator();
    }

    private void Update()
    {

        UpdateActiveAnimator();

        // Debug.Log("Active Animator is: " + charAnimator.gameObject.name);

        if (charAnimation == null) return;

        foreach (AnimationState state in charAnimation)
        {
            if (charAnimation.IsPlaying(state.name))
            {
                currAnim = state.name;
                Debug.Log($"[Old Animation] Play: {currAnim}");
            }
        }

        isRightAnim = currAnim == currAnimState;

        if (!isRightAnim)
        {
            ChangeAnimatorState(currAnim);
        }
    }


    private void UpdateActiveAnimator()

    {
        GameObject avatars = GameObject.Find("avatars");
        if (avatars == null)
        {
            Debug.LogWarning("avatars not found!");
            return;
        }

        foreach (Transform character in avatars.transform)
        {
            Animator anim = FindActiveAnimatorInChildren(character);
            if (anim != null)
            {
                charAnimator = anim;
                //  Debug.Log("Found active Animator on: " + anim.gameObject.name);
                return;
            }
        }

        Debug.LogWarning("No active Animator found!");
    }

    private Animator FindActiveAnimatorInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeInHierarchy)
            {
                Animator anim = child.GetComponent<Animator>();
                if (anim != null)
                    return anim;


                Animator found = FindActiveAnimatorInChildren(child);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    public void SetAbilty(bool ability)
    {
        isAbility = ability;
        //Debug.Log("Ability is setted to " + isAbility);
    }

    public void ChangeAnimatorState(string name)
    {
        //Debug.Log("ChangeAnimatorState playing name: " + name);

        if (isAbility)
        {
            if (name == "jump" || name == "jump_salto" || name == "jump_2" || name == "jump_3")
                currAnimState = "hold_magnet_jump";
            else if (name == "roll")
                currAnimState = "roll";
            else if (name == "run_side_R")
                currAnimState = "run_side_R";
            else if (name == "run_side_L")
                currAnimState = "run_side_L";
            else
                currAnimState = "hold_magnet_run";
        }
        else
        {
            currAnimState = name;
        }

        if (charAnimator != null)
        {
            charAnimator.Play(currAnimState);
        }
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ChangeAnimationState : MonoBehaviour
// {
//     public Animator charAnimator;
//     public Animation charAnimation;
//     private bool isRightAnim;
//     private string currAnim;
//     private string currAnimState = "idle";
//     private string prevAnim;
//     private bool isNormalAnim = true;
//     private bool isAbility;
//     private void Awake()
//     {
//         currAnimState = "idle";
//     }

//     private void Update()
//     {
//         Debug.Log("Currently playing: " + currAnim);


//         foreach (AnimationState state in charAnimation)
//         {
//             if (charAnimation.IsPlaying(state.name))
//             {
//                 currAnim = state.name;

//                 // Debug.Log("Currently playing: " + state.name);
//             }
//         }

//         isRightAnim = currAnim == currAnimState;



//         if (!isRightAnim)
//         {

//             ChangeAnimatorState(currAnim);
//         }
//     }

//     public void SetAbilty(bool ability)
//     {
//         isAbility = ability;
//     }

//     public void ChangeAnimatorState(string name)
//     {
//         if (isAbility)
//         {

//             if (name == "jump" || name == "jump_salto" || name == "jump_2" || name == "jump_3")
//             {

//                 //if(name == "hold_magnet")
//                 currAnimState = "hold_magnet_jump";
//             }

//             else if (name == "roll")
//             {
//                 currAnimState = "roll";
//             }

//             else if (name == "run_side_R")
//             {
//                 currAnimState = "run_side_R";
//             }

//             else if (name == "run_side_L")
//             {
//                 currAnimState = "run_side_L";
//             }

//             else
//             {
//                 currAnimState = "hold_magnet_run";
//             }

//             Debug.Log("Currently playing: " + currAnimState);
//         }


//         /*else if (currAnimState == "jump" || currAnimState == "jump_salto" || currAnimState == "jump_2" || currAnimState == "jump_3")
//         {
//             if (name == "hangtime")
//             {
//                 currAnimState = "run";
//             }

//             else if (name == "hangtime_3")
//             {
//                 currAnimState = "run";
//             }

//         }*/

//         else
//         {
//             currAnimState = name;
//             isNormalAnim = true;
//         }
//         //if(isNormalAnim)
//         Debug.Log("Currently finally playing: " + currAnimState);
//         charAnimator.Play(currAnimState);

//     }


// }
