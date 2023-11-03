using AutoFixture.NUnit3;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace MMQ.Test
{
	[TestFixture]
	public sealed class MemoryMappedQueueTest
	{
		[Test]
		public void TestCreateQueue1()
		{
			using var queue = MemoryMappedQueue.Create("Foobar");
			queue.Should().NotBeNull();

			new Action(() => MemoryMappedFile.CreateFromFile("/dev/shm/Foobar", System.IO.FileMode.CreateNew, null, ushort.MaxValue))
				.ShouldThrow<IOException>("because Create() is supposed to create an underlying memory mapped file");
		}

		[Test, AutoData]
		public void TestCreateProducer1(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var producer = queue.CreateProducer();
			producer.Should().NotBeNull();
		}

		[Test, AutoData]
		public void TestCreateConsumer1(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var consumer = queue.CreateConsumer();
			consumer.Should().NotBeNull();
		}

		[Test]
		[Description("Verifies that simple messages can be exchanged")]
		public void TestEnqueueDequeue1([Values(new byte[] {1},
												new byte[] {42, 99},
												new byte[] {200, 91, 12},
												new byte[] {0, 1, 44, 125, 231, 200, 100})] byte[] message)
		{
			using var queue = MemoryMappedQueue.Create("Test1");
			using var producer = MemoryMappedQueue.CreateProducer("Test1");
			using var consumer = queue.CreateConsumer();
			producer.Enqueue(message);
			var actualMessage = consumer.Dequeue();

			actualMessage.Should().NotBeNull();
			actualMessage.Should().NotBeSameAs(message);
			actualMessage.Should().Equal(message);
		}

		[Test, AutoData]
		public void TestEnqueueDequeueMany(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var producer = queue.CreateProducer();
			using var consumer = MemoryMappedQueue.CreateConsumer(queueName);
			const int messageLength = 512;
			const int numMessages = 1000;

			for(int i = 0; i < numMessages; ++i)
			{
				var message = Enumerable.Range(0, messageLength).Select(n => (byte) (n % 255)).ToArray();

				producer.Enqueue(message);
				var actualMessage = consumer.Dequeue();
				actualMessage.Should().Equal(message);
			}
		}

		[Test, AutoData]
		[Description("Verifies that dequeuing an empty queue results in a timeout")]
		public void TestDequeue1(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var consumer = queue.CreateConsumer();
			new Action(() => consumer.Dequeue(TimeSpan.Zero)).ShouldThrow<TimeoutException>();
			consumer.TryDequeue(out byte[] _, TimeSpan.Zero).Should().BeFalse();
		}

		[Test, AutoData]
		[Description("Verifies that after a dequeue operation times out, the queue can still be used")]
		public void TestDequeue2(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var producer = queue.CreateProducer();
			using var consumer = queue.CreateConsumer();
			consumer.TryDequeue(out byte[] message, TimeSpan.FromMilliseconds(1)).Should().BeFalse();

			producer.Enqueue(new byte[] {1, 2, 3, 4});
			consumer.TryDequeue(out message).Should().BeTrue();
			message.Should().Equal(new byte[] {1, 2, 3, 4});
		}

		[Test, AutoData]
		[Description("Verifies that using a consumer when when it already has been disposed of results in an appropriate exception")]
		public void TestDequeue3(string queueName)
		{
			Console.WriteLine("Queue name: {0}", queueName);
			using var queue = MemoryMappedQueue.Create(queueName);
			using var consumer = queue.CreateConsumer();
			consumer.Dispose();
			// ReSharper disable once AccessToDisposedClosure
			new Action(() => consumer.Dequeue())
				.ShouldThrow<ObjectDisposedException>("because the consumer has been disposed of");
			// ReSharper disable once AccessToDisposedClosure
			new Action(() => consumer.TryDequeue(out byte[] _))
				.ShouldThrow<ObjectDisposedException>("because the consumer has been disposed of");
		}
	}
}