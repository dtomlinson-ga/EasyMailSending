﻿// Copyright (C) 2021 Vertigon
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

namespace EasyMailSending
{
	internal class Globals
	{
		public static AssetEditor AssetEditor { get; set; }
		public static IManifest Manifest { get; set; }
		public static ModConfig Config { get; set; }
		public static IModHelper Helper { get; set; }
		public static IMonitor Monitor { get; set; }
		public static ContentPatcherHelper PackHelper { get; set; }
	}
}
