using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Utils
{
	private static Random random = new Random();

	public static float ConvergeValue(float value, float target, float increment)
	{
		// Move value towards target in steps of size increment.
		// If increment is negative can also be used to do the opposite

		float difference = value - target;
		if (Mathf.Abs(difference) < increment) return target;
		else return value + -Mathf.Sign(difference) * increment;
	}

	public static T RandomElement<T>(IList<T> data)
	{
		// Select a random element from a list
		return data[random.Next(data.Count)];
	}

	public static T RandomElementWeighted<T>(IDictionary<T, float> dataAndWeights)
	{
		// Select a random element from dictionary.
		// Key of dictionary is element, value of dictionary is relative probability of selection
		// If dictionary is empty returns null

		float randValue = (float) GD.RandRange(0, dataAndWeights.Sum(x => x.Value));
		float runningValue = 0;
		foreach (var keyValuePair in dataAndWeights)
		{
			if (randValue < runningValue + keyValuePair.Value) return keyValuePair.Key;
			runningValue += keyValuePair.Value;
		}
		return dataAndWeights.Keys.LastOrDefault();
	}
}
