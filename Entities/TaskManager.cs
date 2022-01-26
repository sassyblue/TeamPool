using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Design;

namespace TeamPool.Entities
{
    public class TaskManager
    {
        public List<Ball> _ballsArr;
        public Team[] _teamArr;
        public int _ballsNum;
        private Task[] _taskArr;
        private object _locker;
        
        public TaskManager(GraphicsDevice graphicsDevice, int ballsNum, int teamNum)
        {
            _ballsNum = ballsNum;
            _ballsArr = new List<Ball>();
            _teamArr = new Team[teamNum];

            // init teams
            int ballIndex = 0;
            
            for (int j=0; j<teamNum; j++)
            {
                _teamArr[j] = new Team(getRandColor());
                
                // Init balls and add them to team
                for (int i=0; i < ballsNum/teamNum; i++)
                {
                    var radius = new Random().Next(5, 20);
                    _ballsArr.Add(new Ball(
                        graphicsDevice, 
                        _teamArr[j].Color(),
                        radius
                    ));
                
                    _ballsArr[ballIndex].setPos( GetRandPosition() );
                    _ballsArr[ballIndex].setVelocity( GetRandVector(1, 2, 1, 2) );
                    // add ball to it's team
                    _teamArr[j].addMember( _ballsArr[ballIndex]._id );
                    _ballsArr[ballIndex].setTeamId(_teamArr[j]._teamId);
                    ballIndex++;
                }
                
            }
        }

        private Vector2 GetRandVector(int minX, int maxX, int minY, int maxY)
        {
            Random rnd = new Random();
            int x = rnd.Next(minX, maxX);
            int y = rnd.Next(minY, maxY);
            return new Vector2(x, y);
        }
        private Vector2 GetRandPosition()
        {
            return GetRandVector(10, 1000, 10, 500);
        }

        private Color getRandColor()
        {
            int r = new Random().Next(0, 255);
            int g = new Random().Next(0, 255);
            int b = new Random().Next(0, 255);
            return new Color(r, g, b);
        }
        
        public void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            // remove all dead groups
            _ballsArr.RemoveAll(ball => ball.isDead);
            
            Parallel.ForEach(_ballsArr, (ball) => ball.Update(delta));
            
            // sweep and prune UwU
            
            // first sort by X position
            _ballsArr.Sort((a, b) => {
                int result = b._pos.X.CompareTo(a._pos.X);
                return result;
            });

            List<Vector2> collides = new List<Vector2>();

            for (int j = 0; j < _ballsArr.Count; j++)
            {
                Ball activeBall = _ballsArr[j];
                Vector2 thePos = activeBall.getPos();

                for (int l = j + 1; l < _ballsArr.Count; l++)
                {
                    Ball other = _ballsArr[l];
                    if (
                        thePos.X+activeBall._radius > other._pos.X-other._radius
                        && thePos.X-activeBall._radius < other._pos.X+other._radius
                    ) {
                        collides.Add(new Vector2(j, l));            
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (Vector2 coll in collides)
            {
                int i1 = (int)coll.X;
                int i2 = (int)coll.Y;
                if (_ballsArr[i1].isColliding(_ballsArr[i2]))
                {
                    // logic on ball collision
                    collide(_ballsArr[i1], _ballsArr[i2]);
                    _ballsArr[i1].calcLifePoints(_ballsArr[i2]);
                }
                // Console.WriteLine("Obj1: {0} Obj2: {1}", coll.X, coll.Y);
            }
        }
        
        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (Ball ball in _ballsArr)
            {
                if (!ball.isDead)
                {
                    ball.Draw(_spriteBatch);
                }
            }
        }

        private void collide(Ball ball1, Ball ball2)
        {
            // normal vector
            Vector2 nVec = new Vector2(
                ball1._pos.X - ball2._pos.X,
                ball1._pos.Y - ball2._pos.Y
            );
            
            // wektor jednostkowy
            float uX = (float)(nVec.X / Math.Sqrt(nVec.X * nVec.X + nVec.Y * nVec.Y));
            float uY = (float)(nVec.Y / Math.Sqrt(nVec.X * nVec.X + nVec.Y * nVec.Y));
            Vector2 unitNormal = Vector2.Normalize(new Vector2(uX, uY));
            
            // wektor jednostkowy stycznej
            Vector2 unitTangen = Vector2.Normalize(
                new Vector2(-unitNormal.Y, -unitNormal.X)
            );

            // tutaj powinny być konkretne wartości
            Single unitSpeed1 = Vector2.Dot(unitNormal, ball1._direction);
            Single unitTan1 = Vector2.Dot(unitTangen, ball1._direction);
            
            Single unitSpeed2 = Vector2.Dot(unitNormal, ball2._direction);
            Single unitTan2 = Vector2.Dot(unitTangen, ball2._direction);

            int mass1 = ball1._mass;
            int mass2 = ball2._mass;
            int massSum = mass1 + mass2;
            
            // tutaj wciąż jesteśmy w jednym wymiarze,
            // bo nie uwzględniliśmy jeszcze stycznej
            // zaś wynikiem są nowe skalary prędkości
            
            Single tempNewDir1 = (unitSpeed1*(mass1 - mass2) + 2*mass2*unitSpeed2) / massSum;
            Single tempNewDir2 = (unitSpeed2*(mass2 - mass1) + 2*mass1*unitSpeed1) / massSum;
            
            //Single tempNewDir1 = unitSpeed2;
            //Single tempNewDir2 = unitSpeed1;
            
            // konwertujemy normalne z powrotem na wektory
            Vector2 newNormal1 = Vector2.Multiply(unitNormal, tempNewDir1);
            Vector2 newNormal2 = Vector2.Multiply(unitNormal, tempNewDir2);
            
            // wektory stycznych się nie zmieniają,
            // więc zostaje unitTangen
            
            // liczymy wynik
            Vector2 newVector1 = Vector2.Add(newNormal1, unitTangen);
            Vector2 newVector2 = Vector2.Add(newNormal2, unitTangen);

            ball1.setVelocity(newVector1);
            ball2.setVelocity(newVector2);
        }
    }
}