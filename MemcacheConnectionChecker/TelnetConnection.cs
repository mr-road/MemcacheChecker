// minimalistic telnet implementation
// conceived by Tom Janssens on 2007/06/06  for codeproject
// http://www.corebvba.be
// released under license: http://www.codeproject.com/info/cpol10.aspx 
// modifed by james atherton on 20111115 for testing memcache


using System.Text;
using System.Net.Sockets;

namespace TelnetConnectionChecker
{
	class TelnetConnection
	{
		readonly TcpClient tcpSocket;

		private const int TIME_OUT_MS = 100;

		public TelnetConnection(string Hostname, int Port)
		{
			tcpSocket = new TcpClient(Hostname, Port);
		}

		public void WriteLine(string cmd)
		{
			Write(cmd + "\n");
		}

		public void Write(string cmd)
		{
			if (!tcpSocket.Connected) return;
			byte[] buf = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
			tcpSocket.GetStream().Write(buf, 0, buf.Length);
		}

		public string Read()
		{
			if (!tcpSocket.Connected) return null;
			var sb = new StringBuilder();
			do
			{
				ParseTelnet(sb);
				System.Threading.Thread.Sleep(TIME_OUT_MS);
			} while (tcpSocket.Available > 0);
			return sb.ToString();
		}

		public bool IsConnected
		{
			get { return tcpSocket.Connected; }
		}

		void ParseTelnet(StringBuilder sb)
		{
			while (tcpSocket.Available > 0)
			{
				int input = tcpSocket.GetStream().ReadByte();
				switch (input)
				{
					case -1:
						break;
					case (int)Verbs.IAC:
						// interpret as command
						int inputverb = tcpSocket.GetStream().ReadByte();
						if (inputverb == -1) break;
						switch (inputverb)
						{
							case (int)Verbs.IAC:
								//literal IAC = 255 escaped, so append char 255 to string
								sb.Append(inputverb);
								break;
							case (int)Verbs.DO:
							case (int)Verbs.DONT:
							case (int)Verbs.WILL:
							case (int)Verbs.WONT:
								// reply to all commands with "WONT", unless it is SGA (suppres go ahead)
								int inputoption = tcpSocket.GetStream().ReadByte();
								if (inputoption == -1) break;
								tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
								if (inputoption == (int)Options.SGA)
									tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
								else
									tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
								tcpSocket.GetStream().WriteByte((byte)inputoption);
								break;
						}
						break;
					default:
						sb.Append((char)input);
						break;
				}
			}
		}
	}
}
