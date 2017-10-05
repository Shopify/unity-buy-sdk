namespace Helpers
{
	public static class StringHelper
	{
		public static string Ellipsisize(string input, int maxCharacters = 120)
		{
			if (input.Length <= maxCharacters) {
				return input;
			}

			return input.Substring (0, maxCharacters).Trim() + "...";
		}
	}
}