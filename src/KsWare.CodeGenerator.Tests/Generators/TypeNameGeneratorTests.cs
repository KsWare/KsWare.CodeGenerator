﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	public class TypeNameGeneratorTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		[DataTestMethod]
		[DataRow(typeof(System.Action<System.EventHandler<bool>>), "System.Action<System.EventHandler<bool>>")]
		[DataRow(typeof(System.Action<>),                          "System.Action<T>")]
		[DataRow(typeof(System.Action<bool>),                      "System.Action<bool>")]
		[DataRow(typeof(System.Action<bool>[]),                    "System.Action<bool>[]")]
		[DataRow(typeof(string),                                   "string")]
		[DataRow(typeof(string[]),                                 "string[]")]
		[DataRow(typeof(System.Tuple),                             "System.Tuple")]
		[DataRow(typeof(System.Action),                            "System.Action")] // delegate
		[DataRow(typeof(System.TimeSpan[]),                        "System.TimeSpan[]")]
		[DataRow(typeof(System.Array),                             "System.Array")]
		[DataRow(typeof(string[,]),                                "string[,]")]
		[DataRow(typeof(string[][]),                               "string[][]")]
		[DataRow(typeof(string[][,]),                              "string[][,]")]
		public void SigTypeTest(Type type, string result) {
			Generator.ForCompare.Generate(type).Should().Be(result);
		}

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
//TODO	public new partial interface IGenericInterface3<out T1, in T2, T3> {}

		public class GenericClass1<T> {}


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
			Generator.ForCode.Generate(type).Should().Be(result);
		}

		[DataTestMethod] 
		[DataRow(typeof(IGenericInterface1<>), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.IGenericInterface1<T>")]
		[DataRow(typeof(IGenericInterface2<,>), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.IGenericInterface2<T1, T2>")]
		[DataRow(typeof(GenericClass1<>), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.GenericClass1<T>")]
		public void GenericType_ForCompare_Test(Type type, string result) {
			//TODO revise <> and <T> for compare because T is a name which can be changed without loose of compatibility
			Generator.ForCompare.Generate(type).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(IGenericInterface1<int>     ), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.IGenericInterface1<int>")]
		[DataRow(typeof(IGenericInterface2<int,bool>), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.IGenericInterface2<int, bool>")]
		[DataRow(typeof(GenericClass1<int>          ), "KsWare.CodeGenerator.Tests.Generators.TypeNameGeneratorTests.GenericClass1<int>")]
		public void GenericTypeName2Test(Type type, string result) {
			Generator.ForCompare.Generate(type).Should().Be(result);
		}


	}

	internal interface IInternalInterface { }

	internal interface IInternalGenericInterface1<T> { }

	internal interface IInternalGenericInterface2<T1, T2> { }

	internal class InternalClass { }

	internal class InternalGenericClass1<T> { }

	internal class InternalGenericClass2<T1, T2> { }

}