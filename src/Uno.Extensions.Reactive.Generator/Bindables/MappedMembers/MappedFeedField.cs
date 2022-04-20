﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Uno.Extensions.Reactive.Generator;

// Note: This also applies for State
internal record MappedFeedField(IFieldSymbol _field, ITypeSymbol _valueType) : IMappedMember
{
	private readonly IFieldSymbol _field = _field;
	private readonly ITypeSymbol _valueType = _valueType;

	/// <inheritdoc />
	public string Name => _field.Name;

	/// <inheritdoc />
	public string GetDeclaration()
		=> $"{_field.GetAccessibilityAsCSharpCodeString()} {NS.Reactive}.IState<{_valueType}> {_field.Name};";

	/// <inheritdoc />
	public string? GetInitialization()
		=> $"{_field.Name} = {N.Ctor.Ctx}.GetOrCreateState({N.Ctor.Model}.{_field.Name} ?? throw new NullReferenceException(\"The feed field '{_field.Name}' is null. Public feeds fields must be initialized in the constructor.\"));";
}
