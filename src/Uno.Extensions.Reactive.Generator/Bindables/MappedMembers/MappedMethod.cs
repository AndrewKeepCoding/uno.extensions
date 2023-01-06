﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Uno.Extensions.Generators;

namespace Uno.Extensions.Reactive.Generator;

internal record MappedMethod(IMethodSymbol _method) : IMappedMember
{
	private readonly IMethodSymbol _method = _method;

	/// <inheritdoc />
	public string Name => _method.Name;

	/// <inheritdoc />
	public string? GetBackingField()
		=> null;

	/// <inheritdoc />
	public string GetDeclaration()
	{
		var parametersDeclaration = _method
			.Parameters
			.Select(p => $"{p.Type.ToFullString()} {p.Name} {(p.IsOptional ? "= " + (p.ExplicitDefaultValue ?? "default") : "")}")
			.JoinBy(", ");

		var parametersUsage = _method
			.Parameters
			.Select(p => p.Name)
			.JoinBy(", ");

		return $@"
			{_method.GetAccessibilityAsCSharpCodeString()} {_method.ReturnType.ToFullString()} {_method.Name}({parametersDeclaration})
				=> {N.Model}.{_method.Name}({parametersUsage});";
	}

	/// <inheritdoc />
	public string? GetInitialization()
		=> null;
}
