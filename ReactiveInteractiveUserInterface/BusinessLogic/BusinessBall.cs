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

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private const double TableWidth = 372.0;
        private const double TableHeight = 392.0;

        private Logger _logger;

        private readonly Data.DataAbstractAPI dataLayer;

        private readonly Data.IBall ball;

        public Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer, Logger logger)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
            this._logger = logger;
            _logger.Log($"New Ball - ID {ball.Id}, position ({Math.Round(ball.Position.x, 4)}, {Math.Round(ball.Position.y, 4)}), velocity ({Math.Round(ball.Velocity.x, 4)}, {Math.Round(ball.Velocity.y, 4)})");
            ball.NewPositionNotification += RaisePositionChangeEvent; 
            thread = new Thread(Run);
            thread.Start();
            _running = true;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private bool _running;
        private readonly Thread thread;

        private void Run()
        {
            while (_running)
            {
                
                Stopwatch sw = Stopwatch.StartNew();
                ball.Move();
                sw.Stop();

                int time = 1000 / 60 - (int)sw.ElapsedMilliseconds;
                if (time > 0) 
                    Thread.Sleep(time);

            }
        }

        internal void Stop() {
            _running = false;
        }


        private void RaisePositionChangeEvent(object? sender, Data.IVector dataPosition)
        {
            double velX = (ball.Velocity.x);
            double velY = (ball.Velocity.y);

            bool bounced = false;

            if (dataPosition.x + velX > TableWidth)
            {
                velX = Math.Abs(velX) * -1;
                bounced = true;
            }
            else if (dataPosition.x + velX < 0)
            {
                velX = Math.Abs(velX);
                bounced = true;
            }

            if (dataPosition.y + velY > TableHeight)
            {
                velY = Math.Abs(velY) * -1;
                bounced = true;
            }
            else if (dataPosition.y + velY < 0)
            {
                velY = Math.Abs(velY);
                bounced = true;
            }

            if (bounced) {
                _logger.Log($"Bounce - {ball.Id} position ({Math.Round(dataPosition.x, 4)}, {Math.Round(dataPosition.y, 4)}), new velocity ({Math.Round(velX, 4)}, {Math.Round(velY, 4)})");
            }

            ball.Velocity = dataLayer.CreateVector(velX, velY);

            NewPositionNotification?.Invoke(this, new Position(dataPosition.x, dataPosition.y));
        }

        #endregion private
    }
}