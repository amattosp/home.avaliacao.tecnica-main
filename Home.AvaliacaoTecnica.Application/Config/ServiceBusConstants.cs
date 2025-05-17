namespace Home.AvaliacaoTecnica.Application.Config;

public static class ServiceBusConstants
{
    public static readonly string ConnectionString =
        "Endpoint=sb://sb-projetossisprev-noprd-premium-dev-tst.servicebus.windows.net/;SharedAccessKeyName=develop;SharedAccessKey=vB4+YV7oIfuE6ENe7h5KSpxhVBbNMs4sy+ASbOjvpEg=";

    public static readonly string LocalServiceBusClientName = nameof(LocalServiceBusClientName);
}