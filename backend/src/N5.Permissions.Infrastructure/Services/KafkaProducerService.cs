using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;

namespace N5.Permissions.Infrastructure.Services;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private const string TopicName = "permissions-operations";
    private bool _disposed = false;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaProducerService(IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "n5-permissions-api"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
        
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task PublishOperationAsync(OperationType operationType)
    {
        var message = new OperationMessage
        {
            Id = Guid.NewGuid(),
            NameOperation = operationType
        };

        var jsonMessage = JsonSerializer.Serialize(message, _jsonOptions);
        var kafkaMessage = new Message<Null, string>
        {
            Value = jsonMessage
        };

        try
        {
            var result = await _producer.ProduceAsync(TopicName, kafkaMessage);
            
            if (result.Status != PersistenceStatus.Persisted)
                throw new Exception($"Failed to publish message to Kafka. Status: {result.Status}");
        }
        catch (ProduceException<Null, string> ex)
        {
            throw new Exception($"Failed to publish message to Kafka: {ex.Message}", ex);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _producer?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

