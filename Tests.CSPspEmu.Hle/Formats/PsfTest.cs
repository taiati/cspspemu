﻿using CSPspEmu.Hle.Formats;
using NUnit.Framework;
using System;
using System.IO;

namespace CSPspEmu.Core.Tests
{
	[TestFixture]
	public class PsfTest
	{
		[Test]
		public void LoadTest()
		{
			var Psf = new Psf();
			Psf.Load(File.OpenRead("../../../TestInput/PARAM.SFO"));
			foreach (var Pair in Psf.EntryDictionary)
			{
				Console.WriteLine("{0}:{1}", Pair.Key, Pair.Value);
			}

			Assert.AreEqual(Psf.EntryDictionary["BOOTABLE"], 1);
			Assert.AreEqual(Psf.EntryDictionary["CATEGORY"], "UG");
			Assert.AreEqual(Psf.EntryDictionary["DISC_ID"], "TEST99999");
			Assert.AreEqual(Psf.EntryDictionary["DISC_NUMBER"], 1);
			Assert.AreEqual(Psf.EntryDictionary["DISC_TOTAL"], 1);
			Assert.AreEqual(Psf.EntryDictionary["DISC_VERSION"], "9.99");
			Assert.AreEqual(Psf.EntryDictionary["PARENTAL_LEVEL"], 5);
			Assert.AreEqual(Psf.EntryDictionary["PSP_SYSTEM_VER"], "3.33");
			Assert.AreEqual(Psf.EntryDictionary["REGION"], 32768);
			Assert.AreEqual(Psf.EntryDictionary["TITLE"], "GAME TITLE TITLE");
		}
	}
}