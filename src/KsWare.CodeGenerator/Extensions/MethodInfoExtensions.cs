﻿// ***********************************************************************
// Assembly         : KsWare.CodeGenerator
// Author           : SchreinerK
// Created          : 2018-03-29
//
// Last Modified By : SchreinerK
// Last Modified On : 2018-03-29
// ***********************************************************************
// <copyright file="MethodInfoExtensions.cs" company="KsWare">
//     Copyright © 2018 KsWare. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.CodeGenerator.Extensions {

	/// <summary>
	/// Class MethodInfoExtensions.
	/// </summary>
	/// <autogeneratedoc />
	public static class MethodInfoExtensions {

		/// <summary>
		/// Determines whether the specified method is overloaded operator.
		/// </summary>
		/// <param name="m">The method.</param>
		/// <returns><c>true</c> if is operator overload; otherwise, <c>false</c>.</returns>
		public static bool IsOperatorOverload(this MethodInfo m) {
			return m.IsSpecialName && m.IsStatic && m.Name.StartsWith("op_"); 
		}

		/// <summary>
		/// Determines whether the specified method is accessor for property or events.
		/// </summary>
		/// <param name="m">The method inforamtion</param>
		/// <returns><c>true</c> if is accessor; otherwise, <c>false</c>.</returns>
		public static bool IsAccessor(this MethodInfo m) {
			return m.IsSpecialName && (m.Name.StartsWith("get_") || m.Name.StartsWith("set_") ||
			                           m.Name.StartsWith("add_") || m.Name.StartsWith("remove_"));
		}
	}
}
