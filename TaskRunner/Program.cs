using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client;
using MQTTnet;
using TaskRunner;
using TaskRunner.Task;
using TaskRunner.TaskCommand;
using TaskRunner.Trigger;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(options =>
{
    options.AddSimpleConsole(c =>
    {
        //c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
        //c.UseUtcTimestamp = true;
        c.SingleLine = true;
    });
});

builder.Services.AddTransient<MqttTaskCommand>();
builder.Services.AddTransient<PowershellTaskCommand>();
builder.Services.AddTransient<BashTaskCommand>();
builder.Services.AddTransient<ScheduleTriggerCommand>();

builder.Services.AddSingleton(provider => new TaskCommandTypeDictionary
{
    { "mqtt", typeof(MqttTaskCommand) },
    { "powershell", typeof(PowershellTaskCommand) },
    { "bash", typeof(BashTaskCommand) }
});

builder.Services.AddSingleton(provider => new TriggerCommandTypeDictionary
{
    { "schedule", typeof(ScheduleTriggerCommand) }
});

builder.Services.AddSingleton<ITriggerCommandFactory, TriggerCommandFactory>();
builder.Services.AddSingleton<ITaskCommandFactory, TaskCommandFactory>();
builder.Services.AddTransient<ITaskRunnerFactory, TaskRunnerFactory>();

var projectFolder = builder.Configuration["Project:Folder"];
if (projectFolder != null && Directory.Exists(projectFolder))
{
    builder.Services.AddHostedService<TaskRunnerService>(provider =>
    new TaskRunnerService(provider.GetRequiredService<ILogger<TaskRunnerService>>(), projectFolder, provider.GetRequiredService<ITaskRunnerFactory>()));
}

var host = builder.Build();
host.Run();
