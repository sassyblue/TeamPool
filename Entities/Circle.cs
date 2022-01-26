using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamPool.Entities
{
    public class Circle
    {
        public Circle(float x, float y, int radius,
            GraphicsDevice graphics)
            : this(x, y, radius, Color.White, graphics) { }

        public Circle(float x, float y, int radius,
            Color color, GraphicsDevice graphics)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.color = color;
            this.graphics = graphics;

            Initialize();
        }

        public void Draw()
        {
            effect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives
                (PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
        }

        private void Initialize()
        {
            InitializeBasicEffect();
            InitializeVertices();
        }

        private void InitializeBasicEffect()
        {
            effect = new BasicEffect(graphics);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.Viewport.Width,
                 graphics.Viewport.Height, 0,
                 0, 1);
        }

        private void InitializeVertices()
        {
            vertices = new VertexPositionColor[CalculatePointCount()];
            var pointTheta = ((float)Math.PI * 2) / (vertices.Length - 1);
            for (int i = 0; i < vertices.Length; i++)
            {
                var theta = pointTheta * i;
                var x = X + ((float)Math.Sin(theta) * Radius);
                var y = Y + ((float)Math.Cos(theta) * Radius);
                vertices[i].Position = new Vector3(x, y, 0);
                vertices[i].Color = Color;
            }
            vertices[vertices.Length - 1] = vertices[0];
        }

        private int CalculatePointCount()
        {
            return (int)Math.Ceiling(Radius * Math.PI);
        }

        private GraphicsDevice graphics;
        
        private VertexPositionColor[] vertices;
        
        private BasicEffect effect;

        private float x;
        public float X
        {
            get { return x; }
            set { x = value; InitializeVertices(); }
        }
        private float y;
        public float Y
        {
            get { return y; }
            set { y = value; InitializeVertices(); }
        }
        private float radius;
        public float Radius
        {
            get { return radius; }
            set { radius = (value < 1) ? 1 : value; InitializeVertices(); }
        }
        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; InitializeVertices(); }
        }
     
        public int Points
        {
            get { return CalculatePointCount(); }
        }
    }
}