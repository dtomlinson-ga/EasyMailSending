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

using StardewModdingAPI;
using System;

namespace BasicSDVHarmonyPatchProjectTemplate
{
	/// <summary>
	/// Collection of methods to simplify the process of adding Generic Mod Config Menu options.
	/// </summary>
	internal class ModConfigMenuHelper
	{

		private static IGenericModConfigMenuAPI api;

		/// <summary>
		/// Checks to see if GMCM is installed - if so, creates options page with all configurable settings.
		/// </summary>
		/// <returns> <c>True</c> if options page successfully created, <c>False</c> otherwise.</returns>
		public static bool TryLoadModConfigMenu()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				{
					Globals.Monitor.Log("GenericModConfigMenu not present - skipping mod menu setup");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
				api.RegisterModConfig(Globals.Manifest,
					() => Globals.Config = new ModConfig(),
					() => Globals.Helper.WriteConfig(Globals.Config)
				);

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log("Failed to register GMCM menu - skipping mod menu setup", LogLevel.Error);
				Globals.Monitor.Log(e.Message, LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Adds all descriptions and options to options page. Adds disclaimer if content packs are loaded that they will take priority over values set here.
		/// </summary>
		public static void RegisterModOptions()
		{
			
		}

		/// <summary>
		/// Shorthand method to create a Label.
		/// </summary>
		private static void AddLabel(string name, string desc = "")
		{
			api.RegisterLabel(Globals.Manifest, name, desc);
		}

		/// <summary>
		/// Shorthand method to create a Paragraph.
		/// </summary>
		private static void AddParagraph(string text)
		{
			api.RegisterParagraph(Globals.Manifest, text);
		}

		/// <summary>
		/// Shorthand method to create a Dropdown menu.
		/// </summary>
		private static void AddDropdown(string name, string desc, Func<string> get, Action<string> set, string[] choices)
		{
			api.RegisterChoiceOption(Globals.Manifest, name, desc, get, set, choices);
		}

		/// <summary>
		/// Shorthand method to create an Integer input field.
		/// </summary>
		private static void AddIntUnclamped(string name, string desc, Func<int> get, Action<int> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}

		/// <summary>
		/// Shorthand method to create an Integer slider.
		/// </summary>
		private static void AddIntClamped(string name, string desc, Func<int> get, Action<int> set, int min, int max)
		{
			api.RegisterClampedOption(Globals.Manifest, name, desc, get, set, min, max);
		}

		/// <summary>
		/// Shorthand method to create a checkbox.
		/// </summary>
		private static void AddCheckBox(string name, string desc, Func<bool> get, Action<bool> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}
	}
}
