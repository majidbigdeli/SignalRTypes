# SignalRTypes

Based on [NJsonSchema](http://njsonschema.org) (see also: [NSwag](http://nswag.org)).



## Sample

Please See Example Directory 


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


** You can also return your model without use generic Hub by implement ISignalRTypesBuilder **

```

            services.AddSignalRTypes<ISignalRTypesBuilder>();
```

# Result

return is json in open api 3

```json
{
"SignalrType": "1.0.0",
"info": {
"title": "SignalrType specification",
"termsOfService": null,
"version": "1.0.0"
},
"hubs": {
"ChatHub": {
"name": "Chat",
"operations": {
"StartWorkAsync": {
"parameters": {
"message": {
"$ref": "#/definitions/StartWorkVm"
}
}
},
"StopWork": {
"parameters": {
"message": {
"$ref": "#/definitions/StopWorkVm"
}
}
},
"StopWork2": {
"parameters": {
"message": {
"type": "string"
}
}
}
},
"callbacks": {
"hello": {
"parameters": {}
},
"startwork": {
"parameters": {
"message": {
"$ref": "#/definitions/StartWorkVm"
}
}
},
"stopwork": {
"parameters": {
"message": {
"$ref": "#/definitions/StopWorkVm"
}
}
}
}
}
},
"definitions": {
"StartWorkVm": {
"type": "object",
"additionalProperties": false,
"required": [
"jobType",
"firstName",
"lastName"
],
"properties": {
"jobType": {
"$ref": "#/definitions/JobType"
},
"firstName": {
"type": "string",
"maxLength": 100,
"minLength": 1
},
"lastName": {
"type": "string",
"maxLength": 200,
"minLength": 1
},
"birthDate": {
"type": "string",
"format": "date-time"
}
}
},
"JobType": {
"type": "string",
"x-enumNames": [
"Programer",
"Manager"
],
"enum": [
"Programer",
"Manager"
]
},
"StopWorkVm": {
"type": "object",
"additionalProperties": false,
"properties": {
"date": {
"type": "string",
"format": "date-time"
},
"description": {
"type": "string",
"x-nullable": true
}
}
}
}
}
```
