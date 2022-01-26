using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TeamPool.Entities
{
    public class Ball
    {
        public string _id;
        private string _teamId;
        Texture2D _rect;
        public int _radius;
        public Vector2 _pos;
        public Vector2 _direction;
        public int _mass;
        private Color _color;
        public int lifeNum; // Ball life ponits
        private Circle _circle;
        public bool isDead = false;

        public Ball(GraphicsDevice graphicsDevice, Color color, int radius)
        {
            _id = getUUID();
            _color = color;
            _radius = radius;
            _mass = radius;
            lifeNum = (int)(radius/2);
            _rect = loadTexture("/home/skrzyniu/RiderProjects/TeamPool/Content/circle_blue.png", graphicsDevice);
            /*
            _rect = new Texture2D(graphicsDevice, radius*2, radius*2);
            */
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            int x = (int) _pos.X;
            int y = (int) _pos.Y;
            
            spriteBatch.Draw(_rect, 
                destinationRectangle: new Rectangle(x, y, _radius*2, _radius*2),
                color:_color);
        }

        public void Update(double delta)
        {
            _pos.X += _direction.X;
            _pos.Y += _direction.Y;
            HandleBorderCollision();
        }

        private void HandleBorderCollision()
        {
            int sWidth = 1000;
            int sHeight = 500;
            
            if (_pos.X + 2*_radius > sWidth || _pos.X < 0)
            {
                _direction.X = -1 * _direction.X;
            }

            if (_pos.Y + 2*_radius > sHeight || _pos.Y < 0)
            {
                _direction.Y = -1 * _direction.Y;
            }
        }
        
        public Vector2 getPos()
        {
            return _pos;
        }

        public void setPos(Vector2 newPos)
        {
            _pos = newPos;
        }

        public Vector2 getRealPosition()
        {
            return new Vector2(_pos.X - _radius, _pos.Y - _radius);
        }

        public bool isColliding(Ball otherBall)
        {
            Vector2 otherPos = otherBall.getPos();
            int dX = Math.Abs((int) (otherPos.X - this._pos.X));
            int dY = Math.Abs((int) (otherPos.Y - this._pos.Y));
            double distance = Math.Sqrt(dX * dX + dY * dY);
            if (distance < _radius*2) // only because each ball has same radius
            {
                return true;
            }
            return false;
        }

        public void setVelocity(Vector2 newVelo)
        {
            this._direction = newVelo;
        }
        
        public void stopBall()
        {
            this._direction = new Vector2(0, 0);
        }

        private string getUUID()
        {
            return Guid.NewGuid().ToString();
        }

        public void calcLifePoints(Ball opponent)
        {
            // jeśli jest ta sama drużyna - to punkty się nie zmieniają
            if (_teamId != opponent.getTeamId())
            {
                if (lifeNum > opponent.lifeNum)
                {
                    opponent.takeLifePoint();
                }
                else if(lifeNum < opponent.lifeNum)
                {
                    takeLifePoint();
                }
            }
        }

        public string getTeamId()
        {
            return _teamId;
        }

        public void setTeamId(string id)
        {
            _teamId = id;
        }

        public void takeLifePoint()
        {
            if (lifeNum == 1)
            {
                lifeNum = 0;
                isDead = true;
            }
            else
            {
                lifeNum--;
            }
        }
        
        private Texture2D loadTexture(String FilePath, GraphicsDevice device)
        {
            Texture2D texture;

            FileStream titleStream = File.OpenRead(FilePath);
            texture = Texture2D.FromStream(device, titleStream);
            titleStream.Close();
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            texture.SetData(buffer);

            return texture;
        }
    }
}