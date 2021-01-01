# SignalRTypes

Based on [NJsonSchema](http://njsonschema.org) (see also: [NSwag](http://nswag.org)).

##Please See Example




## Sample

```csharp
    public class ChatHub : AppHubBase<IChatClient> , IHuB
    {
        public async Task StartWorkAsync(StartWorkVm message)
        {
            await Clients.All.StartWorkAsync(message);
        }
        public async Task StopWork(StopWorkVm message)
        {
            await Clients.All.StopWorkAsync(message);
        }

        public async Task StopWork2(string message)
        {
            await Clients.All.StopWorkAsync(new StopWorkVm());
        }
    }


    public class StartWorkVm
    {

        [Required]
        public JobType JobType { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(200)]
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

    }
    
    
    public class StopWorkVm
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    
     public interface IChatClient
    {
        [HubMethodName("hello")]
        Task Hello();

        [HubMethodName("startwork")]
        Task StartWorkAsync(StartWorkVm message);

        [HubMethodName("stopwork")]
        Task StopWorkAsync(StopWorkVm message);

    }
    
    public enum JobType
    {
        Programer = 1,
        Manager,

    }


```

#StartUp

in ConfigureServices

```csharp

            services.AddSignalRTypes();

```

in Configure

```csharp

            app.UseSignalrType(x =>
            {
                x.RoutePath = "/api/signalRTypes/signalrType.json";
            });

```

