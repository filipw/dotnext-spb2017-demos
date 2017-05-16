using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            var hörgerät = new Hörgerät();
            hörgerät.Verstärker = "SX-456";
        }

        private async Task Foo()
        {
            await Bar();
        }

        private Task Bar()
        {
            return Task.FromResult(0);
        }
    }
}