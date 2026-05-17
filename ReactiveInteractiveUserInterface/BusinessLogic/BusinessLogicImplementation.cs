//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall)  => {

                Ball logicBall = new Ball(databall, layerBellow);

                logicBall.NewPositionNotification += (_, _) => BallCollsion(databall);

                lock (BallsList)
                {
                    BallsList.Add( databall);
                }

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;
        private List<Data.IBall> BallsList = new List<Data.IBall>();

        private readonly UnderneathLayerAPI layerBellow;

        private void BallCollsion(Data.IBall ball)
        {
            var (p1, v1) = ball.getPositionAndVelocity();

            foreach (var databall in BallsList)
                {
                lock (BallsList) {
                    if (ReferenceEquals(ball, databall))
                    {
                        continue;
                    }

                    var (p2, v2) = databall.getPositionAndVelocity();

                    if (AreBallsColliding(ball, databall))
                    {
                        Debug.Print("Colision detected between balls detected");
                    }
                }
            }

        }

        private bool AreBallsColliding(Data.IBall ball1, Data.IBall ball2)
        {
            double dx = ball1.Position.x - ball2.Position.x;
            double dy = ball1.Position.y - ball2.Position.y;
            double distanceSquared = dx * dx + dy * dy;
            double radiusSum = ball1.Diameter + ball2.Diameter;

            return distanceSquared <= radiusSum * radiusSum;
        }

        #endregion private



        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}