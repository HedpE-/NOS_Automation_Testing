using Newtonsoft.Json; // For JSON serialization/deserialization
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework; // For unit testing
using RestSharp; // For REST API calls
using System.Net;

namespace NOS_Automation_Testing
{
    [TestFixture]
    public class GoRestApiTests
    {
        // Setup method to initialize RestClient before each test
        [SetUp]
        public void Setup()
        {
            GlobalVariables.client = new RestClient(GlobalVariables.apiUrl);
        }

        [Test]
        [Description("POST /public/v2/users - Create a new user and verify the response contains the user ID.")]
        public async Task CreateUserTest()
        {
            // Create a user
            RestResponse response = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Assert that the response status is Created (201)
            Assert.That(response.StatusCode == HttpStatusCode.Created, "Failed to create user");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the user ID is not null
            Assert.That(!string.IsNullOrEmpty(responseData["id"]), "User ID should not be null");

            // Validate schema
            ValidateJsonSchema(response.Content, GlobalVariables.userSchema);
        }

        [Test]
        [Description("GET /public/v2/users/{id} - Get an existing user and verify the response contains the user details.")]
        public async Task GetUserTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a request to get the user by ID
            RestRequest request = new RestRequest($"public/v2/users/{userId}", Method.Get);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to get user");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the user ID matches the expected ID
            Assert.That(responseData["id"] == userId, "User ID should match");
        }

        [Test]
        [Description("PUT /public/v2/users/{id} - Update an existing user and verify the response contains the updated details.")]
        public async Task UpdateUserTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Define data for the updated user
            string newUserName = "John Smith";
            string newUserStatus = "inactive";

            // Create a request to update the user by ID
            RestRequest request = new RestRequest($"public/v2/users/{userId}", Method.Put);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body with updated user details
            request.AddJsonBody(new
            {
                name = newUserName,
                status = newUserStatus
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to update user");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the user name and status are updated
            Assert.That(responseData["name"] == newUserName, "User name should be updated");
            Assert.That(responseData["status"] == newUserStatus, "User status should be updated");
        }

        [Test]
        [Description("DELETE /public/v2/users/{id} - Delete an existing user and verify the response status.")]
        public async Task DeleteUserTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = responseData["id"];

            // Create a request to delete the user by ID
            RestRequest request = new RestRequest($"public/v2/users/{userId}", Method.Delete);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is NoContent (204)
            Assert.That(response.StatusCode == HttpStatusCode.NoContent, "Failed to delete user");
        }

        [Test]
        [Description("POST /public/v2/posts - Create a new post and verify the response contains the post ID.")]
        public async Task CreatePostTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse response = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Assert that the response status is Created (201)
            Assert.That(response.StatusCode == HttpStatusCode.Created, "Failed to create post");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the post ID is not null
            Assert.That(!string.IsNullOrEmpty(responseData["id"]), "Post ID should not be null");

            // Validate schema
            ValidateJsonSchema(response.Content, GlobalVariables.postSchema);
        }

        [Test]
        [Description("GET /public/v2/posts/{id} - Get an existing post and verify the response contains the post details.")]
        public async Task GetPostTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a request to get the post by ID
            RestRequest request = new RestRequest($"public/v2/posts/{postId}", Method.Get);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to get post");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the post ID matches the expected ID
            Assert.That(responseData["id"] == postId, "Post ID should match");
        }

        [Test]
        [Description("PUT /public/v2/posts/{postId} - Update an existing post and verify the response contains the updated details.")]
        public async Task UpdatePostTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Define data for the updated post
            string newPostTitle = "Updated Post Title";
            string newPostBody = "Updated body of the post.";

            // Create a request to update the post by ID
            RestRequest request = new RestRequest($"public/v2/posts/{postId}", Method.Put);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body with updated post details
            request.AddJsonBody(new
            {
                title = newPostTitle,
                body = newPostBody
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to update post");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the post title and body are updated
            Assert.That(responseData["title"] == newPostTitle, "Post title should be updated");
            Assert.That(responseData["body"] == newPostBody, "Post body should be updated");
        }

        [Test]
        [Description("DELETE /public/v2/posts/{postId} - Delete an existing post and verify the response status.")]
        public async Task DeletePostTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a request to delete the post by ID
            RestRequest request = new RestRequest($"public/v2/posts/{postId}", Method.Delete);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is NoContent (204)
            Assert.That(response.StatusCode == HttpStatusCode.NoContent, "Failed to delete post");
        }

        [Test]
        [Description("POST /public/v2/comments - Create a new comment and verify the response contains the comment ID.")]
        public async Task CreateCommentTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a comment
            RestResponse response = await CreateComment(postId, "Jane Doe", $"jane.doe@example.com", "This is a comment.");

            // Assert that the response status is Created (201)
            Assert.That(response.StatusCode == HttpStatusCode.Created, "Failed to create comment");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the comment ID is not null
            Assert.That(!string.IsNullOrEmpty(responseData["id"]), "Comment ID should not be null");

            // Validate schema
            ValidateJsonSchema(response.Content, GlobalVariables.commentSchema);
        }

        [Test]
        [Description("GET /public/v2/comments/{commentId} - Get an existing comment and verify the response contains the comment details.")]
        public async Task GetCommentTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a comment
            RestResponse createCommentResponse = await CreateComment(postId, "Jane Doe", $"jane.doe@example.com", "This is a comment.");

            // Deserialize the response content
            Dictionary<string, string> createCommentResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createCommentResponse.Content);

            // Get the comment ID from the deserialized response
            string commentId = createCommentResponseData["id"];

            // Create a request to get the comment by ID
            RestRequest request = new RestRequest($"public/v2/comments/{commentId}", Method.Get);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to get comment");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the comment ID matches the expected ID
            Assert.That(responseData["id"] == commentId, "Comment ID should match");
        }

        [Test]
        [Description("PUT /public/v2/comments/{commentId} - Update an existing comment and verify the response contains the updated details.")]
        public async Task UpdateCommentTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a comment
            RestResponse createCommentResponse = await CreateComment(postId, "Jane Doe", $"jane.doe@example.com", "This is a comment.");

            // Deserialize the response content
            Dictionary<string, string> createCommentResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createCommentResponse.Content);

            // Get the comment ID from the deserialized response
            string commentId = createCommentResponseData["id"];

            // Define data for updated comment
            string newCommentName = "Jane Smith";
            string newCommentBody = "Updated comment body.";

            // Create a request to update the comment by ID
            RestRequest request = new RestRequest($"public/v2/comments/{commentId}", Method.Put);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body with updated comment details
            request.AddJsonBody(new
            {
                name = newCommentName,
                body = newCommentBody
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to update post");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the comment name and body are updated
            Assert.That(responseData["name"] == newCommentName, "Comment name should be updated");
            Assert.That(responseData["body"] == newCommentBody, "Comment body should be updated");
        }

        [Test]
        [Description("DELETE /public/v2/comments/{commentId} - Delete an existing comment and verify the response status.")]
        public async Task DeleteCommentTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a post
            RestResponse createPostresponse = await CreatePost(userId, "New Post", "This is the body of the new post.");

            // Deserialize the response content
            Dictionary<string, string> createPostResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the post ID from the deserialized response
            string postId = createPostResponseData["id"];

            // Create a comment
            RestResponse createCommentResponse = await CreateComment(postId, "Jane Doe", $"jane.doe@example.com", "This is a comment.");

            // Deserialize the response content
            Dictionary<string, string> createCommentResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createCommentResponse.Content);

            // Get the comment ID from the deserialized response
            string commentId = createCommentResponseData["id"];

            // Create a request to delete the comment by ID
            RestRequest request = new RestRequest($"public/v2/comments/{commentId}", Method.Delete);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is NoContent (204)
            Assert.That(response.StatusCode == HttpStatusCode.NoContent, "Failed to delete comment");
        }

        [Test]
        [Description("POST /public/v2/todos - Create a new todo and verify the response contains the todo ID.")]
        public async Task CreateTodoTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a todo
            RestResponse response = await CreateTodo(userId, "New Todo", DateTime.Now.AddDays(7), "pending");

            // Assert that the response status is Created (201)
            Assert.That(response.StatusCode == HttpStatusCode.Created, "Failed to create todo");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the todo ID is not null
            Assert.That(!string.IsNullOrEmpty(responseData["id"]), "Todo ID should not be null");

            // Validate schema
            ValidateJsonSchema(response.Content, GlobalVariables.todoSchema);
        }

        [Test]
        [Description("GET /public/v2/todos/{todoId} - Get an existing todo and verify the response contains the todo details.")]
        public async Task GetTodoTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a todo
            RestResponse createTodoResponse = await CreateTodo(userId, "New Todo", DateTime.Now.AddDays(7), "pending");

            // Deserialize the response content
            Dictionary<string, string> createTodoResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createTodoResponse.Content);

            // Get the todo ID from the deserialized response
            string todoId = createTodoResponseData["id"];

            // Create a request to get the todo by ID
            RestRequest request = new RestRequest($"todos/{todoId}", Method.Get);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to get todo");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the todo ID matches the expected ID
            Assert.That(responseData["id"] == todoId, "Todo ID should match");
        }

        [Test]
        [Description("PUT /public/v2/todos/{todoId} - Update an existing todo and verify the response contains the updated details.")]
        public async Task UpdateTodoTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a todo
            RestResponse createTodoResponse = await CreateTodo(userId, "New Todo", DateTime.Now.AddDays(7), "pending");

            // Deserialize the response content
            Dictionary<string, string> createTodoResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createTodoResponse.Content);

            // Get the todo ID from the deserialized response
            string todoId = createTodoResponseData["id"];

            // Define data for updated todo
            string newTodoTitle = "Updated Todo Title";
            string newTodoStatus = "completed";

            // Create a request to update the todo by ID
            RestRequest request = new RestRequest($"todos/{todoId}", Method.Put);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body with updated todo details
            request.AddJsonBody(new
            {
                title = newTodoTitle,
                status = newTodoStatus
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is OK (200)
            Assert.That(response.StatusCode == HttpStatusCode.OK, "Failed to update todo");

            // Deserialize the response content
            Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            // Assert that the todo title and status are updated
            Assert.That(responseData["title"] == newTodoTitle, "Todo title should be updated");
            Assert.That(responseData["status"] == newTodoStatus, "Todo status should be updated");
        }

        [Test]
        [Description("DELETE /public/v2/todos/{todoId} - Delete an existing todo and verify the response status.")]
        public async Task DeleteTodoTest()
        {
            // Create a user
            RestResponse createUserResponse = await CreateUser("John Doe", "male", "john.doe@example.com", "active");

            // Deserialize the response content
            Dictionary<string, string> createUserResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createUserResponse.Content);

            // Get the user ID from the deserialized response
            string userId = createUserResponseData["id"];

            // Create a todo
            RestResponse createTodoResponse = await CreateTodo(userId, "New Todo", DateTime.Now.AddDays(7), "pending");

            // Deserialize the response content
            Dictionary<string, string> createTodoResponseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(createTodoResponse.Content);

            // Get the todo ID from the deserialized response
            string todoId = createTodoResponseData["id"];

            // Create a request to delete the todo by ID
            RestRequest request = new RestRequest($"todos/{todoId}", Method.Delete);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);

            // Assert that the response status is NoContent (204)
            Assert.That(response.StatusCode == HttpStatusCode.NoContent, "Failed to delete todo");
        }

        #region Private utility methods

        private async Task<RestResponse> CreateUser(string name, string gender, string email, string status)
        {
            // Create a request for creating a user
            RestRequest request = new RestRequest("public/v2/users", Method.Post);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body for the new user
            request.AddJsonBody(new
            {
                name = "John Doe",
                gender = "male",
                email = "john.doe@example.com",
                status = "active"
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            return response;
        }

        private async Task<RestResponse> CreatePost(string userId, string title, string body)
        {
            // Create a request for creating a post
            RestRequest request = new RestRequest("public/v2/posts", Method.Post);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body for the new post
            request.AddJsonBody(new
            {
                user_id = userId,
                title = title,
                body = body
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            return response;
        }

        private async Task<RestResponse> CreateComment(string postId, string name, string email, string body)
        {
            // Create a request for creating a comment
            RestRequest request = new RestRequest("public/v2/comments", Method.Post);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body for the new comment
            request.AddJsonBody(new
            {
                post_id = postId,
                name = name,
                email = email,
                body = body
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            return response;
        }

        private async Task<RestResponse> CreateTodo(string userId, string title, DateTime due_on, string status)
        {
            // Create a request for creating a todo
            RestRequest request = new RestRequest("public/v2/todos", Method.Post);
            // Add authorization header
            request.AddHeader("Authorization", $"Bearer {GlobalVariables.apiKey}");
            // Add JSON body for the new todo
            request.AddJsonBody(new
            {
                user_id = userId,
                title = title,
                due_on = due_on.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                status = status
            });

            // Execute the request and get the response
            RestResponse response = await GlobalVariables.client.ExecuteAsync(request);
            return response;
        }

        private void ValidateJsonSchema(string jsonContent, string schema)
        {
            var jsonSchema = JSchema.Parse(schema);
            var json = JToken.Parse(jsonContent);

            if (!json.IsValid(jsonSchema, out IList<string> validationErrors))
            {
                Assert.Fail("JSON schema validation failed: " + string.Join(", ", validationErrors));
            }
        }

        #endregion Private utility methods
    }
}