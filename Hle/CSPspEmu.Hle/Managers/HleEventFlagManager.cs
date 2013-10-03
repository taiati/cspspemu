﻿using CSPspEmu.Hle.Threading.EventFlags;
using CSPspEmu.Core;

namespace CSPspEmu.Hle.Managers
{
	public enum EventFlagId : int { }

	public class HleEventFlagManager
	{
		public HleUidPoolSpecial<HleEventFlag, EventFlagId> EventFlags = new HleUidPoolSpecial<HleEventFlag, EventFlagId>();
	}
}
