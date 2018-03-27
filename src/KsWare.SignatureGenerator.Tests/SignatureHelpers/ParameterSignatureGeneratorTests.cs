using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.SignatureGenerator.Tests.SignatureHelpers {

	[TestClass()]
	public class ParameterSignatureGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class Parameters {

			public void A() { }
			public void B(bool a) { }			// same signature as B(bool a = false)
			public void C(bool[] a) { }
			public void D(params bool[] a) { } // same signature as C
			public void E(ref bool a) { }
			public void F(out bool a) { a = false; }

			// TODO
			public void G(int a, bool b) { }
			public void H(bool a = false) { }
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "A", "")]
		[DataRow(typeof(Parameters), "B", "bool")]
		[DataRow(typeof(Parameters), "C", "bool[]")]
		[DataRow(typeof(Parameters), "D", "bool[]")] 
		[DataRow(typeof(Parameters), "E", "ref bool")]
		[DataRow(typeof(Parameters), "F", "out bool")]
		[DataRow(typeof(Parameters), "G", "int, bool")]
		[DataRow(typeof(Parameters), "H", "bool")]
		public void SigParameters_Signature_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			SignatureHelper.ForSignature.Sig(mi.GetParameters()).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "A", "")]
		[DataRow(typeof(Parameters), "B", "bool a")]
		[DataRow(typeof(Parameters), "C", "bool[] a")]
		[DataRow(typeof(Parameters), "D", "params bool[] a")]
		[DataRow(typeof(Parameters), "E", "ref bool a")]
		[DataRow(typeof(Parameters), "F", "out bool a")]
		[DataRow(typeof(Parameters), "G", "int a, bool b")]
		[DataRow(typeof(Parameters), "H", "bool a")] //TODO optional
		public void SigParameters_Code_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			SignatureHelper.ForCode.Sig(mi.GetParameters()).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "C", "bool[]")]
		[DataRow(typeof(Parameters), "D", "params bool[]")] 
		public void SigParameters1Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,AllBindingFlags)[0];
			SignatureHelper.ForCompare.Sig(mi.GetParameters()).Should().Be(result);
		}
	}

}