using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.CodeGenerator.Tests.Generators {

	[TestClass()]
	public class PropertyGeneratorTests {

		private const BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private class PropertiesBase {
			public virtual bool OC { get; set; }

			public virtual bool SOC { get; set; }
		}

		private class Properties : PropertiesBase {

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

			public override bool OC { get; set; }

			public sealed override bool SOC { get; set; }
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
		[DataRow(typeof(Properties), "OC", "public override bool OC { get; set; }")]
		[DataRow(typeof(Properties), "SOC", "public sealed override bool SOC { get; set; }")]
		public void GenerateProperty_ForDeclare_Test(Type type, string name, string result) {
			var mi = type.GetProperty(name,AllBindingFlags);
			Generator.ForDeclare.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Properties), "A1",  "private bool A1 { get; set; }")]
		[DataRow(typeof(Properties), "B1",  "protected bool B1 { get; set; }")]
		[DataRow(typeof(Properties), "C1",  "internal bool C1 { get; set; }")]
		[DataRow(typeof(Properties), "D1",  "protected internal bool D1 { get; set; }")]
		[DataRow(typeof(Properties), "F1",  "public bool F1 { get; set; }")]
		[DataRow(typeof(Properties), "D2",  "protected internal bool D2 { get; protected set; }")]
		[DataRow(typeof(Properties), "F2",  "public bool F2 { get; protected set; }")]
		[DataRow(typeof(Properties), "D3",  "protected internal bool D3 { get; internal set; }")]
		[DataRow(typeof(Properties), "F3",  "public bool F3 { get; internal set; }")]
		[DataRow(typeof(Properties), "F4",  "public bool F4 { get; protected internal set; }")]
		[DataRow(typeof(Properties), "C3",  "internal bool C3 { get; private set; }")]
		[DataRow(typeof(Properties), "VA",  "public virtual bool VA { get; private set; }")]
		[DataRow(typeof(Properties), "VB",  "public virtual bool VB { get; internal set; }")]
		[DataRow(typeof(Properties), "VC",  "public virtual bool VC { get; set; }")]
		[DataRow(typeof(Properties), "OC",  "public override bool OC { get; set; }")]
		[DataRow(typeof(Properties), "SOC", "public sealed override bool SOC { get; set; }")]
		public void GenerateProperty_ForCompare_Test(Type type, string name, string result) {
			var mi = type.GetProperty(name, AllBindingFlags);
			Generator.ForCompare.Generate(mi).Should().Be(result);
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
			var mi = (PropertyInfo) type.GetMember(name,AllBindingFlags)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
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
		[DataRow(typeof(Properties3), "RF0", "public System.ValueTuple RF0 { get; }")]
		[DataRow(typeof(Properties3), "RF",  "public ref System.ValueTuple RF { get; }")]
		[DataRow(typeof(Properties3), "SRF", "public static ref System.ValueTuple SRRF { get; }")]
		[DataRow(typeof(Properties3), "RRF", "public ref readonly System.ValueTuple RRF { get; }")]
		[DataRow(typeof(Properties3), "SRRF", "public static ref readonly System.ValueTuple SRRF { get; }")]
		public void GenerateProperty_Declare_2_Test(Type type, string name, string result) {
			var mi = (PropertyInfo) type.GetMember(name,AllBindingFlags)[0];
			Generator.ForDeclare.Generate(mi).Should().Be(result);
		}


		#region Indexer

		public class Indexer1 {
			public string this[string a] { get { return null; } set { } }
		}

		public class Indexer2 {
			public string this[string a, int b] { get { return null; } set { } }
		}

		public class Indexer3 {
			public string this[string a, int b = 1] { get { return null; } set { } }
		}

		public class Indexer4 {
			public string this[string a, params int[] b] { get { return null; } set { } }
		}

		[DataTestMethod]
		[DataRow(typeof(Indexer1), "Item", "this[string] { get; set; }")]
		[DataRow(typeof(Indexer2), "Item", "this[string, int] { get; set; }")]
		[DataRow(typeof(Indexer3), "Item", "this[string, int] { get; set; }")]
		[DataRow(typeof(Indexer4), "Item", "this[string, int[]] { get; set; }")]
		public void GenerateProperty_ForSignature_Indexer_Test(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForSignature.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Indexer1), "Item", "public string this[string] { get; set; }")]
		[DataRow(typeof(Indexer2), "Item", "public string this[string, int] { get; set; }")]
		[DataRow(typeof(Indexer3), "Item", "public string this[string, int] { get; set; }")]
		[DataRow(typeof(Indexer4), "Item", "public string this[string, params int[]] { get; set; }")]
		public void GenerateProperty_ForCompare_Indexer_Test(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}

		[DataTestMethod]
		[DataRow(typeof(Indexer1), "Item", "public string this[string a] { get; set; }")]
		[DataRow(typeof(Indexer2), "Item", "public string this[string a, int b] { get; set; }")]
//TODO	[DataRow(typeof(Indexer3), "Item", "public string this[string a, int b = 1] { get; set; }")]
		[DataRow(typeof(Indexer4), "Item", "public string this[string a, params int[] b] { get; set; }")]
		public void GenerateProperty_ForDeclare_Indexer_Test(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForDeclare.Generate(mi).Should().Be(result);
		}

//TODO	[DataTestMethod,Ignore]
		[DataRow(typeof(Indexer1), "Item", "this[a]")]
		[DataRow(typeof(Indexer2), "Item", "this[a, b]")]
		[DataRow(typeof(Indexer3), "Item", "this[a, b]")]
		[DataRow(typeof(Indexer4), "Item", "this[a, b]")]
		public void GenerateProperty_ForCall_Indexer_Test(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForCall.Generate(mi).Should().Be(result);
		}
		
//TODO	[DataTestMethod, Ignore]
		[DataRow(typeof(Indexer1), "Item", "this[string]")]
		[DataRow(typeof(Indexer2), "Item", "this[string, int]")]
		[DataRow(typeof(Indexer3), "Item", "this[string a, int]")]
		[DataRow(typeof(Indexer4), "Item", "this[string a, int[]]")]
		public void GenerateProperty_ForInheriteDoc_Indexer_Test(Type type, string name, string result) {
			// Debug.WriteLine(string.Join("\n",type.GetMembers(AllBindingFlags).Select(m=>m.Name)));
			var mi = (PropertyInfo) type.GetMember(name, AllBindingFlags)[0];
			Generator.ForInheriteDoc.Generate(mi).Should().Be(result);
		}	

		#endregion

		
	}

}