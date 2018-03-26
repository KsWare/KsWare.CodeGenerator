using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KsWare.SignatureGenerator.SignatureHelpers {

	public class TypeSignature : BaseSignature {

		public TypeSignature(SignatureHelper signatureHelper) : base(signatureHelper) { }

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
				sb.Append(SignatureHelper.Sig(t.GetGenericArguments()));
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
				sb.Append(SignatureHelper.Sig(t.GetGenericArguments()));
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
	}
}
