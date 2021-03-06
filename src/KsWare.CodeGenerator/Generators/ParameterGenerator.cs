﻿// ***********************************************************************
// Assembly         : KsWare.CodeGenerator
// Author           : SchreinerK
// Created          : 03-26-2018
//
// Last Modified By : SchreinerK
// Last Modified On : 03-27-2018
// ***********************************************************************
// <copyright file="ParameterGenerator.cs" company="KsWare">
//     Copyright © 2018 KsWare. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace KsWare.CodeGenerator.Generators {

	/// <summary>
	/// Class ParameterGenerator.
	/// </summary>
	/// <seealso cref="BaseGenerator" />
	/// <autogeneratedoc />
	public class ParameterGenerator : BaseGenerator {
		// Modifier Type Name  default
		//          T    name  = value
		// out      T    name  
		// ref      T    name
		// params  T[]   name

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterGenerator"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <autogeneratedoc />
		public ParameterGenerator(Generator generator) : base(generator) { }

		/// <summary>
		/// Generates code for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <returns>System.String.</returns>
		public string Generate(ParameterInfo parameterInfo) => Generate(parameterInfo, ParameterGeneratorOptions.Create(GeneratorMode));

		/// <summary>
		/// Generates code for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <param name="options">The generator options.</param>
		/// <returns>System.String.</returns>
		public string Generate(ParameterInfo parameterInfo, ParameterGeneratorOptions options) {
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#method-parameters

			var sb = new StringBuilder();
			//Attributes?

//			parameterInfo.IsIn;
//			parameterInfo.IsOut;
//			parameterInfo.IsRetval;
//			parameterInfo.IsLcid;
//			parameterInfo.HasDefaultValue;
//TODO		parameterInfo.DefaultValue;
//			parameterInfo.RawDefaultValue;
//			parameterInfo.IsOptional;


			var isThis = parameterInfo.Position == 0 &&
			             parameterInfo.Member.CustomAttributes.Any(a => a.AttributeType == typeof(ExtensionAttribute));
			var isParams = parameterInfo.CustomAttributes.Any(a => a.AttributeType == typeof(ParamArrayAttribute));

			var type = Generator.Generate(parameterInfo.ParameterType) + " ";

			var modifier = "";

			if (type.StartsWith("ref ")) { // (parameterInfo.ParameterType.IsByRef)
				modifier = "ref ";
				type = type.Substring(4);
			}
			if (isThis) modifier = "this ";
			else if (parameterInfo.IsOut) modifier = "out ";
			else if (isParams) modifier = "params ";

			// "params" and "this" are relevant only for declare and compare
			switch (GeneratorMode) {
				case GeneratorMode.Compare:case GeneratorMode.CompareIgnoreReturnType:case GeneratorMode.Declare: break;
				default: if(isParams || isThis) modifier = ""; break;
			}

			if (options.Modifiers) sb.Append(modifier);
			if (options.Type     ) sb.Append(type);
			if (options.Name     ) sb.Append(parameterInfo.Name);

			//TODO default value

			return sb.ToString().Trim();
		}

		
	}
}
