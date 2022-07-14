var connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("/network-hub")
    .withAutomaticReconnect()
    .build();

const NetworkServiceTypes = {
    Ping: 1,
    TraceRoute: 2
}
const NetworkServiceState = {
    Started: 1,
    Running: 2,
    Stopped: 3,
    StoppedByException: 4
}

var currentServiceType = NetworkServiceTypes.Ping;
connection.on("SendUpdate", function (result) {
    let message = document.createElement('p');
    console.log(result);
    switch (result.state) {
        case NetworkServiceState.Started:
            message.innerHTML = "Usługa została uruchomiona";
            $('#button-start').hide();
            $('#button-stop').show();
            break;
        case NetworkServiceState.Running:
            message.innerHTML = result.message;
            $('#button-start').hide();
            $('#button-stop').show();
            break;
        case NetworkServiceState.Stopped:
            message.innerHTML = "Usługa została zatrzymana";
            $('#button-start').show();
            $('#button-stop').hide();
            break;
        case NetworkServiceState.StoppedByException:
            message.innerHTML = "Usługa została zatrzymana przez nieobsłużony wyjątek";
            $('#button-start').show();
            $('#button-stop').hide();
            break;
    }
    $("#messages-wrapper").append(message);
    message.scrollIntoView();
});

connection.on("SendMessage", function (message) {
    alert(message);
});

connection.start().then(function () {
    console.log('Connected!');

}).catch(function (err) {
    alert("Błąd połączenia z serwerem");
    return console.error(err.toString());
});

$(function () {
    $(document).on('click', '#button-start', function () {
        let addressIp = $("#input-address-ip").val();
        connection.invoke("StartNetworkTask", currentServiceType, addressIp).catch(function (err) {
            return console.error(err.toString());
        });
    });

    $(document).on('click', '#button-stop', function () {
        connection.invoke("StopNetworkTask").catch(function (err) {
            return console.error(err.toString());
        });
    });

    $(document).on('click', '#button-clear', function () {
        $("#messages-wrapper").empty();
    });

    $(document).on('change', 'input[type=radio][name=serviceType]', function () {
        connection.invoke("StopNetworkTask").catch(function (err) {
            return console.error(err.toString());
        });
        currentServiceType = Number($(this).val());
    });
})
