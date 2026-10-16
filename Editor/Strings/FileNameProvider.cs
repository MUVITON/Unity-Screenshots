using System;


namespace Screenshots.Editor
{

    public static class FileNameProvider
    {
        public static string Get()
        {
            return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

    }

}
