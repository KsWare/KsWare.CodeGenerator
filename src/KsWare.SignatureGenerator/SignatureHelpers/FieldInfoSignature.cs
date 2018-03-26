using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class FieldInfoSignature : BaseSignature {

		public FieldInfoSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }

		/// <summary>
		/// Creates signature for the specified field information.
		/// </summary>
		/// <param name="fieldInfo">The field information.</param>
		/// <returns>System.String.</returns>
		public string Sig(FieldInfo fieldInfo) {
			var sb = new StringBuilder();

			sb.Append($"{SignatureHelper.Sig(fieldInfo.Attributes)}");
			sb.Append(SignatureHelper.Sig(fieldInfo.FieldType));
			sb.Append(" ");
			sb.Append(fieldInfo.Name);

			return sb.ToString();
//			return $"field {fieldInfo} // not implemented";
		}

		
	}
}
