﻿using System;
using System.Globalization;
using CSPspEmu.Core;
using CSPspEmu.Core.Rtc;
using CSPspEmu.Hle.Attributes;
using CSPspEmu.Hle.Vfs;
using CSharpUtils;

namespace CSPspEmu.Hle.Modules.rtc
{
	[HlePspModule(ModuleFlags = ModuleFlags.UserMode | ModuleFlags.Flags0x00010011)]
	public unsafe class sceRtc : HleModuleHost
	{
		[Inject]
		PspRtc PspRtc;

		/// <summary>
		/// Get the resolution of the tick counter
		/// </summary>
		/// <returns>Number of ticks per second</returns>
		[HlePspFunction(NID = 0xC41C2853, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public uint sceRtcGetTickResolution()
		{
			return (uint)TimeSpan.FromSeconds(1).GetTotalMicroseconds();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="DateTime"></param>
		/// <param name="UnixTime"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0x27C4594C, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcGetTime_t(ref ScePspDateTime DateTime, out uint UnixTime)
		{
			UnixTime = (uint)DateTime.ToUnixTimestamp();
			return 0;
		}

		/// <summary>
		/// Get current tick count (number of microseconds)
		/// </summary>
		/// <param name="Tick">pointer to u64 to receive tick count</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x3F7AD767, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public int sceRtcGetCurrentTick(long* Tick)
		{
			PspRtc.Update();
			*Tick = PspRtc.ElapsedTime.TotalMicroseconds;
			return 0;
		}

		Calendar Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

		/// <summary>
		/// Get number of days in a specific month
		/// </summary>
		/// <param name="Year">Year in which to check (accounts for leap year)</param>
		/// <param name="Month">Month to get number of days for</param>
		/// <returns># of days in month, less than 0 on error (?)</returns>
		[HlePspFunction(NID = 0x05EF322C, FirmwareVersion = 150)]
		public int sceRtcGetDaysInMonth(int Year, int Month)
		{
			return Calendar.GetDaysInMonth(Year, Month);
			//new DateTime(Year, Month, 1).
			//return Date(Year, Month, 1).daysInMonth;
		}

		/// <summary>
		/// Get day of the week for a date
		/// </summary>
		/// <param name="Year">Year in which to check (accounts for leap year)</param>
		/// <param name="Month">Month that day is in</param>
		/// <param name="Day">Day to get day of week for</param>
		/// <returns>Day of week with 1 representing Monday</returns>
		[HlePspFunction(NID = 0x57726BC1, FirmwareVersion = 150)]
		public PspDaysOfWeek sceRtcGetDayOfWeek(int Year, int Month, int Day)
		{
			var MonthTranslate = new int[] { 0, 3, 2, 5, 0, 3, 5, 1, 4, 6, 2, 4 };
			if (Month == 0)
			{
				Month = 8;
			}
			//if(Month > 12) // After month 12, psp months does 31/31/30/31/30 and repeat
			//{
			//	int restMonth = Month-12;
			//	int grp5 = restMonth / 5;
			//	restMonth = restMonth % 5;
			//	Day += grp5 * (31*3+30*2);
			//	int[] t = { 31, 31*2, 31*2+30, 31*3+30, 31*3+30*2 };
			//	Day += t[restMonth-1];
			//	Month = 12;
			//}
			int MonthMod = (Month - 1) % 12;
			if (MonthMod < 0) MonthMod += 12;
			Year -= (Month < 3) ? 1 : 0;
			return (PspDaysOfWeek)((Year + Year / 4 - Year / 100 + Year / 400 + MonthTranslate[MonthMod] + Day) % 7);

		}

		private static int _sceRtcTickAddTimeSpan(long* dstPtr, long* srcPtr, TimeSpan TimeSpan)
		{
			*dstPtr = (*srcPtr + TimeSpan.GetTotalMicroseconds());
			return 0;
		}

		/// <summary>
		/// Add two ticks
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of ticks to add</param>
		/// <returns>
		///		0 on success
		///		Less than 0 on error
		/// </returns>
		[HlePspFunction(NID = 0x44F45E05, FirmwareVersion = 150)]
		public int sceRtcTickAddTicks(out long dstPtr, ref long srcPtr, long value)
		{
			dstPtr = srcPtr + value;
			return 0;
		}

		/// <summary>
		/// Add an amount of ms to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of ms to add</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x26D25A5D, FirmwareVersion = 150)]
		public int sceRtcTickAddMicroseconds(out long dstPtr, ref long srcPtr, long value)
		{
			return sceRtcTickAddTicks(out dstPtr, ref srcPtr, value);
		}

		/// <summary>
		/// Add an amount of seconds to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of seconds to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0xF2A4AFE5, FirmwareVersion = 150)]
		public int sceRtcTickAddSeconds(long* dstPtr, long* srcPtr, long value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromSeconds(value));
		}

		/// <summary>
		/// Add an amount of minutes to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of minutes to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0xE6605BCA, FirmwareVersion = 150)]
		public int sceRtcTickAddMinutes(long* dstPtr, long* srcPtr, long value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromMinutes(value));
		}

		/// <summary>
		/// Add an amount of hours to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of hours to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0x26D7A24A, FirmwareVersion = 150)]
		public int sceRtcTickAddHours(long* dstPtr, long* srcPtr, int value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromHours(value));
		}

		/// <summary>
		/// Add an amount of days to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of days to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0xE51B4B7A, FirmwareVersion = 150)]
		public int sceRtcTickAddDays(long* dstPtr, long* srcPtr, int value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromDays(value));
		}

		/// <summary>
		/// Add an amount of weeks to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of weeks to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0xCF3A2CA8, FirmwareVersion = 150)]
		public int sceRtcTickAddWeeks(long* dstPtr, long* srcPtr, int value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromDays(value * 7));
		}

		/// <summary>
		/// Add an amount of months to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of months to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0xDBF74F1B, FirmwareVersion = 150)]
		public int sceRtcTickAddMonths(long* dstPtr, long* srcPtr, int value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromDays(value * 30));
		}

		/// <summary>
		/// Add an amount of years to a tick
		/// </summary>
		/// <param name="dstPtr">Pointer to tick to hold result</param>
		/// <param name="srcPtr">Pointer to source tick</param>
		/// <param name="value">Number of years to add</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0x42842C77, FirmwareVersion = 150)]
		public int sceRtcTickAddYears(long* dstPtr, long* srcPtr, int value)
		{
			return _sceRtcTickAddTimeSpan(dstPtr, srcPtr, TimeSpan.FromDays(value * 365));
		}

		/// <summary>
		/// Set ticks based on a PspTimeStruct
		/// </summary>
		/// <param name="Date">Pointer to pspTime to convert</param>
		/// <param name="Tick">Pointer to tick to set</param>
		/// <returns>
		///		0 on success
		///		Less than 0 on error
		/// </returns>
		[HlePspFunction(NID = 0x6FF40ACC, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcGetTick(ref ScePspDateTime Date, out ulong Tick)
		{
			try
			{
				Tick = (ulong)Date.ToDateTime().GetTotalMicroseconds();
				return 0;
			}
			catch (Exception)
			{
				//Console.Error.WriteLine("sceRtcGetTick.Date: " + *Date);
				//Console.Error.WriteLine(Exception);
				throw (new SceKernelException(SceKernelErrors.ERROR_INVALID_VALUE));
			}
		}

		/// <summary>
		/// Set a PspTimeStruct based on ticks
		/// </summary>
		/// <param name="Date">Pointer to PspTimeStruct to set</param>
		/// <param name="Ticks">Pointer to ticks to convert</param>
		/// <returns>
		///		0 on success
		///		Less than 0 on error
		/// </returns>
		[HlePspFunction(NID = 0x7ED29E40, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcSetTick(ScePspDateTime* Date, ulong* Ticks)
		{
			try
			{
				*Date = ScePspDateTime.FromDateTime(new DateTime((long)(*Ticks * 10)));
				return 0;
			}
			catch (Exception Exception)
			{
				Console.Error.WriteLine(Exception);
				return -1;
			}
		}

		/// <summary>
		/// Get current local time into a PspTimeStruct
		/// </summary>
		/// <param name="Time">Pointer to PspTimeStruct to receive time</param>
		/// <returns>
		///		0 on success
		///		Less than 0 on error
		/// </returns>
		[HlePspFunction(NID = 0xE7C27D1B, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public int sceRtcGetCurrentClockLocalTime(out ScePspDateTime Time)
		{
			PspRtc.Update();
			var CurrentDateTime = PspRtc.CurrentDateTime;

			Time = new ScePspDateTime()
			{
				Year = (ushort)CurrentDateTime.Year,
				Month = (ushort)CurrentDateTime.Month,
				Day = (ushort)CurrentDateTime.Day,
				Hour = (ushort)CurrentDateTime.Hour,
				Minute = (ushort)CurrentDateTime.Minute,
				Second = (ushort)CurrentDateTime.Second,
				Microsecond = (uint)(CurrentDateTime.Millisecond * 1000),
			};

			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0x011F03C1, FirmwareVersion = 150)]
		public long sceRtcGetAccumulativeTime()
		{
			// Returns the difference between the last reincarnated time and the current tick.
			// Just return our current tick, since there's no need to mimick such behaviour.

			long result;
			sceRtcGetCurrentTick(&result);

			return result;
		}

		/// <summary>
		/// Converts a date to a Unix time
		/// </summary>
		/// <param name="DatePointer"></param>
		/// <param name="UnixTimePointer"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0xE1C93E47, FirmwareVersion = 200)]
		public int sceRtcGetTime64_t(ref ScePspDateTime DatePointer, ref long UnixTimePointer)
		{
			UnixTimePointer = DatePointer.ToUnixTimestamp();

			return 0;
		}

		/// <summary>
		/// Get current tick count, adjusted for local time zone
		/// </summary>
		/// <param name="DateTime">Pointer to PspTimeStruct to receive time</param>
		/// <param name="TimeZone">Time zone to adjust to (minutes from UTC)</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x4CFA57B0, FirmwareVersion = 150)]
		public int sceRtcGetCurrentClock(out ScePspDateTime DateTime, int TimeZone)
		{
			PspRtc.Update();
			var CurrentDateTime = PspRtc.CurrentDateTime;

			try
			{
				CurrentDateTime += TimeSpan.FromMinutes(TimeZone);
			}
			catch
			{
			}

			DateTime = new ScePspDateTime()
			{
				Year = (ushort)CurrentDateTime.Year,
				Month = (ushort)CurrentDateTime.Month,
				Day = (ushort)CurrentDateTime.Day,
				Hour = (ushort)CurrentDateTime.Hour,
				Minute = (ushort)(CurrentDateTime.Minute),
				Second = (ushort)CurrentDateTime.Second,
				Microsecond = (uint)(CurrentDateTime.GetTotalMicroseconds() % 1000000L),
			};

			return 0;
		}

		/// <summary>
		/// Compare two ticks
		/// </summary>
		/// <param name="Tick1">Pointer to first tick.</param>
		/// <param name="Tick2">Pointer to second tick.</param>
		/// <returns>0 when both ticks are equal, &lt; 0 when tick1 &lt; tick2, &gt; 0 when tick1 &gt; tick2</returns>
		[HlePspFunction(NID = 0x9ED0AE87, FirmwareVersion = 150)]
		[PspUntested]
		public int sceRtcCompareTick(ref ulong Tick1, ref ulong Tick2)
		{
			if (Tick1 < Tick2) return -1;
			if (Tick1 > Tick2) return 1;
			return 0;
		}

		[HlePspFunction(NID = 0xCF561893, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcGetWin32FileTime(ScePspDateTime* DateTime, ulong* Win32Time)
		{
			return 0;
		}
		
		/// <summary>
		/// Format Tick-representation UTC time in RFC3339(ISO8601) format
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0x0498FB3C, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcFormatRFC3339()
		{
			return 0;
		}

		[HlePspFunction(NID = 0x1909C99B, FirmwareVersion = 200)]
		[HlePspNotImplemented]
		public int sceRtcSetTime64_t()
		{
			return 0;
		}

		[HlePspFunction(NID = 0x203CEB0D, FirmwareVersion = 200)]
		[HlePspNotImplemented]
		public int sceRtcGetLastReincarnatedTime()
		{
			return 0;
		}

		/// <summary>
		/// Format Tick-representation UTC time in RFC3339(ISO8601) format
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0x27F98543, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcFormatRFC3339LocalTime()
		{
			return 0;
		}

		/// <summary>
		/// Parse time information represented in RFC3339 format
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0x28E1E988, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcParseRFC3339()
		{
			return 0;
		}

		[HlePspFunction(NID = 0x36075567, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcGetDosTime()
		{
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="date"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0x3A807CC8, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcSetTime_t(out ScePspDateTime date, uint time)
		{
			date = ScePspDateTime.FromDateTime(DateTimeRange.ConvertFromUnixTimestamp(time));
			return 0;
		}

		/// <summary>
		/// Check if a year is a leap year
		/// </summary>
		/// <param name="year">Year to check if it's a leap year</param>
		/// <returns>1 if year is a leap year, 0 if not</returns>
		[HlePspFunction(NID = 0x42307A17, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public bool sceRtcIsLeapYear(int year)
		{
			return Calendar.IsLeapYear(year);
		}

		public struct RtcPspTimeStruct
		{
			public ushort year;
			public ushort month;
			public ushort day;
			public ushort hour;
			public ushort minute;
			public ushort second;
			public uint microsecond;
		}

		public enum pspRtcCheckValidErrors
		{
			OK = 0,
			PSP_TIME_INVALID_YEAR = -1,
			PSP_TIME_INVALID_MONTH = -2,
			PSP_TIME_INVALID_DAY = -3,
			PSP_TIME_INVALID_HOUR = -4,
			PSP_TIME_INVALID_MINUTES = -5,
			PSP_TIME_INVALID_SECONDS = -6,
			PSP_TIME_INVALID_MICROSECONDS = -7
		};

		/// <summary>
		/// Validate PspDateStruct component ranges
		/// </summary>
		/// <param name="date">Pointer to the PspDateStruct to be checked</param>
		/// <returns>0 on success, one of ::CheckValidErrors on error</returns>
		[HlePspFunction(NID = 0x4B1B5E82, FirmwareVersion = 150)]
		public pspRtcCheckValidErrors sceRtcCheckValid(RtcPspTimeStruct* Date)
		{
			if (Date->year < 1 || Date->year > 9999)  
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_YEAR;
			}
			else if (Date->month < 1 || Date->month > 12) 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_MONTH;
			}
			else if (Date->day < 1 || Date->day > 31) // TODO: Needs to check actual days in month, including leaps 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_DAY;
			}
			else if (Date->hour > 23) 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_HOUR;
			}
			else if (Date->minute > 59) 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_MINUTES;
			}
			else if (Date->second > 59) 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_SECONDS;
			}
			else if (Date->microsecond >= 1000000) 
			{
				return pspRtcCheckValidErrors.PSP_TIME_INVALID_MICROSECONDS;
			} 

			return pspRtcCheckValidErrors.OK;
		}

		[HlePspFunction(NID = 0x62685E98, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcGetLastAdjustedTime()
		{
			return 0;
		}

		[Inject]
		HleConfig HleConfig;

		private long GetUTCOffsetInTicks()
		{
			return TimeSpan.FromHours(HleConfig.Timezone).GetTotalMicroseconds();
		}

		/// <summary>
		/// Convert a local time based tickcount into a UTC-based tick count
		/// </summary>
		/// <param name="TickLocal">pointer to u64 tick in local time</param>
		/// <param name="TickUTC">pointer to u64 to receive tick in UTC based time</param>
		/// <returns>0 on success, &lt; 0 on error</returns>
		[HlePspFunction(NID = 0x779242A2, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcConvertLocalTimeToUTC(ref long TickLocal, out long TickUTC)
		{
			TickUTC = TickLocal + GetUTCOffsetInTicks();
			return 0;
		}

		/// <summary>
		/// Convert a UTC-based tickcount into a local time tick count
		/// </summary>
		/// <param name="TickUTC">pointer to u64 tick in UTC time</param>
		/// <param name="TickLocal">pointer to u64 to receive tick in local time</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x34885E0D, FirmwareVersion = 150)]
		public int sceRtcConvertUtcToLocalTime(ref long TickUTC, out long TickLocal)
		{
			TickLocal = TickUTC - GetUTCOffsetInTicks();
			return 0;
		}

		[HlePspFunction(NID = 0x7ACE4C04, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcSetWin32FileTime()
		{
			return 0;
		}

		/// <summary>
		/// Format Tick-representation UTC time in RFC2822 format
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0x7DE6711B, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcFormatRFC2822LocalTime()
		{
			return 0;
		}

		/// <summary>
		/// Format Tick-representation UTC time in RFC2822 format
		/// </summary>
		/// <returns></returns>
		[HlePspFunction(NID = 0xC663B3B9, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcFormatRFC2822()
		{
			return 0;
		}

		[HlePspFunction(NID = 0xDFBC5F16, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcParseDateTime()
		{
			return 0;
		}

		[HlePspFunction(NID = 0xF006F264, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int sceRtcSetDosTime()
		{
			return 0;
		}
	}

	public enum PspDaysOfWeek : int
	{
		Monday = 1,
		Tuesday = 2,
		Wednesday = 3,
		Thursday = 4,
		Friday = 5,
		Saturday = 6,
		Sunday = 0,
	}
}
