using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using RestSharp;
using System.Net;

namespace NOS_Automation_Testing
{
    [TestFixture]
    public class TodosApiTests
    {
        // Setup method to initialize RestClient before each test
        [SetUp]
        public void Setup()
        {
            GlobalVariables.client = new RestClient(GlobalVariables.apiUrl);
        }

        [Test]
        public void GetTodos_ShouldValidateJsonSchema()
        {
            // Parse the JSON schema to JSchema object
            JSchema todosSchema = JSchema.Parse(GlobalVariables.todosSchema);

            // Create a GET request to the "todos" endpoint
            var request = new RestRequest("public/v2/todos", Method.Get);
            // Add Authorization header with the API key
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Execute the request and get the response
            var response = GlobalVariables.client.Execute(request);

            // Ensure the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK);

            // Parse the response content to JArray
            JArray todos = JArray.Parse(response.Content);

            // Validate the JSON response against the schema
            bool isValid = todos.IsValid(todosSchema, out IList<ValidationError> errors);

            // Assert that the response is valid according to the schema
            Assert.That(isValid, "JSON response does not match the expected schema");

            // If there are validation errors, print them
            if (!isValid)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        [Test]
        public void GetTodos_ShouldValidateAllStatusCompletedAndDueOnValue()
        {
            // Create a GET request to the "todos" endpoint
            var request = new RestRequest("public/v2/todos", Method.Get);
            // Add Authorization header with the API key
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Execute the request and get the response
            var response = GlobalVariables.client.Execute(request);

            // Ensure the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK);

            // Parse the response content to JArray
            JArray todos = JArray.Parse(response.Content);

            // Validate that all todos have status "completed" and valid "due_on" value
            foreach (var todo in todos)
            {
                // Check if todo has status "completed"
                string status = todo["status"].ToString();
                Assert.That(status == "completed", $"Todo with ID {todo["id"]} does not have status 'completed'.");

                // Check if the "due_on" is a valid date-time and not in the past
                string dueOn = todo["due_on"].ToString();
                DateTime dueOnDate;
                bool isDateTime = DateTime.TryParse(dueOn, out dueOnDate);
                Assert.That(isDateTime, $"Todo with ID {todo["id"]} has an invalid 'due_on' value.");
                Assert.That(dueOnDate >= DateTime.UtcNow, $"Todo with ID {todo["id"]} has a 'due_on' value in the past.");
            }
        }
    }
}
