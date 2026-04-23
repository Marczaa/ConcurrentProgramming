//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

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
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

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