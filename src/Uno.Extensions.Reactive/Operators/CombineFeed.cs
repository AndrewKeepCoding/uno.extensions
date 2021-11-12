﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions.Reactive.Utils;

namespace Uno.Extensions.Reactive;


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 2 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
internal sealed class CombineFeed<T1, T2> : IFeed<(T1, T2)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2)
	{
		_feed1 = feed1;
		_feed2 = feed2;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2)>(2, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2)>(),
				OptionType.None => Option.None<(T1, T2)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2)> Combine<T1, T2>(IFeed<T1> feed1, IFeed<T2> feed2)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2), (_, feeds) => new CombineFeed<T1, T2>(feeds.feed1, feeds.feed2));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 3 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
internal sealed class CombineFeed<T1, T2, T3> : IFeed<(T1, T2, T3)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3)>(3, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3)>(),
				OptionType.None => Option.None<(T1, T2, T3)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3)> Combine<T1, T2, T3>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3), (_, feeds) => new CombineFeed<T1, T2, T3>(feeds.feed1, feeds.feed2, feeds.feed3));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 4 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4> : IFeed<(T1, T2, T3, T4)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4)>(4, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4), (_, feeds) => new CombineFeed<T1, T2, T3, T4>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 5 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5> : IFeed<(T1, T2, T3, T4, T5)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5)>(5, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5)> Combine<T1, T2, T3, T4, T5>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 6 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6> : IFeed<(T1, T2, T3, T4, T5, T6)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6)>(6, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6)> Combine<T1, T2, T3, T4, T5, T6>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 7 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7> : IFeed<(T1, T2, T3, T4, T5, T6, T7)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7)>(7, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7)> Combine<T1, T2, T3, T4, T5, T6, T7>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 8 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
/// <typeparam name="T8">Type of the feed #8.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8> : IFeed<(T1, T2, T3, T4, T5, T6, T7, T8)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;
	private readonly IFeed<T8> _feed8;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
		_feed8 = feed8;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7, T8)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined(), Option<T8>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7, T8)>(8, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);
		//_feed8
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(7, msg, ref dataSet.Item8), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7)),
			_feed8.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(7, msg, ref dataSet.Item8))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7, T8)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);
			type = Math.Min(type, (short)dataSet.Item8.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7, T8)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7, T8)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!, 
						dataSet.Item8.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7, T8)> Combine<T1, T2, T3, T4, T5, T6, T7, T8>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7, feed8), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7, feeds.feed8));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 9 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
/// <typeparam name="T8">Type of the feed #8.</typeparam>
/// <typeparam name="T9">Type of the feed #9.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;
	private readonly IFeed<T8> _feed8;
	private readonly IFeed<T9> _feed9;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
		_feed8 = feed8;
		_feed9 = feed9;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined(), Option<T8>.Undefined(), Option<T9>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(9, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);
		//_feed8
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(7, msg, ref dataSet.Item8), ct);
		//_feed9
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(8, msg, ref dataSet.Item9), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7)),
			_feed8.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(7, msg, ref dataSet.Item8)),
			_feed9.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(8, msg, ref dataSet.Item9))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);
			type = Math.Min(type, (short)dataSet.Item8.Type);
			type = Math.Min(type, (short)dataSet.Item9.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!, 
						dataSet.Item8.SomeOrDefault()!, 
						dataSet.Item9.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7, feed8, feed9), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7, feeds.feed8, feeds.feed9));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 10 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
/// <typeparam name="T8">Type of the feed #8.</typeparam>
/// <typeparam name="T9">Type of the feed #9.</typeparam>
/// <typeparam name="T10">Type of the feed #10.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;
	private readonly IFeed<T8> _feed8;
	private readonly IFeed<T9> _feed9;
	private readonly IFeed<T10> _feed10;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
		_feed8 = feed8;
		_feed9 = feed9;
		_feed10 = feed10;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined(), Option<T8>.Undefined(), Option<T9>.Undefined(), Option<T10>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>(10, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);
		//_feed8
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(7, msg, ref dataSet.Item8), ct);
		//_feed9
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(8, msg, ref dataSet.Item9), ct);
		//_feed10
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(9, msg, ref dataSet.Item10), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7)),
			_feed8.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(7, msg, ref dataSet.Item8)),
			_feed9.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(8, msg, ref dataSet.Item9)),
			_feed10.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(9, msg, ref dataSet.Item10))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);
			type = Math.Min(type, (short)dataSet.Item8.Type);
			type = Math.Min(type, (short)dataSet.Item9.Type);
			type = Math.Min(type, (short)dataSet.Item10.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!, 
						dataSet.Item8.SomeOrDefault()!, 
						dataSet.Item9.SomeOrDefault()!, 
						dataSet.Item10.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7, feed8, feed9, feed10), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7, feeds.feed8, feeds.feed9, feeds.feed10));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 11 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
/// <typeparam name="T8">Type of the feed #8.</typeparam>
/// <typeparam name="T9">Type of the feed #9.</typeparam>
/// <typeparam name="T10">Type of the feed #10.</typeparam>
/// <typeparam name="T11">Type of the feed #11.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;
	private readonly IFeed<T8> _feed8;
	private readonly IFeed<T9> _feed9;
	private readonly IFeed<T10> _feed10;
	private readonly IFeed<T11> _feed11;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10, IFeed<T11> feed11)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
		_feed8 = feed8;
		_feed9 = feed9;
		_feed10 = feed10;
		_feed11 = feed11;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined(), Option<T8>.Undefined(), Option<T9>.Undefined(), Option<T10>.Undefined(), Option<T11>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>(11, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);
		//_feed8
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(7, msg, ref dataSet.Item8), ct);
		//_feed9
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(8, msg, ref dataSet.Item9), ct);
		//_feed10
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(9, msg, ref dataSet.Item10), ct);
		//_feed11
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(10, msg, ref dataSet.Item11), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7)),
			_feed8.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(7, msg, ref dataSet.Item8)),
			_feed9.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(8, msg, ref dataSet.Item9)),
			_feed10.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(9, msg, ref dataSet.Item10)),
			_feed11.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(10, msg, ref dataSet.Item11))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);
			type = Math.Min(type, (short)dataSet.Item8.Type);
			type = Math.Min(type, (short)dataSet.Item9.Type);
			type = Math.Min(type, (short)dataSet.Item10.Type);
			type = Math.Min(type, (short)dataSet.Item11.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!, 
						dataSet.Item8.SomeOrDefault()!, 
						dataSet.Item9.SomeOrDefault()!, 
						dataSet.Item10.SomeOrDefault()!, 
						dataSet.Item11.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10, IFeed<T11> feed11)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7, feed8, feed9, feed10, feed11), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7, feeds.feed8, feeds.feed9, feeds.feed10, feeds.feed11));
}


/// <summary>
/// An <see cref="IFeed{T}"/> that combines data of 12 parents feeds.
/// </summary>
/// <typeparam name="T1">Type of the feed #1.</typeparam>
/// <typeparam name="T2">Type of the feed #2.</typeparam>
/// <typeparam name="T3">Type of the feed #3.</typeparam>
/// <typeparam name="T4">Type of the feed #4.</typeparam>
/// <typeparam name="T5">Type of the feed #5.</typeparam>
/// <typeparam name="T6">Type of the feed #6.</typeparam>
/// <typeparam name="T7">Type of the feed #7.</typeparam>
/// <typeparam name="T8">Type of the feed #8.</typeparam>
/// <typeparam name="T9">Type of the feed #9.</typeparam>
/// <typeparam name="T10">Type of the feed #10.</typeparam>
/// <typeparam name="T11">Type of the feed #11.</typeparam>
/// <typeparam name="T12">Type of the feed #12.</typeparam>
internal sealed class CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>
{
	private readonly IFeed<T1> _feed1;
	private readonly IFeed<T2> _feed2;
	private readonly IFeed<T3> _feed3;
	private readonly IFeed<T4> _feed4;
	private readonly IFeed<T5> _feed5;
	private readonly IFeed<T6> _feed6;
	private readonly IFeed<T7> _feed7;
	private readonly IFeed<T8> _feed8;
	private readonly IFeed<T9> _feed9;
	private readonly IFeed<T10> _feed10;
	private readonly IFeed<T11> _feed11;
	private readonly IFeed<T12> _feed12;

	public CombineFeed(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10, IFeed<T11> feed11, IFeed<T12> feed12)
	{
		_feed1 = feed1;
		_feed2 = feed2;
		_feed3 = feed3;
		_feed4 = feed4;
		_feed5 = feed5;
		_feed6 = feed6;
		_feed7 = feed7;
		_feed8 = feed8;
		_feed9 = feed9;
		_feed10 = feed10;
		_feed11 = feed11;
		_feed12 = feed12;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<Message<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> GetSource(SourceContext context, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var dataSet = (Option<T1>.Undefined(), Option<T2>.Undefined(), Option<T3>.Undefined(), Option<T4>.Undefined(), Option<T5>.Undefined(), Option<T6>.Undefined(), Option<T7>.Undefined(), Option<T8>.Undefined(), Option<T9>.Undefined(), Option<T10>.Undefined(), Option<T11>.Undefined(), Option<T12>.Undefined());
		var helper = new CombineFeedHelper<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>(12, GetData);

		//_feed1
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(0, msg, ref dataSet.Item1), ct);
		//_feed2
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(1, msg, ref dataSet.Item2), ct);
		//_feed3
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(2, msg, ref dataSet.Item3), ct);
		//_feed4
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(3, msg, ref dataSet.Item4), ct);
		//_feed5
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(4, msg, ref dataSet.Item5), ct);
		//_feed6
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(5, msg, ref dataSet.Item6), ct);
		//_feed7
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(6, msg, ref dataSet.Item7), ct);
		//_feed8
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(7, msg, ref dataSet.Item8), ct);
		//_feed9
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(8, msg, ref dataSet.Item9), ct);
		//_feed10
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(9, msg, ref dataSet.Item10), ct);
		//_feed11
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(10, msg, ref dataSet.Item11), ct);
		//_feed12
		//	.GetSource(context, ct)
		//	.ForEachAsync(msg => helper.ApplyUpdate(11, msg, ref dataSet.Item12), ct);

		var messages = AsyncEnumerableExtensions.Merge(
			_feed1.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(0, msg, ref dataSet.Item1)),
			_feed2.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(1, msg, ref dataSet.Item2)),
			_feed3.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(2, msg, ref dataSet.Item3)),
			_feed4.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(3, msg, ref dataSet.Item4)),
			_feed5.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(4, msg, ref dataSet.Item5)),
			_feed6.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(5, msg, ref dataSet.Item6)),
			_feed7.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(6, msg, ref dataSet.Item7)),
			_feed8.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(7, msg, ref dataSet.Item8)),
			_feed9.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(8, msg, ref dataSet.Item9)),
			_feed10.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(9, msg, ref dataSet.Item10)),
			_feed11.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(10, msg, ref dataSet.Item11)),
			_feed12.GetSource(context, ct).Select(msg => () => helper.ApplyUpdate(11, msg, ref dataSet.Item12))
		);

		await foreach (var messageProvider in messages.WithCancellation(ct).ConfigureAwait(false))
		{
			if (messageProvider() is { } message)
			{
				yield return message;
			}
		}

		Option<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> GetData()
		{
			var type = (short)dataSet.Item1.Type;
			type = Math.Min(type, (short)dataSet.Item2.Type);
			type = Math.Min(type, (short)dataSet.Item3.Type);
			type = Math.Min(type, (short)dataSet.Item4.Type);
			type = Math.Min(type, (short)dataSet.Item5.Type);
			type = Math.Min(type, (short)dataSet.Item6.Type);
			type = Math.Min(type, (short)dataSet.Item7.Type);
			type = Math.Min(type, (short)dataSet.Item8.Type);
			type = Math.Min(type, (short)dataSet.Item9.Type);
			type = Math.Min(type, (short)dataSet.Item10.Type);
			type = Math.Min(type, (short)dataSet.Item11.Type);
			type = Math.Min(type, (short)dataSet.Item12.Type);

			return (OptionType)type switch
			{
				OptionType.Undefined => Option.Undefined<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>(),
				OptionType.None => Option.None<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>(),
				OptionType.Some => Option.Some(
					(
						dataSet.Item1.SomeOrDefault()!, 
						dataSet.Item2.SomeOrDefault()!, 
						dataSet.Item3.SomeOrDefault()!, 
						dataSet.Item4.SomeOrDefault()!, 
						dataSet.Item5.SomeOrDefault()!, 
						dataSet.Item6.SomeOrDefault()!, 
						dataSet.Item7.SomeOrDefault()!, 
						dataSet.Item8.SomeOrDefault()!, 
						dataSet.Item9.SomeOrDefault()!, 
						dataSet.Item10.SomeOrDefault()!, 
						dataSet.Item11.SomeOrDefault()!, 
						dataSet.Item12.SomeOrDefault()!
					)),
				_ => throw new NotSupportedException($"Unsupported option type '{(OptionType)type}'."),
			};
		}

		//return helper;
	}
}

partial class Feed
{
	public static IFeed<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(IFeed<T1> feed1, IFeed<T2> feed2, IFeed<T3> feed3, IFeed<T4> feed4, IFeed<T5> feed5, IFeed<T6> feed6, IFeed<T7> feed7, IFeed<T8> feed8, IFeed<T9> feed9, IFeed<T10> feed10, IFeed<T11> feed11, IFeed<T12> feed12)
		=> AttachedProperty.GetOrCreate(feed1, (feed1, feed2, feed3, feed4, feed5, feed6, feed7, feed8, feed9, feed10, feed11, feed12), (_, feeds) => new CombineFeed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(feeds.feed1, feeds.feed2, feeds.feed3, feeds.feed4, feeds.feed5, feeds.feed6, feeds.feed7, feeds.feed8, feeds.feed9, feeds.feed10, feeds.feed11, feeds.feed12));
}

