using UnityEngine;
using System.Collections.Generic;

public class UISpriteCheckerRuntime : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 UISprite Runtime Check Started");

        UISprite[] sprites = FindObjectsOfType<UISprite>(true);
        List<UISprite> missingAtlas = new List<UISprite>();
        List<UISprite> missingMaterial = new List<UISprite>();

        foreach (var sprite in sprites)
        {
            if (sprite.atlas == null)
                missingAtlas.Add(sprite);

            if (sprite.material == null)
                missingMaterial.Add(sprite);
        }

        Debug.Log($"🧮 Total UISprites found: {sprites.Length}");
        Debug.Log($"❌ Missing Atlas: {missingAtlas.Count}");
        Debug.Log($"❌ Missing Material: {missingMaterial.Count}");

        foreach (var sprite in missingAtlas)
            Debug.LogWarning($"[Missing Atlas] GameObject: {sprite.gameObject.name}", sprite.gameObject);

        foreach (var sprite in missingMaterial)
            Debug.LogWarning($"[Missing Material] GameObject: {sprite.gameObject.name}", sprite.gameObject);

        if (missingAtlas.Count == 0 && missingMaterial.Count == 0)
            Debug.Log("✅ All UISprites have Atlas and Material assigned.");
    }
}