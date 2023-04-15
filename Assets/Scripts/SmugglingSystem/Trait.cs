using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Trait
{
	public string traitName;
	public float rarity;
	[Range(0f, 10f)] public float susMultiplier;
	[Range(0f, 10f)] public float costMultiplier;
	[Range(0f, 10f)] public float sellMultiplier;
	[Range(0f, 10f)] public float escapeMultiplier;
	[Range(0f, 10f)] public float speedMultiplier;
}
