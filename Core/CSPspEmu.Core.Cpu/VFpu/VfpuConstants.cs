﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPspEmu.Core.Cpu.VFpu
{
	static public class VfpuConstants
	{
		static public string GetRegisterNameByIndex(VfpuRegisterType Type, int Size, uint Register)
		{
			var RegisterName = GetRegisterNames(Type, Size)[Register];
			if (RegisterName == "") throw (new InvalidOperationException(String.Format("Invalid register {0}, {1}, {2}", Type, Size, Register)));
			return RegisterName;
		}

		static public uint GetRegisterIndexByName(VfpuRegisterType Type, int Size, string Name)
		{
			// @TODO: Create a dictionary.
			var Names = GetRegisterNames(Type, Size);
			return (uint)Array.IndexOf(Names, Name);
		}

		static private string[] GetRegisterNames(VfpuRegisterType Type, int Size)
		{
			if (Type == VfpuRegisterType.Cell && Size == 1) return vfpu_sreg_names;
			
			if (Type == VfpuRegisterType.Vector && Size == 2) return vfpu_vpreg_names;
			if (Type == VfpuRegisterType.Vector && Size == 3) return vfpu_vtreg_names;
			if (Type == VfpuRegisterType.Vector && Size == 4) return vfpu_vqreg_names;
			
			if (Type == VfpuRegisterType.Matrix && Size == 2) return vfpu_mpreg_names;
			if (Type == VfpuRegisterType.Matrix && Size == 3) return vfpu_mtreg_names;
			if (Type == VfpuRegisterType.Matrix && Size == 4) return vfpu_mqreg_names;

			throw (new InvalidOperationException(String.Format("{0} + {1}", Type, Size)));
		}

		//static readonly string[] vfpu_sreg_names = Enumerable.Range(0, 128).Select(Index => String.Format("S{0}{1}{2}", (Index >> 2) & 7, (Index >> 0) & 3, (Index >> 5) & 3)).ToArray();
		static readonly string[] vfpu_sreg_names =
		{
			"S000",  "S010",  "S020",  "S030",  "S100",  "S110",  "S120",  "S130",
			"S200",  "S210",  "S220",  "S230",  "S300",  "S310",  "S320",  "S330",
			"S400",  "S410",  "S420",  "S430",  "S500",  "S510",  "S520",  "S530",
			"S600",  "S610",  "S620",  "S630",  "S700",  "S710",  "S720",  "S730",
			"S001",  "S011",  "S021",  "S031",  "S101",  "S111",  "S121",  "S131",
			"S201",  "S211",  "S221",  "S231",  "S301",  "S311",  "S321",  "S331",
			"S401",  "S411",  "S421",  "S431",  "S501",  "S511",  "S521",  "S531",
			"S601",  "S611",  "S621",  "S631",  "S701",  "S711",  "S721",  "S731",
			"S002",  "S012",  "S022",  "S032",  "S102",  "S112",  "S122",  "S132",
			"S202",  "S212",  "S222",  "S232",  "S302",  "S312",  "S322",  "S332",
			"S402",  "S412",  "S422",  "S432",  "S502",  "S512",  "S522",  "S532",
			"S602",  "S612",  "S622",  "S632",  "S702",  "S712",  "S722",  "S732",
			"S003",  "S013",  "S023",  "S033",  "S103",  "S113",  "S123",  "S133",
			"S203",  "S213",  "S223",  "S233",  "S303",  "S313",  "S323",  "S333",
			"S403",  "S413",  "S423",  "S433",  "S503",  "S513",  "S523",  "S533",
			"S603",  "S613",  "S623",  "S633",  "S703",  "S713",  "S723",  "S733"
		};

		static readonly string[] vfpu_vpreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
			"C002",  "C012",  "C022",  "C032",  "C102",  "C112",  "C122",  "C132",
			"C202",  "C212",  "C222",  "C232",  "C302",  "C312",  "C322",  "C332",
			"C402",  "C412",  "C422",  "C432",  "C502",  "C512",  "C522",  "C532",
			"C602",  "C612",  "C622",  "C632",  "C702",  "C712",  "C722",  "C732",
			"R020",  "R021",  "R022",  "R023",  "R120",  "R121",  "R122",  "R123",
			"R220",  "R221",  "R222",  "R223",  "R320",  "R321",  "R322",  "R323",
			"R420",  "R421",  "R422",  "R423",  "R520",  "R521",  "R522",  "R523",
			"R620",  "R621",  "R622",  "R623",  "R720",  "R721",  "R722",  "R723"
		};

		static readonly string[] vfpu_vtreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
			"C001",  "C011",  "C021",  "C031",  "C101",  "C111",  "C121",  "C131",
			"C201",  "C211",  "C221",  "C231",  "C301",  "C311",  "C321",  "C331",
			"C401",  "C411",  "C421",  "C431",  "C501",  "C511",  "C521",  "C531",
			"C601",  "C611",  "C621",  "C631",  "C701",  "C711",  "C721",  "C731",
			"R010",  "R011",  "R012",  "R013",  "R110",  "R111",  "R112",  "R113",
			"R210",  "R211",  "R212",  "R213",  "R310",  "R311",  "R312",  "R313",
			"R410",  "R411",  "R412",  "R413",  "R510",  "R511",  "R512",  "R513",
			"R610",  "R611",  "R612",  "R613",  "R710",  "R711",  "R712",  "R713"
		};

		static readonly string[] vfpu_vqreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",

			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
		};

		static readonly string[] vfpu_mpreg_names =
		{
			"M000",  "M000",  "M020",  "M020",  "M100",  "M100",  "M120",  "M120",
			"M200",  "M200",  "M220",  "M220",  "M300",  "M300",  "M320",  "M320",
			"M400",  "M400",  "M420",  "M420",  "M500",  "M500",  "M520",  "M520",
			"M600",  "M600",  "M620",  "M620",  "M700",  "M700",  "M720",  "M720",
			"E000",  "E000",  "E002",  "E002",  "E100",  "E100",  "E102",  "E102",
			"E200",  "E200",  "E202",  "E202",  "E300",  "E300",  "E302",  "E302",
			"E400",  "E400",  "E402",  "E402",  "E500",  "E500",  "E502",  "E502",
			"E600",  "E600",  "E602",  "E602",  "E700",  "E700",  "E702",  "E702",
			"M002",  "M002",  "M022",  "M022",  "M102",  "M102",  "M122",  "M122",
			"M202",  "M202",  "M222",  "M222",  "M302",  "M302",  "M322",  "M322",
			"M402",  "M402",  "M422",  "M422",  "M502",  "M502",  "M522",  "M522",
			"M602",  "M602",  "M622",  "M622",  "M702",  "M702",  "M722",  "M722",
			"E020",  "E020",  "E022",  "E022",  "E120",  "E120",  "E122",  "E122",
			"E220",  "E220",  "E222",  "E222",  "E320",  "E320",  "E322",  "E322",
			"E420",  "E420",  "E422",  "E422",  "E520",  "E520",  "E522",  "E522",
			"E620",  "E620",  "E622",  "E622",  "E720",  "E720",  "E722",  "E722"
		};

		static readonly string[] vfpu_mtreg_names =
		{
			"M000",  "M010",  "M000",  "M010",  "M100",  "M110",  "M100",  "M110",
			"M200",  "M210",  "M200",  "M210",  "M300",  "M310",  "M300",  "M310",
			"M400",  "M410",  "M400",  "M410",  "M500",  "M510",  "M500",  "M510",
			"M600",  "M610",  "M600",  "M610",  "M700",  "M710",  "M700",  "M710",
			"E000",  "E001",  "E000",  "E001",  "E100",  "E101",  "E100",  "E101",
			"E200",  "E201",  "E200",  "E201",  "E300",  "E301",  "E300",  "E301",
			"E400",  "E401",  "E400",  "E401",  "E500",  "E501",  "E500",  "E501",
			"E600",  "E601",  "E600",  "E601",  "E700",  "E701",  "E700",  "E701",
			"M001",  "M011",  "M001",  "M011",  "M101",  "M111",  "M101",  "M111",
			"M201",  "M211",  "M201",  "M211",  "M301",  "M311",  "M301",  "M311",
			"M401",  "M411",  "M401",  "M411",  "M501",  "M511",  "M501",  "M511",
			"M601",  "M611",  "M601",  "M611",  "M701",  "M711",  "M701",  "M711",                                                                                   
			"E010",  "E011",  "E010",  "E011",  "E110",  "E111",  "E110",  "E111",
			"E210",  "E211",  "E210",  "E211",  "E310",  "E311",  "E310",  "E311",
			"E410",  "E411",  "E410",  "E411",  "E510",  "E511",  "E510",  "E511",
			"E610",  "E611",  "E610",  "E611",  "E710",  "E711",  "E710",  "E711"
		};

		static readonly string[] vfpu_mqreg_names =
		{
			"M000",  "M000",  "M000",  "M000",  "M100",  "M100",  "M100",  "M100",
			"M200",  "M200",  "M200",  "M200",  "M300",  "M300",  "M300",  "M300",
			"M400",  "M400",  "M400",  "M400",  "M500",  "M500",  "M500",  "M500",
			"M600",  "M600",  "M600",  "M600",  "M700",  "M700",  "M700",  "M700",
			"E000",  "E000",  "E000",  "E000",  "E100",  "E100",  "E100",  "E100",
			"E200",  "E200",  "E200",  "E200",  "E300",  "E300",  "E300",  "E300",
			"E400",  "E400",  "E400",  "E400",  "E500",  "E500",  "E500",  "E500",
			"E600",  "E600",  "E600",  "E600",  "E700",  "E700",  "E700",  "E700",
			"M000",  "M000",  "M000",  "M000",  "M100",  "M100",  "M100",  "M100",
			"M200",  "M200",  "M200",  "M200",  "M300",  "M300",  "M300",  "M300",
			"M400",  "M400",  "M400",  "M400",  "M500",  "M500",  "M500",  "M500",
			"M600",  "M600",  "M600",  "M600",  "M700",  "M700",  "M700",  "M700",
			"E000",  "E000",  "E000",  "E000",  "E100",  "E100",  "E100",  "E100",
			"E200",  "E200",  "E200",  "E200",  "E300",  "E300",  "E300",  "E300",
			"E400",  "E400",  "E400",  "E400",  "E500",  "E500",  "E500",  "E500",
			"E600",  "E600",  "E600",  "E600",  "E700",  "E700",  "E700",  "E700",
		};

		/*
		static readonly string[] vfpu_sreg_names =
		{
			"S000",  "S010",  "S020",  "S030",  "S100",  "S110",  "S120",  "S130",
			"S200",  "S210",  "S220",  "S230",  "S300",  "S310",  "S320",  "S330",
			"S400",  "S410",  "S420",  "S430",  "S500",  "S510",  "S520",  "S530",
			"S600",  "S610",  "S620",  "S630",  "S700",  "S710",  "S720",  "S730",
			"S001",  "S011",  "S021",  "S031",  "S101",  "S111",  "S121",  "S131",
			"S201",  "S211",  "S221",  "S231",  "S301",  "S311",  "S321",  "S331",
			"S401",  "S411",  "S421",  "S431",  "S501",  "S511",  "S521",  "S531",
			"S601",  "S611",  "S621",  "S631",  "S701",  "S711",  "S721",  "S731",
			"S002",  "S012",  "S022",  "S032",  "S102",  "S112",  "S122",  "S132",
			"S202",  "S212",  "S222",  "S232",  "S302",  "S312",  "S322",  "S332",
			"S402",  "S412",  "S422",  "S432",  "S502",  "S512",  "S522",  "S532",
			"S602",  "S612",  "S622",  "S632",  "S702",  "S712",  "S722",  "S732",
			"S003",  "S013",  "S023",  "S033",  "S103",  "S113",  "S123",  "S133",
			"S203",  "S213",  "S223",  "S233",  "S303",  "S313",  "S323",  "S333",
			"S403",  "S413",  "S423",  "S433",  "S503",  "S513",  "S523",  "S533",
			"S603",  "S613",  "S623",  "S633",  "S703",  "S713",  "S723",  "S733"
		};
 
		static readonly string[] vfpu_vpreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
			"C002",  "C012",  "C022",  "C032",  "C102",  "C112",  "C122",  "C132",
			"C202",  "C212",  "C222",  "C232",  "C302",  "C312",  "C322",  "C332",
			"C402",  "C412",  "C422",  "C432",  "C502",  "C512",  "C522",  "C532",
			"C602",  "C612",  "C622",  "C632",  "C702",  "C712",  "C722",  "C732",
			"R020",  "R021",  "R022",  "R023",  "R120",  "R121",  "R122",  "R123",
			"R220",  "R221",  "R222",  "R223",  "R320",  "R321",  "R322",  "R323",
			"R420",  "R421",  "R422",  "R423",  "R520",  "R521",  "R522",  "R523",
			"R620",  "R621",  "R622",  "R623",  "R720",  "R721",  "R722",  "R723"
		};
 
		static readonly string[] vfpu_vtreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
			"C001",  "C011",  "C021",  "C031",  "C101",  "C111",  "C121",  "C131",
			"C201",  "C211",  "C221",  "C231",  "C301",  "C311",  "C321",  "C331",
			"C401",  "C411",  "C421",  "C431",  "C501",  "C511",  "C521",  "C531",
			"C601",  "C611",  "C621",  "C631",  "C701",  "C711",  "C721",  "C731",
			"R010",  "R011",  "R012",  "R013",  "R110",  "R111",  "R112",  "R113",
			"R210",  "R211",  "R212",  "R213",  "R310",  "R311",  "R312",  "R313",
			"R410",  "R411",  "R412",  "R413",  "R510",  "R511",  "R512",  "R513",
			"R610",  "R611",  "R612",  "R613",  "R710",  "R711",  "R712",  "R713"
		};

		static readonly string[] vfpu_vqreg_names =
		{
			"C000",  "C010",  "C020",  "C030",  "C100",  "C110",  "C120",  "C130",
			"C200",  "C210",  "C220",  "C230",  "C300",  "C310",  "C320",  "C330",
			"C400",  "C410",  "C420",  "C430",  "C500",  "C510",  "C520",  "C530",
			"C600",  "C610",  "C620",  "C630",  "C700",  "C710",  "C720",  "C730",
			"R000",  "R001",  "R002",  "R003",  "R100",  "R101",  "R102",  "R103",
			"R200",  "R201",  "R202",  "R203",  "R300",  "R301",  "R302",  "R303",
			"R400",  "R401",  "R402",  "R403",  "R500",  "R501",  "R502",  "R503",
			"R600",  "R601",  "R602",  "R603",  "R700",  "R701",  "R702",  "R703",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  ""
		};

		static readonly string[] vfpu_mpreg_names =
		{
			"M000",  "",  "M020",  "",  "M100",  "",  "M120",  "",
			"M200",  "",  "M220",  "",  "M300",  "",  "M320",  "",
			"M400",  "",  "M420",  "",  "M500",  "",  "M520",  "",
			"M600",  "",  "M620",  "",  "M700",  "",  "M720",  "",
			"E000",  "",  "E002",  "",  "E100",  "",  "E102",  "",
			"E200",  "",  "E202",  "",  "E300",  "",  "E302",  "",
			"E400",  "",  "E402",  "",  "E500",  "",  "E502",  "",
			"E600",  "",  "E602",  "",  "E700",  "",  "E702",  "",
			"M002",  "",  "M022",  "",  "M102",  "",  "M122",  "",
			"M202",  "",  "M222",  "",  "M302",  "",  "M322",  "",
			"M402",  "",  "M422",  "",  "M502",  "",  "M522",  "",
			"M602",  "",  "M622",  "",  "M702",  "",  "M722",  "",
			"E020",  "",  "E022",  "",  "E120",  "",  "E122",  "",
			"E220",  "",  "E222",  "",  "E320",  "",  "E322",  "",
			"E420",  "",  "E422",  "",  "E520",  "",  "E522",  "",
			"E620",  "",  "E622",  "",  "E720",  "",  "E722",  ""
		};

		static readonly string[] vfpu_mtreg_names =
		{
			"M000",  "M010",  "",  "",  "M100",  "M110",  "",  "",
			"M200",  "M210",  "",  "",  "M300",  "M310",  "",  "",
			"M400",  "M410",  "",  "",  "M500",  "M510",  "",  "",
			"M600",  "M610",  "",  "",  "M700",  "M710",  "",  "",
			"E000",  "E001",  "",  "",  "E100",  "E101",  "",  "",
			"E200",  "E201",  "",  "",  "E300",  "E301",  "",  "",
			"E400",  "E401",  "",  "",  "E500",  "E501",  "",  "",
			"E600",  "E601",  "",  "",  "E700",  "E701",  "",  "",
			"M001",  "M011",  "",  "",  "M101",  "M111",  "",  "",
			"M201",  "M211",  "",  "",  "M301",  "M311",  "",  "",
			"M401",  "M411",  "",  "",  "M501",  "M511",  "",  "",
			"M601",  "M611",  "",  "",  "M701",  "M711",  "",  "",                                                                                   
			"E010",  "E011",  "",  "",  "E110",  "E111",  "",  "",
			"E210",  "E211",  "",  "",  "E310",  "E311",  "",  "",
			"E410",  "E411",  "",  "",  "E510",  "E511",  "",  "",
			"E610",  "E611",  "",  "",  "E710",  "E711",  "",  ""
		};

		static readonly string[] vfpu_mqreg_names =
		{
			"M000",  "",  "",  "",  "M100",  "",  "",  "",
			"M200",  "",  "",  "",  "M300",  "",  "",  "",
			"M400",  "",  "",  "",  "M500",  "",  "",  "",
			"M600",  "",  "",  "",  "M700",  "",  "",  "",
			"E000",  "",  "",  "",  "E100",  "",  "",  "",
			"E200",  "",  "",  "",  "E300",  "",  "",  "",
			"E400",  "",  "",  "",  "E500",  "",  "",  "",
			"E600",  "",  "",  "",  "E700",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  "",
			"",  "",  "",  "",  "",  "",  "",  ""
		};
		*/

		public class Info
		{
			public readonly string Name;
			public readonly float Value;

			internal Info(string Name, float Value)
			{
				this.Name = Name;
				this.Value = Value;
			}
		}

		/// <summary>
		///  vcst.[s | p | t | q] vd, VFPU_CST
		///  vd = vfpu_constant[VFPU_CST], where VFPU_CST is one of:
		///    VFPU_HUGE      infinity
		///    VFPU_SQRT2     sqrt(2)
		///    VFPU_SQRT1_2   sqrt(1/2)
		///    VFPU_2_SQRTPI  2/sqrt(pi)
		///    VFPU_PI        pi
		///    VFPU_2_PI      2/pi
		///    VFPU_1_PI      1/pi
		///    VFPU_PI_4      pi/4
		///    VFPU_PI_2      pi/2
		///    VFPU_E         e
		///    VFPU_LOG2E     log2(e)
		///    VFPU_LOG10E    log10(e)
		///    VFPU_LN2       ln(2)
		///    VFPU_LN10      ln(10)
		///    VFPU_2PI       2*pi
		///    VFPU_PI_6      pi/6
		///    VFPU_LOG10TWO  log10(2)
		///    VFPU_LOG2TEN   log2(10)
		///    VFPU_SQRT3_2   sqrt(3)/2
		/// </summary>
		public static readonly Info[] Constants = new[]
		{
			new Info("VFPU_ZERO", (float)0.0f),
			new Info("VFPU_HUGE", (float)340282346638528859811704183484516925440f),
			new Info("VFPU_SQRT2", (float)(Math.Sqrt(2.0))),
			new Info("VFPU_SQRT1_2", (float)(Math.Sqrt(1.0 / 2.0))),
			new Info("VFPU_2_SQRTPI", (float)(2.0 / Math.Sqrt(Math.PI))),
			new Info("VFPU_2_PI", (float)(2.0 / Math.PI)),
			new Info("VFPU_1_PI", (float)(1.0 / Math.PI)),
			new Info("VFPU_PI_4", (float)(Math.PI / 4.0)),
			new Info("VFPU_PI_2", (float)(Math.PI / 2.0)),
			new Info("VFPU_PI", (float)(Math.PI)),
			new Info("VFPU_E", (float)(Math.E)),
			new Info("VFPU_LOG2E", (float)(Math.Log(Math.E, 2))),
			new Info("VFPU_LOG10E", (float)(Math.Log10(Math.E))),
			new Info("VFPU_LN2", (float)(Math.Log(2))),
			new Info("VFPU_LN10", (float)(Math.Log(10))),
			new Info("VFPU_2PI", (float)(2.0 * Math.PI)),
			new Info("VFPU_PI_6", (float)(Math.PI / 6.0)),
			new Info("VFPU_LOG10TWO", (float)(Math.Log10(2.0))),          
			new Info("VFPU_LOG2TEN", (float)(Math.Log(10.0, 2))),         
			new Info("VFPU_SQRT3_2", (float)(Math.Sqrt(3.0) / 2.0)),
		};

		private static readonly Dictionary<string, int> Indices = Enumerable.Range(0, Constants.Length).CreateDictionary(Index => Constants[Index].Name, Index => Index);

		public static Info GetConstantValueByIndex(int Index)
		{
			if (Index < 0 || Index >= Constants.Length) throw(new InvalidOperationException(String.Format("Invalid constant index '{0}'", Index)));
			return Constants[Index];
		}

		public static int GetConstantIndexByName(string Name)
		{
			return Indices[Name];
		}

		public static Info GetConstantValueByName(string Name)
		{
			return GetConstantValueByIndex(GetConstantIndexByName(Name));
		}
	}
}
