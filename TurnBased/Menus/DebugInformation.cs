﻿#if DEBUG
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Utility;
using ModMaker;
using ModMaker.Utility;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Controllers;
using TurnBased.Utility;
using UnityEngine;
using UnityModManagerNet;
using static ModMaker.Utility.RichTextExtensions;
using static TurnBased.Main;

namespace TurnBased.Menus
{
    public class DebugInformation : Menu.IToggleablePage
    {
        public string Name => "Debug";

        public int Priority => 900;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Core == null || !Core.Enabled)
                return;

            if (GUILayout.Button("Clear HUD", GUILayout.ExpandWidth(false)))
            {
                HUDController.Clear();
            }

            GUILayout.Space(10f);

            OnGUIDebug();

            GUILayout.Space(10f);

            GUILayout.Label($"Ability Execution Process: {Core.Mod?.LastTickTimeOfAbilityExecutionProcess.Count}");
        }

        private void OnGUIDebug()
        {
            GUILayout.Label($"Time Scale: {Time.timeScale:f2}x");
            GUILayout.Label($"Game Time: {Game.Instance.Player.GameTime}");

            RoundController roundController = Core.Mod.RoundController;
            if (roundController != null)
            {
                TurnController currentTurn = roundController.CurrentTurn;

                GUILayout.Space(10f);
                GUILayout.Label($"Combat Initialized: {roundController.CombatInitialized}");
                GUILayout.Label($"Game Time: {roundController.GetGameTime()} (Game Time Calculated By Mod)");

                GUILayout.Space(10f);
                if (GUILayout.Button("Reset Turn", GUILayout.ExpandWidth(false)) && currentTurn != null)
                {
                    roundController.InitTurn(currentTurn.Unit);
                }

                GUILayout.Space(10f);
                GUILayout.Label($"Turn Status: {currentTurn?.Status}");
                GUILayout.Label($"Time Waited For AI: {currentTurn?.TimeWaitedForIdleAI:f2}");
                GUILayout.Label($"Time Waited To End Turn: {currentTurn?.TimeWaitedToEndTurn:f2}");
                GUILayout.Label($"Time Moved: {currentTurn?.TimeMoved:f2}");
                GUILayout.Label($"Meters Moved (5-Foot Step): {currentTurn?.MetersMovedByFiveFootStep:f2}");
                GUILayout.Label($"Feet Moved (5-Foot Step): {currentTurn?.MetersMovedByFiveFootStep / Feet.FeetToMetersRatio:f2}");
                GUILayout.Label($"Has Normal Movement: {currentTurn?.HasNormalMovement()}");
                GUILayout.Label($"Has 5-Foot Step: {currentTurn?.HasFiveFootStep()}");
                GUILayout.Label($"Has Free Touch: {currentTurn?.Unit.HasFreeTouch()}");
                GUILayout.Label($"Prepared Spell Combat: {currentTurn?.Unit.PreparedSpellCombat()}");
                GUILayout.Label($"Prepared Spell Strike: {currentTurn?.Unit.PreparedSpellStrike()}");

                GUILayout.Space(10f);
                GUILayout.Label("Current Unit:");
                GUILayout.Label(currentTurn?.Unit.ToString().Color(RGBA.yellow));
                GUILayout.Label($"Free Action: {currentTurn?.Commands.Raw[0]}");
                GUILayout.Label($"Standard Action: {currentTurn?.Commands.Standard}" +
                    $" (IsFullRoundAction: {currentTurn?.Commands.Raw[1].IsFullRoundAction()}" +
                    $", IsFreeTouch: {currentTurn?.Commands.Raw[1].IsFreeTouch()}" +
                    $", IsSpellCombat: {currentTurn?.Commands.Raw[1].IsSpellCombat()}" +
                    $", IsSpellStrike: {currentTurn?.Commands.Raw[1].IsSpellStrike()})");
                GUILayout.Label($"Move Action: {currentTurn?.Commands.Raw[3]}");
                GUILayout.Label($"Swift Action: {currentTurn?.Commands.Raw[2]}");

                List<UnitEntityData> units = roundController.GetSortedUnitsInCombat().ToList();

                if (units.Count > 0)
                {
                    GUILayout.Space(10f);
                    using (new GUILayout.HorizontalScope())
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Unit ID");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label($"{unit}");
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Name");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label($"{unit.CharacterName}");
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Init");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(unit.CombatState.Initiative.ToString());
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Init_C");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.CombatState.Cooldown.Initiative));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Std_C");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.CombatState.Cooldown.StandardAction));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Move_C");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.CombatState.Cooldown.MoveAction));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Swift_C");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.CombatState.Cooldown.SwiftAction));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("AoO_C");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.CombatState.Cooldown.AttackOfOpportunity));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("AoO");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(unit.CombatState.AttackOfOpportunityCount.ToString());
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("TTNR");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(HightlightedCooldownText(unit.GetTimeToNextTurn()));
                        }

                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("CanAct");
                            foreach (UnitEntityData unit in units)
                                GUILayout.Label(unit.CanPerformAction().ToString());
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        private string HightlightedCooldownText(float cooldown)
        {
            if (cooldown > 0)
                return $"{cooldown:F4}".Color(RGBA.yellow);
            else
                return $"{cooldown:F4}";
        }
    }
}
#endif