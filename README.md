# SelectelSharpCore
### Selectel Cloud Storage .NET Core SDK (.NETStandart 1.3)

SelectelSharp is .net SDK for Selectel Cloud Storage written on C# in Async style.
At this moment most of API methods are implemented, but some of them are still in development.

For more information see: 
[Selectel.com](https://selectel.com/)

Nuget: https://www.nuget.org/packages/SelectelSharpCore/0.8.0

```cs
PM> Install-Package SelectelSharpCore
```


## Basic usage

**Create client**

Everything is starts with SelectelClient initialization:

```cs
var client = new SelectelClient();
```

If you working behind a network Proxy, you should pass proxy parameters to the constructor, something like that:

```cs
var client = new SelectelClient("myproxy.com:8080", "domain\\whoami", "pa$$w0rd");
```

**Authorize it**

Almost every method in API needs _token_ to perform I/O operations under your storage.
You should call AuthorizeAsync method to obtain it. Pass your client id and storage password to this method.

```cs
await client.AuthorizeAsync("userId", "userKey");
```
If authorization was successful, client will recieve authrization token. In other case it will throw WebException.

**Call API methods**

Now you could could Api methods, for example:
```cs
var result = await client.CreateContainerAsync("new-container");
```

## Implemented api methods
**Storage methods**
* GetStorageInfoAsync

**Container methods**
* GetContainersListAsync
* GetContainerInfoAsync
* GetContainerFilesAsync
* CreateContainerAsync
* SetContainerMetaAsync
* SetContainerToGalleryAsync
* DeleteContainerAsync

**File methods**
* GetFileAsync
* UploadFileAsync
* SetFileMetaAsync
* DeleteFileAsync

**Archive methods**
* UploadArchiveAsync

_More methods coming soon..._
