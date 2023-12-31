namespace moe.taw.BRC.FlyMode;

using System.Collections.Generic;
using HarmonyLib;
using Reptile;
using UnityEngine;

[HarmonyPatch(typeof(Player))]
public class PlayerPatch
{
    private static bool m_flyModeActive;
    private static bool m_playerModelActive = true;
    private static float m_flySpeed = 15f;
    private static List<Renderer> m_previouslyEnabled = new();
    
    [HarmonyPostfix]
    [HarmonyPatch("UpdatePlayer")]
    public static void UpdatePlayer(Player __instance)
    {
        var isAI = (bool)Traverse.Create(__instance).Field("isAI").GetValue();
        if (isAI) return;
        
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Changing FlyMode");
            
            m_flyModeActive = !m_flyModeActive;
            SetPlayerState(__instance);
        }

        if (m_flyModeActive)
        {
            UpdateFlymode(__instance);

            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.O))
            {
                TogglePlayerModelVisibility(__instance);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                // Faster
                m_flySpeed += 200f * Time.deltaTime;
                UI.Instance.ShowNotification($"Speed: {m_flySpeed}");
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f && m_flySpeed > 0.1f)
            {
                // Slower
                m_flySpeed -= 200f * Time.deltaTime;
                UI.Instance.ShowNotification($"Speed: {m_flySpeed}");
            }
        }
    }

    private static void SetPlayerState(Player __instance)
    {
        Debug.Log($"SetPlayerState: active={m_flyModeActive}");
        
        if (m_flyModeActive)
        {
            Debug.Log("Enabling FlyMode");
            
            __instance.CompletelyStop();
            __instance.SetCurrentMoveStyleEquipped(MoveStyle.ON_FOOT);
            __instance.motor.SetKinematic(true);

            var colliders = __instance.motor.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            // Lock abilities
            __instance.LockBoostAbility(true);
            __instance.LockBoostpack(true);
            __instance.LockPhone(true);
            __instance.LockSpraycan(true);
            __instance.LockCharacterSelect(true);
            __instance.LockSwitchToEquippedMoveStyle(true);
            
            UI.Instance.ShowNotification("FlyMode Activated");
        }
        else
        {
            Debug.Log("Disabling FlyMode");

            if (!m_playerModelActive)
            {
                TogglePlayerModelVisibility(__instance);
            }
            
            __instance.motor.SetKinematic(false);

            var colliders = __instance.motor.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
            
            // Restore abilities
            __instance.LockBoostAbility(false);
            __instance.LockBoostpack(false);
            __instance.LockPhone(false);
            __instance.LockSpraycan(false);
            __instance.LockCharacterSelect(false);
            __instance.LockSwitchToEquippedMoveStyle(false);
            
            UI.Instance.ShowNotification("FlyMode Deactivated");
        }
    }

    public static Vector3 CalculateMovement()
    {
        var movement = Vector3.zero;

        // Left/Right/Forward/Backward
        if (Input.GetKey(KeyCode.W)) movement.z += 1f;
        if (Input.GetKey(KeyCode.S)) movement.z -= 1f;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1f;
        if (Input.GetKey(KeyCode.D)) movement.x += 1f;
        
        // Movement from Camera perspective
        Vector3 normalized = Vector3.ProjectOnPlane(Camera.main.transform.rotation * Vector3.forward, Vector3.up).normalized;
        if (normalized.sqrMagnitude == 0.0)
        {
            normalized = Vector3.ProjectOnPlane(Camera.main.transform.rotation * Vector3.up, Vector3.up).normalized;
        }
        movement = Quaternion.LookRotation(normalized, Vector3.up) * movement;
        
        // Up/Down
        if (Input.GetKey(KeyCode.Space)) movement.y += 1f;
        if (Input.GetKey(KeyCode.LeftShift)) movement.y -= 1f;

        return movement;
    }
    
    public static void UpdateFlymode(Player __instance)
    {
        Vector3 movement = CalculateMovement();
        __instance.motor.transform.position += movement * m_flySpeed * Time.deltaTime;
    }
    
    // NOP'ing abilities when in flymode
    [HarmonyPrefix]
    [HarmonyPatch("ActivateAbility")]
    public static void ActivateAbility(Player __instance, Ability a)
    {
        if (m_flyModeActive) return;
    }

    public static void TogglePlayerModelVisibility(Player __instance)
    {
        m_playerModelActive = !m_playerModelActive;

        if (m_playerModelActive)
        {
            Debug.Log("Show PlayerModel");
            UI.Instance.ShowNotification("PlayerModel Visible");
            
            // Reenable all renderers who were previously enabled.
            foreach (var renderer in m_previouslyEnabled)
            {
                renderer.enabled = true;
            }

            m_previouslyEnabled.Clear();
        }
        else
        {
            Debug.Log("Hide PlayerModel");
            UI.Instance.ShowNotification("PlayerModel Hidden");
            
            // Disable all renderers on player and store a reference for reactivation
            m_previouslyEnabled.Clear();
            var renderers = __instance.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                // Skip disabled renderers
                if (!renderer.enabled) continue;
                
                renderer.enabled = false;
                m_previouslyEnabled.Add(renderer);
            }
        }
    }
}