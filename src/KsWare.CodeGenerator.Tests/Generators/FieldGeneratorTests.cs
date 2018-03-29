using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	
	public class FieldGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

#pragma warning disable 169, 649
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private class Fields {
			public bool A;
			public static bool SA;
			public const bool CA = true;
			public readonly bool RA = true;
			public static readonly bool SRA = true;
		}
#pragma warning restore 169, 649

		[DataTestMethod]
		[DataRow(typeof(Fields), "A",   "public bool A")]
		[DataRow(typeof(Fields), "SA",  "public static bool SA")]
		[DataRow(typeof(Fields), "CA",  "public const bool CA")]
		[DataRow(typeof(Fields), "RA",  "public readonly bool RA")]
		[DataRow(typeof(Fields), "SRA", "public static readonly bool SRA")]
		public void SigFieldInfoTest(Type type, string name, string result) {
			var mi = (FieldInfo) type.GetMember(name,AllBindingFlags)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}
	}

}