﻿// ***********************************************************************
// Assembly         : KsWare.SignatureGenerator
// Author           : SchreinerK
// Created          : 03-26-2018
//
// Last Modified By : SchreinerK
// Last Modified On : 03-27-2018
// ***********************************************************************
// <copyright file="MethodSignatureGenerator.cs" company="KsWare">
//     Copyright © 2018 KsWare. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;
using System.Text;

namespace KsWare.SignatureGenerator.Generators {

	/// <summary>
	/// Class MethodSignatureGenerator.
	/// </summary>
	/// <seealso cref="KsWare.SignatureGenerator.Generators.BaseSignatureGenerator" />
	/// <autogeneratedoc />
	public class MethodSignatureGenerator : BaseSignatureGenerator {

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodSignatureGenerator"/> class.
		/// </summary>
		/// <param name="signatureHelper">The signature helper.</param>
		/// <autogeneratedoc />
		public MethodSignatureGenerator(SignatureHelper signatureHelper) : base(signatureHelper) { }

		/// <summary>
		/// Creates signature for the specified method information.
		/// </summary>
		/// <param name="methodInfo">The method information.</param>
		/// <returns>System.String.</returns>
		public string Sig(MethodInfo methodInfo) => Sig(methodInfo, MethodSignatureOptions.Create(SignatureMode));

		/// <summary>
		/// Creates signature for the specified method information.
		/// </summary>
		/// <param name="methodInfo">The method information.</param>
		/// <param name="options">The signature options.</param>
		/// <returns>System.String.</returns>
		public string Sig(MethodInfo methodInfo, MethodSignatureOptions options) {
			var sb = new StringBuilder();

			if (options.Access    ) sb.Append(SignatureHelper.SigAccess  (methodInfo.Attributes));
			if (options.Modifiers ) sb.Append(SignatureHelper.SigModifier(methodInfo.Attributes));
			if (options.ReturnType) sb.Append(SignatureHelper.Sig        (methodInfo.ReturnType) + " ");
			if (options.Name      ) sb.Append(methodInfo.Name);

			//TODO use options
			sb.Append("(");
			sb.Append(SignatureHelper.Sig(methodInfo.GetParameters()));
			sb.Append(")");

			if (sb.ToString() == "protected override void Finalize()")
				return $"~{methodInfo.DeclaringType.Name}()"; // Destructor
			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the specified constructor information.
		/// </summary>
		/// <param name="constructorInfo">The constructor information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ConstructorInfo constructorInfo) {
			var sb = new StringBuilder();

			if (constructorInfo.IsStatic) sb.Append("static ");
			else sb.Append($"{SignatureHelper.Sig(constructorInfo.Attributes)}");


			sb.Append(constructorInfo.Name);
			sb.Append("(");
			sb.Append(SignatureHelper.Sig(constructorInfo.GetParameters()));
			sb.Append(")");
			return sb.ToString();
		}

		
	}
}