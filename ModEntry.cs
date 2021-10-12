using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMailSending
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			SetUpGlobals(helper);
			SetUpEventHooks();
			SetUpConsoleCommands();
			SetUpAssets();
		}

		private void SetUpGlobals(IModHelper helper)
		{
			Globals.AssetEditor = new AssetEditor();
			helper.Content.AssetEditors.Add(Globals.AssetEditor);
			Globals.Config = helper.ReadConfig<ModConfig>();
			Globals.Helper = helper;
			Globals.Monitor = Monitor;
			Globals.Manifest = ModManifest;
			Globals.PackHelper = new ContentPatcherHelper();
		}

		private void SetUpEventHooks()
		{
			Globals.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
			{
				ModConfigMenuHelper.RegisterModOptions();
				SetUpAPIs();
			};

			Globals.Helper.Events.GameLoop.UpdateTicked += RegisterContentPacks;

			Globals.Helper.Events.GameLoop.DayStarted += (_, _) => ContentPatcherHelper.ProcessDailyUpdates();
		}
		
		private void RegisterContentPacks(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
		{
			// back out if CP API isn't ready yet (2 ticks after GameStarted event)
			if (!ContentPatcherHelper.api.IsConditionsApiReady)
			{ 
				return;
			}
			// if ready, register content packs and unsubscribe this from UpdateTicked
			else
			{
				ContentPatcherHelper.RegisterContentPacks();
				Globals.Helper.Events.GameLoop.UpdateTicked -= RegisterContentPacks;
			}
		}

		private void SetUpConsoleCommands()
		{
			Globals.Helper.ConsoleCommands.Add("ems_dumpmailflags", "Outputs a list of all mailReceived flags.", (_, _) =>
			{
				if (!Context.IsWorldReady)
				{
					Globals.Monitor.Log("This command should only be used in a loaded save.", LogLevel.Warn);
					return;
				}
				Globals.Monitor.Log(string.Join(", ", Game1.player.mailReceived), LogLevel.Info);
			});

			/* use patch export instead
			Globals.Helper.ConsoleCommands.Add("ems_dumpmailasset", "Outputs a list of entries in 'Data/Mail'.", (_, _) =>
			{
				Dictionary<string, string> mailAsset = Globals.Helper.Content.Load<Dictionary<string, string>>("Data/Mail", ContentSource.GameContent);

				Globals.Monitor.Log("\n" + string.Join("\n", mailAsset.Select(x => $"{x.Key}: {x.Value}")), LogLevel.Info);
			});
			*/

			Globals.Helper.ConsoleCommands.Add("ems_mailbox", "Outputs contents of game's mailbox object.", (_, _) =>
			{
				if (!Context.IsWorldReady)
				{
					Globals.Monitor.Log("This command should only be used in a loaded save.", LogLevel.Warn);
					return;
				}

				Globals.Monitor.Log(string.Join(", ", Game1.player.mailbox), LogLevel.Info);
			});

			Globals.Helper.ConsoleCommands.Add("ems_mail", "Outputs a list of parsed mail from content packs.", (_, _) =>
			{
				if (!Context.IsWorldReady)
				{
					Globals.Monitor.Log("This command should only be used in a loaded save.", LogLevel.Warn);
					return;
				}

				StringBuilder sb = new("\n");
				
				foreach(MailFlag mail in ContentPatcherHelper.mailFlags)
				{
					mail.ManagedConditions.UpdateContext();

					if (Game1.player.mailReceived.Contains(mail.MailType["Id"]))
					{
						sb.Append($"{mail.Name}: Already sent\n");
					}
					else if (mail.ManagedConditions.IsMatch)
					{
						sb.Append($"{mail.Name}: Ready to be sent\n");
					}
					else
					{
						sb.Append($"{mail.Name}: Not ready to be sent\n");
					}
				}

				sb.Length--;

				Globals.Monitor.Log(sb.ToString(), LogLevel.Info);
			});
		}

		private void SetUpAssets()
		{
			Monitor.Log(AssetEditor.LoadAssets() ? "Loaded assets" : "Failed to load assets");
		}

		private void SetUpAPIs()
		{
			Monitor.Log(ContentPatcherHelper.TryLoadContentPatcherAPI() ? "Content Patcher API loaded" : "Failed to load Content Patcher API - ignoring content pack functionality");
			//Monitor.Log(ModConfigMenuHelper.TryLoadModConfigMenu() ? "GMCM API loaded" : "Failed to load GMCM API - reverting to basic config.json functionality");
		}
	}
}
