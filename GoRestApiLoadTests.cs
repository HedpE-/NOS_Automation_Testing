using NBomber.Contracts; // For NBomber contracts and interfaces
using NBomber.CSharp; // For using NBomber in C#
using Newtonsoft.Json; // For JSON serialization and deserialization
using RestSharp; // For making HTTP requests
using System.Net; // For handling HTTP status codes

namespace NOS_Automation_Testing
{
    [TestFixture]
    public class GoRestApiLoadTests
    {
        // Setup method to initialize RestClient before each test
        [SetUp]
        public void Setup()
        {
            GlobalVariables.client = new RestClient(GlobalVariables.apiUrl);
        }

        // NUnit test method to run all load tests
        [Test]
        public void RunLoadTests()
        {
            // Define scenarios for all load tests
            Scenario createUserScenario = CreateUserScenario();
            Scenario getUserScenario = GetUserScenario();
            Scenario createPostScenario = CreatePostScenario();
            Scenario getPostScenario = GetPostScenario();
            Scenario createCommentScenario = CreateCommentScenario();
            Scenario getCommentScenario = GetCommentScenario();
            Scenario createTodoScenario = CreateTodoScenario();
            Scenario getTodoScenario = GetTodoScenario();

            // Register and run all scenarios using NBomber
            NBomberRunner
                .RegisterScenarios(
                    createUserScenario,
                    getUserScenario,
                    createPostScenario,
                    getPostScenario,
                    createCommentScenario,
                    getCommentScenario,
                    createTodoScenario,
                    getTodoScenario
                )
                .Run(); // Execute the load tests
        }

        #region NBomber Scenarios

        private Scenario CreateUserScenario()
        {
            // Define a step to create a user
            IStep step = Step.Create("create_user", async context =>
            {
                // Create a POST request to create a new user
                RestRequest request = new RestRequest("public/v2/users", Method.Post);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
                request.AddJsonBody(new // Add JSON body with user details
                {
                    name = "John Doe",
                    gender = "male",
                    email = $"john.doe@example.com",
                    status = "active"
                });

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.Created
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("create_user_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario GetUserScenario()
        {
            // Define a step to get a user
            IStep step = Step.Create("get_user", async context =>
            {
                // Create a new user and get its ID
                string userId = await CreateUserAndGetId();

                // Create a GET request to fetch the user details
                RestRequest request = new RestRequest($"public/v2/users/{userId}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.OK
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("get_user_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario CreatePostScenario()
        {
            // Define a step to create a post
            IStep step = Step.Create("create_post", async context =>
            {
                // Create a new user and get its ID
                string userId = await CreateUserAndGetId();

                // Create a POST request to create a new post
                RestRequest request = new RestRequest("public/v2/posts", Method.Post);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
                request.AddJsonBody(new // Add JSON body with post details
                {
                    user_id = userId,
                    title = "My Post Title",
                    body = "This is the body of the post."
                });

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.Created
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("create_post_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario GetPostScenario()
        {
            // Define a step to get a post
            IStep step = Step.Create("get_post", async context =>
            {
                // Create a new user and post, then get the post ID
                string postId = await CreatePostAndGetId();

                // Create a GET request to fetch the post details
                RestRequest request = new RestRequest($"public/v2/posts/{postId}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.OK
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("get_post_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario CreateCommentScenario()
        {
            // Define a step to create a comment
            IStep step = Step.Create("create_comment", async context =>
            {
                // Create a new user and post, then get the post ID
                string postId = await CreatePostAndGetId();

                // Create a POST request to create a new comment
                RestRequest request = new RestRequest("public/v2/comments", Method.Post);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
                request.AddJsonBody(new // Add JSON body with comment details
                {
                    post_id = postId,
                    name = "John Doe",
                    email = $"john.doe@example.com",
                    body = "This is a comment."
                });

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.Created
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("create_comment_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario GetCommentScenario()
        {
            // Define a step to get a comment
            IStep step = Step.Create("get_comment", async context =>
            {
                // Create a new user and comment, then get the comment ID
                string commentId = await CreateCommentAndGetId();

                // Create a GET request to fetch the comment details
                RestRequest request = new RestRequest($"public/v2/comments/{commentId}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.OK
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("get_comment_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario CreateTodoScenario()
        {
            // Define a step to create a todo
            IStep step = Step.Create("create_todo", async context =>
            {
                // Create a new user and get its ID
                string userId = await CreateUserAndGetId();

                // Create a POST request to create a new todo
                RestRequest request = new RestRequest("public/v2/todos", Method.Post);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
                request.AddJsonBody(new // Add JSON body with todo details
                {
                    user_id = userId,
                    title = "My Todo",
                    due_on = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss"),
                    status = "pending"
                });

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.Created
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("create_todo_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        private Scenario GetTodoScenario()
        {
            // Define a step to get a todo
            IStep step = Step.Create("get_todo", async context =>
            {
                // Create a new user and todo, then get the todo ID
                string todoId = await CreateTodoAndGetId();

                // Create a GET request to fetch the todo details
                RestRequest request = new RestRequest($"public/v2/todos/{todoId}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header

                // Execute the request and get the response
                RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

                // Return response based on status code
                return response.StatusCode == HttpStatusCode.OK
                    ? Response.Ok(statusCode: (int)response.StatusCode)
                    : Response.Fail(statusCode: (int)response.StatusCode);
            });

            // Define and return the scenario
            Scenario scenario = ScenarioBuilder.CreateScenario("get_todo_scenario", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10)) // Warm-up duration
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)) // Load simulation
                );

            return scenario;
        }

        #endregion NBomber Scenarios

        #region Helper methods

        private async Task<string> CreateUserAndGetId()
        {
            // Create a POST request to create a new user
            RestRequest request = new RestRequest("public/v2/users", Method.Post);
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
            request.AddJsonBody(new // Add JSON body with user details
            {
                name = "John Doe",
                gender = "male",
                email = $"john.doe@example.com",
                status = "active"
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            return responseData["id"];
        }

        private async Task<string> CreatePostAndGetId()
        {
            // Create a new user and get its ID
            string userId = await CreateUserAndGetId();

            // Create a POST request to create a new post
            RestRequest request = new RestRequest("public/v2/posts", Method.Post);
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
            request.AddJsonBody(new // Add JSON body with post details
            {
                user_id = userId,
                title = "My Post Title",
                body = "This is the body of the post."
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            return responseData["id"];
        }

        private async Task<string> CreateCommentAndGetId()
        {
            // Create a new user and post, then get the post ID
            string postId = await CreatePostAndGetId();

            // Create a POST request to create a new comment
            RestRequest request = new RestRequest("public/v2/comments", Method.Post);
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
            request.AddJsonBody(new // Add JSON body with comment details
            {
                post_id = postId,
                name = "John Doe",
                email = $"john.doe@example.com",
                body = "This is a comment."
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            return responseData["id"];
        }

        private async Task<string> CreateTodoAndGetId()
        {
            // Create a new user and get its ID
            string userId = await CreateUserAndGetId();

            // Create a POST request to create a new todo
            RestRequest request = new RestRequest("public/v2/todos", Method.Post);
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}"); // Add authorization header
            request.AddJsonBody(new // Add JSON body with todo details
            {
                user_id = userId,
                title = "My Todo",
                due_on = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                status = "pending"
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            return responseData["id"];
        }

        #endregion Helper methods
    }
}