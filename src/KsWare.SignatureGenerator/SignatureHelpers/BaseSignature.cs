using System;
using System.Reflection;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class BaseSignature {

		protected BaseSignature(SignatureHelper signatureHelper) { SignatureHelper = signatureHelper; }

		protected SignatureHelper SignatureHelper { get; }

		public SignatureMode SignatureMode => SignatureHelper.SignatureMode;
	}

}