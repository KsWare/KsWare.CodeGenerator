// ***********************************************************************
// Assembly         : KsWare.SignatureGenerator.Tests
// Author           : SchreinerK
// Created          : 2018-03-12
//
// Last Modified By : SchreinerK
// Last Modified On : 2018-03-13
// ***********************************************************************
// <copyright file="SignatureHelperTests.cs" company="KsWare">
//     Copyright © 2018
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable 169
#pragma warning disable 67

namespace KsWare.SignatureGenerator.Tests {

	[TestClass()]
	public class SignatureHelperTests {

		private const  BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                              BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		[DataTestMethod]
		[DataRow(typeof(System.Action<System.EventHandler<bool>>), "System.Action<System.EventHandler<bool>>")]
		[DataRow(typeof(System.Action<>), "System.Action<>")]
		[DataRow(typeof(System.Action<bool>), "System.Action<bool>")]
		[DataRow(typeof(System.Action<bool>[]), "System.Action<bool>[]")]
		[DataRow(typeof(string), "string")]
		[DataRow(typeof(string[]), "string[]")]
		[DataRow(typeof(System.Tuple), "System.Tuple")]
		[DataRow(typeof(System.Action), "System.Action")] // delegate
		[DataRow(typeof(System.TimeSpan[]), "System.TimeSpan[]")]
		[DataRow(typeof(System.Array), "System.Array")]
		[DataRow(typeof(string[,]), "string[,]")]
		[DataRow(typeof(string[][]), "string[][]")]
		[DataRow(typeof(string[][,]), "string[][,]")]
		public void SigTypeTest(Type type, string result) {
			SignatureHelper.ForCompare.Sig(type).Should().Be(result);
		}

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
		public void SigMethodInfoTest(Type type, string name, string result) {
			var mi=(MethodInfo)type.GetMember(name,BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(MethodModifiers), "SA", "private static bool SA()")] // Private | Static | HideBySig
		[DataRow(typeof(MethodModifiers), "EX", "private static extern uint EX()")]
		public void SigMethodInfo2Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Events {
			public event System.EventHandler A;								//                 | HideBySig | SpecialName
			public event System.EventHandler B { add { } remove { } }		//                 | HideBySig | SpecialName
			public static event System.EventHandler SA;						//          Static | HideBySig | SpecialName
		}

		[DataTestMethod]
		[DataRow(typeof(Events), "A", "public event System.EventHandler A")]
		[DataRow(typeof(Events), "SA", "public static event System.EventHandler SA")]
		[DataRow(typeof(Events), "B", "public event System.EventHandler B")]
		public void SigEventInfoTest(Type type, string name, string result) {
			var mi = (EventInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Properties {

			private bool A1 { get; set; }
			protected bool B1 { get; set; }
			internal bool C1 { get; set; }
			protected internal bool D1 { get; set; }
			public bool F1 { get; set; }

			protected internal bool D2 { get; protected set; }
			public bool F2 { get; protected set; }

			protected internal bool D3 { get; internal set; }
			public bool F3 { get; internal set; }

			public bool F4 { get; protected internal set; }

			internal bool C3 { get; private set; }


			public virtual bool VA { get; private set; }
			public virtual bool VB { get; internal set; }
			public virtual bool VC { get; set; }


			public static bool SF { get; set; }

		}
		
		[DataTestMethod]
		[DataRow(typeof(Properties), "A1", "private bool A1 { get; set; }")]
		[DataRow(typeof(Properties), "B1", "protected bool B1 { get; set; }")]
		[DataRow(typeof(Properties), "C1", "internal bool C1 { get; set; }")]
		[DataRow(typeof(Properties), "D1", "protected internal bool D1 { get; set; }")]
		[DataRow(typeof(Properties), "F1", "public bool F1 { get; set; }")]
		[DataRow(typeof(Properties), "D2", "protected internal bool D2 { get; protected set; }")]
		[DataRow(typeof(Properties), "F2", "public bool F2 { get; protected set; }")]
		[DataRow(typeof(Properties), "D3", "protected internal bool D3 { get; internal set; }")]
		[DataRow(typeof(Properties), "F3", "public bool F3 { get; internal set; }")]
		[DataRow(typeof(Properties), "F4", "public bool F4 { get; protected internal set; }")]
		[DataRow(typeof(Properties), "C3", "internal bool C3 { get; private set; }")]
		[DataRow(typeof(Properties), "VA", "public virtual bool VA { get; private set; }")]
		[DataRow(typeof(Properties), "VB", "public virtual bool VB { get; internal set; }")]
		[DataRow(typeof(Properties), "VC", "public virtual bool VC { get; set; }")]
		public void SigPropertyInfoTest(Type type, string name, string result) {
			var mi = type.GetProperty(name,BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | 
				BindingFlags.DeclaredOnly);
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Properties2 {

			//TODO A has same signature as B

			public bool A { get; }

			public bool B => false;

			public bool C { set { } }
		}

		[DataTestMethod]
		[DataRow(typeof(Properties2), "A",  "public bool A { get; }")]
		[DataRow(typeof(Properties2), "B",   "public bool B { get; }")]
		[DataRow(typeof(Properties2), "C",  "public bool C { set; }")]
		public void SigfPropertyInfo2Test(Type type, string name, string result) {
			var mi = (PropertyInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Properties3 {

			private static System.ValueTuple structure;

			public System.ValueTuple RF0 => structure;
			public ref System.ValueTuple RF => ref structure;                   //C# 7.2
			public static ref System.ValueTuple SRF => ref structure;           //C# 7.2
			public ref readonly System.ValueTuple RRF => ref structure;         //C# 7.2
			public static ref readonly System.ValueTuple SRRF => ref structure; //C# 7.2

		}

		[DataTestMethod, Ignore]
		[DataRow(typeof(Properties3), "RF0",  "public System.ValueTuple RF0 { get; }")]
		[DataRow(typeof(Properties3), "RF",  "public ref System.ValueTuple RF { get; }")]
		[DataRow(typeof(Properties3), "SRF", "public static ref System.ValueTuple SRRF { get; }")]
		[DataRow(typeof(Properties3), "RRF", "public ref readonly System.ValueTuple RRF { get; }")]
		[DataRow(typeof(Properties3), "SRRF", "public static ref readonly System.ValueTuple SRRF { get; }")]
		public void SigRefPropertyInfoTest(Type type, string name, string result) {
			var mi = (PropertyInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Fields {
			public bool A;
			public static bool SA;
			public const bool CA = true;
			public readonly bool RA = true;
			public static readonly bool SRA = true;
		}

		[DataTestMethod]
		[DataRow(typeof(Fields), "A", "public bool A")]
		[DataRow(typeof(Fields), "SA", "public static bool SA")]
		[DataRow(typeof(Fields), "CA", "public const bool CA")]
		[DataRow(typeof(Fields), "RA", "public readonly bool RA")]
		[DataRow(typeof(Fields), "SRA", "public static readonly bool SRA")]
		public void SigFieldInfoTest(Type type, string name, string result) {
			var mi = (FieldInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
		
		private class Constructors {
			public Constructors() { }

			static Constructors() { }
		}

		[DataTestMethod]
		[DataRow(typeof(Constructors), ".ctor", "public .ctor()")]
		[DataRow(typeof(Constructors), ".cctor", "static .cctor()")]
		public void SigConstructorInfoTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Destructors {

			~Destructors() {
				// protected override void Finalize()
			}
		}

		[DataTestMethod]
		[DataRow(typeof(Destructors), "Finalize",  "~Destructors()")]
		public void SigDestructorInfoTest(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMethod(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly);
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Parameters {

			public void A() { }
			public void B(bool a) { }

			public void C(bool[] a) { }

			public void D(params bool[] a) { } // same signature as C

			public void E(ref bool a) { }

			public void F(out bool a) { a = false; }
		}

		[DataTestMethod]
		[DataRow(typeof(Parameters), "A",  "public void A()")]
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

		public class Indexer {
			public string this[string a] { get { return null; } set { } }
		}

		[DataTestMethod]
		[DataRow(typeof(Indexer), "Item", "public string this[string] { get; set; }")]
		public void SigIndexerTest(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
		
		[DataTestMethod,Ignore] //TODO AbstractTest
		[DataRow(typeof(Parameters), "",  "")]
		public void AbstractTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,AllBindingFlags)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		[DataTestMethod, Ignore] //TODO IndexerTest
		[DataRow(typeof(Parameters), "", "")]
		public void IndexerTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		#region

		public interface IPublicInterface {}
		internal interface IInternalInterface { }
		private interface IPrivateInterface { }

		public class PublicClass { }
		internal class InternalClass { }
		private class PrivateClass { }

		public abstract class PublicAbstractClass { }
		public sealed class PublicSealedClass { }

		public interface IGenericInterface1<T> { }

		public interface IGenericInterface2<T1, T2> { }

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
		public void SigGenericMethodTest(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,AllBindingFlags)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
		[DataTestMethod]
		[DataRow(typeof(GenericClass1<>), "RM2", "public T[] RM2()")]
		public void SigGenericMethod1Test(Type type, string name, string result) {
			var mi = (MethodInfo) type.GetMember(name,AllBindingFlags)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		[DataTestMethod,Ignore] //TODO
		[DataRow(typeof(IPublicInterface), "public interface IPublicInterface")]
		[DataRow(typeof(IInternalInterface), "internal interface IInternalInterface")]
		[DataRow(typeof(IPrivateInterface), "private interface IPrivateInterface")]
		[DataRow(typeof(PublicClass), "public interface PublicClass")]
		[DataRow(typeof(InternalClass), "internal interface InternalClass")]
		[DataRow(typeof(PrivateClass), "private interface PrivateClass")]
		[DataRow(typeof(PublicAbstractClass), "public abstract class PublicAbstractClass")]
		[DataRow(typeof(PublicSealedClass), "public sealed class PublicSealedClass")]
		[DataRow(typeof(IGenericInterface1<>), "public interface IGenericInterface1<T>")]
		[DataRow(typeof(IGenericInterface2<,>), "public interface IGenericInterface2<T1, T2>")]
		[DataRow(typeof(GenericClass1<>), "public class GenericClass1<T>")]
		public void TypeCodeTest(Type type, string result) {
			SignatureHelper.ForCode.Sig(type).Should().Be(result);
		}

		#endregion

		#region

		[DataTestMethod] 
		[DataRow(typeof(IGenericInterface1<>), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.IGenericInterface1<>")]
		[DataRow(typeof(IGenericInterface2<,>), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.IGenericInterface2<, >")]
		[DataRow(typeof(GenericClass1<>), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.GenericClass1<>")]
		public void GenericType_ForCompare_Test(Type type, string result) {
			SignatureHelper.ForCompare.Sig(type).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(IGenericInterface1<int>     ), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.IGenericInterface1<int>")]
		[DataRow(typeof(IGenericInterface2<int,bool>), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.IGenericInterface2<int, bool>")]
		[DataRow(typeof(GenericClass1<int>          ), "KsWare.SignatureGenerator.Tests.SignatureHelperTests.GenericClass1<int>")]
		public void GenericTypeName2Test(Type type, string result) {
			SignatureHelper.ForCompare.Sig(type).Should().Be(result);
		}

		#endregion
		//TODO abstract
		//TODO Attributes
	}

	internal interface IInternalInterface{}

	internal interface IInternalGenericInterface1<T> { }

	internal interface IInternalGenericInterface2<T1,T2> { }

	internal class InternalClass { }

	internal class InternalGenericClass1<T> { }

	internal class InternalGenericClass2<T1, T2> { }

}