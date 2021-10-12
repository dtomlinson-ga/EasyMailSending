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
	internal class ContentPackData
	{
		public SemanticVersion SemanticVersion = new("1.23.0");
		public List<MailFlag> MailFlags { get; set; }
		internal IManifest Manifest { get; set; }
	}

	internal class MailFlag
	{
		public string Name;
		public string Frequency;
		public Dictionary<string, string> MailType = new(StringComparer.OrdinalIgnoreCase);
		public Dictionary<string, string> Conditions;

		internal SDate DateApplied;
		internal IManagedConditions ManagedConditions;
		internal ContentPackData ContentPack;

		internal void Initialize()
		{
			if (MailType is null || MailType["Id"] is null)
			{
				throw new InvalidMailFlagException();
			}

			// Frequency defaults to Once if not specified. Also, there is no reason for a mail flag to trigger more than once, as the flag will only be added once.
			if (Frequency is null or "" || !MailHelper.IsLetter(MailType))
			{
				Frequency = "Once";
			}

			ManagedConditions = ContentPatcherHelper.api.ParseConditions(
				Globals.Manifest,
				Conditions,
				ContentPack.SemanticVersion
			);

			if (Name is null or "")
			{
				Name = GenerateName();
			}

			DateApplied = null;
		}

		internal bool ReadyToApply()
		{
			if (DateApplied is null)
			{
				return true;
			}
			else
			{
				if (Frequency == "Once")
				{
					return false;
				}
				else
				{
					int DaysSinceLastApplied = Frequency switch
					{
						"Daily" => 1,
						"Weekly" => 7,
						"Monthly" => 28,
						"Yearly" => 112,
						_ => 0
					};

					return SDate.Now().DaysSinceStart - DateApplied.DaysSinceStart >= DaysSinceLastApplied;

				}
			}
		}

		private string GenerateName()
		{
			return $"{(MailHelper.IsLetter(MailType) ? "Send Letter" : "Apply Flag")} {MailType["Id"]} {Frequency} {(Conditions is null ? "Unconditionally" : "Conditionally")}";
		}
	}

	[Serializable]
	public class InvalidMailFlagException : Exception
	{
		public InvalidMailFlagException() : base() { }
		public InvalidMailFlagException(string message) : base(message) { }
		public InvalidMailFlagException(string message, Exception inner) : base(message, inner) { }
		protected InvalidMailFlagException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
