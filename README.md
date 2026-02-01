This project follows Code first approach, you need to change in appsetting.json for connection string.

dotnet ef migrations add UpdateCustomerAndRemoveOldLogs --project Blog.Infrastructure --startup-project Blog.WebAPI

dotnet ef database update --project Blog.Infrastructure --startup-project Blog.WebAPI
