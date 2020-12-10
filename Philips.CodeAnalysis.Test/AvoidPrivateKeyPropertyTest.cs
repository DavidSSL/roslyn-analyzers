﻿// © 2019 Koninklijke Philips N.V. See License.md in the project root for license information.
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Philips.CodeAnalysis.Common;
using Philips.CodeAnalysis.MaintainabilityAnalyzers;

namespace Philips.CodeAnalysis.Test
{
	[TestClass]
	public class AvoidPrivateKeyPropertyTest : DiagnosticVerifier
	{
		#region Non-Public Data Members

		private const string ClassString = @"
			using System;
			using System.Globalization;
			class Foo 
			{{
				public void Foo()
				{{
					{0}
				}}
			}}
			";


		#endregion
		#region Non-Public Properties/Methods

		protected override MetadataReference[] GetMetadataReferences()
		{
			string mockReference = typeof(X509Certificate2).Assembly.Location;
			MetadataReference reference = MetadataReference.CreateFromFile(mockReference);

			return base.GetMetadataReferences().Concat(new[] { reference }).ToArray();
		}


		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new AvoidPrivateKeyProperty();
		}

		private DiagnosticResultLocation GetBaseDiagnosticLocation(int rowOffset = 0, int columnOffset = 0)
		{
			return new DiagnosticResultLocation("Test.cs", 8 + rowOffset, 8 + columnOffset);
		}

		#endregion

		#region Test Methods
		[DataTestMethod]
		[DataRow(@"_ = new X509Certificate2().PrivateKey", 0, 2)]
		[DataRow(@"X509Certificate2 cert = new X509Certificate2();
			_ = cert.PrivateKey;", 1, 0)]
		public void AvoidPrivateKeyPropertyOnX509Certificate(string s, int row, int col)
		{

			string code = string.Format(ClassString, s);
			DiagnosticResult expected = new DiagnosticResult
			{
				Id = Helper.ToDiagnosticId(DiagnosticIds.AvoidPrivateKeyProperty),
				Message = new Regex(".+ "),
				Severity = DiagnosticSeverity.Error,
				Locations = new[]
				{
					GetBaseDiagnosticLocation(row,col)
				}
			};

			VerifyCSharpDiagnostic(code, expected);
		}
		#endregion


	}
}