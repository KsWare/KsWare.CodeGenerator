using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class PropertyInfoSignature : BaseSignature {

		public PropertyInfoSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }

		/// <summary>
		/// Creates signature for the specified property information.
		/// </summary>
		/// <param name="propertyInfo">The property information.</param>
		/// <returns>System.String.</returns>
		public string Sig(PropertyInfo propertyInfo) => Sig(propertyInfo, PropertySignatureOptions.Create(SignatureMode));

		/// <summary>
		/// Creates signature for the specified property information.
		/// </summary>
		/// <param name="propertyInfo">The property information.</param>
		/// <param name="options">The signature options.</param>
		/// <returns>System.String.</returns>
		public string Sig(PropertyInfo propertyInfo, PropertySignatureOptions options) {
			var sb     = new StringBuilder();
			var getter = propertyInfo.GetMethod;
			var setter = propertyInfo.SetMethod;

			string access = "";
			if (getter != null && setter != null) {
				var getterAccess = SignatureHelper.SigAccess(getter.Attributes);
				var setterAccess = SignatureHelper.SigAccess(setter.Attributes);
				access = SignatureHelper.MaxAccess(getterAccess, setterAccess) + " ";
			}
			else if (getter != null) {
				access = SignatureHelper.SigAccess(getter.Attributes);
			}
			else if (setter != null) {
				access = SignatureHelper.SigAccess(setter.Attributes);
			}

			var mi = getter ?? setter;

			sb.Append(access);
			sb.Append(SignatureHelper.SigModifier(mi.Attributes));

			sb.Append(SignatureHelper.Sig(propertyInfo.PropertyType));
			sb.Append(" ");
			if (propertyInfo.Name == "Item" && propertyInfo.GetMethod.GetParameters().Length > 0) {
				sb.Append("this[");
				sb.Append(SignatureHelper.Sig(propertyInfo.GetMethod.GetParameters()));
				sb.Append("]");
			}
			else {
				sb.Append(propertyInfo.Name);
			}

			sb.Append(" { ");

			if (propertyInfo.CanRead) {
				var getterAccess = SignatureHelper.SigAccess(getter.Attributes);
				if (getterAccess != access) sb.Append(getterAccess);
				sb.Append("get; ");
			}
			if (propertyInfo.CanWrite) {
				var setterAccess = SignatureHelper.SigAccess(setter.Attributes);
				if (setterAccess != access) sb.Append(setterAccess);
				sb.Append("set; ");
			}
			sb.Append("}");

			return sb.ToString();
		}
	}
}
