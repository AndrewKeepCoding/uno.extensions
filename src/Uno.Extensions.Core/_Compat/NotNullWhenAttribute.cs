﻿using System;

#if NETSTANDARD2_0 || WINDOWS_UWP || NET461
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal class NotNullWhenAttribute : Attribute
{
	public bool ReturnValue { get; }

	public NotNullWhenAttribute(bool returnValue)
	{
		ReturnValue = returnValue;
	}
}

#endif
