using System.Net.Http.Json;
using EsApiProjectsSampleApp;
using Polly;

await ConsoleApp.RunAsync(args, async (arguments, configuration) =>
{
    var client = new HttpClient()
    {
        BaseAddress = configuration.ServiceHost,
        DefaultRequestHeaders = { { "Authorization", $"Bearer {arguments.Token}" } },
    };

    // Get first Billing country from the list
    Console.WriteLine("Fetching billing countries");

    var billingCountries = await client.GetFromJsonAsync<PagedResponse<string>>("/user/v1/users/current/billingCountries");
    var billingCountry = (billingCountries?.Items ?? Enumerable.Empty<string>()).First();

    Console.WriteLine("Will use '{0}' billing country", billingCountry);

    // Get first Data center from the list
    Console.WriteLine("Fetching data centers");

    var dataCenters = await client.GetFromJsonAsync<PagedResponse<string>>(
        $"/project/v1/projectTypes/{configuration.ProjectType}/datacenters");
    var dataCenter = (dataCenters?.Items ?? Enumerable.Empty<string>()).First();

    Console.WriteLine("Will use '{0}' data center", dataCenter);

    // Get first template Id from Global templates by name 
    Console.WriteLine("Fetching global templates");

    var templates = await client.GetFromJsonAsync<PagedResponse<Template>>(
        $"project/v1/projects/templates/bentley?projectType={configuration.ProvisionProjectType}");
    var template = templates?.Items.FirstOrDefault() ?? throw new Exception($"Could not find a global '{configuration.ProvisionProjectType}' template");

    Console.WriteLine("Will use '{0}' template", template.DisplayName);

    // Create project
    Console.WriteLine("Starting project creation");

    var createProject = new CreateProject()
    {
        TemplateId = template.Id,
        DisplayName = arguments.Name,
        Number = arguments.Name,
        DataCenterLocation = dataCenter,
        BillingCountry = billingCountry,
    };
    var createProjectResponse = await client.PostAsync("project/v1/projects", JsonContent.Create(createProject));
    createProjectResponse.EnsureSuccessStatusCode();
    var createdProjectProvision = await createProjectResponse.Content.ReadFromJsonAsync<Provision>();

    // Wait for create project provision to finish
    Console.WriteLine("Waiting for new project to be provisioned");

    var retryPolicy = Policy
        .HandleResult<ProvisionStatus?>(s => s?.State is "Queued" or "Created" or "Started")
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));
    var timeoutPolicy = Policy.TimeoutAsync(120);

    var policy = timeoutPolicy.WrapAsync(retryPolicy);

    var projectId = createdProjectProvision?.ProjectId;
    var provisionId = createdProjectProvision?.ProvisionId;

    var lastState = await policy.ExecuteAndCaptureAsync(async () =>
    {
        var provision = await client.GetFromJsonAsync<ProvisionStatus>($"project/v1/projects/{projectId}/provisions/{provisionId}");

        Console.WriteLine("Provision status: {0}", provision?.State);

        return provision;
    });

    if (lastState.Result?.State is not "Succeeded")
    {
        throw new Exception($"Provision failed. Last known state: '{lastState.Result?.State}'");
    }

    // Get project by Id
    Console.WriteLine("Provisioning finished. Fetching project.");

    var createdProject = await client.GetFromJsonAsync<Project>($"project/v1/projects/{projectId}");

    // Print project info to console
    Console.WriteLine("Created project:");
    Console.WriteLine("    Id: {0}", createdProject?.Id);
    Console.WriteLine("    DisplayName: {0}", createdProject?.DisplayName);
    Console.WriteLine("    Number: {0}", createdProject?.Number);

    // Get projects list
    Console.WriteLine("Fetching user project list.");

    var projectsResponse = await client.GetFromJsonAsync<PagedResponse<Project>>($"project/v1/projects?projectName={arguments.Name}");
    var projects = (projectsResponse?.Items ?? Enumerable.Empty<Project>());

    // Print projects to console
    Console.WriteLine("Project names:");
    foreach (var project in projects)
    {
        Console.WriteLine("    - {0}", project.DisplayName);
    }

    // Delete created project
    Console.WriteLine("Deleting created project.");

    var response = await client.DeleteAsync($"project/v1/projects/{projectId}");
    response.EnsureSuccessStatusCode();

    Console.WriteLine("Project deleted.");
});
