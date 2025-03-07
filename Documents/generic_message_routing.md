
# Generic Message Routing

This is info about registering handlers for [CEF Generic Message Router](https://bitbucket.org/chromiumembedded/cef/wiki/GeneralUsage.md#markdown-header-generic-message-router)

CEF/CefGlue IPC provides a generic implementation for routing asynchronous messages between JavaScript running in the renderer process and .NET C# running in the browser process. These implementations are done internally by registering JavaScript functions on the current window browser. These functions are cefQuery and cefQueryCancel.

The generic message router functions in UI are in the following formats:

````javascript
 // Create and send a new query.
 var request_id = window.cefQuery({
        request: 'my_request',
        persistent: false,
        onSuccess: function(response) {},
        onFailure: function(error_code, error_message) {}
     });
 
// Optionally cancel the query.
window.cefQueryCancel(request_id)
````
For Chromatron CefGlue message routing, the following must be done:
- Use default message router handler or register a new one.
- Add appropriate cefQuery Javascript function in the UI.
- Add C# Controller/Action functionality to handle requests.

#### Register Message Router Handler
To use the [default handler](https://github.com/xplicit/Chromatron/blob/master/src/Chromatron/Browser/Handlers/DefaultMessageRouterHandler.cs) nothing needs to be done. 

For custom handlers, developers need to register one:

````csharp
    public class CustomChromatronApp : ChromatronBasicApp
    {
          public override void ConfigureServices(ServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IChromatronMessageRouter, CustomMessageRouter>();
        }
    }

    public class CustomMessageRouter : IChromatronMessageRouter
    {
    }
````


Chromatron default message route handling only implements cefQuery. To implement cefQueryCancel a new handler must be implemented and registered.

The default handler expects the request to be in the following format:
````javascript
var request = {
      "url": url
      "parameters": parameters,
      "postData": postData,
 };

Where:
   url: route path - "controller/routepath".
   parameters: Dictionary of parameters, where keys are in string, and values can be any primitive object.
   postData: Posted/input data object.
```` 

#### cefQuery Javascript function in the UI
A sample CefQuery GET request in the UI will be:

````javascript
function messageRouterResult(response) {
            var jsonData = JSON.parse(response);
            if (jsonData.ReadyState == 4 && jsonData.Status == 200) {
                   .... process response
            }
        }

 function messageRouterRun() {
            var request = {
                "url": "/democontroller/movies/get",
                "parameters": null,
                "postData": null,
            };
            window.cefQuery({
                request: JSON.stringify(request),
                onSuccess: function (response) {
                    messageRouterResult(response);
                }, onFailure: function (err, msg) {
                    console.log(err, msg);
                }
            });
        }
Where:
   On success callback function: messageRouterResult 
````

#### C# Controller/Action
A sample Controller
````charp
    public class MovieController : ChromatronController
    {
        public MovieController()
        {
        }
		
        [ChromatronRoute(Path = "/democontroller/movies/get")]
        public List<MovieInfo> GetMovies()
        {
		    return new List<MovieInfo>();
        }
	}
 
 Note that the route path: "/democontroller/movies/get" matches url in the UI.
````
