﻿// ***********************************************************************
// Assembly         : KsWare.CodeGenerator
// Author           : SchreinerK
// Created          : 03-27-2018
//
// Last Modified By : SchreinerK
// Last Modified On : 03-27-2018
// ***********************************************************************
// <copyright file="MethodGeneratorOptions.cs" company="KsWare">
//     Copyright © 2018 KsWare. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace KsWare.CodeGenerator.Generators {

	/// <summary>
	/// Class MethodGeneratorOptions.
	/// </summary>
	/// <autogeneratedoc />
	public class MethodGeneratorOptions {

		private const bool X = true;
		private const bool o = false;
		private static Dictionary<GeneratorMode, bool[]> matrix = new Dictionary<GeneratorMode, bool[]> {
			/*                                             Access                               */
			/*                                             |  Modifiers                         */
			/*                                             |  |  ReturnType                     */
			/*                                             |  |  |  Name                        */
			{GeneratorMode.Code                   , new[] {X, X, X, X}},
			{GeneratorMode.Compare                , new[] {X, X, X, X}},
			{GeneratorMode.CompareIgnoreReturnType, new[] {X, X, o, X}},
			{GeneratorMode.InheriteDoc            , new[] {o, o, o, X}},
			{GeneratorMode.Signature              , new[] {o, o, o, X}},
			{GeneratorMode.Call                   , new[] {o, o, o, X}},
			{GeneratorMode.Declare                , new[] {X, X, X, X}},
		};

		/// <summary>
		/// Gets or sets a value indicating whether access is included.
		/// </summary>
		/// <value><c>true</c> if access is included; otherwise, <c>false</c>.</value>
		public bool Access { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this modifiers are included.
		/// </summary>
		/// <value><c>true</c> if modifiers are included; otherwise, <c>false</c>.</value>
		/// <autogeneratedoc />
		public bool Modifiers { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether return type is included.
		/// </summary>
		/// <value><c>true</c> if return type is included; otherwise, <c>false</c>.</value>
		public bool ReturnType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether name is included.
		/// </summary>
		/// <value><c>true</c> if name is included; otherwise, <c>false</c>.</value>
		public bool Name { get; set; }

		/// <summary>
		/// Gets or sets the parameter options.
		/// </summary>
		/// <value>The parameter options.</value>
		/// <autogeneratedoc />
		public ParameterGeneratorOptions Parameter { get; set; }

		/// <summary>
		/// Creates the options for the specified generator mode.
		/// </summary>
		/// <param name="generatorMode">The generator mode.</param>
		/// <returns>MethodGeneratorOptions.</returns>
		/// <autogeneratedoc />
		public static MethodGeneratorOptions Create(GeneratorMode generatorMode) {
			var m = matrix[generatorMode];
			return new MethodGeneratorOptions {
				Access     = m[0],
				Modifiers  = m[1],
				ReturnType = m[2],
				Name       = m[3],
				Parameter  = ParameterGeneratorOptions.Create(generatorMode)
			};
		}
	}

}