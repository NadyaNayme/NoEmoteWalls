using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System;

namespace NoEmoteWalls
{
    public sealed class NoEmoteWalls : IDalamudPlugin
    {
        public string Name => "No Emote Walls";

        private const string commandName = "/noemote";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ChatGui ChatGui { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }

        public NoEmoteWalls(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ChatGui chatGui,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ChatGui = chatGui;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            ChatGui.ChatMessage += OnChat;

            this.PluginUi = new PluginUI(this.Configuration);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open settings"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(commandName);
            this.ChatGui.CheckMessageHandled -= this.OnChat;
        }

        private void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!Configuration.Enabled)
            {
                return;
            }

            isHandled |= KillEmoteLines(type, message.TextValue);
        }

        public bool KillEmoteLines(XivChatType type, string input)
        {
            string targetPattern = @"[a-zA-Z\'\-]{2,15} [a-zA-Z\'\-]{2,45} ((stretches next|succumbs|nods|bids farewell|bows courteously|motions joyfully|waves) to|(spars|disagrees|bumps fists|agrees wholeheartedly|chats|is frustrated) with|(points|nods|smiles|winks) at|((flexes (his|her) muscles)|claps|hums a playful tune) for|(does the side step|weeps in sorrow|staggers|wrings (his|her) hands obsequiously|prays|cringes with pain|does the box step|does the ""get fantasy""|swipes excitedly at (his|her) tomestone) before|psychs (herself|himself) up alongside|snacks on a slice of pizza in front of|(is visibly infuriated|is taken aback) by|casts Megaflare\.|celebrates victory|sweeps up around|falls asleep beside|experiences a brief moment of enlightenment upon seeing|dances sprightly|gently pats|encourages|sees|congratulates|questions|slaps|offers|pokes|plays|bows|gazes|looks (at|away from)|seems|performs|laughs|embraces|controls|consoles|bursts|gives|tries|cheers|blows|dotes|toasts|shows) (?!you|your)";
            string noTargetPattern = @"[a-zA-Z\'\-]{2,15} [a-zA-Z\'\-]{2,45} snaps (his|her) fingers|is uncontainably jubilant|is charmed|strikes a most gentlemanly pose|relaxes (his|her) pose|wipes (his|her) brow|keeps a watchful eye over (his|her) surroundings|buries (his|her) face in disbelief|motions joyfully|scatters coins about the area|lets out a cheer|dances happily|pats the air|bids farewell|performs an exotic Near Eastern dance|offers a moment of silence|readies for battle|raises (his|her) hand in greeting|cannot contain (his|her) feelings of affection|laughs|claps|begins explaining|hums playfully|kneels respectfully|staggers|bows";
            try
            {
                bool isEmoteType = type is XivChatType.CustomEmote or XivChatType.StandardEmote;
                if (isEmoteType)
                {
                    if (Configuration.filterTargetPattern)
                    {
                        foreach (Match match in Regex.Matches(input, targetPattern))
                        {
                            return true;
                        }
                    }
                    if (Configuration.filterNoTargetPattern)
                    {
                        foreach (Match match in Regex.Matches(input, noTargetPattern))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }
       

        private void OnCommand(string command, string args)
        {
            this.PluginUi.Visible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            this.PluginUi.SettingsVisible = true;
        }
    }
}
