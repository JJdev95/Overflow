using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("production")
    .WithDashboard(dashboard => dashboard.WithHostPort(8080));

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithDataVolume("keycloak-data")
    //.WithRealmImport("../infra/realms")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEnvironment("VIRTUAL_HOST", "id.overflow.local")
    .WithEnvironment("VIRTUAL_PORT", "8080");


var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data")
    .WithPgAdmin();

var typesenseApiKey = builder.AddParameter("typesense-api-key", secret: true);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithVolume("typesense-data", "/data")
    .WithEnvironment("TYPESENSE_DATA_DIR", "/data")
    .WithEnvironment("TYPESENSE_ENABLE_CORS", "true")
    .WithEnvironment("TYPESENSE_API_KEY", typesenseApiKey)
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typesenseContainer = typesense.GetEndpoint("typesense");

var questionDb = postgres.AddDatabase("questionDb");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port: 15672);

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WithReference(rabbitmq)
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(questionDb);

var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithEnvironment("typesense-api-key", typesenseApiKey)
    .WithReference(typesenseContainer)
    .WithReference(rabbitmq)
    .WaitFor(typesense)
    .WaitFor(rabbitmq);

// This is only for local development. In production, the YARP gateway will be exposed through the nginx-proxy container
var yarp = builder.AddYarp("gateway")
    .WithConfiguration(yarpBuilder =>
    {
        yarpBuilder.AddRoute("/questions/{**catch-all}", questionService);
        yarpBuilder.AddRoute("/tags/{**catch-all}", questionService);
        yarpBuilder.AddRoute("/search/{**catch-all}", searchService);
    })
    .WithEnvironment("ASPNETCORE_URLS", "http://*:8001")
    .WithHostPort(8001)
    .WithEnvironment("VIRTUAL_HOST", "api.overflow.local")
    .WithEnvironment("VIRTUAL_PORT", "8001");

// This is meant for production, where the YARP gateway is exposed through the nginx-proxy container. It won't work in development because the nginx-proxy container isn't used there
// var yarp = builder.AddYarp("gateway")
//     .WithConfiguration(yarpBuilder =>
//     {
//         yarpBuilder.AddRoute("/questions/{**catch-all}", questionService);
//         yarpBuilder.AddRoute("/tags/{**catch-all}", questionService);
//         yarpBuilder.AddRoute("/search/{**catch-all}", searchService);
//     })
//     .WithEnvironment("ASPNETCORE_URLS", "http://*:8001")
//     .WithEndpoint(port: 8001, scheme: "http", targetPort: 8001, name: "gateway", isExternal: true)
//     .WithEnvironment("VIRTUAL_HOST", "api.overflow.local")
//     .WithEnvironment("VIRTUAL_PORT", "8001");

var webapp = builder.AddJavaScriptApp("webapp", "../webapp")
    .WithReference(keycloak)
    .WithHttpEndpoint(env: "PORT", port: 3000);

if (!builder.Environment.IsDevelopment())
{
    builder.AddContainer("nginx-proxy", "nginxproxy/nginx-proxy", "1.9")
        .WithEndpoint(80, 80, name: "nginx", isExternal: true)
        .WithBindMount("/var/run/docker.sock", "/tmp/docker.sock", true);
}

await builder.Build().RunAsync();

