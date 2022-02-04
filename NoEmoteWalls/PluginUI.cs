﻿using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace NoEmoteWalls
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
            
        }

        public void Draw()
        {
            DrawMainWindow();
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("NoEmoteWalls", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (ImGui.Button("Show Settings"))
                {
                    SettingsVisible = true;
                }
            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(400, 125), ImGuiCond.Always);
            if (ImGui.Begin("NoEmoteWalls Settings", ref this.settingsVisible,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                var enabled = this.configuration.Enabled;
                if (ImGui.Checkbox("Enable filtering.", ref enabled))
                {
                    this.configuration.Enabled = enabled;
                    this.configuration.Save();
                }

                var filterTargetPattern = this.configuration.filterTargetPattern;
                if (ImGui.Checkbox("Filter emotes with any target that isn't you.", ref filterTargetPattern))
                {
                    this.configuration.filterTargetPattern = filterTargetPattern;
                    this.configuration.Save();
                }
                ImGuiComponents.HelpMarker("Filters emotes that are not targeted at you. (eg. Some User sweeps around Another User)");

                var filterNoTargetPattern = this.configuration.filterNoTargetPattern;
                if (ImGui.Checkbox("Filters emotes that lack a target.", ref filterNoTargetPattern))
                {
                    this.configuration.filterNoTargetPattern = filterNoTargetPattern;
                    this.configuration.Save();
                }
                ImGuiComponents.HelpMarker("Filters text from emotes that aren't used by you and lack a target. (eg. Some User offers a moment of silence)");

            }
            ImGui.End();
        }
    }
}