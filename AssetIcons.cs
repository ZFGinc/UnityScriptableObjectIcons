using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ScriptableObjectIcon : Attribute { }

[ExecuteInEditMode]
[InitializeOnLoad]
public class AssetIcons : Editor
{

    static bool unityDeafaultOnNull;
    static bool UnityDeafaultOnNull
    {
        get
        {
            return unityDeafaultOnNull;
        }
        set
        {
            Menu.SetChecked(MENU_NAME_DefOnNull, value);
            EditorPrefs.SetBool(MENU_NAME_DefOnNull, value);
            unityDeafaultOnNull = value;
        }
    }

    static bool disableIcons;
    static bool DisableIcons
    {
        get
        {
            return disableIcons;
        }
        set
        {
            disableIcons = value;
            SetStateDisabled(value);
        }
    }

    public static Color backgroundColor = new Color(82f / 255f, 82f / 255f, 82f / 255f, 1f);
    static Texture2D bg;

    private const string MENU_NAME_DISABLE_ICONS = "Assets/Icons/Disable";
    private const string MENU_NAME_DefOnNull = "Assets/Icons/Unity default for Null";


    [MenuItem(MENU_NAME_DISABLE_ICONS)]
    private static void ToggleShowIconsAction()
    {
        DisableIcons = !DisableIcons;
    }

    [MenuItem(MENU_NAME_DefOnNull)]
    private static void ToggleDefOnNullAction()
    {
        UnityDeafaultOnNull = !UnityDeafaultOnNull;
    }

    static AssetIcons()
    {
        disableIcons = EditorPrefs.GetBool(MENU_NAME_DISABLE_ICONS, false);
        unityDeafaultOnNull = EditorPrefs.GetBool(MENU_NAME_DefOnNull, false);

        EditorApplication.delayCall += () =>
        {
            SetStateDisabled(disableIcons);
            UnityDeafaultOnNull = unityDeafaultOnNull;
        };
    }

    static void SetStateDisabled(bool value)
    {
        Menu.SetChecked(MENU_NAME_DISABLE_ICONS, value);
        EditorPrefs.SetBool(MENU_NAME_DISABLE_ICONS, value);
        if (value)
        {
            EditorApplication.projectWindowItemOnGUI -= MyCallback();
        }
        else
        {
            EditorApplication.projectWindowItemOnGUI -= MyCallback();
            EditorApplication.projectWindowItemOnGUI += MyCallback();
        }
    }

    static EditorApplication.ProjectWindowItemCallback MyCallback()
    {
        EditorApplication.ProjectWindowItemCallback myCallback = new EditorApplication.ProjectWindowItemCallback(IconGUI);
        return myCallback;
    }

    static Texture2D GetSlicedSpriteTexture(Sprite sprite)
    {
        if (CheckSpriteMode(sprite) != SpriteImportMode.Multiple)
        {
            return sprite.texture;
        }

        Rect rect = sprite.rect;
        Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height);
        slicedTex.filterMode = sprite.texture.filterMode;

        slicedTex.SetPixels(0, 0, (int)rect.width, (int)rect.height, sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        slicedTex.Apply();

        return slicedTex;
    }

    static SpriteImportMode CheckSpriteMode(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null!");
            return SpriteImportMode.None;
        }

        // Получаем путь к текстуре спрайта
        string assetPath = AssetDatabase.GetAssetPath(sprite.texture);

        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Не удалось получить путь к текстуре спрайта.");
            return SpriteImportMode.None;
        }

        // Получаем импортер текстуры
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (textureImporter == null)
        {
            Debug.LogError("Не удалось получить импортер текстуры.");
            return SpriteImportMode.None;
        }

        return textureImporter.spriteImportMode;
    }

    static void IconGUI(string s, Rect r)
    {
        var guid = AssetDatabase.GUIDToAssetPath(s);

        if (bg == null)
        {
            bg = new Texture2D(32, 32);
        }

        var t = AssetDatabase.LoadAssetAtPath(guid, typeof(object)) as object;
        if (t == null || t.GetType() == null) return;

        Texture2D texture = null;

        var atts = t.GetType().GetFields().Where(fi => ((fi == null) ? 0 : fi.GetCustomAttributes(typeof(ScriptableObjectIcon), false).Count()) > 0);

        if (atts != null && atts.Count() == 1)
        {
            object obj = atts.First().GetValue(t);
            if (obj == null)
                return;

            if (obj.GetType() == typeof(Sprite))
            {
                Sprite sprite = (Sprite)obj;
                if (sprite != null)
                {
                    texture = GetSlicedSpriteTexture(sprite);
                }
            }

            if (obj.GetType() == typeof(Texture2D))
                texture = (Texture2D)obj;
        }
        else
        {
            return;
        }

        if (texture == null && unityDeafaultOnNull)
            return;

        Rect r2 = new Rect(r);
        r2.height -= 14;
        r2.width = r2.height;
        r.yMin += 5;
        r.height -= 22;

        if (texture != null && r2.height >= 14)
        {
            GUI.DrawTexture(r2, bg, ScaleMode.ScaleToFit, false);
            GUI.DrawTexture(r2, bg, ScaleMode.ScaleToFit, true, 0, backgroundColor, 2, 3);

            GUI.DrawTexture(r, texture, ScaleMode.ScaleToFit);
        }
    }
}
