This project follows Code first approach

dotnet ef migrations add UpdateCustomerAndRemoveOldLogs --project Blog.Infrastructure --startup-project Blog.WebAPI

dotnet ef database update --project Blog.Infrastructure --startup-project Blog.WebAPI
