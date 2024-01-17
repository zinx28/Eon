using DiscordRPC;
using DiscordRPC.Logging;
using Eon.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eon.Services.Launching
{
    public class RPC
    {
        public static DiscordRpcClient client;
        public RPC()
        {
        }

        public static void Init()
        {
            client = new DiscordRpcClient("1128641369066442752");
            client.Logger = new ConsoleLogger()
            {
                Level = LogLevel.Warning
            };

            client.OnReady += (sender, e) =>
            {
                Loggers.Log("RPC CONNECTED");
            };

            client.Initialize();

            Update("In Launcher");
        }

        public static void Update(string State)
        {
            Loggers.Log($"UPDATING PRENSENCE TO {State}");
            client.SetPresence(new RichPresence()
            {
                // Details = "Eon",
                Timestamps = Timestamps.Now,
#if DEVTESTING
                State = $"[DEV] {State}",
#else
State = State,
#endif
                Assets = new Assets()
                {
                    LargeImageKey = "bigimage",
                    LargeImageText = "Eon"
                },
                Buttons = new DiscordRPC.Button[]
               {
                    new DiscordRPC.Button()
                    {
                        Label = "Join Eon",
                        Url = "https://discord.gg/eonfn"
                    }
               }
            });
        }
    }
}
