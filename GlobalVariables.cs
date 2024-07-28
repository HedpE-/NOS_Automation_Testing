using RestSharp;

namespace NOS_Automation_Testing
{
    internal static class GlobalVariables
    {
        // Base URL for the API
        public static readonly string apiUrl = "https://gorest.co.in/";
        // API key for authorization
        public static readonly string apiKey = "18fb9c9414d7dc7b5e96e3df4374ac551144d55fb5e4628069a67fbf2c90828c";
        // RestClient instance for making API calls
        public static RestClient client = null;

        // Define user schema
        public static readonly string userSchema = @"
        {
            'type': 'object',
            'properties': {
                'id': { 'type': 'integer' },
                'name': { 'type': 'string' },
                'email': { 'type': 'string' },
                'gender': { 'type': 'string' },
                'status': { 'type': 'string' }
            },
            'required': ['id', 'name', 'email', 'gender', 'status']
        }";

        // Define post schema
        public static readonly string postSchema = @"
        {
            'type': 'object',
            'properties': {
                'id': { 'type': 'integer' },
                'user_id': { 'type': 'integer' },
                'title': { 'type': 'string' },
                'body': { 'type': 'string' }
            },
            'required': ['id', 'user_id', 'title', 'body']
        }";

        // Define comment schema
        public static readonly string commentSchema = @"
        {
            'type': 'object',
            'properties': {
                'id': { 'type': 'integer' },
                'post_id': { 'type': 'integer' },
                'name': { 'type': 'string' },
                'email': { 'type': 'string' },
                'body': { 'type': 'string' }
            },
            'required': ['id', 'post_id', 'name', 'email', 'body']
        }";

        // Define todo schema
        public static readonly string todoSchema = @"
        {
            'type': 'object',
            'properties': {
                'id': { 'type': 'integer' },
                'user_id': { 'type': 'integer' },
                'title': { 'type': 'string' },
                'due_on': { 'type': 'string' },
                'status': { 'type': 'string' }
            },
            'required': ['id', 'user_id', 'title', 'due_on', 'status']
        }";

        public static readonly string todosSchema = @"
            {
                'type': 'array',
                'items': {
                    'type': 'object',
                    'properties': {
                        'id': { 'type': 'integer' },
                        'user_id': { 'type': 'integer' },
                        'title': { 'type': 'string' },
                        'due_on': { 'type': 'string', 'format': 'date-time' },
                        'status': { 'type': 'string' }
                    },
                    'required': ['id', 'user_id', 'title', 'due_on', 'status']
                }
            }";
    }
}
