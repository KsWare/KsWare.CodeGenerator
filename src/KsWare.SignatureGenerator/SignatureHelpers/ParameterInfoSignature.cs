using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class ParameterInfoSignature : BaseSignature {

		public ParameterInfoSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }

		/// <summary>
		/// Creates signature for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ParameterInfo parameterInfo) => Sig(parameterInfo, PropertySignatureOptions.Create(SignatureMode));

		/// <summary>
		/// Creates signature for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <param name="options">The signature options.</param>
		/// <returns>System.String.</returns>
		public string Sig(ParameterInfo parameterInfo, PropertySignatureOptions options) {
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

			switch (SignatureMode) {
				case SignatureMode.Compare:
				case SignatureMode.CompareIgnoreReturnType:
				case SignatureMode.InheriteDoc:
					var sig                      = SignatureHelper.Sig(parameterInfo.ParameterType);
					if (parameterInfo.IsOut) sig = "out " + sig.Substring(4);
					sb.Append(sig);
					break;
				case SignatureMode.Code:
					sb.Append(SignatureHelper.Sig(parameterInfo.ParameterType));
					sb.Append(" " + parameterInfo.Name);
					break;
			}

			return sb.ToString();
		}

		
	}
}
