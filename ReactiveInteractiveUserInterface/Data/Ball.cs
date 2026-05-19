//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Runtime.CompilerServices;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            _diameter = 10;
            _mass = 1;

        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity
        {
            get
            {
                lock (_lock)
                {
                    return _velocity;
                }
            }
            set
            {
                lock (_lock)
                {
                    _velocity = new Vector(value.x, value.y);
                }
            }
        }
        public IVector Position
        {
            get
            {
                lock (_lock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_lock)
                {
                    _position = new Vector(value.x, value.y);
                }
            }
        }

        public (IVector Position, IVector Velocity) getPositionAndVelocity()
        { 
            {
                lock (_lock)
                {
                    return (_position, _velocity);
                }
            }
        }

        public double Diameter
        {
            get
            {
                lock (_lock)
                {
                    return _diameter;
                }
            }
        }

        public double Mass
        {
            get
            {
                lock (_lock)
                {
                    return _mass;
                }
            }
        }

        #endregion IBall

        public void Move()
        {
            lock (_lock)
            {
                _position = new Vector(_position.x + _velocity.x, _position.y + _velocity.y);
            }
            RaiseNewPositionChangeNotification();

        }

        internal void Stop() 
        {
        }

        #region private

        private Vector _position;
        private Vector _velocity;
        private double _diameter;
        private double _mass;
        private readonly object _lock = new object();
        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, _position);
        }



        #endregion private
    }
}