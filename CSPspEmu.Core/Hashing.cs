﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPspEmu.Core
{
	unsafe public sealed class Hashing
	{
		static public uint FastHash(uint* Pointer, int Count, uint StartHash = 0)
		{
			if (Pointer == null)
			{
				return StartHash;
			}

			var CountInBlocks = Count / 4;
			var Hash = StartHash;

			try
			{
				if (CountInBlocks > 2048 * 2048)
				{
					Console.Error.WriteLine("FastHash too big count!");
				}
				else
				{
					for (int n = 0; n < CountInBlocks; n++)
					{
						Hash ^= (uint)(Pointer[n] + (n << 16));
					}
				}
			}
			catch
			{
			}

			return Hash;
		}
	}
}
