using StardewModdingAPI;
using StardewValley;
using System;

namespace BasicSDVHarmonyPatchProjectTemplate
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private AssetEditor modAssetEditor;

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			SetUpGlobals(helper);
			SetUpEventHooks();
			SetUpConsoleCommands();
			SetUpPatches();
			SetUpAssets();
		}

		private void SetUpPatches()
		{
			Monitor.Log(HarmonyPatches.ApplyHarmonyPatches() ? "Patches successfully applied" : "Failed to apply patches");
		}
		private void SetUpAssets()
		{
			Monitor.Log(AssetEditor.LoadAssets() ? "Loaded assets" : "Failed to load assets");
		}

		private void SetUpEventHooks()
		{
			Globals.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
			{
				ModConfigMenuHelper.RegisterModOptions();
				SetUpAPIs();
			};
		}
		private void SetUpAPIs()
		{
			Monitor.Log(ContentPatcherHelper.TryLoadContentPatcherAPI() ? "Content Patcher API loaded" : "Failed to load Content Patcher API - ignoring extended content pack functionality");
			Monitor.Log(ModConfigMenuHelper.TryLoadModConfigMenu() ? "GMCM API loaded" : "Failed to load GMCM API - reverting to basic config.json functionality");
		}

		private void SetUpConsoleCommands()
		{

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
	}
}
