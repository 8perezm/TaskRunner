using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using TaskRunner.Model;

namespace TaskRunner.TaskCommand;

public class MqttTaskCommand : ITaskCommand
{
    private IMqttClient? _mqttClient;
    private MqttClientOptions? _options;
    private readonly ILogger<MqttTaskCommand> _logger;

    public MqttTaskCommand(ILogger<MqttTaskCommand> logger)
    {
        _logger = logger;
    }

    public void Execute(TaskItem task)
    {
        PrepareMqttOptions(task);

        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        PublishMessage(task);
    }

    private async void PublishMessage(TaskItem task)
    {
        if (_options != null && 
            task.Inputs.TryGetValue("publish", out var publish) &&
            task.Inputs.TryGetValue("topic", out var topic))
        {
            _logger.LogInformation($"Publishing MQTT message: {topic} {publish}");

            var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(publish)
            .Build();

            if(_mqttClient != null)
            {
                await _mqttClient.ConnectAsync(_options, CancellationToken.None);
                await _mqttClient.PublishAsync(message);
                await _mqttClient.DisconnectAsync();
            }
        }
    }

    private void PrepareMqttOptions(TaskItem task)
    {
        if (task.Inputs.TryGetValue("server", out var server))
        {
            // Get the username, password, clientId and port from the task inputs
            task.Inputs.TryGetValue("username", out var username);
            task.Inputs.TryGetValue("password", out var password);
            task.Inputs.TryGetValue("clientId", out var clientId);
            task.Inputs.TryGetValue("port", out var port);

            // Default port
            var mqttPort = 1883;

            // Parse port if provided
            if (port != null)
            {
                mqttPort = Int32.Parse(port);
            }

            // Create the options
            var mqttClientOptionsBuilder = new MqttClientOptionsBuilder();
            mqttClientOptionsBuilder.WithTcpServer(server, mqttPort);

            if (username != null && password != null)
                mqttClientOptionsBuilder.WithCredentials(username, password);

            if (clientId != null)
                mqttClientOptionsBuilder.WithClientId(clientId);

            // Build the options and assign to the field
            _options = mqttClientOptionsBuilder.Build();
        }
    }

}