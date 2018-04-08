using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	public class ParameterGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class Parameters {

			public void A() { }
			public void B(bool a) { }			// same signature as H(bool a = false)
			public void C(bool[] a) { }
			public void D(params bool[] a) { } // same signature as C
			public void E(ref bool a) { }
			public void E(bool a) { }
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
			Generator.ForSignature.Generate(mi.GetParameters()).Should().Be(result);
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
		public void Parameters_ForDeclare_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForDeclare.Generate(mi.GetParameters()).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "A", "")]
		[DataRow(typeof(Parameters), "B", "a")]
		[DataRow(typeof(Parameters), "C", "a")]
		[DataRow(typeof(Parameters), "D", "a")]
		[DataRow(typeof(Parameters), "E", "ref a")]
		[DataRow(typeof(Parameters), "F", "out a")]
		[DataRow(typeof(Parameters), "G", "a, b")]
		[DataRow(typeof(Parameters), "H", "a")] //TODO optional
		public void Parameters_ForCall_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForCall.Generate(mi.GetParameters()).Should().Be(result);
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
		public void Parameters_ForInheriteDoc_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForInheriteDoc.Generate(mi.GetParameters()).Should().Be(result);
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
		public void Parameters_ForSignature_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForSignature.Generate(mi.GetParameters()).Should().Be(result);
		}
	}

}