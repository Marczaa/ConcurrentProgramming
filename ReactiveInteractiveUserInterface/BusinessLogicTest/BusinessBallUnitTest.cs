//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Data.DataAbstractAPI dataLayer = Data.DataAbstractAPI.GetDataLayer();
            Ball newInstance = new(dataBallFixture, dataLayer);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        [TestMethod]
        public void BallsCollisionTestMethod()
        {
            DataBallFixture dataBallFixture1 = new DataBallFixture() { Position = new VectorFixture(0.0, 0.0), Velocity = new VectorFixture(1.0, 0.0) };
            DataBallFixture dataBallFixture2 = new DataBallFixture() { Position = new VectorFixture(10.0, 0.0), Velocity = new VectorFixture(-1.0, 0.0) };

            DataLayerFixture dataLayer = new DataLayerFixture(dataBallFixture1, dataBallFixture2);

            BusinessLogicImplementation businessLogic = new BusinessLogicImplementation(dataLayer);

            businessLogic.Start(2, (position, ball) => { });

            dataBallFixture1.Move();
            dataBallFixture2.Move();

            Assert.IsTrue(dataBallFixture1.Velocity.x == -1.0);
            Assert.IsTrue(dataBallFixture2.Velocity.x == 1.0);

        }

        [TestMethod]
        public void BallsMovingSeparatelyTestMethod()
        {
            DataBallFixture dataBallFixture1 = new DataBallFixture() { Position = new VectorFixture(0.0, 30.0), Velocity = new VectorFixture(1.0, 0.0) };
            DataBallFixture dataBallFixture2 = new DataBallFixture() { Position = new VectorFixture(10.0, 0.0), Velocity = new VectorFixture(-1.0, 0.0) };

            DataLayerFixture dataLayer = new DataLayerFixture(dataBallFixture1, dataBallFixture2);

            BusinessLogicImplementation businessLogic = new BusinessLogicImplementation(dataLayer);

            businessLogic.Start(2, (position, ball) => { });

            dataBallFixture1.Move();
            dataBallFixture2.Move();

            Assert.IsTrue(dataBallFixture1.Velocity.x == 1.0);
            Assert.IsTrue(dataBallFixture2.Velocity.x == -1.0);

        }

        [TestMethod]
        public void BallsWallCollisionTestMethod()
        {
            DataBallFixture dataBallFixture1 = new DataBallFixture() { Position = new VectorFixture(372.0, 30.0), Velocity = new VectorFixture(1.0, 0.0) };
            DataBallFixture dataBallFixture2 = new DataBallFixture() { Position = new VectorFixture(0.0, 0.0), Velocity = new VectorFixture(-1.0, 0.0) };

            DataLayerFixture dataLayer = new DataLayerFixture(dataBallFixture1, dataBallFixture2);

            BusinessLogicImplementation businessLogic = new BusinessLogicImplementation(dataLayer);

            businessLogic.Start(2, (position, ball) => { });

            dataBallFixture1.Move();
            dataBallFixture2.Move();

            Assert.IsTrue(dataBallFixture1.Velocity.x == -1.0);
            Assert.IsTrue(dataBallFixture2.Velocity.x == 1.0);

        }

        #region testing instrumentation

        private class DataLayerFixture : Data.DataAbstractAPI
        {
            private readonly Data.IBall ball1;
            private readonly Data.IBall ball2;
            internal DataLayerFixture(Data.IBall ball1, Data.IBall ball2)
            {
                this.ball1 = ball1;
                this.ball2 = ball2;
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                upperLayerHandler(ball1.Position, ball1);
                upperLayerHandler(ball2.Position, ball2);
            }
            public override IVector CreateVector(double x, double y)
            {
                return new VectorFixture(x, y);
            }

            public override void Dispose()
            {
            }
        }

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get; set; } = new VectorFixture(0.0, 0.0);

            public double Diameter { get; init; } = 10.0;
            public double Mass { get; init; } = 1.0;

            public event EventHandler<Data.IVector>? NewPositionNotification;

            internal void Move()
            {

                Position = new VectorFixture(Position.x + Velocity.x, Position.y + Velocity.y);
                NewPositionNotification?.Invoke(this, Position);
            }


            public IVector Position { get; set; } = new VectorFixture(0.0, 0.0);


            public (IVector Position, IVector Velocity) getPositionAndVelocity() { return (Position, Velocity); }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}