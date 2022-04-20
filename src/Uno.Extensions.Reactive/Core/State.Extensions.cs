﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.Extensions.Reactive;

/// <summary>
/// Provides a set of static methods to create and manipulate <see cref="IState{T}"/>.
/// </summary>
partial class State
{
	/// <summary>
	/// Updates the value of a state
	/// </summary>
	/// <typeparam name="T">Type of the value of the state.</typeparam>
	/// <param name="state">The state to update.</param>
	/// <param name="updater">The update method to apply to the current value.</param>
	/// <param name="ct">A cancellation to cancel the async operation.</param>
	/// <returns>A ValueTask to track the async update.</returns>
	public static ValueTask UpdateValue<T>(this IState<T> state, Func<Option<T>, Option<T>> updater, CancellationToken ct)
		=> state.Update(m => m.With().Data(updater(m.Current.Data)), ct);

	///// <summary/>
	//public static ValueTask UpdateList<T>(this IListState<T> state, Func<Option<IImmutableList<T>>, Option<IImmutableList<T>>> updater, CancellationToken ct)
	//	=> state.Update(m => m.With().Data(updater(m.Current.Data)), ct);

	/// <summary/>
	public static ValueTask UpdateValue<T>(this IListState<T> state, Func<Option<IImmutableList<T>>, Option<IImmutableList<T>>> updater, CancellationToken ct)
		=> state.Update(m => m.With().Data(updater(m.Current.Data)), ct);

	/// <summary/>
	public static ValueTask UpdateValue<T>(this IListState<T> state, Func<Option<IImmutableList<T>>, IImmutableList<T>> updater, CancellationToken ct)
		=> state.Update(m => m.With().Data(updater(m.Current.Data)), ct);

	///// <summary/>
	//public static ValueTask UpdateValue<TCollection, TItem>(this ListState<TCollection, TItem> state, Func<Option<TCollection>, TCollection> updater, CancellationToken ct)
	//	where TCollection : IImmutableList<TItem>
	//	=> state.Update(m => m.With().Data(updater(m.Current.Data)), ct);

	/// <summary>
	/// Sets the value of a state
	/// </summary>
	/// <typeparam name="T">Type of the value of the state.</typeparam>
	/// <param name="state">The state to update.</param>
	/// <param name="value">The value to set.</param>
	/// <param name="ct">A cancellation to cancel the async operation.</param>
	/// <returns>A ValueTask to track the async update.</returns>
	public static ValueTask Set<T>(this IState<T> state, Option<T> value, CancellationToken ct)
		where T : struct
		=> state.Update(m => m.With().Data(value), ct);

	/// <summary>
	/// Sets the value of a state
	/// </summary>
	/// <param name="state">The state to update.</param>
	/// <param name="value">The value to set.</param>
	/// <param name="ct">A cancellation to cancel the async operation.</param>
	/// <returns>A ValueTask to track the async update.</returns>
	public static ValueTask Set(this IState<string> state, Option<string> value, CancellationToken ct)
		=> state.Update(m => m.With().Data(value), ct);
}


//public class ChangeRecorderList<in T> : IImmutableList<T>
//{
	
//}

///// <summary />
//public abstract class FeedList<T> : IImmutableList<T>
//{

//}
