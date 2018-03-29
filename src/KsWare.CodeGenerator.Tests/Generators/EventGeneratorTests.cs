using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	public class EventGeneratorTests {

		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

#pragma warning disable 67
		private class Events {
			public event System.EventHandler A;                       //                 | HideBySig | SpecialName
			public event System.EventHandler B { add { } remove { } } //                 | HideBySig | SpecialName
			public static event System.EventHandler SA;               //          Static | HideBySig | SpecialName
		}
#pragma warning restore 67

		[DataTestMethod]
		[DataRow(typeof(Events), "A",  "public event System.EventHandler A")]
		[DataRow(typeof(Events), "SA", "public static event System.EventHandler SA")]
		[DataRow(typeof(Events), "B",  "public event System.EventHandler B")]
		public void SigEventInfoTest(Type type, string name, string result) {
			var mi = (EventInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}
	}

}