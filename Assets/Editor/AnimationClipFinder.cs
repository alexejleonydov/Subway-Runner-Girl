using UnityEngine;
using UnityEditor;

public class AnimationClipFinder : EditorWindow
{
    public AnimationClip clipToFind;

    [MenuItem("Tools/Find Clip Usage")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipFinder>("Find Clip Usage");
    }

    void OnGUI()
    {
        clipToFind = (AnimationClip)EditorGUILayout.ObjectField("Clip to Find", clipToFind, typeof(AnimationClip), false);

        if (GUILayout.Button("Find in Scene"))
        {
            FindClipUsageInScene();
        }
    }

    void FindClipUsageInScene()
    {
        if (clipToFind == null)
        {
            Debug.LogWarning("No clip assigned!");
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            Animator animator = go.GetComponent<Animator>();
            if (animator && animator.runtimeAnimatorController != null)
            {
                var controller = animator.runtimeAnimatorController;
                var clips = controller.animationClips;
                foreach (var c in clips)
                {
                    if (c == clipToFind)
                    {
                        Debug.Log($"✅ Clip '{clipToFind.name}' is used in Animator on: {go.name}", go);
                    }
                }
            }

            Animation animation = go.GetComponent<Animation>();
            if (animation)
            {
                foreach (AnimationState state in animation)
                {
                    if (state.clip == clipToFind)
                    {
                        Debug.Log($"✅ Clip '{clipToFind.name}' is used in Animation component on: {go.name}", go);
                    }
                }
            }
        }

        Debug.Log("🔍 Search complete.");
    }
}
