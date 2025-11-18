namespace Diquis.Application.Common.Wrapper
{
    /// <summary>
    /// Represents a standard response wrapper for service operations.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Indicates whether the operation succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// A list of messages related to the response.
        /// </summary>
        public List<string> Messages { get; set; } = new();

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        public static Response Success()
        {
            return new Response { Succeeded = true };
        }

        /// <summary>
        /// Creates a failed response with no messages.
        /// </summary>
        public static Response Fail()
        {
            return new Response { Succeeded = false };
        }

        /// <summary>
        /// Creates a failed response with a single message.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public static Response Fail(string message)
        {
            return new Response { Succeeded = false, Messages = new List<string> { message } };
        }

        /// <summary>
        /// Creates a failed response with a list of messages.
        /// </summary>
        /// <param name="messages">The list of failure messages.</param>
        public static Response Fail(List<string> messages)
        {
            return new Response { Succeeded = false, Messages = messages };
        }
    }

    /// <summary>
    /// Represents a standard response wrapper with data for service operations.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the response.</typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// The data returned in the response.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Creates a successful response with no data.
        /// </summary>
        public static new Response<T> Success()
        {
            return new Response<T> { Succeeded = true };
        }

        /// <summary>
        /// Creates a successful response with data.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        public static Response<T> Success(T data)
        {
            return new Response<T> { Succeeded = true, Data = data };
        }

        /// <summary>
        /// Creates a failed response with no messages.
        /// </summary>
        public static new Response<T> Fail()
        {
            return new Response<T> { Succeeded = false };
        }

        /// <summary>
        /// Creates a failed response with a single message.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public static new Response<T> Fail(string message)
        {
            return new Response<T> { Succeeded = false, Messages = new List<string> { message } };
        }

        /// <summary>
        /// Creates a failed response with a list of messages.
        /// </summary>
        /// <param name="messages">The list of failure messages.</param>
        public static new Response<T> Fail(List<string> messages)
        {
            return new Response<T> { Succeeded = false, Messages = messages };
        }
    }
}



