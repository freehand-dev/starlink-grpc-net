using Grpc.Net.Client;
using SpaceX.API.Device;
using System.Text.Json;

using (var channel = GrpcChannel.ForAddress("http://192.168.100.1:9200"))
{
    var client = new Device.DeviceClient(channel);

    var call = await client.HandleAsync(new Request() { GetStatus = new GetStatusRequest() });

    Console.WriteLine(
        JsonSerializer.Serialize(call.DishGetStatus, 
            new JsonSerializerOptions()
            {
                WriteIndented = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            }
        )
    );
}

