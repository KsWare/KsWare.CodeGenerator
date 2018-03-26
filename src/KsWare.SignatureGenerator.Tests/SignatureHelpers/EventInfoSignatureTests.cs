using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.SignatureGenerator.Tests.SignatureHelpers {

	[TestClass()]
	public class EventInfoSignatureTests {
		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class Events {
			public event System.EventHandler A;                       //                 | HideBySig | SpecialName
			public event System.EventHandler B { add { } remove { } } //                 | HideBySig | SpecialName
			public static event System.EventHandler SA;               //          Static | HideBySig | SpecialName
		}

		[DataTestMethod]
		[DataRow(typeof(Events), "A",  "public event System.EventHandler A")]
		[DataRow(typeof(Events), "SA", "public static event System.EventHandler SA")]
		[DataRow(typeof(Events), "B",  "public event System.EventHandler B")]
		public void SigEventInfoTest(Type type, string name, string result) {
			var mi = (EventInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
	}

}