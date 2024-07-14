using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonsManager : MonoBehaviour
{
    public MouseClickSystem mouseClickSystem;
    public GameObject prefab;
    public GameObject silhouettePrefab;
    public int towerCost;
    public string towerName;

    public TMP_Text TowerButtonText;
    private Button button;
    public AudioSource buttonSound;
    public AudioClip buttonClickClip;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        if (TowerButtonText != null)
        {
            TowerButtonText.text = "Nothing Selected";
        }
    }

    private void OnClick()
    {
        PlayButtonSound();
        Debug.Log("Button clicked: " + gameObject.name);
        mouseClickSystem.SetPlacementPrefab(prefab, silhouettePrefab, towerCost);
        TowerButtonText.text = towerName + ": " + towerCost + "G";
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}
