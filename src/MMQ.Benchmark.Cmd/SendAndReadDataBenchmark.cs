using BenchmarkDotNet.Attributes;

namespace MMQ.Benchmark.Cmd
{
    public class SendAndReadDataBenchmark: IDisposable
    {
        private readonly IMemoryMappedQueue queue;
        private readonly IMemoryMappedQueueProducer producer;
        private readonly IMemoryMappedQueueConsumer consumer;
        
        public SendAndReadDataBenchmark()
        {
            queue = MemoryMappedQueue.Create(nameof(SendAndReadDataBenchmark));
            producer = queue.CreateProducer();
            consumer = queue.CreateConsumer();
        }

        [Benchmark]
        public void SendMessage()
        {
            producer.Enqueue(new byte[]{1,2,3,4,5,6,7,8});
        }

        [Benchmark]
        public void ReadMessage()
        {
            _ = consumer.Dequeue();
        }

        public void Dispose()
        {
            producer.Dispose();
            consumer.Dispose();
            queue.Dispose();
        }
    }
}
