using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class MethodInfoSignature : BaseSignature {

		public MethodInfoSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }

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
