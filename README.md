# Code

## Target .Net Framework 4.6.1

## File
- [controller/HomeController.cs::QueryOrder](demo/Controller/HomeController.cs#L41)
- [controller/HomeController.cs::RuntTimeExec](demo/Controller/HomeController.cs#L99)
- [controller/HomeController.cs::insiderHandler](demo/Controller/HomeController.cs#L166)

## Visual Studio Solution (Including libraries)

[Dropbox Link](https://www.dropbox.com/s/x390t6hhbfuc2yb/Demo.zip?dl=0)



# Demo
## Register / Login

<img src="Video/reg.gif" width=666 />

## SQL Injection Attack
 
<img src="Video/SqlInjection.gif" width=666 />

## Insider Handler

<img src="Video/InsiderHandler.gif" width=666 />

## Command Injection

<img src="Video/CommandInjection.gif" width=666 />


### Hard Coded credentials:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L107):
    ```java
    [HttpGet("vulns")]
    public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
        String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
        String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";
    ```
    
### Sensitive data leak:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L114):
    ```java
    [HttpGet("vulns")]
    public ActionResult DataLeakage(string login, string password, string encodedPath, string entityDocument) {
        String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
        String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";
            
        Console.WriteLine(" AWS Properties are " + ACCESS_KEY_ID + " and " + SECRET_KEY);
        Console.WriteLine(" Transactions Folder is " + txns_dir);
    ```
    
### Insider attack:
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/HomeController.cs#L105):
1.  Time Bomb pattern
    ```java
    // RECIPE: Time Bomb pattern
    String command = "c2ggL3RtcC9zaGVsbGNvZGUuc2g=";
    ticking(command);
    ```
    
2. Magic Value leading to command injection
    ```java
    if (tracefn == "C4A938B6FE01E") {
        runtTimeExec(cmd);
    }
    ```

3. // RECIPE: Compiler Abuse Pattern
    ```java
    /*  string code = @"
        using System;
        namespace First
        {
            public class Program
            {
                public static void Main()
                {
                    Console.WriteLine('HAHAHA');
                }
            }
        }
    "; */

    String base64Str = "dXNpbmcgU3lzdGVtOwpuYW1lc3BhY2UgRmlyc3QKewogICBwdWJsaWMgY2xhc3MgUHJvZ3JhbSB7CiAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKCkgewogICAgICAgICAgICAgQ29uc29sZS5Xcml0ZUxpbmUoIkhBSEFIQSIpOwogICAgICB9CiAgIH0KfQ==";
    ```
    
    ```java
    // Compile source file.
    CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    ```
   
    ```java
    // Load and instantiate compiled class.
    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
    var type = assembly.GetType("First.Program");
    MethodInfo main = type.GetMethod("Main");
    main.Invoke(null, null);
    ```

### Business logic flaw
- [Code Snippet](https://gitlab.com/irobert0126/aspdotnet_csharp_vuln_demo/tree/master/SqlInjection/Controllers/OrderController.cs#L94):
    ```java
    [HttpPost("AddNewOrder")]
    public ActionResult AddNewOrderFlaw(string reader) {
        Order order = JsonConvert.DeserializeObject<Order>(reader);
        return Content(order.ToString());
    }
    ```
- Command-Line Demo:
    ```sh
    $ curl -X POST -d "reader={\"Id\":0,\"AspNetUserId\":1,\"LastName\":\"Xu\",\"FirstName\":\"Zhaoyan\",\"FullName\":\"Luke\'' Father\",\"AspNetUser\":null}" http://localhost:5005/AddNewOrder
    
      SqlInjection.Models.Order 
    ```
