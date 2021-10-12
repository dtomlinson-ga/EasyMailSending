// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;

namespace EasyMailSending
{
	public class ContentPatcherHelper
	{
		internal static List<ContentPackData> Packs;
		internal static IContentPatcherAPI api = null;
		internal static List<MailFlag> mailFlags;

		static ContentPatcherHelper()
		{
			mailFlags = new();
		}

		internal static bool TryLoadContentPatcherAPI()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
				{
					Globals.Monitor.Log("Content Patcher not present");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log($"Failed to register ContentPatcher API: {e.Message}", LogLevel.Error);
				return false;
			}
		}

		internal static void ProcessDailyUpdates()
		{
			foreach (MailFlag mail in mailFlags)
			{
				mail.ManagedConditions.UpdateContext();

				if (mail.ManagedConditions.IsMatch)
				{
					if (mail.ReadyToApply())
					{
						MailHelper.ApplyMailFlag(mail);
						mail.DateApplied = SDate.Now();
					}
				}
			}

		}

		/// <summary>
		/// Adds all config values as tokens to ContentPatcher so that they can be referenced dynamically by patches
		/// </summary>
		internal static void RegisterContentPacks()
		{
			if (api == null)
			{
				Globals.Monitor.Log("ContentPatcher API not present - not registering content packs.", LogLevel.Warn);
				return;
			}

			foreach (IContentPack contentPack in Globals.Helper.ContentPacks.GetOwned())
			{
				Globals.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}");

				if (!contentPack.HasFile("content.json"))
				{
					Globals.Monitor.Log($"Content pack has no 'content.json': {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Warn);
					Globals.Monitor.Log($"Skipping content pack {contentPack.Manifest.Name}", LogLevel.Warn);
					continue;
				}

				ContentPackData packData = contentPack.ReadJsonFile<ContentPackData>("content.json");

				if (packData.MailFlags.Count == 0)
				{
					Globals.Monitor.Log($"Content pack contains no mail flags: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Warn);
					Globals.Monitor.Log($"Skipping content pack {contentPack.Manifest.Name}", LogLevel.Warn);
				}

				foreach (MailFlag mail in packData.MailFlags)
				{
					try
					{
						mail.ContentPack = packData; 
						mail.Initialize();
					}
					catch (InvalidMailFlagException)
					{
						string name = mail.Name is null ? "Unnamed Mail Flag" : "Mail Flag '" + mail.Name + "'";
						Globals.Monitor.Log($"{name} in content pack does not contain a valid ID: {contentPack.Manifest.Name} {contentPack.Manifest.Version}.", LogLevel.Error);
						Globals.Monitor.Log($"Skipping {name}", LogLevel.Error);
						continue;
					}
					catch (Exception e)
					{
						string name = mail.Name is null ? "Unnamed Mail Flag" : "Mail Flag '" + mail.Name + "'"; 
						Globals.Monitor.Log($"Encountered exception while parsing {name} in content pack {contentPack.Manifest.Name} {contentPack.Manifest.Version}:\n{e}", LogLevel.Error);
						Globals.Monitor.Log($"Skipping {name}", LogLevel.Error);
						continue;
					}

					if (!mail.ManagedConditions.IsValid)
					{
						Globals.Monitor.Log($"Invalid Conditions provided by {mail.Name} in content pack {contentPack.Manifest.Name} {contentPack.Manifest.Version}: {mail.ManagedConditions.ValidationError}", LogLevel.Error);
						Globals.Monitor.Log($"Skipping {mail.Name}", LogLevel.Error);
						continue;
					}

					mailFlags.Add(mail);
				}
			}

			foreach (MailFlag mail in mailFlags)
			{
				Globals.Monitor.Log($"Parsed '{mail.Name}' successfully");
			}

		}

		/// <summary>
		/// Adds all config values as tokens to ContentPatcher so that they can be referenced dynamically by patches
		/// </summary>
		public static void RegisterAdvancedTokens()
		{
			if (api == null) return;

		}
	}
}
