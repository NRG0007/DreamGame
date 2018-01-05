using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Music,
    Sound,
    Texture,
    Sprite,
    Font,
    Icon,
    Text,
    Prefab,
    UIPrefab,
    ItemPrefab,
    EffectPrefab,
    Max,
}

public class ResourcesManager 
{
    private static Dictionary<ResourceType, string> m_Paths = new Dictionary<ResourceType, string>();

    public static void Init()
    {
        m_Paths.Clear();
        for (ResourceType i = 0; i < ResourceType.Max; i++)
        {
            string path = "";
            switch (i)
            {
                case ResourceType.Music: path = "Musics"; break;
                case ResourceType.Sound: path = "Sounds"; break;
                case ResourceType.Texture: path = "Textures"; break;
                case ResourceType.Sprite: path = "Sprites"; break;
                case ResourceType.Font: path = "Fonts"; break;
                case ResourceType.Icon: path = "Icons"; break;
                case ResourceType.Text: path = "Texts"; break;
                case ResourceType.Prefab: path = "Prefabs"; break;
                case ResourceType.UIPrefab: path = "Prefabs/UI"; break;
                case ResourceType.ItemPrefab: path = "Prefabs/Item"; break;
                case ResourceType.EffectPrefab: path = "Prefabs/Effect"; break;
            }
            m_Paths[i] = path;
        }
    }

    public static T Load<T>(ResourceType type, string name)
        where T : Object
    {
        return Resources.Load<T>(m_Paths[type] + "/" + name);
    }

    public static GameObject Instantiate(ResourceType type, string name)
    {
        Object prefab = Load<Object>(type, name);
        GameObject obj = null;
        if (prefab != null)
        {
            obj = GameObject.Instantiate(prefab) as GameObject;
            Resources.UnloadAsset(prefab);
        }
        return obj;
    }

    public static void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
}