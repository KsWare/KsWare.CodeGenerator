using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.SignatureGenerator.Tests.SignatureHelpers {

	[TestClass()]
	public class FieldSignatureGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class Fields {
			public bool A;
			public static bool SA;
			public const bool CA = true;
			public readonly bool RA = true;
			public static readonly bool SRA = true;
		}

		[DataTestMethod]
		[DataRow(typeof(Fields), "A",   "public bool A")]
		[DataRow(typeof(Fields), "SA",  "public static bool SA")]
		[DataRow(typeof(Fields), "CA",  "public const bool CA")]
		[DataRow(typeof(Fields), "RA",  "public readonly bool RA")]
		[DataRow(typeof(Fields), "SRA", "public static readonly bool SRA")]
		public void SigFieldInfoTest(Type type, string name, string result) {
			var mi = (FieldInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
	}

}