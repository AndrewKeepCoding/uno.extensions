﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Uno.Extensions.Reactive.Generator.Compat;

internal class CompatibilityTypesGenerationTool : ICodeGenTool
{
	private readonly CompatibilityTypesGenerationContext _context;

	/// <inheritdoc />
	public string Version => "1";

	public CompatibilityTypesGenerationTool(CompatibilityTypesGenerationContext context)
	{
		_context = context;
	}

	public IEnumerable<(string fileName, string code)> Generate()
	{
		if (_context.NotNullIfNotNullAttribute is null)
		{
			yield return (nameof(_context.NotNullIfNotNullAttribute), GetNotNullIfNotNullAttribute());
		}
		if (_context.NotNullWhenAttribute is null)
		{
			yield return (nameof(_context.NotNullWhenAttribute), GetNotNullWhenAttribute());
		}
		if (_context.IsExternalInit is null)
		{
			yield return (nameof(_context.IsExternalInit), GetIsExternalInit());
		}
		if (_context.ModuleInitializerAttribute is null)
		{
			yield return (nameof(_context.ModuleInitializerAttribute), GetModuleInitializerAttribute());
		}
	}

	private string GetModuleInitializerAttribute()
		=> $@"{this.GetFileHeader(3)}

			using global::System;

			namespace System.Runtime.CompilerServices
			{{
				/// <summary>
				/// Used to indicate to the compiler that a method should be called in its containing module's initializer.
				/// </summary>
				{this.GetCodeGenAttribute()}
				[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false)]
				internal sealed class ModuleInitializerAttribute : global::System.Attribute
				{{
				}}
			}}".Align(0);

	private string GetIsExternalInit()
		=> $@"{this.GetFileHeader(3)}

			using global::System;

			namespace System.Runtime.CompilerServices
			{{
				/// <summary>
				/// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
				/// </summary>
				{this.GetCodeGenAttribute()}
				internal static class IsExternalInit
				{{
				}}
			}}
			".Align(0);

	private string GetNotNullIfNotNullAttribute()
		=> $@"{this.GetFileHeader(3)}

			using global::System;

			namespace System.Diagnostics.CodeAnalysis
			{{
				/// <summary>
				/// Specifies that the output will be non-null if the named parameter is non-null.
				/// </summary>
				{this.GetCodeGenAttribute()}
				[global::System.AttributeUsage(global::System.AttributeTargets.Parameter | global::System.AttributeTargets.Property | global::System.AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
				internal class NotNullIfNotNullAttribute : global::System.Attribute
				{{
					/// <summary>
					/// Gets the associated parameter name.
					/// </summary>
					public string ParameterName {{ get; }}

					/// <summary>
					/// Initializes the attribute with the associated parameter name.
					/// </summary>
					/// <param name=""parameterName"">The associated parameter name. The output will be non-null if the argument to the parameter specified is non-null.</param>
					public NotNullIfNotNullAttribute(string parameterName)
					{{
						ParameterName = parameterName;
					}}
				}}
			}}
			".Align(0);

	public string GetNotNullWhenAttribute()
		=> $@"{this.GetFileHeader(3)}

			using global::System;

			namespace System.Diagnostics.CodeAnalysis
			{{
				/// <summary>
				/// Specifies that when a method returns <see cref=""ReturnValue""/>, the parameter will not be null even if the corresponding type allows it.
				/// </summary>
				[global::System.AttributeUsage(global::System.AttributeTargets.Parameter, Inherited = false)]
				{this.GetCodeGenAttribute()}
				internal class NotNullWhenAttribute : global::System.Attribute
				{{
					/// <summary>
					/// Gets the return value condition.
					/// </summary>
					public bool ReturnValue {{ get; }}

					/// <summary>
					/// The return value condition. If the method returns this value, the associated parameter will not be null.
					/// </summary>
					/// <param name=""returnValue""></param>
					public NotNullWhenAttribute(bool returnValue)
					{{
						ReturnValue = returnValue;
					}}
				}}
			}}
			".Align(0);

	private (string name, string code) GetTemplate(string fileName)
	{
		var assembly = this.GetType().Assembly;
		using var resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Compat.Templates.{fileName}.cs");
		if (resource is null)
		{
			throw new InvalidOperationException($"Failed to load template '{fileName}'");
		}
		using var resourceReader = new StreamReader(resource);

		var code = this.GetFileHeader() + Environment.NewLine + resourceReader.ReadToEnd();

		return (fileName, code);
	}
}
