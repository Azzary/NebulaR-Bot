using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.World;
using Nebular.Bot.Game.World.PathFinder;
using Nebular.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Extension
{
    public class MovementExtension : IDisposable
    {
        private Account account;
        private Character character;
        private Map map;

        public event Action<bool> MovementFinished;
        private bool disposed;

        public MovementExtension(Account _account, Map _map, Character _character)
        {
            account = _account;
            character = _character;
            map = _map;
        }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void TryCancelToken()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task MovementFinishedEvent(string packet)
        {
            TryCancelToken();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            await semaphore.WaitAsync();

            try
            {
                await Task.Delay(1, cancellationToken);
                var PlayerCell = character.IsFighting()? character.Fight.PlayerFighter.Cell : character.Cell;
                List<Cell> actualPath = PathFinderUtil.getPathFromStrPath(packet, PlayerCell.Id, map);
                if (actualPath == null)
                    return;
                if (account.Character.IsFighting())
                    await Task.Delay(400 + (100 * character.Cell.GetDistance(actualPath[actualPath.Count - 1])), cancellationToken);
                else
                {
                    bool isFight = account.Character.IsFighting();
                    account.Character.CharacterState = CharacterState.MOVING;
                    foreach (var item in actualPath)
                    {
                        await Task.Delay(PathFinderUtil.GetCellTravelTime(character.Cell, item, actualPath.Count, character.UsingDrago), cancellationToken);
                        map.UpdateInterface();
                        character.Cell = item;
                    }
                    if (character.Cell.IsTeleport())
                    {
                        account.Character.CharacterState = CharacterState.CONNECTED_IDLE;
                    }
                }

                character.Cell = actualPath[actualPath.Count - 1];
                if (account.Character.CharacterState == CharacterState.MOVING)
                    account.Character.CharacterState = account.Character.IsFighting() ? CharacterState.FIGHTING : CharacterState.CONNECTED_IDLE;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void MovementUpdated(bool state) => MovementFinished?.Invoke(state);

        #region Dispose Region
        ~MovementExtension() => Dispose(false);
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                account = null;
                character = null;
                disposed = true;
            }
        }
        #endregion
    }
}
