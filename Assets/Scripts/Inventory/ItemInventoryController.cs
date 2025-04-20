using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryController : MonoBehaviour
{
    public string Index;
    public Texture defaultTexture;
    private Outline outline;

    void Start()
    {
        ResetDefaults();
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }

    public void ResetDefaults(){
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if(text != null){
            text.text = Index;
        }

        RawImage icon = gameObject.GetComponentInChildren<RawImage>();
        if(icon != null){
            icon.texture = defaultTexture;
        }
    }

    public void ItemSelected(bool isActive){
        outline.enabled = isActive;
    }

    public void InstantiateItem(Texture texture){
        RawImage icon = gameObject.GetComponentInChildren<RawImage>();
        if(icon != null){
            icon.texture = texture;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
