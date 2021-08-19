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

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;

namespace $safeprojectname$
{
	public class HarmonyPatchTemplateClass
	{

		/// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
		public static bool ApplyHarmonyPatches()
		{
			try
			{
				harmony harmony = new harmony(Globals.Manifest.UniqueID);

				// insert patching code

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log(e.ToString(), LogLevel.Error);
				return false;
			}
		}
	}
}
