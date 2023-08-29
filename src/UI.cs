using HarmonyLib;
using Reptile;
using TMPro;
using UnityEngine;

namespace moe.taw.BRC.FlyMode;

public class UI : MonoBehaviour
{
    public static UI Instance = null!;
    private TextMeshProUGUI m_label = null!;
    private float m_notificationTimer = 5f;
    private bool m_active = false;

    private void Awake()
    {
        Instance = this;

        SetupLabel();
    }

    private void FixedUpdate()
    {
        UpdateTimer();
    }

    public void ShowNotification(string text)
    {
        m_label.text = text;
        m_notificationTimer = 5f;
        
        if (!m_active)
        {
            m_active = true;
            m_label.gameObject.SetActive(true);
        }
    }

    private void UpdateTimer()
    {
        if (m_notificationTimer < 0f && m_active)
        {
            m_notificationTimer = 5f;
            m_active = false;
            m_label.gameObject.SetActive(false);
            m_label.text = "";
        }

        if (m_active)
        {
            m_notificationTimer -= Time.fixedDeltaTime;
        }
    }

    private void SetupLabel()
    {
        m_label = new GameObject("FlyMode_Notification").AddComponent<TextMeshProUGUI>();
        
        var uiManager = Core.Instance.UIManager;
        var gameplay = Traverse.Create(uiManager).Field<GameplayUI>("gameplay").Value;
        var rep = gameplay.graffitiNewLabel;
        m_label.font = rep.font;
        m_label.fontSize = 20;
        m_label.fontMaterial = rep.fontMaterial;
        m_label.alignment = TextAlignmentOptions.TopLeft;
        var rect = m_label.rectTransform;
        rect.anchorMin = new Vector2(0.1f, 0.5f);
        rect.anchorMax = new Vector2(0.6f, 0.1f);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(0.2f, 0.2f);
        m_label.rectTransform.SetParent(gameplay.gameplayScreen.GetComponent<RectTransform>(), false);
    }
}