# TaskRunner

Similar to CI/CD task runners.  The intention was to make it flexible and use it for many other tasks.  Originally built to schedule running IOT Mqtt automation messages.  But could be useful for other tasks.

Place configuration YAML files in a dedicated folder which linked in the appsettings.json file.

Below is an example YAML file:

    variables:
        server: mqtt.com
        port: 1883

    trigger:
        name: schedule
        input:
            cron: "*/1 * * * *"

    tasks:
    - task: mqtt
        displayName: "Send MQTT message"
        inputs:
            server: $(server)
            port: $(port)
            publish: "{ 'message': 'hello' }"
            topic: "app/sensor"
    - task: powershell
        displayName: "Run PowerShell Command"
        inputs:
        command: |
            echo "Hello, Mars"
            echo "Hello, Earth"
    - task: powershell
        displayName: "Run PowerShell Command"
        inputs:
            command: dir
