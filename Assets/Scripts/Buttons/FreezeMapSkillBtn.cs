using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreezeMapSkillBtn : MonoBehaviour
{
    private GoldSystem goldSystem;
    private Button button;
    private bool isOnCooldown = false;
    [SerializeField] private AudioClip freezeClip;
    [SerializeField] private float startTime = 0f;
    private AudioSource freezeAudio;
    [SerializeField] private int skillCost = 50;
    [SerializeField] private float freezeDuration = 3f;
    [SerializeField] private float cooldownDuration = 60f; // 1 minute
    private float cooldownTimer = 0f;
    public TMP_Text cooldownText; // Reference to the TMP Text component

    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.AddListener(OnClick);

        if (goldSystem == null)
        {
            goldSystem = FindFirstObjectByType<GoldSystem>();
        }
    }

    private void OnClick()
    {
        if (!isOnCooldown)
        {
            if (goldSystem.SpendGold(skillCost)) // Check if player has enough gold
            {
                SoundEffect();
                FreezeAllEnemies(); // Freeze enemies
                StartCooldown(); // Start cooldown
            }
            else
            {
                Debug.LogWarning("Not enough gold to use skill.");
            }
        }
        else
        {
            Debug.LogWarning("Skill is on cooldown.");
        }
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString(); // Update TMP text to show the cooldown value

            if (cooldownTimer <= 0f)
            {
                EndCooldown();
            }
        }
    }
    
    private void FreezeAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            AgentBehavior agentBehavior = enemy.GetComponent<AgentBehavior>();
            if (agentBehavior != null)
            {
                agentBehavior.ApplyFreeze(freezeDuration);
            }
            else
            {
                //none
            }
            FlyingSlimeBehavior flyingSlimeBehavior = enemy.GetComponent<FlyingSlimeBehavior>();
            if (flyingSlimeBehavior != null)
            {
                flyingSlimeBehavior.ApplyFreeze(freezeDuration);
            }
            else
            {
                //none
            }
        }
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;
        button.interactable = false;
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
        button.interactable = true; 
        cooldownText.text = ""; 
    }

    private void SoundEffect()
    {
        freezeAudio = GetComponent<AudioSource>();

        if (freezeAudio != null && freezeClip != null)
        {
            freezeAudio.clip = freezeClip;
            freezeAudio.time = startTime;
            freezeAudio.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or hitSound is not assigned. Destroying object immediately.");
            Destroy(gameObject);
        }
    }
}
