using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Text;

namespace SpaceHunter;

public static class SpriteSheetTools
{
	/// <summary>
	/// Calculates the texture coordinates for a sprite inside a sprite sheet.
	/// </summary>
	/// <param name="spriteId">The sprite number. Starts with 0 in the upper left corner and increase in western reading direction up to #sprites - 1.</param>
	/// <param name="columns">Number of sprites per row.</param>
	/// <param name="rows">Number of sprites per column.</param>
	/// <returns>Texture2D coordinates for a single sprite</returns>
	public static Box2 CalcTexCoords(uint spriteId, uint columns, uint rows)
	{
		var result = new Box2(0f, 0f, 1f, 1f);
		uint row = 1;
		uint col = spriteId % columns;

		float x = col / (float)columns;
		float y = 0;
		float width = 1f / columns;
		float height = 1;

		result = new Box2(x, y, x + width, height);
		return result;
	}

	public static IEnumerable<uint> StringToSpriteIds(string text, uint firstCharacter)
	{
		byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
		foreach (var asciiCharacter in asciiBytes)
		{
			yield return asciiCharacter - firstCharacter;
		}
	}
}
