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

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using KsWare.SignatureGenerator.SignatureHelpers;
using MA = System.Reflection.MethodAttributes;

namespace KsWare.SignatureGenerator {

	// ?? System.Reflection.Emit.SignatureHelper

	/// <summary>
	/// Class SignatureHelper.
	/// </summary>
	public partial class SignatureHelper {

		private FieldInfoSignature     _fieldInfoSignature;
		private EventInfoSignature     _eventInfoSignature;
		private MethodInfoSignature    _methodInfoSignature;
		private ParameterInfoSignature _parameterInfoSignature;
		private PropertyInfoSignature  _propertyInfoSignature;
		private TypeSignature          _typeSignature;

		/// <summary>
		/// The signature mode
		/// </summary>
		private readonly SignatureMode _signatureMode;

		/// <summary>
		/// If true ignores parameter name
		/// </summary>
		private bool IgnoreParameterName;
		/// <summary>
		/// If true ignores return type
		/// </summary>
		private bool IgnoreReturnType;

		/// <summary>
		/// For compare
		/// </summary>
		public static readonly SignatureHelper ForCompare = new SignatureHelper(SignatureMode.Compare);
		/// <summary>
		/// For compare ignore return type
		/// </summary>
		public static readonly SignatureHelper ForCompareIgnoreReturnType = new SignatureHelper(SignatureMode.CompareIgnoreReturnType);
		/// <summary>
		/// For code
		/// </summary>
		public static readonly SignatureHelper ForCode = new SignatureHelper(SignatureMode.Code);
		/// <summary>
		/// The inherite document
		/// </summary>
		public static readonly SignatureHelper InheriteDoc = new SignatureHelper(SignatureMode.InheriteDoc);

		/// <summary>
		/// Initializes a new instance of the <see cref="SignatureHelper"/> class.
		/// </summary>
		/// <param name="signatureMode">The signature mode.</param>
		private SignatureHelper(SignatureMode signatureMode) {
			_signatureMode = signatureMode;
			_fieldInfoSignature=new FieldInfoSignature(this);
			_eventInfoSignature=new EventInfoSignature(this);
			_methodInfoSignature=new MethodInfoSignature(this);
			_parameterInfoSignature=new ParameterInfoSignature(this);
			_propertyInfoSignature=new PropertyInfoSignature(this);
			_typeSignature=new TypeSignature(this);
		}

		public SignatureMode SignatureMode => _signatureMode;

		/// <summary>
		/// Creates signature for the specified event information.
		/// </summary>
		/// <param name="eventInfo">The event information.</param>
		/// <returns>System.String.</returns>
		public string Sig(EventInfo eventInfo) => _eventInfoSignature.Sig(eventInfo);

		/// <summary>
		/// Creates signature for the specified method information.
		/// </summary>
		/// <param name="methodInfo">The method information.</param>
		/// <returns>System.String.</returns>
		public string Sig(MethodInfo methodInfo) => _methodInfoSignature.Sig(methodInfo);

		/// <summary>
		/// Creates signature for the specified constructor information.
		/// </summary>
		/// <param name="constructorInfo">The constructor information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ConstructorInfo constructorInfo) => _methodInfoSignature.Sig(constructorInfo);

		/// <summary>
		/// Creates signature for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ParameterInfo parameterInfo) => _parameterInfoSignature.Sig(parameterInfo);

		/// <summary>
		/// Creates signature for the specified property information.
		/// </summary>
		/// <param name="propertyInfo">The property information.</param>
		/// <returns>System.String.</returns>
		public string Sig(PropertyInfo propertyInfo) => _propertyInfoSignature.Sig(propertyInfo);

		/// <summary>
		/// Creates signature for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>System.String.</returns>
		public string Sig(Type type) => _typeSignature.Sig(type);

		/// <summary>
		/// Creates signature for the specified field information.
		/// </summary>
		/// <param name="fieldInfo">The field information.</param>
		/// <returns>System.String.</returns>
		public string Sig(FieldInfo fieldInfo) => _fieldInfoSignature.Sig(fieldInfo);

		/// <summary>
		/// Creates signature for the specified attribute.
		/// </summary>
		/// <param name="attr">The attribute.</param>
		/// <returns>System.String.</returns>
		public string Sig(MethodAttributes attr) {
			var sb=new StringBuilder();

			sb.Append(SigAccess(attr));
			sb.Append(SigModifier(attr));

			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the access.
		/// </summary>
		/// <param name="attr">The attribute.</param>
		/// <returns>System.String.</returns>
		public string SigAccess(MethodAttributes attr) {
			if (_signatureMode == SignatureMode.InheriteDoc) return "";
			switch (attr & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Public: return "public ";
				case MethodAttributes.FamORAssem: return "protected internal ";
				case MethodAttributes.FamANDAssem: return "protected private ";
				case MethodAttributes.Assembly: return "internal ";
				case MethodAttributes.Family: return "protected ";
				default: return "private ";
			}
		}

		/// <summary>
		/// Creates signature for the modifier.
		/// </summary>
		/// <param name="attr">The attribute.</param>
		/// <returns>System.String.</returns>
		public string SigModifier(MethodAttributes attr) {
			/*
				ReuseSlot = 0,
			    Final = 32, // 0x00000020
				Virtual = 64, // 0x00000040
				HideBySig = 128, // 0x00000080

				VtableLayoutMask = 256, // 0x00000100
				NewSlot = VtableLayoutMask, // 0x00000100

				CheckAccessOnOverride = 512, // 0x00000200
				Abstract = 1024, // 0x00000400
				SpecialName = 2048, // 0x00000800
				PinvokeImpl = 8192, // 0x00002000
				UnmanagedExport = 8,
				RTSpecialName = 4096, // 0x00001000
				ReservedMask = 53248, // 0x0000D000
				HasSecurity = 16384, // 0x00004000
				RequireSecObject = 32768, // 0x00008000
			 */
			if (_signatureMode == SignatureMode.InheriteDoc) return "";
			var sb = new StringBuilder();
			const uint @virtual = (uint) MA.Virtual;
			const uint @newslot = (uint) MA.NewSlot;
			const MA mask = (MA) 0x0000FFF0;
			if ((attr & mask) == (MA.HideBySig | MA.Static | MA.PinvokeImpl)) sb.Append("static extern ");
			else if ((attr & MA.Static) >0) sb.Append("static ");
			if ((attr & MA.Abstract)  > 0U) sb.Append("abstract ");
			if ((attr & MA.Final)     > 0U) sb.Append("sealed ");
			if (((uint) attr & @virtual + @newslot) == @virtual + @newslot) sb.Append("virtual ");
			if (((uint) attr & @virtual + @newslot) == @virtual) sb.Append("override ");
			if ((attr & MA.UnmanagedExport) > 0U) sb.Append("extern "); // TODO ??
			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the specified attribute.
		/// </summary>
		/// <param name="attr">The attribute.</param>
		/// <returns>System.String.</returns>
		public string Sig(FieldAttributes attr) {
			var sb = new StringBuilder();
			sb.Append(SigAccess((MethodAttributes) attr));
			sb.Append(SigModifier(attr));
			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the modifier.
		/// </summary>
		/// <param name="attr">The attribute.</param>
		/// <returns>System.String.</returns>
		public string SigModifier(FieldAttributes attr) {
			/*
			Static = 16, // 0x00000010
			InitOnly = 32, // 0x00000020
			Literal = 64, // 0x00000040
			NotSerialized = 128, // 0x00000080
			SpecialName = 512, // 0x00000200
			PinvokeImpl = 8192, // 0x00002000
			ReservedMask = 38144, // 0x00009500
			RTSpecialName = 1024, // 0x00000400
			HasFieldMarshal = 4096, // 0x00001000
			HasDefault = 32768, // 0x00008000
			HasFieldRVA = 256, // 0x00000100
			*/

			var @static  = (uint) FieldAttributes.Static;
			var @literal = (uint) FieldAttributes.Literal;
			var sb       = new StringBuilder();
			if (((uint) attr & @static + @literal)       == @static) sb.Append("static ");
			if (((uint) attr & @static + @literal)       == @static + @literal) sb.Append("const ");
			if ((attr        & FieldAttributes.InitOnly) > 0U) sb.Append("readonly ");

			return sb.ToString();
		}

		

		/// <summary>
		/// Creates signature for the specified parameter infos.
		/// </summary>
		/// <param name="parameterInfos">The parameter infos.</param>
		/// <returns>System.String.</returns>
		public string Sig(ParameterInfo[] parameterInfos) {
			if (parameterInfos.Length == 0) return string.Empty;
			var sb = new StringBuilder();
			foreach (var pi in parameterInfos) sb.Append(", " + Sig(pi));
			return sb.ToString(2, sb.Length                   - 2);
		}

		/// <summary>
		/// Creates signature for the specified generic arguments.
		/// </summary>
		/// <param name="genericArguments">The generic arguments.</param>
		/// <returns>System.String.</returns>
		public string Sig(Type[] genericArguments) {
			if (genericArguments.Length == 0) return string.Empty;
			var sb = new StringBuilder();
			foreach (var ga in genericArguments) sb.Append(", " + Sig(ga));
			return sb.ToString(2, sb.Length                     - 2);
		}

		/// <summary>
		/// Creates signature for the specified member information.
		/// </summary>
		/// <param name="memberInfo">The member information.</param>
		/// <returns>System.String.</returns>
		public string Sig(MemberInfo memberInfo) {
			switch (memberInfo.MemberType) {
				case MemberTypes.Constructor: return Sig((ConstructorInfo) memberInfo);
				case MemberTypes.Event: return Sig((EventInfo) memberInfo);
				case MemberTypes.Field: return Sig((FieldInfo) memberInfo);
				case MemberTypes.Method: return Sig((MethodInfo)memberInfo);
				case MemberTypes.NestedType: return Sig((TypeInfo) memberInfo);
				case MemberTypes.Property: return Sig((PropertyInfo) memberInfo);
				default: return $"unknown {memberInfo.MemberType}";
			}
		}

		/// <summary>
		/// Return the method signature as a string.
		/// </summary>
		/// <param name="method">The Method</param>
		/// <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
		/// <returns>Method signature</returns>
		public static string Sig2(MethodInfo method, bool callable = false)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            if (callable == false)
            {
                if (method.IsPublic)
                    sigBuilder.Append("public ");
                else if (method.IsPrivate)
                    sigBuilder.Append("private ");
                else if (method.IsAssembly)
                    sigBuilder.Append("internal ");
                if (method.IsFamily)
                    sigBuilder.Append("protected ");
                if (method.IsStatic)
                    sigBuilder.Append("static ");
                sigBuilder.Append(Sig2(method.ReturnType));
                sigBuilder.Append(' ');
            }
            sigBuilder.Append(method.Name);

            // Add method generics
            if(method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach(var g in method.GetGenericArguments())
                {
                    if (firstParam)
                        firstParam = false;
                    else
                        sigBuilder.Append(", ");
                    sigBuilder.Append(Sig2(g));
                }
                sigBuilder.Append(">");
            }
            sigBuilder.Append("(");
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParam = true;
                            continue;
                        }
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                    secondParam = false;
                else
                    sigBuilder.Append(", ");
                if (param.ParameterType.IsByRef)
                    sigBuilder.Append("ref ");
                else if (param.IsOut)
                    sigBuilder.Append("out ");
                if (!callable)
                {
                    sigBuilder.Append(Sig2(param.ParameterType));
                    sigBuilder.Append(' ');
                }
                sigBuilder.Append(param.Name);
            }
            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }

		/// <summary>
		/// Get full type name with full namespace names
		/// </summary>
		/// <param name="type">Type. May be generic or nullable</param>
		/// <returns>Full type name, fully qualified namespaces</returns>
		public static string Sig2(Type type) {
			var nullableType = Nullable.GetUnderlyingType(type);
			if (nullableType != null) return nullableType.Name + "?";

			if (!(type.IsGenericType && type.Name.Contains("`")))
				switch (type.Name) {
					case "String":  return "string";
					case "Int32":   return "int";
					case "Decimal": return "decimal";
					case "Object":  return "object";
					case "Void":    return "void";
					default: {
						return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
					}
				}

			var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
			sb.Append('<');
			var first = true;
			foreach (var t in type.GetGenericArguments()) {
				if (!first) sb.Append(',');
				sb.Append(Sig2(t));
				first = false;
			}
			sb.Append('>');
			return sb.ToString();
		}

		/// <summary>
		/// The access prio
		/// </summary>
		private static readonly string[] accessPrio = { 
			 "public",
			 "protected internal",
			 "protected",
			 "internal",
			 "protected private", //??
			 "private",
		};

		/// <summary>
		/// Returns the access value with the heigher prio
		/// </summary>
		/// <param name="a">The 1st access value.</param>
		/// <param name="b">The 2nd access value.</param>
		/// <returns>The access value with the heigher prio.</returns>
		/// <remarks><para>The access values are one of <c>public</c>, <c>protected internal</c>, <c>protected</c>, <c>internal</c>, <c>protected private</c>, <c>private</c> </para></remarks>
		public string MaxAccess(string a, string b) {
			var ai = Array.IndexOf(accessPrio,a.Trim());
			var bi = Array.IndexOf(accessPrio, b.Trim());
			return accessPrio[Math.Min(ai, bi)];
		}

		/// <summary>
		/// Returns the access value with the lower prio
		/// </summary>
		/// <param name="a">The 1st access value.</param>
		/// <param name="b">The 2nd access value.</param>
		/// <returns>The access value with the heigher prio.</returns>
		/// <remarks><para>The access values are one of <c>public</c>, <c>protected internal</c>, <c>protected</c>, <c>internal</c>, <c>protected private</c>, <c>private</c> </para></remarks>
		private string MinAccess(string a, string b) {
			var ai = Array.IndexOf(accessPrio, a.Trim());
			var bi = Array.IndexOf(accessPrio, b.Trim());
			return accessPrio[Math.Max(ai, bi)];
		}
	}

}
