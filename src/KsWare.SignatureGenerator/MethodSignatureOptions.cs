using System.Collections.Generic;

namespace KsWare.SignatureGenerator {

	public class MethodSignatureOptions {

		private const bool X = true;
		private const bool o = false;
		private static Dictionary<SignatureMode, bool[]> matrix = new Dictionary<SignatureMode, bool[]> {
			/*                                             Access                               */
			/*                                             |  Modifiers                         */
			/*                                             |  |  ReturnType                     */
			/*                                             |  |  |  Name                        */
			{SignatureMode.Code                   , new[] {X, X, X, X}},
			{SignatureMode.Compare                , new[] {X, X, X, X}},
			{SignatureMode.CompareIgnoreReturnType, new[] {X, X, o, X}},
			{SignatureMode.InheriteDoc            , new[] {o, o, o, X}},
		};

		public bool Access { get; set; }

		public bool Modifiers { get; set; }

		public bool ReturnType { get; set; }

		public bool Name { get; set; }

		public ParameterSignatureOptions Parameter { get; set; }

		public static MethodSignatureOptions Create(SignatureMode signatureMode) {
			var m = matrix[signatureMode];
			return new MethodSignatureOptions {
				Access     = m[0],
				Modifiers  = m[1],
				ReturnType = m[2],
				Name       = m[3],
				Parameter  = ParameterSignatureOptions.Create(signatureMode)
			};
		}
	}

}