# UnityScriptableObjectIcons
## What?
Its just an Editor script, which allows you to have icons for Scriptable Objects from a Sprite or Texture2D Field.

Without the Editorscript:

![asset view disabled](Images/disabled.PNG)

With the Editorscript enabled:

![asset view enabled](Images/enabled.PNG)

Icons can be Enabled/Disabled in the Assetmenu:

![asset view enabled](Images/assetmenu.PNG)

## Usage

To add an thumbnail for Scriptable just add the [ScriptableObjectIcon] attribute to the supported field, which you want to use as the thumbnail:

```csharp
[CreateAssetMenu(fileName = "GenericItem")]
public class BlockType : ScriptableObject
{
    [ScriptableObjectIcon]
    public Sprite sprite;
    //or
    [ScriptableObjectIcon]
    public Texture2D texture;    


    //...
    public List<Field> OtherFields {get;set;}

}
```

## Error correction Fork

### Bug #1
Sprite files were displaying the full texture, so the method for selecting which texture to display was rewritten.
<br>
<img src="Images/bug1.png" width="200"/>
<img src="Images/arrow.png" width="100"/>
<img src="Images/bug1_solution.png" width="185"/>
<br>
### Bug #2
When zooming out, all the textures were stretched into a heap, covering the file names.
<br>
![asset view disabled](Images/bug2.png)
![asset view disabled](Images/arrow.png)
![asset view disabled](Images/bug2_solution.png)

## Future Development

- allow properties (Is it needed?)
- allow other texture/sprite types
