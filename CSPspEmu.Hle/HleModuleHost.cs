﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CSPspEmu.Core.Cpu;
using CSPspEmu.Core.Cpu.Emiter;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using CSharpUtils;
using CSharpUtils.Extensions;

namespace CSPspEmu.Hle
{
	unsafe public class HleModuleHost : HleModule
	{
		public HleState HleState;
		public Dictionary<uint, String> NamesByNID = new Dictionary<uint, String>();
		public Dictionary<uint, Action<CpuThreadState>> DelegatesByNID = new Dictionary<uint, Action<CpuThreadState>>();
		public Dictionary<string, Action<CpuThreadState>> DelegatesByName = new Dictionary<string, Action<CpuThreadState>>();

		public HleModuleHost()
		{
		}

		public void Initialize(HleState HleState)
		{
			this.HleState = HleState;

			foreach (var MethodInfo in this.GetType().GetMethods())
			{
				var Attributes = MethodInfo.GetCustomAttributes(typeof(HlePspFunctionAttribute), true).Cast<HlePspFunctionAttribute>();
				if (Attributes.Count() > 0)
				{
					var Delegate = CreateDelegateForMethodInfo(MethodInfo, Attributes.First());
					DelegatesByName[MethodInfo.Name] = Delegate;
					foreach (var Attribute in Attributes)
					{
						//Console.WriteLine(Attribute.NID);
						DelegatesByNID[Attribute.NID] = Delegate;
						NamesByNID[Attribute.NID] = MethodInfo.Name;
					}
				}
			}
		}

		static public string StringFromAddress(CpuThreadState CpuThreadState, uint Address)
		{
			if (Address == 0) return null;
			return PointerUtils.PtrToString((byte*)CpuThreadState.GetMemoryPtr(Address), Encoding.UTF8);
		}

		private struct ParamInfo
		{
			public enum RegisterTypeEnum
			{
				Gpr, Fpr,
			}
			public RegisterTypeEnum RegisterType;
			public int RegisterIndex;
			public Type ParameterType;
			public String ParameterName;
		}

		private Action<CpuThreadState> CreateDelegateForMethodInfo(MethodInfo MethodInfo, HlePspFunctionAttribute HlePspFunctionAttribute)
		{
			var MipsMethodEmiter = new MipsMethodEmiter(HleState.MipsEmiter, HleState.CpuProcessor);
			int GprIndex = 4;
			int FprIndex = 0;

			var NotImplementedAttribute = (HlePspNotImplementedAttribute)MethodInfo.GetCustomAttributes(typeof(HlePspNotImplementedAttribute), true).FirstOrDefault();
			bool NotImplemented = (NotImplementedAttribute != null) ? NotImplementedAttribute.Notice : false;
			bool SkipLog = HlePspFunctionAttribute.SkipLog;

			var ParamInfoList = new List<ParamInfo>();

			Action CallAction = () =>
			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldarg_0);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldfld, typeof(CpuThreadState).GetField("ModuleObject"));
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Castclass, this.GetType());
				foreach (var ParameterInfo in MethodInfo.GetParameters())
				{
					var ParameterType = ParameterInfo.ParameterType;

					// The CpuThreadState
					if (ParameterType == typeof(CpuThreadState))
					{
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldarg_0);
					}
					// A stringz
					else if (ParameterType == typeof(string))
					{
						ParamInfoList.Add(new ParamInfo()
						{
							ParameterName = ParameterInfo.Name,
							RegisterType = ParamInfo.RegisterTypeEnum.Gpr,
							RegisterIndex = GprIndex,
							ParameterType = ParameterType,
						});
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldarg_0);
						MipsMethodEmiter.LoadGPR_Unsigned(GprIndex);
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Call, typeof(HleModuleHost).GetMethod("StringFromAddress"));
						GprIndex++;
					}
					// A pointer
					else if (ParameterType.IsPointer)
					{
						ParamInfoList.Add(new ParamInfo()
						{
							ParameterName = ParameterInfo.Name,
							RegisterType = ParamInfo.RegisterTypeEnum.Gpr,
							RegisterIndex = GprIndex,
							ParameterType = typeof(uint),
						});
						MipsMethodEmiter._getmemptr(() =>
						{
							MipsMethodEmiter.LoadGPR_Unsigned(GprIndex);
						}, Safe: true);
						GprIndex++;
					}
					// A long type
					else if (ParameterType == typeof(long) || ParameterType == typeof(ulong))
					{
						while (GprIndex % 2 != 0) GprIndex++;

						ParamInfoList.Add(new ParamInfo()
						{
							ParameterName = ParameterInfo.Name,
							RegisterType = ParamInfo.RegisterTypeEnum.Gpr,
							RegisterIndex = GprIndex,
							ParameterType = ParameterType,
						});

						MipsMethodEmiter.LoadGPR_Unsigned(GprIndex + 0);
						MipsMethodEmiter.LoadGPR_Unsigned(GprIndex + 1);
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, 32);
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Shl);
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Or);
						GprIndex += 2;
					}
					// A float register.
					else if (ParameterType == typeof(float))
					{
						ParamInfoList.Add(new ParamInfo()
						{
							ParameterName = ParameterInfo.Name,
							RegisterType = ParamInfo.RegisterTypeEnum.Fpr,
							RegisterIndex = FprIndex,
							ParameterType = ParameterType,
						});

						MipsMethodEmiter.LoadFPR(FprIndex);
						FprIndex++;
					}
					// An integer register
					else
					{
						ParamInfoList.Add(new ParamInfo()
						{
							ParameterName = ParameterInfo.Name,
							RegisterType = ParamInfo.RegisterTypeEnum.Gpr,
							RegisterIndex = GprIndex,
							ParameterType = ParameterType,
						});

						MipsMethodEmiter.LoadGPR_Unsigned(GprIndex);
						GprIndex++;
					}
					//MipsMethodEmiter.ILGenerator.Emit(OpCodes.ld
				}
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Call, MethodInfo);
			};

			if (MethodInfo.ReturnType == typeof(void))
			{
				CallAction();
			}
			else if (MethodInfo.ReturnType == typeof(long))
			{
				throw(new NotImplementedException());
			}
			else
			{
				MipsMethodEmiter.SaveGPR(2, CallAction);
			}

			var Delegate = MipsMethodEmiter.CreateDelegate();
			return (CpuThreadState) =>
			{
				bool Trace = (!SkipLog && CpuThreadState.CpuProcessor.PspConfig.DebugSyscalls);

				if (NotImplemented)
				{
					Trace = true;
					ConsoleUtils.SaveRestoreConsoleState(() =>
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine(
							"Not implemented {0}.{1}",
							MethodInfo.DeclaringType.Name, MethodInfo.Name
						);
					});
				}

				if (Trace)
				{
					Console.Write(
						"Thread({0}:'{1}') : RA(0x{2:X}) : {3}.{4}",
						HleState.ThreadManager.Current.Id,
						HleState.ThreadManager.Current.Name,
						HleState.ThreadManager.Current.CpuThreadState.RA,
						MethodInfo.DeclaringType.Name, MethodInfo.Name
					);
					Console.Write("(");
					int Count = 0;
					foreach (var ParamInfo in ParamInfoList)
					{
						if (Count > 0) Console.Write(", ");
						Console.Write("{0}:", ParamInfo.ParameterName);
						switch (ParamInfo.RegisterType)
						{
							case HleModuleHost.ParamInfo.RegisterTypeEnum.Fpr:
							case HleModuleHost.ParamInfo.RegisterTypeEnum.Gpr:
								uint Int4 = (uint)CpuThreadState.GPR[ParamInfo.RegisterIndex];
								uint Float4 = (uint)CpuThreadState.FPR[ParamInfo.RegisterIndex];
								Console.Write("{0}", ToNormalizedTypeString(ParamInfo.ParameterType, CpuThreadState, Int4, Float4));
								break;
							default:
								throw(new NotImplementedException());
						}
						Count++;
					}
					Console.Write(")");
					//Console.WriteLine("");
				}

				CpuThreadState.ModuleObject = this;
				try
				{
					Delegate(CpuThreadState);
				}
				finally
				{
					if (Trace)
					{
						Console.WriteLine(" : {0}", ToNormalizedTypeString(MethodInfo.ReturnType, CpuThreadState, (uint)CpuThreadState.GPR[2], (float)CpuThreadState.FPR[0]));
						Console.WriteLine("");
					}
				}
			};
		}

		static public string ToNormalizedTypeString(Type ParameterType, CpuThreadState CpuThreadState, uint Int4, float Float4)
		{
			if (ParameterType == typeof(void))
			{
				return "void";
			}

			if (ParameterType == typeof(string))
			{
				return String.Format("'{0}'", StringFromAddress(CpuThreadState, Int4));
			}

			if (ParameterType == typeof(int))
			{
				return String.Format("{0}", (int)Int4);
			}

			if (ParameterType.IsEnum)
			{
				return ParameterType.GetEnumName(Int4);
			}

			if (ParameterType.IsPointer)
			{
				return "0x%08X".Sprintf(CpuThreadState.CpuProcessor.Memory.PointerToPspAddress((void*)Int4));
			}

			if (ParameterType == typeof(float))
			{
				return String.Format("{0}", Float4);
			}

			return "0x%08X".Sprintf(Int4);
		}
	}
}