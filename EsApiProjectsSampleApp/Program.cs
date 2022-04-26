using System.Net;
using System.Net.Http.Json;
using EsApiProjectsSampleApp;

await ConsoleApp.RunAsync(args, async (arguments, configuration) =>
{
    var client = new HttpClient()
    {
        BaseAddress = configuration.ServiceHost,
        DefaultRequestHeaders = { { "Authorization", $"Bearer {arguments.Token}" } },
    };

    // Get first Billing country from the list
    ConsoleApp.Log("Fetching billing countries");

    var billingCountriesResponse = await client.GetAsync("/user/v1/users/current/billingCountries");

    // Handling first request for possible authentication/authorization errors
    if (!billingCountriesResponse.IsSuccessStatusCode)
    {
        ConsoleApp.Log("Fetching billing countries failed with status code: {0}.\nResponse: {1}",
            billingCountriesResponse.StatusCode,
            await billingCountriesResponse.Content.ReadAsStringAsync(),
            string.Join(";", billingCountriesResponse.Headers.Select(h => h.ToString())));
        if (billingCountriesResponse.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden &&
            billingCountriesResponse.Headers.TryGetValues("WWW-Authenticate", out var reasonHeader))
        {
            ConsoleApp.Log("Authentication failure reason: {0}", string.Join(";", reasonHeader));
        }
        return;
    }
    
    var billingCountries = await billingCountriesResponse.Content.ReadFromJsonAsync<PagedResponse<string>>();
    var billingCountry = billingCountries?.Items.FirstOrDefault() ?? throw new Exception("Could not find any billing countries");

    ConsoleApp.Log("Will use '{0}' billing country", billingCountry);

    // Get first Data center from the list
    ConsoleApp.Log("Fetching data centers");

    // Not handling possible network or status code errors for simplicity's sake
    var dataCenters = await client.GetFromJsonAsync<PagedResponse<string>>(
        $"/project/v1/projectTypes/{configuration.ProjectType}/datacenters");
    var dataCenter = dataCenters?.Items.FirstOrDefault() ?? throw new Exception("Could not find any data centers");

    ConsoleApp.Log("Will use '{0}' data center", dataCenter);

    // Get first template Id from Global templates by name
    ConsoleApp.Log("Fetching global templates");

    // You could also use an organization template by changing endpoint from templates/bentley to templates/organization
    var templates = await client.GetFromJsonAsync<PagedResponse<Template>>(
        $"project/v1/projectTypes/{configuration.ProjectType}/templates/bentley");
    var template = templates?.Items.FirstOrDefault() ?? throw new Exception($"Could not find a global '{configuration.ProjectType}' template");

    ConsoleApp.Log("Will use '{0}' template", template.DisplayName);

    // Create project
    ConsoleApp.Log($"Starting project '{arguments.Name}' creation");

    var createProject = new CreateProject(
        TemplateId: template.Id,
        DisplayName: arguments.Name,
        Number: arguments.Name,
        DataCenterLocation: dataCenter,
        BillingCountry: billingCountry);
    var createProjectResponse = await client.PostAsJsonAsync("project/v1/projects", createProject);

    // Handling create project request because it contains custom status codes:
    //  - 422 - when provided model is incorrect (like non-existent data center location or billing country)
    //  - 409 - when a project with provided project name already exists
    if (!createProjectResponse.IsSuccessStatusCode)
    {
        ConsoleApp.Log("Creating project failed with status code: {0}.\nResponse: {1}",
            createProjectResponse.StatusCode,
            await createProjectResponse.Content.ReadAsStringAsync());
        return;
    }

    var createdProjectProvision = await createProjectResponse.Content.ReadFromJsonAsync<Provision>();
    var projectId = createdProjectProvision?.ProjectId;
    var provisionId = createdProjectProvision?.ProvisionId;

    // Wait for create project provision to finish
    ConsoleApp.Log("Waiting for new project to be provisioned");

    async Task<string?> GetProvisionStateAsync() =>
        (await client.GetFromJsonAsync<ProvisionStatus>($"project/v1/projects/{projectId}/provisions/{provisionId}"))
        ?.State;

    string? provisionState;
    while ((provisionState = await GetProvisionStateAsync()) is CopyState.Queued or CopyState.Created or CopyState.Started)
    {
        ConsoleApp.Log("Provision status: {0}", provisionState);
        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    ConsoleApp.Log("Provisioning finished. Fetching project.");

    // Get project by Id
    var createdProject = await client.GetFromJsonAsync<Project>($"project/v1/projects/{projectId}");

    // Print project info to console
    ConsoleApp.Log("Created project:");
    ConsoleApp.Log("    Id: {0}", createdProject?.Id);
    ConsoleApp.Log("    DisplayName: {0}", createdProject?.DisplayName);
    ConsoleApp.Log("    Number: {0}", createdProject?.Number);

    // Get projects list
    ConsoleApp.Log("Fetching created project list.");

    var projectsResponse = await client.GetFromJsonAsync<PagedResponse<Project>>($"project/v1/projects?projectName={arguments.Name}");
    var projects = projectsResponse?.Items ?? throw new Exception("Could not parse GET projects response.");

    // Print projects to console
    ConsoleApp.Log("Project names:");
    foreach (var project in projects)
    {
        ConsoleApp.Log("    - {0}", project.DisplayName);
    }

    // Delete created project
    ConsoleApp.Log("Deleting created project.");

    var response = await client.DeleteAsync($"project/v1/projects/{projectId}");
    response.EnsureSuccessStatusCode();

    ConsoleApp.Log("Project deleted.");
});
