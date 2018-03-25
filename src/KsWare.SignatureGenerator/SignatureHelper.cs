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
using MA = System.Reflection.MethodAttributes;

namespace KsWare.SignatureGenerator {

	// ?? System.Reflection.Emit.SignatureHelper

	/// <summary>
	/// Class SignatureHelper.
	/// </summary>
	public class SignatureHelper {

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
		private SignatureHelper(SignatureMode signatureMode) { _signatureMode = signatureMode; }

		/// <summary>
		/// Creates signature for the specified method information.
		/// </summary>
		/// <param name="methodInfo">The method information.</param>
		/// <returns>System.String.</returns>
		public string Sig(MethodInfo methodInfo) {
			var sb = new StringBuilder();

			sb.Append(Sig(methodInfo.Attributes));

			if (_signatureMode == SignatureMode.CompareIgnoreReturnType || _signatureMode==SignatureMode.InheriteDoc) { /*skip*/ }
			else sb.Append(Sig(methodInfo.ReturnType) + " ");
			sb.Append(methodInfo.Name);
			sb.Append("(");
			sb.Append(Sig(methodInfo.GetParameters()));
			sb.Append(")");

			if (sb.ToString() == "protected override void Finalize()") return $"~{methodInfo.DeclaringType.Name}()"; // Desctructor
			return sb.ToString();
		}

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
		/// Creates signature for the specified constructor information.
		/// </summary>
		/// <param name="constructorInfo">The constructor information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ConstructorInfo constructorInfo) {
			var sb = new StringBuilder();

			if(constructorInfo.IsStatic)
				sb.Append("static ");
			else 
				sb.Append($"{Sig(constructorInfo.Attributes)}");
			

			sb.Append(constructorInfo.Name);
			sb.Append("(");
			sb.Append(Sig(constructorInfo.GetParameters()));
			sb.Append(")");
			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the specified event information.
		/// </summary>
		/// <param name="eventInfo">The event information.</param>
		/// <returns>System.String.</returns>
		public string Sig(EventInfo eventInfo) {
			var sb = new StringBuilder();
			var mi = eventInfo.AddMethod; // TODO

			sb.Append($"{Sig(mi.Attributes)}");
			sb.Append("event ");
			sb.Append($"{eventInfo.EventHandlerType} ");
			sb.Append($"{eventInfo.Name}");

			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the specified field information.
		/// </summary>
		/// <param name="fieldInfo">The field information.</param>
		/// <returns>System.String.</returns>
		public string Sig(FieldInfo fieldInfo) {
			var sb = new StringBuilder();

			sb.Append($"{Sig(fieldInfo.Attributes)}");
			sb.Append(Sig(fieldInfo.FieldType));
			sb.Append(" ");
			sb.Append(fieldInfo.Name);

			return sb.ToString();
//			return $"field {fieldInfo} // not implemented";
		}

		/// <summary>
		/// Creates signature for the specified property information.
		/// </summary>
		/// <param name="propertyInfo">The property information.</param>
		/// <returns>System.String.</returns>
		public string Sig(PropertyInfo propertyInfo) {
			var sb=new StringBuilder();
			var getter = propertyInfo.GetMethod;
			var setter = propertyInfo.SetMethod;

			string access = "";
			if (getter != null && setter != null) {
				var getterAccess = SigAccess(getter.Attributes);
				var setterAccess = SigAccess(setter.Attributes);
				access = MaxAccess(getterAccess, setterAccess) + " ";
			}
			else if(getter!=null){
				access = SigAccess(getter.Attributes);
			}
			else if (setter != null) {
				access = SigAccess(setter.Attributes);
			}

			var mi = getter ?? setter;

			sb.Append(access);
			sb.Append(SigModifier(mi.Attributes));

			sb.Append(Sig(propertyInfo.PropertyType));
			sb.Append(" ");
			if (propertyInfo.Name == "Item" && propertyInfo.GetMethod.GetParameters().Length>0) {
				sb.Append("this[");
				sb.Append(Sig(propertyInfo.GetMethod.GetParameters()));
				sb.Append("]");
			}
			else {
				sb.Append(propertyInfo.Name);
			}
			
			sb.Append(" { ");

			if (propertyInfo.CanRead) {
				var getterAccess = SigAccess(getter.Attributes);
				if (getterAccess != access) sb.Append(getterAccess);
				sb.Append("get; ");
			}
			if (propertyInfo.CanWrite) {
				var setterAccess = SigAccess(setter.Attributes);
				if (setterAccess != access) sb.Append(setterAccess);
				sb.Append("set; ");
			}
			sb.Append("}");

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
		/// Creates signature for the specified parameter information.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <returns>System.String.</returns>
		public string Sig(ParameterInfo parameterInfo) {
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

			switch (_signatureMode) {
				case SignatureMode.Compare:
				case SignatureMode.CompareIgnoreReturnType:
				case SignatureMode.InheriteDoc:
					var sig = Sig(parameterInfo.ParameterType);
					if (parameterInfo.IsOut) sig = "out " + sig.Substring(4);
					sb.Append(sig);
					break;
				case SignatureMode.Code:
					sb.Append(Sig(parameterInfo.ParameterType));
					sb.Append(" " + parameterInfo.Name);
					break;
			}
			
			return sb.ToString();
		}

		/// <summary>
		/// Creates signature for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>System.String.</returns>
		public string Sig(Type type) {
			var t = type; // store the unchanged type
			var suffix = "";
			var prefix = "";
			start:
			var fn = t.FullName; // possible null on generic types. "T", "T[]"
			var n = t.Name;
			if (n.EndsWith("]")) { // HasElementType==true

				// []   [,]
				// C#: string[][,] == Reflection Name = "String[,][]"
				var match=Regex.Match(n, @"(\[,*\])$",RegexOptions.Compiled);
				suffix = suffix + match.Value;
				t = t.GetElementType();
				goto start;
			}
			if (n.EndsWith("&")) { // HasElementType==true, IsByRef==true
				// T& => ref T, out T
				// the "out" is only known by ParameterInfo.IsOut
				prefix +="ref ";
				t = t.GetElementType();
				goto start;
			}

			if (t.IsGenericParameter) {
				// "T"
				fn = n;
			} 
			else if (t.IsConstructedGenericType) {
				// "KsWare.SignatureGenerator.Tests.SignatureHelperTests+IGenericInterface1`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"
				var sb   = new StringBuilder();
				var gt   = t.GetGenericTypeDefinition();
				var gtfn = gt.FullName.Substring(0, gt.FullName.IndexOf("`"));
				sb.Append(gtfn);
				sb.Append("<");
				sb.Append(Sig(t.GetGenericArguments()));
				sb.Append(">");
				fn = sb.ToString();
			}
			else if (t.IsGenericTypeDefinition) {
				// "KsWare.SignatureGenerator.Tests.SignatureHelperTests+IGenericInterface1`1"
				var sb   = new StringBuilder();
				var gt   = t.GetGenericTypeDefinition();
				var gtfn = gt.FullName.Substring(0, gt.FullName.IndexOf("`"));
				sb.Append(gtfn);
				sb.Append("<");
				sb.Append(Sig(t.GetGenericArguments()));
				sb.Append(">");
				fn = sb.ToString();
			}

			if(fn==null) Debugger.Break();
			if(fn.Contains("`")) Debugger.Break();

			fn = fn.Replace("+", ".");

			switch (fn) {
				case "System.Void":    fn = "void";   break;
				case "System.UInt16":  fn = "ushort"; break;
				case "System.UInt32":  fn = "uint";	  break;
				case "System.UInt64":  fn = "ulong";  break;
				case "System.Int16":   fn = "short";  break;
				case "System.Int32":   fn = "int";	  break;
				case "System.Int64":   fn = "long";	  break;
				case "System.Char":    fn = "char";	  break;
				case "System.String":  fn = "string"; break;
				case "System.Boolean": fn = "bool";	  break;
				case "System.Byte":    fn = "byte";	  break;
				case "System.SByte":   fn = "sbyte";  break;
				case "System.Double":  fn = "double"; break;
				case "System.Single":  fn = "float";  break;
				case "System.Decimal": fn = "decimal";break;
			}
//			if (fn.StartsWith("System.")) return fn.Substring(7);
			return prefix + fn + suffix;
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
		private string MaxAccess(string a, string b) {
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
