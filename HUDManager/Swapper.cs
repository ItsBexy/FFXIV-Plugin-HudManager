﻿using Dalamud.Game;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;
using HUDManager.Configuration;
using System;
using System.Collections.Generic;

namespace HUD_Manager
{
    public class Swapper : IDisposable
    {
        private Plugin Plugin { get; }

        public bool SwapsTemporarilyDisabled = false;

        public Swapper(Plugin plugin)
        {
            this.Plugin = plugin;

            this.Plugin.Framework.Update += this.OnFrameworkUpdate;
            this.Plugin.ClientState.Login += this.OnLogin;
            this.Plugin.ClientState.TerritoryChanged += this.OnTerritoryChange;
        }

        public void Dispose()
        {
            this.Plugin.Framework.Update -= this.OnFrameworkUpdate;
            this.Plugin.ClientState.Login -= this.OnLogin;
            this.Plugin.ClientState.TerritoryChanged -= this.OnTerritoryChange;
        }

        public void OnLogin(object? sender, EventArgs e)
        {
            // Player object is null here, not much to do
        }

        public void OnTerritoryChange(object? sender, ushort tid)
        {
            this.Plugin.Statuses.Update();
            this.Plugin.Statuses.SetHudLayout();
        }

        public void OnFrameworkUpdate(Framework framework)
        {
            if (!this.Plugin.Config.SwapsEnabled || SwapsTemporarilyDisabled || !this.Plugin.Config.UnderstandsRisks) {
                return;
            }

            var player = this.Plugin.ClientState.LocalPlayer;
            if (player == null) {
                return;
            }

            if (Util.IsCharacterConfigOpen()) {
                return;
            }

            var updated = this.Plugin.Statuses.Update()
                || this.Plugin.Keybinder.UpdateKeyState()
                || this.Plugin.Statuses.CustomConditionStatus.IsUpdated();

            if (updated || this.Plugin.Statuses.NeedsForceUpdate) {
                this.Plugin.Statuses.SetHudLayout();
            }
        }
    }
}
