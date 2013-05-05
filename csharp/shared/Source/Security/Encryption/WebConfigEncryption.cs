using System.Web;
using System.Web.Configuration;
using System.Configuration;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// Utility class for encrypting and decrypting the web.config file
	/// </summary>
	public static class WebConfigEncryption
	{
		/// <summary>
		/// Encrypt a section of the web.config
		/// </summary>
		/// <param name="sectionToEncrypt">Name of the section to encrypt</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>
		public static bool EncryptSection(string sectionToEncrypt)
		{
			bool result = false;

			Configuration config = WebConfigurationManager.OpenWebConfiguration(
				HttpContext.Current.Request.ApplicationPath);
			ConfigurationSection section = config.GetSection(sectionToEncrypt);
			
			if (!section.SectionInformation.IsProtected && !section.ElementInformation.IsLocked)
			{
				section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
				config.Save(ConfigurationSaveMode.Full);
				result = true;
			}

			return result;
		}
	
		/// <summary>
		/// Decrypt a section of the web.config
		/// </summary>
		/// <param name="sectionToEncrypt">Name of the section to decrypt</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>
		public static bool DecryptSection(string sectionToEncrypt)
		{
			bool result = false;

			Configuration config = WebConfigurationManager.OpenWebConfiguration(
				HttpContext.Current.Request.ApplicationPath);
			ConfigurationSection section = config.GetSection(sectionToEncrypt);
			
			if (!section.SectionInformation.IsProtected && !section.ElementInformation.IsLocked)
			{
				section.SectionInformation.UnprotectSection();
				config.Save(ConfigurationSaveMode.Full);
				result = true;
			}

			return result;
		}
	}
}
