
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IGameManager
    {
        void JoinGame(string playerName);
        void Quit();
        void SetIdGenerator(Func<Guid> idGenerator);
        Task StartGameAsync();
    }
}
