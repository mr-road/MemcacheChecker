// minimalistic telnet implementation
// conceived by Tom Janssens on 2007/06/06  for codeproject
// http://www.corebvba.be
// split out by james atherton on 20111115

namespace TelnetConnectionChecker
{
	enum Verbs
	{
		WILL = 251,
		WONT = 252,
		DO = 253,
		DONT = 254,
		IAC = 255
	}
}