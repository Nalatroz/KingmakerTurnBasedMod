﻿using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.SettingsUI;
using ModMaker;
using ModMaker.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static TurnBased.Main;
using static TurnBased.Utility.SettingsWrapper;

namespace TurnBased.Controllers
{
    public class HotkeyController : 
        IModEventHandler,
        ISceneHandler
    {
        private IDictionary<string, BindingKeysData> Hotkeys => Core.Settings.hotkeys;

        public IReadOnlyDictionary<string, BindingKeysData> GetHotkeysCopy()
        {
            return new ReadOnlyDictionary<string, BindingKeysData>
                (new Dictionary<string, BindingKeysData>(Core.Settings.hotkeys));
        }

        private void Initialize(Dictionary<string, BindingKeysData> hotkeys)
        {
            foreach (string name in Hotkeys.Keys.ToList())
                if (!hotkeys.ContainsKey(name))
                    Hotkeys.Remove(name);

            foreach (KeyValuePair<string, BindingKeysData> item in hotkeys)
                if (!Hotkeys.ContainsKey(item.Key))
                    Hotkeys.Add(item.Key, item.Value);
        }

        public void SetHotkey(string name, BindingKeysData value)
        {
            Hotkeys[name] = value;
            TryRegisterHotkey(name, value);
        }

        private void TryRegisterHotkey(string name, BindingKeysData value)
        {
            Core.Debug($"{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}({name}, {HotkeyHelper.GetKeyText(value)})");

            if (value != null)
            {
                HotkeyHelper.RegisterKey(name, value, KeyboardAccess.GameModesGroup.World);
            }
            else
            {
                HotkeyHelper.UnregisterKey(name);
            }
        }

        public void HandleModEnable()
        {
            Core.Debug($"{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()");

            Initialize(new Dictionary<string, BindingKeysData>()
            {
                {HOTKEY_FOR_TOGGLE_MODE, new BindingKeysData() { IsAltDown = true, Key = KeyCode.T } },
                {HOTKEY_FOR_TOGGLE_MOVEMENT_INDICATOR, new BindingKeysData() { IsAltDown = true, Key = KeyCode.R }},
                {HOTKEY_FOR_TOGGLE_ATTACK_INDICATOR, new BindingKeysData() { IsAltDown = true, Key = KeyCode.R }},
                {HOTKEY_FOR_FIVE_FOOT_STEP, new BindingKeysData() { IsAltDown = true, Key = KeyCode.F } },
                {HOTKEY_FOR_DELAY, new BindingKeysData() { IsAltDown = true, Key = KeyCode.D }},
                {HOTKEY_FOR_END_TURN, new BindingKeysData() { IsAltDown = true, Key = KeyCode.E } },
            });

            Core.Mod.HotkeyController = this;
            EventBus.Subscribe(this);

            foreach (KeyValuePair<string, BindingKeysData> item in Hotkeys)
                TryRegisterHotkey(item.Key, item.Value);
        }

        public void HandleModDisable()
        {
            Core.Debug($"{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()");

            Core.Mod.HotkeyController = null;
            EventBus.Unsubscribe(this);

            foreach (string name in Hotkeys.Keys)
                TryRegisterHotkey(name, null);
        }

        public void OnAreaBeginUnloading() { }

        public void OnAreaDidLoad()
        {
            Core.Debug($"{MethodBase.GetCurrentMethod().DeclaringType.Name}.{MethodBase.GetCurrentMethod().Name}()");

            foreach (KeyValuePair<string, BindingKeysData> item in Hotkeys)
                TryRegisterHotkey(item.Key, item.Value);
        }
    }
}
