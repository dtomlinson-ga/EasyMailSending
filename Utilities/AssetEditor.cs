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
using System.Collections.Generic;
using System.Linq;

namespace EasyMailSending
{
	internal class AssetEditor : IAssetEditor
	{

		internal static Dictionary<string, string> mailToAdd = new();

		/// <summary>
		/// Attempts to load mod assets.
		/// </summary>
		/// <returns><c>True</c> if successful, <c>False</c> otherwise.</returns>
		public static bool LoadAssets()
		{
			return false;
		}

		/// <summary>
		/// Utilized by SMAPI to determine whether an asset should be edited.
		/// </summary>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Data/Mail");
		}

		/// <summary>
		/// Utilized by SMAPI to determine what edits should be made to an asset.
		/// </summary>
		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("Data/Mail"))
			{
				foreach (KeyValuePair<string, string> mail in mailToAdd.ToList())
				{
					asset.AsDictionary<string, string>().Data.Add(mail.Key, mail.Value);
				}
			}
		}

		/// <summary>
		/// This prevents cached values from being used if the player has changed config options.
		/// </summary>
		public static void InvalidateCache()
		{
			Globals.Helper.Content.InvalidateCache("Data/Mail");
			MailHelper.RefreshMailData();
		}

	}
}
