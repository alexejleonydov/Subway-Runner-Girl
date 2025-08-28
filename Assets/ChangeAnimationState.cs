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
                //     Debug.Log($"[Old Animation]  Play: {currAnim}");
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
        // Debug.Log("rrr Ability is setted to " + isAbility);

        if (isAbility)
        {
            if (currAnimState == "Run_new2 - Queued Clone" || currAnimState == "Run_new1 - Queued Clone" || currAnimState == "run" ||
    currAnimState == "Run - Queued Clone" || currAnimState == "Run_new1" || currAnimState == "Run_new2" || currAnimState == "super_Run" ||
     currAnimState == "super_Run - Queued Clone" || currAnimState == "hold_magnet")

                currAnimState = "hold_magnet_run";

            else currAnimState = currAnim;
        }
    }

    public void ChangeAnimatorState(string name)
    {
        // Debug.Log("rrr ChangeAnimatorState playing name: " + name);

        if (!string.IsNullOrEmpty(currAnim) && currAnim.StartsWith("SW_"))
        {
            currAnimState = "idle";
            //            Debug.Log("rrr OldState was SW_... " + name);
        }
        else if (isAbility)
        {
            if (name == "Run_new2 - Queued Clone" || name == "Run_new1 - Queued Clone" || name == "run" ||
            name == "Run - Queued Clone" || name == "Run_new1" || name == "Run_new2" || name == "super_Run" ||
             name == "super_Run - Queued Clone" || name == "hold_magnet")

                currAnimState = "hold_magnet_run";

            else
                currAnimState = name;
        }
        else
        {
            currAnimState = name;
        }
        //Debug.Log("rrr ChangeAnimatorState playing currAnimState: " + currAnimState);
        if (charAnimator != null)
        {
            charAnimator.Play(currAnimState);
        }
    }
}
