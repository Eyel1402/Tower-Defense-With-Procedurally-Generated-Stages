using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Tower_Gallery_Script : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Name_title;
    [SerializeField]
    private TMP_Text Attack_title;
    [SerializeField]
    private TMP_Text ATTSpeed_title;
    [SerializeField]
    private TMP_Text Range_title;
    [SerializeField]
    private TMP_Text Cost_title;

    [SerializeField]
    private TMP_Text Tower_Description;

    [SerializeField]
    private Image TowerImage;
    [SerializeField]
    private Sprite BasicTowerSprite;
    [SerializeField]
    private Sprite IceTowerSprite;
    [SerializeField]
    private Sprite FireTowerSprite;
    [SerializeField]
    private Sprite ShockTowerSprite;
    [SerializeField]
    private Sprite MegaTowerSprite;
    [SerializeField]
    private Sprite ToxinTowerSprite;

    void Start()
    {
        BasicTower();
    }
    void Update()
    {
        // when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Tower_Back_Button();
        }
    }
    public void BasicTower()
    {
        Name_title.text = "Stone Tower";
        Attack_title.text = "10";
        ATTSpeed_title.text = "2.5 secs";
        Range_title.text = "2.5 units";
        Cost_title.text = "10G";
        Tower_Description.text = "A run-off-the-mill single target tower with moderate damage, attack speed, and range on a cheap price tag. Great for starting off.";
        TowerImage.sprite = BasicTowerSprite;
    }

    public void IceTower()
    {
        Name_title.text = "Ice Tower";
        Attack_title.text = "2";
        ATTSpeed_title.text = "2 secs";
        Range_title.text = "2 units";
        Cost_title.text = "20G";
        Tower_Description.text = "Slowing Tower made of Ice. Attacks inflict a slow effect which reduces movement speed for enemies that it hits for 2 seconds. Attacks deal low damage and have a relatively short range.";
        TowerImage.sprite = IceTowerSprite;
    }
    public void FireTower()
    {
        Name_title.text = "Fire Tower";
        Attack_title.text = "15";
        ATTSpeed_title.text = "7.5 secs";
        Range_title.text = "3.5 units;";
        Cost_title.text = "30G";
        Tower_Description.text = "Splash Damage Tower that launch explosive fire. Attacks inflict high damage on its target and enemies near it. Slow Attack Speed but has a long attack range.";
        TowerImage.sprite = FireTowerSprite;
    }
    public void ShockTower()
    {
        Name_title.text = "Shock Tower";
        Attack_title.text = "5";
        ATTSpeed_title.text = "0.5 secs";
        Range_title.text = "1.5 units";
        Cost_title.text = "25G";
        Tower_Description.text = "Fast attacking Single Target Tower that shocks enemies with electricity. Low Base Damage and short range but compensated by a higher frequency of attacks.";
        TowerImage.sprite = ShockTowerSprite;
    }
    public void MegaTower()
    {
        Name_title.text = "Mega Tower";
        Attack_title.text = "50";
        ATTSpeed_title.text = "8 secs";
        Range_title.text = "8 units";
        Cost_title.text = "500G";
        Tower_Description.text = "A seemingly out of place high-spec tower capable of rotating its turret and piercing through tough enemies from a long distance with powerful attacks. Great for filling in spaces that are too far for the other towers.";
        TowerImage.sprite = MegaTowerSprite;
    }
    public void SpikeTower()
    {
        Name_title.text = "Toxin Tower";
        Attack_title.text = "16";
        ATTSpeed_title.text = "0.5";
        Range_title.text = "0.5";
        Cost_title.text = "10G";
        Tower_Description.text = "Basic single target tower.";
        TowerImage.sprite = ToxinTowerSprite;
    }

    public void Tower_Back_Button()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}
