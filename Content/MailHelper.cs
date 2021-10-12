using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMailSending
{
	class MailHelper
	{
		internal static Dictionary<string, string> mailData;

		static MailHelper()
		{
			mailData = Globals.Helper.Content.Load<Dictionary<string, string>>("Data/Mail", ContentSource.GameContent);
		}

		internal static bool IsLetter(Dictionary<string, string> MailType)
		{
			return mailData.ContainsKey(MailType["Id"]) || MailType.ContainsKey("Message");
		}

		internal static void RefreshMailData()
		{
			mailData = Globals.Helper.Content.Load<Dictionary<string, string>>("Data/Mail", ContentSource.GameContent);
		}

		internal static void ApplyMailFlag(MailFlag mail)
		{
			// is mail flag - can be added directly to mailReceived
			if (!IsLetter(mail.MailType))
			{
				if (!Game1.player.mailReceived.Contains(mail.MailType["Id"]))
				{
					Game1.player.mailReceived.Add(mail.MailType["Id"]);
				}
			}

			// is letter - if not in "Data/Mail", need to add it
			// add to player's mailbox
			else
			{
				// it is possible to set letters to be received multiple times - they need to be removed from the mailbox in order to do so.
				if (Game1.player.mailReceived.Contains(mail.MailType["Id"]))
				{
					Game1.player.mailReceived.Remove(mail.MailType["Id"]);
				}

				if (!mailData.ContainsKey(mail.MailType["Id"]) && !AssetEditor.mailToAdd.ContainsKey(mail.MailType["Id"]))
				{
					AssetEditor.mailToAdd.Add(mail.MailType["Id"], mail.MailType["Message"]);
				}

				Game1.player.mailbox.Add(mail.MailType["Id"]);
			}
		}
	}
}
