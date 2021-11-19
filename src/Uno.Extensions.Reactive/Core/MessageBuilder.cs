﻿using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Extensions.Reactive.Utils;

namespace Uno.Extensions.Reactive;

public readonly struct MessageBuilder<T> : IMessageEntry, IMessageBuilder, IMessageBuilder<T>
{
	private readonly MessageEntry<T> _previous;
	private readonly HashSet<MessageAxis> _changes;
	private readonly Dictionary<MessageAxis, MessageAxisValue> _values;

	internal MessageBuilder(MessageEntry<T> current)
	{
		_previous = current;
		_changes = new();
		_values = current.Values.ToDictionary();
	}

	Option<object> IMessageEntry.Data => CurrentData;
	Exception? IMessageEntry.Error => CurrentError;
	bool IMessageEntry.IsTransient => CurrentIsTransient;
	MessageAxisValue IMessageEntry.this[MessageAxis axis] => Get(axis);

	MessageAxisValue IMessageBuilder.this[MessageAxis axis]
	{
		get => Get(axis);
		set => Set(axis, value);
	}

	internal Option<T> CurrentData => this.GetData<T>();
	internal Exception? CurrentError => this.GetError();
	internal bool CurrentIsTransient => this.GetProgress();
	internal MessageAxisValue this[MessageAxis axis]
	{
		get => Get(axis);
		set => Set(axis, value);
	}

	internal MessageAxisValue Get(MessageAxis axis)
		=> _values.TryGetValue(axis, out var value) ? value : default;

	internal void Set(MessageAxis axis, MessageAxisValue value)
	{
		if (axis.AreEquals(this[axis], value))
		{
			return;
		}

		if (value.IsSet)
		{
			_values[axis] = value;
			_changes.Add(axis);
		}
		else if (_values.Remove(axis))
		{
			_changes.Add(axis);
		}
	}

	internal Message<T> Build()
		=> new(_previous, new MessageEntry<T>(_values), _changes);

	public static implicit operator Message<T>(MessageBuilder<T> builder)
		=> builder.Build();
}