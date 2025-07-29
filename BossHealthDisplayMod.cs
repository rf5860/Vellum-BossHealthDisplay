using MelonLoader;
using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Reflection;
using AccessTools = HarmonyLib.AccessTools;
using static BossHealthDisplay.BossHealthDisplayMod;

// ReSharper disable InconsistentNaming, UnusedMember.Local, ArrangeTypeMemberModifiers
namespace BossHealthDisplay
{
    public class BossHealthDisplayMod : MelonMod
    {
        internal static TextMeshProUGUI healthText;
        internal static TextMeshProUGUI shieldText;
        internal static GameObject healthTextObject;
        internal static GameObject shieldTextObject;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Boss Health Display mod loaded!");
        }
        
        internal static AIControl GetBoss(BossHealthbar __instance)
        {
            FieldInfo bossField = AccessTools.Field(typeof(BossHealthbar), "boss");
            AIControl boss = (AIControl)bossField.GetValue(__instance);
            return boss;
        }
    }

    [HarmonyPatch(typeof(BossHealthbar), "Setup")]
    public class BossHealthbar_Setup_Patch
    {
        static void Postfix(BossHealthbar __instance, EntityControl entity)
        {
            if (healthTextObject == null)
            {
                healthTextObject = new GameObject("HealthText");
                healthTextObject.transform.SetParent(__instance.HealthbarMain.transform.parent, false);

                healthText = healthTextObject.AddComponent<TextMeshProUGUI>();
                healthText.text = "";
                healthText.fontSize = 20;
                healthText.color = new Color(0.9f, 0.5f, 0.3f);
                healthText.alignment = TextAlignmentOptions.Center;
                healthText.fontStyle = FontStyles.Bold;
                
                healthText.outlineWidth = 0.3f;
                healthText.outlineColor = new Color32(0, 0, 0, 255);

                RectTransform rectTransform = healthTextObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(200, 30);   
                
                healthTextObject.transform.SetAsLastSibling();
            }

            if (shieldTextObject == null && __instance.ShieldBar != null)
            {
                shieldTextObject = new GameObject("ShieldText");
                shieldTextObject.transform.SetParent(__instance.ShieldBar.transform.parent, false);

                shieldText = shieldTextObject.AddComponent<TextMeshProUGUI>();
                shieldText.text = "";
                shieldText.fontSize = 18;
                shieldText.color = new Color(0.5f, 0.8f, 1f);
                shieldText.alignment = TextAlignmentOptions.Center;
                shieldText.fontStyle = FontStyles.Bold;
            
                shieldText.outlineWidth = 0.3f;
                shieldText.outlineColor = new Color32(0, 0, 0, 255);

                RectTransform rectTransform = shieldTextObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(200, 30);
                
                shieldTextObject.transform.SetAsLastSibling();
            }
        }
    }

    [HarmonyPatch(typeof(BossHealthbar), "UpdateHealthbar")]
    public class BossHealthbar_UpdateHealthbar_Patch
    {
        static void Postfix(BossHealthbar __instance)
        {
            if (healthText == null) return;
            AIControl boss = GetBoss(__instance);

            if (boss == null || boss.health == null) return;
            int currentHealth = boss.health.health;
            int maxHealth = boss.health.MaxHealth;
            healthText.text = $"{currentHealth:N0} / {maxHealth:N0}";
        }
    }
    
    [HarmonyPatch(typeof(BossHealthbar), "UpdateShields")]
    public class BossHealthbar_UpdateShields_Patch
    {
        static void Postfix(BossHealthbar __instance)
        {
            if (shieldText == null) return;
            AIControl boss = GetBoss(__instance);

            if (boss?.health?.MaxShield > 0)
            {
                int currentShield = Mathf.CeilToInt(boss.health.shield);
                int maxShield = boss.health.MaxShield;
                shieldText.text = $"{currentShield:N0} / {maxShield:N0}";
            }
            else if (shieldText != null)
            {
                shieldText.text = "";
            }
        }
    }
}