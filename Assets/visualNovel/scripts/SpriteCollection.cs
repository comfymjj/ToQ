using UnityEngine;
using System.Collections;

public class SpriteCollection
{
	private Sprite[] sprites;
	private string[] names;
	
	public SpriteCollection(string spritesheet)
	{
		sprites = Resources.LoadAll<Sprite>(spritesheet);
		names = new string[sprites.Length];
		
		for(var i = 0; i < names.Length; i++)
		{
			names[i] = sprites[i].name;
		}
	}
	
	public Sprite GetSprite(string name)
	{
		return sprites[System.Array.IndexOf(names, name)];
	}
}
