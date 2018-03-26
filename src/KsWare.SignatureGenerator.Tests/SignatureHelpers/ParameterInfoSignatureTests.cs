using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.SignatureGenerator.Tests.SignatureHelpers {

	[TestClass()]
	public class ParameterInfoSignatureTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class Parameters {

			public void A() { }
			public void B(bool a) { }

			public void C(bool[] a) { }

			public void D(params bool[] a) { } // same signature as C

			public void E(ref bool a) { }

			public void F(out bool a) { a = false; }
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "A", "public void A()")]
		[DataRow(typeof(Parameters), "B", "public void B(bool)")]
		[DataRow(typeof(Parameters), "C", "public void C(bool[])")]
		[DataRow(typeof(Parameters), "D", "public void D(bool[])")] // TODO "public void D(params bool[])"
//TODO		[DataRow(typeof(Parameters), "E", "public void E(ref bool a)")]
//TODO		[DataRow(typeof(Parameters), "F", "public void F(out bool a)")]
		public void SigParametersTest(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
	}

}