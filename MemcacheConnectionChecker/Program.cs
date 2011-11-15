// conceived by Tom Janssens on 2007/06/06  for codeproject
// http://www.corebvba.be
// released under license: http://www.codeproject.com/info/cpol10.aspx 
// modifed by james atherton on 20111115 for testing memcache

using System;
using System.Text.RegularExpressions;

namespace TelnetConnectionChecker
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args.Length != 2)
			{
				Console.WriteLine("Pass in args: <hostip|hostname> <port>");
				return;
			}

			string hostname = args[0];
			int port = int.Parse( args[1]);

			Console.WriteLine("Checking number of connections on: " + hostname + ", on port: " + port );

			var tc = new TelnetConnection(hostname, port);

			const string prompt = "stats\r\n";

			while (tc.IsConnected && prompt.Trim() != "exit")
			{	
				tc.WriteLine(prompt);
				var output = ParseOutput(tc.Read());	
				Console.WriteLine(output);
				System.Threading.Thread.Sleep(5000);
			}
			Console.WriteLine("***DISCONNECTED");
			Console.ReadLine();
		}

		private static string ParseOutput(string read)
		{
			var rx = new Regex(@"curr_connections\s+\d+");
			return rx.Match(read).ToString();
		}
	}
}
