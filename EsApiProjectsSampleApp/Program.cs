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
            await billingCountriesResponse.Content.ReadAsStringAsync());
        return;
    }
    
    var billingCountries = await billingCountriesResponse.Content.ReadFromJsonAsync<PagedResponse<string>>();
    var billingCountry = (billingCountries?.Items ?? Enumerable.Empty<string>()).First();

    ConsoleApp.Log("Will use '{0}' billing country", billingCountry);

    // Get first Data center from the list
    ConsoleApp.Log("Fetching data centers");

    // Not handling possible network or status code errors for simplicity's sake
    var dataCenters = await client.GetFromJsonAsync<PagedResponse<string>>(
        $"/project/v1/projectTypes/{configuration.ProjectType}/datacenters");
    var dataCenter = (dataCenters?.Items ?? Enumerable.Empty<string>()).First();

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

    var createProject = new CreateProject()
    {
        TemplateId = template.Id,
        DisplayName = arguments.Name,
        Number = arguments.Name,
        DataCenterLocation = dataCenter,
        BillingCountry = billingCountry,
    };
    var createProjectResponse = await client.PostAsync("project/v1/projects", JsonContent.Create(createProject));

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

    string provisionState;
    do
    {
        var status = await client.GetFromJsonAsync<ProvisionStatus>($"project/v1/projects/{projectId}/provisions/{provisionId}");
        
        provisionState = status?.State ?? throw new Exception("Service could not retrieve provision state");
        
        ConsoleApp.Log("Provision status: {0}", provisionState);
        if (provisionState is "Queued" or "Created" or "Started")
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

    } while (provisionState is "Queued" or "Created" or "Started");

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
    var projects = (projectsResponse?.Items ?? Enumerable.Empty<Project>());

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
