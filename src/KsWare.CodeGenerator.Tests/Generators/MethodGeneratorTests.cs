using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	public class MethodGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		#region Modifiers
		private class MethodModifiers {
			private bool A() => true;
			protected bool B() => true;
//			private internal bool C() => true;
			internal bool D() => true;
			protected internal bool E() => true;
			public bool F() => true;

			private static bool SA() => true;
			protected static bool SB() => true;
//			private internal static bool SC() => true;
			internal static bool SD() => true;
			protected internal static bool SE() => true;
			public static bool SF() => true;

//			private virtual bool VA() => true;
			protected virtual bool VB() => true;
//			private virtual internal bool VC() => true;
			internal virtual bool VD() => true;
			protected internal virtual bool VE() => true;
			public virtual bool VF() => true;

			[DllImport("kernel32.dll",EntryPoint = "GetLastError")]
			private static extern uint EX();
		}

		private class MethodModifiersB: MethodModifiers {
//			private override bool VA() => true;
			protected override bool VB() => true;
//			private internal override bool VC() => true;
			internal override bool VD() => true;
			protected internal override bool VE() => true;
			public override bool VF() => true;
		}

		private class MethodModifiersC : MethodModifiers {
//			private sealed override bool VA() => true;
			protected sealed override bool VB() => true;
//			private internal sealed override bool VC() => true;
			internal sealed override bool VD() => true;
			protected internal sealed override bool VE() => true;
			public sealed override bool VF() => true;
		}

		[DataTestMethod]
		[DataRow(typeof(MethodModifiers), "A", "private bool A()")]
		[DataRow(typeof(MethodModifiers), "B", "protected bool B()")]
//		[-------(typeof(MethodModifiers), "C", "private internal bool C()")]
		[DataRow(typeof(MethodModifiers), "D", "internal bool D()")]
		[DataRow(typeof(MethodModifiers), "E", "protected internal bool E()")]
		[DataRow(typeof(MethodModifiers), "F", "public bool F()")]
		[DataRow(typeof(MethodModifiers), "SA", "private static bool SA()")]
		[DataRow(typeof(MethodModifiers), "SB", "protected static bool SB()")]
//		[-------(typeof(MethodModifiers), "SC", "private internal static bool SC()")]
		[DataRow(typeof(MethodModifiers), "SD", "internal static bool SD()")]
		[DataRow(typeof(MethodModifiers), "SE", "protected internal static bool SE()")]
		[DataRow(typeof(MethodModifiers), "SF", "public static bool SF()")]
//		[-------(typeof(MethodModifiers), "VA", "private virtual bool VA()")]
		[DataRow(typeof(MethodModifiers), "VB", "protected virtual bool VB()")]
//		[-------(typeof(MethodModifiers), "VC", "private internal virtual bool VC()")]
		[DataRow(typeof(MethodModifiers), "VD", "internal virtual bool VD()")]
		[DataRow(typeof(MethodModifiers), "VE", "protected internal virtual bool VE()")]
		[DataRow(typeof(MethodModifiers), "VF", "public virtual bool VF()")]
//		[-------(typeof(MethodModifiersB), "VA", "private override bool VA()")]
		[DataRow(typeof(MethodModifiersB), "VB", "protected override bool VB()")]
//		[-------(typeof(MethodModifiersB), "VC", "private internal override bool VC()")]
		[DataRow(typeof(MethodModifiersB), "VD", "internal override bool VD()")]
		[DataRow(typeof(MethodModifiersB), "VE", "protected internal override bool VE()")]
		[DataRow(typeof(MethodModifiersB), "VF", "public override bool VF()")]
//		[-------(typeof(MethodModifiersC), "VA", "private sealed override bool VA()")]
		[DataRow(typeof(MethodModifiersC), "VB", "protected sealed override bool VB()")]
//		[-------(typeof(MethodModifiersC), "VC", "private internal sealed override bool VC()")]
		[DataRow(typeof(MethodModifiersC), "VD", "internal sealed override bool VD()")]
		[DataRow(typeof(MethodModifiersC), "VE", "protected internal sealed override bool VE()")]
		[DataRow(typeof(MethodModifiersC), "VF", "public sealed override bool VF()")]
		[DataRow(typeof(MethodModifiers), "EX", "private static extern uint EX()")]
		public void Method_ForCompare_Test(Type type, string name, string result) {
			var mi=(MethodInfo)type.GetMember(name,BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}
		#endregion

		#region Constructors

		private class Constructors {
			public Constructors() { }

			static Constructors() { }
		}

		[DataTestMethod]
		[DataRow(typeof(Constructors), ".ctor",  "public .ctor()")]
		[DataRow(typeof(Constructors), ".cctor", "static .cctor()")]
		public void SigConstructorInfoTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}

		#endregion

		#region Destructors

		private class Destructors {

			~Destructors() {
				// protected override void Finalize()
			}
		}

		[DataTestMethod]
		[DataRow(typeof(Destructors), "Finalize", "~Destructors()")]
		public void SigDestructorInfoTest(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMethod(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly);
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}

		#endregion

		public class GenericClass1<T> {
			public T RM1() { return default; }
			public T[] RM2() { return default; }
			public T[,] RM3() { return default; }
			public List<T> RM4() { return default; }
			public List<T>[] RM5() { return default; }
			public List<T>[,] RM6() { return default; }
			public List<T[]> RM7() { return default; }
			public List<T[,]> RM8() { return default; }
			public List<T[,]>[] RM9() { return default; }

			public void PM1(T a) { }
			public void PM2(T[] a) { }
			public void PM3(T[,] a) { }
			public void PM4(List<T> a) { }
			public void PM5(List<T>[] a) { }
			public void PM6(List<T>[,] a) { }
			public void PM7(List<T[]> a) { }
			public void PM8(List<T[,]> a) { }
			public void PM9(List<T[,]>[] a) { }

			public void PM10(ref T a) { }
			public void PM11(ref T[] a) { }
			public void PM12(out T a) { a   = default; }
			public void PM13(out T[] a) { a = default; }
		}

		[DataTestMethod]
		[DataRow(typeof(GenericClass1<>), "RM1", "public T RM1()")]
		[DataRow(typeof(GenericClass1<>), "RM2", "public T[] RM2()")]
		[DataRow(typeof(GenericClass1<>), "RM3", "public T[,] RM3()")]
		[DataRow(typeof(GenericClass1<>), "RM4", "public System.Collections.Generic.List<T> RM4()")]
		[DataRow(typeof(GenericClass1<>), "RM5", "public System.Collections.Generic.List<T>[] RM5()")]
		[DataRow(typeof(GenericClass1<>), "RM6", "public System.Collections.Generic.List<T>[,] RM6()")]
		[DataRow(typeof(GenericClass1<>), "RM7", "public System.Collections.Generic.List<T[]> RM7()")]
		[DataRow(typeof(GenericClass1<>), "RM8", "public System.Collections.Generic.List<T[,]> RM8()")]
		[DataRow(typeof(GenericClass1<>), "RM9", "public System.Collections.Generic.List<T[,]>[] RM9()")]
		[DataRow(typeof(GenericClass1<>), "PM1", "public void PM1(T)")]
		[DataRow(typeof(GenericClass1<>), "PM2", "public void PM2(T[])")]
		[DataRow(typeof(GenericClass1<>), "PM3", "public void PM3(T[,])")]
		[DataRow(typeof(GenericClass1<>), "PM4", "public void PM4(System.Collections.Generic.List<T>)")]
		[DataRow(typeof(GenericClass1<>), "PM5", "public void PM5(System.Collections.Generic.List<T>[])")]
		[DataRow(typeof(GenericClass1<>), "PM6", "public void PM6(System.Collections.Generic.List<T>[,])")]
		[DataRow(typeof(GenericClass1<>), "PM7", "public void PM7(System.Collections.Generic.List<T[]>)")]
		[DataRow(typeof(GenericClass1<>), "PM8", "public void PM8(System.Collections.Generic.List<T[,]>)")]
		[DataRow(typeof(GenericClass1<>), "PM9", "public void PM9(System.Collections.Generic.List<T[,]>[])")]
		[DataRow(typeof(GenericClass1<>), "PM10", "public void PM10(ref T)")]
		[DataRow(typeof(GenericClass1<>), "PM11", "public void PM11(ref T[])")]
		[DataRow(typeof(GenericClass1<>), "PM12", "public void PM12(out T)")]
		[DataRow(typeof(GenericClass1<>), "PM13", "public void PM13(out T[])")]
		public void GenericMethod_ForCompare_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,AllBindingFlags)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}

#pragma warning disable 660,661
		class Op { //TODO overload operator

			// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/overloadable-operators
			// https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/operator-overloads

			// 	public static op_Equality(System.Reflection.MethodInfo left, System.Reflection.MethodInfo right)  => System.Reflection.MethodInfo.op_Equality(left, right);
			// op_Inequality

			// +, -, !, ~, ++, --, true, false	These unary operators can be overloaded.

			public static Op operator +(Op a) { return default; }
			public static Op operator -(Op a) { return default; }
			public static Op operator !(Op a) { return default; }
			public static Op operator ++(Op a) { return default; }
			public static Op operator --(Op a) { return default; }
			public static bool operator true(Op a) { return default; }
			public static bool operator false(Op a) { return default; }
			
			// ==, !=, <, >, <=, >=	The comparison operators can be overloaded (but see the note that follows this table).

			public static bool operator ==(Op a, Op b) { return default; }
			public static bool operator !=(Op a, Op b) { return default; }
			public static bool operator <(Op a, Op b) { return default; }
			public static bool operator >(Op a, Op b) { return default; }
			public static bool operator <=(Op a, Op b) { return default; }
			public static bool operator >=(Op a, Op b) { return default; }

			// +, -, *, /, %, &, |, ^, <<, >>	These binary operators can be overloaded.

			public static Op operator +(Op a, Op b) { return default; }
			public static Op operator -(Op a, Op b) { return default; }
			public static Op operator *(Op a, Op b) { return default; }
			public static Op operator /(Op a, Op b) { return default; }
			public static Op operator %(Op a, Op b) { return default; }
			public static Op operator &(Op a, Op b) { return default; }
			public static Op operator |(Op a, Op b) { return default; }
			public static Op operator ^(Op a, Op b) { return default; }
			public static Op operator <<(Op a, int b) { return default; }
			public static Op operator >>(Op a, int b) { return default; }


			public static implicit operator int(Op f) { return default; }
			public static explicit operator double(Op f) { return default; }

		}
#pragma warning restore 660,661

		[DataTestMethod]
		[DataRow(typeof(Extensions), "A", "public static void A(this KsWare.CodeGenerator.Tests.Generators.ExtendedClass extended)")]
		public void Extension_ForDeclare_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForDeclare.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Extensions), "A", "A(extended)")]
		public void Extension_ForCall_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForCall.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Extensions), "A", "A(KsWare.CodeGenerator.Tests.Generators.ExtendedClass)")]
		public void Extension_ForSignature_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForSignature.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Extensions), "A", "A(KsWare.CodeGenerator.Tests.Generators.ExtendedClass)")]
		public void Extension_ForInheriteDoc_Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForInheriteDoc.Generate(mi).Should().Be(result);
		}

		//TODO GenericMethod<T>

	}

	internal class ExtendedClass { }

	internal class ExtendedClass<T> { }

	internal static class Extensions { //TODO extension methods
		public static void A(this ExtendedClass extended) { }

//TODO	public static void A<T>(this ExtendedClass extended) { }

//TODO	public static void B<T>(this ExtendedClass<T> extended) { }
	}
}