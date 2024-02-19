using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;

namespace ShadowRando.Core;

public static class Utils
{
	public static async Task<ButtonResult> ShowSimpleMessage(string title, string message, ButtonEnum messageType, Icon messageIcon)
	{
		var msgboxResult = MessageBoxManager.GetMessageBoxStandard(title, message, messageType, messageIcon);
		return await msgboxResult.ShowAsync();
	}
}
