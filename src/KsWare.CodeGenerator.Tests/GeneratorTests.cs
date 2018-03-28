// ***********************************************************************
// Assembly         : KsWare.CodeGenerator.Tests
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

namespace KsWare.CodeGenerator.Tests {

	[TestClass()]
	public class GeneratorTests {

		private const  BindingFlags AllBindingFlags = BindingFlags.Instance  | BindingFlags.Static | BindingFlags.Public |
		                                              BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
		
		[DataTestMethod,Ignore] //TODO AbstractTest
		[DataRow(typeof(void), "",  "")]
		public void AbstractTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,AllBindingFlags)[0];
			Generator.ForCompare.Generate(mi).Should().Be(result);
		}

		//TODO Attributes
	}



}