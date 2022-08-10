using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eternity.Math
{
	public static class Epoch
	{
		/// <summary>
		/// Returns current epoch time (unix timestamp)
		/// </summary>
		public static int Current
        {
			get { return (int)DateTime.UtcNow.Subtract(Epoch.epoch).TotalSeconds; }
		}
		/// <summary>
		/// Converts epoch time to date
		/// </summary>
		public static DateTime ToDateTime(decimal UnixTime)
        {
			return Epoch.epoch.AddSeconds((double)UnixTime);
		}
		/// <summary>
		/// Converts date to unix timestamp
		/// </summary>
		public static int FromDateTime(DateTime Time)
        {
			return (int)Time.Subtract(Epoch.epoch).TotalSeconds;
        }
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
