namespace ListService.Contracts.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class AppList 
        {
            public const string List = Base + "/list";
            public const string Get = Base + "/list/{listId}";
            public const string Create = Base + "/list";
            public const string Update = Base + "/list/{listId}";
            public const string Delete = Base + "/list/{listId}";
        }
    }
}
