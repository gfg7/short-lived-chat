namespace SimpleLiveChat.Middlewares
{
    public class RefreshUserChatList
    {
        private RequestDelegate _request;

        public RefreshUserChatList(RequestDelegate request)
        {
            _request = request;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _request.Invoke(context);
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
            }
            finally
            {
            
            }
        }
    }
}