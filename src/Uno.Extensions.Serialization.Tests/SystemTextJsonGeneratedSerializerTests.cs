﻿using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Serialization;

namespace Uno.Extensions.Serialization.Tests;

[TestClass]
public class SystemTextJsonGeneratedSerializerTests
{
	private const string SimpleText = "Hello World!";

	private SystemTextJsonGeneratedSerializer<SimpleClass> Serializer { get; set; }

	[TestInitialize]
	public void InitializeTests()
	{
		var services = new ServiceCollection().BuildServiceProvider();
		var reflectionSerializer = new SystemTextJsonStreamSerializer(services);
		Serializer = new SystemTextJsonGeneratedSerializer<SimpleClass>(reflectionSerializer, reflectionSerializer,SimpleClassContext.Default.SimpleClass);
	}

	[TestMethod]
	public void SimpleClassSerializationTest()
	{
		var entity = CreateSimpleClassInstance();
		TestEntitySerialization<SimpleClass>(entity as SimpleClass);
	}

	[TestMethod]
	public void SimpleRecordSerializationTest()
	{
		var entity = CreateSimpleRecordInstance();
		TestEntitySerialization<SimpleRecord>(entity as SimpleRecord);
	}

	[TestMethod]
	public void SimpleClassStringifyTest()
	{
		var entity = CreateSimpleClassInstance();
		TestEntityStringify<SimpleClass>(entity as SimpleClass);
	}

	[TestMethod]
	public void SimpleRecordStringifyTest()
	{
		var entity = CreateSimpleRecordInstance();
		TestEntityStringify<SimpleRecord>(entity as SimpleRecord);
	}

	private void TestEntitySerialization<T>(T entity)
		where T : ISimpleText
	{
		using (var ms = new MemoryStream())
		{
			Serializer.ToStream(ms, entity);
			ms.Flush();

			// Reset the stream so we can read
			ms.Seek(0, SeekOrigin.Begin);

			var clonedEntity = Serializer.FromStream<T>(ms);
			Assert.IsInstanceOfType(clonedEntity, typeof(T));
			Assert.AreNotSame(entity, clonedEntity);
			Assert.AreEqual(((T)entity).SimpleTextProperty, ((T)clonedEntity).SimpleTextProperty);

			// Reset the stream so we can write again
			ms.Seek(0, SeekOrigin.Begin);

			Serializer.ToStream<T>(ms, clonedEntity);
			ms.Flush();

			// Reset the stream so we can read
			ms.Seek(0, SeekOrigin.Begin);
			var anotherClone = Serializer.FromStream<T>(ms);

			Assert.IsInstanceOfType(anotherClone, typeof(T));
			Assert.AreNotSame(clonedEntity, anotherClone);
			Assert.AreEqual(((T)clonedEntity).SimpleTextProperty, ((T)anotherClone).SimpleTextProperty);
		}
	}


	private void TestEntityStringify<T>(T entity)
		where T : ISimpleText
	{
		var stringValue = Serializer.ToString(entity);
		var clonedEntity = Serializer.FromString<T>(stringValue);
		Assert.IsInstanceOfType(clonedEntity, typeof(T));
		Assert.AreNotSame(entity, clonedEntity);
		Assert.AreEqual(((T)entity).SimpleTextProperty, ((T)clonedEntity).SimpleTextProperty);
	}

	public static ISimpleText CreateSimpleClassInstance()
	{
		return new SimpleClass { SimpleTextProperty = SimpleText + "Class" };
	}

	public static ISimpleText CreateSimpleRecordInstance()
	{
		return new SimpleRecord(SimpleTextProperty: SimpleText + "Record");
	}
}

[JsonSerializable(typeof(SimpleClass))]
internal partial class SimpleClassContext : JsonSerializerContext
{
}

