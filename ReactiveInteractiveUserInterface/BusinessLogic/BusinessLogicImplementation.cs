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
            logger = new Logger();
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            logger.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            logger.Log($"Program started with {numberOfBalls} balls");

            layerBellow.Start(numberOfBalls, (startingPosition, databall)  => {

                Ball logicBall = new Ball(databall, layerBellow, logger);

                logicBall.NewPositionNotification += (_, _) => BallCollsion(databall);

                lock (BallsList)
                {
                    BallsList.Add(databall);
                }

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;
        private Logger logger;
        private List<Data.IBall> BallsList = new List<Data.IBall>();

        private readonly UnderneathLayerAPI layerBellow;

        private class BLVector : IVector
        {
            internal BLVector(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public double x { get; init; }
            public double y { get; init; }
        }

        private void BallCollsion(Data.IBall ball)
        {
            lock (BallsList)
            {

                var (p1, v1) = ball.getPositionAndVelocity();

            foreach (var databall in BallsList)
                {
                    if (ReferenceEquals(ball, databall))
                    {
                        continue;
                    }

                    var (p2, v2) = databall.getPositionAndVelocity();

                    if (AreBallsColliding(ball, databall))
                    {
                        var deltax = p1.x - p2.x;
                        var deltay = p1.y - p2.y;

                        var mas1 = (2 * databall.Mass / (ball.Mass + databall.Mass));
                        var mas2 = (2 * ball.Mass / (ball.Mass + databall.Mass));

                        var norm = deltay * deltay + deltax * deltax;

                        var dot = (v1.x - v2.x) * deltax + (v1.y - v2.y) * deltay;

                        if (dot >= 0) continue; // Don't change the velocity if the balls are already moving away from each other

                        var scale1 = mas1 * dot / norm;
                        var scale2 = mas2 * dot / norm;


                        ball.Velocity = new BLVector(v1.x - scale1 * deltax, v1.y - scale1 * deltay);
                        databall.Velocity = new BLVector(v2.x + scale2 * deltax, v2.y + scale2 * deltay);

                        logger.Log($"Collision - ball {ball.Id}, ball {databall.Id}. New velocities: ball1 ({Math.Round(ball.Velocity.x, 4)}, {Math.Round(ball.Velocity.y, 4)}), ball2 ({Math.Round(databall.Velocity.x, 4)}, {Math.Round(databall.Velocity.y, 4)})");
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