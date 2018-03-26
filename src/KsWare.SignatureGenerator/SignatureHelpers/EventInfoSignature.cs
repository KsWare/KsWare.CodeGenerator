using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class EventInfoSignature : BaseSignature {


		public EventInfoSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }


		/// <summary>
		/// Creates signature for the specified event information.
		/// </summary>
		/// <param name="eventInfo">The event information.</param>
		/// <returns>System.String.</returns>
		public string Sig(EventInfo eventInfo) => Sig(eventInfo, EventSignatureOptions.Create(SignatureMode));

		/// <summary>
		/// Creates signature for the specified event information.
		/// </summary>
		/// <param name="eventInfo">The event information.</param>
		/// <param name="options">The signature options.</param>
		/// <returns>System.String.</returns>
		public string Sig(EventInfo eventInfo, EventSignatureOptions options) {
			var sb = new StringBuilder();
			var mi = eventInfo.AddMethod; // TODO

			sb.Append($"{SignatureHelper.Sig(mi.Attributes)}");
			sb.Append("event ");
			sb.Append($"{eventInfo.EventHandlerType} ");
			sb.Append($"{eventInfo.Name}");

			return sb.ToString();
		}
	}
}
