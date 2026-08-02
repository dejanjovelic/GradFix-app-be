namespace GradFix_app_be.Services.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message):base(message)
        {
        }
    }
}
