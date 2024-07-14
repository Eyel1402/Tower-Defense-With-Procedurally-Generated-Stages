using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeletePropsButton : MonoBehaviour
{
    public MouseClickSystem mouseClickSystem;
    public TMP_Text detailButtonText;
    private Button button;
    public int removalCost = 0;
    public AudioSource buttonSound;
    public AudioClip buttonClickClip;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        if (buttonClickClip == null)
        {
            Debug.LogWarning("Button click sound clip is not assigned.");
        }
    }

    void OnClick()
    {
        PlayButtonSound();
        Debug.Log("Delete Prop Button clicked");
        mouseClickSystem.isDeletingProps = true;
        mouseClickSystem.SetPropRemovalCost(removalCost);
        detailButtonText.text = "Remove Obstacle (" + removalCost + "G)"; 
    } 
    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}
