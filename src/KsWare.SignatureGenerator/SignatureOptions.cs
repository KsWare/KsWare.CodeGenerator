// ***********************************************************************
// Assembly         : KsWare.SignatureGenerator
// Author           : SchreinerK
// Created          : 2018-03-12
//
// Last Modified By : SchreinerK
// Last Modified On : 2018-03-13
// ***********************************************************************
// <copyright file="SignatureHelper.cs" company="KsWare">
//     Copyright © 2018 KsWare. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************


namespace KsWare.SignatureGenerator {

	public class TypeSignatureOptions {

		public bool Access { get; set; }

		public bool Modifiers { get; set; }

		public bool Name { get; set; }

		public static TypeSignatureOptions Create(SignatureMode mode) {
			return new TypeSignatureOptions{Access = true, Modifiers = true, Name = true};
		}
	}

	public class MemberSignatureOptions {

		public bool Access { get; set; }

		public bool Modifiers { get; set; }

		public bool ReturnType { get; set; }

		public bool Name { get; set; }

		public static MemberSignatureOptions Create(SignatureMode mode) {
			switch (mode) {
				case SignatureMode.CompareIgnoreReturnType: return new MemberSignatureOptions {Access = true, Modifiers = true, ReturnType=false, Name = true};
				default: return new MemberSignatureOptions {Access = true, Modifiers = true, ReturnType = false, Name = true};
			}
		}
	}

	public class ParameterSignatureOptions {

		public bool Type { get; set; }

		public bool Name { get; set; }

		public static ParameterSignatureOptions Create(SignatureMode mode) {
			switch (mode) {
				case SignatureMode.CompareIgnoreReturnType: 
				case SignatureMode.Compare: return new ParameterSignatureOptions {Type = true, Name = false};
				default: return new ParameterSignatureOptions {Type = true, Name = true};
			}
		}
	}

	public class GenericParameterSignatureOptions {

		public bool Name { get; set; }

		public static ParameterSignatureOptions Create(SignatureMode mode) {
			switch (mode) {
				case SignatureMode.CompareIgnoreReturnType:
				case SignatureMode.Compare: return new ParameterSignatureOptions {Name = false};
				default: return new ParameterSignatureOptions {Name = true};
			}
		}
	}

	public class PropertySignatureOptions {

		public static PropertySignatureOptions Create(SignatureMode signatureMode) {
			return new PropertySignatureOptions{};
		}
	}

	public class FieldSignatureOptions {

		public static FieldSignatureOptions Create(SignatureMode signatureMode) {
			return new FieldSignatureOptions { };
		}
	}
	public class EventSignatureOptions {

		public static EventSignatureOptions Create(SignatureMode signatureMode) {
			return new EventSignatureOptions { };
		}
	}
}
