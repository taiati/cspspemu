﻿using System.IO;
using CSPspEmu.Hle.Formats;
using CSPspEmu.Hle.Vfs.Iso;
using NUnit.Framework;

namespace CSPspEmu.Core.Tests
{
	[TestFixture]
	public class IsoTest
	{
		[Test]
		public void IsoConstructorTest()
		{
			var CsoName = "../../../TestInput/test.cso";
			var Cso = new Cso(File.OpenRead(CsoName));
			var Iso = new IsoFile(new CompressedIsoProxyStream(Cso), CsoName);
			var ContentNode = Iso.Root.Locate("path/content.txt");
			var Lines = ContentNode.Open().ReadAllContentsAsString().Split('\n');
			foreach (var Line in Lines)
			{
				Iso.Root.Locate(Line);
			}
		}
	}
}