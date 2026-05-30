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

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private const double TableWidth = 372.0;
        private const double TableHeight = 392.0;

        private readonly Data.DataAbstractAPI dataLayer;

        private readonly Data.IBall ball;

        public Ball(Data.IBall ball, Data.DataAbstractAPI dataLayer)
        {
            this.ball = ball;
            this.dataLayer = dataLayer;
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

            if (dataPosition.x + velX > TableWidth)
            {
                velX = Math.Abs(velX) * -1;
            }
            else if (dataPosition.x + velX < 0)
            {
                velX = Math.Abs(velX);
            }

            if (dataPosition.y + velY > TableHeight)
            {
                velY = Math.Abs(velY) * -1;
            }
            else if (dataPosition.y + velY < 0)
            {
                velY = Math.Abs(velY);
            }

            ball.Velocity = dataLayer.CreateVector(velX, velY);

            NewPositionNotification?.Invoke(this, new Position(dataPosition.x, dataPosition.y));
        }

        #endregion private
    }
}