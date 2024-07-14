using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerStatsUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text attackCooldownText;
    public TMP_Text projectileDamageText;
    public TMP_Text levelText;
    public TMP_Text goldText;
    public TMP_Text insufficientGoldText;
    public Image towerImage;
    public Button confirmButton;
    public TMP_Text upgradeCostText;
    public TMP_Text refundCostText;
    public Button destroyButton;

    private int currentUpgradeCost;
    private int currentRefundCost;

    void Start()
    {
        // Check if UI components are assigned
        if (panel == null || attackCooldownText == null || projectileDamageText == null ||
             levelText == null || goldText == null ||
            insufficientGoldText == null || towerImage == null || refundCostText == null ||
            confirmButton == null || destroyButton == null || upgradeCostText == null)
        {
            Debug.LogError("UI components are not assigned in the inspector.");
            return;
        }

        // Add listeners to buttons
        confirmButton.onClick.AddListener(OnConfirmUpgrade);
        destroyButton.onClick.AddListener(OnDestroyTower);
        HideUI(); // Hide panel initially
    }

    public void Show(TowerStats currentStats, TowerStats upgradedStats, Sprite towerSprite, int currentGold)
    {
        if (panel == null || attackCooldownText == null || upgradeCostText == null ||
            projectileDamageText == null || levelText == null ||
            goldText == null || insufficientGoldText == null || towerImage == null)
        {
            Debug.LogError("UI components are not assigned in the inspector.");
            return;
        }

        currentUpgradeCost = CalculateUpgradeCost(currentStats);
        currentRefundCost = CalculateRefundCost(currentStats);

        attackCooldownText.text = $"Attack Interval: {currentStats.AttackCooldown}" + "s";
        projectileDamageText.text = $"Damage: {currentStats.ProjectileDamage} -> {upgradedStats.ProjectileDamage}";
        upgradeCostText.text = $"Upgrade Cost: {currentUpgradeCost}G";
        levelText.text = $"Level: {currentStats.Level} -> {currentStats.Level + 1}";
        goldText.text = "Gold: " + currentGold;
        refundCostText.text = $"Refunds: {currentRefundCost}G";
        towerImage.sprite = towerSprite;

        bool hasEnoughGold = currentGold >= currentUpgradeCost;
        insufficientGoldText.gameObject.SetActive(!hasEnoughGold);
        confirmButton.interactable = hasEnoughGold;

        panel.SetActive(true);
    }

    public void HideUI()
    {
        panel.SetActive(false);
    }

    private int CalculateUpgradeCost(TowerStats stats)
    {
        return Mathf.RoundToInt(stats.BaseUpgradeCost * Mathf.Pow(stats.UpgradeCostMultiplier, stats.Level));
    }

    private int CalculateRefundCost(TowerStats stats)
    {
        return Mathf.RoundToInt((stats.BaseUpgradeCost * Mathf.Pow(stats.UpgradeCostMultiplier, stats.Level))*0.5f);;
    }
    private void OnConfirmUpgrade()
    {
        if (MouseClickSystem.instance != null)
        {
            MouseClickSystem.instance.ApplyUpgrade();
        }
        else
        {
            Debug.LogError("MouseClickSystem instance is not found.");
        }
    }

    private void OnDestroyTower()
    {
        if (MouseClickSystem.instance != null)
        {
            MouseClickSystem.instance.RefundTower();
            HideUI();
        }
        else
        {
            Debug.LogError("MouseClickSystem instance is not found.");
        }
    }
}
