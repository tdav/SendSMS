using SMS_Gate.Model;
using System.Threading.Tasks;

namespace SMS_Gate
{
    public interface ISender
    {
        DevInfo Info();
        void Run();
    }
}