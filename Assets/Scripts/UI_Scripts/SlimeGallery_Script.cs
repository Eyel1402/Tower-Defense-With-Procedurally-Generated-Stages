using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Slime_Script : MonoBehaviour
{
    [SerializeField] private TMP_Text Basic_title;
    [SerializeField] private TMP_Text Health_Title;
    [SerializeField] private TMP_Text Speed_title;
    [SerializeField] private TMP_Text Description;

    [SerializeField] private Image slimeImage;
    [SerializeField] private Sprite basicSlimeSprite;
    [SerializeField] private Sprite pinkSlimeSprite;
    [SerializeField] private Sprite redSlimeSprite;
    [SerializeField] private Sprite yellowSlimeSprite;
    [SerializeField] private Sprite flyingSlimeSprite;
    [SerializeField] private Sprite takoSlimeSprite;

    void Start()
    {
        // Set default values upon initialization
        Basic_Slime();
    }

    void Update()
    {
        // when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back_Button();
        }
    }
    public void Basic_Slime()
    {
        Basic_title.text = "Blue Slime";
        Health_Title.text = "Health: 30";
        Speed_title.text = "Speed: 0.75";
        Description.text = "A simple-minded gelatinous slime monster, it is generally harmless on its own.";
        slimeImage.sprite = basicSlimeSprite;
    }

    public void Pink_Slime()
    {
        Basic_title.text = "Pink Slime";
        Health_Title.text = "Health: 150";
        Speed_title.text = "Speed: 0.5";
        Description.text = "A gelatinous slime monster, it is able to absorb more damage due to its elastic and bouncy body.";
        slimeImage.sprite = pinkSlimeSprite;
    }

    public void Red_Slime()
    {
        Basic_title.text = "Red Slime";
        Health_Title.text = "Health: 75";
        Speed_title.text = "Speed: 0.5";
        Description.text = "A gelatinous slime monster that is highly active and might explode to multiple tiny versions of itself. Splits into 3 Lesser Red Slimes.";
        slimeImage.sprite = redSlimeSprite;
    }

    public void Yellow_Slime()
    {
        Basic_title.text = "Yellow Slime";
        Health_Title.text = "Health: 20";
        Speed_title.text = "Speed: 2.0";
        Description.text = "A gelatinous slime monster, its body seems to be quite slippery thus allowing it to move fast.";
        slimeImage.sprite = yellowSlimeSprite;
    }

    public void Flying_Slime()
    {
        Basic_title.text = "Flying Slime";
        Health_Title.text = "Health: 20";
        Speed_title.text = "Speed: 0.75";
        Description.text = "A gelatinous slime monster that has wings. It actually cannot fly on its own and relies on the balloon attached to it.";
        slimeImage.sprite = flyingSlimeSprite;
    }

    public void Tako_Slime()
    {
        Basic_title.text = "Tako Slime";
        Health_Title.text = "Health: 420";
        Speed_title.text = "Speed: 0.25";
        Description.text = "A gelatinous slime monster that vaguely resembles an octopus with very short tentacles. Its body is quite resilient. These slimes are particularly fond of cookies, word puns, and dad jokes.";
        slimeImage.sprite = takoSlimeSprite;
    }

    public void Back_Button()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}
